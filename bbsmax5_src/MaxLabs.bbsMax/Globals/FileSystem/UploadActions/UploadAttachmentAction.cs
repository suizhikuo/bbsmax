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
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using System.Collections.Specialized;

namespace MaxLabs.bbsMax.FileSystem
{
    /// <summary>
    /// 上传/下载附件的Action
    /// </summary>
    public class UploadAttachmentAction : FileActionBase
    {
        public override FileActionBase CreateInstance()
        {
            return new UploadAttachmentAction();
        }

        public override string Name
        {
            get { return "attach"; }
        }

        #region 上传文件

        bool m_CheckFileSize = true;

        private const string Key_TodayTotalUsedFileSize = "TodayTotalUsedFileSize";
        public override bool BeforeUpload(HttpContext context, string fileName, string serverFilePath, NameValueCollection queryString, ref object customResult)
        {
            Forum forum = GetForum(context);
            if (forum == null)
                return false;

            AuthUser operatorUser = User.Current;

            if (operatorUser.UserID == 0)
            {
                WebEngine.Context.ThrowError<NoPermissionCreateAttachmentError>(new NoPermissionCreateAttachmentError(forum.ForumID));
                return false;
            }

            ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forum.ForumID);

            if (false == forumSetting.AllowAttachment[operatorUser])
            {
                WebEngine.Context.ThrowError<NoPermissionCreateAttachmentError>(new NoPermissionCreateAttachmentError(forum.ForumID));
                return false;
            }

            int usedAttachmentCount;
            long totalUsedFileSize;

            PostBOV5.Instance.GetUserTodayAttachmentInfo(operatorUser.UserID, null, out usedAttachmentCount, out totalUsedFileSize);

            PageCacheUtil.Set(Key_TodayTotalUsedFileSize,totalUsedFileSize);

            int maxCountInDay = AllSettings.Current.BbsSettings.MaxAttachmentCountInDay[operatorUser];
            if (maxCountInDay == 0)
                maxCountInDay = int.MaxValue;
            int count = maxCountInDay - usedAttachmentCount;

            if (count < 1)
            {
                WebEngine.Context.ThrowError<OverTodayAlowAttachmentCountError>(new OverTodayAlowAttachmentCountError(maxCountInDay, 0, 1));
                return false;
            }

            int index = fileName.LastIndexOf('.') + 1;
            string fileType = fileName.Substring(index, fileName.Length - index);

            if (false == forumSetting.AllowFileExtensions[operatorUser].Contains(fileType))
            {
                WebEngine.Context.ThrowError<NotAllowAttachmentFileTypeError>(new NotAllowAttachmentFileTypeError(forum.ForumID,fileType));
                return false;
            }

            if (m_CheckFileSize == true)
            {
                long fileSize = 0;

                if (long.TryParse(queryString["filesize"], out fileSize))
                    return CheckFileSize(context, fileName, fileSize);
                else
                    return false;
            }

            return true;
        }

        public Forum GetForum(HttpContext context)
        {
            int forumID = 0;
            try
            {
                forumID = int.Parse(context.Request.QueryString["ForumID"].Trim());
            }
            catch
            {
                WebEngine.Context.ThrowError<InvalidParamError>(new InvalidParamError("forumID"));
                return null;
            }

            Forum forum = ForumBO.Instance.GetForum(forumID);
            if (forum == null)
            {
                WebEngine.Context.ThrowError<InvalidParamError>(new InvalidParamError("forumID"));
                return null;
            }
            return forum;
        }

        public override bool Uploading(HttpContext context, string fileName, string serverFilePath, long fileSize, long uploadedSize, ref object customResult)
        {
            if (m_CheckFileSize == true)
            {
                m_CheckFileSize = false;
                return CheckFileSize(context, fileName, fileSize);
            }
            return true;
        }

        private bool CheckFileSize(HttpContext context, string fileName, long fileSize)
        {
            Forum forum = GetForum(context);
         
            if (forum == null)
                return false;

            AuthUser operatorUser = User.Current;

            ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forum.ForumID);

            long maxSignleSize = forumSetting.MaxSignleAttachmentSize[operatorUser];
            if (maxSignleSize == 0)
                maxSignleSize = long.MaxValue;

            if (fileSize > maxSignleSize)
            {
                WebEngine.Context.ThrowError<OverAttachmentMaxSingleFileSizeError>(new OverAttachmentMaxSingleFileSizeError(forum.ForumID, maxSignleSize, fileName, fileSize));
                return false;
            }

