<!--[DialogMaster title="修改$CurrentActionName验证码样式" width="600"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="dialogform">
        <div class="scroller" style="height:300px;">
            <!--[loop $ValidateCodeType in $ValidateCodeTypeList with $i]-->
            <div class="formrow">
                <div class="form-enter">
                    <input class="radio" name="validateCodeStyle" id="validateCodeStyle.$i" type="radio" value="$ValidateCodeType.Type" $_form.checked('validateCodeStyle','$ValidateCodeType.Type',$CurrentValidateCodeStyle) />
                    <label for="validateCodeStyle.$i"><span class="vc-name">$ValidateCodeType.Name</span></label>
                    <img src="$GetValidateCodeImageUrl($ValidateCodeType.Type)" alt="" />
                </div>
            </div>
            <!--[/loop]-->
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="savevalidatestyle" accesskey="s" title="保存"><span>保存(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[if $Success]-->
<script type="text/javascript">
    dialog.close();
</script>
<!--[/if]-->
<!--[/place]-->
<!--[/dialogmaster]-->