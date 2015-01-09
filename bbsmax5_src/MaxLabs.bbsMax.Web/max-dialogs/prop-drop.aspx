<!--[DialogMaster title="丢弃$prop.Name" width="450"]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<form action="$_form.action" method="post">
<div class="clearfix dialogbody">
    
    <div class="clearfix propentry">
        <p class="prop-image"><img src="$prop.IconUrl" alt="" /></p>
        <p class="prop-desc">$prop.Description</p>
        <p class="prop-info">
            原价: $prop.PriceName <strong class="numeric">$prop.Price</strong> $prop.PriceUnit,
            数量: <strong class="numeric">$prop.Count</strong>.
        </p>
    </div>

    <div class="dialogform propbuyform">
        <div class="formrow">
            <h3 class="label">丢弃数量</h3>
            <div class="form-enter">
                <input type="text" name="count" id="count" class="text number" value="$_form.text('count', 1)" />
            </div>
        </div>
    </div>

</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="drop" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->