<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>记录管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[if $HasNoPermissionManageRole]-->
<div class="Tip Tip-permission">由于权限限制，你无法管理“$NoPermissionManageRoleNames”用户组的记录。此处不会列出这些数据。</div>
<!--[/if]-->
<div class="Content">
    <h3>记录管理</h3>
	<div class="SearchTable">
        <form id="filter" action="$_form.action" method="post">
        <table>
        <tr>
            <td style="width:100px;">作者ID</td>
            <td><input type="text" class="text" id="Text1" name="UserID" maxlength="50" value="$filter.UserID"/></td>
            <td>作者名</td>
            <td><input type="text" class="text" id="Text2" name="Username" maxlength="50" value="$filter.UserName"/></td>
        </tr>
        <tr>
            <td>内容</td>
            <td><input type="text" class="text" id="Text9" name="Content" size="20" maxlength="50" value="$filter.Content"/></td>
            <td>IP</td>
            <td><input type="text" class="text" id="Text3" name="IP" size="20" maxlength="50" value="$filter.IP"/></td>
        </tr>
        <tr>
            <td>发布时间</td>
            <td colspan="3">
            <input type="text" class="text" style="width:6em;" name="BeginDate" id="begindate"  size="20"  value="$filter.BeginDate"/>
            <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            ~
            <input type="text" class="text" style="width:6em;" name="EndDate" id="enddate" size="20" value="$filter.EndDate"/>
            <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            <span class="desc">(时间格式: YYYY-MM-DD)</span>
            </td>
        </tr>
        <tr>
            <td>结果排序</td>
            <td>
                <select name="Order">
                <option value="ID" $_Form.selected('Order','ID',$filter.OrderBy)>发布时间</option>
                </select>
                <select name="IsDesc">
                <option value="true" $_Form.selected('IsDesc','true',$filter.IsDesc.ToString())>按降序排列</option>
                <option value="false" $_Form.selected('IsDesc','false',$filter.IsDesc.ToString())>按升序排列</option>
                </select>
            </td>
            <td>每页显示数</td>
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
            <td colspan="3"><input type="submit" class="button" name="searchdoing" value="搜索"/></td>
        </tr>
        </table>
        </form>
	</div>
    <form id="doingPost" action="$_form.action" method="post" name="doingPost">
        <div class="DataTable">
        <h4>记录 <span class="counts">总数: $DoingTotalCount</span></h4>
        <!--[if $DoingTotalCount > 0]-->
        <div class="Actions">
            <input type="checkbox" name="checkAll" value="checkbox" id="checkAll_2" />
            <label for="checkAll_2">全选</label>
            <input value="1" id="updatePoint_2" name="updatePoint" checked="checked" type="checkbox" />
            <label for="updatePoint_2">删除时更新用户积分</label>
            <input name="deletedoing" class="button" onclick="return confirm('确认要删除吗?删除后不可恢复!');" type="submit" value="批量删除"/>
             <input name="deletesearch" class="button"  onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" type="submit"  value="删除搜索到的数据" />
        </div>
        <table>
            <thead>
	            <tr>
	                <td class="CheckBoxHold">&nbsp;</td>
	                <td>记录</td>
	                <td style="width:150px;">时间</td>
	                <td style="width:150px;">作者</td>
	                <td style="width:80px;">操作</td>
	            </tr>
            </thead>
            <tbody>
	    <!--[/if]-->
	<!--[loop $doing in $DoingList]-->
                <tr id="doing-$doing.id">
                    <td><input type="checkbox" name="DoingID" value="$doing.ID"/></td>
                    <td>$doing.OriginalContent</td><td>$OutputDateTime($doing.CreateDate)</td>
                    <td><a class="menu-dropdown" href="javascript:;" onclick="return openUserMenu(this,$doing.User.id,'doing')">$doing.User.Username</a></td>
                    <td>
                    <a href="$dialog/doing-delete.aspx?id=$doing.ID" onclick="return openDialog(this.href,this,function(r){delElement($('doing-'+r.id));});">删除</a>
                    </td>
                </tr>
    <!--[/loop]-->
        <!--[if $DoingTotalCount > 0]-->
            </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" name="checkAll" value="checkbox" id="checkAll" />
            <label for="checkAll">全选</label>
            <input value="1" id="updatePoint" name="updatePoint" checked="checked" type="checkbox" />
            <label for="updatePoint">删除时更新用户积分</label>
            <input name="deletedoing" class="button" onclick="return confirm('确认要删除吗?删除后不可恢复!');" type="submit" value="批量删除"/>
            <input name="deletesearch" class="button"  onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" type="submit"  value="删除搜索到的数据" />
        </div>
        <!--[AdminPager Count="$DoingTotalCount" PageSize="$Filter.PageSize" /]-->
        <!--[else]-->
        <div class="NoData">未搜索到任何记录.</div>
        <!--[/if]-->
    </div>
    </form>
</div>
<!--[include src="../_foot_.aspx" /]-->
<script type="text/javascript">
new checkboxList('DoingID','checkAll');
new checkboxList('DoingID', 'checkAll_2');

initDatePicker('begindate','A0');
initDatePicker('enddate','A1');
</script>
</body>
</html>
