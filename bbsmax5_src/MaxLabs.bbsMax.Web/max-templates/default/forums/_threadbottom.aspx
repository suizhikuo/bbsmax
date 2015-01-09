<!--此文件为帖子附件信息脚本以及模板等，包括左侧用户扩展信息， 图片浏览器等-->
<script type="text/javascript" src="$root/max-assets/javascript/max-viewthread.js"></script>
<!--左侧用户信息扩展-->
<div id="authorExtraTempalte" style="display:none">
    <div class="user-impression">
    <!--{impressions}-->
    <span class="imp-{_i}"><a href="$dialog/user-impressions.aspx?uid={userid}" onclick="openDialog(this.href);return false;">{text}</a></span> 
    <!--{/impressions}-->
    </div>
    <div class="user-activity">
        <div class="activity-blog">
        <!--{newarticle}-->
        <span class="title"><a href="{url}" target="_blank">{subject}</a></span><span class="summary">{content}</span>
        <!--{/newarticle}-->
        </div>
        <div class="clearfix activity-album">
        <!--{newphotos}--><a href="{url}" target="_blank"><img src="{img}" alt="" onload="setStyle(this,{height:'auto',width:'auto'}); imageScale(this,50,50);" style="height:50px;width:50px" title="{subject}"/>&nbsp;</a><!--{/newphotos}-->
        </div>
    </div>
</div>

<!--[if $IsShowModeratorManageLink]-->
<div class="dropdownmenu-wrap moderatordropdown" id="moderatorMenu" style="display:none;">
    <div class="dropdownmenu">
        <div class="dropdownmenu-inner">
            <div class="clearfix moderator-head">
                <h3 class="moderator-title">
                    选中<strong class="count" id="selectCount">1</strong>篇
                </h3>
                <p class="moderator-action"><a href="javascript:;" onclick="switchMenu(0);return false;">缩小</a></p>
            </div>
            <div class="clearfix moderator-body">
                <div class="moderator-operate">
                    <ul class="clearfix moderator-operate-list">
                        <li><a href="javascript:postTo('$dialog/forum/deletepost.aspx?codename=$Codename');">删除选中回复</a></li>
                        <li><a href="javascript:postTo('$dialog/forum/shieldpost.aspx?codename=$Codename&shield=true');">屏蔽选中回复</a></li>
                        <li><a href="javascript:postTo('$dialog/forum/shieldpost.aspx?codename=$Codename&shield=false');">解除屏蔽选中回复</a></li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="dropdownmenu-wrap moderatordropdown moderatordropdown-mini" id="mMenuMini" style="display:none;" onclick="switchMenu(1)">
    <div class="dropdownmenu">
        <div class="dropdownmenu-inner">
            <strong class="count" id="selectCount_mini">0</strong>
        </div>
    </div>
</div>
<script type="text/javascript">
    function postTo(url) {
        postToDialog({ formId: "threadform", url: url, callback: refresh });
    }

    function openPage(action) {
        var u = "$dialog/forum/" + action + ".aspx?codename=$Forum.CodeName";
        return openDialog(u, refresh);
    }

    var mMenuTop = 0;
    var mmode = 1;
    var switchMenu = function (s) {
        var right = s ? getRect(mMenuMini).right : getRect(moderatorMenu).right;
        mMenuMini.style.display = s ? 'none' : '';
        moderatorMenu.style.display = s ? '' : 'none';

        obj = s ? moderatorMenu : mMenuMini;
        obj.style.left = (right - obj.offsetWidth-3) + "px";

        mmode = s;
    }
    var lChange = function (e) {
        var s0 = moderatorMenu.style;
        var s1 = mMenuMini.style;
        var c = threadList.selectCount();
        if (c == 0) {
            s0.display = "none";
            s1.display = "none";
            mMenuTop = 0;
        }
        else {

            var eRect = getRect(e);
            switchMenu(mmode);
            s0.left = (eRect.right - moderatorMenu.offsetWidth) + "px";
            s1.left = (eRect.right - mMenuMini.offsetWidth) + "px";
            if (e) {
                var top = eRect.bottom;
                s0.top = top + "px";
                s1.top = top + "px";
                var bRect = max.global.getBrowserRect();
                mMenuTop = top - bRect.top;
            }
        }

        var count = c.toString();
        $("selectCount_mini").innerHTML = count;
        $("selectCount").innerHTML = count;
    }
    var moderatorMenu = $("moderatorMenu");
    var mMenuMini = $("mMenuMini");
    function createModeratorMenu() {
        window.threadList = new checkboxList("postIDs");
        threadList.SetItemChangeHandler(lChange);
    }

    var pageScroll = function () {

        if (mMenuTop) { 
       
            var brect = max.global.getBrowserRect();
            
            mMenuMini.style.top = (brect.top + mMenuTop) +"px";
            moderatorMenu.style.top = (brect.top + mMenuTop)+"px";
        }
    }

