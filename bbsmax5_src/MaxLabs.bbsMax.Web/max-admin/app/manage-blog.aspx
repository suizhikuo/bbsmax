<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>日志管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
function deleteBlog(id)
{
    removeElement($('item_'+id));
}
</script>
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $HasNoPermissionManageRole]-->
<div class="Tip Tip-permission">由于权限限制，你无法管理“$NoPermissionManageRoleNames”用户组的日志。此处不会列出这些数据。</div>
<!--[/if]-->
<div class="Content">
    <h3>日志搜索</h3>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
    <table>
    <tr>
        <td><label for="username">用户名</label></td>
        <td><input class="text" id="username" name="username" type="text" value="$AdminForm.Username" /></td>
        <td><label for="authorid">作者ID</label></td>
        <td><input class="text" id="authorid" name="authorid" type="text" value="$AdminForm.AuthorID" /></td>
    </tr>
    <tr>
        <td>搜索方式</td>
        <td>
            <input id="subject" type="radio" value="Subject" name="searchmode"  $_form.checked("searchmode","Subject",$AdminForm.SearchMode.ToString()) /><label for="subject">标题</label>
            <input id="fulltext" type="radio" value="FullText" name="searchmode" $_form.checked("searchmode","FullText",$AdminForm.SearchMode.ToString()) /><label for="fulltext">全文</label>
            <input id="subjectorfulltext" type="radio" value="Default" name="searchmode" $_form.checked("searchmode","Default",$AdminForm.SearchMode.ToString()) /><label for="subjectorfulltext">标题或全文</label>
        </td>
        <td><label for="searchkey">关键字</label></td>
        <td><input class="text" id="searchkey" name="searchkey" type="text" value="$AdminForm.SearchKey" /></td>
    </tr>
    <tr>
        <td><label for="articleid">指定日志ID</label></td>
        <td colspan="3"><input class="text" id="articleid" name="articleid" type="text" value="$AdminForm.ArticleID" /></td>
    </tr>
    <tr>
        <td><label for="createip">作者IP</label></td>
        <td><input class="text" id="createip" name="createip" type="text" value="$AdminForm.CreateIP" /></td>
        <td><label for="privacytype">隐私类型</label></td>
        <td>
            <select name="privacytype">
                <option value="null">不限制</option>
                <option value="AllVisible" $_form.selected("privacytype","AllVisible",$AdminForm.PrivacyType.ToString())>全站用户可见</option>
                <option value="FriendVisible" $_form.selected("privacytype","FriendVisible",$AdminForm.PrivacyType.ToString())>全好友可见</option>
                <option value="SelfVisible" $_form.selected("privacytype","SelfVisible",$AdminForm.PrivacyType.ToString())>仅自己可见</option>
                <option value="NeedPassword" $_form.selected("privacytype","NeedPassword",$AdminForm.PrivacyType.ToString())>凭密码查看</option>
            </select>
        </td>
    </tr>
    <tr>
        <td><label>查看数</label></td>
        <td><input name="totalviewsscopebegin" class="text" style="width:6em;" type="text" value="$AdminForm.TotalViewsScopeBegin" /> ~ <input name="totalviewsscopeend" class="text" style="width:6em;" type="text" value="$AdminForm.TotalViewsScopeEnd" /></td>
        <td><label>评论数</label></td>
        <td><input name="totalcommentsscopebegin" class="text" style="width:6em;" type="text" value="$AdminForm.TotalCommentsScopeBegin" /> ~ <input name="totalcommentsscopeend" class="text" style="width:6em;" type="text" value="$AdminForm.TotalCommentsScopeEnd" /></td>
    </tr>
    <tr>
        <td>搜索时间</td>
        <td colspan="3">
            <input name="begindate" id="begindate" class="text" style="width:6em;" type="text" value="$AdminForm.BeginDate" />
            <a class="selector-date" title="选择日期" href="javascript:void(0);" id="A0"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            ~
            <input name="enddate" id="enddate" class="text" style="width:6em;" type="text" value="$AdminForm.EndDate" /> 
            <a class="selector-date" title="选择日期" href="javascript:void(0);" id="A1"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            <span class="desc">(时间格式: YYYY-MM-DD)</span>
        </td>
    </tr>
    <tr>
        <td>结果排序</td>
        <td>
            <select name="order">
                <option value="ArticleID" $_form.selected("order","ArticleID",$AdminForm.Order)>发布时间</option>
                <option value="CommentDate" $_form.selected("order","CommentDate",$AdminForm.Order)>评论时间</option>
                <option value="Replies" $_form.selected("order","Replies",$AdminForm.Order)>回复数量</option>
                <option value="Views" $_form.selected("order","Views",$AdminForm.Order)>浏览次数</option>
            </select>
            <select name="isdesc">
                <option value="true" $_form.selected("isdesc","true",$AdminForm.IsDesc.ToString())>按降序排列</option>
                <option value="false" $_form.selected("isdesc","false",$AdminForm.IsDesc.ToString())>按升序排列</option>
            </select>
        </td>
        <td>每页显示数</td>
        <td>
            <select name="pagesize">
                <option value="10" $_form.selected("pagesize","10",$AdminForm.PageSize)>10</option>
                <option value="20" $_form.selected("pagesize","20",$AdminForm.PageSize)>20</option>
                <option value="50" $_form.selected("pagesize","50",$AdminForm.PageSize)>50</option>
                <option value="100" $_form.selected("pagesize","100",$AdminForm.PageSize)>100</option>
                <option value="200" $_form.selected("pagesize","200",$AdminForm.PageSize)>200</option>
                <option value="500" $_form.selected("pagesize","500",$AdminForm.PageSize)>500</option>
            </select>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td colspan="3">
            <input type="submit" name="advancedsearch" class="button" value="搜索" />
        </td>
    </tr>
    </table>
    </form>
    </div>

    <form action="$_form.action" method="post" name="articlelistForm" id="articlelistForm">
        <div class="DataTable">
        <h4>日志 <span class="counts">总数: $TotalArticleCount</span></h4>
        <!--[if $TotalArticleCount > 0]-->
        <div class="Actions">
            <input type="checkbox" id="selectAll_top" /> <label for="selectAll_top">全选</label>
            <input value="1" id="updatePoint" name="updatePoint" checked="checked" type="checkbox" />
            <label for="updatePoint">删除时更新用户积分</label>
            <input type="submit" class="button" name="deletechecked" value="删除选中" onclick="return confirm('确认要删除吗?删除后不可恢复!');" />
            <input type="submit" class="button" name="deletesearched" value="删除搜索到的数据" onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!');" />
        </div>
        <table>
            <thead>
                <tr>
                    <th class="CheckBoxHold">&nbsp;</th>
                    <th>标题</th>
                    <th style="width:100px;">作者</th>
                    <th style="width:100px;">作者IP</th>
                    <th style="width:100px;">评论/查看</th>
                    <th style="width:100px;">分类</th>
                    <th style="width:100px;">发布时间</th>
                    <th style="width:100px;">更新时间</th>
                    <th style="width:50px;">操作</th>
                </tr>
            </thead>
            <tbody>
        <!--[/if]-->
    <!--[loop $article in $ArticleList]-->
                <tr id="item_$Article.ID">
                    <td><input name="articleids" type="checkbox" value="$Article.ID" /></td>
                    <td><a href="$url(app/blog/view)?id=$Article.ID" target="_blank">$Article.OriginalSubject</a></td>
                    <td><a class="menu-dropdown" href="javascript:;" onclick="return openUserMenu(this,$Article.User.id,'blog')">$Article.User.username</a></td>
                    <td>$outputip($Article.CreateIP)</td>
                    <td>$Article.TotalComments/$Article.TotalViews</td>
                    <td><a href="$url(app/blog/index)?cid=$Article.CategoryID" target="_blank">$article.CategoryName</a></td>
                    <td>$OutputDateTime($Article.CreateDate)</td>
                    <td>$OutputDateTime($Article.UpdateDate)</td>
                    <td><a href="$dialog/blog-blogarticle-delete.aspx?id=$Article.ID" onclick="return openDialog(this.href,function(result){ delElement($('item_$Article.ID')); });">删除</a></td>
                </tr>
    <!--[/loop]-->
    <!--[if $TotalArticleCount > 0]-->
            </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" id="selectAll_bottom" /> <label for="selectAll_bottom">全选</label>
            <input value="1" id="updatePoint_2" name="updatePoint" checked="checked" type="checkbox" />
            <label for="updatePoint_2">删除时更新用户积分</label>
            <input type="submit" class="button" name="deletechecked" value="删除选中" onclick="return confirm('确认要删除吗?删除后不可恢复!');" />
            <input type="submit" class="button" name="deletesearched" value="删除搜索到的数据" onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');"/>
        </div>
        <script type="text/javascript">
            new checkboxList( 'articleids', 'selectAll_top');
            new checkboxList( 'articleids', 'selectAll_bottom');
        </script>
        <!--[AdminPager Count="$TotalArticleCount" PageSize="$ArticleListPageSize" /]-->
    <!--[else]-->
        <div class="NoData">未搜索到任何日志.</div>
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
