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
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Entities
{
    public class MenuPermissions
    {
        private AuthUser user;
        private BackendPermissions spacePremissions;
        private ManageUserPermissionSet userPremissons;
        private int formID;
        
        public MenuPermissions(AuthUser user ,int formID )
        {
            this.user = user;
            this.formID = formID;
            spacePremissions = AllSettings.Current.BackendPermissions;
            userPremissons = AllSettings.Current.ManageUserPermissionSet;
        }

        private bool? m_Shield = null;
        [JsonItem]
        public bool Shield
        {
            get
            {
                if (m_Shield == null)
                    m_Shield = AllSettings.Current.BackendPermissions.HasPermissionForSomeone(user
                   , BackendPermissions.ActionWithTarget.Manage_BanUsers);

                return m_Shield.Value;
            }

        }

        private bool? m_Chat = null;
        [JsonItem]
        public bool Chat
        {
            get
            {
                if (m_Chat == null)
                    m_Chat = spacePremissions.HasPermissionForSomeone(user, BackendPermissions.ActionWithTarget.Manage_Chat);
                
                return m_Chat.Value;
            }
        }

        private bool? m_DeleteUser =null;
        [JsonItem]
        public bool DeleteUser
        {
            get
            {
                if (m_DeleteUser == null)
                    m_DeleteUser = userPremissons.HasPermissionForSomeone(user, ManageUserPermissionSet.ActionWithTarget.DeleteUser);
                
                return m_DeleteUser.Value;
            }
        }

        private bool? m_UpdatUserProfile = null;
        [JsonItem]
        public bool UpdatUserProfile
        {
            get
            {
                if (m_UpdatUserProfile == null)
                    m_UpdatUserProfile = userPremissons.HasPermissionForSomeone(user, ManageUserPermissionSet.ActionWithTarget.EditUserProfile);

                return m_UpdatUserProfile.Value;
            }
        }

#if Passport
        [JsonItem]
        public bool Post
        {
            get
            {
                return false;
            }
        }

        [JsonItem]
        public bool Album
        {
            get
            {
                return false;
            }
        }

        [JsonItem]
        public bool Doing
        {
            get
            {
                return false;
            }
        }

        [JsonItem]
        public bool Collection
        {
            get
            {
                return false;
            }
        }

        [JsonItem]
        public bool NetDisk
        {
            get
            {
                return false;
            }
        }

        [JsonItem]
        public bool Notify
        {
            get
            {
                return false;
            }
        }

        [JsonItem]
        public bool Share
        {
            get
            {
                return false;
            }
        }

        [JsonItem]
        public bool Emoticon
        {
            get
            {
                return false;
            }
        }

        [JsonItem]
        public bool Blog
        {
            get
            {
                return false;
            }
        }
#endif

#if !Passport
        private bool? m_Post;
        [JsonItem]
        public bool Post
        {
            get
            {
                if (m_Post == null)
                {
                    m_Post = false;
                    if (formID == 0)
                    {
                        //Dictionary<int, Forum> Forums = ForumManager.GetAllForums(false);

                        ForumCollection forums = ForumBO.Instance.GetAllForums();
                        foreach (Forum forum in forums)
                        {
                            if (forum.ManagePermission.HasPermissionForSomeone(user, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads))
                            {
                                m_Post = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Forum forum = ForumBO.Instance.GetForum(formID);
                        m_Post = forum.ManagePermission.HasPermissionForSomeone(user, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads);
                    }
                }
                return m_Post.Value;
            }
        }

        private bool? m_Album = null;
        [JsonItem]
        public bool Album
        {
            get
            {
                if (m_Album == null)
                    m_Album = spacePremissions.HasPermissionForSomeone(user, BackendPermissions.ActionWithTarget.Manage_Album);

                return m_Album.Value;
            }
        }

        private bool? m_Doing = null;
        [JsonItem]
        public bool Doing
        {
            get
            {
                if (m_Doing == null)
                    m_Doing = spacePremissions.HasPermissionForSomeone(user, BackendPermissions.ActionWithTarget.Manage_Doing);
                
                return m_Doing.Value;
            }
        }

        private bool? m_Collecion = null;
        [JsonItem]
        public bool Collection
        {
            get
            {
                if (m_Collecion == null)
                    m_Collecion = spacePremissions.HasPermissionForSomeone(user, BackendPermissions.ActionWithTarget.Manage_Favorite);

                return m_Collecion.Value; }
        }

        private bool? m_NetDisk = null;
        [JsonItem]
        public bool NetDisk
        {
            get
            {
                if (m_NetDisk == null)
                    m_NetDisk = spacePremissions.HasPermissionForSomeone(user, BackendPermissions.ActionWithTarget.Manage_NetDisk);
                
                return m_NetDisk.Value;
            }
        }

        private bool? m_Notify = null;
        [JsonItem]
        public bool Notify
        {
            get
            {
                if (m_Notify == null)
                    m_Notify = spacePremissions.HasPermissionForSomeone(user, BackendPermissions.ActionWithTarget.Manage_Notify);
                
                return m_Notify.Value;
            }
        }

        private bool? m_Share = null;
        [JsonItem]
        public bool Share
        {
            get
            {
                if (m_Share == null)
                    m_Share = spacePremissions.HasPermissionForSomeone(user, BackendPermissions.ActionWithTarget.Manage_Share);

                return m_Share.Value;            
            }
        }

        private bool? m_Emoticon = null;
        [JsonItem]
        public bool Emoticon
        {
            get
            {
                if (m_Emoticon == null)
                    m_Emoticon = spacePremissions.HasPermissionForSomeone(user, BackendPermissions.ActionWithTarget.Manage_Emoticon);
                
                return m_Emoticon.Value;
            }
        }

        private bool? m_Blog = null;
        [JsonItem]
        public bool Blog
        {
            get
            {
                if (m_Blog == null)
                    m_Blog = spacePremissions.HasPermissionForSomeone(user, BackendPermissions.ActionWithTarget.Manage_Blog);
                
                return m_Blog.Value;
            }
        }
#endif
        private string m_ToString = null;
        public override string ToString()
        {
            if (m_ToString == null)
            {
                m_ToString = JsonBuilder.GetJson(this);
            }
            return m_ToString;
        }
    }
}