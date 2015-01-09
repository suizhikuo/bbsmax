//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using System.Collections;


namespace MaxLabs.bbsMax.Templates
{
    [TemplatePackage]
    public class BackendTemplateMembers// : BaseTemplateMembers
    {
        private const string CacheKey_CurrentAdminPage = "BackendTemplateMembers.CurrentPage";
        private const string CacheKey_MenuJsonData = "BackendMenuData.user.{0}";

        public delegate void AdminMenuTemplate(int i, BackendPage page, BackendPage selected);

        public delegate void AdminMenuDataTemplate(string menuJsonObject);

        private BackendPage m_CurrentPage = null;

        private BackendPage CurrentPage
        {
            get
            {
                if (m_CurrentPage != null)
                    return m_CurrentPage;

                object value = HttpContext.Current.Items[CacheKey_CurrentAdminPage];

                if (value == null)
                {
                    string url = HttpContext.Current.Request.RawUrl;
					NameValueCollection query = null;

                    int indexOfQuertyString = url.IndexOf('?');

					if (indexOfQuertyString > 0)
					{
						query = HttpUtility.ParseQueryString(url.Substring(indexOfQuertyString + 1));
						url = url.Substring(0, indexOfQuertyString);
					}

                    //string requestFileName = Path.GetFileName(HttpContext.Current.Request.MapPath(url));
                    string requestFileName = BbsRouter.GetCurrentUrlScheme().Main.Substring(10) + ".aspx";

                    foreach (BackendPage level1Page in BackendPages)
                    {
                        value = level1Page.GetPage(requestFileName, query);
                        if (value != null)
                        {
                            HttpContext.Current.Items.Add(CacheKey_CurrentAdminPage, value);
                            m_CurrentPage = (BackendPage)value;
                            break;
                        }
                    }
                }
                else
                {
                    m_CurrentPage = (BackendPage)value;
                }

                //if (m_CurrentPage == null)
                //{
                //    m_CurrentPage = BackendPages[0];
                //}

                return m_CurrentPage;
            }
        }

		[TemplateTag]
		public void AdminMenu(int? level, AdminMenuTemplate template)
		{
			AdminMenu(level, null, template);
		}

        [TemplateTag]
        public void AdminMenu(int? level, BackendPage parent, AdminMenuTemplate template)
        {

            BackendPage selectedPage = null;
            BackendPage[] subPages = null;

            if (CurrentPage == null)
            {
                subPages = BackendPages;
                goto label_currentPageNull;
            }



            if (level == 1)
            {
                if (CurrentPage.ParentPage == null)
                    selectedPage = CurrentPage;
                else if (CurrentPage.ParentPage.ParentPage == null)
                    selectedPage = CurrentPage.ParentPage;
                else
                    selectedPage = CurrentPage.ParentPage.ParentPage;

                subPages = BackendPages;
            }
            else if (level == 2)
            {
                if (CurrentPage.ParentPage == null) //如果当前页是第一级，那么就不可能存在第二级页面，也不可能有第二级的选中页面
                    selectedPage = null;
                else if (CurrentPage.ParentPage.ParentPage == null) //如果当前页是第二级，那么选中页就正好是当前页
                    selectedPage = CurrentPage;
                else
                    selectedPage = CurrentPage.ParentPage; //如果当前页面是第三级页面，那么选中的页面就是当前页面的父级页面
            }
            else
            {
                if (CurrentPage.ParentPage == null || CurrentPage.ParentPage.ParentPage == null) //如果当前页是第一或二级，那么就不可能存在第三级页面
                    selectedPage = null;
                else
                    selectedPage = CurrentPage;
            }

            if (parent != null)
                subPages = parent.SubPages;
            else if (selectedPage != null && subPages == null)
                subPages = selectedPage.ParentPage.SubPages;

label_currentPageNull:

            if (subPages != null)
            {
                int i = 1;

                foreach (BackendPage subPage in subPages)
                {
                    template(i, subPage, selectedPage);
                    i++;
                }
            }
        }

