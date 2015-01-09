<div class="appsidebar">
    <!--#include file="../_inc/_round_top.aspx"-->
    <div class="appsidebar-inner">
        <div class="appsidebar-toggle">
            <a id="sidebarcloser" href="javascript:void(0);" onclick="appSidebarToggle('hasappsidebar', 'minappsidebar');return false;">&laquo;<span>.</span></a>
        </div>
        <div id="appsidebar-menu" class="appsidebar-menu">
            <ul class="menugroup menugroup-1">
                <li><a $_if($PageName=='userhome','class="current"') href="$url(my/default)" title="中心首页"><span style="background-image:url($root/max-assets/icon/home.gif);">动态</span></a></li>
                <!--[if $EnableMissionFunction]-->
                <li><a $_if($PageName=='mission','class="current"') href="$url(mission/index)" title="任务"><span style="background-image:url($root/max-assets/icon/event_time.gif);">任务</span></a></li>
                <!--[/if]-->
                <!--[if $EnablePropFunction]-->
                <li><a $_if($PageName=='prop','class="current"') href="$url(prop/index)" title="道具"><span style="background-image:url($root/max-assets/icon/magic.gif);">道具</span></a></li>
                <!--[/if]-->
            </ul>
            <ul class="menugroup menugroup-2">
                <li><a $_if($PageName=='friends','class="current"') href="$url(my/friends)" title="好友"><span style="background-image:url($root/max-assets/icon/friend.gif);">好友</span></a></li>
                <li><a $_if($PageName=='chat','class="current"') href="$url(my/chat)" title="消息"><span style="background-image:url($root/max-assets/icon/chat.gif);">消息</span></a></li>
                <li><a $_if($PageName=='notify' || $PageName=='notify-setting','class="current"') href="$url(my/notify)" title="通知"><span style="background-image:url($root/max-assets/icon/megaphone.gif);">通知</span></a></li>
                <li><a $_if($PageName=='mythreads','class="current"') href="$url(my/mythreads)" title="主题"><span style="background-image:url($root/max-assets/icon/topic.gif);">主题</span></a></li>
                <%--
                <li ><a href="$url(app/club)" title="群组"><span style="background-image:url($root/max-assets/icon/group.gif);">群组</span></a></li>
                --%>
            </ul>
            <ul class="menugroup menugroup-3">
                <!--[if EnableDoingFunction]-->
                <li><a $_if($PageName=='doing','class="current"') href="$url(app/doing/index)?view=friend" title="记录"><span style="background-image:url($root/max-assets/icon/doing.gif);">记录</span></a></li>
                <!--[/if]-->
                <!--[if EnableBlogFunction]-->
                <li><a $_if($PageName=='blog','class="current"') href="$url(app/blog/index)?view=friend" title="日志"><span style="background-image:url($root/max-assets/icon/blog_edit.gif);">日志</span></a></li>
                <!--[/if]-->
                <!--[if EnableAlbumFunction]-->
                <li><a $_if($PageName=='album','class="current"') href="$url(app/album/index)?view=friend" title="相册"><span style="background-image:url($root/max-assets/icon/album.gif);">相册</span></a></li>
                <!--[/if]-->
                <!--[if EnableShareFunction]-->
                <li><a $_if($PageName=='share','class="current"') href="$url(app/share/index)?view=friend" title="分享"><span style="background-image:url($root/max-assets/icon/share.gif);">分享</span></a></li>
                <!--[/if]-->
                <!--[if EnableFavoriteFunction]-->
                <li><a $_if($PageName=='favorite','class="current"') href="$url(app/share/index)?mode=fav" title="收藏"><span style="background-image:url($root/max-assets/icon/fav.gif);">收藏</span></a></li>
                <!--[/if]-->
                <!--[if EnableEmoticonFunction]-->
                <li><a $_if($PageName=='emoticon','class="current"') href="$url(app/emoticon/index)" title="表情"><span style="background-image:url($root/max-assets/icon/emoticons.gif);">表情</span></a></li>
                <!--[/if]-->
                <!--[if $EnableNetDiskFunction]-->
                <li><a $_if($PageName=='disk','class="current"') href="$url(app/disk/index)" title="网络硬盘"><span style="background-image:url($root/max-assets/icon/folder_web.gif);">网络硬盘</span></a></li>
                <!--[/if]-->
                 <%--
                <li ><a href="#" title="点亮图标"><span style="background-image:url($root/max-assets/icon/ruby.gif);">点亮图标</span></a></li>
                 --%>
            </ul>
        </div>
    </div>
    <!--#include file="../_inc/_round_bottom.aspx"-->
</div>
<script type="text/javascript">
    addHandler(window, "load", function() {
        var elem = $("main");
        var elem_class = elem.className;
        if ((document.cookie.indexOf("appsidebar=minappsidebar") != -1) && (elem_class.indexOf("hasappsidebar") != -1)) {
            elem.className = elem_class.replace("hasappsidebar", "minappsidebar");
        }
        if (document.cookie.indexOf("sidebar=off") != -1) {
            elem.className = elem_class + " nosidebar";
        }
    }
);

function appSidebarToggle(c_1, c_2) {
    var elem = $("main");
    if ( elem ) {
        var elem_class = elem.className;
        if (elem_class.indexOf(c_1) != -1) {
            elem_class = elem_class.replace(c_1, c_2);
            elem.className = elem_class;
            document.cookie = "appsidebar=" + c_2 + ";";
        } else {
            removeCssClass(elem, c_2);
            addCssClass(elem, c_1);
            document.cookie = "appsidebar=" + c_1 + ";";
        }
    }
}

function sidebarToggle() {
    var elem = $("main");
    if (elem) {
        var ec = elem.className;
        var nextyear = new Date( );
        nextyear.setFullYear(nextyear.getFullYear() + 1);
        document.cookie = "version=" + document.lastModified + "; expires=" + nextyear.toGMTString() + ";";

        if (ec.indexOf("nosidebar") != -1) {
            removeCssClass(elem, "nosidebar");
            document.cookie = "sidebar=on;";
            return true;
        } else {
            addCssClass(elem, "nosidebar");
            document.cookie = "sidebar=off;";
            return false;
        }
    }
}
</script>