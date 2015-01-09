<div class="content-sub">
    <div class="content-sub-inner">
        <!--#include file="../_inc/_round_top.aspx"-->
        <div class="sidebar">
            <!--[if $IsLogin == false]-->
            <div class="panel quicklogin">
                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                    <h3 class="panel-title"><span>用户登录</span></h3>
                </div></div></div>
                <div class="panel-body formgroup quicklogin-form">
                    <form method="post" enctype="multipart/form-data" action="$_form.action">
                    <!--[unnamederror]-->
                    <div class="errormsg">$message</div>
                    <!--[/unnamederror]-->
                    <div class="formrow">
                        <!--[if $LoginType==UserLoginType.Username]-->
                            <label class="label" for="username">用户名</label>
                        <!--[else if $LoginType==UserLoginType.Email]-->
                            <label class="label" for="username">邮箱</label>
                        <!--[else]-->
                            <select id="logintype" class="label" name="logintype">
                            <option value="username" $_form.selected("logintype","username")>账号</option>
                            <option value="email" $_form.selected("logintype","email")>邮箱</option>
                            </select>
                        <!--[/if]-->
                        <div class="form-enter">
                            <input type="text" class="text" name="username" id="all" value="$_form.text('username')" />
                        </div>
                    </div>
                    <div class="formrow">
                        <label class="label" for="password">密码</label>
                        <div class="form-enter">
                            <input class="text" type="password" name="password" id="password" />
                        </div>
                    </div>
                    <!--[ValidateCode actionType="login"]-->
                    <div class="formrow">
                        <label class="label" for="$inputName">验证码</label>
                        <div class="form-enter">
                            <input class="text validcode" type="text" name="$inputName" id="$inputName" onfocus="showVCode(this,'$imageurl');" autocomplete="off" $_if($disableIme,'style="ime-mode:disabled;"') />
                        </div>
                        <div class="form-note">$tip</div>
                    </div>
                    <!--[/ValidateCode]-->
                    <div class="formrow">
                        <input type="checkbox" name="cookietime" id="cookietime" value="820" $_form.checked("cookietime","820",true) />
                        <label for="cookietime">记住我的登录状态</label>
                    </div>
                    <div class="formrow">
                        <input type="checkbox" name="invisible" id="invisible" value="" />
                        <label for="invisible">隐身登录</label>
                    </div>
                    <div class="formrow formrow-action">
                        <span class="minbtn-wrap"><span class="btn"><input class="button" name="btLogin" type="submit" value="登录" /></span></span>
                    </div>
                    </form>
                    <div class="formrow">
                        <a href="$url(register)">注册</a> -
                        <a href="$url(recoverpassword)">找回密码</a>
                    </div>
                    <!--[if $LoginType==UserLoginType.Email]-->
                        <div class="formrow">
                            <a href="loginemailbind.aspx" >您已经拥有本站的用户名？请点击此处绑定email后再登陆</a>
                        </div>
                    <!--[/if]-->
                </div>
                <div class="panel-foot"><div><div>-</div></div></div>
            </div>
            <!--[else]-->
            <div class="panel myuserinfo">
                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                    <h3 class="panel-title"><span>我的信息</span></h3>
                </div></div></div>
                <div class="panel-body">
                    <p class="username">
                        <a href="$url(space/$MyUserID)" target="_blank"><strong>$My.Username</strong></a>
                        <!--[if $My.Realname != ""]-->
                        <span class="realname">$My.Realname</span>
                        <!--[/if]-->
                    </p>
                    <p class="avatar">
                        <a href="$url(my/avatar)" title="点击更换头像">
                            <img src="$My.AvatarPath" alt="" width="48" height="48" />
                            <span class="avatar-change">更改头像</span>
                        </a>
                    </p>
                    <ul class="mythreadcate">
                        <li><a href="$url(my/mythreads)">我发表的主题</a></li>
                        <li><a href="$url(my/mythreads)?type=myparticipantthread">我参与的主题</a></li>
                    </ul>
                    <ul class="myscore">
                        <li><span class="label">头衔</span> <em class="value">$My.RoleTitle</em></li>
                        <li><span class="label">主题/发帖</span> <em class="value">$My.TotalTopics / $My.TotalPosts</em> </li>
                        <li><span class="label">$GeneralPointName</span> <em class="value">$My.Points</em> <a href="$url(members)?view=show">参加竞价排名</a></li>
                        $GetPoints($My,'<li><span class="label">{0}</span> <em class="value">{1}{2}</em></li>')
                    </ul>
                </div>
                <div class="panel-foot"><div><div>-</div></div></div>
            </div>
            <!--[/if]-->
            
            <!--[ajaxpanel id="ap_GetTopThreads"]-->
            <div class="panel topiclist">
                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                    <div class="filtertab">
                        <ul class="clearfix tab-list">
                            <li><a href="$url(default)?topthreadtype=recent" <!--[if $TopThreadType=="recent"]-->class="current"<!--[/if]--> onclick="return ajaxRender(this.href,'ap_GetTopThreads')"><span>最新</span></a></li>
                            <li><a href="$url(default)?topthreadtype=hot" <!--[if $TopThreadType=="hot"]-->class="current" <!--[/if]--> onclick="return ajaxRender(this.href,'ap_GetTopThreads')"><span>热门</span></a></li>
                            <li><a href="$url(default)?topthreadtype=valued" <!--[if $TopThreadType=="valued"]-->class="current"<!--[/if]--> onclick="return ajaxRender(this.href,'ap_GetTopThreads')"><span>精华</span></a></li>
                        </ul>
                    </div></div></div>
                </div>
                <div class="panel-body">
                    <ul class="topiclist-list">
                    <!--[if $TopThreadType=="recent"]-->
                        <!--[NewThreadList count="9"]-->
                            <!--[if $threadList.Count == 0]-->
                                <li>暂时没有帖子</li>
                            <!--[else]-->
                                <!--[loop $thread in $threadList]-->
                                    <li>$GetThreadLink($thread, 25,@"<a href=""{0}"" title=""{2}"">{1}</a>",false)</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/NewThreadList]-->
                    <!--[else if $TopThreadType=="hot"]-->
                        <!--[WeekHotThreadList count="9"]-->
                            <!--[if $threadList.Count == 0]-->
                                <li>暂时没有热门帖子</li>
                            <!--[else]-->
                                <!--[loop $thread in $threadList]-->
                                    <li>$GetThreadLink($thread, 25,@"<a href=""{0}"" title=""{2}"">{1}</a>",false)</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/WeekHotThreadList]-->
                    <!--[else if $TopThreadType=="valued"]-->
                        <!--[ValuedThreadList count="9"]-->
                            <!--[if $threadList.Count == 0]-->
                                <li>暂时没有精华帖子</li>
                            <!--[else]-->
                                <!--[loop $thread in $threadList]-->
                                    <li>$GetThreadLink($thread, 25,@"<a href=""{0}"" title=""{2}"">{1}</a>",false)</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/ValuedThreadList]-->
                    <!--[/if]-->
                    </ul>
                </div>
                <div class="panel-foot"><div><div>-</div></div></div>
            </div>
            <!--[/ajaxpanel]-->

            <div class="panel membershowlist">
                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                    <h3 class="panel-title"><span>本周发帖明星</span></h3>
                </div></div></div>
                <div class="panel-body">
                    <!--[WeekMostPostUserList count="9"]-->
                    <!--[if $userList.Count == 0]-->
                    <div class="nodata">暂时没有</div>
                    <!--[else]-->
                    <ul class="clearfix membershow-list">
                        <!--[loop $user in $userList]-->
                        <li>
                            <a class="avatar" href="$url(space/$user.id)" title="$user.weekposts">
                                $user.avatar
                            </a>
                            <a class="name" href="$url(space/$user.id)" title="$user.weekposts">$user.username</a>
                        </li>
                        <!--[/loop]-->
                    </ul>
                    <!--[/if]-->
                    <!--[/WeekMostPostUserList]-->
                </div>
                <div class="panel-foot"><div><div>-</div></div></div>
            </div>
            <!--[if $myuserID == -10000]--> 
            <div class="panel membershowlist">
                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                    <h3 class="panel-title"><span>本周在线排行</span></h3>
                </div></div></div>
                <div class="panel-body">
                    <!--[WeekMostOnlineUserList count="9"]-->
                    <!--[if $userList.Count == 0]-->
                    <div class="nodata">暂时没有</div>
                    <!--[else]-->
                    <ul class="clearfix membershow-list">
                        <!--[loop $user in $userList]-->
                        <li>
                            <a class="avatar" href="$url(space/$user.id)" title="$user.weekonlinetime">
                                $user.avatar
                            </a>
                            <a class="name" href="$url(space/$user.id)" title="$user.weekOnlineTime">$user.username</a>
                        </li>
                        <!--[/loop]-->
                    </ul>
                    <!--[/if]-->
                    <!--[/WeekMostOnlineUserList]-->
                </div>
                <div class="panel-foot"><div><div>-</div></div></div>
            </div>

            <div class="panel membershowlist">
                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                    <h3 class="panel-title"><span>本日在线排行</span></h3>
                </div></div></div>
                <div class="panel-body">
                    <!--[DayMostOnlineUserList count="9"]-->
                    <!--[if $userList.Count == 0]-->
                    <div class="nodata">暂时没有</div>
                    <!--[else]-->
                    <ul class="clearfix membershow-list">
                        <!--[loop $user in $userList]-->
                        <li>
                            <a class="avatar" href="$url(space/$user.id)" title="$user.dayonlinetime">
                                $user.avatar
                            </a>
                            <a class="name" href="$url(space/$user.id)" title="$user.dayOnlineTime">$user.username</a>
                        </li>
                        <!--[/loop]-->
                    </ul>
                    <!--[/if]-->
                    <!--[/DayMostOnlineUserList]-->
                </div>
                <div class="panel-foot"><div><div>-</div></div></div>
            </div>
            
            <div class="panel membershowlist">
                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                    <h3 class="panel-title"><span>本日发帖排行</span></h3>
                </div></div></div>
                <div class="panel-body">
                    <!--[DayMostPostUserList count="9"]-->
                    <!--[if $userList.Count == 0]-->
                    <div class="nodata">暂时没有</div>
                    <!--[else]-->
                    <ul class="clearfix membershow-list">
                        <!--[loop $user in $userList]-->
                        <li>
                            <a class="avatar" href="$url(space/$user.id)" title="$user.dayposts">
                                $user.avatar
                            </a>
                            <a class="name" href="$url(space/$user.id)" title="$user.dayposts">$user.username</a>
                        </li>
                        <!--[/loop]-->
                    </ul>
                    <!--[/if]-->
                    <!--[/DayMostPostUserList]-->
                </div>
                <div class="panel-foot"><div><div>-</div></div></div>
            </div>
            
            <div class="panel topiclist">
                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                    <h3 class="panel-title"><span>最新回复</span></h3>
                </div></div></div>
                <div class="panel-body">
                    <ul class="topiclist-list">
                        <!--[NewRepliedThreadList count="9"]-->
                            <li><span style="color:Red">最新回复</span></li>
                            <!--[if $threadList.Count == 0]-->
                                <li>暂时没有帖子</li>
                            <!--[else]-->
                                <!--[loop $thread in $threadList]-->
                                    <li>$GetThreadLink($thread, 25,@"<a href=""{0}"" title=""{2}"">{1}</a>",false)  $thread.UpdateDate</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/NewRepliedThreadList]-->
                        
                        
                        <!--[NewRepliedThreadList ForumID="145" count="9"]-->
                            <li><span style="color:Red">(版块145)最新回复</span></li>
                            <!--[if $threadList.Count == 0]-->
                                <li>暂时没有帖子</li>
                            <!--[else]-->
                                <!--[loop $thread in $threadList]-->
                                    <li>$GetThreadLink($thread, 25,@"<a href=""{0}"" title=""{2}"">{1}</a>",false)  $thread.UpdateDate</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/NewRepliedThreadList]-->
                        
                        <!--[DayHotThreadList count="9"]-->
                            <li><span style="color:Red">今日热门</span></li>
                            <!--[if $threadList.Count == 0]-->
                                <li>暂时没有热门帖子</li>
                            <!--[else]-->
                                <!--[loop $thread in $threadList]-->
                                    <li>$GetThreadLink($thread, 25,@"<a href=""{0}"" title=""{2}"">{1}</a>",false)  $thread.totalreplies</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/DayHotThreadList]-->
                        
                        <!--[WeekTopViewThreadList count="9"]-->
                            <li><span style="color:Red">本周查看最多</span></li>
                            <!--[if $threadList.Count == 0]-->
                                <li>暂时没有精华帖子</li>
                            <!--[else]-->
                                <!--[loop $thread in $threadList]-->
                                    <li>$GetThreadLink($thread, 25,@"<a href=""{0}"" title=""{2}"">{1}</a>",false) $thread.TotalViews</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/WeekTopViewThreadList]-->
                        
                        
                        <!--[DayTopViewThreadList count="9"]-->
                            <li><span style="color:Red">今日查看最多</span></li>
                            <!--[if $threadList.Count == 0]-->
                                <li>暂时没有精华帖子</li>
                            <!--[else]-->
                                <!--[loop $thread in $threadList]-->
                                    <li>$GetThreadLink($thread, 25,@"<a href=""{0}"" title=""{2}"">{1}</a>",false) $thread.TotalViews</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/DayTopViewThreadList]-->
                        
                        
                        <!--[MonthMostOnlineUserList count="9"]-->
                            <li><span style="color:Red">本月在线最长</span></li>
                            <!--[if $userList.Count == 0]-->
                                <li>暂时没有</li>
                            <!--[else]-->
                                <!--[loop $user in $userList]-->
                                    <li>$user.username $user.monthOnlineTime</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/MonthMostOnlineUserList]-->
                        
                        <!--[WeekMostOnlineUserList count="9"]-->
                            <li><span style="color:Red">本周在线最长</span></li>
                            <!--[if $userList.Count == 0]-->
                                <li>暂时没有</li>
                            <!--[else]-->
                                <!--[loop $user in $userList]-->
                                    <li>$user.username $user.weekOnlineTime</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/WeekMostOnlineUserList]-->
                        
                        <!--[DayMostOnlineUserList count="9"]-->
                            <li><span style="color:Red">本日在线最长</span></li>
                            <!--[if $userList.Count == 0]-->
                                <li>暂时没有</li>
                            <!--[else]-->
                                <!--[loop $user in $userList]-->
                                    <li>$user.username $user.dayOnlineTime</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/DayMostOnlineUserList]-->
                        
                        <!--[MonthMostPostUserList count="9"]-->
                            <li><span style="color:Red">本月发帖最多</span></li>
                            <!--[if $userList.Count == 0]-->
                                <li>暂时没有</li>
                            <!--[else]-->
                                <!--[loop $user in $userList]-->
                                    <li>$user.username $user.monthposts</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/MonthMostPostUserList]-->

                        <!--[WeekMostPostUserList count="9"]-->
                            <li><span style="color:Red">本周发帖最多</span></li>
                            <!--[if $userList.Count == 0]-->
                                <li>暂时没有</li>
                            <!--[else]-->
                                <!--[loop $user in $userList]-->
                                    <li>$user.username $user.weekposts</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/WeekMostPostUserList]-->
                        
                        <!--[DayMostPostUserList count="9"]-->
                            <li><span style="color:Red">本日发帖最多</span></li>
                            <!--[if $userList.Count == 0]-->
                                <li>暂时没有</li>
                            <!--[else]-->
                                <!--[loop $user in $userList]-->
                                    <li>$user.username $user.dayposts</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/DayMostPostUserList]-->
                        
                        <!--[PointShowUserList count="9"]-->
                            <li><span style="color:Red">竞价排行</span></li>
                            <!--[if $showPointUserList.Count == 0]-->
                                <li>暂时没有</li>
                            <!--[else]-->
                                <!--[loop $showPointUser in $showPointUserList]-->
                                    <li>$showPointUser.User.username $showPointUser.Price</li>
                                <!--[/loop]-->
                            <!--[/if]-->
                        <!--[/PointShowUserList]-->
                    </ul>
                </div>
                <div class="panel-foot"><div><div>-</div></div></div>
            </div>
            <!--[/if]-->
            
        </div>
        <!--#include file="../_inc/_round_bottom.aspx"-->
    </div>
</div>
