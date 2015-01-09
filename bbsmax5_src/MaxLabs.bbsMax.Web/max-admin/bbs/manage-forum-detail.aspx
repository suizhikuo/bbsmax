<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title><!--[if $isCreateForum]-->添加版块<!--[else]-->编辑版块<!--[/if]--></title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<form action="$_form.action" method="post">
<div class="Content">
     <div class="Columns">
        <div class="ColumnLeft">
            <div class="MenuTree">
            <h3>版块目录 (<a class="back" href="$admin/bbs/manage-forum.aspx"><span>返回版块列表</span></a>)</h3>
            <div class="MenuWrapper">
            <ul>
            <!--[if $ShowGloable]-->
            <li><a href="?action=$TreeAction&forumid=0">全局</a></li>
            <!--[/if]-->
            $GetForumsTree("<ul>{0}</ul>",@"<li><a {3} href=""?ForumID={0}&action=$TreeAction\"><span style=\"{4}\">{1}</span></a>{2}</li>")
            </ul>
            </div>
            </div>
        </div>
        <div class="ColumnRight">
            
            <!--[if $IsCreateForum==false]-->
            <div class="TabDock">
                <ul>
                <!--[if $ForumID>0]-->
                <li><a href="?action=editforum&forumid=$ForumID" $GetLinkClass($_get.action,"editforum")><span>基本信息</span></a></li>
                <!--[/if]-->
                <li><a href="?action=editsetting&forumid=$ForumID" $GetLinkClass($_get.action,"editsetting")><span>版块选项</span></a></li>
                <li><a href="?action=editusepermission&forumid=$ForumID" $GetLinkClass($_get.action,"editusepermission")><span>使用权限</span></a></li>
                <li><a href="?action=editmanagepermission&forumid=$ForumID" $GetLinkClass($_get.action,"editmanagepermission")><span>管理权限</span></a></li>
                <li><a href="?action=editpoint&forumid=$ForumID" $GetLinkClass($_get.action,"editpoint")><span>积分策略</span></a></li>
                <li><a href="?action=editrate&forumid=$ForumID" $GetLinkClass($_get.action,"editrate")><span>帖子评分</span></a></li>
                </ul>
            </div>
            <!--[else]-->
            <h3>添加版块</h3>
            <!--[/if]-->
            <!--[if $IsEditPoint]-->
                <!--[load src="../_setting-pointaction_.aspx" nodeID="$ForumID" type="ForumPointAction" params="action=$_get.action&forumID=$ForumID" /]-->
            <!--[else if $IsCreateForum || $IsEditForum]-->
                <!--[load src="_forumdetail_.aspx" ForumID="$ForumID" IsEdit="$IsEditForum" /]-->
            <!--[else if $IsEditSetting]-->
                <!--[load src="_forumsetting_.aspx" ForumID="$ForumID" params="action=$_get.action&forumID=$ForumID" /]-->
            <!--[else if $IsEditUsePermission]-->
                <!--[load src="../_setting-permissions_.aspx" nodeID="$ForumID" action="editusepermission" type="ForumPermissionSet" /]-->
            <!--[else if $IsEditManagePermission]-->
                <!--[load src="../_setting-permissions_.aspx" nodeID="$ForumID" action="editmanagepermission" type="ManageForumPermissionSet" /]-->
            <!--[else if $IsEditRate]-->
                <!--[load src="../_setting-rate_.aspx" nodeID="$ForumID" action="editrate"  params="action=$_get.action&forumID=$ForumID" /]-->
            <!--[/if]-->
        </div>
    </div>

</div>
</form>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>