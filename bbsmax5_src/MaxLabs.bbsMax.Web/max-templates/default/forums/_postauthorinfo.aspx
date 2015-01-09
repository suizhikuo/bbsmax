<div class="authorinfo"  id="authorinfoInner_$post.postid">
<!--[if $post.UserID == 0]-->
    <div class="authorinfo-inner">
        <div class="authorinfo-wrap">
            <div class="useridentity">
                <p class="user-name">
                    <strong>游客:<!--[if $post.Username!=""]-->$post.Username<!--[else]-->匿名<!--[/if]--></strong>
                </p>
            </div>
        </div>
    </div>
<!--[else if $my.IsSpider]-->
    <div class="vcard authorinfo-inner">
        <div class="authorinfo-wrap">
            <div class="useridentity">
                <p class="user-name">
                    <a class="url" href="$url(space/$post.UserID)">
                        <strong class="fn">$post.Username</strong>
                    </a>
                </p>
            </div>
        </div>
    </div>
<!--[else if $post.User.IsDeleted]-->
    <div class="authorinfo-inner">
        <div class="authorinfo-wrap">
            <div class="useridentity">
                <p class="user-name">
                    <strong class="fn">$post.Username</strong>
                </p>
            </div>
            <div class="user-delete">
                该用户已被删除.
            </div>
        </div>
    </div>
<!--[else if $forum.IsShieldedUser($post.UserID) == true]-->
    <div class="authorinfo-inner">
        <div class="authorinfo-wrap">
            <div class="useridentity">
                <p class="user-name">
                    <a href="$url(space/$post.UserID)">
                        <strong class="fn">$post.Username</strong>
                    </a>
                </p>
            </div>
            <div class="user-delete">
                该用户发言被屏蔽.
            </div>
        </div>
    </div>
