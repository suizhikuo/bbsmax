<div class="clearfix rankinglayout $_if($islogin == false,' rankinglayout-nosidebar ')">
    <div class="rankinglayout-content">
        <div class="rankinglayout-content-inner">
            <div class="filtertab rankingfilter-category">
                <ul class="clearfix tab-list">
                    <li><a href="$url(members)?view=viewnumber" class="$_if($View=='viewnumber','current')"><span>全部</span></a></li>
                    <li><a href="$url(members)?view=male" class="$_if($View=='male','current')"><span>只显示帅哥</span></a></li>
                    <li><a href="$url(members)?view=female" class="$_if($View=='female','current')"><span>只显示美女</span></a></li>
                </ul>
            </div>
            <div class="panel userstablepanel usertable-ranking">
                <div class="panel-head">
                    <h3 class="panel-title"><span>人气排行</span></h3>
                </div>
                <div class="panel-body">
                    <table class="userstable">
                        <thead>
                            <tr>
                                <td class="order">&nbsp;</td>
                                <td class="avatar">&nbsp;</td>
                                <td class="user">用户</td>
                                <td class="value"><!--[if $SortField==UserOrderBy.TotalViews]--><strong>空间访问</strong><!--[else]--><a href="$AttachQueryString('sort=TotalViews')">空间访问</a><!--[/if]--></td>
                                <td class="value"><!--[if $SortField==UserOrderBy.TotalFriends]--><strong>好友</strong><!--[else]--><a href="$AttachQueryString('sort=TotalFriends')">好友</a><!--[/if]--></td>
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
                                <td class="value">$user.TotalViews</td>
                                <td class="value">$user.TotalFriends</td>
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
                                <p>您的<strong>空间访问量</strong>排名为 <span class="value">$RankInfos["SpaceViews"]</span></p>
                            </div>
                            <div class="rankingorder">
                                <p>您的<strong>好友数</strong>排名为 <span class="value">$RankInfos["TotalFriends"]</span></p>
                            </div>
                            <ul class="rankingvalue">
                                <li><span class="label">空间访问</span> <span class="value">$my.TotalViews</span></li>
                                <li><span class="label">好友</span> <span class="value">$my.TotalFriends</span></li>
                            </ul>
                        </div></div>
                    </div>
                </div>
                
            </div>
        </div>
    </div>
    <!--[/if]-->
</div>
