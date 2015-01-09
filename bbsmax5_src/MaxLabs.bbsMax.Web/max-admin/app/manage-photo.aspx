<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>相片管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $HasNoPermissionManageRole]-->
<div class="Tip Tip-permission">由于权限限制，你可能无法管理“$NoPermissionManageRoleNames”用户组的照片。此处不会列出这些数据。</div>
<!--[/if]-->

<div class="Content">
    <h3>相片搜索</h3>
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
	    <td><label for="albumid">所属相册ID</label></td>
	    <td><input class="text" id="albumid" name="albumid" type="text" value="$AdminForm.AlbumID" /></td>
	    <td><label for="photoid">指定图片ID</label></td>
	    <td><input class="text" id="photoid" name="photoid" type="text" value="$AdminForm.PhotoID" /></td>
	</tr>
	<tr>
	    <td><label for="searchkey">关键字</label></td>
	    <td><input class="text" id="searchkey" name="searchkey" type="text" value="$AdminForm.SearchKey" /></td>
	    <td><label for="createip">创建者IP</label></td>
	    <td><input class="text" id="createip" name="createip" type="text" value="$AdminForm.CreateIP" /></td>  
	</tr>
	<tr>
	    <td>搜索时间</td>
	    <td colspan="3">
	        <input name="begindate" id="begindate" class="text" style="width:6em;" type="text" value="$AdminForm.BeginDate" />
	        <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
	        ~ <input id="enddate" name="enddate" class="text" style="width:6em;" type="text" value="$AdminForm.endDate" />
	        <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
	        <span class="desc">(时间格式: YYYY-MM-DD)</span>
	    </td>
	</tr>
	<tr>
	    <td>结果排序</td>
	    <td>
            <select name="order">
                <option value="PhotoID" $_form.selected("order","PhotoID",$AdminForm.Order)>创建时间</option>
                <option value="UpdateDate" $_form.selected("order","UpdateDate",$AdminForm.Order)>更新时间</option>
            </select>
            <select name="isdesc">
                <option value="True" $_form.selected("isdesc", "True", $AdminForm.IsDesc.ToString())>按降序排列</option>
                <option value="False" $_form.selected("isdesc", "False", $AdminForm.IsDesc.ToString())>按升序排列</option>
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

    <form action="$_form.action" method="post" id="photolistForm" name="photolistForm">
    <div class="DataTable">
    <h4>相片 <span class="counts">总数: $PhotoTotalCount</span></h4>
        <!--[if $PhotoTotalCount > 0]-->
        <div class="Actions">
            <input type="checkbox" id="selectAll_2" />
            <label for="selectAll_2">全选</label>
            <input type="submit" class="button" name="deletechecked" value="删除选中" onclick="return confirm('确认要删除吗?删除后不可恢复!');" />
	        <input type="submit" class="button" name="deletesearched" value="删除搜索到的相片" onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" />
			<input value="1" id="updatePoint_2" name="updatePoint" checked="checked" type="checkbox" />
			<label for="updatePoint_2">删除时更新用户积分</label>
        </div>
        <table>
			<thead>
				<tr>
	                <th class="CheckBoxHold">&nbsp;</th>	
	                <th>&nbsp;</th>
					<th>相片名称</th>
					<th>描述</th>
					<th>上传时间</th>
					<th>创建者</th>
					<th>创建者IP</th>
					<th>操作</th>
				</tr>
			</thead>
            <tbody>
        <!--[/if]-->
    <!--[loop $photo in $PhotoList with $i]-->
            <tr>
				<td>
					<input id="photoId$i" type="checkbox" class="CheckBoxHold" name="photoids" value="$Photo.ID" />
				</td>
                <td>
                    <a href="$url(app/album/photo)?id=$Photo.ID" target="_blank"><img src="$Photo.ThumbSrc" alt="" title="$Photo.Name" /></a>
                </td>
                <td>
					<a href="$url(app/album/photo)?id=$Photo.ID" target="_blank">$Photo.Name</a>
				</td>
                <td>$Photo.Description</td>
                <td>$Photo.CreateDate</td>
                <td><a class="menu-dropdown" href="javascript:;" onclick="return openUserMenu(this,$Photo.User.id,'photo')">$Photo.User.username</a></td>
                <td>$Photo.CreateIP</td>
                <td>
                    <a href="$admin/app/manage-album.aspx?AlbumID=$Photo.AlbumID">管理所属相册</a>
                    <a href="$dialog/photo-delete.aspx?photoid=$Photo.ID" onclick="openDialog(this.href,this); return false;">删除</a>
                </td>
             </tr>
    <!--[/loop]-->
        <!--[if $PhotoTotalCount > 0]-->
            </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" id="selectAll" />
            <label for="selectAll">全选</label>
            <input type="submit" class="button" name="deletechecked" value="删除选中" onclick="return confirm('确认要删除吗?删除后不可恢复!');" />
	        <input type="submit" class="button" name="deletesearched" value="删除搜索到的相片" onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" />
			<input value="1" id="updatePoint" name="updatePoint" checked="checked" type="checkbox" />
			<label for="updatePoint">删除时更新用户积分</label>
        </div>
        <script type="text/javascript">
            checkboxList( 'photoids', 'selectAll');
            checkboxList( 'photoids', 'selectAll_2');
        </script>
        <!--[AdminPager Count="$PhotoTotalCount" PageSize="$AdminForm.PageSize" /]-->
		<!--[else]-->
	    <div class="NoData">未搜索到任何相片.</div>
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
