<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>评论留言管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[if $HasNoPermissionManageRole]-->
<div class="Tip Tip-permission">由于权限限制，你无法管理“$NoPermissionManageRoleNames”用户组的评论和留言数据。此处不会列出这些数据。</div>
<!--[/if]-->
<div class="Content">
    <h3>评论留言管理</h3>
	<div class="SearchTable">
    <form id="filter" action="$_form.action" method="post">
    <table>
        <tr>
            <td style="width:100px;">评论类型</td>
            <td>
                <select name="Type">
                <option value="All" $_Form.selected('Type','All',$filter.Type)>所有</option>
                <option value="Board" $_Form.selected('Type','Board',$filter.Type)>留言</option>
                <option value="Blog" $_Form.selected('Type','Blog',$filter.Type)>日志</option>
                <option value="Doing" $_Form.selected('Type','Doing',$filter.Type)>记录</option>
                <option value="Photo" $_Form.selected('Type','Photo',$filter.Type)>相片</option>
                <option value="Share" $_Form.selected('Type','Share',$filter.Type)>分享</option>
                </select>
            </td>
            <td>是否审核</td>
            <td>
                <select name="IsApproved">
                <option value="" $_Form.selected('IsApproved','',$filter.IsApproved==null)>全部</option>
                <option value="false" $_Form.selected('IsApproved','false',$filter.IsApproved==false)>未审核</option>
                <option value="true" $_Form.selected('IsApproved','true',$filter.IsApproved==true)>审核通过</option>
                </select>
            </td>
        </tr>
        <tr>
            <td>评论用户名</td>
            <td><input type="text" class="text" name="Username" size="20" maxlength="50" value="$filter.Username"/></td>
            <td>被评论用户名</td>
            <td><input type="text" class="text" name="TargetUsername" size="20" maxlength="50" value="$filter.TargetUsername"/></td>
        </tr>
        <tr>
            <td>发布时间</td>
            <td colspan="3">
            <input type="text" class="text" style="width:6em;" name="BeginDate" size="20"  value="$filter.BeginDate"/> ~
            <input type="text" class="text" style="width:6em;" name="EndDate" size="20" value="$filter.EndDate"/>
            <span class="desc">(时间格式: YYYY-MM-DD)</span>
            </td>
        </tr>
        <tr>
            <td>内容</td>
            <td><input type="text" class="text" name="Content" size="20"  value="$filter.Content"/></td>
            <td>发布IP</td>
            <td><input type="text" name="IP" class="text" size="20" value="$filter.IP"/></td>
        </tr>
        <tr>
            <td>结果排序</td>
            <td>
                <select name="Order">
                <option value="CommentID" $_Form.selected('Order','CommentID',$filter.Order)>默认排序</option>
                </select>
                <select name="IsDesc">
                <option value="true" $_Form.selected('IsDesc','true',$filter.IsDesc==true)>按降序排列</option>
                <option value="false" $_Form.selected('IsDesc','false',$filter.IsDesc==false)>按升序排列</option>
                </select>
            </td>
            <td>每页显示</td>
            <td>
                <select name="PageSize">
                <option value="10" $_Form.selected('PageSize','10',$filter.PageSize)>10</option>
                <option value="20" $_Form.selected('PageSize','20',$filter.PageSize)>20</option>
                <option value="50" $_Form.selected('PageSize','50',$filter.PageSize)>50</option>
                <option value="100" $_Form.selected('PageSize','100',$filter.PageSize)>100</option>
                <option value="200" $_Form.selected('PageSize','200',$filter.PageSize)>200</option>
                <option value="500" $_Form.selected('PageSize','500',$filter.PageSize)>500</option>
                </select>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td><input type="submit" class="button" name="searchcomment" value="搜索"/></td>
        </tr>
    </table>
    </form>
	</div>
    <form id="form2" action="$_form.action" method="post">
        <div class="DataTable">
        <h4>评论 <span class="counts">总数: $CommentTotalCount</span></h4>
        <!--[if $CommentTotalCount > 0]-->
        <div class="Actions">
            <input type="checkbox" name="checkAll" value="checkbox" id="checkAll_2"/>
            <label for="checkAll_2">全选</label>
            <input value="1" id="updatePoint_2" name="updatePoint" checked="checked" type="checkbox" />
            <label for="updatePoint_2">删除时更新用户积分</label>
            <input name="deletecomment" class="button" onclick="return confirm('确认要删除吗?删除后不可恢复!');" type="submit" value="批量删除"/>
            <!--[if $filter.IsApproved != true]-->
            <input name="approvecomment" class="button" onclick="return confirm('确认要审核吗?审核后不可恢复!');" type="submit" value="批量审核"/>
            <!--[/if]-->
            <input name="deletesearch" class="button"  onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" type="submit"  value="删除搜索到的数据" />
        </div>
        <table>
            <thead>
                <tr>
                    <th class="CheckBoxHold">&nbsp;</th>
                    <th>评论</th>
                    <th style="width:100px;">时间</th>
                    <th style="width:80px;">作者</th>
                    <th style="width:50px;">审核</th>
                    <th style="width:100px;">操作</th>
                </tr>
            </thead>
            <tbody>
        <!--[/if]-->
		<!--[loop $comment in $CommentList]-->
                <tr id="comment_$comment.id">
                    <td><input type="checkbox" name="CommentID" value="$comment.ID"/></td>
                    <td>$comment.Content</td>
                    <td>$comment.CreateDate</td>
                    <td>$comment.User.PopUpnameLink</td>
                    <td>$_if($comment.IsApproved,"通过","待审核")</td>
                    <td>
                        <a href="$dialog/manage-comment-delete.aspx?commentid=$comment.ID" onclick="return openDialog(this.href, function(result){delElement('comment_$comment.id')})">删除</a> 
                        <!--[if !$comment.IsApproved]-->
                        <a href="$dialog/manage-comment-approve.aspx?commentid=$comment.ID" onclick="return openDialog(this.href, function(result){})">审核</a>
                        <!--[/if]-->
                    </td>
                </tr>
		<!--[/loop]-->
        <!--[if $CommentTotalCount > 0]-->
            </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" name="checkAll" value="checkbox" id="checkAll"/>
            <label for="checkAll">全选</label>
            <input value="1" id="updatePoint" name="updatePoint" checked="checked" type="checkbox" />
            <label for="updatePoint">删除时更新用户积分</label>
            <input name="deletecomment" class="button" onclick="return confirm('确认要删除吗?删除后不可恢复!');" type="submit" value="批量删除"/>
            <!--[if $filter.IsApproved != true]-->
            <input name="approvecomment" class="button" onclick="return confirm('确认要审核吗?审核后不可恢复!');" type="submit" value="批量审核"/>
            <!--[/if]-->
            <input name="deletesearch" class="button"  onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" type="submit"  value="删除搜索到的数据" />
        </div>
        <!--[AdminPager Count="$CommentTotalCount" PageSize="$Filter.PageSize"/]-->
        <!--[else]-->
        <div class="NoData">未搜索到任何评论.</div>
        <!--[/if]-->
        </div>
    <!--[/foot]-->
    <!--[/SearchComments ]-->
    </form>
</div>
<script type="text/javascript">
new checkboxList('CommentID','checkAll');
new checkboxList('CommentID','checkAll_2');
</script>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
