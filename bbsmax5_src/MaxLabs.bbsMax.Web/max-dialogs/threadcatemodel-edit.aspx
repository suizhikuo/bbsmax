<!--[DialogMaster title="编辑模板" width="500"]-->
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
        <div class="datatablewrap" style="height:320px;">
            <table class="datatable">
                <tr>
                    <td>启用</td>
                    <td>顺序</td>
                    <td>模板名称</td>
                    <td>操作</td>
                </tr>
                <!--[loop $model in $ModelList]-->
                <tr>
                    <td>
                    <input type="hidden" name="modelIDs" value="$model.ModelID" />
                    <input type="checkbox" name="enableIDs" value="$model.ModelID" $_form.checked("enableIDs","$model.ModelID",$model.Enable) />
                    </td>
                    <td><input id="sortorder_$model.ModelID" style="width:3em;" maxlength="3" type="text" value="$_form.text('sortorder_$model.ModelID',$model.SortOrder)" name="sortorder_$model.ModelID" class="text" /></td>
                    <td><input id="modelname_$model.ModelID" type="text" maxlength="200" value="$_form.text('modelname_$model.ModelID',$model.ModelName)" name="modelname_$model.ModelID" class="text" /></td>
                    <td><a href="threadcatemodel-edit.aspx?action=delete&cateid=$cateID&modelID=$model.modelID">删除</td>
                </tr>
                <!--[/loop]-->
            </table>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" id="save" type="submit" name="save" accesskey="s" title="保存"><span>保存(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->