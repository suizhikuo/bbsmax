<div class="clearfix rankinglayout $_if($islogin == false,' rankinglayout-nosidebar ')">
    <div class="rankinglayout-content">
        <div class="rankinglayout-content-inner">
            <div class="penel userstablepanel userstable-ranking">
                <div class="panel-head">
                    <h3 class="panel-title"><span>活跃度排行</span></h3>
                </div>
                <div class="panel-body">
                    <table class="userstable">
                        <thead>
                            <tr>
                                <td class="order">&nbsp;</td>
                                <td class="avatar">&nbsp;</td>
                                <td class="user">用户</td>
                                <td class="value"><!--[if $SortField==UserOrderBy.Points]--><strong>积分</strong><!--[else]--><a href="$AttachQueryString('sort=Points')">积分</a><!--[/if]--></td>
                                <!--[loop $up in $EnabledPoints]-->
                                <td class="value"><!--[if $IsSortByPoint($Up.Type)]--><strong>$up.name</strong><!--[else]--><a href="$AttachQueryString('sort=$Up.Type')">$up.name</a><!--[/if]--></td>
                                <!--[/loop]-->
                                <td class="value"><!--[if $SortField==UserOrderBy.TotalOnlineTime]--><strong>在线</strong><!--[else]--><a href="$AttachQueryString('sort=TotalOnlineTime')">在线</a><!--[/if]--></td>
                                <td class="value"><!--[if $SortField==UserOrderBy.TotalPosts]--><strong>发帖</strong><!--[else]--><a href="$AttachQueryString('sort=TotalPosts')" >发帖</a><!--[/if]--></td>
                            </tr>
                        </thead>
                        <tbody>
                            <!--[loop $user in $userlist with $i]-->
                            <tr $_if($user.UserID == $My.userID,'class="rankingtablerow-my"')>
                                <td class="order"><strong class="number">$index</strong></td>
                                <td class="avatar">$user.popupavatarlink</td>
                                <td class="user">
                                    <div class="rankinguser-name">
                                        $user.popupnamelink
                                        <!--[if $user.Realname!=""]-->
                                        <span class="realname">($user.realname)</span>
                                        <!--[/if]-->
                                        <!--[if $User.Gender == Gender.Male]-->
                                        <img src="$root/max-assets/icon/male.gif" alt="" />
                                        <!--[else if $User.Gender == Gender.Female]-->
                                        <img src="$root/max-assets/icon/female.gif" alt="" />
                                        <!--[/if]-->
                                    </div>
                                    <!--[if $user.UserID != $My.userID && $islogin]-->
                                    <div class="rankinguser-action">
                                        <!--[if !$My.ismyfriend($user.UserID)]-->
                                        <a class="addnewfriend" href="$dialog/friend-tryadd.aspx?uid=$User.ID" onclick="return openDialog(this.href, function(result){});">加为好友</a>
                                        <!--[/if]-->
                                        <a class="<!--[if $user.isonline]-->chat-online<!--[else]-->chat-offline<!--[/if]-->" href="$dialog/chat.aspx?to=$User.ID" onclick="return openDialog(this.href, function(result){});"><!--[if $user.isonline]-->会话<!--[else]-->发消息<!--[/if]--></a>
                                    </div>
                                    <!--[/if]-->
                                </td>
                                <td class="value">$user.points</td>
                                <!--[loop $up in $EnabledPoints]-->
                                <td class="value">$GetUserPointValue($user,$up)</td>
                                <!--[/loop]-->
                                <td class="value">$user.TotalOnlineHours</td>
                                <td class="value">$user.TotalPosts</td>
                            </tr>
                            <!--[/loop]-->
                        </tbody>
                    </table>
                </div>
            </div>
            <!--[pager name="list" skin="../_inc/_pager_app.aspx"]-->
        </div>
    </div>
    <!--[if $islogin]-->
    <div class="rankinglayout-sidebar">
        <div class="rankinglayout-sidebar-inner">
            <div class="clearfix rankingheader">
                
                <div class="panel round-tl myrankingorder">
                    <div class="panel-body round-tr">
                        <div class="round-bl"><div class="clearfix round-br">
                            <div class="rankingorder">
                                <p>您的<strong>总积分</strong>排名为 <span class="value">$rankinfos["Points"]</span></p>
                            </div>
                            <div class="rankingorder">
                                <p>您的<strong>总在线时间</strong>排名为 <span class="value">$rankinfos["TotalOnlineTime"]</span></p>
                            </div>
                             <div class="rankingorder">
                                <p>您的<strong>本月在线时间</strong>排名为 <span class="value">$rankinfos["MonthOnlineTime"]</span></p>
                            </div>
                            <div class="rankingorder">
                                <p>您的<strong>发帖数</strong>排名为 <span class="value">$rankinfos["TotalPosts"]</span></p>
                            </div>
                             <!--[loop $up in $EnabledPoints]-->
                                <div class="rankingorder">
                                <p>您的<strong>$up.name</strong>排名为 <span class="value">$GetPointRank($up)</span></p>
                                </div>
                            <!--[/loop]-->
                            <ul class="rankingvalue">
                                <li><span class="label">积分</span> <span class="value">$my.points</span></li>
                                <!--[loop $up in $EnabledPoints]-->
                                <li><span class="label">$up.name</span> <span class="value">$GetUserPointValue($my,$up)</span></li>
                                <!--[/loop]-->
                                <li><span class="label">在线</span> <span class="value">$my.TotalOnlineHours</span></li>
                                <li><span class="label">发帖</span> <span class="value">$my.TotalPosts</span></li>
                            </ul>
                        </div></div>
                    </div>
                </div>
                
            </div>
        </div>
    </div>
    <!--[/if]-->
</div>