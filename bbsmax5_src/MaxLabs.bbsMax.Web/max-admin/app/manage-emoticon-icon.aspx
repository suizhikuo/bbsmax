<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>用户表情管理——$user.userName</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
var l;
addPageEndEvent(
function(){
l=new checkboxList( "iconids","selectAll" );
}
);

function batchDelete()
{
    if(l.selectCount()==0)
    {
        alert("请选择要删除的表情图标");
        return false;
    }
    else
    {
        return confirm("确定删除选中的表情文件吗？");
    }
}

<!-- 表情图标最大宽高 80 -->

</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <div class="PageHeading">
	<h3>用户表情管理——$user.userName</h3>
	<div class="ActionsBar">
        <a class="back" href="$admin/app/manage-emoticon.aspx"><span> 用户表情管理</span></a>
    </div>
    </div>
    <!--[if $EmoticonList.totalRecords > 0]-->
	<form action="$_form.action" method="post">
    	<div class="DataTable">
        <table id="onlineiconlist">
        <thead>
        <tr>
            <td class="CheckBoxHold">&nbsp;</td>
            <td>表情</td>
            <td>快捷方式<span class="request" title="必填项">*</span></td>
            <td>表情地址</td>
        </tr>
        </thead>
        <tbody>
        <!--[loop $icon in $EmoticonList with $i]-->
        <tr>
            <td><input type="checkbox" name="iconids" id="$icon.EmoticonID" value="$icon.EmoticonID" /></td>
            <td><label for="$icon.EmoticonID"><img onload="imageScale(this,80,80)" alt="$icon.shortcut" src="$icon.imageurl" /></label></td>
            <td>$icon.Shortcut</td>
            <td>$icon.imagesrc</td>
        </tr>
        <!--[/loop]-->
        </tbody>
        </table>
    </div>
    <div class="Actions">
        <input type="checkbox" id="selectAll" />
        <label for="selectAll">全选</label>
        <input type="submit" name="delete" class="button" onclick="return batchDelete()" value="删除"/>
    </div>
     <!--[AdminPager count="$EmoticonList.totalRecords" pagesize="20" /]-->
    </form>
    <!--[else]-->
    <div class="NoData">该用户还没有上传表情文件.</div>
    <!--[/if]-->
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
