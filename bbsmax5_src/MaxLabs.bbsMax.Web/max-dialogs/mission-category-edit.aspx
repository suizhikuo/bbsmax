<!--[DialogMaster title="编辑任务分类" width="400"]-->
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
    <button class="button button-highlight" type="submit" name="edit" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
   <input type="hidden" name="mission-category-id" value="$_get.id" />

</form>
<!--[/place]-->
<!--[/dialogmaster]-->