<!--[else]-->
    <div class="vcard authorinfo-inner" <!--[if $post.PostType != PostType.ThreadContent || ($Thread.ThreadType != ThreadType.Poll && $Thread.ThreadType != ThreadType.Question && $Thread.ThreadType != ThreadType.Polemize)]--> onmouseover="if(window.showUserExtradata)showUserExtradata(this,$post.UserID,$post.postid);" <!--[/if]-->>
        <div class="clearfix authorinfo-wrap">
            <div class="useridentity">
                <p class="clearfix user-name">
                    <a class="url" href="$url(space/$post.UserID)">
                        <strong class="fn nickname">$post.Username</strong><!--[if $post.User.IsOnline]--><img src="$root/max-assets/icon/online_mini.gif" alt="" title="在线" /><!--[/if]-->
                    </a>
                </p>
                <p class="user-foruminfo">
                    <span class="user-baseinfo">
                        <!--[if $post.User.Birthday.Year > 1900]--><img src="$post.User.AtomImg" width="16" height="16" alt="$post.User.AtomName" title="$outputFriendlyDate($post.User.Birthday), $post.User.AtomName." /><!--[/if]-->
                        <!--[if $post.User.Gender==Gender.Male]-->
                        <img src="$root/max-assets/icon/male.gif" width="16" height="16" alt="男" title="男" />
                        <!--[else if $post.User.Gender==Gender.Female]-->
                        <img src="$root/max-assets/icon/female.gif" width="16" height="16" alt="女" title="女" />
                        <!--[else]-->
                        <!--[/if]-->
                    </span>
                    <!--[if $post.User.OnlineLevel > 0]-->
                    <span class="user-level">$post.User.GetOnlineLevelIcon(@"<img src=""{0}"" alt="""" title=""{1}"" /> ",$root+"/max-assets/icon-star/star.gif",$root+"/max-assets/icon-star/moon.gif",$root+"/max-assets/icon-star/sun.gif")</span>
                    <!--[/if]-->
                    <span class="user-userid">ID: $post.UserID</span>
                </p>
                <!--[if $post.User.doing != ""]-->
                <p class="user-currentstutes">$post.User.Doing</p>
                <!--[/if]-->
            </div>
            <div class="user-maindata">
                <!--[if $DisplayAvatar]-->
                <div class="user-avatar">
                    <img class="photo" src="$post.User.BigAvatarPath" alt="" width="120" height="120" />
                </div>
                <!--[/if]-->
                <div class="user-role">
                    <span>$post.User.RoleTitleIcon</span>
                    <span>$post.User.RoleTitle</span>
                </div>
                <!--[if $IsShowMedal($post.User)]-->
                <div class="user-medal">
                    $GetMedals(@"<a href=""{0}"" target=""_blank"">{1}</a>",@"<img src=""{0}"" alt=""{1}"" title=""{1}"" /> ",$post.User)
                </div>
                <!--[/if]-->
                <!--[if $IsLogin && ($post.UserID != $MyUserID)]-->
                <div class="user-interactive">
                    <!--[if $My.IsMyFriend($post.userid) == false]-->
                    <a class="addfriend" href="$dialog/friend-tryadd.aspx?uid=$post.userid" onclick="return openDialog(this.href);" title="加为好友">加为好友</a>
                    <!--[/if]-->
                    <!--[if $EnableChatFunction]-->
                    <a class="chat" href="$dialog/chat.aspx?to=$post.userid" onclick="return openDialog(this.href);" title="站内交谈">对话</a>
                    <!--[/if]-->
                </div>
                <!--[/if]-->
            </div>
            <div class="user-extradata" id="extradata_{=$post.UserID}_{=$post.postid}">
                <ul class="clearfix user-scorelist">
                    <li><strong>$GeneralPointName</strong> $post.User.Points</li>
                    $GetPoints($post.User,"<li><strong>{0}</strong> {1}{2}</li>")
                    <li><strong>帖子</strong> $post.User.TotalPosts</li>
                    <li class="user-created"><strong>注册时间</strong> $outputFriendlyDateTime($post.User.CreateDate)</li>
                    <li class="user-group"><strong>用户组</strong> $GetRoleNames($post.User.Roles,",")</li>
                </ul>
            </div>
            <!--[if $IsShowUserExtendProfile($post.User,"homepage","blog","qq","msn","taobao","alipay")]-->
            <div class="clearfix user-contact">
                <!--[if $GetUserExtendProfile($post.User,"homepage") != ""]--><a target="_blank" href="$FormatLink($GetUserExtendProfile($post.User,"homepage"))" title="网站:$FormatLink($GetUserExtendProfile($post.User,"homepage"))"><img src="$root/max-assets/icon-contact/web.gif" alt="网站" /></a><!--[/if]-->
                <!--[if $GetUserExtendProfile($post.User,"blog") != ""]--><a target="_blank" href="$FormatLink($GetUserExtendProfile($post.User,"blog"))" title="博客:$FormatLink($GetUserExtendProfile($post.User,"blog"))"><img src="$root/max-assets/icon-contact/blog.gif" alt="博客" /></a><!--[/if]-->
                <!--[if $GetUserExtendProfile($post.User,"qq") != ""]--><a href="tencent://message/?uin={=$GetUserExtendProfile($post.User,"qq")}&amp;Site={=$BbsName}&amp;Menu=yes" target="_blank"><img src="http://wpa.qq.com/pa?p=1:{=$GetUserExtendProfile($post.User,"qq")}:8" alt="" title="QQ交谈" /></a><!--[/if]-->
                <!--[if $GetUserExtendProfile($post.User,"msn") != ""]--><a href="msnim:chat?contact=$GetUserExtendProfile($post.User,"msn")" title="MSN交谈:$GetUserExtendProfile($post.User,"msn")"><img src="$root/max-assets/icon-contact/msn.gif" alt="msn" /></a><!--[/if]-->
                <!--[if $GetUserExtendProfile($post.User,"taobao") != ""]--><a target="_blank" href="http://webww.taobao.com/wangwang/webww1.htm" onclick="return webWW('$GetUserExtendProfile($post.User,"taobao")');" title="淘宝交谈:$GetUserExtendProfile($post.User,"taobao")"><img src="$root/max-assets/icon-contact/taobao.gif" alt="淘宝旺旺" /></a><!--[/if]-->
                <!--[if $GetUserExtendProfile($post.User,"alipay") != ""]--><a target="_blank" href="http://www.alipay.com" title="支付宝:$GetUserExtendProfile($post.User,"alipay")"><img src="$root/max-assets/icon-contact/alipay.gif" alt="支付宝" /></a><!--[/if]-->
            </div>
            <!--[/if]-->
        </div>
    </div>
<!--[/if]-->
    
</div>