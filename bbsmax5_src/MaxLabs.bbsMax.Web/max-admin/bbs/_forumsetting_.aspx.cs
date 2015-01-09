//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;


using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using System.Collections.Generic;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Errors;
using System.Text;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.ValidateCodes;
using System.Reflection;
using MaxLabs.bbsMax.ExceptableSetting;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class _forumsetting_ : AdminPageBase //: SettingPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_ForumDetail_EditSetting; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ForumSetting == null)
            {
                ShowError(new InvalidParamError("forumID"));
            }
            if (_Request.IsClick("saveforumsettings"))
            {
                SaveSettings2();
            }
        }

        protected bool IsForumPage
        {
            get
            {
                return Request.CurrentExecutionFilePath.ToLower().IndexOf("manage-forum-detail.aspx") >= 0;
            }
        }

        private void SaveSettings2()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("PostContentLengths", "PostSubjectLengths", "PolemizeValidDays", "PollValidDays", "QuestionValidDays",
                "RecycleOwnThreadsIntervals", "UpdateThreadSortOrderIntervals", "CreatePostIntervals", "DeleteOwnThreadsIntervals", "AllowFileExtensions",
                "new_PostContentLengths", "new_PostSubjectLengths", "new_PolemizeValidDays", "new_PollValidDays", "new_QuestionValidDays",
                "new_RecycleOwnThreadsIntervals", "new_UpdateThreadSortOrderIntervals", "new_CreatePostIntervals", "new_DeleteOwnThreadsIntervals", "new_AllowFileExtensions"
                , "AllowAttachment", "AllowImageAttachment", "CreatePostAllowAudioTag", "CreatePostAllowEmoticon", "CreatePostAllowFlashTag", "CreatePostAllowHTML"
                , "CreatePostAllowImageTag", "CreatePostAllowMaxcode", "CreatePostAllowTableTag", "CreatePostAllowUrlTag", "CreatePostAllowVideoTag", "MaxPostAttachmentCount"
                , "MaxTopicAttachmentCount", "MaxSignleAttachmentSize", "ShowSignatureInThread", "ShowSignatureInPost", "CreateThreadNeedApprove", "ReplyNeedApprove"
                , "new_AllowAttachment", "new_AllowImageAttachment", "new_CreatePostAllowAudioTag", "new_CreatePostAllowEmoticon", "new_CreatePostAllowFlashTag", "new_CreatePostAllowHTML"
                , "new_CreatePostAllowImageTag", "new_CreatePostAllowMaxcode", "new_CreatePostAllowTableTag", "new_CreatePostAllowUrlTag", "new_CreatePostAllowVideoTag", "new_MaxPostAttachmentCount"
                , "new_MaxTopicAttachmentCount", "new_MaxSignleAttachmentSize", "new_ShowSignatureInThread", "new_ShowSignatureInPost", "new_CreateThreadNeedApprove", "new_ReplyNeedApprove"
                , "UpdateOwnPostIntervals", "new_UpdateOwnPostIntervals"
                , "EnableSellThread", "new_EnableSellThread"
                , "EnableSellAttachment", "new_EnableSellAttachment"
                , "DisplayInList", "new_DisplayInList"
                , "VisitForum", "new_VisitForum"
                , "SellAttachmentDays"
                , "SellThreadDays"
                , "ReplyReturnThreadLastPage"
                , "ThreadSortField"
                );


            if (_Request.Get("inheritType", Method.Post, "False").ToLower() == "true")//继承上级
            {
                ForumSettings tempSetting = AllSettings.Current.ForumSettings.Clone();

                ForumSettingItemCollection tempItems = new ForumSettingItemCollection();

                for (int i = 0; i < tempSetting.Items.Count; i++)
                {
                    if (tempSetting.Items[i].ForumID == ForumID)
                    {
                    }
                    else
                        tempItems.Add(tempSetting.Items[i]);
                }

                tempSetting.Items = tempItems;
                try
                {
                    if (!SettingManager.SaveSettings(tempSetting))
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                        m_Success = false;
                    }
                    else
                    {
                        BbsRouter.JumpToUrl(Request.RawUrl, "success=true");
                    }
                }
                catch (Exception ex)
                {
                    m_Success = false;
                    msgDisplay.AddError(ex.Message);
                }
                return;
            }



            ForumSettingItem forumSetItem = new ForumSettingItem();

            ExceptableItem_Int32scope int32scope = new ExceptableItem_Int32scope();
            ExceptableItem_Second<long> second_long = new ExceptableItem_Second<long>();
            ExceptableItem_Second<int> second_int = new ExceptableItem_Second<int>();
            ExceptableItem_ExtensionList extensionList = new ExceptableItem_ExtensionList();

            forumSetItem.ForumID = ForumID;
            forumSetItem.PostContentLengths = int32scope.GetExceptable("PostContentLengths", msgDisplay); //GetInt32Exceptable("PostContentLengths",msgDisplay);
            forumSetItem.PostSubjectLengths = int32scope.GetExceptable("PostSubjectLengths", msgDisplay);//GetInt32Exceptable("PostSubjectLengths",msgDisplay);
            forumSetItem.PolemizeValidDays = second_long.GetExceptable("PolemizeValidDays",msgDisplay);
            forumSetItem.PollValidDays = second_long.GetExceptable("PollValidDays", msgDisplay);
            forumSetItem.QuestionValidDays = second_long.GetExceptable("QuestionValidDays", msgDisplay);
            forumSetItem.RecycleOwnThreadsIntervals = second_int.GetExceptable("RecycleOwnThreadsIntervals", msgDisplay);
            forumSetItem.UpdateThreadSortOrderIntervals = second_int.GetExceptable("UpdateThreadSortOrderIntervals", msgDisplay);
            forumSetItem.UpdateOwnPostIntervals = second_int.GetExceptable("UpdateOwnPostIntervals", msgDisplay);
            forumSetItem.CreatePostIntervals = second_int.GetExceptable("CreatePostIntervals", msgDisplay);
            forumSetItem.DeleteOwnThreadsIntervals = second_int.GetExceptable("DeleteOwnThreadsIntervals", msgDisplay);
            forumSetItem.AllowFileExtensions = extensionList.GetExceptable("AllowFileExtensions", msgDisplay);

            forumSetItem.AllowAttachment = new ExceptableItem_bool().GetExceptable("AllowAttachment", msgDisplay);
            forumSetItem.AllowImageAttachment = new ExceptableItem_bool().GetExceptable("AllowImageAttachment", msgDisplay);
            forumSetItem.CreatePostAllowAudioTag = new ExceptableItem_bool().GetExceptable("CreatePostAllowAudioTag", msgDisplay);
            forumSetItem.CreatePostAllowEmoticon = new ExceptableItem_bool().GetExceptable("CreatePostAllowEmoticon", msgDisplay);
            forumSetItem.CreatePostAllowFlashTag = new ExceptableItem_bool().GetExceptable("CreatePostAllowFlashTag", msgDisplay);
            forumSetItem.CreatePostAllowHTML = new ExceptableItem_bool().GetExceptable("CreatePostAllowHTML", msgDisplay);
            forumSetItem.CreatePostAllowImageTag = new ExceptableItem_bool().GetExceptable("CreatePostAllowImageTag", msgDisplay);
            forumSetItem.CreatePostAllowMaxcode = new ExceptableItem_bool().GetExceptable("CreatePostAllowMaxcode", msgDisplay);
            forumSetItem.CreatePostAllowTableTag = new ExceptableItem_bool().GetExceptable("CreatePostAllowTableTag", msgDisplay);
            forumSetItem.CreatePostAllowUrlTag = new ExceptableItem_bool().GetExceptable("CreatePostAllowUrlTag", msgDisplay);
            forumSetItem.CreatePostAllowVideoTag = new ExceptableItem_bool().GetExceptable("CreatePostAllowVideoTag", msgDisplay);
            forumSetItem.MaxPostAttachmentCount = new ExceptableItem_Int_MoreThenZero().GetExceptable("MaxPostAttachmentCount", msgDisplay);
            forumSetItem.MaxTopicAttachmentCount = new ExceptableItem_Int_MoreThenZero().GetExceptable("MaxTopicAttachmentCount", msgDisplay);
            forumSetItem.MaxSignleAttachmentSize = new ExceptableItem_FileSize().GetExceptable("MaxSignleAttachmentSize", msgDisplay);
            forumSetItem.ShowSignatureInPost = new ExceptableItem_bool().GetExceptable("ShowSignatureInPost",msgDisplay);
            forumSetItem.ShowSignatureInThread = new ExceptableItem_bool().GetExceptable("ShowSignatureInThread", msgDisplay);
            forumSetItem.CreateThreadNeedApprove = new ExceptableItem_bool().GetExceptable("CreateThreadNeedApprove", msgDisplay);
            forumSetItem.ReplyNeedApprove = new ExceptableItem_bool().GetExceptable("ReplyNeedApprove", msgDisplay);
            forumSetItem.EnableSellThread = new ExceptableItem_bool().GetExceptable("EnableSellThread", msgDisplay);
            forumSetItem.EnableSellAttachment = new ExceptableItem_bool().GetExceptable("EnableSellAttachment", msgDisplay);
            forumSetItem.EnableHiddenTag = _Request.Get<bool>("enableHiddenTag", Method.Post, true);
            forumSetItem.EnableThreadRank = _Request.Get<bool>("EnableThreadRank", Method.Post, true);
            forumSetItem.DefaultThreadSortField = _Request.Get<ThreadSortField>("ThreadSortField", Method.Post, ThreadSortField.LastReplyDate);


            ThreadSortField oldThreadSortField = ForumSetting.DefaultThreadSortField;
            ThreadSortField newThreadSortField = forumSetItem.DefaultThreadSortField;


            forumSetItem.AllowGuestVisitForum = _Request.Get<bool>("AllowGuestVisitForum", Method.Post, true);
            forumSetItem.DisplayInListForGuest = _Request.Get<bool>("DisplayInListForGuest", Method.Post, true);
            forumSetItem.VisitForum = new ExceptableItem_bool().GetExceptable("VisitForum", msgDisplay);
            forumSetItem.DisplayInList = new ExceptableItem_bool().GetExceptable("DisplayInList", msgDisplay);



            forumSetItem.SellThreadDays = GetSeconds("SellThreadDays", msgDisplay);
            forumSetItem.SellAttachmentDays = GetSeconds("SellAttachmentDays", msgDisplay);

            forumSetItem.ReplyReturnThreadLastPage = _Request.Get<bool>("ReplyReturnThreadLastPage", Method.Post, false);

            if (msgDisplay.HasAnyError())
            {
                m_Success = false;
                return;
            }

            ForumSettings settings = AllSettings.Current.ForumSettings.Clone();
            ForumSettingItemCollection items = new ForumSettingItemCollection();


            bool hasAdd = false;
            bool hasTopItem = false;//是否有ForumID为0的设置  
            foreach (ForumSettingItem item in settings.Items)
            {
                if (item.ForumID == forumSetItem.ForumID)
                {
                    items.Add(forumSetItem);
                    hasAdd = true;
                }
                else
                {
                    ProcessApplyAllForumSetting(item,forumSetItem);

                    items.Add(item);
                }
                if (item.ForumID == 0)
                    hasTopItem = true;
            }
            if (hasAdd == false)
                items.Add(forumSetItem);


            if (hasTopItem == false)//如果没有 加入
            {
                ForumSettingItem tempItem = new ForumSettingItem();
                ProcessApplyAllForumSetting(tempItem, forumSetItem);
                items.Add(tempItem);
            }

            settings.Items = items;

            try
            {
                using (new ErrorScope())
                {

                    bool success = SettingManager.SaveSettings(settings);

                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                        m_Success = false;
                    }
                    else
                    {
                        if (_Request.Get<bool>("ThreadSortField_aplyallnode", Method.Post, false))
                        {
                            ThreadCachePool.ClearAllCache();
                        }
                        else if (oldThreadSortField != newThreadSortField)
                        {
                            ThreadCachePool.ClearAllCache();
                        }

                        string rawUrl = Request.RawUrl;

                        BbsRouter.JumpToUrl(rawUrl, "success=true");
                    }

                }
            }
            catch (Exception ex)
            {
                m_Success = false;
                msgDisplay.AddError(ex.Message);
            }
        }

        private long GetSeconds(string name,MessageDisplay msgDisplay)
        {
            string valueString = _Request.Get(name + "Value", Method.Post, string.Empty);
            if(string.IsNullOrEmpty(valueString))
                return 0;
            long value;
            if (long.TryParse(valueString, out value))
            {
                TimeUnit unit = _Request.Get<TimeUnit>(name + "Unit", Method.Post, TimeUnit.Second);
                return DateTimeUtil.GetSeconds(value, unit);
            }
            else
            {
                msgDisplay.AddError(name, "请填写整数");
                return 0;
            }
        }

        private bool? m_Success;
        protected bool Success
        {
            get
            {
                if (m_Success == null)
                {
                    m_Success = _Request.Get("success", Method.Get, string.Empty).ToLower() == "true";
                }

                return m_Success.Value;
            }
        }

        private void ProcessApplyAllForumSetting(ForumSettingItem item, ForumSettingItem forumSetItem)
        {
            ExceptableItem_Int32scope int32scope = new ExceptableItem_Int32scope();
            ExceptableItem_Second<long> second_long = new ExceptableItem_Second<long>();
            ExceptableItem_Second<int> second_int = new ExceptableItem_Second<int>();
            ExceptableItem_ExtensionList extensionList = new ExceptableItem_ExtensionList();

            if (int32scope.AplyAllNode("PostContentLengths"))
                item.PostContentLengths = forumSetItem.PostContentLengths;

            if (int32scope.AplyAllNode("PostSubjectLengths"))
                item.PostSubjectLengths = forumSetItem.PostSubjectLengths;

            if (second_long.AplyAllNode("PolemizeValidDays"))
                item.PolemizeValidDays = forumSetItem.PolemizeValidDays;

            if (second_long.AplyAllNode("PollValidDays"))
                item.PollValidDays = forumSetItem.PollValidDays;

            if (second_long.AplyAllNode("QuestionValidDays"))
                item.QuestionValidDays = forumSetItem.QuestionValidDays;


            if (second_long.AplyAllNode("SellThreadDays"))
                item.SellThreadDays = forumSetItem.SellThreadDays;


            if (second_long.AplyAllNode("SellAttachmentDays"))
                item.SellAttachmentDays = forumSetItem.SellAttachmentDays;

            if (second_int.AplyAllNode("RecycleOwnThreadsIntervals"))
                item.RecycleOwnThreadsIntervals = forumSetItem.RecycleOwnThreadsIntervals;

            if (second_int.AplyAllNode("UpdateThreadSortOrderIntervals"))
                item.UpdateThreadSortOrderIntervals = forumSetItem.UpdateThreadSortOrderIntervals;

            if (second_int.AplyAllNode("CreatePostIntervals"))
                item.CreatePostIntervals = forumSetItem.CreatePostIntervals;

            if (second_int.AplyAllNode("DeleteOwnThreadsIntervals"))
                item.DeleteOwnThreadsIntervals = forumSetItem.DeleteOwnThreadsIntervals;

            if (extensionList.AplyAllNode("AllowFileExtensions"))
                item.AllowFileExtensions = forumSetItem.AllowFileExtensions;


            ExceptableItem_bool except_bool = new ExceptableItem_bool();

            if (except_bool.AplyAllNode("AllowAttachment"))
                item.AllowAttachment = forumSetItem.AllowAttachment;

            if (except_bool.AplyAllNode("AllowImageAttachment"))
                item.AllowImageAttachment = forumSetItem.AllowImageAttachment;

            if (except_bool.AplyAllNode("CreatePostAllowAudioTag"))
                item.CreatePostAllowAudioTag = forumSetItem.CreatePostAllowAudioTag;

            if (except_bool.AplyAllNode("CreatePostAllowEmoticon"))
                item.CreatePostAllowEmoticon = forumSetItem.CreatePostAllowEmoticon;

            if (except_bool.AplyAllNode("CreatePostAllowFlashTag"))
                item.CreatePostAllowFlashTag = forumSetItem.CreatePostAllowFlashTag;

            if (except_bool.AplyAllNode("CreatePostAllowHTML"))
                item.CreatePostAllowHTML = forumSetItem.CreatePostAllowHTML;

            if (except_bool.AplyAllNode("CreatePostAllowImageTag"))
                item.CreatePostAllowImageTag = forumSetItem.CreatePostAllowImageTag;

            if (except_bool.AplyAllNode("CreatePostAllowMaxcode"))
                item.CreatePostAllowMaxcode = forumSetItem.CreatePostAllowMaxcode;

            if (except_bool.AplyAllNode("CreatePostAllowTableTag"))
                item.CreatePostAllowTableTag = forumSetItem.CreatePostAllowTableTag;

            if (except_bool.AplyAllNode("CreatePostAllowUrlTag"))
                item.CreatePostAllowUrlTag = forumSetItem.CreatePostAllowUrlTag;

            if (except_bool.AplyAllNode("CreatePostAllowVideoTag"))
                item.CreatePostAllowVideoTag = forumSetItem.CreatePostAllowVideoTag;

            if (new ExceptableItem_Int_MoreThenZero().AplyAllNode("MaxPostAttachmentCount"))
                item.MaxPostAttachmentCount = forumSetItem.MaxPostAttachmentCount;

            if (new ExceptableItem_Int_MoreThenZero().AplyAllNode("MaxTopicAttachmentCount"))
                item.MaxTopicAttachmentCount = forumSetItem.MaxTopicAttachmentCount;

            if (new ExceptableItem_FileSize().AplyAllNode("MaxSignleAttachmentSize"))
                item.MaxSignleAttachmentSize = forumSetItem.MaxSignleAttachmentSize;

            if (new ExceptableItem_bool().AplyAllNode("ShowSignatureInThread"))
                item.ShowSignatureInThread = forumSetItem.ShowSignatureInThread;

            if (new ExceptableItem_bool().AplyAllNode("ShowSignatureInPost"))
                item.ShowSignatureInPost = forumSetItem.ShowSignatureInPost;


            if (new ExceptableItem_bool().AplyAllNode("CreateThreadNeedApprove"))
                item.CreateThreadNeedApprove = forumSetItem.CreateThreadNeedApprove;

            if (new ExceptableItem_bool().AplyAllNode("ReplyNeedApprove"))
                item.ReplyNeedApprove = forumSetItem.ReplyNeedApprove;

            if (new ExceptableItem_bool().AplyAllNode("EnableSellThread"))
                item.EnableSellThread = forumSetItem.EnableSellThread;

            if (new ExceptableItem_bool().AplyAllNode("EnableSellAttachment"))
                item.EnableSellAttachment = forumSetItem.EnableSellAttachment;

            if (second_int.AplyAllNode("UpdateOwnPostIntervals"))
                item.UpdateOwnPostIntervals = forumSetItem.UpdateOwnPostIntervals;

            if (_Request.Get<bool>("enableHiddenTag_aplyallnode", Method.Post, false))
            {
                item.EnableHiddenTag = forumSetItem.EnableHiddenTag;
            }

            if (_Request.Get<bool>("enableThreadRank_aplyallnode", Method.Post, false))
            {
                item.EnableThreadRank = forumSetItem.EnableThreadRank;
            }


            if (_Request.Get<bool>("displayInListForGuest_aplyallnode", Method.Post, false))
            {
                item.DisplayInListForGuest = forumSetItem.DisplayInListForGuest;
            }

            if (_Request.Get<bool>("allowGuestVisitForum_aplyallnode", Method.Post, false))
            {
                item.AllowGuestVisitForum = forumSetItem.AllowGuestVisitForum;
            }

            if (_Request.Get<bool>("SellThreadDays_aplyallnode", Method.Post, false))
            {
                item.SellThreadDays = forumSetItem.SellThreadDays;
            }
            if (_Request.Get<bool>("SellAttachmentDays_aplyallnode", Method.Post, false))
            {
                item.SellAttachmentDays = forumSetItem.SellAttachmentDays;
            }

            if (new ExceptableItem_bool().AplyAllNode("VisitForum"))
                item.VisitForum = forumSetItem.VisitForum;

            if (new ExceptableItem_bool().AplyAllNode("DisplayInList"))
                item.DisplayInList = forumSetItem.DisplayInList;

            if (_Request.Get<bool>("ReplyReturnThreadLastPage_aplyallnode", Method.Post, false))
            {
                item.ReplyReturnThreadLastPage = forumSetItem.ReplyReturnThreadLastPage;
            }

            if (_Request.Get<bool>("ThreadSortField_aplyallnode", Method.Post, false))
            {
                item.DefaultThreadSortField = forumSetItem.DefaultThreadSortField;
            }
        }

        private ForumSettingItem m_ForumSetting;
        protected ForumSettingItem ForumSetting
        {
            get
            {
                if(m_ForumSetting == null)
                    m_ForumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(ForumID);

                return m_ForumSetting;
            }
        }

        private Forum m_ForumSettingForum;
        protected Forum ForumSettingForum
        {
            get
            {
                if (m_ForumSettingForum == null)
                    m_ForumSettingForum = ForumBO.Instance.GetForum(ForumSetting.ForumID);
                return m_ForumSettingForum;
            }
        }

        private Forum m_Forum;
        protected Forum Forum
        {
            get
            {
                if (m_Forum == null)
                    m_Forum = ForumBO.Instance.GetForum(ForumID);
                return m_Forum;
            }
        }

        protected int ForumID
        {
            get
            {
                return int.Parse(Parameters["forumID"].ToString());
            }
        }
 

        private RoleCollection m_RoleList;
        /// <summary>
        /// 用户组
        /// </summary>
        protected RoleCollection RoleList
        {
            get
            {
                if (m_RoleList == null)
                {
                    m_RoleList = AllSettings.Current.RoleSettings.GetRoles(Role.Guests, Role.Everyone, Role.Users);
                }
                return m_RoleList;
            }
        }


        protected long GetTimeVale(long seconds)
        {
            TimeUnit timeUnit;
            return DateTimeUtil.FormatSecond(seconds, out timeUnit);
        }
        protected TimeUnit GetTimeUnit(long seconds)
        {
            TimeUnit timeUnit;
            DateTimeUtil.FormatSecond(seconds, out timeUnit);

            return timeUnit;
        }
    }
}