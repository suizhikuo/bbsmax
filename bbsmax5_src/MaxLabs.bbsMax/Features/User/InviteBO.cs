//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;

using MaxLabs.WebEngine;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.RegExp;
using MaxLabs.bbsMax.Email;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.PointActions;

namespace MaxLabs.bbsMax
{
    public class InviteBO : BOBase<InviteBO>
    {
        //public void UpdateInviteSerialEmailAndStatus(int userID, InviteSerialCollection serials)
        //{
        //    InviteDao.Instance.UpdateInviteSerialEmailAndStatus(userID, serials);
        //}

        public bool SendInviteByEmail(AuthUser operatorUser, Guid serial, string email)
        {
            if (AllSettings.Current.EmailSettings.EnableSendEmail == false)
            {
                ThrowError(new EmailDisabledError());
                return false;
            }

            if (string.IsNullOrEmpty(email))
                ThrowError(new EmptyEmailError("email"));

            else if (!ValidateUtil.IsEmail(email) == false)
            {
                ThrowError(new EmailFormatError("email", email));
            }

            InviteSerial inviteSerial = this.GetInviteSerial(serial);

            if (inviteSerial == null
                || inviteSerial.Status == InviteSerialStatus.Expires
                || inviteSerial.Status == InviteSerialStatus.Used
                || inviteSerial.UserID != operatorUser.UserID
                )
            {
                ThrowError(new InviteSerialError("serial", serial.ToString()));
            }

            if (HasUnCatchedError)
                return false;

            InviteEmail emailSender = new InviteEmail(
                email
                , inviteSerial.Serial
                , operatorUser.Username
                , operatorUser.UserID);
                emailSender.Send();


            if (!HasUnCatchedError)
            {
                inviteSerial.ToEmail = email;
                inviteSerial.Status = InviteSerialStatus.Unused;

                InviteSerialCollection serials = new InviteSerialCollection();
                serials.Add(inviteSerial);

                InviteDao.Instance.UpdateInviteSerialEmailAndStatus(operatorUser.UserID, serials);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 批量添加邀请码
        /// </summary>
        /// <param name="operatorUserID">操作者ID</param>
        /// <param name="userIDs">目标用户ID</param>
        /// <param name="addNum">添加数量</param>
        /// <param name="ignoreRights">是否忽略权限判断</param>
        private void CreateInviteSerials(AuthUser operatorUser, IEnumerable<int> userIDs, int addNum, bool ignoreRights)
        {
            if (!ignoreRights)
            {
                if (operatorUser == User.Guest)
                {
                    ThrowError(new NotLoginError());
                    return;
                }
                else
                {
                    if (!CanAddInviteSerial(operatorUser))
                    {
                        ThrowError(new NoPermissionManageInviteSerialError());
                        return;
                    }
                }
            }

            DateTime Expires;
            int EffectiveHours = AllSettings.Current.InvitationSettings.InviteEffectiveHours;

            if (EffectiveHours <= 0)
                Expires = DateTime.MaxValue;
            else
                Expires = DateTimeUtil.Now.AddHours(EffectiveHours);

            if (addNum <= 0)
            {
                ThrowError(new InviteSerialCountFormatError("addNum", ""));
                return;
            }

            if (!ValidateUtil.HasItems<int>(userIDs))
            {
                //TODO 没有选择用户
                return;
            }

            string remark;
            if (operatorUser != null)
                remark = string.Format("由管理员{0}添加", operatorUser.Username);
            else
                remark = "由系统自动添加(比如任务奖励)";

            InviteDao.Instance.CreateInviteSerials(userIDs, addNum, Expires,remark);
        }

        /// <summary>
        /// 给指定的用户生成指定数量的邀请码（不检查操作者权限）
        /// </summary>
        /// <param name="userIDs"></param>
        /// <param name="addNum"></param>
        public void CreateInviteSerials(IEnumerable<int> userIDs, int addNum)
        {
            CreateInviteSerials(null, userIDs, addNum, true);
        }

        /// <summary>
        /// 给指定的用户生成指定数量的邀请码（会检查操作者是否有足够权限）
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <param name="userIDs"></param>
        /// <param name="addNum"></param>
        public void CreateInviteSerials(AuthUser operatorUser, IEnumerable<int> userIDs, int addNum)
        {
            CreateInviteSerials(operatorUser, userIDs, addNum, false);
        }


        public void DeleteInviteSerials(AuthUser operatorUser, IEnumerable<int> serialIDs)
        {

            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return;
            }
            else
            {
                if (!CanDeleteInviteSerial(operatorUser))
                {
                    ThrowError(new NoPermissionManageInviteSerialError());
                    return;
                }
            }

            InviteDao.Instance.DeleteInviteSerials(serialIDs);
        }


        /// <summary>
        /// 搜索单个邀请码
        /// </summary>
        public InviteSerial GetInviteSerial(string toEmail)
        {
            return InviteDao.Instance.GetInviteSerial(toEmail);
        }

        public InviteSerial GetInviteSerial(int toUserID)
        {
            return InviteDao.Instance.GetInviteSerial(toUserID);
        }

        public InviteSerial GetInviteSerial(Guid serial)
        {
            return InviteDao.Instance.GetInviteSerial(serial);
        }

        /// <summary>
        /// 获取邀请码列表
        /// </summary>
        public InviteSerialCollection GetInviteSerials(AuthUser operatorUser, int? ownerUserId, InviteSerialFilter filter, int pageNumber, out int totalCount)
        {
            totalCount = 0;
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return new InviteSerialCollection();
            }
            if (operatorUser.UserID != ownerUserId)
            {
                if (!CanManageSerial(operatorUser))
                {
                    ThrowError(new NoPermissionManageInviteSerialError());
                    return new InviteSerialCollection();
                }
            }
            return InviteDao.Instance.GetInviteSerials(ownerUserId, filter, pageNumber, out totalCount);
        }

