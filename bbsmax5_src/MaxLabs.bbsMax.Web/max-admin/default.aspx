<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>BBSMax控制面板</title>
<!--[include src="_htmlhead_.aspx"/]-->
<style type="text/css">
.subnav{position:relative;z-index:9999;margin-bottom:-49px;padding-bottom:5px;background:url($root/max-assets/images/maxmanager_bgset.png) repeat-x 0 -85px;}
</style>
</head>
<body class="page-default">
<!--[include src="_head_.aspx" /]-->
<div class="container">
<div class="Content HomeColumns2">
    <div class="ColumnLeft"><div class="ColumnLeft-inner">
        <div class="panel quickboard">
            <div class="panel-head">
                <h3 style="background-image:url($root/max-assets/images/simple_tools.gif);">常用操作</h3>
            </div>
            <div class="clearfix panel-body">
                <h4 class="grouptitle"><span>全局</span></h4>
                <ul class="clearfix">
                <li><a href="global/setting-site.aspx"><span class="icon" style="background-position:0 0;">.</span>站点设置</a></li>
                <li><a href="global/setting-accesslimit.aspx"><span class="icon" style="background-position:-48px 0;">.</span>IP限制</a></li>
                <li><a href="global/setting-bannedword.aspx"><span class="icon" style="background-position:-96px 0;">.</span>敏感关键字</a></li>
                <li><a href="global/setting-validatecode.aspx"><span class="icon" style="background-position:-144px 0;">.</span>验证码</a></li>
                <li><a href="global/manage-announcement.aspx"><span class="icon" style="background-position:-192px 0;">.</span>管理公告</a></li>
                <!--[if $HasBBS]-->
                <li><a href="global/setting-links.aspx"><span class="icon" style="background-position:0 -48px;">.</span>友情链接</a></li>
                <li><a href="global/manage-report.aspx"><span class="icon" style="background-position:-48px -48px;">.</span>管理举报</a></li>
                <!--[/if]-->
                <li><a href="global/setting-email.aspx"><span class="icon" style="background-position:-48px -192px;">.</span>Email功能</a></li>
                </ul>
                <h4 class="grouptitle"><span>用户</span></h4>
                <ul class="clearfix">
                <li><a href="user/manage-user.aspx"><span class="icon" style="background-position:-96px -48px;">.</span>用户管理</a></li>
                <li><a href="user/manage-namecheck.aspx"><span class="icon" style="background-position:-144px -48px;">.</span>实名认证<%--span class="tips"><em>99</em></span--%></a></li>
                <li><a href="user/manage-shielduers.aspx"><span class="icon" style="background-position:-192px -48px;">.</span>屏蔽用户</a></li>
                <li><a href="user/manage-avatarcheck.aspx"><span class="icon" style="background-position:0 -96px;">.</span>头像审核</a></li>
                <li><a href="user/setting-roles-level.aspx"><span class="icon" style="background-position:-48px -96px;">.</span>用户组管理</a></li>
                <!--[if $HasBBS]-->
                <li><a href="user/setting-permissions.aspx?t=user"><span class="icon" style="background-position:-96px -96px;">.</span>用户权限分配</a></li>
                <!--[/if]-->
                <li><a href="user/manage-inviteserial.aspx"><span class="icon" style="background-position:-144px -96px;">.</span>邀请码管理</a></li>
                </ul>
                <!--[if $HasBBS]-->
                <h4 class="grouptitle"><span>论坛</span></h4>
                <ul class="clearfix">
                <li><a href="bbs/setting-bbs.aspx"><span class="icon" style="background-position:-192px -96px;">.</span>基本设置</a></li>
                <li><a href="bbs/manage-forum.aspx"><span class="icon" style="background-position:0 -288px;">.</span>版块及版主</a></li>
                <li><a href="bbs/manage-forum-detail.aspx?action=editsetting"><span class="icon" style="background-position:0 -144px;">.</span>发帖选项</a></li>
                <li><a href="bbs/manage-forum-detail.aspx?action=editusepermission"><span class="icon" style="background-position:-144px -288px;">.</span>版块用户权限</a></li>
                <li><a href="bbs/manage-forum-detail.aspx?action=editpoint"><span class="icon" style="background-position:-192px -288px;">.</span>版块积分策略</a></li>
                <li><a href="bbs/manage-forum-detail.aspx?action=editrate"><span class="icon" style="background-position:-48px -288px;">.</span>版块评分控制</a></li>
                <li><a href="bbs/manage-forum-detail.aspx?action=editmanagepermission"><span class="icon" style="background-position:-92px -288px;">.</span>版块管理权限</a></li>
                <li><a href="bbs/manage-topic.aspx"><span class="icon" style="background-position:-96px -192px;">.</span>管理帖子</a></li>
                </ul>
                <h4 class="grouptitle"><span>应用</span></h4>
                <ul class="clearfix">
                <li><a href="app/manage-blog.aspx"><span class="icon" style="background-position:-48px -144px;">.</span>文章管理</a></li>
                <li><a href="app/manage-photo.aspx"><span class="icon" style="background-position:-144px -144px;">.</span>照片管理</a></li>
                <li><a href="app/manage-doing.aspx"><span class="icon" style="background-position:0 -192px">.</span>记录管理</a></li>
                <li><a href="app/manage-share-data.aspx?type=favorite"><span class="icon" style="background-position:0 -240px;">.</span>收藏管理</a></li>
                <li><a href="app/manage-share-data.aspx?type=share"><span class="icon" style="background-position:-48px -240px;">.</span>分享管理</a></li>
                <li><a href="app/manage-netdisk.aspx"><span class="icon" style="background-position:-192px -240px;">.</span>网络硬盘管理</a></li>
                <li><a href="app/manage-emoticon.aspx"><span class="icon" style="background-position:-192px -192px;">.</span>表情管理</a></li>
                </ul>
                <!--[/if]-->
                <h4 class="grouptitle"><span>互动</span></h4>
                <ul class="clearfix">
                <!--[if $HasBBS]-->
                <li><a href="interactive/manage-mission-list.aspx"><span class="icon" style="background-position:-192px -336px">.</span>用户任务</a></li>
                <li><a href="interactive/manage-prop.aspx"><span class="icon" style="background-position:-96px -336px;">.</span>物品管理</a></li>
                <!--[/if]-->
                <li><a href="interactive/manage-chatsession.aspx"><span class="icon" style="background-position:0 -336px;">.</span>对话管理</a></li>
                <li><a href="interactive/manage-notify.aspx"><span class="icon" style="background-position:-144px -240px;">.</span>通知管理</a></li>
                <!--[if $HasBBS]-->
                <li><a href="global/manage-feed-data.aspx"><span class="icon" style="background-position:-48px -336px;">.</span>动态管理</a></li>
                <!--[/if]-->
                <li><a href="user/setting-friend.aspx"><span class="icon" style="background-position:-144px -336px;">.</span>好友设置</a></li>
                </ul>
                <h4 class="grouptitle"><span>维护</span></h4>
                <ul class="clearfix">
                <li><a href="other/manage-operationlog.aspx"><span class="icon" style="background-position:-192px -144px;">.</span>系统日志</a></li>
                <li><a href="other/manage-template.aspx"><span class="icon" style="background-position:-96px -240px;">.</span>模板管理</a></li>
                <!--[if $HasBBS]-->
                <li><a href="other/manage-plugin.aspx"><span class="icon" style="background-position:-96px -144px;">.</span>插件管理</a></li>
                <li><a href="other/manage-a.aspx?all"><span class="icon" style="background-position:-144px -192px;">.</span>广告管理</a></li>
                <!--[/if]-->
                </ul>
            </div>
        </div>

        <div class="clearfix">
        <div class="Column1">
            <div class="panel systeminfo">
                <div class="panel-head">
                    <h3 style="background-image:url($root/max-assets/images/simple_info.gif);">程序和系统信息</h3>
                </div>
                <div class="clearfix panel-body">
                    <ul id="systeminfo">
                    <li><span class="label">程序版本</span> <p class="stat">$Version</p></li>
                    <li><span class="label">操作系统</span> <p class="stat">$OSVersion</p></li>
                    <li><span class="label">程序内存占用</span> <p class="stat">物理内存:{=$memoryInfo.dwTotalPhys/(1024*1024)}M / 当前程序已占用物理内存:{=$memoryInfo.dwCurrentMemorySize/1024}M</p></li>
                    <li><span class="label">IIS版本</span> <p class="stat">$IISVersion</p></li>
                    <li><span class="label">.Net版本</span> <p class="stat">$NETVersion</p></li>
                    <li><span class="label">数据库版本</span> <p class="stat">$dataBaseInfo.Version</p></li>
                    <li><span class="label">数据库大小</span> <p class="stat">数据库:$FormatFileSize($dataBaseInfo.DataSize) / 日志:{=$FormatFileSize(dataBaseInfo.LogSize)}</p></li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="Column2">
            <div class="panel sitestatic">
                <div class="panel-head">
                    <h3 style="background-image:url($root/max-assets/images/simple_stat.gif);">网站统计</h3>
                </div>
                <div class="clearfix panel-body">
                    <ul id="sitestatic">
                    <li>
                        <span class="label">注册用户总数</span>
                        <p class="stat">$TotalUsers</p>
                    </li>
                    <!--[if $HasBBS]-->
                    <li>
                        <span class="label">在线人数</span>
                        <p class="stat">注册用户 $OnlineMemberCount<br />游客 $OnlineGuestCount</p>
                    </li>
                    <li>
                        <span class="label">论坛发帖总数</span>
                        <p class="stat">$TotalThreads</p>
                        <p class="stat">今天 $TodayPosts</p>
                        <p class="stat">昨日 $YestodayPosts</p>
                    </li>
                    <li><span class="label">论坛主题总数</span> <p class="stat">$TotalPosts</p></li>
                    <!--[/if]-->
                    </ul>
                </div>
            </div>
        </div>
        </div>
    </div></div>
    <div class="ColumnRight">
        <div class="panel">
            <div class="panel-head">
                <h3 style="background-image:url($root/max-assets/images/simple_check.gif);">版本检查</h3>
            </div>
            <div class="clearfix panel-body versioncheck">
                <div class="versiontips">
                    <iframe frameborder="0" width="100%" height="80" src="http://update.bbsmax.com/checkversion.aspx?ver=$InternalVersion&type=2&site=$SiteUrl" allowtransparency="true"></iframe>
                </div>
                <p><a href="http://www.bbsmax.com" target="_blank">访问官方网站获取最新版本.</a> </p>
                <p><a href="http://www.bbsmax.com/service.html" target="_blank"><img src="$root/max-assets/images/button_1.gif" alt="购买合法的商业授权" /></a></p>
            </div>
        </div>
        
        <div class="panel">
            <div class="panel-head">
                <h3 style="background-image:url($root/max-assets/images/simple_search.gif);">功能搜索</h3>
            </div>
            <div class="clearfix panel-body">
                <div class="quickform">
                    <form action="">
                    <input name="editusername" id="editusername" type="text" class="text" style="width:222px;" onkeyup="findMenu(this.value)" />
                    <div id="finddedMenus" class="featureresult" style="display:none;"></div>
                    </form>
                </div>
            </div>
        </div>
        <%-- 
        <div class="panel">
            <div class="panel-head"><div class="inner1"><div class="inner2">
                <h3>修改用户屏蔽状态</h3>
            </div></div></div>
            <div class="clearfix panel-body">
                <div class="quickform">
                    <form action="">
                    <input type="text" class="text" />
                    <input type="submit" class="button" value="提交" />
                    </form>
                </div>
            </div>
        </div>
        --%>
        <div class="panel loginlog">
            <div class="panel-head"><div class="inner1"><div class="inner2">
                <h3 style="background-image:url($root/max-assets/images/simple_log.gif);">后台登录日志</h3>
            </div></div></div>
            <div class="clearfix panel-body">
                <ul id="loginlog">
                <!--[loop $cl in $ConsoleLog]-->
                <li>
                    <p class="username"><strong>$cl.User.username</strong></p>
                    <%--<p>登录IP: 127.0.0.1(本地)</p>--%>
                    <p>登录时间: $outputDatetime($cl.logindate)</p>
                    <p>作业时间: $cl.timelength</p>
                </li>
                <!--[/loop]-->
                </ul>
            </div>
        </div>
    </div>
