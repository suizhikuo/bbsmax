<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_forumhtmlhead_.aspx"-->
</head>
<body>
<div class="container section-newtopic">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../_inc/_round_top.aspx"-->
                        <div class="clearfix content-main-inner">
                            <div class="clearfix postoperate">
                                <div class="pagination">
                                    <div class="pagination-inner">
                                        <!--[pager name="list" skin="../_inc/_pager_bbs.aspx"]-->
                                    </div>
                                </div>
                            </div>
                            
                            <div class="panel forumtopics">
                                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                                    <h3 class="panel-title"><span>最新主题</span></h3>
                                </div></div></div>
                                <div class="panel-body">
                                    <table>
                                        <tbody class="forumtopics-head">
                                            <tr>
                                                <td class="icon">&nbsp;</td>
                                                <td class="author">作者</td>
                                                <td class="title">主题</td>
                                                <td class="cate">版块</td>
                                                <td class="last">最后回复</td>
                                                <td class="stats">回复/查看</td>
                                            </tr>
                                        </tbody>
                                        <tbody class="forumtopics-list">
                                            <!--[if $threadList.Count > 0]-->
                                            <!--[loop $thread in $threadList]-->
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
                                                    <span class="date">$outputFriendlyDateTime($thread.CreateDate)</span>
                                                </td>
                                                <td class="title">
                                                    $GetIcon($thread.IconID)
                                                    $GetThreadCatalogName($thread.ThreadCatalogID, @"<span class=""cate"">[{0}]</span>")
                                                    $GetThreadLink($thread, 60, @"<a href=""{0}"" target=""_blank"">{1}</a>")
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
                                                    <!--[if $thread.Price > 0]-->
                                                    <img src="$root/max-assets/icon/coin_gold.gif" alt="" />
                                                    价格:$GetSellThreadPoint($thread.PostUserID).Name $thread.Price $GetSellThreadPoint($thread.PostUserID).UnitName
                                                    <!--[/if]-->
                                                    <!--[if $thread.Rank > 0]-->
                                                    <a href="$Dialog/rankusers.aspx?threadid=$thread.threadid" onclick="return openDialog(this.href, null)"" title="当前{=$thread.Rank}分"><img src="$skin/images/icons/postrank_{=$thread.Rank}.gif" alt="" /></a>
                                                    <!--[/if]-->
                                                    $GetThreadPager($thread,@"<span class=""topic-pages"">{0}</span>", @"<a href=""{0}"" target=""_blank"">{1}</a>")
                                                </td>
                                                <td class="cate">
                                                    <a href="$url($thread.Forum.CodeName/list-1)" target="_blank">$thread.Forum.ForumName</a>
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
                                                    <span class="stats-reply">$thread.TotalReplies</span>/<span class="stats-view">$thread.TotalViews</span>
                                                </td>
                                            </tr>
                                            <!--[/loop]-->
                                            <!--[else]-->
                                            <tr>
                                                <td colspan="6">
                                                    <div class="forumtopics-nodata">
                                                        暂时没有主题.
                                                    </div>
                                                </td>
                                            </tr>
                                            <!--[/if]-->
                                        </tbody>
                                    </table>
                                </div>
                                <div class="panel-foot"><div><div>-</div></div></div>
                            </div>
                            
                            <div class="clearfix postoperate">
                                <div class="pagination">
                                    <div class="pagination-inner">
                                        <!--[pager name="list" skin="../_inc/_pager_bbs.aspx"]-->
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!--#include file="../_inc/_round_bottom.aspx"-->
                    </div>
                </div>
            </div>
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>
</body>
</html>