        [TemplateTag]
        public void AdminMenuData(AdminMenuDataTemplate data)
        {
            string menuData = null;

            if (!CacheUtil.TryGetValue<string>( string.Format( CacheKey_MenuJsonData, UserBO.Instance.GetCurrentUserID()), out menuData))
            {
                menuData = StringUtil.BuildJsonObject(BackendPages, "ParentPage");
                CacheUtil.Set<string>(CacheKey_MenuJsonData, menuData);
            }

            data(menuData);
        }

        private static BackendPage[] s_BackendPages = null;
        private static object s_BackendPagesLockObject = new object();

        public static BackendPage[] GetSubBackendPages(string rootName)
        {
            foreach (BackendPage bp in BackendPages)
            {
                if (bp.Title == rootName)
                {
                    return bp.SubPages;
                }
            }
            return null;
        }

        [TemplateVariable]
        public BackendPage[] AdminMenuList
        {
            get { return BackendPages; }
        }

        private static BackendPage[] BackendPages
        {
            get
            {
                AllSettings settings = AllSettings.Current;

                if (s_BackendPages == null)
                {
                    lock (s_BackendPagesLockObject)
                    {
                        if (s_BackendPages == null)
                        {
                            ADCategoryCollection adCategorys = ADCategory.SystemADCategoryes;

                            BackendPage[] adSystemSubPages = new BackendPage[adCategorys.Count+1];

                            BackendPage.CheckPermission adSystemCheckPermission = delegate() {
                                return AdvertBO.Instance.CanManageAdvert(UserBO.Instance.GetCurrentUserID()); 
                            };
                            adSystemSubPages[0] = new BackendPage("所有广告", adSystemCheckPermission,  "other/manage-a.aspx?all");
                            for (int i = 0; i < adCategorys.Count; i++)
                            {
                                adSystemSubPages[i+1] = new BackendPage(adCategorys[i].Name, adSystemCheckPermission, "other/manage-a.aspx?categoryID=" + adCategorys[i].ID);
                            }


                                s_BackendPages = new BackendPage[] {
								new BackendPage("全局", 0
									,new BackendPage("设置", 255
										,new BackendPage("站点设置", "global/setting-site.aspx")
										,new BackendPage("邮件设置", "global/setting-email.aspx")
                                        ,new BackendPage("缓存设置", "global/setting-cache.aspx")
                                        ,new BackendPage("防盗链设置", "global/setting-download.aspx")
                                        ,new BackendPage("在线列表","global/setting-onlinelist.aspx","global/setting-onlinelist-role.aspx")
                                        ,new BackendPage("IP黑名单", "global/setting-accesslimit.aspx")
                                        //,new PassportServerBackendPage()
                                        ,new PassportClientBackendPage()
									)
                                    ,new BackendPage("权限设置", 90
                                        ,new BackendPage("用户权限分配", "user/setting-permissions.aspx?t=user")
									    ,new BackendPage("管理权限分配", "user/setting-permissions.aspx?t=manager", "user/setting-managerpermissions.aspx")
									)
                                    ,new BackendPage("非法信息", 90
                                        ,new BackendPage("敏感词语设置", "global/setting-bannedword.aspx")
									    ,new BackendPage("处理用户举报", "global/manage-report.aspx")
									)
                                    ,new BackendPage("显示", 185
                                        //,new BackendPage("顶部链接", "global/setting-toplinks.aspx")
                                        ,new BackendPage("导航菜单", "global/setting-navigation.aspx?istoplink=0")
                                        ,new BackendPage("左上链接", "global/setting-navigation.aspx?istoplink=1")
                                        ,new BackendPage("友情链接", "global/setting-links.aspx")

									    ,new BackendPage("模板管理", "other/manage-template.aspx")
                                        ,new BackendPage("公告管理", "global/manage-announcement.aspx")
                                        ,new BackendPage("广告管理", "other/setting-a.aspx","other/manage-a.aspx")
                                        
									)
                                    ,new BackendPage("体验", 137
                                        ,new BackendPage("验证码设置", "global/setting-validatecode.aspx")
                                        ,new BackendPage("路径模式", "global/setting-friendlyurl.aspx")
										//,new BackendPage("Ajax设置", "global/setting-ajax.aspx")
										,new BackendPage("日期与时间", "global/setting-datetime.aspx")
									)
									,new BackendPage("搜索引擎", 125
										,new BackendPage("基本设置", "global/setting-seobasic.aspx")
										,new BackendPage("百度优化", "global/setting-baiduseo.aspx")
                                        ,new BackendPage("屏蔽搜索引擎","global/setting-shieldspider.aspx")
									)

								)

                                //=================================================================

								,new BackendPage("帐号", 0
                                     ,new BackendPage("管理", 65
										,new BackendPage("用户管理", "user/manage-user.aspx")
                                        ,new BackendPage("新建用户", "user/manage-user-add.aspx")
									)

									,new BackendPage("基本设置",100
										,new BackendPage("注册", "global/setting-register.aspx", "global/setting-registerlimit.aspx")
                                        ,new BackendPage("扩展字段", "user/setting-extendedfield.aspx")
                                        ,new BackendPage("登录","global/setting-logintype.aspx")
                                        ,new BackendPage("找回密码", "global/setting-recoverpassword.aspx")

									)

                                    ,new BackendPage("用户组管理", 135
                                        ,new BackendPage("1 基本组", "user/setting-roles-basic.aspx","user/manage-rolemembers.aspx?t=1")
                                        ,new BackendPage("3 自定义组", "user/setting-roles-other.aspx","user/manage-rolemembers.aspx?t=3")
										,new BackendPage("2 等级组", "user/setting-roles-level.aspx","user/manage-rolemembers.aspx?t=2")
										,new BackendPage("4 管理员组", "user/setting-roles-manager.aspx","user/manage-rolemembers.aspx?t=4")
									)

                                    ,new BackendPage("手机绑定", 65
                                        
										,new BackendPage("绑定设置", "user/setting-phonevalidate.aspx")
                                        ,new BackendPage("绑定日志", "other/manage-mobilelog.aspx?t=0")
									)
                                    ,new BackendPage("实名认证", 65
                                        
										,new BackendPage("实名设置", "global/setting-namecheck.aspx")
                                        ,new BackendPage("实名审查", "user/manage-namecheck.aspx")
									)
                                    ,new BackendPage("头像管理", 65
										,new BackendPage("头像审查", "user/manage-avatarcheck.aspx")
									)
                                    
                                    ,new BackendPage("邀请码", 76
                                        ,new BackendPage("设置", "global/setting-invitation.aspx")
                                        ,new BackendPage("管理","user/manage-inviteserial.aspx")
                                        ,new BackendPage("拥有数排行","user/manage-inviteserial-order.aspx")
                                        
                                    )

                                    ,new BackendPage("帐号屏蔽", 65
                                        ,new BackendPage("屏蔽管理", "user/manage-shielduers.aspx?t=0")
                                        ,new BackendPage("屏蔽日志", "other/manage-banuserlog.aspx?t=0")
									)


									,new BackendPage("积分", 124
										,new BackendPage("基本设置", "user/setting-userpoint.aspx")
                                        ,new BackendPage("积分策略", "user/setting-pointaction.aspx")
										,new BackendPage("兑换设置", "user/setting-pointexchange.aspx")
										,new BackendPage("转帐设置", "user/setting-pointtransfer.aspx")
									)
									,new BackendPage("充值", 65
										,new BackendPage("充值设置", "user/setting-pointrecharge.aspx")
                                        ,new BackendPage("充值日志", "user/manage-paylogs.aspx?t=0")
									)
							    )

                                //=================================================================

								,new BackendPage("论坛", 0
									,new BackendPage("管理版块", 294
										,new BackendPage("版块及版主管理", "bbs/manage-forum.aspx","bbs/manage-forum-detail.aspx?action=editforum","bbs/manage-forum-detail.aspx?action=createforum","user/manage-shielduers.aspx?t=f")
                                        ,new BackendPage("各版块发帖选项", "bbs/manage-forum-detail.aspx?action=editsetting")
                                        ,new BackendPage("各版块用户权限", "bbs/manage-forum-detail.aspx?action=editusepermission")
                                        ,new BackendPage("各版块积分策略", "bbs/manage-forum-detail.aspx?action=editpoint")
                                        ,new BackendPage("各版块评分控制", "bbs/manage-forum-detail.aspx?action=editrate")
                                        ,new BackendPage("各版块管理权限", "bbs/manage-forum-detail.aspx?action=editmanagepermission")
									)
                                    ,new BackendPage("设置", 185
                                        ,new BackendPage("基本设置", "bbs/setting-bbs.aspx")
                                        ,new BackendPage("搜索设置", "bbs/setting-search.aspx")
										,new BackendPage("楼层别名", "bbs/setting-postaliasname.aspx")
                                        ,new BackendPage("帖子签名", "user/setting-user.aspx")
                                        ,new BackendPage("主题鉴定","bbs/setting-judgement.aspx")
                                        ,new BackendPage("帖子图标","bbs/setting-posticon.aspx")
									)
                                    ,new BackendPage("内容审核", 67
                                        ,new BackendPage("审核主题", "bbs/manage-unapprovedtopic.aspx")
                                        ,new BackendPage("审核回复", "bbs/manage-unapprovedpost.aspx")
									)
									,new BackendPage("管理", 115
                                        ,new BackendPage("管理帖子", "bbs/manage-topic.aspx?type=normal")
                                        ,new BackendPage("回收站", "bbs/manage-topic.aspx?type=recycle")
                                        ,new BackendPage("管理附件", "bbs/manage-attachment.aspx")
									)
								)

                                //=================================================================
                                
                                ,new BackendPage("互动", 0
                                    ,new BackendPage("全局", 65
                                        ,new BackendPage("系统表情","global/setting-emoticon-group.aspx","global/setting-emoticon-icon.aspx")
                                    )
                                    ,new BackendPage("消息", 65
                                        ,new BackendPage("消息管理", "interactive/manage-chatsession.aspx", "interactive/manage-chatmessage.aspx")
                                        ,new BackendPage("选项设置", "interactive/setting-chat.aspx")
                                    )
                                    ,new BackendPage("通知", 101
                                        ,new BackendPage("通知管理", "interactive/manage-notify.aspx")
                                        ,new BackendPage("群发", "interactive/manage-systemnotify.aspx", "interactive/systemnotify-edit.aspx")
									    ,new BackendPage("选项设置", "interactive/setting-notify.aspx")
									)
                                    ,new BackendPage("活动", 125
                                        ,new BackendPage("用户任务", "interactive/manage-mission-list.aspx", "interactive/manage-mission-detail.aspx")
                                        ,new BackendPage("任务分类", "interactive/manage-mission-category.aspx")
                                        ,new BackendPage("点亮图标", "interactive/setting-medals.aspx", "interactive/setting-medal-detail.aspx")
                                    )
                                    ,new BackendPage("道具", 150
                                        ,new BackendPage("基本选项", "interactive/setting-prop.aspx")
                                        ,new BackendPage("用户物品管理", "interactive/manage-userprop.aspx")
                                        ,new BackendPage("物品设置", "interactive/manage-prop.aspx", "interactive/manage-prop-detail.aspx")
                                        ,new BackendPage("获取物品记录","interactive/manage-usergetprop.aspx?t=hd")
                                    )
                                    ,new BackendPage("动态", 125
                                        ,new BackendPage("动态管理", "global/manage-feed-data.aspx")
                                        ,new BackendPage("动态模板", "global/manage-feed-template.aspx")
                                        ,new BackendPage("发布动态", "global/manage-feed-sitefeedlist.aspx", "global/manage-feed-sitefeed.aspx")
                                        ,new BackendPage("自动清理", "global/setting-feedjob.aspx")
                                    )
                                    ,new BackendPage("好友", 176
                                        ,new BackendPage("好友数量限制", "user/setting-friend.aspx")
                                        ,new BackendPage("好友印象设置", "user/setting-impression.aspx")
                                        ,new BackendPage("好友印象管理", "user/manage-impressionrecord.aspx")
                                        ,new BackendPage("好友印象词库", "user/manage-impressiontype.aspx")
									)
                                )

                                //=================================================================

								,new BackendPage("应用", 0

                                    ,new BackendPage("全局", 125
                                        ,new BackendPage("空间设置", "app/setting-space.aspx")
                                        ,new BackendPage("默认隐私", "global/setting-privacy.aspx")
                                        ,new BackendPage("留言及评论管理", "app/manage-comment.aspx")
                                        
                                    )
                                    ,new BackendPage("日志", 101
                                        ,new BackendPage("文章管理","app/manage-blog.aspx")
                                        ,new BackendPage("设置", "app/setting-blog.aspx")
                                        ,new BackendPage("分类管理", "app/manage-blogcategory.aspx")
                                    )
                                    ,new BackendPage("相册", 101
                                        ,new BackendPage("照片管理", "app/manage-photo.aspx")
                                        ,new BackendPage("设置", "app/setting-album.aspx")
                                        ,new BackendPage("分类管理", "app/manage-album.aspx")
                                    )
                                    ,new BackendPage("记录", 65
                                        ,new BackendPage("记录管理", "app/manage-doing.aspx")
                                        ,new BackendPage("选项设置", "app/setting-doing.aspx")
                                    )
								    ,new BackendPage("收藏", 65
                                        ,new BackendPage("收藏管理", "app/manage-share-data.aspx?type=favorite")
                                        ,new BackendPage("选项设置","app/setting-favorite.aspx")
                                    )
                                    ,new BackendPage("分享", 65
                                        ,new BackendPage("分享管理", "app/manage-share-data.aspx?type=share")
                                        ,new BackendPage("选项设置", "app/setting-share.aspx")
                                    )
                                    ,new BackendPage("网络硬盘", 65
                                        ,new BackendPage("文件管理", "app/manage-netdisk.aspx")
                                        ,new BackendPage("选项设置", "app/setting-netdisk.aspx")
                                    )
                                    ,new BackendPage("用户表情", 65
                                        ,new BackendPage("表情管理", "app/manage-emoticon.aspx","app/manage-emoticon-icon.aspx")
                                        ,new BackendPage("选项设置", "app/setting-useremoticon.aspx")
                                    )
                                    ,new BackendPage("竞价排名", 65
                                        ,new BackendPage("管理榜单", "app/manage-pointshow.aspx")
                                        ,new BackendPage("选项设置", "app/setting-pointshow.aspx")
                                    )

                                    ,new BackendPage("其他", 65
                                        ,new BackendPage("标签管理", "app/manage-tag.aspx")
									)
								)

                                //=================================================================

								,new BackendPage("维护", 0
									,new BackendPage("系统日志", 415
                                        ,new BackendPage("帐号屏蔽日志", "other/manage-banuserlog.aspx?t=wh")
                                        ,new BackendPage("找回密码日志","other/manage-recoverpasswordlog.aspx?t=wh")
                                        ,new BackendPage("现金充值日志", "user/manage-paylogs.aspx?t=wh")
                                        ,new BackendPage("用户IP日志", "other/manage-iplog.aspx?t=wh")

                                        ,new BackendPage("日志清理设置", "other/setting-deleteoperationlogjob.aspx")

                                        ,new BackendPage("积分变更日志","other/manage-pointlog.aspx","other/setting-pointlogclear.aspx")
                                        ,new BackendPage("手机绑定日志", "other/manage-mobilelog.aspx?t=wh")                                        
										,new BackendPage("获取物品记录","interactive/manage-usergetprop.aspx?t=wh")
                                        ,new BackendPage("其他类型日志", "other/manage-operationlog.aspx?t=wh")
                                        
                                        
									)
                                    ,new BackendPage("扩展性", 67
									    ,new BackendPage("插件管理", "other/manage-plugin.aspx")
									    ,new BackendPage("远程调用", "other/manage-invoker.aspx","other/manage-invoker-detail.aspx")
									)
                                    ,new BackendPage("工具", 125
                                        ,new BackendPage("重新启动", "other/tool-restart.aspx")
                                        ,new BackendPage("内存整理", "other/tool-freememory.aspx")
                                        ,new BackendPage("重新统计数据", "other/tool-updatedatas.aspx")
                                    )
                                )
							};
                        }
                    }
                }

                return s_BackendPages;
            }
        }



