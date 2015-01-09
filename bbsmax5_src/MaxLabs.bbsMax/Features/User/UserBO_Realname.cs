//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;
using System.IO;
using MaxLabs.bbsMax.Entities;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Settings;
using System.Xml;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax
{
    public partial class UserBO
    {


#region Passport client only
    #if !Passport
        public  void Client_SetUserRealnameChecked( int userID , string realname)
        {
            UserDao.Instance.UpdateUserRealname(userID,realname);
        }

        public void Client_SetUserRealnameUncheck(int userID)
        {
            UserDao.Instance.UpdateUserRealname(userID, string.Empty);
        }


    #endif
#endregion

        /// <summary>
        /// 实名认证
        /// </summary>
        /// <param name="targetUserIds"></param>
        /// <param name="realnameChecked">是否</param>
        public void AdminSetRealnameChaecked(AuthUser operatorUser, int targetUserId, bool realnameChecked, string remark,bool sendNotify)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return;
            }

            if (!CanRealnameCheck(operatorUser))
            {
                ThrowError(new NoPermissionRealnameCheckError());
                return;
            }



            UserDao.Instance.SetRealnameChecked(operatorUser.UserID, targetUserId, realnameChecked,remark );

            if (sendNotify)
            {
                string content = realnameChecked ? "恭喜您已通过实名认证" : "您的实名认证被拒绝," + (!string.IsNullOrEmpty(remark) ? "原因：" + StringUtil.CutString(remark, 100) : "");
                AdminManageNotify notify = new AdminManageNotify(targetUserId, content);
                notify.UserID = targetUserId;
                NotifyBO.Instance.AddNotify(operatorUser, notify);
            }

            RemoveUserCache(targetUserId);

            if (realnameChecked)
            {
                if (OnUserRealnameChecked != null)
                {
                    AuthenticUser authenticUserInfo = GetAuthenticUserInfo(operatorUser, targetUserId);

                    if (authenticUserInfo != null)
                    {
                        OnUserRealnameChecked(targetUserId, authenticUserInfo.Realname, authenticUserInfo.IDNumber);
                    }
                    else
                    {
                        AuthUser user = GetAuthUser(targetUserId);
                        if (user != null)
                        {
                            OnUserRealnameChecked(targetUserId, user.Realname, string.Empty);
                        }
                    }
                }
            }
            else
            {
                if (OnUserCancelRealnameCheck != null)
                    OnUserCancelRealnameCheck(targetUserId);
            }
        }


        public AuthenticUserCollection GetAuthenticUsers( AuthUser operatorUser,AuthenticUserFilter filter,int pageNumber)
        {

            if (this.CanRealnameCheck(operatorUser))
            {

                if (filter.PageSize <= 0)
                    filter.PageSize = Consts.DefaultPageSize;

                if (pageNumber <= 0)
                    pageNumber = 1;

                return  UserDao.Instance.GetAuthenticUsers(filter, pageNumber);
            }
            else
            {
                ThrowError(new NoPermissionRealnameCheckError());
            }
            return new AuthenticUserCollection();
        }

        public AuthenticUser GetAuthenticUserInfo( AuthUser operatorUser,int userID )
        {
            if (userID == operatorUser.UserID || UserBO.Instance.CanRealnameCheck(operatorUser))
            {
               return  UserDao.Instance.GetAuthenticUser(userID);
            }
            else
            {
               ThrowError(new NoPermissionRealnameCheckError());
                return null;
            }

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="username"></param>
        /// <param name="idCardImage"></param>
        public bool SaveUserRealnameData(AuthUser operatorUser,string idNumber, string realname,HttpPostedFile idCardFileFace,HttpPostedFile idCardFileBack)
        {
            if(operatorUser.UserID<=0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if(!AllSettings.Current.NameCheckSettings.EnableRealnameCheck)
            {
                ThrowError(new CustomError("管理员未开启实名认证功能"));
                return false;
            }

            if (UserDao.Instance.CheckIdNumberExist(idNumber))
            { 
                ThrowError(new CustomError("idnumber","您输入的身份证号码已经存在"));
                return false;
            }

            AuthenticUser AuthenticUser = UserDao.Instance.GetAuthenticUser(operatorUser.UserID);

            if (AuthenticUser != null)
            {

                if (AuthenticUser.Processed == false)
                {
                    ThrowError(new CustomError("您的实名认证材料正在审核中请勿重复提交"));
                    return false;
                }

                if (AuthenticUser.Processed==true && operatorUser.RealnameChecked)
                {
                    ThrowError(new CustomError("您已经通过实名认证， 不可再更改身份信息"));
                    return false;
                }
            }

            realname = (string.Empty + realname).Trim();

            if(realname.Length<2||realname.Length>15)
            {
                ThrowError(new CustomError("realname", "姓名不能少于2个字符并且不能超过15个字符"));
                return false;
            }

            #region  中英文格式检查

            ////中文检查
            //bool formatchecked = false;
            //if (setting.CanChinese)
            //{
            //    if (Regex.IsMatch(realname, (@"^[\u4e00-\u9fa5\s]{2,8}$")))
            //    {
            //        formatchecked = true;
            //    }
            //}

            ////英文检查
            //if (setting.CanEnglish)
            //{
            //    if (Regex.IsMatch(realname, @"^[a-zA-Z]+\s{0,1}[a-zA-Z]+$"))
            //    {
            //        formatchecked = true;
            //    }
            //}

            //if (!setting.CanEnglish &&
            //    !setting.CanChinese &&
            //    !string.IsNullOrEmpty(realname)
            //   )
            //{
            //    //两种都不行 设置上 疏忽了, 那就没有限制
            //    formatchecked = true;
            //}

            //if (!formatchecked)
            //{
            //    ThrowError(new RealnameFormatError(realname, setting.CanChinese, setting.CanEnglish));
            //    return;
            //}

            #endregion

            if (!Regex.IsMatch(realname, (@"^[\u4e00-\u9fa5\s]{2,15}$")))
            {
                ThrowError(new CustomError("realname", "您输入的真实姓名包含无效的非中文字符"));
                return false;
            }

            string[] idCardInfo ;
            
            if( !IsIDCardNumber(idNumber,out idCardInfo))
            {
                ThrowError(new CustomError("idnumber","身份证号码无效"));
                return false;
            }

            string fullPathFace=string.Empty, fullPathBack= string.Empty;
            if (AllSettings.Current.NameCheckSettings.NeedIDCardFile)
            {
                bool saveFaceFile = true;
                bool saveBackFile = true;

                saveFaceFile = ValidateAndSavePostedFile(operatorUser, idCardFileFace, "idcardfileface", "face", out fullPathFace);
                saveBackFile = ValidateAndSavePostedFile(operatorUser, idCardFileBack, "idcardfileback", "back", out fullPathBack);

                if (saveBackFile == false || saveFaceFile == false)
                {
                    return false;
                }
            }

            Gender gender = StringUtil.TryParse<Gender>( idCardInfo[0]);
            DateTime birthday = StringUtil.TryParse<DateTime>(idCardInfo[1]);

            UserDao.Instance.SaveAuthenticUserInfo(operatorUser.UserID, realname, idNumber, fullPathFace,fullPathBack, birthday, gender, idCardInfo[2]);
            return true;
        }


        private bool ValidateAndSavePostedFile(AuthUser operatorUser,HttpPostedFile postedFile, string errorName,string extraFileSuffix,out string fullPath)
        {
            fullPath = string.Empty;
            if (postedFile == null)
            {
                ThrowError(new CustomError(errorName, "请上传身份证" + (extraFileSuffix=="face"?"正面":"背面") + "扫描件"));
                return false;
            }

            List<string> allowedFileType = new List<string>(new string[] { ".jpg", ".png", ".gif" });

            byte[] data = new byte[postedFile.ContentLength];

            if (data.Length > AllSettings.Current.NameCheckSettings.MaxIDCardFileSize)
            {
                ThrowError(new CustomError(errorName, "身份证扫描件文件大小不能超过" + ConvertUtil.FormatSize(AllSettings.Current.NameCheckSettings.MaxIDCardFileSize)));
                return false;
            }

            postedFile.InputStream.Read(data, 0, data.Length);

            string fileType = Path.GetExtension(postedFile.FileName).ToLower();

            if (!allowedFileType.Contains(fileType) || !IOUtil.IsImageFile(data, ImageFileType.GIF | ImageFileType.JPG | ImageFileType.PNG))
            {
                ThrowError(new CustomError(errorName, "身份证扫描件格式不正确"));
                return false;
            }

            string newFileName = string.Format("{0}-{1}{2}", operatorUser.UserID, extraFileSuffix, ".config");
            string path = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Upload_IDCard), operatorUser.UserID.ToString());

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path); //不做异常捕获
            }

            try
            {
                File.WriteAllBytes(IOUtil.JoinPath(path, newFileName), data);
            }
            catch (Exception ex)
            {
                ThrowError(new CustomError("发生了系统错误" + ex.Message));
                return false;
            }

            fullPath = IOUtil.JoinPath(Globals.GetVirtualPath(SystemDirecotry.Upload_IDCard),operatorUser.UserID.ToString(),newFileName);

            return true;

        }

        /// <summary>
        /// 身份证号码校验身份证号码
        /// </summary>
        /// <param name="idNumber">身份证号码</param>
        /// <param name="userInfo">返回身份证号码里面所包含的信息(0:性别，1:生日，2:所在地)</param>
        /// <returns>True 有效身份证号码 False 错误的身份证号码</returns>
        private static bool IsIDCardNumber(string idNumber, out string[] userInfo)
        {
            userInfo = null;

            if (idNumber == null || idNumber.Trim().Length != 18)
            {
                return false;
            }

            int year, month, day;
            int areaCode;
            int serialCode;
            string checkCode;

            try
            {
                areaCode = int.Parse(idNumber.Substring(0, 6));
            }
            catch
            {
                return false;
            }

            try
            {
                year = int.Parse(idNumber.Substring(6, 4));
            }
            catch
            {
                return false;
            }

            try
            {
                month = int.Parse(idNumber.Substring(10, 2));
            }
            catch
            {
                return false;
            }

            try
            {
                day = int.Parse(idNumber.Substring(12, 2));
            }
            catch
            {
                return false;
            }

            try
            {
                serialCode = int.Parse(idNumber.Substring(14, 3));
            }
            catch
            {
                return false;
            }

            #region 校验码校验
            checkCode = idNumber.Substring(17, 1);
            int[] checkSerial = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            string[] allowCheckCodes = new string[] { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };

            int sCount = 0;

            for (int i = 0; i < 17; i++)
            {
                sCount += int.Parse(idNumber[i].ToString()) * checkSerial[i];
            }

            string checkCodeResult = allowCheckCodes[sCount % 11];

            if (checkCodeResult != checkCode) //校验码错误
            {
                return false;
            }

            #endregion

            if (areaCode < 100000) //区域码无效
            {
                return false;
            }

            if (year > DateTime.Now.Year - 10 || year < DateTime.Now.Year - 90) //年龄限制10岁到90岁
            {
                return false;
            }

            if (month > 12 || month < 1)//月份无效
            {
                return false;
            }

            if (day < 1 || day > 31)//日无效
            {
                return false;
            }
            else if (DateTime.DaysInMonth(year, month) < day)//最大日期数超出
            {
                return false;
            }

            userInfo = new string[3];

            userInfo[0] = (serialCode % 2 == 1 ? Gender.Male : Gender.Female).ToString();
            userInfo[1] = string.Format("{0}-{1}-{2}", year, month, day);
            userInfo[2] = string.Empty;

            return true;
        }


        public int DetectAuthenticInfo(AuthUser operatorUser, int userID, out List<string> photos)
        {
            photos = null;

            if (operatorUser.UserID <= 0)
            {
                ThrowError(new NotLoginError());
                return 4;
            }

            if (!CanRealnameCheck(operatorUser))
            {
                ThrowError(new NoPermissionRealnameCheckError());
                return 4;
            }

            AuthenticUser userInfo = GetAuthenticUserInfo(operatorUser, userID);

            if (userInfo == null)
            {
                ThrowError(new CustomError("没有该用户提交的实名认证材料"));
                return 4;
            }
            List<byte[]> photoData;
            int state = DetectAuthenticInfo(userInfo.Realname, userInfo.IDNumber, out photoData);

            if (state == 0)
            {
                photos = new List<string>();
                if (photoData != null)
                {
                    string photoString = "";
                    string temp;
                    string photoDirName = "Photos";
                    string photoPath = Globals.GetPath(SystemDirecotry.Upload_IDCard, photoDirName);
                    string virtualPath = Globals.GetVirtualPath(SystemDirecotry.Upload_IDCard, photoDirName);

                    if (!Directory.Exists(photoPath))
                    {
                        Directory.CreateDirectory(photoPath);
                    }

                    for (int i = 0; i < photoData.Count; i++)
                    {
                        string fileName = string.Format("{0}_{1}.jpg", userInfo.IDNumber, i);

                        if (photoString.Length > 0)
                            photoString += "|";

                        temp = UrlUtil.JoinUrl(virtualPath, fileName);
                        photoString += temp;

                        photos.Add(temp);

                        fileName = IOUtil.JoinPath(photoPath, fileName);
                        if (!File.Exists(fileName))
                            File.WriteAllBytes(fileName, photoData[i]);

                        if (photos.Count > 1) //多余的照片不要， 只要最多两张
                            break;
                    }

                    UserDao.Instance.UpdateAuthenticUserPhoto(userID, photoString,state);
                }
            }

            return state;
        }

        /// <summary>
        /// 远程接口调用
        /// </summary>
        /// <param name="realname"></param>
        /// <param name="idNumber"></param>
        /// <param name="photoData"></param>
        /// <returns></returns>
        private int DetectAuthenticInfo( string realname,string idNumber , out List<byte[]> photoData )
        {
            int state = 0;
            photoData = null;

            return state;
        }
    }
}