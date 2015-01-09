<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>默认表情管理——$EmotGroup.GroupName</title>
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
	<h3>默认表情管理 — $EmotGroup.groupname</h3>
	<div class="ActionsBar">
        <a class="back" href="$admin/global/setting-emoticon-group.aspx"><span> 返回分组管理</span></a>
        <a href="$dialog/emoticon-upload.aspx?groupid=$EmotGroup.groupid" onclick="return openDialog(this.href,refresh)"><span>上传表情</span></a>
        <a href="$dialog/emoticon-setshortcut.aspx?groupid=$EmotGroup.groupid" class="item" onclick="return openDialog(this.href,refresh)"><span>批量设置快捷方式</span></a>
    </div>
    </div>
    <!--[if $EmoticonList.count > 0]-->
	<form action="$_form.action" method="post">
    	<div class="DataTable">
        <table id="onlineiconlist">
        <thead>
        <tr>
            <td class="CheckBoxHold">&nbsp;</td>
            <td>排序</td>
            <td>表情</td>
            <td>快捷方式<span class="request" title="必填项">*</span></td>
            <td>表情地址</td>
        </tr>
        </thead>
        <tbody>
        <!--[loop $icon in $PagedEmoticons with $i]-->
        <!--[error line="$i"]-->
        <tr class="ErrorMessage">
            <td colspan="5" class="Message"><div class="Tip Tip-error">$message</div></td>
        </tr>
        <tr class="ErrorMessageArrow">
            <td>&nbsp;</td>
            <td><!--[if $HasError("sortorder")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td>&nbsp;</td>
            <td><!--[if $HasError("shortcut")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td>&nbsp;</td>
        </tr>
        <!--[/error]-->
        <tr>
            <td><input type="checkbox" name="iconids" id="$icon.EmoticonID" value="$icon.EmoticonID" /></td>
            <td><input type="text" style="width:3em;" class="text" name="sortorder.$icon.EmoticonID" value="$icon.sortorder" /></td>
            <td><label for="$icon.EmoticonID"><img id="image_$icon.filename" onload="imageScale(this,80,80)" alt="$icon.filename" src="$icon.imageurl" /></label></td>
            <td><input type="text" name="Shortcut.$icon.EmoticonID" value="$icon.Shortcut" class="text" /></td>
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
        <input type="submit" name="SaveEmotSettings"  class="button" value="保存设置" />
    </div>
     <!--[AdminPager count="$EmoticonList.count" pagesize="20" /]-->
    </form>
    <!--[else]-->
    <div class="NoData">本组下没有表情文件.</div>
    <!--[/if]-->
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
