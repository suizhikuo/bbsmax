<div id="top" class="topwrapper">
    <div class="clearfix topwrapper-inner">
        <!--#include file="_toolbar.aspx"-->
        
        <div class="header">
            <div class="clearfix header-inner">
                <div class="brand">
                    <h1 class="logo">
                        <a href="$url(default)" title="$BbsName" rel="home">
                            <img src="$skin/images/logo.gif" alt="$BbsName" />
                        </a>
                    </h1>
                </div>
                <!--[if $hasheaderAd]-->
                <div class="ad-banner-header">
                    $headerAd
                </div>
                <!--[/if]-->
            </div>
        </div>

        <!--[if $ParentNavigations.count > 0]-->
        <script type="text/javascript">
            //<!--[if $ChildNavigationItemCount>0]-->
            var navids = new Array();
            //<!--[loop $item in $ParentNavigations with $i]-->
            navids[$i] = $item.id;
            //<!--[/loop]-->
            //<!--[/if]-->
            function selectNavigation(id) {
                //<!--[if $ChildNavigationItemCount>0]-->
                if ($('navigation_' + id) == null)
                    return;
                for (var i = 0; i < navids.length; i++) {
                    var nav = $('navigation_' + navids[i]);
                    if (nav) {
                        if (id == navids[i]) {
                            nav.style.display = '';

                            $('child_navigations').style.display = '';
                        }
                        else
                            nav.style.display = 'none';
                    }
                }
                //<!--[/if]-->
            }
        </script>
        <div class="nav">
            <div class="clearfix nav-inner">
                <div class="majornav-wrap">
                    <div class="majornav">
                        <div class="clearfix majornav-inner">
                            <ul class="clearfix majornav-list">
                                <!--[loop $item in $ParentNavigations]-->
                                <li><a class="$_if($ParentSelectedNavigatonItemID == $item.id,'current','')" href="$item.url" $_if($item.NewWindow,'target="_blank"','')><span>$item.name</span></a></li>
                                <!--[/loop]-->
                            </ul>
                            <div class="myapptrigger" onmouseover="addCssClass(this,'myapptrigger-expand')" onmouseout="removeCssClass(this,'myapptrigger-expand')">
                                <a class="myapptrigger-button" href="$url(my/default)" id="appdock_dropdown"><span><strong>互动中心</strong></span></a>
                                <div class="dropdownmenu-wrap menu-appdock" id="menu_appdock">
                                    <div class="dropdownmenu">
                                        <div class="clearfix dropdownmenu-inner">
                                            <!--[if $islogin]-->
                                            <ul class="clearfix applist myaccesslist">
                                                <li><a href="$url(space/$my.userid)" title="我的空间"><img src="$root/max-assets/icon32/home.gif" alt="" /><span>我的空间</span></a></li>
                                                <!--[if $EnableMissionFunction]-->
                                                    <!--[if $My.TotalMissions > 0]-->
                                                <li><a href="$url(mission/current)" title="任务"><img src="$root/max-assets/icon32/event.gif" alt="" /><span>任务</span><span class="counts">$My.TotalMissions</span></a></li>
                                                    <!--[else]-->
                                                <li><a href="$url(mission/index)" title="任务"><img src="$root/max-assets/icon32/event.gif" alt="" /><span>任务</span></a></li>
                                                    <!--[/if]-->
                                                <!--[/if]-->
                                                <!--[if $EnablePropFunction]-->
                                                <li><a href="$url(prop/index)" title="商店"><img src="$root/max-assets/icon32/store.gif" alt="" /><span>商店</span></a></li>
                                                <li><a href="$url(prop/my)" title="我的物品"><img src="$root/max-assets/icon32/shop.gif" alt="" /><span>我的物品</span></a></li>
                                                <!--[/if]-->
                                            </ul>
                                            <ul class="applist">
                                                <li><a $_if($PageName=='mythreads','class="current"') href="$url(my/mythreads)" title="主题"><img src="$root/max-assets/icon32/topic.gif" alt="" /><span>主题</span></a></li>
                                                <!--[if EnableDoingFunction]-->
                                                <li><a $_if($PageName=='doing','class="current"') href="$url(app/doing/index)?view=friend" title="记录"><img src="$root/max-assets/icon32/doing.gif" alt="" /><span>记录</span></a></li>
                                                <!--[/if]-->
                                                <!--[if EnableBlogFunction]-->
                                                <li><a $_if($PageName=='blog','class="current"') href="$url(app/blog/index)?view=friend" title="日志"><img src="$root/max-assets/icon32/blog.gif" alt="" /><span>日志</span></a></li>
                                                <!--[/if]-->
                                                <!--[if EnableAlbumFunction]-->
                                                <li><a $_if($PageName=='album','class="current"') href="$url(app/album/index)?view=friend" title="相册"><img src="$root/max-assets/icon32/album.gif" alt="" /><span>相册</span></a></li>
                                                <!--[/if]-->
                                                <!--[if EnableShareFunction]-->
                                                <li><a $_if($PageName=='share','class="current"') href="$url(app/share/index)?view=friend" title="分享"><img src="$root/max-assets/icon32/share.gif" alt="" /><span>分享</span></a></li>
                                                <!--[/if]-->
                                                <!--[if EnableFavoriteFunction]-->
                                                <li><a $_if($PageName=='favorite','class="current"') href="$url(app/share/index)?mode=fav" title="收藏"><img src="$root/max-assets/icon32/fav.gif" alt="" /><span>收藏</span></a></li>
                                                <!--[/if]-->
                                                <!--[if $EnableEmoticonFunction]-->
                                                <li><a $_if($PageName=='emoticon','class="current"') href="$url(app/emoticon/index)" title="表情"><img src="$root/max-assets/icon32/emoticon.gif" alt="" /><span>表情</span></a></li>
                                                <!--[/if]-->
                                                <!--[if $EnableNetDiskFunction]-->
                                                <li><a $_if($PageName=='disk','class="current"') href="$url(app/disk/index)" title="网络硬盘"><img src="$root/max-assets/icon32/folder_web.gif" alt="" /><span>网络硬盘</span></a></li>
                                                <!--[/if]-->
                                            </ul>
                                            <!--[else]-->
                                            <div class="needlogin">你还没有登录.</div>
                                            <!--[/if]-->
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="extranav">
                    <!--[if $ChildNavigationItemCount>0]-->
                    <div class="minornav-wrap" id="child_navigations" style="display:none;">
                        <div class="minornav">
                            <div class="minornav-inner">
                    <!--[loop $item in $ParentNavigations with $i]-->
                            <!--[if $item.ChildItems.count>0]-->
                                <input type="hidden" name="childnavItems" value="$item.id" />
                                <ul class="clearfix minornav-list" id="navigation_$item.id" style="display:none">
                                    <!--[loop $tempItem in $item.ChildItems]-->
                                    <!--[if $tempItem.OnlyLoginCanSee == false || $islogin]-->
                                    <li><a class="$_if($CurrentNavigatonItemID == $tempItem.id,'current','')" href="$tempItem.url" $_if($tempItem.NewWindow,'target="_blank"','')><span>$tempItem.name</span></a></li>
                                    <!--[/if]-->
                                    <!--[/loop]-->
                                </ul>
                            <!--[/if]-->
                    <!--[/loop]-->
                            </div>
                        </div>
                    </div>
                    <!--[/if]-->
                    <div class="subnav-wrap">
                        <div class="subnav">
                            <div class="clearfix subnav-inner">
                                <div class="crumbnav">
                                    <a class="crumbnav-label" href="javascript:void(0)" id="forumboard_menu" title="页面导航"><span>页面导航:</span></a>
                                    $Navigation
                                </div>
                                <!--[if $PageName=="default"]-->
                                <div class="forumstatus">
                                    今日: <em class="numeric">$Sys.todayPosts</em> ,
                                    昨日: <em class="numeric">$Sys.YestodayPosts</em> ,
                                    主题: <em class="numeric"><a href="$url(new)">$Sys.totalThreads</a></em> ,
                                    帖子: <em class="numeric">$Sys.totalPosts</em> ,
                                    用户: <em class="numeric"><a href="$url(members)?view=search">$Sys.totalUsers</a></em>
                                    <a class="rss" href="$root/rss.aspx?ticket=_ticket">订阅</a>
                                </div>
                                <!--[/if]-->
                            </div>
                        </div>
                    </div>
                </div>
                
                <!--#include file="_round_bottom.aspx"-->
            </div>
            <!--[if $ChildNavigationItemCount>0]-->
            <script type="text/javascript">
                selectNavigation($ParentSelectedNavigatonItemID);
            </script>
            <!--[/if]-->
        </div>
        <!--[/if]-->

    </div>
</div>

<!--[if $hasTopBannerAD]-->
<div class="ad-banner-forumheader">
    $TopBannerAD
</div>
<!--[/if]-->

<!--[if $hasPageWordAD]-->
<!--#include file="_ad_text.aspx"-->
<!--[/if]-->
