<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_forumhtmlhead_.aspx"-->
<link rel="alternate" type="application/rss+xml" href="$root/rss.aspx?ticket=$ticket&forumid=$forumid" title="$PageTitle" />
</head>
<body>
<div class="container">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main nosidebar section-forumtopics">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">
                            
                            <div class="clearfix forumtoptext">
                                <div class="forumnotice" id="forumnotice">
                                    <h3 class="forumnotice-label">公告</h3>
                                    <ul class="forumnotice-list" id="forumnotice_list">
                                        <!--[if $announcements.count == 0]-->
                                        <li>没有任何公告</li>
                                        <!--[else]-->
                                            <!--[loop $announcement in $announcements]-->
                                            <!--[if $announcement.AnnouncementType == AnnouncementType.Text]-->
                                        <li><a href="$url(announcements)?id=$announcement.announcementID#$announcement.announcementID" target="_blank">$announcement.Subject</a> <span class="date">$outputFriendlyDateTime($announcement.BeginDate)</span></li>
                                            <!--[else]-->
                                        <li><a href="$formatLink($announcement.Content)" target="_blank">$announcement.Subject</a> <span class="date">$outputFriendlyDateTime($announcement.BeginDate)</span></li>
                                            <!--[/if]-->
                                            <!--[/loop]-->
                                        <!--[/if]-->
                                    </ul>
                                </div>
                                <!--[if $announcements.count > 0]-->
                                <script type="text/javascript">
                                    announcement("forumnotice_list");
                                </script>
                                <!--[/if]-->
                                <div class="forumstatus">
                                     版主: $GetModeratorLinks($forum,@"<a class=""fn"" href=""{0}"">{1}</a>{2}",", ")
                                     <a class="rss" href="$root/rss.aspx?ticket=$ticket&forumid=$forumid">订阅</a>
                                </div>
                            </div>
                            
                            <!--[if $IsNormalThreads && $forum.SubForumsForList.Count > 0]-->
                                <!--[if $forum.ColumnSpan < 2]-->
                            <div class="panel forumthumbs">
                                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                                    <h3 class="panel-title"><span>子板块</span></h3>
                                    <p class="panel-toggle"><a class="collapse" href="#" title="收起/展开">收起/展开</a></p>
                                </div></div></div>
                                <div class="panel-body">
                                    <table>
                                        <!--[loop $subforum in $forum.SubForumsForList]-->
                                        <tr>
                                            <td class="forum-info">
                                                <dl class="forumunit">
                                                    <dt class="title">
                                                        <a href="$url($subforum.CodeName/list-1)" <!--[if $subforum.ForumType==ForumType.Link && $subforum.ExtendedAttribute.LinkOpenByNewWidow]-->target="_blank"<!--[/if]-->>$subforum.ForumName</a>
                                                        <!--[if $subforum.TodayPostsWithSubForums > 0]-->
                                                        <span class="status">(今日:<em>$subforum.TodayPostsWithSubForums</em>)</span>
                                                        <!--[/if]-->
                                                    </dt>
                                                    <dd class="image">
                                                        <a href="$url($subforum.CodeName/list-1)" target="_blank">
                                                        <!--[if $subforum.logoUrl!=""]-->
                                                            <img src="$subforum.logoUrl" alt="" />
                                                        <!--[else]-->
                                                            <img src="$skin/images/icons/topicicon_default.png" alt="" />
                                                        <!--[/if]-->
                                                        <!--[if $subforum.ForumType!=ForumType.Link && $subforum.Password != ""]-->
                                                            <img class="overlayicon" src="$skin/images/icons/overlay_lock.png" alt="" title="加密版块" />
                                                        <!--[else if $subforum.CanVisit($my)]-->
                                                            <!--[if $subforum.ForumType == ForumType.Link]-->
                                                            <img class="overlayicon" src="$skin/images/icons/overlay_link.png" alt="" title="外部版块" />
                                                            <!--[else]-->
                                                            <img class="overlayicon" src="$skin/images/icons/overlay_accessible.png" alt="" title="有访问权" />
                                                            <!--[/if]-->
                                                        <!--[else]-->
                                                            <img class="overlayicon" src="$skin/images/icons/overlay_ban.png" alt="" title="无访问权" />
                                                        <!--[/if]-->
                                                        </a>
                                                    </dd>
                                                    <dd class="intro">
                                                        $subforum.Description
                                                    </dd>
                                                    <!--[if $DisplaySubforumsInIndexpage && $subforum.SubForumsForList.Count>0]-->
                                                    <dd class="childforum">
                                                        子版块:
                                                        <!--[loop $tempForum in $subforum.SubForumsForList with $i]-->
                                                        <a href="$url($tempForum.CodeName/list-1)" <!--[if $tempforum.ForumType==ForumType.Link && $tempforum.ExtendedAttribute.LinkOpenByNewWidow]-->target="_blank"<!--[/if]-->>$tempForum.ForumName</a><!--[if $i < $subforum.SubForumsForList.Count-1]-->, <!--[/if]-->
                                                        <!--[/loop]-->
                                                    </dd>
                                                    <!--[/if]-->
                                                    <dd class="boardmaster">
                                                        版主: $GetModeratorLinks($subforum,@"<a class=""fn"" href=""{0}"">{1}</a>{2}",", ")
                                                    </dd>
                                                </dl>
                                            </td>
                                            <td class="forum-stats">
                                                <span class="stats-topic">$subforum.TotalThreadsWithSubForums</span> /
                                                <span class="stats-post">$subforum.TotalPostsWithSubForums</span>
                                            </td>
                                            <td class="forum-last">
                                                <!--[if $CanSeeLastUpdate($subforum)]-->
                                                    <!--[if $subforum.LastThread == null]-->
                                                <p class="lastpost-nopost">
                                                    暂时没有主题
                                                </p>
                                                    <!--[else]-->
                                                <p class="lastpost-title">
                                                    <a href="$url($subforum.codename/$subforum.LastThread.ThreadTypeString-$subforum.LastThread.ThreadID-$subforum.LastThread.TotalPages-1)#last">$CutString($subforum.LastThread.SubjectText,30)</a>
                                                </p>
                                                <p class="lastpost-user">
                                                    <!--[if $subforum.LastThread.LastReplyUserID != 0]-->
                                                    <a class="fn" href="$url(space/$subforum.LastThread.LastReplyUserID)" target="_blank">$subforum.LastThread.LastReplyUsername</a>
                                                    <!--[else]-->
                                                        <!--[if $subforum.LastThread.LastReplyUsername != ""]-->
                                                    游客:$subforum.LastThread.LastReplyUsername
                                                        <!--[else]-->
                                                    匿名游客
                                                        <!--[/if]-->
                                                    <!--[/if]-->
                                                    <span class="date">$outputFriendlyDateTime($subforum.LastThread.UpdateDate)</span>
                                                </p>
                                                    <!--[/if]-->
                                                <!--[else]-->
                                                <p class="inaccessible">
                                                    未获得查看权限
                                                </p>
                                                <!--[/if]-->
                                            </td>
                                        </tr>
                                        <!--[/loop]-->
                                    </table>
                                </div>
                                <div class="panel-foot"><div><div>-</div></div></div>
                            </div>
                                <!--[else]-->
                            <div class="panel forumcates">
                                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                                    <h3 class="panel-title"><span>子板块</span></h3>
                                    <p class="panel-toggle"><a class="collapse" href="#" title="收起/展开">收起/展开</a></p>
                                </div></div></div>
                                <div class="panel-body">
                                <!--[loop $subforum in $forum.SubForumsForList with $LoopIndex]-->
                                    <!--[if ($LoopIndex+1) % ($forum.ColumnSpan) == 1]-->
                                    <ul class="clearfix multicolumns$forum.ColumnSpan">
                                    <!--[/if]-->
                                        <li>
                                            <dl class="forumunit">
                                                <dt class="title">
                                                    <a href="$url($subforum.CodeName/list-1)" <!--[if $subforum.ForumType==ForumType.Link && $subforum.ExtendedAttribute.LinkOpenByNewWidow]-->target="_blank"<!--[/if]-->>$subforum.ForumName</a>
                                                    <!--[if $subforum.TodayPostsWithSubForums > 0]-->
                                                    <span class="status">(今日:<em>$subforum.TodayPostsWithSubForums</em>)</span>
                                                    <!--[/if]-->
                                                </dt>
                                                <dd class="image">
                                                    <a href="$url($subforum.CodeName/list-1)" target="_blank">
                                                        <!--[if $subforum.logoUrl!=""]-->
                                                        <img src="$subforum.logoUrl" alt="" />
                                                        <!--[else]-->
                                                        <img src="$skin/images/icons/topicicon_default.png" alt="" />
                                                        <!--[/if]-->
                                                        <!--[if $subforum.ForumType!=ForumType.Link && $subforum.Password != ""]-->
                                                        <img class="overlayicon" src="$skin/images/icons/overlay_lock.png" alt="" title="加密版块" />
                                                        <!--[else if $subforum.CanVisit($my)]-->
                                                            <!--[if $subforum.ForumType == ForumType.Link]-->
                                                        <img class="overlayicon" src="$skin/images/icons/overlay_link.png" alt="" title="外部版块" />
                                                            <!--[else]-->
                                                        <img class="overlayicon" src="$skin/images/icons/overlay_accessible.png" alt="" title="有访问权" />
                                                            <!--[/if]-->
                                                        <!--[else]-->
                                                        <img class="overlayicon" src="$skin/images/icons/overlay_ban.png" alt="" title="无访问权" />
                                                        <!--[/if]-->
                                                    </a>
                                                </dd>
                                                <dd class="stat">
                                                    主题: <span class="stat-topic">$subforum.TotalThreadsWithSubForums</span>,
                                                    帖数: <span class="stat-topic">$subforum.TotalPostsWithSubForums</span>
                                                </dd>
                                                <!--[if $DisplaySubforumsInIndexpage && $subforum.SubForumsForList.Count > 0]-->
                                                <dd class="childforum">
                                                    子版块:
                                                    <!--[loop $tempForum in $subforum.SubForumsForList with $i]-->
                                                    <a href="$url($tempForum.CodeName/list-1)" <!--[if $tempforum.ForumType==ForumType.Link && $tempforum.ExtendedAttribute.LinkOpenByNewWidow]-->target="_blank"<!--[/if]-->>$tempForum.ForumName</a><!--[if $i < $subforum.SubForumsForList.Count-1]-->, <!--[/if]-->
                                                    <!--[/loop]-->
                                                </dd>
                                                <!--[/if]-->
                                                <dd class="last">
                                                    最后发表:
                                                    <!--[if $subforum.LastThread != null]-->
                                                    <a href="$url($subforum.codename/$subforum.LastThread.ThreadTypeString-$subforum.LastThread.ThreadID-$subforum.LastThread.TotalPages-1)#last">$outputFriendlyDateTime($subforum.LastThread.UpdateDate)</a>
                                                    <!--[else]-->
                                                    -
                                                    <!--[/if]-->
                                                </dd>
                                            </dl>
                                        </li>
                                    <!--[if ($LoopIndex+1) % ($forum.ColumnSpan)==0  || ($LoopIndex+1) == $forum.SubForumsForList.Count]-->
                                    </ul>
                                    <!--[/if]-->
                                <!--[/loop]-->
                                </div>
                                <div class="panel-foot"><div><div>-</div></div></div>
                            </div>
                                <!--[/if]-->
                            <!--[/if]-->
                            
                        <!--[if $forum.ForumType == ForumType.Normal && $IsCatalogForum == false]-->
                            <!--[if $IsNormalThreads && $forum.Readme != ""]-->
                            <div class="forumboard">
                                <div class="clearfix forumboard-inner">
                                    $forum.Readme
                                </div>
                            </div>
                            <!--[/if]-->
                            
                            <div class="clearfix postoperate">
                                <!--[if $IsNormalThreads || $IsShowModeratorManageLink]-->
                                <div class="topic-action">
                                    <div class="action-button">
                                        <!--[if $IsNormalThreads]-->
                                        <a id="action-newtopic" class="action-newtopic" href="$url($codename/post)?action=thread" title="发新帖"><span><img src="$skin/images/theme/button_post.png" alt="" /></span></a>
                                        <!--[/if]-->
                                        <!--[if $IsShowModeratorManageLink]-->
                                        <a id="action-manage" class="action-manage" href="#"><span><img src="$skin/images/theme/button_manage.png" alt="" /></span></a>
                                        <!--[/if]-->
                                    </div>
                                </div>
                                <!--[/if]-->
                                
                                <!--[if $IsShowModeratorManageLink]-->
                                <div id="action-manage-list" class="dropdownmenu-wrap managetoipc-dropdownmenu" style="display:none;">
                                    <div class="dropdownmenu">
                                        <div class="dropdownmenu-inner">
                                            <h3 class="managetoipc-title">主题管理</h3>
                                            <ul class="clearfix managetoipc-list">
                                                <!--[if $IsNormalThreads == false]-->
                                                <li><a href="javascript:void(postthread2('deletethread.aspx'))">删除</a></li>
                                                <!--[/if]-->
                                                <!--[if $IsRecycleBin]-->
                                                <li><a href="javascript:void(postthread2('revertthread.aspx'))">还原</a></li>
                                                <!--[else if $IsUnapprovedThreads]-->
                                                <li><a href="javascript:void(postthread2('approvethread.aspx'))">通过审核</a></li>
                                                <!--[else if $IsUnapprovedPostsThreads]-->
                                                <li><a href="javascript:void(postthread2('approvepostsbythreadid.aspx'))">主题的所有回复通过审核</a></li>
                                                <li><a href="javascript:void(postthread2('deleteunapprovedpostbythreadid.aspx'))">删除主题的所有未审核回复</a></li>
                                                <!--[/if]-->
                                                <!--[if $IsNormalThreads]-->
                                                $GetModeratorActionLinks(@"<li><a href=""javascript:void(postthread('{1}'))"">{0}</a></li>","")
                                                <!--[/if]-->
                                            </ul>
                                            <h3 class="managetoipc-title">其他操作</h3>
                                            <ul class="clearfix managetoipc-list">
                                                <li><a href="javascript:void(0);" onclick="openPage('UpdateForumReadme');">修改本版规则</a></li>
                                                <li><a href="$url($forum.CodeName/unapproved-1)">本版未审核主题</a></li>
                                                <li><a href="$url($forum.CodeName/unapprovedpost-1)">本版未审核回复</a></li>
                                                <li><a href="$url($forum.CodeName/recycled-1)">本版回收站</a></li>
                                                <li><a href="$dialog/forumshielduers.aspx?forumid=$Forum.ForumID" onclick="return openDialog(this.href);">本版屏蔽用户列表</a></li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                                <!--[/if]-->
                                
                                <div id="action-newtopic-list" class="dropdownmenu-wrap newtoipc-dropdownmenu" style="display:none;">
                                    <!--[if $CanCreateThread || $CanCreatePoll || $CanCreateQuestion || $CanCreatePolemize]-->
                                    <div class="dropdownmenu">
                                        <ul class="dropdownmenu-list">
                                            <!--[if $CanCreateThread]-->
                                            <li><a class="icon1" href="$url($codename/post)?action=thread">主题</a></li>
                                            <!--[/if]-->
                                            <!--[if $CanCreatePoll]-->
                                            <li><a class="icon2" href="$url($codename/post)?action=poll">投票</a></li>
                                            <!--[/if]-->
                                            <!--[if $CanCreateQuestion]-->
                                            <li><a class="icon3" href="$url($codename/post)?action=question">提问</a></li>
                                            <!--[/if]-->
                                            <!--[if $CanCreatePolemize]-->
                                            <li><a class="icon4" href="$url($codename/post)?action=polemize">辩论</a></li>
                                            <!--[/if]-->
                                        </ul>
                                    </div>
                                    <!--[/if]-->
                                </div>
                                
                                <div class="pagination">
                                    <div class="pagination-inner">
                                        <!--[ajaxpanel id="ap_threads_toppager" idonly="true"]-->
                                        <!--[if !$IsNormalThreads]-->
                                        <a class="back" href="$url($forum.codename/list-1)" title="返回列表页">&laquo; 返回列表页</a>
                                        <!--[/if]-->
                                        <!--[pager name="ThreadListPager" skin="../_inc/_pager_bbs.aspx"]-->
                                        <!--[/ajaxpanel]-->
                                    </div>
                                </div>
                            </div>
                            
                            <!--[ajaxpanel id="ap_threads" idonly="true"]-->
                            <div class="panel forumtopics">
                                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                                    <h3 class="panel-title"><span>$forum.ForumName</span></h3>
                                    <p class="forum-stat">主题 $forum.TotalThreads / 回复 $forum.TotalPosts</p>
                                </div></div></div>
                                <div class="panel-body">
                                <form id="thread-list-form" action="#">
                                    <!--[if $IsNormalThreads]-->
                                    <!--[if $forum.ThreadCatalogStatus != ThreadCatalogStatus.DisEnable && $ThreadCatalogs.Count > 0]-->
                                    <div class="forumtopic-category">
                                        <div class="panel-tab">
                                            <ul class="clearfix tab-list">
                                                $GetThreadCatalogLinks(@"<li><a href=""{0}"">{1}</a></li>",@"<li><a href=""{0}"" class=""current"">{1}</a></li>")
                                            </ul>
                                        </div>
                                    </div>
                                    <!--[/if]-->
                                    <!--[/if]-->
                                    <table>
                                    <!--[if $IsNormalThreads]-->
                                        <tbody class="forumtopics-head">
                                            <tr>
                                                <td class="icon">
                                                    &nbsp;
                                                </td>
                                                <td class="author">
                                                   <a id="order-publisheddate" href="$AttachQueryString("sorttype=createdate",false)">发表时间</a>
                                                </td>
                                                <td class="title">
                                                    <a class="dropdown topicfilter topicfilter-type" id="topic-types" href="###">类型</a>
                                                    <div id="topic-types-list" class="dropdownmenu-wrap topictype-list" style="display:none;">
                                                        <div class="dropdownmenu">
                                                            <ul class="dropdownmenu-list">
                                                                $GetThreadTypeLinks(@"<li><a href=""{0}"">{1}</a></li>",@"<li><a href=""{0}"" class=""checked"">{1}</a></li>")
                                                            </ul>
                                                        </div>
                                                    </div>
                                                    <span class="label">主题</span>
                                                    <span class="topicfilter topicfilter-value">
                                                        $GetThreadTypeLinks2(@"<a href=""{0}""><span>{1}</span></a>",@"<a href=""{0}"" class=""current""><span>{1}</span></a>")
                                                    </span>
                                                    <span class="edge">|</span>
                                                    <span class="label">时间</span>
                                                    <span class="topicfilter topicfilter-date">
                                                        <a href="$AttachQueryString("days=1",false)" $setDayCurreantClass(1,"current")><span>一天</span></a>
                                                        <a href="$AttachQueryString("days=2",false)" $setDayCurreantClass(2,"current")><span>两天</span></a>
                                                        <a href="$AttachQueryString("days=7",false)" $setDayCurreantClass(7,"current")><span>周</span></a>
                                                        <a href="$AttachQueryString("days=30",false)" $setDayCurreantClass(30,"current")><span>月</span></a>
                                                        <a href="$AttachQueryString("days=120",false)" $setDayCurreantClass(120,"current")><span>季</span></a>
                                                    </span>
                                                </td>
                                                <td class="last">
                                                    <a id="order-lastauthor" href="$AttachQueryString("sorttype=lastreplydate",false)">最后更新</a>
                                                </td>
                                                <td class="stats">
                                                    <a href="$AttachQueryString("sorttype=replies",false)">回复</a>/<a id="order-views" href="$AttachQueryString("sorttype=views",false)">查看</a>
                                                </td>
                                            </tr>
                                        </tbody>
                                    <!--[else]-->
                                        <tbody class="forumtopics-head">
                                            <tr>
                                                <td class="icon">&nbsp;</td>
                                                <td class="author">发布</td>
                                                <td class="title">主题</td>
                                                <td class="last"><span id="order-lastauthor">最后更新</span></td>
                                                <td class="stats">回复/<span id="order-views">查看</span></td>
                                            </tr>
                                        </tbody>
                                    <!--[/if]-->
                                        <!--[if $StickThreads.Count > 0]-->
                                        <tbody class="forumtopics-list forumtopics-stickylist">
                                            <!--[loop $thread in $StickThreads]-->
                                            <tr>
                                                <td class="icon">
                                                    <a href="$GetThreadUrl($thread,false)" target="_blank">
                                                        <!--[if $thread.ThreadStatus == ThreadStatus.GlobalSticky]-->
                                                        <img src="$skin/images/icons/topic_pinned.gif" alt="" />
                                                        <!--[else]-->
                                                        <img src="$skin/images/icons/topic_sticky.gif" alt="" />
                                                        <!--[/if]-->
                                                    </a>
                                                </td>
                                                <td class="author">
                                                <!--[if $thread.PostUserID!=0]-->
                                                    <a href="$url(space/$thread.PostUserID)" class="fn username" target="_blank">$thread.PostUsername</a>
                                                <!--[else]-->
                                                    <!--[if $thread.PostUsername!=""]-->
                                                    游客:<span class="fn">$thread.PostUsername</span>
                                                    <!--[else]-->
                                                    匿名游客
                                                    <!--[/if]-->
                                                <!--[/if]-->
                                                    <span class="date">$outputDate($thread.CreateDate)</span>
                                                </td>
                                                <td class="title">
                                                    <!--[if $IsShowCheckBox($thread)]-->
                                                    <input type="checkbox" name="threadIDs" value="$thread.ThreadID" />
                                                    <!--[/if]-->
                                                    <!--[if $EnablePostIcon]-->
                                                    $GetIcon($thread.IconID)
                                                    <!--[/if]-->
                                                    $GetThreadCatalogName($thread.ThreadCatalogID, @"<span class=""cate"">[{0}]</span>")
                                                    $GetThreadLink($thread, 60, @"<a href=""{0}"">{1}</a>")
                                                    <!--[if $IsNewThread($thread)]-->
                                                    <img src="$skin/images/icons/topic_new.gif" alt="" title="最新主题" />
                                                    <!--[/if]-->
                                                    <!--[if $thread.ThreadType == ThreadType.Poll]-->
                                                    <img src="$root/max-assets/icon/poll.gif" alt="[投票]" title="投票" />
                                                    <!--[else if $thread.ThreadType == ThreadType.Question]-->
                                                    <img src="$root/max-assets/icon/ask.gif" alt="[问答]" title="问答" />
                                                    <!--[else if $thread.ThreadType == ThreadType.Polemize]-->
                                                    <img src="$root/max-assets/icon/polemize.gif" alt="[辩论]" title="辩论" />
                                                    <!--[/if]-->
                                                    <!--[if $thread.IsValued]-->
                                                    <img src="$root/max-assets/icon/diamond_blue.gif" alt="[精华]" title="精华" />
                                                    <!--[/if]-->
                                                    
                                                    <!--[if $thread.AttachmentType == ThreadAttachType.Image]-->
                                                    <img src="$root/max-assets/icon/photo.gif" alt="本帖有图片" title="本帖有图片" />
                                                    <!--[else if $thread.AttachmentType == ThreadAttachType.Normal]-->
                                                    <img src="$root/max-assets/icon/attachment.gif" alt="本帖有附件" title="本帖有附件" />
                                                    <!--[/if]-->
                                                    
                                                    <!--[if $thread.Price > 0]-->
                                                    <img src="$root/max-assets/icon/coin_gold.gif" alt="" />
                                                    价格:$GetSellThreadPoint($thread.PostUserID).Name $thread.Price $GetSellThreadPoint($thread.PostUserID).UnitName
                                                    <!--[/if]-->
                                                    <!--[if $thread.Rank > 0]-->
                                                    <a href="$Dialog/rankusers.aspx?threadid=$thread.threadid" onclick="return openDialog(this.href, null)" title="当前{=$thread.Rank}分"><img src="$skin/images/icons/postrank_{=$thread.Rank}.gif" alt="" /></a>
                                                    <!--[/if]-->
                                                    $GetThreadPager($thread,@"<span class=""topic-pages"">{0}</span>", @"<a href=""{0}"">{1}</a>")
                                                </td>
                                                <td class="last">
                                                <!--[if $thread.LastReplyUserID != 0]-->
                                                    <a href="$url(space/$thread.LastReplyUserID)" class="fn username" target="_blank">$thread.LastReplyUsername</a>
                                                <!--[else]-->
                                                    <!--[if $thread.LastReplyUsername != ""]-->
                                                    游客:<span class="fn">$thread.LastReplyUsername</span>
                                                    <!--[else]-->
                                                    匿名游客
                                                    <!--[/if]-->
                                                <!--[/if]-->
                                                    <a class="date" href="$GetThreadUrl($thread,true)#last">$outputFriendlyDateTime($thread.UpdateDate)</a>
                                                </td>
                                                <td class="stats">
                                                    <span class="stats-reply">$thread.TotalReplies</span> / <span class="stats-view">$thread.TotalViews</span>
                                                </td>
                                            </tr>
                                            <!--[/loop]-->
                                        </tbody>
                                        <!--[/if]-->
                                        <!--[if $NormalThreads.Count == 0 && $StickThreads.Count != 0]-->
                                        <!--[else]-->
                                            <!--[if $NormalThreads.Count > 0 && $StickThreads.Count > 0]-->
                                                <!--[if $HasInListAD]-->
                                        <tbody class="forumtopics-gap forumtopics-gap-ad">
                                            <tr>
                                                <td colspan="5">
                                                    <div class="ad-banner-topicgap">$InListAD</div>
                                                </td>
                                            </tr>
                                        </tbody>
                                                <!--[else]-->
                                        <tbody class="forumtopics-gap">
                                            <tr>
                                                <td colspan="5">&nbsp;</td>
                                            </tr>
                                        </tbody>
                                                <!--[/if]-->
                                            <!--[/if]-->
                                            <!--[if $NormalThreads.Count > 0]-->
                                        <tbody class="forumtopics-list">
                                                <!--[loop $thread in $NormalThreads]-->
                                            <tr>
                                                <td class="icon">
                                                    <a href="$GetThreadUrl($thread,false)" target="_blank">
                                                        <!--[if $thread.IsLocked]-->
                                                        <img src="$skin/images/icons/topic_lock.gif" alt="" title="锁定主题" />
                                                        <!--[else if $thread.TotalReplies >= $HotThreadRequireReplies]-->
                                                        <img src="$skin/images/icons/topic_hot.gif" alt="" title="热门主题" />
                                                        <!--[else]-->
                                                        <img src="$skin/images/icons/topic_normal.gif" alt="" title="一般主题" />
                                                        <!--[/if]-->
                                                    </a>
                                                </td>
                                                <td class="author">
                                                <!--[if $thread.PostUserID!=0]-->
                                                    <a href="$url(space/$thread.PostUserID)" class="fn username" target="_blank">$thread.PostUsername</a>
                                                <!--[else]-->
                                                    <!--[if $thread.PostUsername!=""]-->
                                                    游客:<span class="fn">$thread.PostUsername</span>
                                                    <!--[else]-->
                                                    匿名游客
                                                    <!--[/if]-->
                                                <!--[/if]-->
                                                    <span class="date">$outputDate($thread.CreateDate)</span>
                                                </td>
                                                <td class="title">
                                                    <!--[if $IsShowCheckBox($thread)]-->
                                                    <input type="checkbox" name="threadIDs" value="$thread.ThreadID" />
                                                    <!--[/if]-->
                                                    $GetIcon($thread.IconID)
                                                    $GetThreadCatalogName($thread.ThreadCatalogID, @"<span class=""cate"">[{0}]</span>")
                                                    $GetThreadLink($thread, 60, @"<a href=""{0}"">{1}</a>")
                                                    <!--[if $IsUnapprovedPostsThreads]-->
                                                    <span class="unapprovecomments">(未审核回复: $thread.UnApprovedPostsCount)</span>
                                                    <!--[/if]-->
                                                    <!--[if $IsNewThread($thread)]-->
                                                    <img src="$skin/images/icons/topic_new.gif" alt="" title="最新主题" />
                                                    <!--[/if]-->
                                                    <!--[if $thread.ThreadType == ThreadType.Poll]-->
                                                    <img src="$root/max-assets/icon/poll.gif" alt="[投票]" title="投票" />
                                                    <!--[else if $thread.ThreadType == ThreadType.Question]-->
                                                    <img src="$root/max-assets/icon/ask.gif" alt="[问答]" title="问答" />
                                                    <!--[else if $thread.ThreadType == ThreadType.Polemize]-->
                                                    <img src="$root/max-assets/icon/polemize.gif" alt="[辩论]" title="辩论" />
                                                    <!--[/if]-->
                                                    <!--[if $thread.IsValued]-->
                                                    <img src="$root/max-assets/icon/diamond_blue.gif" alt="[精华]" title="精华" />
                                                    <!--[/if]-->
                                                    <!--[if $thread.AttachmentType == ThreadAttachType.Image]-->
                                                    <img src="$root/max-assets/icon/photo.gif" alt="本帖有图片" title="本帖有图片" />
                                                    <!--[else if $thread.AttachmentType == ThreadAttachType.Normal]-->
                                                    <img src="$root/max-assets/icon/attachment.gif" alt="本帖有附件" title="本帖有附件" />
                                                    <!--[/if]-->
                                                    
                                                    <!--[if $thread.Price > 0]-->
                                                    <img src="$root/max-assets/icon/coin_gold.gif" alt="" />
                                                    价格:$GetSellThreadPoint($thread.PostUserID).Name $thread.Price $GetSellThreadPoint($thread.PostUserID).UnitName
                                                    <!--[/if]-->
                                                    <!--[if $thread.Rank > 0]-->
                                                    <a href="$Dialog/rankusers.aspx?threadid=$thread.threadid" onclick="return openDialog(this.href, null)"" title="当前{=$thread.Rank}分"><img src="$skin/images/icons/postrank_{=$thread.Rank}.gif" alt="" /></a>
                                                    <!--[/if]-->
                                                    $GetThreadPager($thread,@"<span class=""topic-pages"">{0}</span>", @"<a href=""{0}"">{1}</a>")
                                                </td>
                                                <td class="last">
                                                <!--[if $thread.LastReplyUserID!=0]-->
                                                     <a href="$url(space/$thread.LastReplyUserID)" class="fn username" target="_blank">$thread.LastReplyUsername</a>
                                                <!--[else]-->
                                                    <!--[if $thread.LastReplyUsername!=""]-->
                                                    游客:<span class="fn">$thread.LastReplyUsername</span>
                                                    <!--[else]-->
                                                    匿名游客
                                                    <!--[/if]-->
                                                <!--[/if]-->
                                                    <a class="date" href="$GetThreadUrl($thread,true)#last">$outputFriendlyDateTime($thread.UpdateDate)</a>
                                                </td>
                                                <td class="stats">
                                                    <span class="stats-reply">$thread.TotalReplies</span> / <span class="stats-view">$thread.TotalViews</span>
                                                </td>
                                            </tr>
                                                <!--[/loop]-->
                                        </tbody>
                                            <!--[else]-->
                                        <tbody class="forumtopics-list">
                                            <tr>
                                                <td colspan="5">
                                                    <div class="forumtopics-nodata">
                                                        暂时没有主题.
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                            <!--[/if]-->
                                        <!--[/if]-->
                                    </table>
                                </form>
                                </div>
                                <div class="panel-foot"><div><div>-</div></div></div>
                            </div>
                            
                            <div class="clearfix postoperate">
                                <!--[if $IsNormalThreads || $IsShowModeratorManageLink]-->
                                <div class="topic-action">
                                    <div class="action-button">
                                        <!--[if $IsNormalThreads]-->
                                        <a id="action-newtopic2" class="action-newtopic" href="$url($codename/post)?action=thread" title="发新帖"><span><img src="$skin/images/theme/button_post.png" alt="" /></span></a>
                                        <!--[/if]-->
                                        <!--[if $IsShowModeratorManageLink]-->
                                        <a class="action-manage" id="action-manage1" href="#"><span><img src="$skin/images/theme/button_manage.png" alt="" /></span></a>
                                        <!--[/if]-->
                                    </div>
                                </div>
                                <!--[/if]-->
                                <div class="pagination">
                                    <div class="pagination-inner">
                                        <!--[ajaxpanel id="ap_threads_toppager2" idonly="true"]-->
                                        <!--[if !$IsNormalThreads]-->
                                        <a class="back" href="$url($forum.codename/list-1)" title="返回列表页">&laquo; 返回列表页</a>
                                        <!--[/if]-->
                                        <!--[pager name="ThreadListPager" skin="../_inc/_pager_bbs.aspx"]-->
                                        <!--[/ajaxpanel]-->
                                    </div>
                                </div>
                            </div>
                            <!--[/ajaxpanel]-->
                            
                            <!--[if $IsShowQuicklyPost]-->
                            <form id="quicklypost" method="post" enctype="multipart/form-data" action="$_form.action">
                            <div class="panel quickpost quickpost-publish">
                                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                                    <h3 class="panel-title">快速发布主题</h3>
                                </div></div></div>
                                <div class="panel-body">
                                    <div class="postwrap">
                                        <table class="posttable">
                                            <tr>
                                                <td class="postauthor">
                                                    <div class="authorinfo">
                                                        <div class="authorinfo-inner">
                                                            <div class="authorinfo-wrap">
                                                                <div class="useridentity">
                                                                    <!--[if $isLogin==false]-->
                                                                    <p class="user-name">
                                                                        <strong class="fn">游客</strong>
                                                                    </p>
                                                                    <!--[else]-->
                                                                    <p class="user-name">
                                                                        <a class="url" href="$url(space/$my.UserID)">
                                                                            <strong class="fn">$my.Username</strong>
                                                                        </a>
                                                                    </p>
                                                                    <!--[/if]-->
                                                                </div>
                                                                <div class="user-maindata">
                                                                    <div class="user-avatar">
                                                                        <!--[if $isLogin==false]-->
                                                                        <img class="photo" src="$root/max-assets/avatar/avatar_120.gif" alt="" width="120" height="120" />
                                                                        <!--[else]-->
                                                                        <img class="photo" src="$my.BigAvatarPath" alt="" width="120" height="120" />
                                                                        <!--[/if]-->
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </td>
                                                <td class="postmain">
                                                    <div class="postmain-inner">
                                                        <div class="quickpost-enter">
                                                            <div class="quickpost-enter-inner">
                                                                <!--[ajaxpanel id="ap_error" idonly="true"]-->
                                                                <!--[unnamederror]-->
                                                                <div class="errormsg">$Message</div>
                                                                <!--[/unnamederror]-->
                                                                <!--[/ajaxpanel]-->
                                                                <div class="formgroup quickpost-form">
                                                                    <!--[if $IsLogin == false && $EnableGuestNickName]-->
                                                                    <div class="formrow">
                                                                        <h3 class="label"><label for="guestNickName">昵称</label></h3>
                                                                        <div class="form-enter">
                                                                            <input name="guestNickName" id="guestNickName" type="text" class="text" value="$_form.text("guestNickName")" />
                                                                        </div>
                                                                    </div>
                                                                    <!--[/if]-->
                                                                    <div class="formrow">
                                                                        <div class="form-enter">
                                                                            $GetThreadCatalogList()
                                                                            <input name="subject" id="subject" type="text" class="text subject" value="$_form.text("subject")" />
                                                                        </div>
                                                                    </div>
                                                                    <div class="formrow">
                                                                        <div class="form-enter">
                                                                            <textarea name="editor" id="txtTempTextArea" style="width:100%; height:80px;">$_form.text("editor")</textarea>
                                                                            <script type="text/javascript" src="$root/max-assets/javascript/max-smalleditor.js"></script>
                                                                            <script type="text/javascript">
                                                                                createQuicklyEditor("editor");
                                                                                addPageEndEvent(function () { removeElement($("txtTempTextArea")); });
                                                                            </script>
                                                                        </div>
                                                                    </div>
                                                                    <!--[ajaxpanel id="ap_vcode" idonly="true"]-->
                                                                    <!--[ValidateCode actionType="CreateTopic"]-->
                                                                    <div class="formrow">
                                                                        <div class="form-enter">
                                                                            <input name="$inputName" id="$inputName" type="text" class="text validcode" value="输入验证码" onfocus="showVCode(this,'$imageurl');" autocomplete="off" $_if($disableIme,'style="ime-mode:disabled;"') />
                                                                            <span class="form-note">$tip</span>
                                                                        </div>
                                                                    </div>
                                                                    <!--[/ValidateCode]-->
                                                                    <!--[/ajaxpanel]-->
                                                                    <div class="formrow formrow-action">
                                                                        <div class="quickpost-submit">
                                                                            <span class="minbtn-wrap"><span class="btn"><input class="button" name="postButton" id="postButton" type="submit" onclick="return clickPostButton();" value="确认发布" /></span></span>
                                                                            <span class="quickpost-submit-tip">[直接按 Ctrl+Enter 可完成发布]</span>
                                                                        </div>
                                                                        <div class="quickpost-extraoption">
                                                                        <!--[if $IsShowHtmlAndMaxCode]-->
                                                                            <input type="radio" checked="checked" name="eritoSellect" value="0" id="eritoSellect1" onclick="editorSellect()" />
                                                                            <label for="eritoSellect1">使用UBB</label> 
                                                                            <input type="radio" name="eritoSellect" value="1" id="eritoSellect2" onclick="editorSellect()" />
                                                                            <label for="eritoSellect2">使用HTML</label>
                                                                            <script type="text/javascript">
                                                                                function editorSellect() {
                                                                                    if ($('eritoSellect1').checked)
                                                                                        $('editortool').style.display = '';
                                                                                    else
                                                                                        $('editortool').style.display = 'none';
                                                                                }
                                                                            </script>
                                                                        <!--[else if $AllowHtml]-->
                                                                            <span>允许使用HTML</span>
                                                                        <!--[else if $AllowMaxcode]-->
                                                                            <span>允许使用UBB</span>
                                                                        <!--[/if]-->
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                                <div class="panel-foot"><div><div>-</div></div></div>
                            </div>
                            </form>
                            <!--[/if]-->
                            
                            <!--[if $IsShowOnline]-->
                            <!--#include file="_forumonline_.aspx"-->
                            <!--[/if]-->
                            
                            <div class="forumsign">
                                <div class="forumsign-inner">
                                    <img src="$skin/images/icons/topic_normal.gif" alt="" /> 普通主题
                                    <img src="$skin/images/icons/topic_hot.gif" alt="" /> 热门主题
                                    <img src="$skin/images/icons/topic_lock.gif" alt="" /> 锁定主题
                                    <img src="$skin/images/icons/topic_pinned.gif" alt="" /> 总置顶主题
                                    <img src="$skin/images/icons/topic_sticky.gif" alt="" /> 置顶主题
                                </div>
                            </div>
                        <!--[/if]-->
                        </div>
                        <!--#include file="../_inc/_round_bottom.aspx"-->
                    </div>
                </div>
            </div>
        </div>
    </div>

