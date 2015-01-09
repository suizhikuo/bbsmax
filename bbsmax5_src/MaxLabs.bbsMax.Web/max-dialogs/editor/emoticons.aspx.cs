//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Entities;
using System.Collections.Generic;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class editor_face : DialogPageBase
    {
        private int m_UserID;
        private string m_Action;
        private int m_TargetID;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_Action = _Request.Get("action", Method.Get);
            m_TargetID = _Request.Get("targetid", Method.Get, 0);

            switch (m_Action)
            {
                case "post":

                    MaxLabs.bbsMax.Entities.PostV5 post = PostBOV5.Instance.GetPost(m_TargetID, false);
                    if (post != null)
                    {
                        ManageForumPermissionSetNode permission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(post.ForumID);
                        if (permission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.UpdateThreads, post.UserID)
                            || permission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.UpdatePosts, post.UserID))
                        {
                            m_UserID = post.UserID;
                            break; ;
                        }
                    }
                    m_UserID = MyUserID;
                    break;

                case "user":

                    if (UserBO.Instance.CanEditUserProfile(My, m_TargetID))
                        m_UserID = m_TargetID;
                    else
                        m_UserID = MyUserID;

                    break;

                default:
                    m_UserID = MyUserID;
                    break;
            }

            SetPager("list", null, PageNumber, PageSize, CurrentGroup.TotalEmoticons);
            //SetPager("list", string.Format("emoticons.aspx?id={0}&page={1}&groupid={2}&ispanel={3}&from={4}&callback={5}",
            //    _Request.Get("id", Method.Get),
            //    "{0}",
            //    _Request.Get("groupid", Method.Get),
            //    IsPanel ? "true" : "false",
            //    _Request.Get("from", Method.Get),
            //    Callback
            //    ),
            //    PageNumber,
            //    PageSize,
            //    CurrentGroup.TotalEmoticons
            //    );
        }

        protected string Callback
        {
            get
            {
                return _Request.Get("callback", Method.Get);
            }
        }

        private string m_EditorID;
        protected string EditorID
        {
            get
            {
                if (m_EditorID == null)
                {
                    m_EditorID = string.Empty + _Request.Get("id", Method.Get);
                    m_EditorID = HttpUtility.HtmlEncode( m_EditorID);
                    m_EditorID = StringUtil.ToJavaScriptString(m_EditorID);
                }

                return m_EditorID;
            }
        }

        protected int UserID { get { return m_UserID; } }

        protected bool IsPanel
        {
            get
            {
                return _Request.Get<bool>("ispanel", Method.Get,false);
            }
        }

        protected bool CanUseUserEmotion
        {
            get { return EmoticonBO.Instance.CanUseEmoticon(MyUserID); }
        }

        protected bool OutDefaultEmotion
        {
            get { return _Request.Get<bool>("defalut", Method.Get, true); }
        }

        protected bool OutUserEmoticon
        {
            get { return _Request.Get<bool>("user", Method.Get, true); }
        }



        private List<EmoticonGroupBase> m_FaceGroupList;
        protected List<EmoticonGroupBase> FaceGroupList
        {
            get
            {
                if (m_FaceGroupList == null)
                {
                    List<EmoticonGroupBase> g = new List<EmoticonGroupBase>();

                    if (OutDefaultEmotion)
                    {
                        foreach (EmoticonGroupBase group in AllSettings.Current.DefaultEmotSettings.AvailableGroups)
                        {
                            g.Add(group);
                        }
                    }

                    if (OutUserEmoticon)
                    {
                        EmoticonGroupCollection temp = EmoticonBO.Instance.GetEmoticonGroups(UserID);
                        if (temp.Count == 0)
                        {
                            EmoticonGroupBase defaultGroup = EmoticonBO.Instance.CreateDefaultGroup(UserID);
                            if (defaultGroup != null)
                            {
                                g.Add(defaultGroup);
                            }
                        }
                        else
                        {
                            foreach (EmoticonGroupBase group in temp)
                            {
                                g.Add(group);
                            }
                        }
                    }

                    m_FaceGroupList = g;

                }
                return m_FaceGroupList;
            }
        }

        protected int Left
        {
            get
            {
                return _Request.Get<int>("left", Method.Get, 0);
            }
        }

        private EmoticonGroupBase m_CurrentGroup;

        private EmoticonGroupBase m_MyDefaultGroup;
        protected EmoticonGroupBase MyDefaultGroup
        {
            get
            {
                if (!CurrentGroup.IsDefault)
                {
                    m_MyDefaultGroup = CurrentGroup;
                }
                else
                {
                    foreach (EmoticonGroupBase b in FaceGroupList)
                    {
                        if (!b.IsDefault)
                        {
                            m_MyDefaultGroup = b;
                            break;
                        }
                    }
                }
                return m_MyDefaultGroup; ;
            }
            set
            {
                m_MyDefaultGroup = value;
            }
        }

        protected string GetUrl(string url)
        {
            url = url.Replace('\\', '/');
            return StringUtil.ToJavaScriptString(url).Replace(this.EmoteRoot, string.Empty);
        }

        private string m_HttpRoot;
        protected string HttpRoot
        {
            get
            {
                if (m_HttpRoot == null)
                    m_HttpRoot = string.Concat(Globals.FullAppRoot, EmoteRoot);
                return m_HttpRoot;
            }
        }

        private string m_emoteroot;
        protected string EmoteRoot
        {
            get
            {
                if (m_emoteroot == null)
                {
                    if (this.CurrentGroup.IsDefault)
                    {

                        m_emoteroot = UrlUtil.ResolveUrl((CurrentGroup as DefaultEmoticonGroup).Url);
                    }
                    else
                    {
                        m_emoteroot = Globals.GetVirtualPath(SystemDirecotry.Upload_Emoticons);
                    }
                    m_emoteroot = m_emoteroot.Replace('\\', '/');
                    m_emoteroot = StringUtil.ToJavaScriptString(m_emoteroot);
                }

                return m_emoteroot;
            }
        }

        private string m_EncodeHttpRoot;
        protected string EncodeHttpRoot
        {
            get
            {
                if (m_EncodeHttpRoot == null)
                {
                    m_EncodeHttpRoot = HttpUtility.UrlPathEncode(HttpRoot);
                }

                return m_EncodeHttpRoot;
            }
        }

        protected string GetJsString(string s)
        {
            return StringUtil.ToJavaScriptString(s);
        }

        protected EmoticonGroupBase CurrentGroup
        {
            get
            {
                if (m_CurrentGroup == null)
                {
                    int groupID = _Request.Get<int>("groupid", Method.Get, 0);
                    if (groupID == 0)
                        return FaceGroupList[0];
                    else
                        foreach (EmoticonGroupBase group in FaceGroupList)
                        {
                            if (group.GroupID == groupID)
                            {
                                m_CurrentGroup = group;
                                break;
                            }
                        }
                    if (m_CurrentGroup == null)
                        m_CurrentGroup = FaceGroupList[0];
                }
                return m_CurrentGroup;
            }
        }

        protected int PageNumber
        {
            get { return _Request.Get<int>("page", Method.Get, 1); }
        }

        protected int PageSize
        {
            get { return 120; }
        }

        private List<IEmoticonBase> m_emoticonList;
        protected List<IEmoticonBase> EmoticonList
        {
            get
            {
                if (m_emoticonList == null)
                {
                    m_emoticonList = new List<IEmoticonBase>();
                    if (CurrentGroup.IsDefault)
                    {
                        for (int i = (PageNumber - 1) * PageSize; i < PageNumber * PageSize; i++)
                        {
                            if (i >= CurrentGroup.TotalEmoticons) break;
                            m_emoticonList.Add((CurrentGroup as DefaultEmoticonGroup).Emoticons[i]);
                        }
                    }
                    else
                    {
                        EmoticonCollection userEmoticons = EmoticonBO.Instance.GetEmoticons(UserID, CurrentGroup.GroupID, PageNumber, PageSize, false);
                        foreach (IEmoticonBase emote in userEmoticons)
                            m_emoticonList.Add(emote);
                    }
                }
                return m_emoticonList;
            }
        }
    }
}