<script type="text/javascript">
//动态表格取消新行
function cancelNewrow( id )
{
    removeElement($('row-'+id));
    if($("error"+id))
        removeElement($("error"+id))
    if($("errorarray"+id))
        removeElement($("errorarray"+id))
}

var menuTree    =null;
var lastMenu    =null; 
var current     =null;
var currentSub  =null;
var html        =null;
function getMenu(menu){
   // if (current == null)
    //     return;
    if (lastMenu==menu) return ;
    if(html==null) html=new stringBuilder(""); else html.clear();
    lastMenu = menu;
    var container = $('subMenu');
    container.style.display = 'block';
    var mainMenu =$('mainMenu');
    if(menuTree==null)
    {
        return ;
    }
    var menus;
    
    for( var k=0;k< menuTree.length;k++) {
        if(menuTree[k].Id==menu)
        {
            $('topMenu' + menuTree[k].Id).className = "current";
            menus=menuTree[k].SubPages;
        }
        else {
            m = $('topMenu' + menuTree[k].Id);

            m.className = "";
        }
    }

    if (current != null) {
        $('topMenu' + current).className = "current"

        if (menu != current) {

            $('topMenu' + current).className = "fade";
        }
    } else {
        $('TheDefaultPage').className = "fade";
    }
    
    if(menus instanceof Array)
    {
        for(var i=0;i<menus.length;i++)
        {
               html.append("<div class=\"subnavbox");
               html.append( menus[i].HasPermission?'"' : ' disable"');
               html.append( " style=\"width:"+ menus[i].Width + "px\">");
               html.append("<dl>");
               html.append( "<dt>" +menus[i].Title + "</dt>");
               if(menus[i].SubPages!=null)
               {
                   for(var j=0;j<menus[i].SubPages.length;j++)
                   {
                        if(menus[i].SubPages[j].HasPermission==true)
                        {
                             html.append("<dd><a href=\""+ root + '/max-admin/' + menus[i].SubPages[j].Url +"\"");
                             if(menus[i].SubPages[j].Id==currentSub)
                                html.append(' class="current" '); 
                             html.append(">" + menus[i].SubPages[j].Title+  "</a></dd>");
                        }
                     else
                     {
                        html.append("<dd><a class=\"disable\">"+ menus[i].SubPages[j].Title +"</a></dd>");
                     }
                   }
               }
            html.append("</dl></div>");
        }
    }
    container.innerHTML=html.toString();
    html=null;
    return false;
}
function recoverMenu() {
    if (current == null) {
        var container = $('subMenu');
        container.style.display = '';
        if (lastMenu) {
            $('topMenu' + lastMenu).className = "";
            lastMenu = null;
        }
        $('TheDefaultPage').className = "current";
    }
        getMenu(current);
}
</script>
<div class="dropdownmenu-wrap manageusermenu" id="usermenuTemplate" style="display:none;">
    <div class="dropdownmenu">
        <div class="dropdownmenu-inner">
            <!--[if $MenuPermission.Shield||$MenuPermission.UpdatUserProfile||$MenuPermission.DeleteUser]-->
            <h3 class="manageusermenu-title">用户操作</h3>
            <ul class="clearfix manageusermenu-list">
                <!--[if $MenuPermission.UpdatUserProfile]-->
                <li><a target="_blank" id="umm_UpdatUserProfile" href="$dialog/user-edit.aspx?id={0}" onclick="return openDialog(this.href);">编辑资料</a></li>
                <!--[/if]-->
                <!--[if $MenuPermission.Shield]-->
                <li><a target="_blank" id="umm_Shield" href="$dialog/user-shield.aspx?userid={0}" onclick="return openDialog(this.href)">屏蔽用户</a></li>
                <!--[/if]-->
                <!--[if $MenuPermission.DeleteUser]-->
                <li><a target="_blank" id="umm_DeleteUser" href="$dialog/user-delete.aspx?userid={0}" onclick="return openDialog(this.href,refresh)">删除用户</a></li>
                <!--[/if]-->
            </ul>
            <!--[/if]-->
            <h3 class="manageusermenu-title">用户数据管理</h3>
            <ul class="clearfix manageusermenu-list">
                <li><a target="_blank" id="umm_Thread" href="$url(Search)?SearchText={0}&Mode=3">主题管理</a></li>
                <li><a target="_blank" id="umm_Post" href="$url(Search)?SearchText={0}&Mode=5">帖子管理</a></li>
                <!--[if $EnableBlogFunction &&  $MenuPermission.Blog]-->
                <li><a target="_blank" id="umm_Blog" href="$admin/app/manage-blog.aspx?userid={0}">日志管理</a></li>
                <!--[/if]--><!--[if $EnableAlbumFunction &&  $MenuPermission.Album]-->
                <li><a target="_blank" id="umm_Album" href="$admin/app/manage-album.aspx?userid={0}">相册管理</a></li>
                <!--[/if]--><!--[if $EnableDoingFunction &&  $MenuPermission.Doing]-->
                <li><a target="_blank" id="umm_Doing" href="$admin/app/manage-doing.aspx?userid={0}">记录管理</a></li>
                <!--[/if]--><!--[if $EnableFavoriteFunction &&  $MenuPermission.Collection]-->
                <li><a target="_blank" id="umm_Collection" href="$admin/app/manage-share-data.aspx?type=favorite&userid={0}">收藏管理</a></li>
                <!--[/if]--><!--[if $EnableShareFunction &&  $MenuPermission.Share]-->
                <li><a target="_blank" id="umm_Share" href="$admin/app/manage-share-data.aspx?userid={0}">分享管理</a></li>
                <!--[/if]--><!--[if $EnableNetDiskFunction &&  $MenuPermission.NetDisk]-->
                <li><a target="_blank" id="umm_NetDisk" href="$admin/app/manage-netdisk.aspx?userid={0}">网络硬盘</a></li>
                <!--[/if]--><!--[if $EnableEmoticonFunction &&  $MenuPermission.Emoticon]-->
                <li><a target="_blank" id="umm_Emoticon" href="$admin/app/manage-emoticon.aspx?userid={0}">表情管理</a></li>
                <!--[/if]--><!--[if $MenuPermission.Notify]-->
                <li><a target="_blank" id="umm_Notify" href="$admin/interactive/manage-notify.aspx?userid={0}">通知管理</a></li>
                <!--[/if]--><!--[if $EnableChatFunction &&  $MenuPermission.Chat]-->
                <li><a target="_blank" id="umm_Chat" href="$admin/interactive/manage-chatsession.aspx?userid={0}">对话记录</a></li>
                <!--[/if]-->
            </ul>
        </div>
    </div>
