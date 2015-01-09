<!--[DialogMaster title="购买$prop.name" width="450"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    
    <div class="clearfix propentry">
        <p class="prop-image"><img src="$prop.IconUrl" alt="" /></p>
        <p class="prop-desc">$prop.Description</p>
        <!--[if $AutoUse]-->
        <p class="prop-note"><strong>提示:</strong> 本道具一次只能购买一个，并且在购买后将自动使用，所以购买后将不会在您的道具列表中看到此道具。</p>
        <!--[/if]-->
        <p class="prop-info">
            单价: $PriceName <strong class="numeric">$Prop.Price</strong> $PriceUnit,
            占用空间: <strong class="numeric">$Prop.PackageSize</strong>.
        </p>
        <p class="prop-info">
            库存: <strong class="numeric">$prop.TotalNumber</strong>.
        </p>
    </div>
    

        <div class="dialogform propbuyform">
            <div class="formrow">
                <h3 class="label">购买数量</h3>
                <div class="form-enter">
                    
                    <input type="text" name="count" id="count" class="text number" value="$_form.text('count', 1)" onkeyup="showinfo(this)" <!--[if $AutoUse]-->disabled="disabled"<!--[/if]--> />
                    <%--
                    <span class="numerictextbox">
                        <input type="text" name="count" id="count" class="text number" value="$_form.text('count', 1)" onkeyup="showinfo(this)" />
                        <span class="trigger">
                            <a class="plus" href="javascript:;" onclick="setCount(true);return false;">+</a>
                            <a class="minus" href="javascript:;" onclick="setCount(false);return false;">-</a>
                        </span>
                    </span>
                    --%>
                </div>
            </div>
            <div class="formrow">
                所需 $PriceName <strong id="price" class="numeric">$Prop.Price</strong> $PriceUnit,
                所需容量 <strong id="size" class="numeric">$Prop.PackageSize</strong>.
            </div>
            <div class="formrow">
                可用 $PriceName <strong class="numeric">$UserPoint</strong> $PriceUnit,
                可用容量 <strong class="numeric">$PackageSpace</strong>.
            </div>
        </div>

        <%-- 
        <div class="prop-statschart">
            <div class="statschart-money">
                <div class="statschart"><div style="height:100%;" id="divmoney"><div>-</div></div></div>
                $PriceName
            </div>
            <div class="statschart-bag">
                <div class="statschart"><div id="divspace" style="height:0%;"><div>-</div></div></div>
                容量
            </div>
        </div>
        --%>


</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" accesskey="y" type="submit" name="buy" title="确认"><span>$_if($AutoUse, '购买并立即使用', '购买')(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
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
        var needPoint = $Prop.Price * count;

        e.value = e.value.replace(/[^\d]/g, '');
        if (e.value == '') {
            $('price').innerHTML = $Prop.Price + '';
        }
        else {
            $('price').innerHTML = needPoint + '';
            $('size').innerHTML = needSpace + '';
        }
        /*
        var divm, divs;
        divm = $("divmoney");
        divs = $("divspace");
        var temp = needSpace + usedSpace;
        if (temp > allSpace) temp = allSpace;
        temp = Math.floor(temp / allSpace * 100);
        if (temp > 100) temp = 100;
        if (temp < 0) temp = 0;
        setStyle(divs, { height: temp + "%" });

        temp = needPoint;
        if (temp > myPoint) temp = myPoint;
        temp = Math.floor(100 - temp / myPoint * 100);
        if (temp > 100) temp = 100;
        if (temp < 0) temp = 0;
        setStyle(divm, { height: temp + "%" });
        */
    }
    showinfo($("count"));
    /*
    function setCount(isAdd) {
        var v = parseInt($('count').value);
        if (isAdd) {
            if (v > 0)
                $('count').value = v + 1;
            else
                $('count').value = 1;
        } else {
            if (v > 0)
                $('count').value = v - 1;
            else
                $('count').value = 0;
        }
    }
    */
</script>
<!--[/place]-->
<!--[/dialogmaster]-->