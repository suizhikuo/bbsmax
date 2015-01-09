<!--[DialogMaster title="$_if($propid == -1, '使用道具','使用$PropName')" width="450"]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<form action="$_form.action" method="post" id="propidform">
<!--[if $propid == -1]-->
<div class="clearfix dialogbody">
    <script type="text/javascript">
        var p = currentPanel;
        function submitPropID() {
            p.postToPage(null, getFormData($("propidform")));
        }
    </script>
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="PropID">选择道具</label></h3>
            <div class="form-enter">
                <select name="PropID" id="PropID" onchange="submitPropID()">
                    <option value="-1" $_if($propid == -1, 'selected="selected"')>选择道具</option>
                <!--[loop $prop in $PropList]-->
                    <option value="$prop.PropID" $_if($propid == $prop.propid, 'selected="selected"')>$prop.Name</option>
                <!--[/loop]-->
                </select>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<!--[else]-->
<div class="clearfix dialogbody">
    <input type="hidden" name="propid" value="$propid" />
   
    <div class="clearfix propentry">
        <p class="prop-image"><img src="$prop.IconUrl" alt="" /></p>
        <p class="prop-desc">$prop.Description</p>
        <p class="prop-info">
            可用数量 <strong class="numeric">$Prop.Count</strong>
        </p>
    </div>

    <div class="dialogform propbuyform">
        <div class="formrow">
            <div class="form-enter">
                <!--[if $prop.IsAutoUsePropType == false]-->
                $PropHtml
                <!--[else]-->
                本道具为自动使用道具，您不能手工使用它
                <!--[/if]-->
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <!--[if $prop.IsAutoUsePropType == false]-->
    <button class="button button-highlight" type="submit" name="use" accesskey="u" title="使用"><span>使用(<u>U</u>)</span></button>
    <!--[/if]-->
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<!--[/if]-->
</form>
<!--[/place]-->
<!--[/dialogmaster]-->