<!--#include file="../_inc/_foot.aspx"-->
</div>

<!--[if $IsCatalogForum == false]-->
<!--[if $IsShowModeratorManageLink]-->
<div class="dropdownmenu-wrap moderatordropdown" id="moderatorMenu" style="display:none;">
    <div class="dropdownmenu">
        <div class="dropdownmenu-inner">
            <div class="clearfix moderator-head">
                <h3 class="moderator-title">
                    选中<strong class="count" id="selectCount">1</strong>篇
                    <span class="moderator-select">
                        <input name="selelctAll" type="checkbox" id="selAll" />
                        <label for="selAll">全选 </label>
                    </span>
                </h3>
                <p class="moderator-action"><a href="javascript:;" onclick="switchMenu(0);return false;">缩小</a></p>
            </div>
            <div class="clearfix moderator-body">
                <div class="moderator-operate">
                    <ul class="clearfix moderator-operate-list">
                        <!--[if $IsNormalThreads == false]-->
                        <li><a href="javascript:void(postthread2('deletethread.aspx'))">删除</a></li>
                        <!--[/if]-->
                        <!--[if $IsRecycleBin]-->
                        <li><a href="javascript:void(postthread2('revertthread.aspx'))">还原</a></li>
                        <!--[else if $IsUnapprovedThreads]-->
                        <li><a href="javascript:void(postthread2('approvethread.aspx'))">通过审核</a></li>
                        <!--[else if $IsUnapprovedPostsThreads]-->
                        <li><a href="javascript:void(postthread2('approvepostsbythreadid.aspx'))">主题的所有回复通过审核</a></li>
                        <li><a href="javascript:void(postthread2('deleteunapprovedpostbythreadid.aspx'))">删除主题的所有未审核回复</a></li>
                        <!--[/if]-->
                        <!--[if $IsNormalThreads]-->
                        $GetModeratorActionLinks(@"<li><a href=""javascript:void(postthread('{1}'))"">{0}</a></li>","")
                        <!--[/if]-->
                    </ul>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="dropdownmenu-wrap moderatordropdown moderatordropdown-mini" id="mMenuMini" style="display:none;" onclick="switchMenu(1)">
    <div class="dropdownmenu">
        <div class="dropdownmenu-inner">
            <strong class="count" id="selectCount_mini">0</strong>
        </div>
    </div>