//    if (window.all) {
        addHandler(window, "scroll", pageScroll);
//    }
//    else {
//        document.body.addEventListener('scroll', pageScroll, true);
    //    }

    //window.onscroll = pageScroll;
        

    createModeratorMenu();
</script>

<div id="action-manage-list" class="dropdownmenu-wrap managetoipc-dropdownmenu" style="display:none;">
    <div class="dropdownmenu">
        <div class="dropdownmenu-inner">
            <input type="hidden" name="ThreadID" value="$thread.ThreadID" />
            <h3 class="managetoipc-title">主题管理</h3>
            <ul class="clearfix managetoipc-list">
                $GetModeratorActionLinks(@"<li><a href=""javascript:postTo('{1}');"">{0}</a></li>","")
                <li><a href="$dialog/forum/recountposts.aspx?threadid=$thread.ThreadID" onclick="return openDialog(this.href,refresh);">修复回复数</a></li>
            </ul>
            <h3 class="managetoipc-title">回复管理</h3>
            <ul class="clearfix managetoipc-list">
                $GetPostActionLink(@"<li><a href=""javascript:postTo('{1}');"">{0}</a></li>","")
            </ul>
            <h3 class="managetoipc-title">其他操作</h3>
            <ul class="clearfix managetoipc-list">
                <li><a href="javascript:void(0);" onclick="openPage('UpdateForumReadme')">修改本版规则</a></li>
                <li><a href="$url($forum.CodeName/unapproved-1)">本版未审核主题</a></li>
                <li><a href="$url($forum.CodeName/unapprovedpost-1)">本版未审核回复</a></li>
                <li><a href="$url($forum.CodeName/recycled-1)">本版回收站</a></li>
                <li><a href="$dialog/forumshielduers.aspx?forumid=$Forum.ForumID" onclick="return openDialog(this.href);">本版屏蔽用户列表</a></li>
            </ul>
        </div>
    </div>
</div>
<!--[/if]-->
<script type="text/javascript">
var authorUrl = "$url(Handler/authorinfo)?userid={0}&r={1}";

function _pageLoaded() {
    var f = window.threadImageCollection ? 1 : 0;
    var f2 = window._IV ? 1 : 0;
    if (f) {
        var imgs = document.images;
        for (var j = 0; j < imgs.length; j++) {
            var im = imgs[j];
            if (im.processed) continue;
            if (im.width < 300 || im.height < 300) continue;
            var w = 0;
            
            var n = im.parentNode;
            var rt2 = getRect(n);
            w = rt2.width * .90;

            if (im.width > w) {
                imageScale(im, w, 0);
            }

            threadImageCollection.push({ src: im.src, width: im.width, height: im.height, object: im });
            addHandler(im, "click", showImage);
            if (f2) _IV.appendImageItem(im.src);
        }
    }
}
addHandler(window, "onload", _pageLoaded);

PostViewStyle.current_css = ''; 
function PostViewStyle(th, css) {
    var current_css = PostViewStyle.current_css; while (th) {
        if (th.tagName.toLowerCase() == 'table') {
            th = th.getElementsByTagName('div'); for (var i = 0, len = th.length; i < len; i++) { if (th[i].className.indexOf('post-content') > -1 || th[i].className.indexOf('post-attach') > -1) { removeCssClass(th[i], current_css); addCssClass(th[i], css); PostViewStyle.current_css = css; break; } }
            break;
        }
        th = th.parentNode;
    }
}
var imgs = document.images;

function pushToCollection(img) {
    var loadded = img.processed == 1;
    threadImageCollection.push({ src: img.src, width: img.width, height: img.height, object: img, load: loadded });
}
if (!window.threadImageCollection) window.threadImageCollection = [];

if (window.threadImageCollection) {
    for (var i = 0; i < imgs.length-1; i++) {
        if (imgs[i].onload) pushToCollection(imgs[i]);
    }
}
 </script>
