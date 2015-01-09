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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.Common;
using System.Collections;
using MaxLabs.bbsMax.RegExp;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Ubb
{
    /// <summary>
    /// 论坛帖子UBB处理器
    /// </summary>
    public class PostUbbParserV5 : UbbParser
    {
        public PostUbbParserV5()
        { 
        }

        public PostUbbParserV5(User postUser, int forumID, bool useHTML, bool useUbb, bool processQuoteTag)
        {

            ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID);

            if (processQuoteTag)
                AddTagHandler(new QUOTE());

            if (useHTML == false && useUbb)
            {
                AddTagHandler(new B());
                AddTagHandler(new I());
                AddTagHandler(new S());
                AddTagHandler(new U());
                AddTagHandler(new ALIGN());
                AddTagHandler(new FONT());
                AddTagHandler(new SIZE());
                AddTagHandler(new COLOR());
                AddTagHandler(new BGCOLOR());
                AddTagHandler(new EMAIL());

                bool allowUrl = forumSetting.CreatePostAllowUrlTag.GetValue(postUser);

                //if (forumSetting.CreatePostAllowUrlTag.GetValue(user))
                AddTagHandler(new URL(allowUrl));

                AddTagHandler(new SUB());
                AddTagHandler(new SUP());
                AddTagHandler(new INDENT());
                AddTagHandler(new LIST());
                AddTagHandler(new BR());
                AddTagHandler(new CODE());

                if (forumSetting.CreatePostAllowTableTag.GetValue(postUser)) AddTagHandler(new TABLE());


                AddTagHandler(new IMG(forumSetting.CreatePostAllowImageTag.GetValue(postUser), allowUrl));
                AddTagHandler(new FLASH(forumSetting.CreatePostAllowFlashTag.GetValue(postUser), allowUrl));

                bool allowAudio = forumSetting.CreatePostAllowAudioTag.GetValue(postUser);

                AddTagHandler(new WMA(allowAudio, allowUrl));
                AddTagHandler(new MP3(allowAudio, allowUrl));
                AddTagHandler(new RA(allowAudio, allowUrl));

                bool allowVideo = forumSetting.CreatePostAllowVideoTag.GetValue(postUser);

                AddTagHandler(new WMV(allowVideo, allowUrl));
                AddTagHandler(new RM(allowVideo, allowUrl));
            }

            //使用HTML则不编码
            EncodeHtml = !useHTML;

        }

        public static Regex regex_AllAttach = new Regex(@"\[attach[imgeda=\d+,]*\](?<id>\d+)\[/attach[imgeda]*\]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
        


        #region 显示的时候 处理

        private static AttachRegex regex_allAttach = new AttachRegex();
        private static Regex regex_fileInfo = new Regex(@"\(#fileInfo:(?<id>\d+)#\)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex regex_attachImgV30 = new Regex(@"<img\ssrc=""#(?<type>attach|file):(?<id>\d+)#", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex regex_attachV30 = new Regex(@"#(?<type>attach|file):(?<id>\d+)#", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);


        private static string GetNopermissionStyle(string message)
        {
            return string.Concat("<div style=\"overflow:hidden;zoom:1;\"><div style=\"float:left;padding:30px 20px 30px 60px;border:1px solid #a2d1e5;background:#fff url(", Globals.AppRoot, "/max-assets/images/nopermi_icon.gif) no-repeat 20px 50%;\">", message, "</div></div>");
        }
        private static string GetNoPermissonfileStyle(string fileName, bool isGuest)
        {
            return string.Concat(@"<div class=""post-file"">
                                        <a class=""file-name"">", fileName, @"</a>
                                        <span class=""file-status"">(", isGuest ? "您是游客" : "", @"您没有权限下载)</span>
                                   </div>");

            //return string.Concat("<a class=\"downloadfiles\">", fileName, "</a> (", isGuest ? "您是游客" : "", "您没有权限下载)");
        }
        private static string GetMustBuyFileLink(string fileName)
        {
            return string.Concat(@"<div class=""post-file"">
                                        <a class=""file-name"">", fileName, @"</a>
                                        <span class=""file-status"">(请先购买)</span>
                                   </div>");
            //return string.Concat("<a class=\"downloadfiles\">", fileName, "</a> (请先购买)");
        }

        private static string GetAttachimgLinkWithSize(string url, string width, string height)
        {
            return string.Concat("<img src=\"", url, "\" width=\"", width, "\" height=\"", height, "\" alt=\"\" onload=\"ImageLoaded(this)\" onerror=\"ImageError(this)\" />");
        }

        private static string GetAttachimgLink(string url)
        {
            return string.Concat("<img src=\"", url, "\" alt=\"\" onload=\"ImageLoaded(this)\" onerror=\"ImageError(this)\" />");
        }

        private static string GetAttachLink(string url, string fileName, string fileSize, int totalDowanloads)
        {
            return string.Concat(@"<div class=""post-file"">
                                        <a class=""file-name"" href=""", url, @""">", fileName, @"</a>
                                         <span class=""file-status"">(文件大小: ", fileSize, @", 下载次数: ", totalDowanloads.ToString(), @")</span>
                                   </div>");
            //return string.Concat("<br /><a class=\"downloadfiles\" href=\"", url, "\" title=\"\" target=\"_blank\">", fileName, "</a> <span class=\"filesize gray\">(文件大小:", fileSize, ", 下载次数:", totalDowanloads.ToString(), ")</span> ");
        }


        private static string OnMatchAllAttach(Match match, AuthUser operatorUser, User postUser, int forumID, AttachmentCollection attachments, ForumSettingItem forumSetting
            , ref bool? hasViewAttachPermission, ref bool? canAlwaysViewContents, ref bool? allowImageTag, ref bool? allowAudioTag, ref bool? allowVideoTag, ref bool? allowFlashTag)
        {
            if (match.Success == false)
                return match.Value;

            string type = match.Groups["type"].Value;

            if (hasViewAttachPermission == null)
                hasViewAttachPermission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forumID).Can(operatorUser, ForumPermissionSetNode.Action.ViewAttachment);

            if (!hasViewAttachPermission.Value)
            {
                string message;
                if (operatorUser.UserID == 0)
                {
                    message = "您是游客";
                }
                else
                {
                    message = string.Empty;
                }

                if (StringUtil.EqualsIgnoreCase(type, "img"))
                    return GetNopermissionStyle(string.Concat(message, "您没有权限查看该图片"));
                else if (StringUtil.EqualsIgnoreCase(type, "media"))
                    return GetNopermissionStyle(string.Concat(message, "您没有权限查看该多媒体"));
                else
                {
                    int attachID = int.Parse(match.Groups["id"].Value);
                    Attachment attachment = attachments.GetValue(attachID);
                    if (attachment != null)
                        return GetNoPermissonfileStyle(attachment.FileName, operatorUser.UserID == 0);
                    else
                        return match.Value;
                }
            }
            else
            {
                int attachID = int.Parse(match.Groups["id"].Value);
                Attachment attachment = attachments.GetValue(attachID);

                if (attachment == null)
                    return match.Value;

                if (canAlwaysViewContents == null)
                {
                    ForumPermissionSetNode forumPermission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forumID);
                    canAlwaysViewContents = forumPermission.Can(operatorUser, ForumPermissionSetNode.Action.AlwaysViewContents);
                }

                if (StringUtil.EqualsIgnoreCase(type, "img"))
                {
                    if (allowImageTag == null)
                    {
                        allowImageTag = forumSetting.CreatePostAllowImageTag.GetValue(postUser);
                    }
                    if (allowImageTag.Value)
                    {
                        if (attachment.Price == 0 || attachment.UserID == operatorUser.UserID || canAlwaysViewContents.Value || attachment.IsBuyed(operatorUser) || attachment.IsOverSellDays(forumSetting))
                        {
                            //string info = string.Concat("<br /><img src=\"", attachment.FileIcon, "\" alt=\"\" /><a href=\"", BbsUrlHelper.GetAttachmentUrl(attachment.AttachmentID), "\">", attachment.FileName, "</a>  <span class=\"filesize gray\">(大小:", attachment.FileSizeFormat, "    下载次数:", attachment.TotalDownloads.ToString(), ")</span><br />");

                            string[] param = StringUtil.Split(match.Groups["param"].Value);
                            string width, height;
                            if (param.Length > 1)
                            {
                                width = param[0];
                                height = param[1];
                            }
                            else
                            {
                                width = string.Empty;
                                height = string.Empty;
                            }
                            return GetImageUrl(attachment.AttachmentID, false, width, height);
                        }
                        else
                        {
                            return string.Concat("<br /><img src=\"", attachment.FileIcon, "\" alt=\"\" />", attachment.FileName, " <span class=\"filesize gray\">(大小:", attachment.FileSizeFormat, "    下载次数:" + attachment.TotalDownloads.ToString(), ")</span><br />", GetNopermissionStyle("您需要购买后才能查看该图片"));
                        }
                    }
                    else
                        return ProcessAttach(attachment, operatorUser, forumSetting, canAlwaysViewContents.Value);
                }
                else if (StringUtil.EqualsIgnoreCase(type, "media"))
                {
                    if (attachment.Price == 0 || canAlwaysViewContents.Value || attachment.UserID == operatorUser.UserID || attachment.IsBuyed(operatorUser)
                        || attachment.IsOverSellDays(forumSetting))
                    {
                        string[] param = StringUtil.Split(match.Groups["param"].Value);
                        string width, height;
                        bool auto = false;
                        if (param.Length > 1)
                        {
                            width = param[0];
                            height = param[1];
                            if (param.Length > 2)
                            {
                                if (string.Compare(param[2], "1") == 0)
                                {
                                    auto = true;
                                }
                            }
                        }
                        else
                        {
                            width = string.Empty;
                            height = string.Empty;
                        }

                        //return string.Concat("<br /><img src=\"", attachment.FileIcon, "\" alt=\"\" />", "<a href=\"", BbsUrlHelper.GetAttachmentUrl(attachment.AttachmentID), "\">", attachment.FileName
                        //    , "</a>  <span class=\"filesize gray\">(大小:", attachment.FileSizeFormat, "    下载次数:", attachment.TotalDownloads, ")</span><br />"
                        //    , GetMediaContent(attachment, false, width, height, auto, forumSetting, user, ref allowAudioTag, ref allowVideoTag, ref allowFlashTag));

                        return GetMediaContent(attachment, false, width, height, auto, forumSetting, operatorUser, ref allowAudioTag, ref allowVideoTag, ref allowFlashTag);

                    }
                    else
                    {
                        return string.Concat("<br /><img src=\"", attachment.FileIcon, "\" alt=\"\" />", attachment.FileName, "<span class=\"filesize gray\">(大小:", attachment.FileSizeFormat
                            , "    下载次数:", attachment.TotalDownloads, ")</span><br />", GetNopermissionStyle("您需要购买后才能查看该多媒体"));
                    }
                }
                else
                {
                    return ProcessAttach(attachment, operatorUser, forumSetting, canAlwaysViewContents.Value);
                }
            }
        }

        private static string ProcessAttach(Attachment attachment, AuthUser operatorUser, ForumSettingItem forumSetting, bool canAlwaysViewContents)
        {
            if (attachment.Price == 0 || canAlwaysViewContents || attachment.UserID == operatorUser.UserID || attachment.IsBuyed(operatorUser) || attachment.IsOverSellDays(forumSetting))
            {
                return GetAttachUrl(attachment);
            }
            else//附件没购买，不显示。
            {
                return GetMustBuyFileLink(attachment.FileName);
            }
        }

        public static string ParseWhenDisplay(int posterUserID, int forumID, int postID, string content, bool allowHtmlForV2, bool allowMaxcodeForV2, bool isV5_0, AttachmentCollection attachmemts)
        {
            content = StringUtil.EncodeInnerUrl(content);

            User postUser = UserBO.Instance.GetUser(posterUserID, GetUserOption.WithAll);

            if (attachmemts != null && attachmemts.Count > 0)
            {
                ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID);
                AuthUser my = User.Current;

                bool? hasViewAttachPermission = null, canAlwaysViewContents = null, allowImageTag = null
                    , allowAudioTag = null, allowVideoTag = null, allowFlashTag = null;
                content = regex_allAttach.Replace(content, delegate(Match match)
                {
                    return OnMatchAllAttach(match, my, postUser, forumID, attachmemts, forumSetting, ref hasViewAttachPermission
                        , ref canAlwaysViewContents, ref allowImageTag, ref allowAudioTag, ref allowVideoTag, ref allowFlashTag);
                });

                if (isV5_0 == false)
                {
                    content = ReplaceV30AttachTag(content, postID);

                    content = regex_fileInfo.Replace(content, string.Empty);
                }

            }

            if (isV5_0 == false)
                content = new PostUbbParserV5(postUser, forumID, true, allowMaxcodeForV2, false).UbbToHtml(content);

            return UrlUtil.ReplaceRootVar(content);
        }





        private static string ReplaceV30AttachTag(string content, int postID)
        {
            content = regex_attachImgV30.Replace(content, delegate(Match match)
            {
                return OnMatchV30AttachImg(match, postID);
            });

            content = regex_attachV30.Replace(content, delegate(Match match)
            {
                return OnMatchV30Attach(match, postID);
            });

            return content;
        }

        private static string OnMatchV30AttachImg(Match match, int postID)
        {
            if (match.Success == false)
                return match.Value;

            if (StringUtil.EqualsIgnoreCase(match.Groups["type"].Value, "attach"))
                return string.Concat("<img src=\"", BbsUrlHelper.GetV30AttachmentUrl(match.Groups["id"].Value, true));
            else
                return string.Concat("<img src=\"", BbsUrlHelper.GetV30AttachmentUrl(match.Groups["id"].Value, postID, true));
        }
        private static string OnMatchV30Attach(Match match, int postID)
        {
            if (match.Success == false)
                return match.Value;

            if (StringUtil.EqualsIgnoreCase(match.Groups["type"].Value, "attach"))
                return BbsUrlHelper.GetV30AttachmentUrl(match.Groups["id"].Value, false);
            else
                return BbsUrlHelper.GetV30AttachmentUrl(match.Groups["id"].Value, postID, false);
        }





        private static string GetAttachUrl(Attachment attachment)
        {
            return GetAttachUrl(attachment, false);
        }
        private static string GetAttachUrl(Attachment attachment, bool isDiskFile)
        {
            string url;
            if (isDiskFile)
                url = BbsUrlHelper.GetDiskAttachmentUrl(attachment.DiskFileID);
            else
                url = BbsUrlHelper.GetAttachmentUrl(attachment.AttachmentID);

            return GetAttachLink(url, attachment.FileName, ConvertUtil.FormatSize(attachment.FileSize), attachment.TotalDownloads);
        }

        private static string GetImageUrl(object attachmentID, bool addRandomNumber, object width, object height)
        {
            return GetImageUrl(attachmentID, addRandomNumber, width, height, false);
        }
        private static string GetImageUrl(object attachmentID, bool addRandomNumber, object width, object height, bool isDiskFile)
        {
            if (width.ToString() != string.Empty && width.ToString() != string.Empty)
            {
                string url;
                if (isDiskFile)
                    url = BbsUrlHelper.GetDiskImageAttachmentUrl(attachmentID);
                else
                    url = BbsUrlHelper.GetImageAttachmentUrl(attachmentID, addRandomNumber);

                return GetAttachimgLinkWithSize(url, width.ToString(), height.ToString());
            }
            else
                return GetImageUrl(attachmentID, addRandomNumber, isDiskFile);
        }

        private static string GetImageUrl(object attachmentID, bool addRandomNumber)
        {
            return GetImageUrl(attachmentID, addRandomNumber, false);
        }
        private static string GetImageUrl(object attachmentID, bool addRandomNumber, bool isDiskFile)
        {
            string url;
            if (isDiskFile)
                url = BbsUrlHelper.GetDiskImageAttachmentUrl(attachmentID);
            else
                url = BbsUrlHelper.GetImageAttachmentUrl(attachmentID, addRandomNumber);
            return GetAttachimgLink(url);
        }


        public static string GetMediaContent(Attachment attachment, bool isDiskFile, object width, object height, bool auto
            , ForumSettingItem forumSetting, User user)
        {
            return GetMediaContent(attachment, isDiskFile, width, height, auto, forumSetting, user);
        }

        public static string GetMediaContent(Attachment attachment, bool isDiskFile, object width, object height, bool auto
            , ForumSettingItem forumSetting, User user, ref bool? allowAudioTag, ref bool? allowVideoTag, ref bool? allowFlashTag)
        {
            string url;
            if (isDiskFile)
                url = BbsUrlHelper.GetDiskImageAttachmentUrl(attachment.DiskFileID);
            else
                url = BbsUrlHelper.GetImageAttachmentUrl(attachment.AttachmentID, false);

            switch (attachment.FileType.ToLower())
            {
                case "mp3":
                case "wma":
                case "mid":
                case "wav":
                    if (allowAudioTag == null)
                        allowAudioTag = forumSetting.CreatePostAllowAudioTag.GetValue(user);
                    if (allowAudioTag.Value)
                    {
                        MP3 mp3 = new MP3(true, true);
                        if (width == null || height == null)
                        {
                            return mp3.BuildHtml(url);
                        }
                        else
                        {
                            return mp3.BuildHtml(width, height, auto, url);
                        }
                    }
                    break;
                case "wmv":
                case "avi":
                    if (allowVideoTag == null)
                        allowVideoTag = forumSetting.CreatePostAllowVideoTag.GetValue(user);
                    if (allowVideoTag.Value)
                    {
                        WMV wmv = new WMV(true, true);
                        if (width == null || height == null)
                        {
                            return wmv.BuildHtml(url);
                        }
                        else
                        {
                            return wmv.BuildHtml(width, height, auto, url);
                        }
                    }
                    break;
                case "ra":
                case "rm":
                case "rmvb":
                    if (allowVideoTag == null)
                        allowVideoTag = forumSetting.CreatePostAllowVideoTag.GetValue(user);
                    if (allowVideoTag.Value)
                    {
                        RM media = new RM(true, true);
                        if (width == null || height == null)
                        {
                            return media.BuildHtml(url);
                        }
                        else
                        {
                            return media.BuildHtml(width, height, auto, url);
                        }
                    }
                    break;
                case "flv":
                    if (allowVideoTag == null)
                        allowVideoTag = forumSetting.CreatePostAllowVideoTag.GetValue(user);
                    if (allowVideoTag.Value)
                    {
                        FLV media = new FLV(true, true);
                        if (width == null || height == null)
                        {
                            return media.BuildHtml(url);
                        }
                        else
                        {
                            return media.BuildHtml(width, height, auto, url);
                        }
                    }
                    break;
                case "swf":
                    if (allowFlashTag == null)
                        allowFlashTag = forumSetting.CreatePostAllowFlashTag.GetValue(user);
                    if (allowFlashTag.Value)
                    {
                        FLASH media = new FLASH(true, true);
                        if (width == null || height == null)
                        {
                            return media.BuildHtml(url);
                        }
                        else
                        {
                            return media.BuildHtml(width, height, auto, url);
                        }
                    }
                    break;
                default: break;
            }
            return GetAttachUrl(attachment, isDiskFile);
        }
        #endregion



        #region 保存的时候 处理
        //public static Regex regex_allAttach = new Regex(@"\[attach[imgeda=\d+,]*\](?<id>\d+)\[/attach[imgeda]*\]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// 保存的时候 处理UBB 处理表情 不处理[local]标签
        /// </summary>
        /// <param name="posterUserID"></param>
        /// <param name="userEmoticon"></param>
        /// <param name="forumID"></param>
        /// <param name="content"></param>
        /// <param name="allowHtml"></param>
        /// <param name="allowMaxcode3"></param>
        /// <returns></returns>
        public static string ParseWhenSave(int posterUserID, bool useEmoticon, int forumID, string content, bool allowHtml, bool allowMaxcode3, AttachmentCollection attachments)
        {
            User currentUser = User.Current;

            content = content.Replace("[/quote]\r", "[/quote]");
            content = content.Replace("[/quote]\n", "[/quote]");

            content = new PostUbbParserV5(UserBO.Instance.GetUser(posterUserID,GetUserOption.WithAll), forumID, allowHtml, allowMaxcode3, true).UbbToHtml(content);

            bool allowEmoticon = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID).CreatePostAllowEmoticon.GetValue(currentUser);

            if (useEmoticon && allowEmoticon)
            {
                content = EmoticonParser.ParseToHtml(posterUserID, content);
            }

            //attachments  同一个附件出现多次 只显示一个？

            return content;
        }

        #endregion


        #region 处理 [local]

        private static LocalAttachRegex regex_LocalAttach = new LocalAttachRegex();

         /// <summary>
        ///处理 [local] （帖子发完后 临时附件转为真的附件 再处理帖子内容的[local] ）
        /// </summary>
        /// <param name="content"></param>
        /// <param name="attachments">附件 </param>
        /// <param name="realAttachmentIDs">真实附件ID 与 attachments 一一对应</param>
        /// <returns></returns>
        public static string ParseLocalAttachTag(string content, AttachmentCollection attachments, List<int> realAttachmentIDs, Dictionary<string, int> fileIDs)
        {
            if (attachments != null && attachments.Count > 0)
            {
                content = regex_LocalAttach.Replace(content, delegate(Match match)
                {
                    return OnMatchLocalFile(match, null, null, attachments, realAttachmentIDs, fileIDs, false);
                });
            }

            return content;
        }

        /// <summary>
        /// 预览的时候 处理[local]标签
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="postUserID"></param>
        /// <param name="content"></param>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public static string ParsePreviewLocalAttachTag(int forumID, User postUser, string content, AttachmentCollection attachments)
        {
            if (attachments != null && attachments.Count > 0)
            {
                ForumSettingItem forumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(forumID);
                content = regex_LocalAttach.Replace(content, delegate(Match match)
                {
                    return OnMatchLocalFile(match, forumSetting, postUser, attachments, null, null, true);
                });
            }

            return content;
        }


        private static string GetAttachTag(int attachID)
        {
            return string.Concat("[attach]", attachID, "[/attach]");
        }
        private static string GetAttachImgTag(int attachID, string width, string height)
        {
            if (string.IsNullOrEmpty(width) || string.IsNullOrEmpty(height))
                return string.Concat("[attachimg]", attachID, "[/attachimg]");
            else
                return string.Concat("[attachimg=", width, ",", height, "]", attachID, "[/attachimg]");
        }
        private static string GetAttachMediaTag(int attachID, string width, string height, bool isAuto)
        {
            if (string.IsNullOrEmpty(width) || string.IsNullOrEmpty(height))
                return string.Concat("[attachmedia]", attachID, "[/attachmedia]");
            else
                return string.Concat("[attachmedia=", width, ",", height, ",", isAuto ? "1" : "0", "]", attachID, "[/attachmedia]");
        }

        private static string OnMatchLocalFile(Match match, ForumSettingItem forumSetting, User postUser, AttachmentCollection attachments, List<int> realAttachmentIDs, Dictionary<string, int> fileIDs, bool isReview)
        {
            if (match.Success == false)
                return match.Value;

            string atype = match.Groups["atype"].Value;
            string type = match.Groups["type"].Value;
            string param = match.Groups["param"].Value;

            bool isLocal = StringUtil.EqualsIgnoreCase(atype, "local");

            int id = int.Parse(match.Groups["id"].Value);

            if (type == string.Empty || StringUtil.EqualsIgnoreCase(type, "file"))//[local]12[/local] [diskfile]12[/diskfile]
            {
                int j = 0;
                for (int i = 0; i < attachments.Count; i++)
                {

                    if (isLocal&& attachments[i].AttachType == AttachType.TempAttach)
                    {
                        if (attachments[i].AttachmentID > 0)
                            continue;

                        if (realAttachmentIDs != null && j == realAttachmentIDs.Count)
                            return match.Value;

                        if (attachments[i].AttachmentID == -id)
                        {
                            if (isReview)
                                return GetAttachUrl(attachments[i]);
                            else
                                return GetAttachTag(realAttachmentIDs[j]);
                        }

                        j++;
                    }
                    else if(attachments[i].AttachType == AttachType.DiskFile)
                    {
                        if (attachments[i].DiskFileID == id)
                        {
                            if (isReview)
                                return GetAttachUrl(attachments[i]);
                            else
                            {
                                int attachmentID;
                                if (fileIDs.TryGetValue(attachments[i].FileID, out attachmentID))
                                {
                                    return GetAttachTag(attachmentID);
                                }
                                else
                                    return match.Value;
                            }
                        }
                    }

                }
            }
            else if (StringUtil.EqualsIgnoreCase(type, "img"))//[localimg]12[/localimg]
            {
                string[] p = StringUtil.Split(param);
                string width, height;
                if (p.Length > 1)
                {
                    width = p[0];
                    height = p[1];
                }
                else
                {
                    width = string.Empty;
                    height = string.Empty;
                }

                int j = 0;
                for (int i = 0; i < attachments.Count; i++)
                {

                    Attachment attachment = attachments[i];

                    if (isLocal && attachment.AttachType == AttachType.TempAttach)
                    {
                        if (attachment.AttachmentID > 0)
                            continue;
                        if (realAttachmentIDs != null && j == realAttachmentIDs.Count)
                            return match.Value;

                        if (attachment.AttachmentID == -id)
                        {
                            if (isImage(attachment.FileType))
                            {
                                if (isReview)
                                {
                                    if (forumSetting.CreatePostAllowImageTag.GetValue(postUser))
                                    {
                                        return GetImageUrl(attachment.AttachmentID, false, width, height, false);
                                    }
                                    else
                                    {
                                        return GetAttachUrl(attachment, false);
                                    } 
                                }
                                else
                                    return GetAttachImgTag(realAttachmentIDs[j], width, height);
                            }
                            else
                                return match.Value;
                        }

                        j++;
                    }
                    else if(attachment.AttachType == AttachType.DiskFile)
                    {
                        if (attachment.DiskFileID == id)
                        {
                            if (isImage(attachment.FileType))//确实是图片才处理
                            {
                                if (isReview)
                                {
                                    if (forumSetting.CreatePostAllowImageTag.GetValue(postUser))
                                    {
                                        return GetImageUrl(attachment.DiskFileID, false, width, height, true);
                                    }
                                    else
                                    {
                                        return GetAttachUrl(attachment, true);
                                    }
                                }
                                else
                                {
                                    int attachmentID;
                                    if (fileIDs.TryGetValue(attachment.FileID, out attachmentID))
                                    {

                                        return GetAttachImgTag(attachmentID, width, height);
                                    }
                                    else
                                        return match.Value;
                                }
                            }
                            else
                                return match.Value;
                        }
                    }
                }
            }
            else if (StringUtil.EqualsIgnoreCase(type, "media"))//[localmedia]12[/localmedia]
            {
                string[] p = StringUtil.Split(param);
                string width, height;
                bool auto = false;
                if (p.Length > 1)
                {
                    width = p[0];
                    height = p[1];
                    if (p.Length > 2)
                    {
                        if (string.Compare(p[2], "1") == 0)
                        {
                            auto = true;
                        }
                    }
                }
                else
                {
                    width = string.Empty;
                    height = string.Empty;
                }

                int j = 0;
                for (int i = 0; i < attachments.Count; i++)
                {

                    Attachment attachment = attachments[i];

                    if (isLocal && attachment.AttachType == AttachType.TempAttach)
                    {
                        if (attachment.AttachmentID > 0)
                            continue;

                        if (realAttachmentIDs != null && j == realAttachmentIDs.Count)
                            return match.Value;

                        if (attachment.AttachmentID == -id)
                        {
                            if (isMediaFile(attachment.FileType))
                            {
                                if (isReview)
                                    return GetMediaContent(attachment, false, width, height, auto, forumSetting, postUser);
                                else
                                    return GetAttachMediaTag(realAttachmentIDs[j], width, height, auto);
                            }
                            else
                                return match.Value;
                        }

                        j++;
                    }
                    else if(attachment.AttachType == AttachType.DiskFile)
                    {
                        if (attachment.DiskFileID == id)
                        {
                            if (isMediaFile(attachment.FileType))
                            {
                                if (isReview)
                                    return GetMediaContent(attachment, true, width, height, auto, forumSetting, postUser);
                                else
                                {
                                    int attachmentID;
                                    if (fileIDs.TryGetValue(attachment.FileID, out attachmentID))
                                    {
                                        return GetAttachMediaTag(attachmentID, width, height, auto);
                                    }
                                    else
                                        return match.Value;
                                }
                            }
                            else
                                return match.Value;
                        }
                    }
                }
            }

            return match.Value;
        }

        public static bool isImage(string fileType)
        {
            switch (fileType.ToLower())
            {
                case "jpg": 
                case "png":
                case "gif":
                case "bmp":
                case "jpeg": 
                    return true;
                default:
                    return false;
            }
        }
        public static bool isMediaFile(string fileType)
        {
            switch (fileType.ToLower())
            {
                case "mp3": 
                case "wma": 
                case "wav": 
                case "mid": 
                case "ra": 
                case "rm": 
                case "rmvb": 
                case "wmv": 
                case "avi": 
                case "flv": 
                case "swf": 
                    return true;
                default: 
                    return false;
            }
        }

        #endregion

        #region FormatRequotePost
        /// <summary>
        /// 引用的时候 把“[attach]12[attach]” 替换成 “12”
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ProcessQuote(string content)
        {
            return regex_allAttach.Replace(content, "$3");
            //content = regex_attach.Replace(content, "$1");
            //content = regex_allattachimg.Replace(content, "$1");
            //return content;
        }

        internal static readonly Regex regex_code = new Regex(@"<code>(?<code>(?is)(.*?|\s*?))</code>", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
        internal static readonly Regex regex_ubb = new Regex(@"<!--ubb-begin-->(?is)(.*?|\s*?)<!--ubb-end-->", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
        public static Regex regex_hide = new Regex(@"\[hide\](?is)(.*?|\s*?)\[/hide\]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex regex_faceShortcut = new Regex(@"emoticon="".*?""", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string FormatRequotePost(string content, PostV5 reply, BasicThread thread)
        {
            //引用的时候  用户自定义的表情 不再使用快捷方式显示 （会照成无法显示，因为发帖页 只列出了当前发帖者的表情）
            //TODO:管理员编辑别人的帖子时 也会出现同样的情况--
            content = regex_faceShortcut.Replace(content, "");

            //bool userEmotion = reply.EnableEmoticons;
            //if(userEmotion)
            //{
            //    ForumSettingItem setting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(reply.ForumID);
            //    userEmotion = setting.CreatePostAllowEmoticon.GetValue(reply.UserID);
            //}
            //content = EmoticonParser.ParseToHtml(reply.UserID, content, userEmotion, userEmotion);

            if (thread.Price > 0)
            {

                PostV5 post = PostBOV5.Instance.GetThreadFirstPost(thread.ThreadID, false);
                if (post != null && post.PostID == reply.PostID) //需要购买的帖子
                {
                    content = ProcessFreeWhenRequoteTag(content);
                }
            }
            content = content.Replace("{$root}", Globals.AppRoot);

            content = regex_ubb.Replace(content, delegate(Match m)
            {
                string temp = regex_code.Replace(m.Groups[1].Value, delegate(Match m2)
                {
                    return string.Concat("[code]", m2.Groups[1].Value, "[/code]");
                });

                return temp;
            });

            ForumSettingItem setting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(thread.ForumID);
            if (setting.EnableHiddenTag)
            {
                content = regex_hide.Replace(content, "[******隐藏内容******]");
            }

            return content;//System.Web.HttpUtility.HtmlEncode(content);
        }
        #endregion

        #region ParseWhenEdit
        /// <summary>
        /// 编辑的时候 把HTML转回为UBB
        /// </summary>
        /// <param name="content"></param>
        /// <param name="allowHTML"></param>
        /// <param name="allowMaxCode3"></param>
        /// <returns></returns>
        public static string ParseWhenEdit(int postUserID, string content, bool enableMaxCode, bool enableMaxCode3, bool enableHtml, bool allowHtml, bool allowMaxCode3)
        {
            //if (allowHTML || allowMaxCode3)

            content = new UbbParser().QuoteToUbb(content);

            //编辑时 为使[code]里的内容能正确显示  再编码一次[code]里的内容
            //处理CODE

            Dictionary<Guid, string> codeTable = new Dictionary<Guid, string>();

            
            content = regex_ubb.Replace(content, delegate(Match m)
            {
                StringBuilder sb = new StringBuilder();
                string temp = regex_code.Replace(m.Groups[1].Value, delegate(Match m2)
                {
                    Guid key = Guid.NewGuid();
                    codeTable.Add(key, string.Concat("[code]", m2.Groups[1].Value, "[/code]"));
                    sb.Append(key.ToString());
                    return key.ToString();
                });
                if (sb.Length > 0)
                    return sb.ToString();
                else
                    return temp;
            });

            if (enableMaxCode3 || enableMaxCode)
            {
                if (allowMaxCode3)
                    content = HtmlToUbbParser.Html2Ubb(
                        postUserID,
                        content);
                else if (allowHtml == false)
                    content = StringUtil.ClearAngleBracket(content);
            }
            else if (enableHtml && allowHtml == false)
            {
                if (allowMaxCode3)
                    content = HtmlToUbbParser.Html2Ubb(
                        postUserID,
                        content);
                else
                    content = StringUtil.ClearAngleBracket(content);
            }

            content = System.Web.HttpUtility.HtmlEncode(content);

            if (codeTable.Count > 0)
            {
                //只有一个[code]标签，直接用string.Replace
                if (codeTable.Count == 1)
                {
                    foreach (KeyValuePair<Guid, string> item in codeTable)
                    {
                        return content.Replace(item.Key.ToString(), item.Value);
                    }
                }
                //有多个[code]标签，用StringBuilder.Replace性能更好
                else
                {
                    StringBuilder contentBuilder = new StringBuilder(content);

                    foreach (KeyValuePair<Guid, string> item in codeTable)
                    {
                        contentBuilder.Replace(item.Key.ToString(), item.Value);
                    }

                    return contentBuilder.ToString();
                }
            }

            return content;
        }
        #endregion

        #region 处理[hide][free]标签  ProcessThreadContentTags

        /// <summary>
        /// 处理[hide][free]标签
        /// </summary>
        /// <param name="content"></param>
        /// <param name="threadID"></param>
        /// <returns></returns>
        public static string ProcessThreadContentTags(string content, int threadID)
        {

            BasicThread thread = PostBOV5.Instance.GetThread(threadID);

            Forum forum = null;
            if (thread != null)
                forum = ForumBO.Instance.GetForum(thread.ForumID);

            AuthUser currentUser = User.Current;
            if (AllSettings.Current.ForumSettings.Items.GetForumSettingItem(thread.ForumID).EnableHiddenTag)
            {

                MaxHideTag handler = new MaxHideTag();
                handler.OnCheckPermission = delegate(string hideTagParam)
                {

                    if (thread == null)
                        return MaxHideTag.CheckPermissionResults.NoProcessHideTag;


                    if (forum == null)
                        return MaxHideTag.CheckPermissionResults.NoProcessHideTag;

                    if (AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forum.ForumID).Can(currentUser, ForumPermissionSetNode.Action.AlwaysViewContents))
                        return MaxHideTag.CheckPermissionResults.HasManagePermission;

                    AuthUser user = User.Current;

                    if (hideTagParam != null && hideTagParam.Trim() != string.Empty && Regex.IsMatch(hideTagParam, "^\\d+$"))
                    {
                        int need = int.Parse(hideTagParam);

                        if (user.UserID == thread.PostUserID && user.UserID != 0)
                            return MaxHideTag.CheckPermissionResults.HasPermission;

                        if (user != null && user.Points >= need)
                            return MaxHideTag.CheckPermissionResults.HasPermission;
                        else
                            return MaxHideTag.CheckPermissionResults.NoPermission;
                    }
                    else
                    {
                        if(thread.IsReplied(user))
                            return MaxHideTag.CheckPermissionResults.HasPermission;
                        else
                            return MaxHideTag.CheckPermissionResults.NeedComment;
                    }

                    //return MaxHideTag.CheckPermissionResults.NeedComment;
                };

                UbbParser parser = new UbbParser();
                parser.EncodeHtml = false;
                parser.AddTagHandler(handler);


                if (thread != null && forum != null && thread.Price > 0)
                {
                    content = ProcessFreeTag(content, thread, forum);
                }
                return parser.UbbToHtml(content);
            }
            else
            {
                if (thread != null && forum != null && thread.Price > 0)
                {
                    content = ProcessFreeTag(content, thread, forum);
                }
                return content;
            }

        }

        #endregion

        #region free
        private const string freeStyle = "<div class=\"maxcode-freecontent\"><p class=\"maxcode-freetips\">以下是不需要购买即可见的内容</p>{0}</div>";
        private static Regex regex_free = new Regex(@"\[free\](?is)(.*?|\s*?)\[/free\]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);


        private static string ProcessFreeTag(string content, BasicThread thread, Forum forum)
        {
            AuthUser user = User.Current;
            if (user.UserID == thread.PostUserID || thread.IsOverSellDays(forum.ForumSetting) || thread.IsBuyed(user) || (AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forum.ForumID).Can(user, ForumPermissionSetNode.Action.AlwaysViewContents)))
                content = regex_free.Replace(content, string.Format(freeStyle, "$1"));
            else
            {
                content = ProcessFreeTag(content,thread.IsBuyed(user));
            }
            return content;
        }

        /// <summary>
        /// 引用的时候 在编辑器里显示
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string ProcessFreeWhenRequoteTag(string content)
        {
            bool isMatch = false;
            content = regex_free.Replace(content, delegate(Match match)
            {
                isMatch = true;

                return string.Concat(match.Groups[1].Value, "\r\n");
            });

            if (isMatch == false)
                return string.Empty;
            return content;

            //MatchCollection mc = regex_free.Matches(content);
            //if (mc.Count > 0)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    foreach (Match m in mc)
            //    {
            //        sb.Append(m.Groups[1].Value);
            //        sb.Append("\r\n");
            //    }
            //    content = sb.ToString();
            //}
            //else
            //    content = "";
            //return content;
        }

        private static string ProcessFreeTag(string content, bool isBuyed)
        {
            StringBuilder result = new StringBuilder();
            content = regex_free.Replace(content, delegate(Match match)
            {
                string tempContent = string.Concat(string.Format(freeStyle, match.Groups[1].Value), "\r\n");
                if (isBuyed == false)
                    result.Append(tempContent);
                return tempContent;
            });

            if (isBuyed)
                return content;
            else
                return result.ToString();
        }


        //private const string attachV3 = "#attach:{0}#";
        //private const string 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attachmentID"></param>
        /// <returns></returns>
        public static bool IsFreeAttachmentID(int attachmentID, string content)
        {
            string attach1 = GetAttachTag(attachmentID).ToLower();
            string attach2 = GetAttachImgTag(attachmentID, null, null).ToLower();
            string attach3 = string.Concat("#attach:", attachmentID, "#");
            string attach4 = GetAttachMediaTag(attachmentID,null,null,false).ToLower();

            MatchCollection mc = regex_free.Matches(content);
            Regex reg = new Regex(@"\[attachimg=(?<width>\d+),(?<height>\d+)\]" + attachmentID + @"\[/attachimg\]", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex regMedia = new Regex(@"\[attachmedia=(?<width>\d+),(?<height>\d+),(?<auto>\d+)\]" + attachmentID + @"\[/attachmedia\]", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            foreach (Match m in mc)
            {
                string matchValue = m.Groups[1].Value.ToLower();

                if (matchValue.IndexOf(attach1) >= 0)
                    return true;
                if (matchValue.IndexOf(attach2) >= 0)
                    return true;
                if (matchValue.IndexOf(attach3) >= 0)
                    return true;
                if (matchValue.IndexOf(attach4) >= 0)
                    return true;

                if (reg.IsMatch(matchValue))
                    return true;

                if (regMedia.IsMatch(matchValue))
                    return true;
            }

            return false;
        }

        public static bool IsFreeDiskFileID(int diskFileID, string content)
        {
            string attach = string.Concat("#file:", diskFileID, "#");

            MatchCollection mc = regex_free.Matches(content);
            foreach (Match m in mc)
            {
                if (m.Groups[1].Value.IndexOf(attach, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }

        #endregion
    }
}