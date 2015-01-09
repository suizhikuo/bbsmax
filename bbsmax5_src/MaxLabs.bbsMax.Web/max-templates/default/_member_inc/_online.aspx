<div class="panel userstablepanel userstable-online">
    <div class="panel-head">
        <h3 class="panel-title"><span>在线</span></h3>
    </div>
    <div class="panel-body">
        <table class="userstable">
            <thead>
                <tr>
                    <td class="roleicon">&nbsp;</td>
                    <td class="avatar mediumavatar">&nbsp;</td>
                    <td class="user">用户</td>
                    <td class="value">目前操作</td>
                    <td class="value">系统信息</td>
                    <td class="value">IP地址</td>
                    <td class="date">时间</td>
                </tr>
            </thead>
            <tbody>
            <!--[if $View=="onlineuser"]-->
                <!--[if $OnlineMembers.count > 0]-->
                    <!--[loop $member in $OnlineMembers]-->
                <tr>
                    <td class="roleicon"><img src="$GetRoleLogoUrl($member)" alt="" /></td>
                    <td class="avatar mediumavatar">
                        <!--[if $DisplayUserInfo($member)]-->
                        $member.User.smallAvatarlink
                        <!--[else]-->
                        <a href="javasript:void();"><img src="$root/max-assets/avatar/avatar_24.gif" alt="" /></a>
                        <!--[/if]-->
                    </td>
                    <td class="user">
                        <div class="rankinguser-name">
                            <!--[if $DisplayUserInfo($member)]-->
                                $member.user.popupnamelink
                                <!--[if $member.user.Realname!=""]-->
                                <span class="realname">($member.user.realname)</span>
                                <!--[/if]-->
                                <!--[if $member.user.Gender == Gender.Male]-->
                                <img src="$root/max-assets/icon/male.gif" alt="" />
                                <!--[else if $member.user.Gender == Gender.Female]-->
                                <img src="$root/max-assets/icon/female.gif" alt="" />
                                <!--[/if]-->
                                <!--[if $member.IsInvisible]-->
                                (隐身用户)
                                <!--[/if]-->
                            <!--[else]-->
                            (隐身用户)
                            <!--[/if]-->
                        </div>
                        <!--[if $DisplayUserInfo($member)]-->
                        <!--[if $member.UserID != $MyUserID && $islogin]-->
                        <div class="rankinguser-action">
                            <!--[if $IsFriend($member.UserID) == false]-->
                            <a class="addnewfriend" href="$dialog/friend-tryadd.aspx?uid=$member.UserID" onclick="return openDialog(this.href, function(result){});">加为好友</a>
                            <!--[/if]-->
                            <a class="<!--[if $member.user.isonline]-->chat-online<!--[else]-->chat-offline<!--[/if]-->" href="$dialog/chat.aspx?to=$member.UserID" onclick="return openDialog(this.href, function(result){});"><!--[if $member.user.isonline]-->会话<!--[else]-->发消息<!--[/if]--></a>
                        </div>
                        <!--[/if]-->
                        <!--[/if]-->
                    </td>
                    <td class="value">$GetMemberPosition($member) $GetOnlineActionName($member.Action)</td>
                    <td class="value">$member.Platform<br />$member.Browser</td>
                    <td class="value">$OutputIP($member.IP)<!--[if $CanShowIpArea]--><br />$member.location<!--[/if]--></td>
                    <td class="date">$outputdatetime($member.UpdateDate)</td>
                </tr>
                    <!--[/loop]-->
                <!--[else]-->
                <tr>
                    <td class="nodata" colspan="7">当前没有在线用户.</td>
                </tr>
                <!--[/if]-->
            <!--[/if]-->
            <!--[if $View=="onlineguest"]-->
                <!--[if $OnlineGuests.count > 0]-->
                    <!--[loop $guest in $OnlineGuests]-->
                <tr>
                    <td class="roleicon"><img src="$GuestRoleLogoUrl" alt="" /></td>
                    <td class="avatar mediumavatar">
                        <a href="javasript:vouid(0);"><img src="$root/max-assets/avatar/avatar_24.gif" alt="" /></a>
                    </td>
                    <td class="user">
                        <div class="rankinguser-name">
                            游客
                        </div>
                    </td>
                    <td class="value">$GetGuestPosition($guest) $GetOnlineActionName($guest.Action)</td>
                    <td class="value"><!--[if $guest.IsSpider]-->$guest.Platform (蜘蛛)<!--[else]-->$guest.Platform<br />$guest.Browser<!--[/if]--></td>
                    <td class="value">$OutputIP($guest.IP)<!--[if $CanShowIpArea]--><br />$guest.location<!--[/if]--></td>
                    <td class="date">$guest.UpdateDate</td>
                </tr>
                    <!--[/loop]-->
                <!--[else]-->
                <tr>
                    <td class="nodata" colspan="7">当前没有在线游客.</td>
                </tr>
                <!--[/if]-->
            <!--[/if]-->
            </tbody>
        </table>
    </div>
</div>
<!--[if ($View=="onlineuser" && $OnlineMembers.count > 0) || ($View=="onlineguest" && $OnlineGuests.count > 0)]-->
<!--[pager name="list" skin="../_inc/_pager_app.aspx"]-->
<!--[/if]-->