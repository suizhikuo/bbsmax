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
<script type="text/javascript">var root = '$Root'; var cookieDomain = '$CookieDomain'</script>
<script type="text/javascript" src="$root/max-assets/javascript/max-lib.js"></script>
<script type="text/javascript" src="$root/max-assets/javascript/max-showflash.js"></script>
<script type="text/javascript" src="$root/max-assets/javascript/max-commentlist.js"></script>
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
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/share.gif);"><!--[if $IsSpace]-->$AppOwner.Username 的<!--[/if]-->$FunctionName</span></h3>
                                <!--[if $IsSpace == false]-->
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <!--[if $IsFav == false]-->
                                        <li><a $_if($selectedHot, 'class="current"') href="$url(app/share/index)?view=hot"><span>热门分享</span></a></li>
                                        <li><a $_if($selectedFriend, 'class="current"') href="$url(app/share/index)?view=friend"><span>好友的分享</span></a></li>
                                        <li><a $_if($selectedMy, 'class="current"') href="$url(app/share/index)?view=my"><span>我的分享</span></a></li>
                                        <li><a $_if($selectedEveryone, 'class="current"') href="$url(app/share/index)?view=everyone"><span>最新分享</span></a></li>
                                        <li><a $_if($selectedcommented, 'class="current"') href="$url(app/share/index)?view=commented"><span>我评论过的</span></a></li>
                                        <!--[else]-->
                                        <li><a class="current" href="$url(app/share/index)?mode=fav"><span>收藏</span></a></li>
                                        <!--[/if]-->
                                    </ul>
                                    <div class="addshare">
                                        <span class="share-tip">
                                            输入网页, 视频, 音乐的网址
                                        </span>
                                        <span class="share-url">
                                            <input type="text" class="text" name="Url" id="share_link" value="$_form.text('url','http://')" />
                                        </span>
                                        <span class="minbtn-wrap"><span class="btn"><input type="submit" id="createshare" name="Publish" value="$functionName" class="button" onclick="openDialog('$root/max-dialogs/share-create.aspx?isfav=$isfav&sharetype=url&url=' + $('share_link').value, function(r){if(r=='close') setButtonDisable('createshare',false);else refresh();}); return false;" /></span></span>
                                    </div>
                                </div>
                                <!--[/if]-->
                            </div>
                            
                            <div class="clearfix workspace app-share">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        
                                        <div class="clearfix sharefilter">
                                            <div class="filtertab sharefilter-category">
                                                <ul class="clearfix tab-list">
                                                    <li><a href="$AttachQueryString("type=")" class="$IsSelected(ShareType.All,"current")"><span>全部</span></a></li>
                                                    <li><a href="$AttachQueryString("type="+ShareType.Video)" class="$IsSelected(ShareType.Video,"current")"><span>视频</span></a></li>
                                                    <li><a href="$AttachQueryString("type="+ShareType.Music)" class="$IsSelected(ShareType.Music,"current")"><span>音乐</span></a></li>
                                                    <li><a href="$AttachQueryString("type="+ShareType.URL)" class="$IsSelected(ShareType.URL,"current")"><span>网站</span></a></li>
                                                    <li><a href="$AttachQueryString("type="+ShareType.Flash)" class="$IsSelected(ShareType.Flash,"current")"><span>Flash</span></a></li>
                                                    <li><a href="$AttachQueryString("type="+ShareType.Blog)" class="$IsSelected(ShareType.Blog,"current")"><span>日志</span></a></li>
                                                    <li><a href="$AttachQueryString("type="+ShareType.Album)" class="$IsSelected(ShareType.Album,"current")"><span>相册</span></a></li>
                                                    <li><a href="$AttachQueryString("type="+ShareType.Picture)" class="$IsSelected(ShareType.Picture,"current")"><span>图片</span></a></li>
                                                    <li><a href="$AttachQueryString("type="+ShareType.Topic)" class="$IsSelected(ShareType.Topic,"current")"><span>话题</span></a></li>
                                                    
                                                    <%--<li><a href="$AttachQueryString("type="+ShareType.Forum)" class="$IsSelected(ShareType.Forum,"current")"><span>群组</span></a></li>--%>
                                                    <%--<li><a href="$AttachQueryString("type="+ShareType.User)" class="$IsSelected(ShareType.User,"current")"><span>用户</span></a></li> --%>
                                                </ul>
                                            </div>
                                            <!--[if $SelectedHot]-->
                                            <div class="filtertab sharefilter-date">
                                                <ul class="clearfix tab-list">
                                                    <li><a $_if($Scope == "0", 'class="current"') href="$AttachQueryString('scope=0')"><span>最近</span></a></li>
                                                    <li><a $_if($Scope == "1", 'class="current"') href="$AttachQueryString('scope=1')"><span>24小时</span></a></li>
                                                    <li><a $_if($Scope == "2", 'class="current"') href="$AttachQueryString('scope=2')"><span>1周</span></a></li>
                                                </ul>
                                            </div>
                                            <!--[/if]-->
                                        </div>
                                        
                                    <!--[if $ShareTotalCount > 0]-->
                                        <div class="hfeed sharelist">
                                            <!--[loop $share in $sharelist]-->
                                            <div class="clearfix shareitem" id="sharebox_$share.usershareid">
                                                <div class="sharelovehate">
                                                    <!--[ajaxpanel id="agree_{=$share.usershareid}"]-->
                                                    <form action="$_form.action" method="post" id="form_$share.usershareid">
                                                    <input type="hidden" name="shareid" value="$share.shareid" />
                                                    <a class="sharelovehate-love $_if($AgreeStates[$share.shareid] != null && (bool)$AgreeStates[$share.shareid] == true, 'sharelovehate-love-checked', '')" href="#" title="顶" onclick="ajaxSubmit('form_$share.usershareid', 'agree', 'agree_$share.usershareid', null, null, true); return false;"><span>$share.AgreeCount</span></a>
                                                    <a class="sharelovehate-hate $_if($AgreeStates[$share.shareid] != null && (bool)$AgreeStates[$share.shareid] == false, 'sharelovehate-hate-checked', '')" href="#" onclick="ajaxSubmit('form_$share.usershareid', 'oppose', 'agree_$share.usershareid', null, null, true); return false;" title="踩"><span>$share.OpposeCount</span></a>
                                                    </form>
                                                    <!--[/ajaxpanel]-->
                                                </div>
                                                <div class="share-content">
                                                    <div class="share-content-inner">
                                                        <div class="clearfix shareitem-layout">
                                                            <div class="hentry shareentry">
                                                                <div class="shareentry-inner">
                                                                    <h3 class="entry-title">
                                                                        <!--[if $isFav]-->
                                                                            <a href="$Share.Url" target="_blank" rel="bookmark">$share.subject</a>
                                                                        <!--[else]-->
                                                                            <a href="$url(app/share/view)?id=$share.userShareID" target="_blank" rel="bookmark">$share.subject</a>
                                                                        <!--[/if]-->
                                                                        <!--[if $Share.DisplayForFriendOnly]-->
                                                                        <span class="privacy">仅好友可见</span>
                                                                        <!--[/if]-->
                                                                    </h3>
                                                                    <div class="entry-url"> 
                                                                        <!--[if $isFav]-->
                                                                            <a href="$Share.Url" target="_blank" rel="bookmark">$share.subject</a>
                                                                        <!--[else]-->
                                                                            <a href="$url(app/share/view)?id=$share.userShareID" target="_blank">$Share.Url</a>
                                                                        <!--[/if]-->
                                                                    </div>
                                                                    <div class="entry-meta">
                                                                        <span class="vcard author">
                                                                            <img class="photo" src="$Share.User2.SmallAvatarpath" alt="" width="16" height="16" />
                                                                            $share.User2.PopupNameLink
                                                                        </span>
                                                                        <span class="published">$outputFriendlyDateTime($Share.CreateDate)</span>
                                                                    </div>
                                                                    <div class="entry-content">
                                                                        <!--[if $Share.Type == ShareType.Video]-->
                                                                        <div class="videoplayer">
                                                                            <!--[if $share.video.imgurl == ""]-->
                                                                            <img src="$Root/max-assets/images/default_media.gif" alt="" title="点击播放" width="120" height="90" />
                                                                            <!--[else]-->
                                                                            <img src="$share.video.imgurl" alt="" title="点击播放" width="120" height="90" />
                                                                            <!--[/if]-->
                                                                            <a class="video-play" href="javascript:void(0)" title="播放该视频" onclick="javascript:showFlash('$Root', '$Share.Video.Domain', '$Share.Video.VideoID', this.parentNode, '$share.usershareid');" >播放该视频</a>
                                                                        </div>
                                                                        <!--[else if $Share.Type == ShareType.Music]-->
                                                                        <div class="audioplayer">
                                                                            <object id="audioplayer_$share.usershareid" height="24" width="290" data="/max-assets/flash/player.swf" type="application/x-shockwave-flash">
                                                                                <param value="/max-assets/flash/player.swf" name="movie" />
                                                                                <param value="autostart=no&bg=0xEBF3F8&leftbg=0x6B9FCE&lefticon=0xFFFFFF&rightbg=0x6B9FCE&rightbghover=0x357DCE&righticon=0xFFFFFF&righticonhover=0xFFFFFF&text=0x357DCE&slider=0x357DCE&track=0xFFFFFF&border=0xFFFFFF&loader=0xAF2910&soundFile=$Share.Content" name="FlashVars" />
                                                                                <param value="high" name="quality" />
                                                                                <param value="false" name="menu" />
                                                                                <param value="#ffffff" name="bgcolor" />
                                                                            </object>
                                                                        </div>
                                                                        <!--[else if $Share.Type == ShareType.URL]-->
                                                                        <div class="contentwrapper">
                                                                            <a href="$Share.Content" target="_blank">$Share.Content</a>
                                                                        </div>
                                                                        <!--[else]-->
                                                                        <div class="contentwrapper">
                                                                        $Share.Content
                                                                        </div>
                                                                        <!--[/if]-->
                                                                        <!--[if $Share.description != null && $Share.description.length > 0]-->
                                                                        <div class="description">
                                                                            <div class="description-inner">
                                                                                $Share.description
                                                                            </div>
                                                                        </div>
                                                                        <!--[/if]-->
                                                                        <!--[if $isFav == false]-->
                                                                        <div class="operate">
                                                                             <a href="javascript:void(0)" onclick="expandReply($share.usershareid)"><span id="comment_button_$share.usershareid">收起评论</span></a>
                                                                        </div>
                                                                        <!--[/if]-->
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="shareotherinfo">
                                                                <div class="shareotherinfo-inner">
                                                                    <div class="clearfix sharecount">
                                                                    <!--[if $CanUseShare]-->
                                                                        <a class="sharecount-button" href="$root/max-dialogs/share-create.aspx?refshareid=$share.shareid" onclick="openDialog(this.href); return false;">
                                                                            <span class="inner">
                                                                                <span class="text">我也分享 <span class="stat">$Share.ShareCount</span></span>
                                                                            </span>
                                                                        </a>
                                                                    <!--[/if]-->
                                                                    <!--[if $CanUseCollection]-->
                                                                        <a class="favcount-button" href="$root/max-dialogs/share-create.aspx?type=collection&refshareid=$share.shareid" onclick="openDialog(this.href); return false;">
                                                                            <span class="inner">
                                                                                <span class="text">收藏</span>
                                                                            </span>
                                                                        </a>
                                                                    <!--[/if]-->
                                                                    </div>
                                                                    <div class="sharefirst">
                                                                        <p class="published sharefirst-date">$Share.CreateDate</p>
                                                                        <p class="sharefirst-user">
                                                                            首次分享人
                                                                            <span class="vcard">
                                                                                <img class="photo" src="$Share.User.SmallAvatarpath" alt="" width="16" height="16" />
                                                                                $share.User.PopupNameLink
                                                                            </span>
                                                                        </p>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        
                                                        <!--[if $isFav == false]-->
                                                        <div id="comment_div_$share.usershareid" class="entry-comment statuscomment">
                                                        <!-- #include file="../../_inc/_commentlist.aspx" targetid="$share.usershareid" targetCommentCount="$share.commentlist.count" commentList="$GetComments($share)" IsShowGetAll="$IsShowGetAll($share)" TotalComments="$share.TotalComments" commentType="share" IsShowCommentPager="$IsShowCommentPager($share)" -->
                                                        </div>
                                                        <!--[/if]-->
                                                    </div>
                                                </div>
                                                
                                                <div class="entry-action">
                                                    <!--[if $share.candelete]-->
                                                    <a class="action-delete" title="删除" href="$dialog/share-delete.aspx?shareid=$share.shareid$_if($Share.PrivacyType == PrivacyType.SelfVisible, '&isfav=1','')" id="share_delete_$share.usershareid" onclick="openDialog(this.href,this, function(result){delElement($('sharebox_$share.usershareid'));}); return false;">删除</a>
                                                    <!--[/if]-->
                                                    
                                                    <!--[if $isFav == false]-->
                                                    <a class="action-report" title="举报" href="$dialog/report-add.aspx?type=share&id=$share.id&uid=$share.userid" onclick="openDialog(this.href); return false;">举报</a>
                                                    <!--[/if]-->
                                                    
                                                </div>
                                            </div>
                                            
                                            <!--[/loop]-->
                                            
                                        </div>
                                        <!--[pager name="pager1" skin="../../_inc/_pager_app.aspx"]-->
                                    <!--[else]-->
                                        <div class="nodata">
                                            <!--[if $selectedFriend]-->你可能没有任何好友, 或者你的好友未添加任何{=$FunctionName}.<!--[else]-->当前没有任何{=$FunctionName}.<!--[/if]-->
                                        </div>
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
