<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>主题鉴定</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
var l;
addPageEndEvent(
function(){
l=new checkboxList("deletejudgementid","deleteAll");
}
);

function batchDelete()
{
    if(l.selectCount()==0)
    {
        alert("请选择要删除的主题鉴定图标");
        return false;
    }
    else
    {
    return confirm("确定删除这些图标吗？");
    }
}
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>主题鉴定管理</h3>
	<form action="$_form.action" method="post">
	<div class="DataTable">
        <table id="judgementTable">
        <thead>
            <tr>
            <th class="CheckBoxHold">&nbsp;</th>
            <th style="width:200px;">图片 </th>
            <th style="width:150px;">描述 <span class="request" title="必填项">*</span></th>
            <th>图片地址 <span class="request" title="必填项">*</span></th>
            <th>&nbsp;</th>
            </tr>
        </thead>
        <tbody id="roleList">
        <!--[loop $j in $JudgementList with $i]-->
            <!--[error line="$i"]-->
        <tr class="ErrorMessage" id="error$j.id">
            <td colspan="5" class="Message"><div class="Tip Tip-error">$message</div></td>
        </tr>
        <tr class="ErrorMessageArrow" id="errorarray$j.id">
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td><!--[if $HasError("description")]--><div class="TipArrow"></div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td><!--[if $HasError("logourl")]--><div class="TipArrow"></div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td>&nbsp;</td>
        </tr>
            <!--[/error]-->
        <tr id="row-$j.id">
            <td>
                $_if(!$j.isnew,'<input type="checkbox" id="deletejudgementid_$j.id" name="deletejudgementid" value="$j.id" />')
                <input type="hidden" name="judgementids" value="$j.id" />
            </td>
            <td><label for="deletejudgementid_$j.id"><img alt="" id="judimage-$j.id" src="$j.logourl" /></label></td>
            <td><input type="text" class="text" name="description.$j.id" value="$_form.text('description.$j.id', $j.description)" /></td>
            <td>
                <input type="text" class="text" id="logourl.$j.id" name="logourl.$j.id" value="$_form.text('logourl.$j.id', $j.logourlsrc)" />
                <a title="选择图片" class="selector-image" href="javascript:void(openImage('Assets_Judgement',function(r){$('logourl.$j.id').value=r;$('judimage-$j.id').src=r.url; }))">
                <img src="$Root/max-assets/images/image.gif" alt="" />
                </a>
            </td>
            <td>
                 $_if($j.isnew,"<a href=\"javascript:void(cancelNewrow($j.id))\"> 取消 </a>")
                 $_if($j.isnew,'<input type="hidden" value="true" name="isnew.$j.id" />')
            </td>
        </tr>
        <!--[/loop]-->
        <tr id="newrow">
            <td><input type="hidden" value="{0}" name="newjudgements" /></td>
            <td><img alt="" src="/" id="logo.new.{0}" style="display:none;" /> </td>
            <td><input type="text" class="text" id="description.new.{0}" name="description.new.{0}" value="" /></td>
            <td>
                <input type="text" class="text" id="logourl.new.{0}" name="logourl.new.{0}" value="" />
                <a title="选择图片" class="selector-image" href="javascript:void(openImage('Assets_Judgement',function(r){$('logourl.new.{0}').value=r; var img= $('logo.new.{0}');img.style.display=''; img.src=r.url;}))">
                <img src="$Root/max-assets/images/image.gif" alt="" />
                </a>
            </td>
            <td><a href="javascript:;" id="deleteRow{0}">取消</a></td>
        </tr>
        </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" id="deleteAll" />
            <label for="deleteAll">全选</label>
            <input type="submit" onclick="return batchDelete()" name="delete" class="button" value="删除选中" />
            <input type="submit" name="savesetting" class="button" value="保存设置" />
            <span class="edge">|</span>
            <input type="button" class="button" onclick="dt.insertRow();" value="添加主题鉴定" />
        </div>
	</div>
	</form>
</div>
<!--[include src="../_foot_.aspx"/]-->
<script type="text/javascript">
var dt=new DynamicTable("judgementTable","newjudgement");
</script>
</body>
</html>