        public class PassportServerBackendPage : PassportClientBackendPage
        {
            public PassportServerBackendPage()
                : base("Passport服务设置", "passport/setting-passportserver.aspx")
            {


            }
        }

        public class PassportClientBackendPage : BackendPage
        {
            public PassportClientBackendPage()
              : this("Passport客户端设置", "global/setting-passportclient.aspx")
            {


            }

            public PassportClientBackendPage(string title,string pagename)
                : base(title,pagename)
            {


            }
        

            internal override string FileName
            {
                get
                {
                    return base.FileName;
                }
                set
                {
                    base.FileName = value;
                }
            }

            [JsonItem]
            public override bool HasPermission
            {
                get
                {
                    return User.Current.IsOwner;
                }
            }

            [JsonItem]
            public override string Title
            {
                get
                {
                    return base.Title;
                }
                set
                {
                    base.Title = value;
                }
            }
        }
 
        public class BackendPage
        {
            public BackendPage() { }
            private static int _index=0;
            public delegate bool CheckPermission();

            public BackendPage(string title, params string[] fileNames)
                : this(title, null, fileNames)
            { }

            public BackendPage(string title, CheckPermission check, params string[] fileNames)
            {
                Title = title;
                FileNames = fileNames;
                m_CheckPermission = check;
                _index++;
                this._id = _index;

				Queries = new List<NameValueCollection>();

                FileName = fileNames[0];

				for(int i=0; i<FileNames.Length; i++)
                {
                    string fileName = FileNames[i];
					
                    int indexOfQuery = fileName.IndexOf('?');

					if (indexOfQuery >= 0)
					{
						Queries.Add(HttpUtility.ParseQueryString(fileName.Substring(indexOfQuery + 1)));

                        FileNames[i] = fileName.Substring(0, indexOfQuery);
                    }
					else
					{
						Queries.Add(null);
					}
				}
            }

