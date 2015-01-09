<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>相册管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $HasNoPermissionManageRole]-->
<div class="Tip Tip-permission">由于权限限制，你无法管理“$NoPermissionManageRoleNames”用户组的相册。此处不会列出这些数据。</div>
<!--[/if]-->

<div class="Content">
    <h3>相册搜索</h3>
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
	    <td><label for="searchkey">相册名</label></td>
	    <td><input class="text" id="name" name="name" type="text" value="$AdminForm.Name" /></td>
	    <td><label for="albumid">相册ID</label></td>
	    <td><input class="text" id="albumid" name="albumid" type="text" value="$AdminForm.AlbumID" /></td>
	</tr>
	<tr>
	    <td style="text-align:right;"><label for="privacytype">隐私类型</label></td>
	    <td colspan="3">
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
	    <td>搜索时间</td>
	    <td colspan="3">
	        <input name="begindate" id="begindate" class="text" style="width:6em;" type="text" value="$AdminForm.BeginDate" />
	        <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a> ~
	        <input name="enddate" class="text" id="enddate" style="width:6em;" type="text" value="$AdminForm.EndDate" />
	        <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a> 
	        <span class="desc">(时间格式: YYYY-MM-DD)</span>
	    </td>
	</tr>
	<tr>
	    <td>
	        <label>结果排序</label>
	    </td>
	    <td>
            <select name="order">
                <option value="CreateDate" $_form.selected("order","CreateDate",$AdminForm.Order)>创建时间</option>
                <option value="UpdateDate" $_form.selected("order","UpdateDate",$AdminForm.Order)>更新时间</option>
                <option value="TotalPhotos" $_form.selected("order","TotalPhotos",$AdminForm.Order)>图片数</option>
            </select>
            <select name="isdesc">
                <option value="True" $_form.selected("isdesc","True",$AdminForm.IsDesc.ToString())>按降序排列</option>
                <option value="False" $_form.selected("isdesc","False",$AdminForm.IsDesc.ToString())>按升序排列</option>
            </select>
        </td>
        <td>每页显示数</td>
        <td>
            <select name="pagesize">
                <option value="10" $_form.selected("pagesize","10",$AdminForm.PageSize.ToString())>10</option>
                <option value="20" $_form.selected("pagesize","20",$AdminForm.PageSize.ToString())>20</option>
                <option value="50" $_form.selected("pagesize","50",$AdminForm.PageSize.ToString())>50</option>
                <option value="100" $_form.selected("pagesize","100",$AdminForm.PageSize.ToString())>100</option>
                <option value="200" $_form.selected("pagesize","200",$AdminForm.PageSize.ToString())>200</option>
                <option value="500" $_form.selected("pagesize","500",$AdminForm.PageSize.ToString())>500</option>
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

    <form action="$_form.action" method="post" name="albumlistForm" id="albumlistForm">
    <div class="DataTable">
    <h4>相册 <span class="counts">总数: $AlbumTotalCount</span></h4>
    <!--[if $AlbumTotalCount > 0]-->
    <div class="Actions">
        <input type="checkbox" id="selectAll_2" />
        <label for="selectAll_2">全选</label>
        <input type="submit" class="button" name="deletechecked" value="删除选中" onclick="return confirm('确认要删除吗?删除后不可恢复!');"/>
        <input type="submit" class="button" name="deletesearched" value="删除搜索到的相册" onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');"  />
		<input value="1" id="updatePoint_2" name="updatePoint" checked="checked" type="checkbox" />
		<label for="updatePoint_2">删除时更新用户积分</label>
    </div>
    <table>
        <tbody>
            <thead>
                <tr>
                    <th class="CheckBoxHold">&nbsp;</th>
                    <th style="width:100px;">封面</th>
                    <th style="width:100px;">名称</th>
                    <th style="width:100px;">创建者</th>
                    <th style="width:100px;">创建时间</th>
                    <th style="width:100px;">最后编辑者</th>
                    <th style="width:100px;">更新时间</th>
                    <th style="width:50px;">操作</th>
                </tr>
            </thead>
            <tbody>
    <!--[/if]-->
	<!--[loop $album in $albumlist with $i]-->
			<tr id="album_$album.id">
				<td><input id="albumId$i" type="checkbox" name="albumids" value="$Album.ID" /></td>
				<td>
					<a href="$url(app/album/list)?id=$Album.ID" target="_blank">
					<img src="$Album.CoverSrc" alt="" title="$Album.Name" />
					</a>
				</td>
				<td>
					<a href="$url(app/album/list)?id=$Album.ID" target="_blank">$Album.Name</a>
				</td>
				<td><a href="javascript:;" onclick="return openUserMenu(this,$Album.User.id,'photo')">$Album.User.username</a></td>
				<td>$OutputDateTime($Album.CreateDate)</td>
				<td> $Album.LastEditUser.PopUpNameLink</td>
				<td>$OutputDateTime($Album.UpdateDate)</td>
				<td>
					<a href="$admin/app/manage-photo.aspx?AlbumID=$Album.ID">管理</a> |
					<a href="$dialog/album-delete.aspx?albumid=$Album.ID" onclick="return openDialog(this.href,this, function(){delElement($('album_$album.id'))});">删除</a>
				</td>
			 </tr>
	<!--[/loop]-->
    <!--[if $AlbumTotalCount > 0]-->
			</tbody>
    </table>
    <div class="Actions">
        <input type="checkbox" id="selectAll" />
        <label for="selectAll">全选</label>
        <input type="submit" class="button" name="deletechecked" value="删除选中" onclick="return confirm('确认要删除吗?删除后不可恢复!');"/>
        <input type="submit" class="button" name="deletesearched" value="删除搜索到的相册" onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');"  />
		<input value="1" id="updatePoint" name="updatePoint" checked="checked" type="checkbox" />
		<label for="updatePoint">删除时更新用户积分</label>
    </div>
    <script type="text/javascript">
       new checkboxList( 'albumids', 'selectAll');
       new checkboxList( 'albumids', 'selectAll_2');
    </script>
    <!--[AdminPager Count="$AlbumTotalCount" PageSize="$AdminForm.PageSize"/]-->
    <!--[else]-->
    <div class="NoData">未找到任何相册.</div>
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
