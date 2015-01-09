//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.Security;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web
{
    public abstract class AppPageBase : CenterPageBase
	{
        protected override void OnInit(EventArgs e)
		{
            base.OnInit(e);
            if (AppOwner == null)
                ShowError("您要查看的用户不存在");
		}


        protected override bool NeedLogin
        {
            get
            {
                return false;
            }
        }


        //===============================================
        private int? m_AppOwnerUserID;
        /// <summary>
        /// 要查看的用户
        /// </summary>
        protected virtual int AppOwnerUserID
        {
            get
            {
                if (m_AppOwnerUserID == null)
                {
                    m_AppOwnerUserID = _Request.Get<int>("uid", Method.Get, MyUserID);
                }
                return m_AppOwnerUserID.Value;
            }
        }



        private User m_AppOwner;
        public User AppOwner
        {
            get
            {
                if (m_AppOwner == null)
                {
                    if (AppOwnerUserID > 0)
                        m_AppOwner = UserBO.Instance.GetUser(AppOwnerUserID);
                    else
                        m_AppOwner = My;
                }
                return m_AppOwner;
            }
        }


        private bool? m_IsShowAppList;
        protected bool IsShowAppList
        {
            get
            {
                if (m_IsShowAppList == null)
                {
                    if (EnableAlbumFunction || EnableBlogFunction || EnableDoingFunction || EnableShareFunction)
                        m_IsShowAppList = true;
                    else
                        m_IsShowAppList = false;
                }

                return m_IsShowAppList.Value;
            }
        }

        private string m_OwnerIt;
        protected string OwnerIt
        {
            get
            {
                if (m_OwnerIt == null)
                {
                    if (AppOwnerUserID == MyUserID)
                        m_OwnerIt = "我";
                    else if (AppOwner.Gender == Gender.Female)
                        m_OwnerIt = "她";
                    else if (AppOwner.Gender == Gender.Male)
                        m_OwnerIt = "他";
                    else
                        m_OwnerIt = "TA";
                }

                return m_OwnerIt;
            }
        }

        private bool? m_VisitorIsFriend;
        protected bool VisitorIsFriend
        {
            get
            {
                if (m_VisitorIsFriend.HasValue == false)
                    m_VisitorIsFriend = FriendBO.Instance.IsFriend(AppOwnerUserID, MyUserID);

                return m_VisitorIsFriend.Value;
            }
        }

        public bool VisitorIsOwner
        {
            get { return MyUserID == AppOwnerUserID; }
        }

        private bool? m_VisitorIsOwnerFriend;

        public bool VisitorIsOwnerFriend
        {
            get
            {
                if (m_VisitorIsOwnerFriend.HasValue == false)
                    m_VisitorIsOwnerFriend = FriendBO.Instance.IsFriend(MyUserID, AppOwnerUserID);

                return m_VisitorIsOwnerFriend.Value;
            }
        }
	}
}