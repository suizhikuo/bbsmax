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
                                        <li><a $_if(SelectedMy, 'class="current" ' ) href="$url(app/blog/index)"><span>我的日志</span></a></li>
                                        <li><a $_if(SelectedCommented, 'class="current" ' ) href="$url(app/blog/index)?view=visited"><span>评论过的日志</span></a></li>
                                    </ul>
                                    <ul class="pagebutton">
                                        <li><a class="addblog" href="$url(app/blog/write)">发布新日志</a></li>
                                    </ul>
                                </div>
                                <!--[/if]-->
                            </div>
                            
                            <div class="clearfix workspace app-blog">
                                <div class="workspace-content">
                                    <div class="workspace-content-inner">
                                        
                                        
                                        <!--[if $IsShowPasswordBox]-->
                                        <form id="passwordform" action="$_form.action" method="post">
                                        <div class="formgroup passwordform">
                                            <div class="passwordform-tip">日志主人为日志设置了密码.</div>
                                            <div class="formrow">
                                                <label class="label" for="password">输入密码</label>
                                                <div class="form-enter">
                                                    <input class="text" type="password" id="password" name="password" />
                                                </div>
                                                <!--[error name="password" form="passwordform"]-->
                                                <div class="form-tip tip-error">
                                                    密码错误
                                                </div>
                                                <!--[/error]-->
                                            </div>
                                            <div class="formrow formrow-action">
                                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="submitPassword" value="确认" class="button" /></span></span>
                                            </div>
                                        </div>
                                        </form>
                                        <!--[else]-->
                                        <div class="hfeed blogarticle">
                                            <div class="clearfix articleitem">
                                                <div class="blog-avatar avatar">
                                                    <a href="$url(space/$article.UserID)"><img src="$Article.User.Avatarpath" alt="" width="48" height="48" /></a>
                                                </div>
                                                <div class="blog-entry">
                                                    <div class="blog-entry-inner">
                                                        <div class="clearfix entry-head">
                                                            <h3 class="entry-title">
                                                                $Article.Subject  
                                                            </h3>
                                                                <!--[if $Article.PrivacyType == PrivacyType.SelfVisible]-->
                                                                    <p class="privacy">(当前内容仅日志主人可见<!--[if $IsShowAdminCanSeeNot]-->,您有管理权限可以查看<!--[/if]-->)</p>
                                                                <!--[else if $Article.PrivacyType == PrivacyType.FriendVisible]-->
                                                                    <p class="privacy">(当前内容仅日志主人好友可见<!--[if $IsShowAdminCanSeeNot]-->,您有管理权限可以查看<!--[/if]-->)</p>
                                                                <!--[else if $Article.PrivacyType == PrivacyType.NeedPassword]-->
                                                                    <p class="privacy">(当前内容需要密码可见<!--[if $IsShowAdminCanSeeNot]-->,您有管理权限可以查看<!--[/if]-->)</p>
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
                                                            <span class="vcard author"><a class="url fn" href="$url(space/$article.UserID)">$Article.User.Name</a></span>
                                                            发布于 <span class="published" title="#">$Article.FriendlyCreateDate</span>
                                                            <!--[if $Article.FriendlyCreateDate != $Article.FriendlyUpdateDate]-->
                                                            | 更新于 <span class="updated" title="#">$Article.FriendlyUpdateDate</span>
                                                            <!--[/if]-->
                                                        </div>
                                                        <!--[if $Article.Tags.count > 0]-->
                                                        <div class="entry-tag">
                                                            标签: 
                                                            <!--[loop $tag in $Article.Tags]-->
                                                            <a href="$url(app/blog/index)?tid=$tag.id&uid=$Article.UserID" title="$tag.id">$tag.name</a>
                                                            <!--[/loop]-->
                                                        </div>
                                                        <!--[/if]-->
                                                        <div class="clearfix entry-content">
                                                            $Article.Content
                                                        </div>
                                                        <!--[ajaxpanel id="ap_commentCount" idonly="true"]-->
                                                        <div class="entry-stats">
                                                            阅读 $Article.TotalViews
                                                            - 回复 $Article.TotalComments
                                                        </div>
                                                        <!--[/ajaxpanel]-->
                                                        <div class="entry-comment articlecomment" id="comments">
                                                        <!-- #include file="../../_inc/_commentlist2.aspx"  commentList="$commentList"  commentType="blog" -->
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="entry-action">
                                                    <!--[if $Article.CanDelete]-->
                                                    <a class="action-delete" title="删除" href="$dialog/blog-blogarticle-delete.aspx?id=$Article.ID" onclick="return openDialog(this.href, this, function(result){ location.href='$url(app/blog/index)'; });">删除</a>
                                                    <!--[/if]-->
                                                    <!--[if $Article.CanEdit]-->
                                                    <a class="action-edit" title="编辑" href="$url(app/blog/write)?id=$Article.ID">编辑</a>
                                                    <!--[/if]-->
                                                    <a class="action-report" href="$dialog/report-add.aspx?type=blog&id=$Article.ID&uid=$appOwnerUserID" onclick="return openDialog(this.href);" title="举报" href="#">举报</a>
                                                </div>
                                            </div>
                                        </div>
                                        <!--[/if]-->
                                    </div>
                                </div>
                                <div class="workspace-sidebar">
                                    <div class="workspace-sidebar-inner">
                                        <div class="panel recentarticle">
                                            <div class="panel-head">
                                                <h3 class="panel-title"><span>相似的日志</span></h3>
                                            </div>
                                            <div class="panel-body">
                                                <ul>
                                                    <!--[if $articlelist.count > 0]-->
                                                    <!--[loop $article in $articlelist]-->
                                                    <li><a href="$url(app/blog/view)?id=$article.id">$article.Subject</a></li>
                                                    <!--[/loop]-->
                                                    <!--[else]-->
                                                    <li>暂时没有相似的日志</li>
                                                    <!--[/if]-->
                                                </ul>
                                            </div>
                                        </div>
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