            long todayFileSize;
            if (!PageCacheUtil.TryGetValue<long>(Key_TodayTotalUsedFileSize, out todayFileSize))
            {
                int todayCount;
                PostBOV5.Instance.GetUserTodayAttachmentInfo(operatorUser.UserID, null, out todayCount, out todayFileSize);
            }

            long maxTotalAttachmentsSizeInDay = AllSettings.Current.BbsSettings.MaxTotalAttachmentsSizeInDay[operatorUser];
            if (maxTotalAttachmentsSizeInDay == 0)
                maxTotalAttachmentsSizeInDay = long.MaxValue;

            if (todayFileSize + fileSize > maxTotalAttachmentsSizeInDay)
            {
                WebEngine.Context.ThrowError<OverTodayAlowAttachmentFileSizeError>(new OverTodayAlowAttachmentFileSizeError("OverTodayAlowAttachmentFileSizeError", maxTotalAttachmentsSizeInDay, maxTotalAttachmentsSizeInDay - todayFileSize, fileSize));
                return false;
            }

            return true;
        }

        public override bool AfterUpload(HttpContext context, string fileName, string serverFilePath, long filesize, int tempUploadFileID, string md5, NameValueCollection queryString, ref object customResult)
        {
            return true;
        }

        #endregion

        #region 下载文件

        private int attachmentID;
        //private string mode;
        OutputFileMode outputMode;
        private Attachment attachment;
        private AuthUser operatorUser;
        private int userID;
        private string filePath = string.Empty;

        private bool isMedia = false;
        public override bool Downloading(HttpContext context)
        {

            if (context.Request.QueryString["mode"] != null)
            {
                if (StringUtil.EqualsIgnoreCase(context.Request.QueryString["mode"], "image"))
                {
                    outputMode = OutputFileMode.Inline;
                }
                else
                {
                    outputMode = OutputFileMode.Attachment;

                    if (StringUtil.EqualsIgnoreCase(context.Request.QueryString["mode"], "media"))
                        isMedia = true;
                }

            }
            else
            {
                outputMode = OutputFileMode.Attachment;
            }

            operatorUser = User.Current;
            userID = operatorUser.UserID; //UserBO.Instance.GetCurrentUserID();

            //====处理3.0的附件============================================

            if (context.Request.QueryString["v"] == "3")
            {
                ProcessV30(context);
                return true;
            }

            //===================================================

            //预览或编辑器里显示 从网络硬盘里插入的附件
            if (context.Request.QueryString["diskfileID"] != null)
            {
                ProcessDiskFile(context);
                return true;
            }

            if (context.Request.QueryString["ID"] != null)
            {
                try
                {
                    attachmentID = int.Parse(context.Request.QueryString["ID"].Trim());
                }
                catch
                {
                    Context.ThrowError<InvalidParamError>(new InvalidParamError("ID"));
                    return false;
                    //Bbs3Globals.ShowError("error", "参数错误!", 0);
                }
            }
            else
            {
                Context.ThrowError<InvalidParamError>(new InvalidParamError("ID"));
                return false;
                //Bbs3Globals.ShowError("error", "参数错误!", 0);
            }

            //附件ID小于0，表示这是一个发帖时使用的临时文件
            if (attachmentID < 0)
            {
                if (isMedia)
                {
                    TempUploadFile tempFile = FileManager.GetUserTempUploadFile(userID, 0 - attachmentID);
                    if (tempFile == null)
                        return false;

                    int index = tempFile.FileName.LastIndexOf('.');
                    string fileType = tempFile.FileName.Substring(index, tempFile.FileName.Length - index);
                    ProcessMedia(context, fileType);
                }
                else if (false == OutputTempFile(context, userID, 0 - attachmentID, outputMode))
                {
                    ShowErrorMessage(context, "文件不存在,可能长时间没有发表导致被系统清理！", "临时文件不存在.gif");
                }
            }
            //附件ID大于0，表示这是一个真实的附件
            else
            {
                attachment = PostBOV5.Instance.GetAttachment(attachmentID, outputMode == OutputFileMode.Attachment);

                if (attachment == null)
                {
                    if (isMedia)
                        return false;
                    ShowErrorMessage(context, "该附件不存在,可能被移动或被删除！", "文件不存在.gif");
                    return false;
                }

                if (isMedia)
                {
                    ProcessMedia(context, "." + attachment.FileType);
                }

                string fileID = attachment.FileID;



                //处理3.0的 #attach:id#
                
                if (context.Request.QueryString["m"] != null)
                {
                    if (context.Request.QueryString["m"].ToLower() == "i" &&  MaxLabs.bbsMax.Ubb.PostUbbParserV5.isImage(attachment.FileType))
                    {
                        outputMode = OutputFileMode.Inline;
                    }
                }

                switch (Action(context))
                {
                    //case "buy":
                    //    ProcessBuy(context,fileID,null);
                    //    break;
                    default:
                        ProcessDownload(context, fileID);
                        //context.Response.End();
                        break;
                }
            }

            return true;
        }

        private string Action(HttpContext context)
        {
            string action;
            if (context.Request.QueryString["ac"] != null)
            {
                action = context.Request.QueryString["ac"].ToLower();
            }
            else
                action = "download";

            return action;
        }

        //判断有没有购买过
        private bool CheckPermission(ForumSettingItem forumSettting)
        {
            //是否已经买过了
            if (attachment.UserID != userID
                && attachment.Price != 0
                && !attachment.IsOverSellDays(forumSettting)
                && !attachment.IsBuyed(operatorUser))
            {
                return false;
            }
            return true;
        }

        protected bool AlwaysViewContents(Forum forum, int posterUserID)
        {
            //if (forum.Permission.AlwaysViewContents && forum.AllowManageOtherUser(posterUserID))
            if (AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forum.ForumID).Can(User.Current,ForumPermissionSetNode.Action.AlwaysViewContents))
                return true;
            else
                return false;
        }

        private void ProcessDownload(HttpContext context,string fileID)
        {

            PostV5 post = PostBOV5.Instance.GetPost(attachment.PostID, false);
            Forum forum = ForumBO.Instance.GetForum(post.ForumID);

            ForumPermissionSetNode permission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(post.ForumID);

            //if (!forum.Permission.ViewAttachment && attachment.UserID != userID)
            if (false == permission.Can(userID, ForumPermissionSetNode.Action.ViewAttachment) && attachment.UserID != userID)
            {
                ShowErrorMessage(context, "您没有权限下载附件！", "您没有权限下载附件.gif");
                return;
            }

            if (!AlwaysViewContents(forum, post.UserID) && attachment.UserID != userID)//检查是否是出售的帖子
            {
                AuthUser user = User.Current;
                bool isBuy = false;
                if (user.BuyedThreads.TryGetValue(post.ThreadID, out isBuy))//缓存中获取
                {
                }
                if (!isBuy)
                {
                    BasicThread thread = PostBOV5.Instance.GetThread(post.ThreadID);
                    if (thread.Price > 0)
                    {
                        //Post firstPost = PostManager.GetThreadFirstPost(thread.ThreadID);
                        //if (firstPost != null && firstPost.PostID == post.PostID)//附件所在的帖子是主题的内容
                        if(post.PostType == MaxLabs.bbsMax.Enums.PostType.ThreadContent)
                        {
                            //if (!firstPost.FreeAttachmentIDs.Contains(attachment.AttachmentID))//attachment.DiskFileID))//不在[free]标签中
                            if(false == post.IsFreeAttachmentID(attachment.AttachmentID))
                            {
                                if (!thread.IsOverSellDays(forum.ForumSetting) && !thread.IsBuyed(user))
                                {
                                    Context.ThrowError<CustomError>(new CustomError("error", "该附件所在的帖子需要购买,您还没有购买该帖子,没有权限下载附件！"));
                                    //Bbs3Globals.ShowError("error", "该附件所在的帖子需要购买,您还没有购买该帖子没有权限下载附件！", 0);
                                    return;
                                }
                            }
                        }
                    }
                }

            }

            if (CheckPermission(forum.ForumSetting) || AlwaysViewContents(forum, attachment.UserID))
            {
                //if (!IOUtil.DownloadFile(context.Response, context.Request, filePath, attachment.FileName, true, false))
                if(false == OutputFileByID(context,fileID,attachment.FileName,attachment.FileType,outputMode))
                {
                    ShowErrorMessage(context, "该附件不存在,可能被移动或被删除！", "文件不存在.gif");
                    return;
                }

            }
            else
            {
                ShowErrorMessage(context,  "你还没有购买此附件，不能下载！", "needpay.gif");
                return;
            }

            context.Response.End();
        }

        private void ProcessDownload3(HttpContext context, BasicThread thread, PostV5 post,int diskFileID)
        {
            Forum forum = ForumBO.Instance.GetForum(post.ForumID);
            ForumPermissionSetNode permission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(post.ForumID);

            if (false == permission.Can(userID,ForumPermissionSetNode.Action.ViewAttachment) && attachment.UserID != userID)
            {
                ShowErrorMessage(context, "您没有权限下载附件！", "您没有权限下载附件.gif");
                return;
            }

            if (!AlwaysViewContents(forum, post.UserID) && attachment.UserID != userID)//检查是否是出售的帖子
            {
                AuthUser user = User.Current;
                bool isBuy = false;
                if (user.BuyedThreads.TryGetValue(post.ThreadID, out isBuy))//缓存中获取
                {
                }
                if (!isBuy)
                {
                    if (thread.Price > 0)
                    {
                        if (post.PostType == MaxLabs.bbsMax.Enums.PostType.ThreadContent)
                        {
                            if (false == post.IsFreeDiskFileID(diskFileID) && false == post.IsFreeAttachmentID(attachment.AttachmentID))
                            {
                                if (!thread.IsOverSellDays(forum.ForumSetting) && !thread.IsBuyed(user))
                                {
                                    Context.ThrowError<CustomError>(new CustomError("error", "该附件所在的帖子需要购买,您还没有购买该帖子没有权限下载附件！"));
                                    return;
                                    //Bbs3Globals.ShowError("error", "该附件所在的帖子需要购买,您还没有购买该帖子没有权限下载附件！", 0);
                                }
                            }
                        }
                    }
                }

            }

            if (CheckPermission(forum.ForumSetting) || AlwaysViewContents(forum, attachment.UserID))
            {
                if (false == OutputFileByID(context, attachment.FileID, attachment.FileName, attachment.FileType, outputMode))
                {
                    ShowErrorMessage(context, "该附件不存在,可能被移动或被删除！", "文件不存在.gif");
                }

            }
            else
            {
                ShowErrorMessage(context, "你还没有购买此附件，不能下载！", "needpay.gif");
            }
        }

        private void ProcessV30(HttpContext context)
        {
            int diskFileID, postID;

            if (int.TryParse(context.Request.QueryString["DiskFileID"], out diskFileID) && int.TryParse(context.Request.QueryString["postID"], out postID))
            {
                PostV5 post;
                BasicThread thread;
                PostBOV5.Instance.GetAttachment(diskFileID, postID, out attachment, out post, out thread);

                if (attachment == null || post == null || thread == null)
                {
                    Context.ThrowError<CustomError>(new CustomError("error", "该附件不存在,可能被移动或被删除！"));
                    return;
                }

                //if (Action(context) == "buy")
                //    ProcessBuy(context, Guid.Empty, post);
                //else
                ProcessDownload3(context, thread, post, diskFileID);
            }
        }

        private void ProcessDiskFile(HttpContext context)
        {
            int diskFileID;
            if (int.TryParse(context.Request.QueryString["DiskFileID"], out diskFileID))
            {
                DiskFile diskFile = DiskBO.Instance.GetDiskFile(userID, diskFileID);

                if (diskFile == null)
                {
                    Context.ThrowError<CustomError>(new CustomError("error", "该附件不存在,可能被移动或被删除！"));
                    return;
                }
                else if (isMedia)
                {
                    ProcessMedia(context, diskFile.ExtensionName);
                }
                else if (false == OutputFileByID(context, diskFile.FileID, diskFile.FileName, diskFile.ExtensionName, outputMode))
                {
                    ShowErrorMessage(context, "该附件不存在,可能被移动或被删除！", "文件不存在.gif");
                }
            }
        }


        private void ProcessMedia(HttpContext context,string mediaType)
        {
            mediaType = mediaType.ToLower();
            string imgFile;

            switch (mediaType)
            {
                case ".mp3":
                case ".wma":
                case ".wav":
                case ".mid":
                    imgFile = "editor_audio.gif";
                    break;

                case ".ra":
                case ".rm":
                case ".rmvb":

                    imgFile = "editor_real.gif";
                    break;

                case ".wmv":
                case ".avi":
                    imgFile = "editor_video.gif";
                    break;

                case ".flv":
                case ".swf":
                    imgFile = "editor_flash.gif";
                    break;

                default:
                    imgFile = null;
                    break;
            }

            if (imgFile != null)
            {
                imgFile = UrlUtil.JoinUrl(Globals.ApplicationPath, "max-assets/images/" + imgFile);

                OutputFile(context, imgFile, "", ".gif", OutputFileMode.Inline);
            }
        }

        private void ShowErrorMessage(HttpContext context, string message, string messageImage)
        {


            if (outputMode == OutputFileMode.Inline)
                IOUtil.DownloadFile(context.Response, context.Request, messageImage, "alert.gif", true, true);
            else
                Context.ThrowError<CustomError>(new CustomError("error", message));
            //Bbs3Globals.ShowError("error", message, 0);
        }

        #endregion
    }
}