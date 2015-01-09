<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>友情链接管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
var l;
addPageEndEvent(
function(){
l=new checkboxList( "deletelink","deleteAll" );
}
);

function deleteLink(r)
{
if(r)
    for( var i=0;i<r.length;i++)
    {
        //alert(r[i]);
        removeElement( $("link-"+r));
    }
}

function batchDelete()
{
if(l.selectCount()>0)
    return confirm("确定要删除这些链接吗？");
else
    alert("请选择要删除的友情链接");
}
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
    <div class="Content">
    <div class="Help">友情链接序号越小的， 越排列在前面</div>
	
	<h3>友情链接管理 </h3>
	<form action="$_form.action" method="post" id="linkForm">
	<div class="DataTable">
        <table id="linktable">
        <thead>
        <tr>
            <td class="CheckBoxHold">&nbsp;</td>
            <td style="width:40px;">序号</td>
            <td>链接名称<span class="request" title="必填项">*</span></td>
            <td>链接地址<span class="request" title="必填项">*</span></td>
            <td style="width:90px;">图片</td>
            <td>图片地址</td>
            <td>链接描述</td>
            <td style="width:100px">可用操作</td>
        </tr>
        </thead>
        <tbody>
        <!--[loop $l in $LinkList with $i]-->
            <!--[error line="$i"]-->
        <tr class="ErrorMessage" id="error$l.linkid">
            <td colspan="8" class="Message"><div class="Tip Tip-error">$message</div></td>
        </tr>
        <tr class="ErrorMessageArrow" id="errorarray$l.linkid">
            <td colspan="2">&nbsp;</td>
            <td><!--[if $HasError("name")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td><!--[if $HasError("url")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td colspan="4"></td>
        </tr>
            <!--[/error]-->
        <tr id="row-$l.linkid">
            <td>
                <!--[if !$l.isnew]-->
                <input type="checkbox" id="deletelink" name="deletelink" value="$l.linkid" />
                <!--[/if]-->
                <input type="hidden" name="linkids" value="$l.linkid" />
            </td>
            <td><input  style="width:95%;" type="text" class="text" name="index.$l.linkid" value="$_form.text('index.$l.linkid', $l.index)" /></td>
            <td><input type="text" class="text" name="name.$l.linkid" value="$_form.text('name.$l.linkid', $l.name)" /></td>
            <td><input type="text" class="text" name="url.$l.linkid" value="$_form.text('url.$l.linkid', $l.Url)" /></td>
            <td>
                <img $_if( !$l.isimage,'style="display:none;"') id="img.$l.linkid" src="$l.imageurl" alt="" />
            </td>
            <td>
                <input type="text" class="text" style="" id="imageurl.$l.linkid" name="imageurl.$l.linkid" value="$_form.text('imageurl.$l.linkid', $l.imageurlsrc)" />
                <a title="选择图片" class="selector-image" href="javascript:void(browseImage('Assets_LinkIcon','imageurl.$l.linkid','img.$l.linkid'));">
                <img src="$Root/max-assets/images/image.gif" alt="" />
                </a>
            </td>
            <td><input type="text" class="text" name="description.$l.linkid" value="$_form.text('description.$l.linkid', $l.description)" /></td>
            <td>
                <!--[if $l.isnew]-->
                <a href="javascript:void(cancelNewrow('$l.linkid'))">取消</a>
                <input type="hidden" value="true" name="isnew.$l.linkid" />
                <!--[else]-->
                <a href="$dialog/link-delete.aspx?linkid=$l.linkid" onclick="return openDialog(this.href,this,function(){removeElement($('row-$l.linkid'))})">删除</a>
                <!--[/if]-->
            </td>
        </tr>
        <!--[/loop]-->
        <tr id="newrow">
            <td> <input type="hidden" name="newlinkid" value="{0}" /></td>
            <td><input type="text" style="width:95%;" class="text" name="index.new.{0}" value="0" /></td>
            <td><input type="text" class="text" name="name.new.{0}" value="" /></td>
            <td><input type="text" class="text" name="url.new.{0}" value="" /></td>
            <td>
                <img style="display:none" id="img.new.{0}" alt="" />
            </td>
            <td>
                <input type="text" class="text" style="" id="imageurl.new.{0}" name="imageurl.new.{0}" value="" />
                <a title="选择图片" class="selector-image" href="javascript:void(browseImage('Assets_LinkIcon','imageurl.new.{0}','img.new.{0}'));">
                <img src="$Root/max-assets/images/image.gif" alt="" />
                </a>
            </td>
            <td><input type="text" class="text" name="description.new.{0}" value="" /></td>
            <td><a href="javascript:;" id="deleteRow{0}">取消</a></td>
        </tr>
        </tbody>
        </table>
        <div class="Actions">
            <input type="hidden" name="newlinkcount" id="newlinkcount" value="0" />
            <input type="checkbox" id="deleteAll" />
            <label for="deleteAll">全选</label>
            <input type="submit" onclick="return batchDelete()" name="deleteAll" class="button" value="删除选中" /> 
            <input type="submit" name="savesetting" class="button" value="保存设置" />
            <span class="edge">|</span>
            <input type="button" class="button" onclick="dt.insertRow();" value="添加友情链接" />
        </div>
	</div>
	</form>
</div>
<script type="text/javascript">
var dt=new DynamicTable("linktable","newlinkid");
</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>