</div>
</div>
<script type="text/javascript">

var finddedMenus;
function findMenu(m) {

    var c =$('finddedMenus');

    if(m=="")
    {
        c.innerHTML = '';
        c.style.display='none';
        return ;
    }
    m=m.toLowerCase();
    finddedMenus=new Array();
    for( var i=0;i<menuTree.length;i++)
    {
        if( menuTree[i].Title.contains(m))
        {
            getLeafs(menuTree[i],finddedMenus);
        }
        else
        {
            findChild(m,menuTree[i],1);
        }
    }
    var html='<p class="noresult">没有查找到相应的功能!</p>';
    
    if(finddedMenus.length)
    {
        html="";
        for(var i=0;i<finddedMenus.length;i++)
        {
            html+=String.format('<p><a href="{0}">{1}</a></p>',finddedMenus[i].Url,finddedMenus[i].Title);
        }
    }

    c.innerHTML = html;
    c.style.display='';
}

function getLeafs(menu,f,t)
{
    var s =t||menu.Title;
    if(menu.SubPages instanceof Array)
    {
        for(var i =0 ;i<menu.SubPages.length;i++)
           getLeafs(menu.SubPages[i],f,s+" - "+menu.SubPages[i].Title);
    }
    else
    {
        f.push({Url:menu.Url,Title:s});
    }
}

