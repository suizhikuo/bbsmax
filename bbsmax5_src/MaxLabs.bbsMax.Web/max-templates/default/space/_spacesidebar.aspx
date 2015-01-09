<div class="spacesidebar">
    <div class="spacesidebar-inner">
        
        <div class="spaceavatar">
            <a href="$url(space/$AppOwner.ID)">$AppOwner.BigAvatar</a>
        </div>
        <!--[if $SpaceCanAccess]-->
        <!--[if $VisitorIsOwner == false && $VisitorIsOwnerFriend == false]-->
        <div class="spaceaddfriend">
            <a href="$dialog/friend-tryadd.aspx?uid=$AppOwnerUserID" onclick="return openDialog(this.href, function(result){})">
                加为好友
            </a>
        </div>
        <!--[/if]-->
        <!--[if $IsShowAppList]-->
        <div class="spaceapplist">
            <ul class="spaceapp-list">
                <!--[if $EnableDoingFunction]-->
                <li>
                    <a href="$url(app/doing/index)?uid=$AppOwnerUserID$_if(VisitorIsOwner,'&view=my')" title="$DoingFunctionName">
                        <img src="$root/max-assets/icon/doing.gif" alt="" width="16" height="16" />
                        $OwnerIt的$DoingFunctionName
                    </a>
                </li>
                <!--[/if]-->
                <!--[if $EnableBlogFunction]-->
                <li>
                    <a href="$url(app/blog/index)?uid=$AppOwnerUserID$_if(VisitorIsOwner,'&view=my')" title="日志">
                        <img src="$root/max-assets/icon/blog_edit.gif" alt="" width="16" height="16" />
                        $OwnerIt的日志
                    </a>
                </li>
                <!--[/if]-->
                <!--[if $EnableAlbumFunction]-->
                <li>
                    <a href="$url(app/album/index)?uid=$AppOwnerUserID$_if(VisitorIsOwner,'&view=my')" title="相册">
                        <img src="$root/max-assets/icon/album.gif" alt="" width="16" height="16" />
                        $OwnerIt的相册
                    </a>
                </li>
                <!--[/if]-->
                <!--[if $EnableShareFunction]-->
                <li>
                    <a href="$url(app/share/index)?uid=$AppOwnerUserID$_if(VisitorIsOwner,'&view=my')" title="分享">
                        <img src="$root/max-assets/icon/share.gif" alt="" width="16" height="16" />
                        $OwnerIt的分享
                    </a>
                </li>
                <!--[/if]-->
            </ul>
        </div>
        <!--[/if]-->
        <!--[/if]-->
        <div class="spaceoperatelist">
            <ul class="spaceoperate-list">
                <!--[if $VisitorIsOwner]-->
                <li>
                    <a href="$url(my/setting)" target="_blank">
                        <img src="$Root/max-assets/icon/vcard.gif" alt="" width="16" height="16" />
                        编辑资料
                    </a>
                </li>
                <!--[else]-->
                <li>
                    <a href="$dialog/hail.aspx?uid=$AppOwnerUserID" onclick="return openDialog(this.href, function(result){})">
                        <img src="$Root/max-assets/icon/hi.gif" alt="" width="16" height="16" />
                        打个招呼
                    </a>
                </li>
                <!--[/if]-->
                <!--[if $EnableChatFunction && !$VisitorIsOwner]-->
                <li>
                    <a href="$dialog/chat.aspx?to=$AppOwnerUserID" onclick="return openDialog(this.href, function(result){})">
                        <img src="$Root/max-assets/icon/chat.gif" alt="" width="16" height="16" />
                        发起对话
                    </a>
                </li>
                <!--[/if]-->
                <!--[if $EnablePropFunction]-->
                <li>
                    <a href="$dialog/prop-use.aspx?uid=$AppOwnerUserID" onclick="return openDialog(this.href, function(result){})">
                        <img src="$Root/max-assets/icon/magic.gif" alt="" width="16" height="16" />
                        使用道具
                    </a>
                </li>
                <!--[/if]-->
                <!--[if $my.ismanager]-->
                <li>
                    <a href="javascript:;" onclick="openUserMenu(this,$AppOwner.userid,null,true)">
                        <img src="$Root/max-assets/icon/user.gif" alt="" width="16" height="16" />
                        管理用户
                    </a>
                </li>
                <!--[/if]-->
                <!--[if !$VisitorIsOwner]-->
                <li>
                    <a href="$dialog/report-add.aspx?type=space&id=$AppOwnerUserID&uid=$AppOwnerUserID" onclick="return openDialog(this.href, function(result){})">
                        <img src="$Root/max-assets/icon/report.gif" alt="" width="16" height="16" />
                        违规举报
                    </a>
                </li>
                <!--[/if]-->
            </ul>
        </div>
        
        <!--[if $my.ismanager]-->
            <!--#include file="../_inc/_managerusermenu.aspx"-->
        <!--[/if]-->
    </div>
</div>
