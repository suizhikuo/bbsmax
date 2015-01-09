<!--[DialogMaster title="添加模板" width="550"]-->
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
            <h3 class="label"><label for="modelname">模板名称</label></h3>
            <div class="form-enter">
                <input id="modelname" type="text" maxlength="200" value="$_form.text('modelname')" name="modelname" class="text" />
                <!--[error name="modelname"]--><span class="form-tip tip-error">$message</span><!--[/error]-->
            </div>
            <div class="form-note">最多允许20个字节</div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="sortorder">排序</label></h3>
            <div class="form-enter">
               <input id="sortorder" style="width:3em;" maxlength="3" type="text" value="$_form.text('sortorder')" name="sortorder" class="text" />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">启用</h3>
            <div class="form-enter">
                <input type="radio" name="enable" id="enable1" value="1" $_form.checked("enable","1","1") />
                <label for="enable1">开启</label>
                <input type="radio" name="enable" id="enable2" value="0" $_form.checked("enable","0") />
                <label for="enable2">关闭</label>
            </div>
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