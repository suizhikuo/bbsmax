<!--[DialogMaster title="编辑日志分类" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="name">分类新名称</label></h3>
            <div class="form-enter">
                <input type="text" class="text" name="name" id="name" value="$CategoryName" maxlength="$CategoryNameMaxLength" />
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <input type="hidden" name="categoryid" value="$_get.id" />
    <button class="button button-highlight" type="submit" name="edit" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
