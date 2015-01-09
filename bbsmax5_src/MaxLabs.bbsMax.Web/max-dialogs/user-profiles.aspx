<!--[DialogMaster width="550"]-->
<!--[place id="body"]-->
<div class="clearfix dialoghead" id="dialogTitleBar_$PanelID">
    <h3 class="dialogtitle">$user.username的资料</h3>
    <div class="dialogclose"><a href="javascript:void(panel.close());" accesskey="Q" title="关闭">关闭(<u>Q</u>)</a></div>
    <script type="text/javascript">
        maxDragObject(currentPanel.panel, $('dialogTitleBar_$PanelID'));
    </script>
</div>
<div class="clearfix dialogbody dialoguserprofile">
    <div class="dialoguser-sidebar">
        <div class="dialoguser-avatar">
            <a href="$url(space/$user.ID)" target="_blank"><img src="$user.avatarpath" alt="" /></a>
        </div>
        <ul class="dialoguser-menu">
            <li><a class="current icon-profile" href="$dialog/user-profiles.aspx?uid=$user.id">个人资料</a></li>
            <li><a class="icon-impression" href="$dialog/user-impressions.aspx?uid=$user.id">好友印象</a></li>
        </ul>
    </div>
    <div class="dialoguser-content">
    <div class="dialoguser-profilelist">
        <ul class="clearfix dialoguser-profilelist-info">
            <li class="user-title">
                <strong class="user-name">$user.username</strong>
                <span class="user-id">(ID: $user.ID)</span>
                <!--[if $User.OnlineLevel > 0]-->
                <span class="user-level">$User.GetOnlineLevelIcon(@"<img src=""{0}"" alt="""" title=""{1}"" />",$root+"/max-assets/icon-star/star.gif",$root+"/max-assets/icon-star/moon.gif",$root+"/max-assets/icon-star/sun.gif")</span>
                <!--[/if]-->
            </li>
            <li class="user-status">
                <span class="label">心情:</span>
                <span class="value">$user.Doing</span>
            </li>
            <li>
                <span class="label">性别:</span>
                <span class="value">
                    <!--[if $user.Gender == Gender.Male]-->
                    <img class="gander" src="$Root/max-assets/icon/male.gif" alt="" title="男" />
                    <!--[else if $user.Gender == Gender.Female]-->
                    <img class="gander" src="$Root/max-assets/icon/female.gif" alt="" title="女" />
                    <!--[else]-->
                    保密
                    <!--[/if]-->
                </span>
            </li>
            <li>
                <span class="label">生日:</span>
                <span class="value">
                    <!--[if $User.Birthday.Year > 1900]-->
                    $outputFriendlyDate($User.Birthday)
                    <img src="$User.AtomImg" width="16" height="16" alt="$User.AtomName" title="$User.AtomName." />
                    <!--[/if]-->
                </span>
            </li>
            <!--[UserExtendedFieldList userID="$user.userID"]-->
            <!--[if $IsShow($privacyType)]-->
            <li>
                <span class="label">$Name:</span>
                <span class="value">$userValue</span>
            </li>
            <!--[/if]-->
            <!--[/UserExtendedFieldList]-->
            <li>
                <span class="label">总积分:</span>
                <span class="value">$user.Points</span>
            </li>
            $GetPoints($User,'<li><span class="label">{0}:</span> <span class="value">{1}{2}</span></li>')
            <li>
                <span class="label">等级:</span>
                <span class="value">$User.RoleTitle</span>
            </li>
            <li>
                <span class="label">好友数量:</span>
                <span class="value">$user.TotalFriends</span>
            </ii>
            <li>
                <span class="label">帖子:</span>
                <span class="value">$User.TotalPosts</span>
            </li>
            <li>
                <span class="label">注册时间:</span>
                <span class="value">$outputDatetime($user.createdate)</span>
            </li>
            <li>
                <span class="label">注册IP:</span>
                <span class="value" <!--[if $IsShowAddress]-->title="$GetIpAddress($user.CreateIP)"<!--[/if]-->>$outputip($user.CreateIP)</span>
            </li>
            <li>
                <span class="label">最后浏览IP:</span>
                <span class="value" <!--[if $IsShowAddress]-->title="$GetIpAddress($user.lastvisitip)"<!--[/if]-->>
                <!--[if $CanShowIpArea]-->
                <a onclick="return openDialog(this.href);" href="$dialog/ip.aspx?ip=$OutputIP($user.lastvisitip)">$outputip($user.lastvisitip)</a>
                <!--[else]-->
                $outputip($user.lastvisitip)
                <!--[/if]-->
                </span>
            </li>
            <li class="user-group">
                <span class="label">用户组:</span>
                <span class="value">$GetRoleNames($User.Roles,",")</span>
            </li>
            <!--[if $IsShowUserExtendProfile($User,"homepage","blog","qq","msn","taobao","alipay")]-->
            <li class="user-contact">
                <!--[if $GetUserExtendProfile($User,"homepage") != ""]--><a target="_blank" href="$FormatLink($GetUserExtendProfile($User,"homepage"))" title="网站:$FormatLink($GetUserExtendProfile($User,"homepage"))"><img src="$root/max-assets/icon-contact/web.gif" alt="网站" /></a><!--[/if]-->
                <!--[if $GetUserExtendProfile($User,"blog") != ""]--><a target="_blank" href="$FormatLink($GetUserExtendProfile($User,"blog"))" title="博客:$FormatLink($GetUserExtendProfile($User,"blog"))"><img src="$root/max-assets/icon-contact/blog.gif" alt="博客" /></a><!--[/if]-->
                <!--[if $GetUserExtendProfile($User,"qq") != ""]--><a href="tencent://message/?uin={=$GetUserExtendProfile($User,"qq")}&amp;Site={=$BbsName}&amp;Menu=yes" target="_blank"><img src="http://wpa.qq.com/pa?p=1:{=$GetUserExtendProfile($User,"qq")}:8" alt="" title="QQ交谈" /></a><!--[/if]-->
                <!--[if $GetUserExtendProfile($User,"msn") != ""]--><a href="msnim:chat?contact=$GetUserExtendProfile($User,"msn")" title="MSN交谈:$GetUserExtendProfile($User,"msn")"><img src="$root/max-assets/icon-contact/msn.gif" alt="msn" /></a><!--[/if]-->
                <!--[if $GetUserExtendProfile($User,"taobao") != ""]--><a target="_blank" href="http://webww.taobao.com/wangwang/webww1.htm" onclick="return webWW('$GetUserExtendProfile($User,"taobao")');" title="淘宝交谈:$GetUserExtendProfile($User,"taobao")"><img src="$root/max-assets/icon-contact/taobao.gif" alt="淘宝旺旺" /></a><!--[/if]-->
                <!--[if $GetUserExtendProfile($User,"alipay") != ""]--><a target="_blank" href="http://www.alipay.com" title="支付宝:$GetUserExtendProfile($User,"alipay")"><img src="$root/max-assets/icon-contact/alipay.gif" alt="支付宝" /></a><!--[/if]-->
            </li>
            <!--[/if]-->
        </ul>


        <!--[if $IsShowMedal($User)]-->
        <div class="dialoguser-profilelist-medal">
            $GetMedals(@"<a href=""{0}"" target=""_blank"">{1}</a>",@"<img src=""{0}"" alt=""{1}"" title=""{1}"" /> ",$User)
        </div>
        <!--[/if]-->

    </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭(<u>C</u>)</span></button>
</div>
<!--[/place]-->
<!--[/dialogmaster]-->
