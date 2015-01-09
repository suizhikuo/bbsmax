<!--[DialogMaster title="$ActionName" width="500" ]-->
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
            <h3 class="label"><label for="catename">主题名称</label></h3>
            <div class="form-enter">
                <input id="catename" type="text" maxlength="200" value="$_form.text('catename',$CateName)" name="catename" class="text" />
                <!--[error name="CateName"]--><span class="form-tip tip-error">$message</span><!--[/error]-->
            </div>
            <div class="form-note">最多允许20个字节</div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="sortorder">排序</label></h3>
            <div class="form-enter">
                <input id="sortorder" maxlength="3" type="text" value="$_form.text('sortorder',$sortorder)" name="sortorder" class="text number" />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">开启分类主题</h3>
            <div class="form-enter">
                <input type="radio" name="enable" id="enable1" value="1" $_form.checked("enable","1",$IsEnable) />
                <label for="enable1">开启</label>
                <input type="radio" name="enable" id="enable2" value="0" $_form.checked("enable","0",{=$IsEnable==false}) />
                <label for="enable1">关闭</label>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">模板启用</h3>
            <div class="form-enter">
                <ul class="clearfix optionlist">
                    <!--[loop $model in $modelList]-->
                    <li>
                        <input type="checkbox" name="modelIDs" id="model_$model.modelID" value="$model.ModelID" $_form.checked("modelIDs","$model.ModelID",$model.Enable) />
                        <label for="model_$model.modelID"> $model.modelname</label>
                    </li>
                    <!--[/loop]-->
                    <!--[if $modelList.Count == 0]-->
                    <li>
                        <input type="checkbox" disabled="disabled" checked="checked" />
                        默认模板
                    </li>
                    <!--[/if]-->
                </ul>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" id="save" type="submit" name="save" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->