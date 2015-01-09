<!--[DialogMaster title="购买二手$prop.Name" width="450"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<!--[if $DisplayMessage]-->
<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>$Message</h3>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button" type="reset" onclick="panel.close();" accesskey="c" title="关闭"><span>关闭(<u>C</u>)</span></button>
</div>
<!--[else]-->
<div class="clearfix dialogbody">
    <div class="clearfix propentry">
        <p class="prop-image"><img src="$prop.IconUrl" alt="" /></p>
        <p class="prop-desc">$prop.Description</p>
        <p class="prop-info">
            单价: $prop.PriceName <strong class="numeric">$prop.SellingPrice</strong> $prop.PriceUnit (原价:$prop.Price $prop.PriceUnit),
            占用空间: <strong class="numeric">$prop.PackageSize</strong>.
        </p>
        <p class="prop-info">
            库存: <strong class="numeric">$prop.Count</strong>
        </p>
    </div>
    <div class="clearfix propbuy">
        <div class="dialogform propbuyform">
            <div class="formrow">
                <h3 class="label">购买数量</h3>
                <div class="form-enter">
                    <span class="numerictextbox">
                        <input type="text" name="count" id="count" class="text number" value="$_form.text('count', 1)" onkeyup="showinfo(this)" />
                        
                    </span>
                </div>
            </div>
            <div class="formrow">
                所需 $PriceName <strong id="price" class="numeric">$prop.SellingPrice</strong> $PriceUnit,
                所需空间 <strong id="size" class="numeric">$Prop.PackageSize</strong>
            </div>
            <div class="formrow">
                可用 $PriceName <strong class="numeric">$UserPoint</strong> $PriceUnit,
                可用容量 <strong class="numeric">$PackageSpace</strong>.
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="buy" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<!--[/if]-->
</form>
<script type="text/javascript">

    var allSpace = $AllSpaceSize;
    var usedSpace = $UsedSpace;
    var thisSize = $Prop.PackageSize;
    var myPoint = $UserPoint;

    function showinfo(e) {
        var count = parseInt(e.value);
        if (isNaN(count)) count = 0;
        var needSpace = $Prop.PackageSize * count;
        var needPoint = $Prop.SellingPrice * count;

        e.value = e.value.replace(/[^\d]/g, '');
        if (e.value == '') {
            $('price').innerHTML = $Prop.Price + '';
        }
        else {
            $('price').innerHTML = needPoint + '';
            $('size').innerHTML = needSpace + '';
        }


    }
    showinfo($("count"));


</script>
<!--[/place]-->
<!--[/dialogmaster]-->