        /// <summary>
        /// 获取邀请码列表
        /// </summary>
        public InviteSerialCollection GetInviteSerials(AuthUser operatorUser, InviteSerialStatus status, string filter, int pageNumber, out int totalCount)
        {
            totalCount = 0;
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return new InviteSerialCollection();
            }
            return InviteDao.Instance.GetInviteSerials(operatorUser.UserID, status, filter, pageNumber, out totalCount);
        }

        /// <summary>
        /// 查看邀请关系
        /// </summary>
        public InviteSerialCollection GetInviteRelation(int userID)
        {
            return InviteDao.Instance.GetInviteRelation(userID);
        }

        /// <summary>
        /// 用户注册时验证邀请码
        /// </summary>
        /// <param name="serial">邀请码</param>
        /// <param name="invitationUser">如果邀请码有限的话，输出邀请人</param>
        /// <returns></returns>
        public ErrorInfo ValidateInvideCode(string serial, out User invitationUser)
        {
            invitationUser = null;
            if (string.IsNullOrEmpty(serial)) return new EmptyInviteSerialError("serial");

            InvitationSettings settings = AllSettings.Current.InvitationSettings;

            if (settings.InviteMode == InviteMode.Close)
            {
                return new CustomError("serial", "邀请/推广功能并未开放");
            }
            else if (settings.InviteMode == InviteMode.InviteSerialRequire
                || settings.InviteMode == InviteMode.InviteSerialOptional)//邀请码模式
            {
                Guid serialGuid = Guid.Empty;
                InviteSerial inviteSerial = null;
                try
                {
                    serialGuid = new Guid(serial);
                }
                catch
                {
                    return new InviteSerialError("serial", serial);
                }

                inviteSerial = InviteBO.Instance.GetInviteSerial(serialGuid);

                if (inviteSerial == null) return new InviteSerialError("serial", string.Empty);

                if (inviteSerial.ExpiresDate < DateTimeUtil.Now)
                {
                    return new InviteExpiredError();
                }

                switch (inviteSerial.Status)
                {
                    case InviteSerialStatus.Used:
                        {
                            return new InviteUsedError();
                        }
                    case InviteSerialStatus.Expires:
                        {
                            return new InviteExpiredError();
                        }
                    default:
                        {
                            invitationUser = UserBO.Instance.GetUser(inviteSerial.UserID);
                            ////如果邀请人本身就是未受邀请的用户的话
                            //if (invitationUser.Roles.IsInRole(Role.InviteLessUsers)) invitationUser = null; 
                            break;
                        }
                }
            }
            else  //固定连接模式
            {
                //string inviteUsername;

                invitationUser = FixInviteCodeToUser(serial);

                if (invitationUser == null)
                {
                    return new InviteSerialError("serial", serial);
                }
            }
            return null;
        }

        public InviteSerialStat GetStat(int userID)
        {
            return InviteDao.Instance.GetStat(userID);
        }

        /// <summary>
        /// 邀请码统计列表
        /// </summary>
        /// <returns></returns>
        public InviteSerialStatCollection GetStatList(InviteSerialStatus state, int pageSize, int pageNumber, out int rowCount)
        {
            return InviteDao.Instance.GetStatList(state, pageSize, pageNumber, out rowCount);
        }

