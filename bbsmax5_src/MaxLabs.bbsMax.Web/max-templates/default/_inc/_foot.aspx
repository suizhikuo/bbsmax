<!--#include file="_ad_foot.aspx"-->

<div class="footer">
    <div class="clearfix footer-inner">
        <div class="copyright">
            <p>Powered by <a href="http://www.bbsmax.com/" rel="bookmark" target="_blank"><strong>$Version</strong></a> 2002-2010 MaxLabs.</p>
            <p>Processed in $ProcessTime seconds, $QueryTimes Queries 
            <!--[if EnablePassportClient]-->
            & Passport Server $RemoteCallCount Queries
            <!--[/if]-->
            . GMT $_if($My.TimeZone>0,'+')$My.TimeZone, $usernow </p>
        </div>
        <div class="extralink">
            <a href="$url(archiver/default)">简洁版</a>
            <!--[if $ForumIcp!=""]--><a rel="nofollow" href="http://www.miibeian.gov.cn/">$ForumIcp</a><!--[/if]-->
            <span class="skinswitcher">
                <select class="skinlist" name="theme" onchange="if(this.value!=''){location.replace('$url(handler/changeskin)?skin='+this.value+'&u='+encodeURIComponent(location.href));}">
                    <option value="">界面风格</option>
                    <!--[loop $TheSkin in $TheSkinList]-->
                    <option <!--[if $theskin.SkinID == $CurrentSkinID]-->selected="selected" <!--[/if]--> value="$theskin.SkinID">$theskin.Name</option>
                    <!--[/loop]-->
                </select>
            </span>
        </div>
    </div>
</div>

<!--/* 网站导航 */-->
<div class="dropdownmenu-wrap menu-sitemap" id="menu_sitemap" style="display:none;">
    <div class="dropdownmenu">
        <div class="clearfix dropdownmenu-inner">
            <div class="sitemap-group">
                <!--[loop $forumCatalog in $ForumCatalogs]-->
                <h3 class="sitemap-title"><span>$forumCatalog.ForumName</span></h3>
                <ul class="clearfix sitemap-list">
                    <!--[loop $navForum in $forumCatalog.SubForumsForList]-->
                    <li><a href="$url($navForum.CodeName/list-1)">$navForum.ForumName</a></li>
                    <!--[/loop]-->
                </ul>
                <!--[/loop]-->
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
    new popup("menu_sitemap", ["forumboard_menu", "max_nav_root_0", "max_nav_root_1"], true);
</script>

<!--[if $StatCode != ""]-->
<div class="siteanalytics">$StatCode</div>
<!--[/if]-->
<script type="text/javascript">
    page_end();
    addHandler(document.getElementsByTagName("html")[0], "click", function (e) { window.isClickDialog = 0;});
</script>
<!--
{=HttpContext.Current.Items["max_update_time"]}
============
{=HttpContext.Current.Items["max_runinfo"]}
-->
