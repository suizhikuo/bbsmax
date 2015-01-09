<!--[DialogMaster title="修改板块规则" width="400"]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<form method="post" action="$_form.action">
<div class="clearfix dialogbody">
    <div class="dialogform">
        <div class="formrow">
            <h3 class="label"><label for="readme">版块规则</label></h3>
            <div class="form-enter">
                <textarea name="readme" id="readme" class="text" style=" width:380px; height:130px;" cols="30" rows="5">$_form.text("readme",$Forum.Readme)</textarea>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="ok" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<script type="text/javascript">
var _thePanel = currentPanel;
loadScript("$root/max-assets/nicedit/nicEdit.js", null, function() {
    var _editor = initMiniEditor("readme");
    _thePanel.onSubmit = function() {
        _editor.saveContent();
        return true;
    }
}
 );
</script>
<!--[/place]-->
<!--[/DialogMaster]-->