</div>

<script type="text/javascript">
    function openPage(action) {
        var u = "$dialog/forum/" + action + ".aspx?codename=$Forum.CodeName";
        return openDialog(u,refresh);
    }

    var mMenuTop = 0;
    var mmode = 1;
    var switchMenu = function(s) {
        mMenuMini.style.display = s ? 'none' : '';
        moderatorMenu.style.display = s ? '' : 'none';
        mmode = s;
    }
    new popup('action-manage-list', ['action-manage', 'action-manage1'], true);
    var lChange = function (e) {
        var s0 = moderatorMenu.style;
        var s1 = mMenuMini.style;
        var c = threadList.selectCount();
       
        if (c == 0) {
            s0.display = "none";
            s1.display = "none";
            mMenuTop = 0;
        }
        else {
            switchMenu(mmode);
            var bRect = max.global.getBrowserRect();
            s0.left = (bRect.width / 2) + "px";
            s1.left = s0.left; //(bRect.right - 500) + "px";
            if (e) {
                var top = getTop(e.parentNode);
                if (top + moderatorMenu.offsetHeight > bRect.bottom) {
                    top = bRect.bottom - 10 - moderatorMenu.offsetHeight;
                }
                s0.top = top + "px";
                s1.top = top + "px";
                mMenuTop = top - bRect.top;
            }
        }
        var count = c.toString();
        $("selectCount_mini").innerHTML = count;
        $("selectCount").innerHTML = count;
    }
    var moderatorMenu = $("moderatorMenu");
    var mMenuMini = $("mMenuMini");
    function createModeratorMenu() {
        window.threadList = new checkboxList("threadIDs", "selAll");
        var selAll = $("selAll");
        addHandler(selAll, "click", function() { setTimeout(function() { lChange(null); }, 10); });
        threadList.SetItemChangeHandler(lChange);
    }
    var postthread = function(url) {
        postToDialog({ formId: "thread-list-form", url: url, callback: refresh });
    }
    var postthread2 = function(page) {
        var url = "$dialog/forum/" + page + "?codename=$Forum.CodeName";
        postthread(url);
    }
    createModeratorMenu();

    addHandler(window, "scroll", function () {
        if (mMenuTop) {
            var brect = max.global.getBrowserRect();

            mMenuMini.style.top = (brect.top + mMenuTop) +"px";
            moderatorMenu.style.top = (brect.top + mMenuTop) +"px" ;
        }
    });
