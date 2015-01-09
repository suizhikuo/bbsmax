<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>帖子图标管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
var l;
addPageEndEvent(
function(){
l=new checkboxList( "deleteposticonids","deleteAll" );
}
);

function batchDelete()
{
    if(l.selectCount()==0)
    {
        alert("请选择要删除的图标");
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
    <div class="Help">
    <p style="color:Red">
    <!--[if $IsEnable == false]-->
    系统当前设置已经关闭帖子图标功能，要使帖子图标生效请 <a href="setting-posticon.aspx?enable=true">开启帖子图标功能</a> 
    <!--[else]-->
    系统当前设置已经开启帖子图标功能，要使帖子图标失效请 <a href="setting-posticon.aspx?enable=false">关闭帖子图标功能</a> 
    <!--[/if]-->
    </p>
    </div>
	<div class="PageHeading">
    <h3>帖子图标管理</h3>
    <div class="ActionsBar">
    <!--[if $IsEnable == false]-->
        <a href="setting-posticon.aspx?enable=true"><span>开启帖子图标功能</span></a>
    <!--[else]-->
        <a href="setting-posticon.aspx?enable=false"><span>关闭帖子图标功能</span></a>
    <!--[/if]-->
    </div>
    </div>
	<form action="$_form.action" method="post">
	<div class="DataTable">
        <table id="onlineiconlist">
        <thead>
        <tr>
            <td>&nbsp;</td>
            <td>序号</td>
            <td>图片</td>
            <td>图片地址</td>
            <td>&nbsp;</td>
        </tr>
        </thead>
        <tbody>
        <!--[loop $icon in $PostIconList with $i]-->
        <!--[error line="$i"]-->
        <tr class="ErrorMessage" id="error$icon.iconid">
            <td colspan="5" class="Message"><div class="Tip Tip-error">图标地址不能为空</div></td>
        </tr>
        <tr class="ErrorMessageArrow" id="errorarray$icon.iconid">
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td><!--[if $HasError("iconurl")]--><div class="TipArrow">  </div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td>&nbsp;</td>
        </tr>
        <!--[/error]-->
        <tr id="row-$icon.iconid">
            <td>
                <!--[if !$icon.isnew]-->
                <input type="checkbox" id="deleteposticonids" name="deleteposticonids" value="$icon.iconid" />
                <!--[/if]-->
                <input type="hidden" name="posticonids" value="$icon.iconid" />
            </td>
            <td><input type="text" class="text" name="sortorder.$icon.IconID" value="$icon.sortorder" /></td>
            <td><img alt="" id="image_$icon.iconid" src="$icon.iconurl" /></td>
            <td onclick="i=$icon.iconid;">
                <input type="text" class="text" value="$_form.text('iconurl.$icon.iconid',$icon.iconurlsrc)" id="iconurl.$icon.iconid" name="iconurl.$icon.iconid" />
                <a class="selector-image" title="选择图片" href="javascript:void(openImage('Assets_PostIcon',changeimage))"><img src="$Root/max-assets/images/image.gif" alt="" /></a>
            </td>
            <td>
                <!--[if $icon.isnew]-->
                <a href="javascript:void(cancelNewrow('$icon.iconid'))">取消</a>
                <input type="hidden" name="isnew.$icon.iconid" value="true" />
                <!--[/if]-->
            </td>
        </tr>
        <!--[/loop]-->
        <tr id="newrow">
            <td><input type="hidden" value="{0}" id="newposticons" name="newposticons" /></td>
            <td><input type="text" class="text" name="sortorder.new.{0}" value="0" /></td>
            <td><img alt="" src="" id="img.new.{0}" style="display:none;" /></td>
            <td>
                <input type="text" class="text" id="iconurl.new.{0}" name="iconurl.new.{0}" />
                <a class="selector-image" title="选择图片" href="javascript:void(openImage('Assets_PostIcon',function(r){ $('iconurl.new.{0}').value=r; var img=$('img.new.{0}'); img.src=r.url;img.style.display='';  }))">
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
            <input type="submit" name="savesetting"  class="button" value="保存设置" />
            <span class="edge">|</span>
            <input type="button" class="button" onclick="dt.insertRow();" value="添加帖子图标" />
        </div>
	</div>
	</form>
	<script type="text/javascript">
	var i;
	function changeimage(img)
	{
	    $('image_'+i).src=img.url;
	    $('iconurl.'+i).value=img;
	}
	
    var dt=new DynamicTable("onlineiconlist","newposticons");
	</script>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>