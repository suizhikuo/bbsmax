<!--[DialogMaster title="导入字段" width="500"]-->
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
            <h3 class="label"><label for="cates">选择模板</label></h3>
            <div class="form-enter">
                <select name="cates" id="cates" onchange="onTypeChange(this.value)">
                    <option value="0">-</option>
                    <!--[loop $cate in $ThreadCateList]-->
                    <optgroup label="$cate.CateName">
                        <!--[loop $model in $GetModels($cate.cateID)]-->
                        <option value="$model.modelID" $_form.selected('cates','$model.modelID','$LoadModelID')>&nbsp;&nbsp;&nbsp;&nbsp;$model.ModelName</option>
                        <!--[/loop]-->
                    </optgroup>
                    <!--[/loop]-->
                </select>
            </div>
            <div class="form-note"><a href="$dialog/threadcatemodelfield.aspx?modelID=$modelID&isdialog=1">手动添加</a></div>
        </div>
        <div class="formrow">
            <div class="form-enter">
                <div class="scroller" style="height:320px;">
                    <ul class="clearfix optionlist">
                    <!--[loop $field in $FieldList]-->
                    <li>
                        <input type="checkbox" id="field_$field.fieldID" name="fieldIDs" value="$field.fieldID" />
                        <label for="field_$field.fieldID">$field.fieldName</label>
                    </li>
                    <!--[/loop]-->
                    </ul>
                </div>
            </div>
            <div class="form-enter">
                <input type="checkbox" id="listSelectAll" />
                <label for="listSelectAll">全选</label>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" id="save" type="submit" name="save" accesskey="y" title="确认"><span>导入(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<script type="text/javascript">
var l =new checkboxList("fieldIDs","listSelectAll");
function onTypeChange(modelid)
{
    var url = 'threadcatemodelfield-loadfield.aspx?isdialog=1&modelid=$modelID&loadmodelid='+modelid;
    location.href=url;
}
</script>
<!--[/place]-->
<!--[/dialogmaster]-->