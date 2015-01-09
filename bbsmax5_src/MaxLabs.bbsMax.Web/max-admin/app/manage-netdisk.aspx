<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>网络硬盘管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $HasNoPermissionManageRole]-->
<div class="Tip Tip-permission">由于权限限制，您没有权限管理“$NoPermissionManageRoleNames”用户组的文件。此处不会列出这些数据。</div>
<!--[/if]-->
<div class="Content">
    <div class="PageHeading">
	    <h3>网络硬盘管理 </h3>
    </div>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
<table>
    <tr> 
        <td>用户名 </td>
        <td><input type="text" name="username" class="text" value="$_form.text('username',$filter.username)" /></td>
        <td>用户ID </td>
        <td><input type="text" name="userid" class="text" value="$_form.text('userid',$filter.userid)" /></td>
    </tr>
    <tr>
        <td>
        文件名
        </td>
        <td><input type="text" name="filename" class="text" value="$_form.text('filename',$filter.filename)" /></td>
        <td></td>
        <td></td>
    </tr>
    <tr>
        <td>创建时间介于</td>
        <td colspan="3">
        <input type="text"  style="width:6em;" class="text" id="createDate_1" name="createDate_1" value="$_form.text('createDate_1',$filter.CreateDate_1)" />
        <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
        ~ <input type="text" class="text"  style="width:6em;" id="createDate_2" name="createDate_2" value="$_form.text('createDate_2',$filter.CreateDate_2)" />
        <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
        </td>
        </tr>
        <tr>
        <td>文件大小介于</td>
        <td colspan="3">
        <input type="text"  style="width:3em;" class="text"  name="size_1" value="$_form.text('size_1',$filter.size_1)"/>
        <select name="SizeUnit_1">
            <option value="K" $_form.selected("SizeUnit_1","K",$Filter.SizeUnit_1)>KB</option>
            <option value="M" $_form.selected("SizeUnit_1","M",$Filter.SizeUnit_1)>MB</option>
            <option value="B" $_form.selected("SizeUnit_1","B",$Filter.SizeUnit_1)>B</option>
            <option value="G" $_form.selected("SizeUnit_1","G",$Filter.SizeUnit_1)>GB</option>
        </select> ~
        <input type="text" class="text"  style="width:3em;"  name="size_2" value="$_form.text('size_2',$filter.size_2)"  />
        <select name="SizeUnit_2">
            <option value="K" $_form.selected("SizeUnit_2","K",$Filter.SizeUnit_2)>KB</option>
            <option value="M" $_form.selected("SizeUnit_2","M",$Filter.SizeUnit_2)>MB</option>
            <option value="B" $_form.selected("SizeUnit_2","B",$Filter.SizeUnit_2)>B</option>
            <option value="G" $_form.selected("SizeUnit_2","G",$Filter.SizeUnit_2)>GB</option>
        </select>
        </td>
    </tr>
    <tr>
        <td>
        排序
        </td>
        <td colspan="3"><select name="orderby">
        <option value="">文件ID</option>
        <option value="Name" $_form.selected('orderby','Name',$filter.order==FileOrderBy.Name)>文件名</option>
        <option value="Size" $_form.selected('orderby','Size',$filter.order==FileOrderBy.Size)>文件大小</option>
        <option value="CreateDate" $_form.selected('orderby','CreateDate',$filter.order==FileOrderBy.CreateDate)>创建时间</option>
        </select>
        <select name="isdesc">
        <option value="true" $_form.selected('isdesc','true',$filter.isdesc==true)>倒序</option>
        <option value="false" $_form.selected('isdesc','false',$filter.isdesc==false)>顺序</option>
        </select>
        每页显示
        <select name="pageSize">
        <option $_form.selected('pageSize','10',$filter.pagesize==10) value="10">10</option>
        <option $_form.selected('pageSize','20',$filter.pagesize==20) value="20">20</option>
        <option $_form.selected('pageSize','50',$filter.pagesize==50) value="50">50</option>
        <option $_form.selected('pageSize','100',$filter.pagesize==100) value="100">100</option>
        <option $_form.selected('pageSize','200',$filter.pagesize==200) value="200">200</option>
        </select>
        </td>
    </tr>
    <tr>
        <td colspan="4"><input type="submit" class="button" value="搜索" name="search" /></td>
    </tr>
</table>
	</form>
	</div>
	<!--[if $DiskFileList.totalrecords>0]-->
	<form action="$_form.action" method="post">
	<div class="DataTable">
	  <h4>文件 <span class="counts">总数: $DiskFileList.totalrecords</span></h4>
        <table>
        <thead>
        <tr>
            <td style="width:20px"> </td>
            <td> 文件名 </td>
            <td> 所有者</td>
            <td> 大小 </td>   
            <td> 上传时间 </td>
            <td> 操作 </td>
        </tr>
        </thead>
        <tbody>
        <!--[loop $file in $DiskFileList with $i]-->
        <tr>
            <td><input type="checkbox" name="diskFileids" value="$file.diskfileid" /></td>
            <td><a href="$url(handler/down)?action=disk&fileid=$File.DiskFileID"><img src="$file.smallicon" alt="" /> $file.Filename </a></td> 
            <td><a class="menu-dropdown" href="javascript:;" onclick="return openUserMenu(this,$file.user.id,'disk')">$file.user.username</a></td>
            <td> $OutputFileSize($file.size) </td>
            <td> $outputdatetime( $file.createDate)</td>
            <td>
            <a  href="$url(handler/down)?action=disk&fileid=$File.DiskFileID"> 下载 </a>
            </td>
         </tr>   
        <!--[/loop]-->
        </tbody>
        </table>
	</div>
	<div class="Actions">
            <input type="checkbox" id="selectAll" />
            <label for="selectAll">全选</label>
            <input class="button" id="Button1" type="submit" name="delete" onclick="return  deleteFiles()" value="删除" />             
    <!--[AdminPager count="$DiskFileList.totalrecords" PageSize="20" /]-->
    </div>
	</form>
   <!--[else]-->
    <div class="NoData">没有符合条件的数据.</div>
    <!--[/if]-->
</div>
<!--[include src="../_foot_.aspx"/]-->
<script type="text/javascript">
var l=new checkboxList("diskFileids","selectAll");

function   deleteFiles()
{
    if(!l.selectCount())
    {
        alert("没有选择任何文件！");
        return false;
    }
    else
    {
        return confirm("确定删除选中的文件吗？");
    }
}

initDatePicker('createDate_1','A0');
initDatePicker('createDate_2','A1');
</script>
</body>
</html>