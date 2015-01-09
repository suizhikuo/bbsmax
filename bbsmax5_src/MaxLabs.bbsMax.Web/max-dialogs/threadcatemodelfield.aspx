<!--[DialogMaster title="$ActionName" width="550"]-->
<!--[place id="body"]-->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">操作成功</div>
<!--[/success]-->
<!--[unnamederror]-->
<div class="dialogmsg dialogmsg-error">$Message</div>
<!--[/unnamederror]-->
<form id="form1" method="post" action="$_form.action">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="fieldname">名称</label></h3>
            <div class="form-enter">
                <input id="fieldname" type="text" maxlength="200" value="$_form.text('fieldname','$Field.FieldName')" name="fieldname" class="text" />
                <!--[error name="modelname"]--><span class="form-tip tip-error">$message</span><!--[/error]-->
            </div>
            <div class="form-enter">
                <a href="$dialog/threadcatemodelfield-loadfield.aspx?modelID=$modelID&isdialog=1">从其它模板导入</a>
            </div>
            <div class="form-note">最多允许20个字节</div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="description">描述</label></h3>
            <div class="form-enter">
                <input type="text" maxlength="200" value="$_form.text('description','$Field.Description')" name="description" id="description" class="text" />
                <!--[error name="description"]--><span class="form-tip tip-error">$message</span><!--[/error]-->
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="sortorder">排序</label></h3>
            <div class="form-enter">
                <input id="sortorder" style="width:3em;" maxlength="3" type="text" value="$_form.text('sortorder',$field.SortOrder)" name="sortorder" class="text" />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">启用</h3>
            <div class="form-enter">
                <input type="radio" name="enable" id="enable1" value="1" $_form.checked("enable","1",$field.enable) /> <label for="enable1">开启</label>
                <input type="radio" name="enable" id="enable2" value="0" $_form.checked("enable","0",{=$field.enable==false}) /> <label for="enable2">关闭</label>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">默认查询</h3>
            <div class="form-enter">
                <input type="radio" name="Search" id="Search1" value="1" $_form.checked("Search","1",$field.Search) /> <label for="Search1">开启</label>
                <input type="radio" name="Search" id="Search2" value="0" $_form.checked("Search","0",{=$field.Search==false}) /> <label for="Search2">关闭</label>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">高级查询</h3>
            <div class="form-enter">
                <input type="radio" name="AdvancedSearch" id="AdvancedSearch1" value="1" $_form.checked("AdvancedSearch","1",$field.AdvancedSearch) /> <label for="AdvancedSearch1">开启</label>
                <input type="radio" name="AdvancedSearch" id="AdvancedSearch2" value="0" $_form.checked("AdvancedSearch","0",{=$field.AdvancedSearch==false}) /> <label for="AdvancedSearch2">关闭</label>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">帖子列表显示</h3>
            <div class="form-enter">
                <input type="radio" name="DisplayInList" id="DisplayInList1" value="1" $_form.checked("DisplayInList","1",$field.DisplayInList) /> <label for="DisplayInList1">是</label>
                <input type="radio" name="DisplayInList" id="DisplayInList2" value="0" $_form.checked("DisplayInList","0",{=$field.DisplayInList==false}) /> <label for="DisplayInList2">否</label>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">必填</h3>
            <div class="form-enter">
                <input type="radio" name="MustFilled" id="MustFilled1" value="1" $_form.checked("MustFilled","1",$field.MustFilled) /> <label for="MustFilled1">是</label>
                <input type="radio" name="MustFilled" id="MustFilled2" value="0" $_form.checked("MustFilled","0",{=$field.MustFilled==false}) /> <label for="MustFilled2">否</label>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="fieldtype">字段类型</label></h3>
            <div class="form-enter">
                <select name="fieldtype" id="fieldtype" onchange="onTypeChange(this.value)" <!--[if $isEdit]--> disabled="disabled"<!--[/if]-->>
                <option value="0">请选择字段类型</option>
                <!--[loop $type in $ExtendedFieldTypeList]-->
                <option value="$type.TypeName" $_form.selected("fieldtype","$type.TypeName","$field.FieldType")>$type.DisplayName</option>
                <!--[/loop]-->
                </select>
            </div>
        </div>
        <!--[ajaxpanel id="ap_fieldtypedatas" idonly="true"]-->
        <!--[if $CurrentField.FieldTypeName!=""]-->
        <div class="datatablewrap">
        <table class="datatable">
        <!--[ExtendedFieldType type="$CurrentField.FieldTypeName"]-->
        <!--[if $settingable]-->
        <!--[load src="$BackendControlSrc" field="$CurrentField" /]-->
        <!--[/if]-->
        <!--[/ExtendedFieldType]-->
        </table>
        </div>
        <!--[/if]-->
        <!--[/ajaxpanel]-->
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" id="save" type="submit" name="save" accesskey="s" title="保存"><span>保存(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<script type="text/javascript">
function onTypeChange(type)
{
    var url = 'threadcatemodelfield.aspx?modelid=$modelID&isdialog=1&type='+type;
    ajaxRender(url,'ap_fieldtypedatas',ajaxCallback);
}
</script>
<!--[/place]-->
<!--[/dialogmaster]-->