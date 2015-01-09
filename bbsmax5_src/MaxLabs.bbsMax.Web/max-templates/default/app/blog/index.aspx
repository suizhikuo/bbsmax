<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/app.css" />
<!--[if $IsSpace]-->
<link rel="stylesheet" type="text/css" href="$skin/styles/space.css" />
<!--/* 用户空间自定义样式 */-->

<!--[/if]-->
</head>
<body>
<div class="container">
<!--#include file="../../_inc/_head.aspx"-->
    <div id="main" class="main <!--[if $IsSpace]--> section-space<!--[else]--> hasappsidebar section-app<!--[/if]-->">
        <!--[if $IsSpace]--><!--#include file="../../_inc/_round_top.aspx"--><!--[/if]-->
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">

                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/blog.gif);"><!--[if $IsSpace]-->$AppOwner.Username 的<!--[/if]-->日志</span></h3>
                                <!--[if $IsSpace == false]-->
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a $_if(SelectedEveryone, 'class="current" ' ) href="$url(app/blog/index)?view=everyone"><span>大家的日志</span></a></li>
                                        <li><a $_if(SelectedFriend, 'class="current" ' ) href="$url(app/blog/index)?view=friend"><span>好友的日志</span></a></li>
                                        <li><a $_if(SelectedMy, 'class="current" ' ) href="$url(app/blog/index)?view=my"><span>我的日志</span></a></li>
                                        <li><a $_if(SelectedCommented, 'class="current" ' ) href="$url(app/blog/index)?view=commented"><span>评论过的日志</span></a></li>
                                    </ul>
                                    <ul class="pagebutton">
                                        <li><a class="addblog" href="$url(app/blog/write)">发布新日志</a></li>
                                    </ul>
                                </div>
                                <!--[/if]-->
                            </div>
                            
                            <div class="clearfix workspace app-blog">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        <!--[if $TotalArticleCount > 0]-->
                                        <div class="hfeed bloglist">
                                            <!--[loop $Article in $ArticleList]-->
                                            <div class="clearfix blogitem" id="blogarticle-$article.id">
                                                <div class="blog-avatar avatar">
                                                    <a href="$url(space/$Article.User.id)"><img src="$Article.User.Avatarpath" alt="" width="48" height="48" /></a>
                                                </div>
                                                <div class="hentry blog-entry">
                                                    <div class="blog-entry-inner">
                                                        <div class="clearfix entry-head">
                                                            <h3 class="entry-title">
                                                                <a rel="bookmark" href="$url(app/blog/view)?id=$Article.ID">$Article.Subject</a>
                                                            </h3>
                                                            <!--[if $Article.DisplayForPasswordHolderOnly]-->
                                                            <p class="privacy">凭密码可见</p>
                                                            <!--[else if $Article.DisplayForFriendOnly]-->
                                                            <p class="privacy">仅日志主人的好友可见</p>
                                                            <!--[else if $Article.DisplayForOwnerOnly]-->
                                                            <p class="privacy">仅日志主人可见</p>
                                                            <!--[/if]-->
                                                            <!--[if $IsShowShareLink]-->
                                                            <div class="clearfix sharecount">
                                                                <!--[if $CanUseCollection]-->
                                                                <a class="sharecount-button" title="收藏" href="$dialog/share-create.aspx?type=collection&sharetype=Blog&targetID=$Article.ID" onclick="return openDialog(this.href);">
                                                                    <span class="inner">
                                                                        <span class="text">收藏</span>
                                                                    </span>
                                                                </a>
                                                                <!--[/if]-->
                                                                <!--[if $CanUseShare]-->
                                                                <a class="favcount-button" title="分享" href="$dialog/share-create.aspx?type=share&sharetype=Blog&targetID=$Article.ID" onclick="return openDialog(this.href);">
                                                                    <span class="inner">
                                                                        <span class="text">分享</span>
                                                                    </span>
                                                                </a>
                                                                <!--[/if]-->
                                                            </div>
                                                            <!--[/if]-->
                                                        </div>
                                                        <div class="entry-meta">
                                                            <span class="vcard author"><a class="url fn" href="$url(space/$Article.User.id)">$Article.User.Name</a></span>
                                                            发布于 <span class="published" title="$Article.CreateDate">$Article.FriendlyCreateDate</span>
                                                            <!--[if $Article.FriendlyCreateDate != $Article.FriendlyUpdateDate]-->
                                                            | 更新于 <span class="updated" title="$Article.UpdateDate">$Article.FriendlyUpdateDate</span>
                                                            <!--[/if]-->
                                                        </div>
                                                        <div class="clearfix entry-summary">
                                                            <!--[if $Article.CanDisplayThumb]-->
                                                            <img class="entry-thumb" src="$Article.Thumb" alt="$Article.Subject.ToHtml" onload="imageScale(this, 100, 100)" onerror="this.style.display='none';" />
                                                            <!--[/if]-->
                                                            <!--[if $CanSee($article)]-->
                                                             $Article.SummaryContent
                                                            <!--[else]-->
                                                                <!--[if $Article.DisplayForPasswordHolderOnly]-->
                                                                <p class="privacy">凭密码可见</p>
                                                                <!--[else if $Article.DisplayForFriendOnly]-->
                                                                <p class="privacy">仅日志主人的好友可见</p>
                                                                <!--[else if $Article.DisplayForOwnerOnly]-->
                                                                <p class="privacy">仅日志主人可见</p>
                                                                <!--[/if]-->
                                                            <!--[/if]-->
                                                        </div>
                                                        <div class="entry-stats">
                                                            <a href="$url(app/blog/view)?id=$Article.ID" target="_blank">阅读 $Article.TotalViews</a>
                                                            - <a href="$url(app/blog/view)?id=$Article.ID#comments" target="_blank">回复 $Article.TotalComments</a>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="entry-action">
                                                    <!--[if $Article.CanDelete]-->
                                                    <a class="action-delete" title="删除" href="$dialog/blog-blogarticle-delete.aspx?id=$Article.ID" onclick="return openDialog(this.href, this, function(result){ delElement($('blogarticle-' + result.id)); });">删除</a>
                                                    <!--[/if]-->
                                                    <!--[if $Article.CanEdit]-->
                                                    <a class="action-edit" title="编辑" href="$url(app/blog/write)?id=$Article.ID">编辑</a>
                                                    <!--[/if]-->
                                                    <a class="action-report" title="举报" href="$dialog/report-add.aspx?type=blog&id=$article.id&uid=$article.userid" onclick="return openDialog(this.href)">举报</a>
                                                </div>
                                            </div>
                                            <!--[/loop]-->
                                        </div>
                                        <!--[pager name="pager1" skin="../../_inc/_pager_app.aspx" Count="$TotalArticleCount" PageSize="$ArticleListPageSize" ]-->
                                        <!--[else]-->
                                        <div class="nodata">
                                            <!--[if $SelectedMy]-->当前没有发布任何日志.
                                            <!--[else if $SelectedFriend]-->你可能没有任何好友, 或者你的好友并未发布任何日志.
                                            <!--[else if $SelectedCommented]-->你没有评论过的任何日志.
                                            <!--[else if]-->当前没有任何日志.
                                            <!--[/if]-->
                                        </div>
                                        <!--[/if]-->
                                    </div>
                                </div>
                                <div class="workspace-sidebar">
                                    <div class="workspace-sidebar-inner">
                                    <!--[if $SelectedMy || $IsSpace]-->
                                        <div class="panel categorylist blogcategory">
                                            <div class="panel-head">
                                                <h3 class="panel-title"><span>日志分类</span></h3>
                                            </div>
                                            <div class="panel-body">
                                                <ul class="category-list actioncount-2">
                                                    <li class="clearfix $_if($categoryid < 0, 'current')">
                                                        <div class="name">
                                                            <a href="$url(app/blog/index)?uid=$appOwnerUserID">全部日志</a>
                                                        </div>
                                                    </li>
                                                    <li class="clearfix $_if($categoryid == 0, 'current')">
                                                        <div class="name">
                                                            <a href="$url(app/blog/index)?cid=0&uid=$appOwnerUserID">未分类</a>
                                                        </div>
                                                    </li>
                                                    <!--[loop $Category in $CategoryList]-->
                                                    <li class="clearfix $_if($category.categoryid == $categoryid, 'current')" id="blogcategory-$Category.ID">
                                                        <div class="name">
                                                            <a href="$url(app/blog/index)?cid=$Category.ID&uid=$appOwnerUserID" id="blogcategory-$category.id-name">$Category.Name</a>
                                                        </div>
                                                        <!--[if $CanManageCategory]-->
                                                        <div class="entry-action">
                                                            <a class="action-rename" title="重命名分类" id="c_edit_$Category.ID" onclick="return openDialog(this.href, function(result){ $('blogcategory-' + result.id + '-name').innerHTML = result.name; });" href="$dialog/blog-blogcategory-edit.aspx?id=$Category.ID">重命名</a>
                                                            <a class="action-delete" title="删除" id="c_delete_$Category.ID" onclick="return openDialog(this.href, function(result){ delElement($('blogcategory-' + result.id)); }); " href="$dialog/blog-blogcategory-delete.aspx?id=$Category.ID">删除</a>
                                                        </div>
                                                        <!--[/if]-->
                                                    </li>
                                                    <!--[/loop]-->
                                                </ul>
                                            </div>
                                        </div>
                                        <!--[if $TagList.Count > 0]-->
                                        <div class="panel blogtaglist">
                                            <div class="panel-head">
                                                <h3 class="panel-title"><span>按标签查看</span></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="blogtag-list">
                                                <!--[loop $tag in $TagList]-->
                                                    <a href="$url(app/blog/index)?tid=$tag.ID&uid=$appOwnerUserID">$tag.Name</a>
                                                <!--[/loop]-->
                                                </div>
                                            </div>
                                        </div>
                                        <!--[/if]-->
                                    <!--[else]-->
                                        <!--[if $ArticleAuthorList.Count > 0]-->
                                        <div class="panel categorylist blogauthorlist">
                                            <div class="panel-head">
                                                <h3 class="panel-title"><span>按作者查看</span></h3>
                                            </div>
                                            <div class="panel-body">
                                                <ul class="category-list blogauthor-list">
                                                <!--[loop $author in $ArticleAuthorList]-->
                                                    <li>
                                                        <a class="clearfix" href="$url(app/blog/index)?uid=$Author.ID" target="_blank">
                                                            <span class="avatar">
                                                                <img src="$Author.avatarpath" alt="" width="24" height="24" />
                                                            </span>
                                                            $Author.Username
                                                        </a>
                                                    </li>
                                                <!--[/loop]-->
                                                </ul>
                                            </div>  
                                        </div>
                                        <!--[else]-->
                                        &nbsp;
                                        <!--[/if]-->
                                    <!--[/if]-->
                                    </div>
                                </div>
                            </div>

                        </div>
                        <!--#include file="../../_inc/_round_bottom.aspx"-->
                    </div>
                </div>
            </div>
            <!--[if $IsSpace]-->
                <!--#include file="../../space/_spacesidebar.aspx"-->
            <!--[else]-->
                <!--#include file="../../_inc/_sidebar_app.aspx"-->
            <!--[/if]-->
        </div>
        <!--[if $IsSpace]--><!--#include file="../../_inc/_round_bottom.aspx"--><!--[/if]-->
    </div>
<!--#include file="../../_inc/_foot.aspx"-->
</div>
</body>
</html>