        /// <summary>
        /// 用户通过积分购买邀请码
        /// </summary>
        /// <param name="buyNumber"></param>
        public void BuyInviteSerial(AuthUser operatorUser, int buyCount)
        {
            if (buyCount <= 0)
            {
                ThrowError(new InviteSerialCountFormatError("buyCount", buyCount.ToString()));
                return;
            }
            if (operatorUser.UserID <= 0)
            {
                ThrowError(new NotLoginError());
                return;
            }

            InvitationSettings invitationSettings = AllSettings.Current.InvitationSettings;

            if (invitationSettings.InviteMode == InviteMode.Close
                || invitationSettings.InviteMode == InviteMode.InviteLinkOptional
                || invitationSettings.InviteMode == InviteMode.InviteLinkRequire
                )
            {
                ThrowError(new InviteSerialDisableError());
                return;
            }

            if (operatorUser.NeedInputInviteSerial)
            {
                ThrowError(new CustomError("operatorUser", "您的账号本身尚未输入邀请码，您不具备购买邀请码的资格"));
                return;
            }

            if (!CanBuyInviteSerial(operatorUser))
            {
                ThrowError(new NoPermissionManageInviteSerialError());
                return;
            }

            DateTime ExpiresDate;
            if (AllSettings.Current.InvitationSettings.InviteEffectiveHours <= 0)
                ExpiresDate = DateTime.MaxValue;
            else
                ExpiresDate = DateTimeUtil.Now.AddHours(AllSettings.Current.InvitationSettings.InviteEffectiveHours);

            //连续购买数量检查
            if (AllSettings.Current.InvitationSettings.Interval != InviteBuyInterval.Disable
                && (AllSettings.Current.InvitationSettings.InviteSerialBuyCount < buyCount
                || InviteDao.Instance.CheckOverFlowBuyCount(operatorUser.UserID
                , AllSettings.Current.InvitationSettings.Interval
                , AllSettings.Current.InvitationSettings.InviteSerialBuyCount
                )))
            {
                ThrowError(new InviteSerialBuyOverflow(AllSettings.Current.InvitationSettings.InviteSerialBuyCount,
                    AllSettings.Current.InvitationSettings.Interval));
                return;
            }

            UserPointType type = invitationSettings.PointFieldIndex;

            UserPoint point = AllSettings.Current.PointSettings.GetUserPoint(type);

            if (!point.Enable)
            {
                ThrowError(new BuyInviteSerialPointNotEnableError(point.Name));
                return;
            }

            int[] points = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };


            points[(int)type] = 0 - invitationSettings.IntiveSerialPoint * buyCount;

            if (UserBO.Instance.UpdateUserPoints(operatorUser.UserID, false, true, points,"邀请",string.Concat("购买", buyCount, "个邀请码每个"+invitationSettings.IntiveSerialPoint)))
            {
                string remark = string.Format("花费{0}{1}{2}购买", AllSettings.Current.InvitationSettings.IntiveSerialPoint, point.UnitName, point.Name); 
                InviteDao.Instance.BuyInviteSerial(operatorUser.UserID, buyCount, ExpiresDate,remark);
            }
        }


        public int GetCanBuyCount(int userid)
        {
            if (AllSettings.Current.InvitationSettings.Interval == InviteBuyInterval.Disable)
                return -1;

            int a = InviteDao.Instance.GetBuyCountOfTimeSpan(userid, AllSettings.Current.InvitationSettings.Interval);

            return AllSettings.Current.InvitationSettings.InviteSerialBuyCount - a;
        }



        #region 推广链接/邀请码/邮箱验证码生成与解码

        /// <summary>
        /// 得到固定的推广链接
        /// </summary>
        /// <param name="username"></param>
        /// <param name="urlEncode">是否进行URL编码</param>
        /// <returns></returns>
        public string BuildFixInviteCode(int userID)
        {
            string result;
            result = string.Format("{0}-{1}", userID, UserBO.Instance.GetUser(userID).CreateDate.Ticks.ToString().TrimEnd('0'));
            return result;
        }

        public User FixInviteCodeToUser(string inviteCode)
        {
            int userID;
            string[] temp = inviteCode.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length != 2) return null;

            if (int.TryParse(temp[0], out userID))
            {
                User u = UserBO.Instance.GetUser(userID);
                if (u != null)
                {
                    if (u.CreateDate.Ticks.ToString().TrimEnd('0') == temp[1]) return u;
                }
            }

            return null;
        }

        #endregion


        public void DeleteByFilter(AuthUser operatorUser, InviteSerialFilter filter)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return;
            }
            else
            {
                if (!CanManageSerial(operatorUser))
                {
                    ThrowError(new NoPermissionManageInviteSerialError());
                    return;
                }
            }
            InviteDao.Instance.DeleteByFilter(filter);
        }

        /// <summary>
        /// 设置用户的邀请码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serial"></param>
        /// <returns></returns>
        public void SetUserInviteSerial(int userId, int inviterID, Guid serial)
        {
            InviteDao.Instance.SetUserInviteSerial(userId, inviterID, serial);
        }

        public bool CanAddInviteSerial(AuthUser operatorUser)
        {
            return AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_InviteSerial);
        }

        public bool CanDeleteInviteSerial(AuthUser operatorUser)
        {
            return AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_InviteSerial);
        }

        /// <summary>
        /// 是否可以高买邀请码
        /// </summary>
        /// <param name="targetUserID"></param>
        /// <returns></returns>
        public bool CanBuyInviteSerial(AuthUser operatorUser)
        {
            return AllSettings.Current.UserPermissionSet.Can(operatorUser, UserPermissionSet.Action.BuyInvieteSerial);
        }

        /// <summary>
        /// 管理邀请码
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <returns></returns>
        public bool CanManageSerial(AuthUser operatorUser)
        {
            return AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_InviteSerial);
        }
    }
}