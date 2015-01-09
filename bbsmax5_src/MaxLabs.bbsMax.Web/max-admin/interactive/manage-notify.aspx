<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>通知管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[if $HasNoPermissionManageRole]-->
<div class="Tip Tip-permission">由于权限限制，你可能无法管理“$NoPermissionManageRoleNames”用户组的通知。此处不会列出这些数据。</div>
<!--[/if]-->
<div class="Content">
    <h3>通知搜索</h3>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
    <table>
    <tbody id="SearchByUserBox">
    <tr>
        <td><label for="username">接收者用户名</label></td>
        <td><input type="text" id="owner" name="owner" class="text" value="$_form.text('owner',$NotifyFilter.owner)" /></td>
        <td></td>
        <td></td>
    </tr>
    </tbody>
    <tr>
        <td><label for="begindate">搜索时间段</label></td>
        <td colspan="3">
            <input name="begindate" id="begindate" class="text" style="width:6em;" type="text" value="$NotifyFilter.BeginDate" /><a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            ~
            <input name="enddate" id="enddate" class="text" style="width:6em;" type="text" value="$NotifyFilter.EndDate" />
            <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            <span class="desc">(时间格式: YYYY-MM-DD)</span>
        </td>
    </tr>
    <tr>
        <td>类型</td>
        <td>
            <select name="notifytype">
                <option value="null">不限制</option>
                <!--[loop $t in $NotifyTypeList]-->
                <option value="$t.typeid" $_form.selected("notifytype","$t.typeid",$NotifyFilter.NotifyType.ToString())>$t.typename</option>
                <!--[/loop]-->
            </select>&nbsp;
        </td>
        <tr>
        <td>状态</td>
        <td>
            <input type="radio" name="isread" id="Radio1"value="" $_form.checked("isread","",$NotifyFilter.isread==null)/><label for="isread1">全部 </label> 
            <input type="radio" name="isread" id="Radio2" value="true" $_form.checked("isread","",$NotifyFilter.isread==true) /><label for="isread2">已读 </label>  
            <input type="radio" name="isread" id="Radio3" value="false" $_form.checked("isread","",$NotifyFilter.isread==false) /><label for="isread3">未读 </label> 
        </td>
        </tr>
        <td>每页显示数</td>
        <td>
            <select name="pagesize">
                <option value="10" $_form.selected("pagesize","10",$NotifyFilter.PageSize)>10</option>
                <option value="20" $_form.selected("pagesize","20",$NotifyFilter.PageSize)>20</option>
                <option value="50" $_form.selected("pagesize","50",$NotifyFilter.PageSize)>50</option>
                <option value="100" $_form.selected("pagesize","100",$NotifyFilter.PageSize)>100</option>
                <option value="200" $_form.selected("pagesize","200",$NotifyFilter.PageSize)>200</option>
                <option value="500" $_form.selected("pagesize","500",$NotifyFilter.PageSize)>500</option>
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
    
<!--[if $NotifyList.totalRecords>0]-->
    <form action="$_form.action" method="post" name="notifylistForm" id="notifylistForm">
        <div class="DataTable">
        <h4>通知 <span class="counts">总数: $NotifyList.totalRecords</span></h4>

        <table>
            <thead>
                <tr>
                    <th class="CheckBoxHold">&nbsp;</th>
                    <th>内容</th>
                    <th style="width:90px;">接收者</th>
                    <th style="width:80px;">类型</th>
                    <td style="width:100px;">日期</td>
                    <th style="width:50px;">操作</th>
                </tr>
            </thead>
            <tbody>
            <!--[loop $Notify in $NotifyList]-->
                <tr>
                    <td><input name="notifyids" type="checkbox" value="$Notify.notifyID" /></td>
                    <td>$Notify.content</td>
                    <td><a class="menu-dropdown" href="javascript:;" onclick="return openUserMenu(this,$Notify.User.id,'notify')">$Notify.User.username</a></td>
                    <td>$getTypename( $Notify.Typeid)</td>
                    <td>$Notify.UpdateDate.Friendly</td>
                    <td><a href="$dialog/notify-delete.aspx?action=deleteone&notifyid=$Notify.notifyID&manage=true" onclick="openDialog(this.href,refresh); return false;">删除</a></td>
                </tr>
            <!--[/loop]-->
                </tbody>
            </table>
	        <div class="Actions">
                <input type="checkbox" id="selectAll" />
                <label for="selectAll">全选</label>
                <input type="submit" class="button" name="deletechecked" value="删除选中" />
		        <input type="submit" class="button" name="deletesearched" value="删除搜索到的数据" />
            </div>
            <script type="text/javascript">
                new checkboxList('notifyids','selectAll');
            </script>
            <!--[AdminPager Count="$NotifyList.totalRecords" PageSize="$NotifyFilter.PageSize" /]-->
        <!--[else]-->
            <div class="NoData">未搜索到任何通知.</div>
        <!--[/if]-->
        </div>
    </form>
    <!--[/foot]-->
<!--[/NotifySearchList]-->
</div>
<script type="text/javascript">
    initDatePicker('begindate','A0');
    initDatePicker('enddate','A1');

</script>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