</div>

<div class="container">
<div class="head" id="headMenu">
    <p class="userbar">
    $my.username -
    <!--[if $islogin ]-->
    <a href="$admin/logout.aspx?userid=$my.id">退出</a> -
    <!--[else]-->
    <a href="$url(login)">登录</a> -
    <!--[/if]-->
    <a href="$url(default)">网站首页</a>
      - <a href="$dialog/admin-menus.aspx" onclick="return openDialog(this.href)">功能导航</a>
    </p>
    <h1 class="logo"><span>bbsmax 4 管理员控制台</span></h1>
    <div class="nav" id="adminMenu" >
    <div class="masternav">
    <ul>
    <li><a id="TheDefaultPage" href="$admin/default.aspx" class="$_if($IsDefaultPage, 'current')"><span><em class="icon1">.</em>首页</span></a></li>
    <!--[AdminMenu level="1"]-->
    <li onmouseover="getMenu($page.id)">
        <!--[if $page.haspermission]-->
        <a href="$admin/$page.url" id="topMenu$page.id" class="$_if($page==$selected , 'current')"><span><em class="icon{=$i+1}">.</em> $page.Title</span></a>
        <!--[else]-->
        <a class="disable"><span><em class="icon{=$i+1}">.</em> $page.Title</span></a>
        <!--[/if]-->
    </li>
    <!--[/AdminMenu]-->
    </ul>
    </div>
    <div class="subnav" id="subMenu">
    <!--[AdminMenu level="2"]-->
        <div class="subnavbox $_if($page.haspermission==false, ' disable')" style="width:{=$page.width}px">
        <dl>
        <dt>$page.Title</dt>
            <!--[AdminMenu level="3" parent="$page"]-->
            <!--[if $page.haspermission]-->
            <dd><a href="$admin/$page.url" class="$_if( $page==$selected, 'current')">$page.Title</a></dd>
            <!--[else]-->
            <dd><a class="disable">$page.Title</a></dd>
            <!--[/if]-->
            <!--[/AdminMenu]-->
        </dl>
        </div>
    <!--[/AdminMenu]-->
    </div>
    </div>
</div>
<!--[if $RunningTaskList.Count > 0]-->
<script type="text/javascript">
    var task_DoNotReload = new Array();
    function updateTask(i, p, t) {
        
        document.getElementById('task_progressing_' + i).style.width = p + '%';
        document.getElementById('task_title_' + i).innerHTML = t;

        task_DoNotReload[i] = false;
    }
    function reloadTask() {
        for (var i = 0; i < task_DoNotReload.length; i++) {
            if (task_DoNotReload[i] != true) {
                task_DoNotReload[i] = true;
                document.getElementById('task_frame_' + i).contentWindow.location.reload(true);
            }
        }
        setTimeout(reloadTask, 200);
    }
    function finishTask(i, t) {
        task_DoNotReload[i] = true;
        document.getElementById('task_progressing_' + i).style.width = '100%';
        document.getElementById('task_title_' + i).innerHTML = t;
    }
</script>
<div class="TasksDock">
    <!--[loop $task in $RunningTaskList with $i]-->
    <div class="progressbar" id="task_all_$i">
        <div class="progressing" style="width:0%;" id="task_progressing_$i"><p>&nbsp;</p></div>
        <div class="progressbar-text" id="task_title_$i">$task.Title</div>
    </div>
    <script type="text/javascript">task_DoNotReload.push(true);</script>
    <iframe id="task_frame_$i" width="0" height="0" frameborder="1" src="$task.HandlerUrl&i=$i"></iframe>
    <!--[/loop]-->
</div>
<script type="text/javascript">reloadTask();</script>
<!--[/if]-->