			public BackendPage(string title, int width, params BackendPage[] subPages)
                : this(title, width, null, subPages)
            { }

            public BackendPage(string title, int width, CheckPermission check, params BackendPage[] subPages)
            {
                Width = width;
                Title = title;
                SubPages = subPages;
                m_CheckPermission = check;
                _index++;
                this._id = _index;
                foreach (BackendPage subPage in SubPages)
                {
                    subPage.ParentPage = this;
                }
            }

            private CheckPermission m_CheckPermission;

            [JsonItem]
            public int Width { get; set; }
            
            [JsonItem]
            public virtual string Title { get; set; }

            [JsonItem]
            internal virtual string FileName { get; set; }
            
            internal string[] FileNames { get; set; }

            [JsonItem]
            public BackendPage[] SubPages { get; private set; }
            
            public BackendPage ParentPage { get; set; }

            public string Tip { get; set; }
            
            private int _id;

            [JsonItem]
            public int Id { get { return _id; } }

			internal List<NameValueCollection> Queries { get; set; }

            [JsonItem]
            public string Url
            {
                get
                {
                    if (FileNames == null)
                    {
                        return SubPages[0].Url;
                    }

                    return FileName;
                }
            }

            [JsonItem]
            public virtual bool HasPermission
            {
                get
                {
                    if (ParentPage != null && ParentPage.HasPermission == false)
                        return false;

                    else if (m_CheckPermission == null || m_CheckPermission())
                        return true;
                    else
                        return false;
                }
            }

            public BackendPage GetPage(string fileName, NameValueCollection query)
            {
				if (FileNames != null && FileNames.Length > 0)
				{

					for(int i=0; i<FileNames.Length; i++)
					{
						if (StringUtil.EqualsIgnoreCase(FileNames[i], fileName))
						{
							bool queryMatched = true;

							if (query != null && Queries[i] != null)
							{
								foreach (string key in Queries[i].AllKeys)
								{
									if (query[key] != Queries[i][key])
									{
										queryMatched = false;
										break;
									}
								}
							}

							if (queryMatched)
								return this;
						}
					}
				}

                if (SubPages != null)
                {
                    foreach (BackendPage subPage in SubPages)
                    {
                        BackendPage result = subPage.GetPage(fileName, query);

                        if (result != null)
                            return result;
                    }
                }

                return null;
            }
        }
    }
}