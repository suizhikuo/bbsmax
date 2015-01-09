<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_spacehtmlhead.aspx"-->
<script type="text/javascript">
function textCounter(target, display, maxcount) {
    var a = maxcount - target.value.length;
    if (a < 0) {
        target.value = target.value.substr(0, maxcount);
        a = 0;
    }
    document.getElementById(display).innerHTML = a;
}
</script>
</head>
<body>
<div class="container section-space section-space-home">
<!--#include file="_spacethemetip.aspx"-->
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main">
        <!--#include file="../_inc/_round_top.aspx"-->
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <div class="content-main-inner">

                            <div class="spacecaption">
                                <div class="clearfix caption-head">
                                    <div class="ownername">
                                        {=$SpaceOwner.Username}
                                        <!--[if $IsShowRealName]-->
                                        <span>(姓名: {=$SpaceOwner.Realname})</span>
                                        <!--[/if]-->
                                        <!--[if $SpaceOwner.IsOnline]-->
                                        <span class="online">在线</span>
                                        <!--[/if]-->
                                    </div>
                                    <%--
                                    <!--空间可以访问-->
                                    <div class="ownerfeed">
                                        <a class="rss" href="#">订阅</a>
                                    </div>
                                    <!--/if-->
                                    --%>
                                </div>
                                
                                <!--[if $IsShowDoingInput]--><!--/*看自己空间的时候显示*/-->
                                <!--[ajaxpanel id="sp_doing" idonly="true"]-->
                                <div class="">
                                <div class="clearfix ownerstatus-wrap" id="mystate">
                                    <div class="ownerstatus">
                                        <div class="ownerstatus-inner" id="CenterCurrentDoing">
                                            <!--[if $SpaceOwner.Doing != ""]-->
                                            <span id="CenterCurrentDoingContent" onclick="showDoingInput();">$SpaceOwner.Doing</span>
                                            <!--[else]-->
                                            <span id="CenterCurrentDoingContent" onclick="showDoingInput();">说点什么吧.</span>
                                            <!--[/if]-->
                                        </div>
                                    </div>
                                </div>
                                <div class="clearfix ownerstatus-wrap ownerstatus-form" id="CenterDoingInput" style="display:none;">
                                    <div class="ownerstatus">
                                        <div class="ownerstatus-inner">
                                            <form id="doing_form" action="$url(handler/doing)" target="CenterDoingForm" method="post" onsubmit="return submitDoing()">
                                                <input type="text" name="content" id="CenterDoingContent" value="" class="text" onblur="if(this.value=='')hideCenterDoingInput();" />
                                                <a href="#" onclick="javascript:if(submitDoing()){$('doing_form').submit();}">更新</a>
                                                <a href="#" onclick="javascript:hideCenterDoingInput()">取消</a>
                                            </form>
                                            <iframe style="display:none;" name="CenterDoingForm" src="about:blank"></iframe>
                                        </div>
                                    </div>
                                </div>
                                </div>
                                <!--[/ajaxpanel]-->
                                <script type="text/javascript">
                                    function SubmitBack(issuccess, content) {
                                        if (issuccess == false)
                                            showAlert(content);
                                        else {
                                            $('CenterCurrentDoingContent').innerHTML = content;
                                            hideCenterDoingInput();
                                        }
                                    }
                                    function showDoingInput(){
                                        $('mystate').style.display = 'none';
                                        $('CenterDoingInput').style.display = '';
                                        $('CenterDoingContent').value = '';
                                        $('CenterDoingContent').focus();
                                    }
                                    function submitDoing(){
                                        //hideCenterDoingInput();
                                        var newContent =  HTMLEncode($('CenterDoingContent').value);
                                        if($('CenterCurrentDoingContent').innerHTML == newContent){
                                            return false;
                                        } else {
                                           // $('CenterCurrentDoingContent').innerHTML = newContent;
                                        }
                                        
                                        return true;
                                    }
                                    function hideCenterDoingInput(){						
                                        $('mystate').style.display = '';
                                        $('CenterDoingInput').style.display = 'none';
                                    }
                                </script>
                                <!--[else if $IsShowDoing && $DoingCanDisplay]--><!--/*看别人空间的时候显示*/-->
                                <div class="clearfix ownerstatus-wrap">
                                    <div class="ownerstatus">
                                        <div class="ownerstatus-inner">
                                            $SpaceOwner.Doing
                                        </div>
                                    </div>
                                </div>
                                <!--[/if]-->
                                <div class="ownerstat">
                                    空间共有 <span class="numeric">$SpaceOwner.TotalViews</span> 人次访问,
                                    <span class="numeric">$SpaceOwner.Points</span> 个$GeneralPointName 
                                </div>
                            </div>
                            
                            <!--[if $SpaceCanAccess]-->
                            <!--[if $UserInfoCanDisplay]-->
                            <div class="spaceownerinfo">
                                <ul class="clearfix spaceownerinfo-list">
                                    <li><span class="label">性别</span> <span class="value">$SpaceOwner.GenderName</span></li>
                                    <!--[if $SpaceOwner.Birthday.Year > 1900]-->
                                    <li><span class="label">生日</span> <span class="value">$OutputDate($SpaceOwner.Birthday)</span></li>
                                    <li><span class="label">星座</span> <span class="value">$SpaceOwner.AtomName</span></li>
                                    <!--[/if]-->
                                    <li><span class="label">最后登录时间</span> <span class="value">$OutputFriendlyDate($SpaceOwner.LastVisitDate)</span></li>
                                    <li><span class="label">用户头衔</span> <span class="value">$SpaceOwner.RoleTitle</span></li>
                                    <li><span class="label">注册时间</span> <span class="value">$OutputFriendlyDate($SpaceOwner.CreateDate)</span></li>
                                    <li><span class="label">$GeneralPointName</span> <span class="value">$SpaceOwner.Points</span></li>
                                    <!--[loop $point in $PointList]-->
                                    <li><span class="label">$point.name</span> <span class="value">$point.value </span></li>
                                    <!--[/loop]-->
                                    <li><span class="label">论坛主题数</span> <span class="value">$SpaceOwner.TotalTopics</span></li>
                                    <li><span class="label">论坛发帖数</span> <span class="value">$SpaceOwner.TotalPosts</span></li>
                                    <li><span class="label">主页访问数</span> <span class="value">$SpaceOwner.TotalViews</span></li>
                                    <!--[UserExtendedFieldList userID="$SpaceOwner.userID"]-->
                                    <!--[if $IsShow($privacyType,$SpaceOwner)]-->
                                    <li><span class="label">$Name</span> <span class="value">$fieldType.GetHtmlForDisplay($userValue)</span></li>
                                    <!--[/if]-->
                                    <!--[/UserExtendedFieldList]-->
                                </ul>
                                <%--
                                <p class="spaceownerinfo-viewall"><a href="#">查看详细资料</a></p>
                                --%>
                            </div>
                            <!--[/if]-->
                            <!--[/if]-->
                            
                            <!--[if $SpaceCanAccess == false]-->
                                <!--[if $IsSpaceOwnerFullSiteBanned]-->
                            <div class="spacewarning">
                                此空间的主人已被管理员全站屏蔽, 所以您暂时无法查看.
                            </div>
                                <!--[else]-->
                            <div class="spacewarning">
                                <div class="spacewarning-content">
                                    抱歉, 您无法查看此空间的内容.
                                    此空间的主人只对特定的好友开放访问, 您暂时无法查看.
                                </div>
                                <!--[if $VisitorIsFriend == false]-->
                                <div class="spaceaddfriend">
                                    <a href="$dialog/friend-tryadd.aspx?uid=$SpaceOwnerID" onclick="return openDialog(this.href, function(result){})">
                                        加为好友
                                    </a>
                                </div>
                                <!--[/if]-->
                            </div>
                                <!--[/if]-->
                            <!--[else]-->
                                <!--[if $SpaceDisplayAdminNote]-->
                            <div class="spacewarning">
                                <!--[if $IsSpaceOwnerFullSiteBanned]-->
                                您当前访问的用户已被全站屏蔽，因为您是管理员所以可以查看此空间。
                                <!--[else]-->
                                您当前访问的用户空间只对空间主人所指定的用户群体开放，因为您是管理员所以可以查看此空间。
                                <!--[/if]-->
                            </div>
                                <!--[/if]-->
                            <!--[/if]-->
                            
                        <!--[if $SpaceCanAccess]-->
                            <!--[if $IsShowAlbums]-->
                            <div class="panel spacealbum">
                                <div class="panel-head">
                                    <h3 class="panel-title"><span>相册</span></h3>
                                    <!--[if $AlbumList != null && $AlbumList.Count > 0 && $AlbumCanDisplay]-->
                                    <div class="panel-more"><a href="$url(app/album/index)?uid=$appownerUserID">全部</a></div>
                                    <!--[/if]-->
                                </div>
                                <div class="panel-body">
                                    <!--[if $AlbumCanDisplay == false]-->
                                    <div class="nodata">
                                        由于空间主人对相册设置了隐私您无法查看他/她的相册.
                                    </div>
                                    <!--[else if $AlbumList != null && $AlbumList.Count > 0]-->
                                    <div class="spacealbumlist">
                                        <ul class="clearfix spacealbum-list">
                                            <!--[loop $Album in $AlbumList with $i]-->
                                            <!--[if $Album.CanVisit]-->
                                            <li class="albumitem"> 
                                                <div class="clearfix albumitem-inner">
                                                    <div class="cover">
                                                        <a href="$url(app/album/list)?uid=$SpaceOwnerID&id=$Album.ID">
                                                            <!--[if $album.DisplayForPasswordHolderOnly]-->
                                                            <img src="$skin/../a.gif" alt="" />
                                                            <!--[else]-->
                                                            <img src="$Album.CoverSrc" alt="" />
                                                            <!--[/if]-->
                                                            &nbsp;
                                                        </a>
                                                    </div>
                                                    <div class="title">
                                                        <a class="name" href="$url(app/album/list)?uid=$SpaceOwnerID&id=$Album.ID">$Album.Name</a>
                                                        <!--[if $album.DisplayForPasswordHolderOnly]-->
                                                            <span class="privacy">凭密码可见</span>
                                                        <!--[else if $album.DisplayForOwnerOnly]-->
                                                            <span class="privacy">自己可见</span>
                                                        <!--[else if $album.DisplayForFriendOnly]-->
                                                            <span class="privacy">好友可见</span>
                                                        <!--[/if]-->
                                                    </div>
                                                    <div class="description">
                                                        $CutString($Album.Description,30)
                                                    </div>
                                                    <div class="counts"><!--[if $Album.TotalPhotos > 0]-->$Album.TotalPhotos张照片<!--[else]-->暂无照片<!--[/if]--></div>
                                                    <div class="update">更新于 $outputFriendlyDateTime($Album.UpdateDate)</div>
                                                </div>
                                            </li>
                                            <!--[/if]-->
                                            <!--[/loop]-->
                                        </ul>
                                    </div>
                                    <!--[else]-->
                                    <div class="nodata">
                                        <!--[if $VisitorIsOwner]-->
                                        你还没有相册. <a href="$url(app/album/upload)">点击这里添加相册</a>
                                        <!--[else]-->
                                        当前没有相册.
                                        <!--[/if]-->
                                    </div>
                                    <!--[/if]-->
                                </div>
                            </div>
                            <!--[/if]-->
                            
                            <!--[if $IsShowBlog]-->
                            <div class="panel spaceblog">
                                <div class="panel-head">
                                    <h3 class="panel-title"><span>日志</span></h3>
                                    <!--[if $BlogCanDisplay != false && $ArticleList.Count > 0]-->
                                    <div class="panel-more"><a href="$url(app/blog/index)?uid=$SpaceOwnerID">全部</a></div>
                                    <!--[/if]-->
                                </div>
                                <div class="panel-body">
                                    <!--[if $BlogCanDisplay == false]-->
                                    <div class="nodata">
                                        由于空间主人对日志设置了隐私您无法查看他/她的日志.
                                    </div>
                                    <!--[else if $ArticleList.Count > 0]-->
                                    <div class="spacebloglist">
                                        <ul class="spaceblog-list">
                                            <!--[loop $Article in $ArticleList]-->
                                            <li class="spaceblog-item">
                                                <div class="title">
                                                    <a href="$url(app/blog/view)?id=$Article.ID">$Article.Subject</a>
                                                    <span class="date">$Article.FriendlyCreateDate</span>
                                                </div>
                                                <!--[if $Article.CanVisit]-->
                                                <div class="summary">
                                                    $Article.SummaryContent
                                                    <!--[if $Article.DisplayForPasswordHolderOnly]-->
                                                        <span class="privacy">凭密码可见</span>
                                                    <!--[else if $Article.DisplayForFriendOnly]-->
                                                        <span class="privacy">好友可见</span>
                                                    <!--[else if $Article.DisplayForOwnerOnly]-->
                                                        <span class="privacy">自己可见</span>
                                                    <!--[/if]-->
                                                </div>
                                                <!--[/if]-->
                                                <div class="stat">
                                                    <span class="comments">评论($Article.TotalComments)</span>
                                                </div>
                                            </li>
                                            <!--[/loop]-->
                                        </ul>
                                    </div>
                                    <!--[else]-->
                                    <div class="nodata">
                                        <!--[if $VisitorIsOwner]-->
                                        你还没有日志. <a href="$url(app/blog/write)">点击这里可以发表日志</a>
                                        <!--[else]-->
                                        暂时没有日志.
                                        <!--[/if]-->
                                    </div>
                                    <!--[/if]-->
                                </div>
                            </div>
                            <!--[/if]-->
                            
                            <div class="panel activities">
                                <div class="panel-head">
                                    <h3 class="panel-title"><span>动态</span></h3>
                                    <!--[if $FeedCanDisplay != false && $feedList.Count > 0]-->
                                    <div class="panel-more"><a href="$url(space/feed-list)?uid=$SpaceOwnerID">更多动态</a></div>
                                    <!--[/if]-->
                                </div>
                                <div class="panel-body">
                                    <!--[if $feedList.Count > 0]-->
                                    <!--#include file="_feedbody.aspx"-->
                                    <!--[else if $FeedCanDisplay == false]-->
                                    <div class="nodata">
                                        由于空间主人对动态设置了隐私您无法查看他/她的动态.
                                    </div>
                                    <!--[else]-->
                                    <div class="nodata">
                                        当前没有任何动态.
                                    </div>
                                    <!--[/if]-->
                                </div>
                            </div>
                            
                            <div class="panel spacecomment" id="borad">
                                <div class="panel-head">
                                    <h3 class="panel-title"><span>留言板</span></h3>
                                    <!--[if $CommentList.Count > 0 && $BoardCanDisplay != false]-->
                                    <p class="panel-more"><a href="$url(space/board-list)?uid=$SpaceOwnerID">全部</a></p>
                                    <!--[/if]-->
                                </div>
                                <div class="panel-body">
                                    <!--#include file="_boardbody.aspx"-->
                                </div>
                            </div>
                            
                          <!--[/if]-->
                        </div>
                    </div>
                    
                    <div class="content-sub">
                        <div class="content-sub-inner">
                            
                        <!--[if $SpaceCanAccess]-->
                            <!--[if $IsShowImpression]-->
                            <!--[ajaxpanel id="sp_impression" idonly="true"]-->
                            <div class="panel spaceimpression" id="implist">
                                <div class="panel-head">
                                    <h3 class="panel-title"><span>好友印象</span></h3>
                                    <!--[if $IsShowImpressionInput == false]-->
                                        <!--[if $VisitorIsFriend && $CanImpression]-->
                                    <div class="panel-more"><a href="$url(space/$SpaceOwnerID)?imp=1" onclick="ajaxRender(this.href,'sp_impression',null);return false;">我要对他进行描述</a></div>
                                        <!--[/if]-->
                                    <!--[/if]-->
                                </div>
                                <div class="panel-body">
                                    <script type="text/javascript">
                                        function submitImpressionForm() {
                                            ajaxSubmit('ImpressionForm', 'CreateImpression', 'sp_impression', null, null, true);
                                        }
                                        function submitImpression(text) {
                                            $('ImpressionText').value = text;
                                            submitImpressionForm();
                                        }
                                    </script>
                                    <!--[if $IsShowImpressionInput && $VisitorIsFriend && $CanImpression]-->
                                    <form action="$url(space/$SpaceOwnerID)" method="post" id="ImpressionForm">
                                    <div class="formgroup impressionform">
                                        <!--[unnamederror form="ImpressionForum"]-->
                                        <div class="errormsg">$message</div>
                                        <!--[/unnamederror]-->
                                        <div class="formrow impressionform-input">
                                            <h3 class="label"><label for="ImpressionText">输入您对$SpaceOwner.Name的印象</label></h3>
                                            <div class="form-enter">
                                                <input class="text" type="text" name="Text" id="ImpressionText" onkeyup="textCounter(this,'impressionlimit',100)" />
                                            </div>
                                        </div>
                                        <!--[if $ImpressionTypeList.count != 0]-->
                                        <div class="formrow impressionform-suggest">
                                            <h3 class="label">供您参考的印象词:</h3>
                                            <div class="form-enter">
                                                <!--[loop $ImpressionType in $ImpressionTypeList with $i]-->
                                                <a href="#" onclick="submitImpression('$ImpressionType.Text');return false;">{=$ImpressionType.Text}</a>
                                                <!--[/loop]-->
                                            </div>
                                        </div>
                                        <!--[/if]-->
                                        <div class="formrow formrow-action">
                                            <div class="impressionform-submit">
                                                <span class="impression-textlimit" id="impressionlimit">100</span>
                                                <span class="minbtn-wrap"><span class="btn"><input class="button" id="ImpressionSubmit" type="button" onclick="submitImpressionForm();" value="提交" name="CreateImpression" /></span></span>
                                                <a href="$url(space/$SpaceOwnerID)?imp=0" onclick="ajaxRender(this.href,'sp_impression',null);return false;">取消</a>
                                            </div>
                                        </div>
                                    </div>
                                    </form>
                                    <!--[else]-->
                                        <!--[if $ImpressionList.Count == 0]-->
                                    <div class="nodata">
                                        <!--[if $VisitorIsOwner]-->
                                            你还没有收到任何印象描述
                                        <!--[else]-->
                                            当前没有任何印象描述。
                                        <!--[/if]-->
                                    </div>
                                        <!--[else]-->
                                    <script type="text/javascript">
                                        function submitImpressionDeleteForm(typeid) {
                                            if (confirm('确认要删除所选的好友印象吗？')) {
                                                var proxy = document.createElement('input');
                                                proxy.value = typeid.toString();
                                                proxy.name = 'TypeID';
                                                proxy.type = 'hidden';
                                                $('ImpressionDeleteForm').appendChild(proxy);
                                                ajaxSubmit('ImpressionDeleteForm', 'DeleteImpression', 'sp_impression', null, null, true);
                                            }
                                            return false;
                                        }
                                    </script>
                                    <form action="$url(space/$SpaceOwnerID)" method="post" id="ImpressionDeleteForm">
                                    <div class="impression-word">
                                        <!--[loop $Impression in $ImpressionList with $i]-->
                                        <a href="javascript:;" class="imp-{=$i%10+1}" title="被这样描述了{=$Impression.Count}次" onclick="return false;">
                                            $Impression.Text
                                            <!--[if $VisitorIsOwner]-->
                                            <span class="delete" onclick="submitImpressionDeleteForm($Impression.TypeID)">删除</span>
                                            <!--[/if]-->
                                        </a>
                                        <!--[/loop]-->
                                    </div>
                                    </form>
                                        <!--[/if]-->
                                    <!--[if $ImpressionRecordList != null && $ImpressionRecordList.count > 0]-->
                                    <div class="impression-log">
                                        <ul>
                                            <!--[loop $record in $ImpressionRecordList]-->
                                            <li><a class="fn" href="$url(space/$record.User.id)">$record.User.Name</a> <span  class="date">$OutputFriendlyDate($record.CreateDate)</span> 描述了: $record.Text</li>
                                            <!--[/loop]-->
                                        </ul>
                                    </div>
                                    <!--[/if]-->
                                    <!--[if $VisitorIsFriend && !$CanImpression]-->
                                    <div class="impression-note">
                                        您对他的印象描述 "$LastImpression", $NextImpressionTime后才能继续描述。
                                    </div>
                                    <!--[/if]-->
                                <!--[/if]-->
                                </div>
                            </div>
                            <!--[/ajaxpanel]-->
                            <!--[/if]-->
                        <!--[/if]-->
                            
                            <div class="panel spacevisitor">
                                <div class="panel-head">
                                    <h3 class="panel-title"><span>最近访客</span></h3>
                                    <!--[if $VisitorList.Count > 0]-->
                                    <div class="panel-more"><a href="$url(space/$SpaceOwnerID/friend)?view=visitor">全部</a></div>
                                    <!--[/if]-->
                                </div>
                                <div class="panel-body">
                                    <!--[if $VisitorList.Count == 0]-->
                                    <div class="nodata">
                                        最近一段时间内没有人访问过我的空间.
                                    </div>
                                    <!--[else]-->
                                    <div class="spacevisitorlist">
                                        <ul class="clearfix spacevisitor-list">
                                            <!--[loop $Visitor in $VisitorList]-->
                                            <li class="clearfix visitoritem">
                                                <%--<div class="action"><a class="action-delete" href="#">删除</a></div>--%>
                                                <div class="avatar"><a href="$url(space/$Visitor.User.id)"><img src="$Visitor.User.AvatarPath" alt="" width="48" height="48" /></a></div>
                                                <div class="name"><a class="fn" href="$url(space/$Visitor.User.id)">$Visitor.User.Name</a></div>
                                                <div class="date">$OutputFriendlyDate($Visitor.CreateDate)</div>
                                                <!--[if $Visitor.user.id != $myuserid]-->
                                                <div class="operate">
                                                    <span class="chat">
                                                        <!--[if $Visitor.user.isonline]-->
                                                        <a class="chat-online" href="$root/max-dialogs/chat.aspx?to=$Visitor.VisitorUserID" onclick="return openDialog(this.href);">对话</a>
                                                        <!--[else]-->
                                                        <a class="chat-offline" href="$root/max-dialogs/chat.aspx?to=$Visitor.VisitorUserID" onclick="return openDialog(this.href);">对话</a>
                                                        <!--[/if]-->
                                                    </span>
                                                </div>
                                                <!--[/if]-->
                                            </li>
                                            <!--[/loop]-->
                                        </ul>
                                    </div>
                                    <!--[/if]-->
                                </div>
                            </div>
                            
                        <!--[if $SpaceCanAccess]-->
                            <div class="panel spacefriend">
                                <div class="panel-head">
                                    <h3 class="panel-title">好友</h3>
                                    <!--[if $FriendList.Count > 0 && $FriendCanDisplay]-->
                                    <div class="panel-more"><a href="$url(space/$SpaceOwnerID/friend-list)">全部</a></div>
                                    <!--[/if]-->
                                </div>
                                <div class="panel-body">     
                                    <!--[if $FriendCanDisplay == false]-->
                                    <div class="nodata">
                                        由于空间主人设置了隐私您无法看到他/她的好友.
                                    </div>
                                    <!--[else if $FriendList.Count == 0]-->
                                    <div class="nodata">
                                        暂时还没有好友。
                                    </div>
                                    <!--[else]-->
                                    <div class="spacevfriendlist">
                                        <ul class="clearfix spacevfriend-list">
                                            <!--[loop $Friend in $FriendList]-->
                                            <li>
                                                <a class="avatar" href="$url(space/$friend.User.id)"><img src="$friend.User.AvatarPath" alt="" width="48" height="48" /></a>
                                                <a class="fn name" href="$url(space/$friend.User.id)">$friend.User.Name</a>
                                            </li>
                                            <!--[/loop]-->
                                        </ul>
                                    </div>
                                    <!--[/if]-->
                                </div>
                            </div>
                        <!--[/if]-->
                            
                        </div>
                    </div>
                    
                </div>
            </div>
            <!--#include file="_spacesidebar.aspx"-->
        </div>
        <!--#include file="../_inc/_round_bottom.aspx"-->
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>
</body>
</html>