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
using System.Text.RegularExpressions;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine.Routing;
using MaxLabs.WebEngine.Template;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax
{
	public class BbsRouter
	{
		private static RouteTable s_RouteTable = null;

        public static void Init()
        {

            s_RouteTable = new RouteTable(

                new RouteTable("^default$"
                        , "~/max-templates/default/forums/default.aspx"
				)

			#region 群组

				, new RouteTable("^club/"
					, new RouteTable(
						@"\Gcreate$"
						, "~/max-templates/default/icenter/club-create.aspx"
					)
					, new RouteTable(
						@"\Glist$"
						, "~/max-templates/default/icenter/club-list.aspx"
					)

					, new RouteTable(
						@"\G(?<cid>\d+)/"

						, new RouteTable(
							@"\Ginvite$"
							, "~/max-templates/default/icenter/club-invite.aspx?id=${cid}"
						)
						, new RouteTable(
							@"\Gmembers$"
							, "~/max-templates/default/icenter/club-members.aspx?id=${cid}"
						)
						, new RouteTable(
							@"\Gsetting$"
							, "~/max-templates/default/icenter/club-setting.aspx?id=${cid}"
						)
					)

					,new RouteTable(
						@"\G(?<cid>\d+)$"

						,"~/max-templates/default/club/home.aspx?cid=${cid}"
					)
				)

			#endregion

			#region SNS

			#region 空间

, new RouteTable(@"^space/(?<uid>\d+)/"

                    , new RouteTable(@"\Gblog/"

                        , new RouteTable(
                            @"\Garticle-(?<id>\d+)/(?<page>\d+)$"
                            , "~/max-templates/default/space/blog-view.aspx?uid=${uid}&id=${id}&page=${page}"
                        )
                        , new RouteTable(
                            @"\Garticle-(?<id>\d+)$"
                            , "~/max-templates/default/space/blog-view.aspx?uid=${uid}&id=${id}"
                        )
                        , new RouteTable(
                            @"\Gcategory-(?<cat>\d+)/(?<page>\d+)$"
                            , "~/max-templates/default/space/blog-list.aspx?uid=${uid}&cat=${cat}&page=${page}"
                        )
                        , new RouteTable(
                            @"\Gcategory-(?<cat>\d+)$"
                            , "~/max-templates/default/space/blog-list.aspx?uid=${uid}&cat=${cat}"
                        )
                        , new RouteTable(
                            @"\Gtag-(?<tag>\d+)$"
                            , "~/max-templates/default/space/blog-list.aspx?uid=${uid}&tag=${tag}"
                        )
                        , new RouteTable(
                            @"\G(?<page>\d+)$"
                            , "~/max-templates/default/space/blog-list.aspx?uid=${uid}&page=${page}"
                        )
                    )

                    , new RouteTable(
                        @"\Gblog$"
                        , "~/max-templates/default/space/blog-list.aspx?uid=${uid}"
                    )

                    , new RouteTable(
                        @"\Gdoing$"
                        , "~/max-templates/default/space/doing-list.aspx?uid=${uid}"
                    )

                    , new RouteTable(
                        @"\Galbum/album-(?<id>\d+)$"
                        , "~/max-templates/default/space/album-view.aspx?uid=${uid}&id=${id}"
                    )
                    , new RouteTable(
                        @"\Galbum/photo-(?<id>\d+)/(?<page>\d+)$"
                        , "~/max-templates/default/space/album-photo.aspx?uid=${uid}&id=${id}&page=${page}"
                    )
                    , new RouteTable(
                        @"\Galbum/photo-(?<id>\d+)$"
                        , "~/max-templates/default/space/album-photo.aspx?uid=${uid}&id=${id}"
                    )
                    , new RouteTable(
                        @"\Galbum$"
                        , "~/max-templates/default/space/album-list.aspx?uid=${uid}"
                    )

                    , new RouteTable(
                        @"\Gshare/share-(?<id>\d+)$"
                        , "~/max-templates/default/space/share-view.aspx?uid=${uid}&id=${id}"
                    )
                    , new RouteTable(
                        @"\Gshare$"
                        , "~/max-templates/default/space/share-list.aspx?uid=${uid}"
                    )

                    , new RouteTable(
                        @"\Gboard$"
                        , "~/max-templates/default/space/board-list.aspx?uid=${uid}"
                    )

                    , new RouteTable(
                        @"\Gfriend$"
                        , "~/max-templates/default/space/friend-list.aspx?uid=${uid}"
                    )

                    , new RouteTable(@"\G(?<path>.*)", "~/max-templates/default/space/${path}.aspx?uid=${uid}")
                )

                , new RouteTable(
                    @"^space/(?<uid>\d+)"
                    , "~/max-templates/default/space/space.aspx?uid=${uid}"
                )

            #endregion

            #region 杂项

, new RouteTable(
                    @"^logout/(?<uid>\d+)$"
                    , "~/max-templates/default/logout.aspx?uid=${uid}"
                )

                , new RouteTable(
                    @"^register/(?<invite>[\w\-\{\}%]+)$"
                    , "~/max-templates/default/register.aspx?invite=${invite}"
                )

                , new RouteTable(
                    @"^favorite$"
                    , "~/max-templates/default/icenter/share.aspx?type=private"
                )

                , new RouteTable(
                    "^tag/"

                    , new RouteTable(
                        @"\Glist$"
                        , "~max-templates/default/tag-list.aspx"
                    )

                    , new RouteTable(
                        @"\G(?<id>\d+)/(?<type>\w+)$"
                        , "~max-templates/default/tag-view.aspx?tagid=${id}&type=${type}"
                    )
                )

                , new RouteTable(
                    @"^vistor/(?<id>\d+)$"
                    , "~/max-templates/default/vistors-view.aspx?uid=${id}"
                )

                , new RouteTable(
                    "^vistor$"
                    , "~/max-templates/default/vistors.aspx"
                )

            #endregion

            #endregion

            #region 论坛


//            #region 公告

//, new RouteTable("^announcement/"

//                    , new RouteTable(
//                        @"\G(?<id>\d+)$"
//                        , "~/max-templates/default/announcements.aspx?id=${id}"
//                    )
//                )

//                , new RouteTable("^announcement$"
//                    , "~/max-templates/default/announcements.aspx"
//                )

//            #endregion

                , new RouteTable("^archiver/default$"
                    , "~/archiver/default.aspx"
                )
                , new RouteTable("^archiver/"

                    , new RouteTable(
                        @"\G(?<codename>.+)/list-(?<page>[\{\}\d]+)$"
                        , "~/archiver/showforum.aspx?codename=${codename}&page=${page}"
                    )

                    //, new RouteTable(
                //    @"\G(?<codename>.+)$"
                //    , "~/max-templates/default/forums/archiver_showforum.aspx?codename=${codename}"
                //)
                )

                , new RouteTable("^archiver/"

                    , new RouteTable(
                        @"\G(?<codename>.+)/thread-(?<tid>\d+)-(?<page>[\{\}\d]+)$"
                        , "~/archiver/showthread.aspx?codename=${codename}&threadid=${tid}&page=${page}"
                    )

                    //, new RouteTable(
                //    @"\G(?<codename>.+)/thread-(?<tid>\d+)$"
                //    , "~/max-templates/default/forums/archiver_showthread.aspx?codename=${codename}&threadid=${tid}"
                //)
                )

                //, new RouteTable("^threadmanagelogs/"

                //    , new RouteTable(
                //        @"\G(?<codename>.+)/(?<stype>\d+)/(?<keyword>.+)/(?<page>[\{\}\d]+)$"
                //        , "~/max-templates/default/forums/threadmanagelogs.aspx?codename=${codename}&searchtype=${stype}&keyword=${keyword}&page=${page}"
                //    )

                //    , new RouteTable(
                //        @"\G(?<codename>.+)/(?<stype>\d+)/(?<keyword>.+)$"
                //        , "~/max-templates/default/forums/threadmanagelogs.aspx?codename=${codename}&searchtype=${stype}&keyword=${keyword}"
                //    )

                //)




                , new RouteTable(@"^new/(?<page>[\{\}\d]+)$"
                        , "~/max-templates/default/forums/new.aspx?page=${page}"
                )

                , new RouteTable("^systemforum/"

                    , new RouteTable(
                        @"\G(?<type>.+)/code-(?<codename>.+)/(?<page>[\{\}\d]+)$"
                        , "~/max-templates/default/forums/showsystemforum.aspx?type=${type}&codename=${codename}&page=${page}"
                    )

                    , new RouteTable(
                        @"\G(?<type>.+)/code-(?<codename>.+)$"
                        , "~/max-templates/default/forums/showsystemforum.aspx?type=${type}&codename=${codename}"
                    )


                    , new RouteTable(
                        @"\G(?<type>.+)/(?<page>[\{\}\d]+)$"
                        , "~/max-templates/default/forums/showsystemforum.aspx?type=${type}&page=${page}"
                    )


                    , new RouteTable(
                        @"\G(?<type>.+)$"
                        , "~/max-templates/default/forums/showsystemforum.aspx?type=${type}"
                    )

                )


                , new RouteTable("^systemforum$"
                        , "~/max-templates/default/forums/showsystemforum.aspx"
                )

                , new RouteTable("^moderatorcenter/"

                    , new RouteTable(
                        @"\Gcode-(?<codename>.*)/action-(?<action>.+)/tid-(?<tid>\d+)/pid-(?<pid>\d+)/pindex-(?<pindex>\d+)$"
                        , "~/max-templates/default/forums/moderatorcenter.aspx?codename=${codename}&action=${action}&threadid=${tid}&postid=${pid}&pageIndex=${pindex}"
                    )

                    //, new RouteTable(
                    //    @"\Gcode-(?<codename>.*)/action-(?<action>.+)/tid-(?<tid>\d+)/pid-(?<pid>\d+)$"
                    //    , "~/max-templates/default/forums/moderatorcenter.aspx?codename=${codename}&action=${action}&threadid=${tid}&postid=${pid}"
                    //)

                    //, new RouteTable(
                    //    @"\Gcode-(?<codename>.*)/action-(?<action>.+)/tid-(?<tid>\d+)$"
                    //    , "~/max-templates/default/forums/moderatorcenter.aspx?codename=${codename}&action=${action}&threadid=${tid}"
                    //)

                    //, new RouteTable(
                    //    @"\Gcode-(?<codename>.*)/action-(?<action>.+)/stype-(?<stype>.+)/keyword-(?<keyword>.+)/(?<page>[\{\}\d]+)$"
                    //    , "~/max-templates/default/forums/moderatorcenter.aspx?codename=${codename}&action=${action}&searchtype=${stype}&keyword=${keyword}&page=${page}"
                    //)

                    //, new RouteTable(
                    //    @"\Gcode-(?<codename>.*)/action-(?<action>.+)/stype-(?<stype>.+)/keyword-(?<keyword>.+)$"
                    //    , "~/max-templates/default/forums/moderatorcenter.aspx?codename=${codename}&action=${action}&searchtype=${stype}&keyword=${keyword}"
                    //)


                    //, new RouteTable(
                    //    @"\Gcode-(?<codename>.*)/action-(?<action>.+)/type-(?<type>.+)$"
                    //    , "~/max-templates/default/forums/moderatorcenter.aspx?codename=${codename}&action=${action}&type=${type}"
                    //)


                    //, new RouteTable(
                    //    @"\Gcode-(?<codename>.*)/action-(?<action>.+)/(?<page>[\{\}\d]+)$"
                    //    , "~/max-templates/default/forums/moderatorcenter.aspx?codename=${codename}&action=${action}&page=${page}"
                    //)


                    //, new RouteTable(
                    //    @"\Gcode-(?<codename>.*)/action-(?<action>.+)$"
                    //    , "~/max-templates/default/forums/moderatorcenter.aspx?codename=${codename}&action=${action}"
                    //)

                    //, new RouteTable(
                    //    @"\Gpid-(?<pid>\d+)/action-rate$"
                    //    , "~/max-dialogs/post-rate.aspx?postID=${pid}"
                    //)

                    //, new RouteTable(
                    //    @"\Gpid-(?<pid>\d+)/action-cancelrate$"
                    //    , "~/max-dialogs/post-cancelrate.aspx?postID=${pid}"
                    //)

                    , new RouteTable(
                        @"\Gpid-(?<pid>\d+)/action-(?<action>.+)$"
                        , "~/max-templates/default/forums/moderatorcenter.aspx?postID=${pid}&action=${action}"
                    )

                )


                 , new RouteTable(
                        @"^moderatorcenter$"
                        , "~/max-templates/default/forums/moderatorcenter.aspx"
                )

                 , new RouteTable(
                        @"^rankusers/(?<tid>\d+)$"
                        , "~/max-templates/default/forums/showrankusers.aspx?threadid=${tid}"
                )

                 , new RouteTable(
                        @"^threadlogs/(?<tid>\d+)$"
                        , "~/max-templates/default/forums/showthreadlogs.aspx?threadid=${tid}"
                )

                 , new RouteTable(
                        @"^attachmentexchanges/(?<attachid>\d+)/(?<pid>\d+)$"
                        , "~/max-templates/default/forums/attachmentexchanges.aspx?attachmentID=${attachid}&postID=${pid}"
                )

                 , new RouteTable(
                        @"^votedusers/(?<tid>\d+)$"
                        , "~/max-templates/default/forums/showvotedusers.aspx?threadid=${tid}"
                )

                 , new RouteTable(
                        @"^js$"
                        , "~/max-templates/default/forums/js.aspx"
                )

                 , new RouteTable(
                        @"^new$"
                        , "~/max-templates/default/forums/new.aspx"
                )

                , new RouteTable(
                    @"^(?<codename>.+)/thread-(?<tid>\d+)-(?<page>[\{\}\d]+)-(?<listpage>[\{\}\d]+)$"
                    , "~/max-templates/default/forums/view_thread.aspx?codename=${codename}&threadid=${tid}&page=${page}&listpage=${listpage}"
                )

                 , new RouteTable(
                    @"^(?<codename>.+)/thread-(?<tid>\d+)-(?<page>[\{\}\d]+)$"
                    , "~/max-templates/default/forums/view_thread.aspx?codename=${codename}&threadid=${tid}&page=${page}"
                )

                , new RouteTable(
                    @"^(?<codename>.+)/question-(?<tid>\d+)-(?<page>[\{\}\d]+)-(?<listpage>[\{\}\d]+)$"
                    , "~/max-templates/default/forums/view_question.aspx?codename=${codename}&threadid=${tid}&page=${page}&listpage=${listpage}"
                )

                 , new RouteTable(
                    @"^(?<codename>.+)/question-(?<tid>\d+)-(?<page>[\{\}\d]+)$"
                    , "~/max-templates/default/forums/view_question.aspx?codename=${codename}&threadid=${tid}&page=${page}"
                )

                , new RouteTable(
                    @"^(?<codename>.+)/poll-(?<tid>\d+)-(?<page>[\{\}\d]+)-(?<listpage>[\{\}\d]+)$"
                    , "~/max-templates/default/forums/view_poll.aspx?codename=${codename}&threadid=${tid}&page=${page}&listpage=${listpage}"
                )

                 , new RouteTable(
                    @"^(?<codename>.+)/poll-(?<tid>\d+)-(?<page>[\{\}\d]+)$"
                    , "~/max-templates/default/forums/view_poll.aspx?codename=${codename}&threadid=${tid}&page=${page}"
                )


                , new RouteTable(
                    @"^(?<codename>.+)/polemize-(?<tid>\d+)-(?<page>[\{\}\d]+)-(?<listpage>[\{\}\d]+)$"
                    , "~/max-templates/default/forums/view_polemize.aspx?codename=${codename}&threadid=${tid}&page=${page}&listpage=${listpage}"
                )

                 , new RouteTable(
                    @"^(?<codename>.+)/polemize-(?<tid>\d+)-(?<page>[\{\}\d]+)$"
                    , "~/max-templates/default/forums/view_polemize.aspx?codename=${codename}&threadid=${tid}&page=${page}"
                )

//, new RouteTable("^search/"

//                    // , new RouteTable(
//                //    @"\G(?<fid>\d+)/(?<mode>\d+)/(?<page>[\{\}\d]+)/(?<keyword>.+)$"
//                //    , "~/max-templates/default/forums/search.aspx?fid=${fid}&mode=${mode}&page=${page}&searchtext=${keyword}"
//                //)

//                    // , new RouteTable(
//                //    @"\G(?<fid>\d+)/(?<mode>\d+)/(?<keyword>.+)$"
//                //    , "~/max-templates/default/forums/search.aspx?fid=${fid}&mode=${mode}&searchtext=${keyword}"
//                //)

//                     , new RouteTable(
//                        @"\G(?<fid>\d+)/(?<mode>\d+)$"
//                        , "~/max-templates/default/forums/search.aspx?fid=${fid}&mode=${mode}"
//                    )

//                    // , new RouteTable(
//                //    @"\G(?<mode>\d+)/(?<keyword>.+)$"
//                //    , "~/max-templates/default/forums/search.aspx?mode=${mode}&searchtext=${keyword}"
//                //)
//                )

//                , new RouteTable(
//                    @"^search$"
//                    , "~/max-templates/default/forums/search.aspx"
//                )


                // , new RouteTable(
                //    @"^rss$"
                //    , "~/rss.aspx"
                //)

                 , new RouteTable(
                    @"^finalquestion/(?<fid>\d+)/(?<tid>\d+)$"
                    , "~/max-templates/default/forums/finalquestion.aspx?forumid=${fid}&threadid=${tid}"
                )


                 , new RouteTable(
                    @"^onlines/(?<type>.+)/(?<page>[\{\}\d]+)$"
                    , "~/max-templates/default/forums/onlines.aspx?type=${type}&page=${page}"
                )


                , new RouteTable("^onlines$"
                        , "~/max-templates/default/forums/onlines.aspx"
                )
               
                 , new RouteTable(
                        @"^(?<codename>.+)/catalog-(?<cid>\d+)-(?<page>[\{\}\d]+)$"
                        , "~/max-templates/default/forums/list.aspx?codename=${codename}&threadcatalogid=${cid}&page=${page}"
                    )

                 , new RouteTable(
                        @"^(?<codename>.+)/(?<action>[a-zA-Z]+)-(?<page>[\{\}\d]+)$"
                        , "~/max-templates/default/forums/list.aspx?codename=${codename}&action=${action}&page=${page}"
                    )

                 , new RouteTable(
                    @"^(?<codename>.+)/post$"
                    , "~/max-templates/default/forums/post.aspx?codename=${codename}"
                    )

                 , new RouteTable(
                @"^(?<codename>.+)/signinforum$"
                , "~/max-templates/default/forums/signinforum.aspx?codename=${codename}"
                )


            #region Post

                /*

                , new RouteTable("^post/"

                     , new RouteTable(
                        @"\G(?<codename>.+)/(?<action>.+)/(?<tid>\d+)/(?<pid>\d+)/type-(?<type>.+)$"
                        , "~/max-templates/default/forums/post.aspx?codename=${codename}&action=${action}&threadID=${tid}&postid=${pid}&type=${type}"
                    )

                    // , new RouteTable(
                    //    @"\G(?<codename>.+)/(?<action>.+)/(?<tid>\d+)/(?<pid>\d+)/pia-(?<pia>.+)$"
                    //    , "~/max-templates/default/forums/post.aspx?codename=${codename}&action=${action}&threadID=${tid}&postid=${pid}&posttndexalias=${pia}"
                    //)

                     , new RouteTable(
                        @"\G(?<codename>.+)/(?<action>.+)/(?<tid>\d+)/(?<pid>\d+)$"
                        , "~/max-templates/default/forums/post.aspx?codename=${codename}&action=${action}&threadID=${tid}&postid=${pid}"
                    )

                     , new RouteTable(
                        @"\G(?<codename>.+)/(?<action>.+)/(?<tid>\d+)$"
                        , "~/max-templates/default/forums/post.aspx?codename=${codename}&action=${action}&threadID=${tid}"
                    )

                     , new RouteTable(
                        @"\G(?<codename>.+)/(?<action>.+)$"
                        , "~/max-templates/default/forums/post.aspx?codename=${codename}&action=${action}"
                    )
                )

                */

            #endregion

            #endregion

, new RouteTable(@"^handler/(?<name>\w+)$", "~/handler.aspx?max_handler=${name}")

#if Publish
                , new RouteTable(@"^(?<path>max-js/.*)", "~/${path}.aspx")
#else
                , new RouteTable(@"^(?<path>max-(admin|dialogs|js)/.*)", "~/${path}.aspx")
                , new RouteTable(@"^(?<path>archiver/.*)", "~/${path}.aspx")
#endif
                //, new RouteTable(@"^(?<path>.*)", "~/max-templates/default/${path}.aspx")
                , new RouteTable(@"^(?<path>.*)", delegate(UrlScheme urlScheme) {

                    if (TemplateManager.IsTemplateFile(string.Concat("~/max-templates/default/", urlScheme.Main, ".aspx")))
                        return string.Concat("~/max-templates/default/", urlScheme.Main, ".aspx", urlScheme.QueryString);

                    else
                        return string.Empty;
                })
            );

        }

        private static string RawUrl
        {
            get { return HttpContext.Current.Request.RawUrl; }
        }

        public static bool Route()
        {
            string absolutePath = HttpContext.Current.Request.Url.AbsolutePath;

            if (IsNeedProcess(absolutePath) == false)
                return false;

            UrlScheme scheme = GetCurrentUrlScheme();

            RouteResult routeResult = s_RouteTable.Route(scheme);

            if (routeResult.Succeed)
            {
                string path2 = TemplateManager.ParseTemplate(routeResult.OriginalPath);

                HttpContext.Current.RewritePath(path2);

                return true;
            }
            else
            {
                if (AllSettings.Current.FriendlyUrlSettings.UrlFormat == UrlFormat.Query)
                {
                    if (IsRequestBySafeUrlRewrite())
                    {
                        HttpContext.Current.Response.Redirect("~/");
                    }
                }
            }

            return false;
        }

        public static bool IsRequestBySafeUrlRewrite()
        {
            string rawurl = RawUrl;

            return StringUtil.StartsWithIgnoreCase(rawurl, Globals.AppRoot + "/default.aspx?")
            || StringUtil.StartsWithIgnoreCase(rawurl, Globals.AppRoot + "/index.aspx?")
            || StringUtil.StartsWithIgnoreCase(rawurl, Globals.AppRoot + "/?");
        }

        public static bool IsNeedProcess(string absolutePath)
        {

            if (StringUtil.EndsWithIgnoreCase(absolutePath, ".aspx") || StringUtil.EndsWith(absolutePath, '/'))
                return true;

            UrlFormat urlFormat = AllSettings.Current.FriendlyUrlSettings.UrlFormat;

            switch (urlFormat)
            {

                case UrlFormat.Html:
                    return StringUtil.EndsWithIgnoreCase(absolutePath, ".html");

                case UrlFormat.Folder:
                    string ext = Path.GetExtension(absolutePath);
                    return string.Compare(ext, string.Empty, true) == 0;
                    
                default:
                    return false;
            }
        }


        const string cachekey_CurrentUrlScheme = "CurrentUrlScheme-Max";
        public static UrlScheme GetCurrentUrlScheme()
        {
            UrlScheme scheme;

            if (HttpContext.Current.Items[cachekey_CurrentUrlScheme] == null)
            {
                scheme = UrlScheme.Parse(RawUrl);

                ////检查是否板块的首页
                //string codename = null;
                //if (StringUtil.EndsWithIgnoreCase(scheme.Main, "/default"))
                //    codename = scheme.Main.Remove(scheme.Main.Length - 8);

                //else if (StringUtil.EndsWithIgnoreCase(scheme.Main, "/index"))
                //    codename = scheme.Main.Remove(scheme.Main.Length - 6);

                //else
                //    codename = scheme.Main;

                //if (string.IsNullOrEmpty(codename) != null && ForumBOV5.Instance.GetForum(codename) != null)
                //    scheme.Main = codename + "/list-1";

                HttpContext.Current.Items.Add(cachekey_CurrentUrlScheme, scheme);
            }
            else
                scheme = (HttpContext.Current.Items[cachekey_CurrentUrlScheme] as UrlScheme).Clone();

            return scheme;
        }


        public static string GetCurrentUrl()
        {
            string rawUrl = string.Empty;

            string url = RawUrl;

            int indexOfQuery = url.IndexOf('?');

            string query = string.Empty;
            string appAbsolutePath = url;

            if (indexOfQuery >= 0)
            {
                query = url.Substring(indexOfQuery + 1);
                appAbsolutePath = url.Substring(0, indexOfQuery);
            }

            if (Globals.AppRoot != "")
                appAbsolutePath = appAbsolutePath.Substring(Globals.AppRoot.Length);

            if (AllSettings.Current.FriendlyUrlSettings.UrlFormat == UrlFormat.Query)
            {
                if (IsRequestBySafeUrlRewrite())
                    rawUrl = query;

                if (rawUrl == string.Empty)
                {
                    if (string.Compare(appAbsolutePath, "/default.aspx", true) == 0 || string.Compare(appAbsolutePath, "/index.aspx", true) == 0)
                        rawUrl = "default";
                }
            }
            else
            {
                if (rawUrl == string.Empty && AllSettings.Current.FriendlyUrlSettings.UrlFormat != UrlFormat.Aspx)
                {
                    if (string.Compare(appAbsolutePath, "/default.aspx", true) == 0 || string.Compare(appAbsolutePath, "/index.aspx", true) == 0)
                    {
                        if (AllSettings.Current.FriendlyUrlSettings.UrlFormat == UrlFormat.Html)
                        {
                            if (string.IsNullOrEmpty(query))
                                return "/default.html";
                            else
                                return "/default.html?" + query;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(query))
                                return "/default";
                            else
                                return "/default?" + query;
                        }
                    }

                }

                if (string.IsNullOrEmpty(query))
                    rawUrl = appAbsolutePath;
                else
                    rawUrl = string.Concat(appAbsolutePath, "?", query);
            }

            return rawUrl;
        }


        /// <summary>
        /// 替换 $url(xxx/xx) 标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ReplaceUrlTag(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;
            return Pool<bbsMax.RegExp.TemplateTagUrlRegex>.Instance.Replace(content, ReplaceUrl);
        }

        private static string ReplaceUrl(Match match)
        {
            FriendlyUrlSettings settings = AllSettings.Current.FriendlyUrlSettings;

            string root = Globals.GetVirtualPath(SystemDirecotry.Root);

            string url;

            if (root == "/")
            {
                switch (settings.UrlFormat)
                {
                    case UrlFormat.Query:
                        url = "/?" + match.Groups[1].Value;
                        break;

                    case UrlFormat.Aspx:
                        url = string.Concat("/", match.Groups[1].Value, ".aspx");
                        break;

                    case UrlFormat.Html:
                        url = string.Concat("/", match.Groups[1].Value, ".html");
                        break;

                    default:
                        url = "/" + match.Groups[1].Value;
                        break;
                }
            }
            else
            {
                switch (settings.UrlFormat)
                {
                    case UrlFormat.Query:
                        url = string.Concat(root, "/?", match.Groups[1].Value);
                        break;

                    case UrlFormat.Aspx:
                        url = string.Concat(root, "/", match.Groups[1].Value, ".aspx");
                        break;

                    case UrlFormat.Html:
                        url = string.Concat(root, "/", match.Groups[1].Value, ".html");
                        break;

                    default:
                        url = string.Concat(root, "/", match.Groups[1].Value);
                        break;
                }
            }

            return url;
        }

        public static void JumpTo(string urlInfo)
        {
            HttpContext.Current.Response.Redirect(GetUrl(urlInfo));
        }

        public static void JumpTo(string urlInfo, string query)
        {
            HttpContext.Current.Response.Redirect(GetUrl(urlInfo, query));
        }

        public static void JumpToUrl(string rawurl, string query)
        {
            UrlScheme urlScheme = UrlScheme.Parse(rawurl);
            urlScheme.AttachQuery(query);
            HttpContext.Current.Response.Redirect(urlScheme.ToString());
        }

        public static void JumpToCurrentUrl()
        {
            UrlScheme urlScheme = GetCurrentUrlScheme();
            HttpContext.Current.Response.Redirect(urlScheme.ToString());
        }

        public static void JumpToCurrentUrl(string attachQuery)
        {
            UrlScheme urlScheme = GetCurrentUrlScheme();
            urlScheme.AttachQuery(attachQuery);
            HttpContext.Current.Response.Redirect(urlScheme.ToString());
        }

        public static void JumpToCurrentUrl(bool clearQuery)
        {
            UrlScheme urlScheme = GetCurrentUrlScheme();
            HttpContext.Current.Response.Redirect(urlScheme.ToString(!clearQuery, false));
        }

        public static void JumpToCurrentUrl(bool clearQuery, string attachQuery)
        {
            UrlScheme urlScheme = GetCurrentUrlScheme();
         
            if (clearQuery)
                urlScheme.ClearQuery();
            
            urlScheme.AttachQuery(attachQuery);
            HttpContext.Current.Response.Redirect(urlScheme.ToString(true, false));
        }

        /// <summary>
        /// 首页地址
        /// </summary>
        /// <returns></returns>
        public static string GetIndexUrl()
        {
            return GetUrl("default");
        }

        public static string GetUrl(string urlInfo)
        {
            //return GetUrl(urlInfo, null);
            switch (AllSettings.Current.FriendlyUrlSettings.UrlFormat)
            {
                case UrlFormat.Folder:
                    return string.Concat(Globals.AppRoot, "/", urlInfo);
                case UrlFormat.Aspx:
                    return string.Concat(Globals.AppRoot, "/", urlInfo, ".aspx");
                case UrlFormat.Html:
                    return string.Concat(Globals.AppRoot, "/", urlInfo, ".html");
                default:
                    return string.Concat(Globals.AppRoot, "/?", urlInfo);
            }
        }


        public static string GetUrl(string urlInfo, string query)
        {
            if (string.IsNullOrEmpty(query))
                return GetUrl(urlInfo);

            int indexOfQuery = urlInfo.IndexOf('?');

            if (indexOfQuery >= 0)
            {

                #region 处理原来有参数同时也要追加参数的情况

                NameValueCollection q2 = HttpUtility.ParseQueryString(query);

                StringBuffer newUrl = new StringBuffer();

                newUrl += Globals.AppRoot;

                if (AllSettings.Current.FriendlyUrlSettings.UrlFormat == UrlFormat.Query)
                    newUrl += "/?";

                else
                    newUrl += "/";

                string query1 = string.Empty;

                if (indexOfQuery >= 0)
                {
                    newUrl += urlInfo.Substring(0, indexOfQuery);

                    query1 = urlInfo.Substring(indexOfQuery + 1);
                }
                else
                    newUrl += urlInfo;

                switch (AllSettings.Current.FriendlyUrlSettings.UrlFormat)
                {
                    //case MaxLabs.bbsMax.Enums.UrlFormat.Folder:
                    //    newUrl += "/";
                    //    break;

                    case MaxLabs.bbsMax.Enums.UrlFormat.Aspx:
                        newUrl += ".aspx";
                        break;

                    case MaxLabs.bbsMax.Enums.UrlFormat.Html:
                        newUrl += ".html";
                        break;
                }


                NameValueCollection q1 = HttpUtility.ParseQueryString(query1);

                if (q2 != null)
                {
                    for (int i = 0; i < q2.Count; i++)
                    {
                        string value = q2[i];

                        q1[q2.GetKey(i)] = value;
                    }
                }

                StringBuilder newQuery = new StringBuilder("?");

                for (int i = 0; i < q1.Count; i++)
                {
                    newQuery.Append(HttpUtility.UrlEncode(q1.GetKey(i))).Append("=").Append(HttpUtility.UrlEncode(q1[i])).Append("&");
                }

                newQuery.Remove(newQuery.Length - 1, 1);

                newUrl += newQuery;

                #endregion

                return newUrl.ToString();
            }
            else
            {
                return string.Concat(GetUrl(urlInfo), "?", query);
            }

        }

        public static string GetUrlForAction(string urlInfo)
        {
            switch (AllSettings.Current.FriendlyUrlSettings.UrlFormat)
            {
                case UrlFormat.Folder:
                    return string.Concat(Globals.AppRoot, "/default.aspx?", urlInfo);
                case UrlFormat.Aspx:
                    return string.Concat(Globals.AppRoot, "/", urlInfo, ".aspx");
                case UrlFormat.Html:
                    return string.Concat(Globals.AppRoot, "/", urlInfo, ".html");
                default:
                    return string.Concat(Globals.AppRoot, "/", urlInfo);
            }
        }

		public static string GetUrlForAction(string urlInfo, string query)
		{
            if (string.IsNullOrEmpty(query))
                return GetUrlForAction(urlInfo);

            int indexOfQuery = urlInfo.IndexOf('?');

            if (indexOfQuery >= 0)
            {

                #region 处理原来有参数同时也要追加参数的情况

                NameValueCollection q2 = HttpUtility.ParseQueryString(query);

                StringBuffer newUrl = new StringBuffer();

                if (AllSettings.Current.FriendlyUrlSettings.UrlFormat == UrlFormat.Query)
                    newUrl += Globals.AppRoot + "/default.aspx?";

                else
                    newUrl += Globals.AppRoot + "/";

                string query1 = string.Empty;

                if (indexOfQuery >= 0)
                {
                    newUrl += urlInfo.Substring(0, indexOfQuery);

                    query1 = urlInfo.Substring(indexOfQuery + 1);
                }
                else
                    newUrl += urlInfo;

                switch (AllSettings.Current.FriendlyUrlSettings.UrlFormat)
                {
                    //case MaxLabs.bbsMax.Enums.UrlFormat.Folder:
                    //    newUrl += "/";
                    //    break;

                    case MaxLabs.bbsMax.Enums.UrlFormat.Aspx:
                        newUrl += ".aspx";
                        break;

                    case MaxLabs.bbsMax.Enums.UrlFormat.Html:
                        newUrl += ".html";
                        break;
                }


                NameValueCollection q1 = HttpUtility.ParseQueryString(query1);

                if (q2 != null)
                {
                    for (int i = 0; i < q2.Count; i++)
                    {
                        string value = q2[i];

                        q1[q2.GetKey(i)] = value;
                    }
                }

                StringBuilder newQuery = new StringBuilder("?");

                for (int i = 0; i < q1.Count; i++)
                {
                    newQuery.Append(HttpUtility.UrlEncode(q1.GetKey(i))).Append("=").Append(HttpUtility.UrlEncode(q1[i])).Append("&");
                }

                newQuery.Remove(newQuery.Length - 1, 1);

                newUrl += newQuery;

                #endregion

                return newUrl.ToString();
            }
            else
            {
                return string.Concat(GetUrlForAction(urlInfo), "?", query);
            }

		}

        public static string GetUrlInfo(string url)
        {
            string webRoot = Globals.AppRoot + "/";
            string fullWebRoot = Globals.FullAppRoot + "/";

            UrlFormat urlFormat = AllSettings.Current.FriendlyUrlSettings.UrlFormat;

            int baseUrlLength = 0;// = urlFormat == UrlFormat.Query ? 1 : 0;

            string urlInfo;

            if (StringUtil.StartsWithIgnoreCase(url, webRoot))
            {
				baseUrlLength += webRoot.Length;
            }
            else if (StringUtil.StartsWithIgnoreCase(url, fullWebRoot))
            {
                baseUrlLength += fullWebRoot.Length;
            }
            else
                return string.Empty;

			if (urlFormat == UrlFormat.Query)
			{
				//确认是本程序内的地址以保证安全
				if (UrlUtil.IsUrlInApp(url))
				{
					if (url.IndexOf("default.aspx?", baseUrlLength) == baseUrlLength)
						baseUrlLength += 13;
					else if (url.IndexOf("index.aspx?", baseUrlLength) == baseUrlLength)
						baseUrlLength += 11;
					else
						baseUrlLength += 1;
				}
				else
					baseUrlLength += 1;
			}

            if (url.Length > baseUrlLength)
                urlInfo = url.Substring(baseUrlLength);
            else
                urlInfo = "default";


            int queryIndex = urlInfo.IndexOf('?');

            if (queryIndex != -1)
                urlInfo = urlInfo.Substring(0, queryIndex);

            if (urlFormat == UrlFormat.Aspx)
            {
                if (urlInfo.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
                    urlInfo = urlInfo.Substring(0, urlInfo.Length - 5);
                else
                    return string.Empty;
            }
            else if (urlFormat == UrlFormat.Html)
            {
                if (urlInfo.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                    urlInfo = urlInfo.Substring(0, urlInfo.Length - 5);
                else
                    return string.Empty;
            }

            return urlInfo.ToLower();
        }
	}
}