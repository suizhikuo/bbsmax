<!--[DialogMaster title="新建道具" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="PropType">类型:</label></h3>
            <div class="form-enter">
                <script type="text/javascript">
                var descs = [
                ''
                <!--[loop $type in $PropTypeList]-->
                ,'$type.Description'
                <!--[/loop]-->
                ];
                </script>
                <select id="PropType" onchange="">
                    <option>选择道具类型</option>
                    <!--[loop $type in $PropTypeList]-->
                    <option value="$type">$type.Name</option>
                    <!--[/loop]-->
                </select>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="buy" accesskey="y" title="确认"  onclick="if($('PropType').selectedIndex > 0){var result = $('PropType').options[$('PropType').selectedIndex].value; setValue( result);dialog.close();}"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<script type="text/javascript">
    var thePanel = currentPanel;
    function setValue(v) {
        thePanel.result = v;
    }
</script>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->