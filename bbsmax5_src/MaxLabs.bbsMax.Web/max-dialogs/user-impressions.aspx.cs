//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs.user
{
    public partial class user_impressions : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("CreateImpression"))
            {
                MessageDisplay msgDisplay = CreateMessageDisplayForForm("ImpressionForum");
                string text = _Request.Get("Text", Method.Post, string.Empty);

                bool success;

                using (ErrorScope es = new ErrorScope())
                {
                    try
                    {
                        success = ImpressionBO.Instance.CreateImpression(My, User, text);
                        if (success == false)
                        {
                            es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                msgDisplay.AddError(error);
                            });
                        }
                        else
                        {
                            m_IsShowImpressionInput = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        msgDisplay.AddException(ex);
                    }

                    if (msgDisplay.HasAnyError())
                        m_IsShowImpressionInput = true;
                }
            }
            else if (_Request.Get<int>("typeid",Method.Get,0)>0)
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();
                int typeID = _Request.Get<int>("TypeID", Method.Get, 0);
                using (ErrorScope es = new ErrorScope())
                {
                    try
                    {
                        ImpressionBO.Instance.DeleteImpressionTypeForUser(My, typeID);
                    }
                    catch (Exception ex)
                    {
                        msgDisplay.AddError(ex.Message);
                    }
                }
            }

            WaitForFillSimpleUsers<ImpressionRecord>(ImpressionRecordList, 0);
            WaitForFillSimpleUsers<Impression>(ImpressionList);
        }


        private bool? m_IsShowImpressionInput;
        protected bool IsShowImpressionInput
        {
            get
            {
                if (m_IsShowImpressionInput == null)
                {
                    m_IsShowImpressionInput = _Request.Get("imp", Method.Get, "0") == "1";
                }

                return m_IsShowImpressionInput.Value;
            }
        }

        private int? m_UserID;
        protected int UserID
        {
            get
            {
                if (m_UserID == null)
                {
                    m_UserID = _Request.Get<int>("uid", Method.Get, 0);
                }
                return m_UserID.Value;
            }
        }

        private User m_User;
        protected User User
        {
            get
            {
                if (m_User == null)
                {
                    m_User = UserBO.Instance.GetUser(UserID);
                }
                return m_User;
            }
        }

        private bool? m_VisitorIsFriend;
        protected bool VisitorIsFriend
        {
            get
            {
                if (m_VisitorIsFriend.HasValue == false)
                    m_VisitorIsFriend = FriendBO.Instance.IsFriend(UserID, MyUserID);

                return m_VisitorIsFriend.Value;
            }
        }
        private ImpressionRecord m_LastImpressionRecord;
        protected ImpressionRecord LastImpressionRecord
        {
            get
            {
                if (m_LastImpressionRecord == null)
                {
                    m_LastImpressionRecord = ImpressionBO.Instance.GetLastImpressionRecord(MyUserID, UserID);
                }
                return m_LastImpressionRecord;
            }
        }

        private bool? m_CanImpression;
        public bool CanImpression
        {
            get
            {
                if (m_CanImpression == null)
                {
                    if (LastImpressionRecord == null)
                        m_CanImpression = true;
                    else
                    {
                        m_CanImpression = LastImpressionRecord.CreateDate.AddHours(AllSettings.Current.ImpressionSettings.TimeLimit) <= DateTimeUtil.Now;
                    }
                }

                return m_CanImpression.Value;
            }
        }


        private ImpressionTypeCollection m_ImpressionTypeList;

        protected ImpressionTypeCollection ImpressionTypeList
        {
            get 
            {
                if (m_ImpressionTypeList == null)
                {
                    m_ImpressionTypeList = ImpressionBO.Instance.GetImpressionTypesForUse(UserID, 8, 4); 
                }
                return m_ImpressionTypeList;
            }
        }

        private ImpressionCollection m_ImpressionList;

        public ImpressionCollection ImpressionList
        {
            get 
            {
                if (m_ImpressionList == null)
                { 
                    SpaceData spaceData = SpaceBO.Instance.GetSpaceDataForVisit(MyUserID, UserID);
                    if (spaceData == null)
                        m_ImpressionList = new ImpressionCollection();
                    else
                        m_ImpressionList = spaceData.ImpressionList;
                }

                return m_ImpressionList;
            }
        }

        private ImpressionRecordCollection m_ImpressionRecordList;

        protected ImpressionRecordCollection ImpressionRecordList
        {
            get
            {
                if (m_ImpressionRecordList == null)
                {
                    m_ImpressionRecordList = ImpressionBO.Instance.GetTargetUserImpressionRecords(UserID, 1, 5);
                }
                return m_ImpressionRecordList;
            }
        }

        public string NextImpressionTime
        {
            get
            {
                if (LastImpressionRecord != null)
                {
                    DateTime nextTime = LastImpressionRecord.CreateDate.AddHours(AllSettings.Current.ImpressionSettings.TimeLimit);

                    TimeSpan span = nextTime - DateTimeUtil.Now;

                    int hours = (int)Math.Round(span.TotalHours);

                    if (hours >= 24)
                    {
                        int days = (int)Math.Round(span.TotalDays);

                        if (days == 1)
                            return "明天";
                        else if (days == 2)
                            return "后天";
                        else if (days == 3)
                            return "大后天";
                        else if (days == 7)
                            return "一星期";

                        return days + "天";
                    }
                    else
                        return hours + "小时";
                }

                return string.Empty;
            }
        }


    }
}