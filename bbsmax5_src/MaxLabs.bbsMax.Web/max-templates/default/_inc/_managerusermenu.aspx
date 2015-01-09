<div class="dropdownmenu-wrap manageusermenu" id="usermenuTemplate" style="display:none;">
    <div class="dropdownmenu">
        <div class="dropdownmenu-inner">
            <!--[if  $MenuPermission.Shield||$MenuPermission.UpdatUserProfile||$MenuPermission.DeleteUser]-->
            <h3 class="manageusermenu-title">用户操作</h3>
            <ul class="clearfix manageusermenu-list">
                <!--[if $MenuPermission.UpdatUserProfile]-->
                <li><a target="_blank" id="umm_UpdatUserProfile" href="$dialog/user-edit.aspx?id={0}" onclick="openDialog(this.href);return false;">编辑资料</a></li>
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
