<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>重新统计数据</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<form action="$_form.action" method="post" name="missionLists">
<div class="Content">
    <h3>重新统计数据</h3>
    <div class="Help">
    重新统计用户数据
    </div>
    <div class="FormTable">
	    <table class="multiColumns">
	    <tbody>
	    <tr> 
	        <td>
	            <input type="checkbox" id="posts" name="userdatas" value="1" /><label for="posts">帖子数</label>
	            <input type="checkbox" id="blogs" name="userdatas" value="2" /><label for="blogs">日志数</label>
	            <input type="checkbox" id="comments" name="userdatas" value="3" /><label for="comments">评论数</label>
	            <input type="checkbox" id="photos" name="userdatas" value="4" /><label for="photos">相册和图片数</label>
	            <input type="checkbox" id="shares" name="userdatas" value="5" /><label for="shares">收藏和分享数</label>
	            <input type="checkbox" id="doings" name="userdatas" value="6" /><label for="doings">记录数</label>
	            <input type="checkbox" id="diskfiles" name="userdatas" value="7" /><label for="diskfiles">网络硬盘文件数</label>
	            <input type="checkbox" id="invites" name="userdatas" value="8" /><label for="invites">成功邀请用户数</label>
	        </td>
	    </tr>
	    <tr class="nohover">
	    <td>
        <input type="checkbox" id="selectAll1" />
        <label for="selectAll1">全选</label>
        <script type="text/javascript">
            new checkboxList( 'userdatas', 'selectAll1');
        </script>
	    <input class="button" name="updateuserdata" type="submit" value="更新" />
        </td>
        </tr>
	    </tbody>
	    </table>
	</div>
	
	 <div class="Help">
    重新统计今日帖子数，昨日帖子数，会员数
    </div>
	<div class="FormTable">
	    <table class="multiColumns">
	    <tbody>
	    <tr> 
	        <td>
	            <input type="checkbox" id="todayposts" name="vardatas" value="1" /><label for="todayposts">今日帖子数</label>
	            <input type="checkbox" id="yestodayposts" name="vardatas" value="2" /><label for="yestodayposts">昨日帖子数</label>
	            <input type="checkbox" id="usercount" name="vardatas" value="3" /><label for="usercount">会员数</label>
	        </td>
	    </tr>
	    <tr class="nohover">
	        <td>
	        <input type="checkbox" id="selectAll2" />
            <label for="selectAll2">全选</label>
            <script type="text/javascript">
                new checkboxList( 'vardatas', 'selectAll2');
            </script>
                <input class="button" name="updatevarsdata" type="submit" value="更新" />
	        </td>
	    </tr>
	    </tbody>
	    </table>
	</div>
	
    <div class="Help">
    重新统计主题数、帖子数、主题分类中的主题数
    </div>
	<div class="FormTable">
	    <table class="multiColumns">
	    <tbody>
	    <!--[loop $forum in $forums with $i]-->
	    <tr> 
	        <td>
	            <div class="forumblock forumblock-noorder">
	            <div style="width:{=$ForumSeparators[$i].length*50}px;"></div>
	            <ul>
                <li class="forum-type"><img src="$root/max-assets/icon/forum.gif" alt="" /></li>
                <li class="forum-title"><input type="checkbox" id="forum_$forum.ForumID" name="forumIDs" value="$forum.ForumID" /> <label for="forum_$forum.ForumID">$forum.ForumName</label>  <span class="forum-id">(主题:$forum.TodayThreads/$forum.TotalThreads,帖子$forum.TodayPosts/$forum.TotalPosts)</span></li>
                </ul>
	            </div>
	        </td>
	    </tr>
	    <!--[/loop]-->
	    <tr class="nohover">
	        <td>
	            <input type="checkbox" id="selectAll" />
	            <label for="selectAll">全选</label>
	            <script type="text/javascript">
                    new checkboxList( 'forumIDs', 'selectAll');
                </script>
                <input class="button" name="updatedata" type="submit" value="更新" />
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
