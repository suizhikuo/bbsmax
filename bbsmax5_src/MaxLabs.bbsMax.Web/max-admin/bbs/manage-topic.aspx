<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title><!--[if $IsRecycleBin]-->回收站<!--[else]-->帖子管理<!--[/if]--></title>
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
    <h3><!--[if $IsRecycleBin]-->回收站<!--[else]-->帖子管理<!--[/if]--></h3>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
    <table>
    <tr>
	    <td><label for="forum">版块</label></td>
	    <td colspan="3">
	    <select name="ForumID">
		    <option value="" <!--[if $IsOwner == false]--> style="color:Red"<!--[/if]-->>所有版块</option>
		    <!--[loop $tempForum in $Forums with $i]-->
		    <option value="$tempForum.ForumID" <!--[if $HasPermission($tempForum) == false]--> style="color:Red"<!--[/if]-->  $_form.selected("ForumID","$tempForum.ForumID","$out($TopicForm.ForumID)")>&nbsp;&nbsp;&nbsp;&nbsp;$ForumSeparators[$i]|--$tempForum.ForumName</option>
		    <!--[/loop]-->
		</select>
	    </td>
	</tr>
	<tr>
	    <td><label for="username">用户名</label></td>
	    <td><input class="text" id="username" name="username" type="text" value="$TopicForm.Username" /></td>
	    <td><label for="UserID">作者ID</label></td>
	    <td><input class="text" id="UserID" name="UserID" type="text" value="$TopicForm.UserID" /></td>
	</tr>
	<tr>
	    <td>搜索方式</td>
	    <td>
	        <input id="subject" type="radio" value="Subject" name="searchmode"  $_form.checked("searchmode","Subject",$TopicForm.SearchMode.ToString()) /><label for="subject">标题</label>
            <input id="fulltext" type="radio" value="FullText" name="searchmode" $_form.checked("searchmode","FullText",$TopicForm.SearchMode.ToString()) /><label for="fulltext">全文</label>
            <input id="subjectorfulltext" type="radio" value="Default" name="searchmode" $_form.checked("searchmode","Default",$TopicForm.SearchMode.ToString()) /><label for="subjectorfulltext">标题或全文</label>
	    </td>
	    <td><label for="keyword">关键字</label></td>
	    <td><input class="text" id="keyword" name="keyword" type="text" value="$TopicForm.keyword" /></td>
	</tr>
	<tr>
	    <td><label for="topicid">指定主题ID</label></td>
	    <td colspan="3"><input class="text" id="topicid" name="topicid" type="text" value="$TopicForm.topicid" /></td>
	</tr>
	<tr>
	    <td><label for="createip">作者IP</label></td>
	    <td><input class="text" id="createip" name="createip" type="text" value="$TopicForm.CreateIP" /></td>
	</tr>
	<tr>
	    <td><label>查看数</label></td>
	    <td><input name="MinViewCount" class="text" style="width:6em;" type="text" value="$TopicForm.MinViewCount" /> ~ <input name="MaxViewCount" class="text" style="width:6em;" type="text" value="$TopicForm.MaxViewCount" /></td>
	    <td><label>回复数</label></td>
	    <td><input name="MinReplyCount" class="text" style="width:6em;" type="text" value="$TopicForm.MinReplyCount" /> ~ <input name="MaxReplyCount" class="text" style="width:6em;" type="text" value="$TopicForm.MaxReplyCount" /></td>
	</tr>
	
	<tr>
	    <td><label>包括精华</label></td>
	    <td>
	    <input id="includeValued" type="radio" value="true" name="includeValued"  $_form.checked("includeValued","true",$TopicForm.IncludeValued.ToString()) /><label for="includeValued">是</label>
	    <input id="excludeValued" type="radio" value="false" name="includeValued"  $_form.checked("includeValued","false",$TopicForm.IncludeValued.ToString()) /><label for="excludeValued">否</label>
	    </td>
	    <td><label>包括置顶</label></td>
	    <td>
	    <input id="includeStick" type="radio" value="true" name="includeStick"  $_form.checked("includeStick","true",$TopicForm.IncludeStick.ToString()) /><label for="includeStick">是</label>
	    <input id="excludeStick" type="radio" value="false" name="includeStick"  $_form.checked("includeStick","false",$TopicForm.IncludeStick.ToString()) /><label for="excludeStick">否</label></td>
	</tr>
	<tr>
	    <td>发表时间</td>
	    <td colspan="3">
	        <input name="begindate" id="begindate" class="text" style="width:6em;" type="text" value="$TopicForm.BeginDate" />
	        <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
	        ~
	        <input name="enddate" id="enddate" class="text" style="width:6em;" type="text" value="$TopicForm.EndDate" /> 
	        <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
	        <span class="desc">(时间格式: YYYY-MM-DD)</span>
	    </td>
	</tr>
	<tr>
	    <td>结果排序</td>
	    <td>
            <select name="order">
                <option value="TopicID" $_form.selected("order","TopicID",$TopicForm.Order)>发表时间</option>
                <option value="LastReplyDate" $_form.selected("order","LastReplyDate",$TopicForm.Order)>最后回复时间</option>
                <option value="ReplyCount" $_form.selected("order","ReplyCount",$TopicForm.Order)>回复数量</option>
                <option value="ViewCount" $_form.selected("order","ViewCount",$TopicForm.Order)>浏览次数</option>
            </select>
            <select name="isdesc">
                <option value="true" $_form.selected("isdesc","true",$TopicForm.IsDesc.ToString())>按降序排列</option>
                <option value="false" $_form.selected("isdesc","false",$TopicForm.IsDesc.ToString())>按升序排列</option>
            </select>
        </td>
        <td>每页显示数</td>
        <td>
            <select name="pagesize">
                <option value="10" $_form.selected("pagesize","10",$TopicForm.PageSize)>10</option>
                <option value="20" $_form.selected("pagesize","20",$TopicForm.PageSize)>20</option>
                <option value="50" $_form.selected("pagesize","50",$TopicForm.PageSize)>50</option>
                <option value="100" $_form.selected("pagesize","100",$TopicForm.PageSize)>100</option>
                <option value="200" $_form.selected("pagesize","200",$TopicForm.PageSize)>200</option>
                <option value="500" $_form.selected("pagesize","500",$TopicForm.PageSize)>500</option>
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
        <h4>主题 <span class="counts">总数: $TotalCount</span></h4>
        <!--[if $TotalCount > 0]-->
        <table>
            <thead>
                <tr>
                    <th class="CheckBoxHold">&nbsp;</th>
                    <th>标题</th>
                    <th style="width:100px;">所属版块</th>
                    <th style="width:100px;">作者</th>
                    <th style="width:100px;">回复/查看</th>
                    <th style="width:100px;">发布时间</th>
                    <th style="width:100px;">最后更新</th>
                    <th style="width:50px;">操作</th>
                </tr>
            </thead>
            <tbody>
        <!--[/if]-->
    <!--[loop $thread in $ThreadList]-->
                <tr id="item_$thread.ID">
                    <td><input name="topicids" type="checkbox" value="$thread.ThreadID" /></td>
                    <td><a href="$GetThreadUrl($thread)" target="_blank">$thread.SubjectText</a></td>
                    <td>$thread.Forum.ForumName</td>
                    <td><a href="$url(space/$thread.PostUserID)" onclick="return openUserMenu(this,$thread.PostUserID,'post')" target="_blank">$thread.PostUsername</a></td>
                    <td>$thread.TotalReplies/$thread.TotalViews</td>
                    <td>$thread.CreateDate</td>
                    <td><a href="$url(space/$thread.LastReplyUserID)"  onclick="return openUserMenu(this,$thread.LastReplyUserID,'post')" target="_blank">$thread.LastReplyUsername</a>($thread.UpdateDate)</td>
                    <td>
                    <!--[if $IsRecycleBin]-->
                    <a href="$dialog/forum/revertthread.aspx?codename=$thread.forum.codename&threadids=$thread.ThreadID" onclick="return openDialog(this.href,this,refresh);">还原主题</a>
                    <!--[/if]-->
                    <a href="$dialog/forum/deletethread.aspx?codename=$thread.forum.codename&threadids=$thread.ThreadID" onclick="return openDialog(this.href,this,refresh);">删除</a>
                    </td>
                </tr>
    <!--[/loop]-->
    <!--[if $TotalCount > 0]-->
            </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" id="selectAll" /> <label for="selectAll">全选</label>
            <!--[if $IsRecycleBin]-->
            <input type="submit" class="button" name="restorechecked" onclick="if(confirm('确认还原选中的数据吗?')==false)return false;" value="还原选中" />
            <input type="submit" class="button" name="restoresearched" onclick="if(confirm('确认还原搜索到的数据吗?')==false)return false;" value="还原搜索到的数据" />
            <!--[/if]-->
            <input type="submit" class="button" name="deletechecked" onclick="if(confirm('确认删除选中的数据吗?')==false)return false;" value="删除选中" />
            <input type="submit" class="button" name="deletesearched" onclick="if(confirm('确认删除搜索到的数据吗?')==false)return false;" value="删除搜索到的数据" />
            <input value="1" id="updatePoint_2" name="updatePoint" checked="checked" type="checkbox" />
            <label for="updatePoint_2">删除时更新用户积分</label>
        </div>
        <script type="text/javascript">
            new checkboxList( 'topicids', 'selectAll');
        </script>
        <!--[AdminPager Count="$TotalCount" PageSize="$PageSize" /]-->
    <!--[else]-->
        <div class="NoData">未搜索到主题.</div>
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
