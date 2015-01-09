<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>版块管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<form action="$_form.action" method="post" name="missionLists">
<div class="Content">
    <div class="PageHeading">
        <h3>版块列表</h3>
        <div class="ActionsBar">
            <a href="$admin/bbs/manage-forum-detail.aspx?action=createforum"><span>添加版块</span></a>
        </div>
    </div>
    <div class="FormTable">
        <table class="multiColumns">
        <thead>
        <tr class="nohover">
            <th><strong>版块结构</strong> <a href="$admin/bbs/manage-forum-detail.aspx?action=createforum&parentid=0">添加版块分类</a></th>
            <th style="width:170px;"><strong>操作</strong></th>
        </tr>
        </thead>
        <tbody>
        <!--[loop $forum in $forums with $i]-->
        <tr> 
            <td>
                <div class="forumblock">
                <div style="width:{=$ForumSeparators[$i].length*50}px;"></div>
                <ul>
                <li class="forum-type"><img src="$root/max-assets/icon/forum.gif" alt="" /></li>
                <li class="forum-order">排序 <input type="text" class="text" name="sortorder_$forum.ForumID" value="$_form.text('sortorder_$forum.ForumID',$forum.SortOrder)" style="width:2em;" /></li>
                <li class="forum-title"><a href="$admin/bbs/manage-forum-detail.aspx?forumID=$forum.ForumID&action=editforum">$forum.ForumName</a> <span class="forum-id">(ID:$forum.ForumID)</span> <a href="$admin/bbs/manage-forum-detail.aspx?action=createforum&parentid=$forum.ForumID">添加子版块</a></li>
                <li class="forum-mod">版主:
                <!--[if $forum.moderators.Count==0 && $forum.NoEffectModerators.count==0]--> 
                (无)
                <!--[else]-->
                    <!--[loop $m in $forum.moderators]-->
                    <a href="$dialog/user-view.aspx?id=$m.user.userid" onclick="return openDialog(this.href)">$m.user.Name$_if($m.ModeratorType!=ModeratorType.Moderators,' ($m.name)')</a> 
                    <!--[/loop]-->
                    
                    <!--[if $forum.NoEffectModerators.Count > 0]-->
                        未上任:
                        <!--[loop $m in $forum.NoEffectModerators]-->
                        <a href="$dialog/user-view.aspx?id=$m.user.userid" onclick="return openDialog(this.href)" title="上任时间：$outputdatetime($m.begindate)">$m.user.Name$_if($m.ModeratorType!=ModeratorType.Moderators,' ($m.name)')</a> 
                        <!--[/loop]-->
                    <!--[/if]-->
                
                <!--[/if]-->
                <!--[if $CanEdit($forum) && CanManageModerator]--><a class="edit" href="$dialog/user-moderators.aspx?forumid=$forum.forumid" onclick="return openDialog(this.href,refresh)">管理</a><!--[/if]-->
                </li>
                <!--[if $forum.ParentID!=0]-->
                <li class="forum-threadcate">主题分类:
                $GetForumThreadCatalogNames($forum.ForumID," ")
                <!--[if $CanEdit($forum)]--><a class="edit" href="$dialog/threadcategories.aspx?forumID=$Forum.forumid" onclick="return openDialog(this.href, function(result){})">管理</a><!--[/if]-->
                </li>
                <!--[/if]-->
                </ul>
                </div>
            </td>
            <td>
                <!--[if $CanEdit($forum) == false ]-->
                屏蔽用户($GetBannedUserCount($forum.forumid)) |
                没有权限对该版块进行操作
                <!--[else]-->
                <a href="$admin/user/manage-shielduers.aspx?t=f&forumid=$forum.forumid">屏蔽用户($GetBannedUserCount($forum.forumid))</a> |
                <a href="$admin/bbs/manage-forum-detail.aspx?forumID=$forum.forumid&action=editforum">编辑</a> |
                <a href="$dialog/forum-delete.aspx?forumID=$forum.forumid" onclick="return openDialog(this.href, refresh)">删除</a> 
                    <!--[if $forum.ParentID!=0]-->
                    |
                    <a href="$dialog/forum-move.aspx?forumID=$forum.forumid" onclick="return openDialog(this.href, refresh)">移动</a> |
                    <a href="$dialog/forum-join.aspx?forumID=$forum.forumid" onclick="return openDialog(this.href, refresh)">合并</a>
                    <!--[/if]-->
                <!--[/if]-->
            </td>
        </tr>
        <!--[/loop]-->
        <tr class="nohover">
            <td colspan="2">
                <input class="button" name="saveforums" type="submit" value="保存更改" />
            </td>
        </tr>
        </tbody>
        </table>
    </div>

</div>
</form>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