function findChild(m,parent,depth,root)
{
    if(parent.SubPages instanceof Array)
    {
        for(var i=0;i<parent.SubPages.length;i++)
        {
          
            if(parent.SubPages[i].Title.toLowerCase().indexOf(m)>=0)
            if(depth>1)
            {
                
                finddedMenus.push({Title:root.Title + " - "+parent.Title+" - "+parent.SubPages[i].Title,Url:parent.SubPages[i].Url});
            }   
            else
            {
                getLeafs(parent.SubPages[i],finddedMenus,parent.Title+" - "+parent.SubPages[i].Title);
                continue;  
            }   
            findChild(m,parent.SubPages[i],depth+1,parent)
        }
    }
}

function styleRows(parent_elem_id, child_elem_tag, palette) {
    if ( document.getElementById(parent_elem_id) && document.getElementById(parent_elem_id).getElementsByTagName(child_elem_tag) ) {
        var targets = document.getElementById(parent_elem_id).getElementsByTagName(child_elem_tag);
        for (var i=0; i<targets.length; i++) {
            if (palette[i%palette.length] != '') {
                targets[i].style.backgroundColor = palette[i%palette.length];
            }
        }
    }
}
styleRows('systeminfo', 'li', ['','#f5f8fb']);
styleRows('sitestatic', 'li', ['','#f5f8fb']);
styleRows('loginlog', 'li', ['','#f5f8fb']);

</script>
<!--[include src="_foot_.aspx" /]-->
</body>
</html>