</script>
<!--[/if]-->
<!--[/if]-->

<!--[if $IsCatalogForum == false]-->
<script type="text/javascript">
//<!--[if $IsNormalThreads]-->
new popup('action-newtopic-list',["action-newtopic2",'action-newtopic'],true);
new popup('topic-types-list','topic-types',true);
//<!--[/if]-->

//<!--[if $IsShowQuicklyPost]-->
function postCheck() {
    var form = $('quicklypost');
    if (form.subject.value == '') {
        showAlert('标题不能为空');
        return false;
    }
    if (form.editor.value == '') {
        showAlert('内容不能为空');
        return false;
    }
    return true;
}

var isSending = 0;

function clickPostButton(){
    if (postCheck() == false) {
        setButtonDisable("postButton", false);
        return false;
    }

    if (isSending) return;
    isSending = 1;

    var _pb = $('postButton');

    _pb.value = "正在发布";
    //<!--[if $QuicklyPostUseAjax]-->
    ajaxSubmit('quicklypost', 'postButton', 'ap_threads,ap_threads_toppager,ap_threads_toppager2,ap_error,ap_vcode', function(result) {
        if (result != null) {
            document.getElementsByName('editor')[0].value = '';
            var threadCatalogs = document.getElementsByName('threadCatalogs');
            if (threadCatalogs.length > 0) { threadCatalogs[0].selectedIndex = 0; }
            $('subject').value = '';
            //<!--[if $IsShowModeratorManageLink]-->
            createModeratorMenu();
            lChange(0);
            //<!--[/if]-->
            if (result.iswarning)
                showAlert(result.message);
        }
        _pb.disabled = false;
        _pb.value = "确认发布";
        isSending = 0;
    }, null, true);
    return false;
    //<!--[else]-->
    var form = $('quicklypost');
    form.submit();
    return true;
    //<!--[/if]-->
}
onCtrlEnter($("quicklypost"), function () { setButtonDisable("postButton", true); clickPostButton(); });
//<!--[/if]-->
</script>
<!--[/if]-->
</body>
</html>