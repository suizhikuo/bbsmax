<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>未审核的回复</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
function deleteTopic(id)
{
    removeElement($('item_'+id));
}
</script>
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $IsOwner == false]-->
<div class="Tip Tip-permission">由于权限限制, 你可能无法管理部分用户组的帖子数据. 此处不会列出这些数据.红色的版块是您没有权限管理的版块;只有创始人才能选择全部版块</div>
<!--[/if]-->
<div class="Content">
    <h3>未审核的回复</h3>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
    <table>
    <tr>
	    <td><label for="forum">版块</label></td>
	    <td colspan="3">
	    <select name="ForumID">
		    <option value="" <!--[if $IsOwner == false]--> style="color:Red"<!--[/if]-->>所有版块</option>
		    <!--[loop $tempForum in $Forums with $i]-->
		    <option value="$tempForum.ForumID" <!--[if $HasPermission($tempForum) == false]--> style="color:Red"<!--[/if]-->  $_form.selected("ForumID","$tempForum.ForumID","$out($PostForm.ForumID)")>&nbsp;&nbsp;&nbsp;&nbsp;$ForumSeparators[$i]|--$tempForum.ForumName</option>
		    <!--[/loop]-->
		</select>
	    </td>
	</tr>
	<tr>
	    <td><label for="username">用户名</label></td>
	    <td><input class="text" id="username" name="username" type="text" value="$PostForm.Username" /></td>
	    <td><label for="UserID">作者ID</label></td>
	    <td><input class="text" id="UserID" name="UserID" type="text" value="$PostForm.UserID" /></td>
	</tr>
	<tr>
	    <td>搜索方式</td>
	    <td>
	        <input id="subject" type="radio" value="Subject" name="searchmode"  $_form.checked("searchmode","Subject",$PostForm.SearchMode.ToString()) /><label for="subject">标题</label>
            <input id="fulltext" type="radio" value="FullText" name="searchmode" $_form.checked("searchmode","FullText",$PostForm.SearchMode.ToString()) /><label for="fulltext">全文</label>
            <input id="subjectorfulltext" type="radio" value="Default" name="searchmode" $_form.checked("searchmode","Default",$PostForm.SearchMode.ToString()) /><label for="subjectorfulltext">标题或全文</label>
	    </td>
	    <td><label for="keyword">关键字</label></td>
	    <td><input class="text" id="keyword" name="keyword" type="text" value="$PostForm.keyword" /></td>
	</tr>
	<tr>
	    <td><label for="topicid">指定帖子ID</label></td>
	    <td colspan="3"><input class="text" id="postid" name="postID" type="text" value="$PostForm.postid" /></td>
	</tr>
	<tr>
	    <td><label for="createip">作者IP</label></td>
	    <td><input class="text" id="createip" name="createip" type="text" value="$PostForm.CreateIP" /></td>
	</tr>
	<tr>
	    <td>发表时间</td>
	    <td colspan="3">
	        <input name="begindate" id="begindate" class="text" style="width:6em;" type="text" value="$PostForm.BeginDate" />
	        <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
	        ~
	        <input name="enddate" id="enddate" class="text" style="width:6em;" type="text" value="$PostForm.EndDate" /> 
	        <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
	        <span class="desc">(时间格式: YYYY-MM-DD)</span>
	    </td>
	</tr>
	<tr>
	    <td>结果排序</td>
	    <td>
            <select name="order">
                <option value="">发表时间</option>
            </select>
            <select name="isdesc">
                <option value="true" $_form.selected("isdesc","true",$PostForm.IsDesc.ToString())>按降序排列</option>
                <option value="false" $_form.selected("isdesc","false",$PostForm.IsDesc.ToString())>按升序排列</option>
            </select>
        </td>
        <td>每页显示数</td>
        <td>
            <select name="pagesize">
                <option value="10" $_form.selected("pagesize","10",$PostForm.PageSize)>10</option>
                <option value="20" $_form.selected("pagesize","20",$PostForm.PageSize)>20</option>
                <option value="50" $_form.selected("pagesize","50",$PostForm.PageSize)>50</option>
                <option value="100" $_form.selected("pagesize","100",$PostForm.PageSize)>100</option>
                <option value="200" $_form.selected("pagesize","200",$PostForm.PageSize)>200</option>
                <option value="500" $_form.selected("pagesize","500",$PostForm.PageSize)>500</option>
            </select>
	    </td>
	</tr>
	<tr>
	    <td>&nbsp;</td>
	    <td colspan="3">
            <input type="submit" name="search" class="button" value="搜索" />
	    </td>
	</tr>
	</table>
	</form>
	</div>

    <form action="$_form.action" method="post" name="topiclistForm" id="topiclistForm">
        <div class="DataTable">
        <h4> <span class="counts">总数: $TotalCount</span></h4>
        <!--[if $TotalCount > 0]-->
        <table>
            <thead>
                <tr>
                    <th class="CheckBoxHold">&nbsp;</th>
                    <th>内容</th>
                    <th style="width:100px;">所属版块</th>
                    <th style="width:100px;">作者</th>
                    <th style="width:100px;">发布时间</th>
                    <th style="width:50px;">操作</th>
                </tr>
            </thead>
            <tbody>
        <!--[/if]-->
        <!--[loop $post in $PostList]-->
                    <tr id="item_$post.ID">
                        <td><input name="postids" type="checkbox" value="$post.PostID" /></td>
                        <td>
                        <div>所在主题:<a href="$GetThreadUrl($post.ThreadID)" target="_blank">$GetThreadSubject($post.ThreadID)</a></div>
                        <div><a href="$GetThreadUrl($post.ThreadID)" target="_blank">$GetPostContent($Post)</a></div>
                        </td>
                        <td>$post.Forum.ForumName</td>
                        <td><a href="$url(space/$post.UserID)" onclick="return openUserMenu(this,$post.UserID,'post')" target="_blank">$post.Username</a></td>
                        <td>$Post.CreateDate</td>
                        <td>
                        <a href="$dialog/forum/approvepost.aspx?codename=$post.forum.codename&threadids=$post.ThreadID&postids=$post.PostID" onclick="return openDialog(this.href,this,refresh);">审核通过</a>
                        <a href="$dialog/forum/deletepost.aspx?codename=$post.forum.codename&threadids=$post.ThreadID&postids=$post.PostID" onclick="return openDialog(this.href,this,refresh);">删除</a>
                        </td>
                    </tr>
        <!--[/loop]-->
    <!--[if $TotalCount > 0]-->
            </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" id="selectAll" /> <label for="selectAll">全选</label>
            <input type="submit" class="button" name="approvedchecked" onclick="if(confirm('确认审核通过选中的数据吗?')==false)return false;" value="审核通过选中" />
            <input type="submit" class="button" name="approvedsearched" onclick="if(confirm('确认审核通过搜索到的数据吗?')==false)return false;" value="审核通过搜索到的数据" />
            <input type="submit" class="button" name="deletechecked" onclick="if(confirm('确认删除选中的数据吗?')==false)return false;" value="删除选中" />
            <input type="submit" class="button" name="deletesearched" onclick="if(confirm('确认删除搜索到的数据吗?')==false)return false;" value="删除搜索到的数据" />
            <input value="1" id="updatePoint_2" name="updatePoint" checked="checked" type="checkbox" />
            <label for="updatePoint_2">删除时更新用户积分</label>
        </div>
        <script type="text/javascript">
            new checkboxList( 'postids', 'selectAll');
        </script>
        <!--[pager name="list" skin="../_pager.aspx"]-->
    <!--[else]-->
        <div class="NoData">未搜索到数据.</div>
    <!--[/if]-->
        </div>
    </form>
</div>
<script type="text/javascript">

    initDatePicker('begindate','A0');
    initDatePicker('enddate','A1');
</script>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
