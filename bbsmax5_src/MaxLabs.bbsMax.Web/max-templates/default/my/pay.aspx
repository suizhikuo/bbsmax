<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$pagetitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/setting.css" />
</head>
<body>
<div class="container section-setting">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main">
        <div class="clearfix main-inner">
            <div class="content">
                <!--#include file="../_inc/_round_top.aspx"-->
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <div class="content-main-inner">
                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/.gif);">充值</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a class="current" href="$url(my/pay)"><span>充值</span></a></li>
                                        <li><a href="$url(my/paylog)"><span>充值记录</span></a></li>
                                    </ul>
                                </div>
                            </div>
                            
                            <form name="formpost" id="formpost" action="$_form.action" target="_blank" onsubmit="setButtonDisable('confirmResult',false)" method="post">
                            <div class="formgroup topupform">
                            
                                <!--[unnamederror]-->
                                <div class="errormsg">$message</div>
                                <!--[/unnamederror]-->
                                
                                <div class="errormsg" id="error" style="display:none"></div>

                                <!--[if $Points.count>1]-->
                                <div class="formrow">
                                    <h3 class="label"><label for="pointtype">选择要充值的积分类型</label></h3>
                                    <div class="formrow-enter">
                                        <select name="pointtype" id="pointtype" onchange="changePointType(this)">
                                            <!--[loop $point in $Points]-->
                                            <option value="$point.type" $_form.selected('pointtype','$point.type')>$point.name</option>
                                            <!--[/loop]-->
                                         </select>
                                    </div>
                                </div>
                                <!--[/if]-->
                                <div class="formrow">
                                    <h3 class="label"><label for="value">请输入要充值的<span id="pointname">$Points[0].name</span>数 <span class="note">(一次最少需要充值<span id="minvalue">0</span>)</span></label></h3>
                                    <div class="formrow-enter">
                                        <input type="hidden" name="currentPointType" id="currentPointType" value="$Points[0].type" />
                                        <input type="text" class="text number" name="payvalue" id="payvalue" value="$_form.text('payvalue')" onkeyup="value=value.replace(/[^\d]/g,'');if(value>2147483647)value=2147483647;setMustPay()" />
                                        <span id="pointunit">$Points[0].UnitName</span>
                                        <span class="surplus">(当前<span id="pointname2">$Points[0].name</span>余额为: <span id="remainvalue">$GetPoint($Points[0].type)</span>)</span>
                                    </div>
                                </div>
                                <div class="formrow">
                                    <h3 class="label">实际应付</h3>
                                    <div class="formrow-enter">
                                        <span class="pay" id="mustpay">0</span> 元
                                        <span class="note" id="paytype">(你选择了支付宝支付)</span>
                                    </div>
                                </div>
                                <div class="formrow">
                                    <h3 class="label">请选择付款方式</h3>
                                    <div class="formrow-enter">
                                        <ul class="clearfix paymentlist">
                                         <!--[if $EnableAlipay]-->
                                            <li id="lialipay" class="selected">
                                                <label for="payment_alipay" class="image">
                                                    <img src="$root/max-assets/payment/alipay.png" alt="支付宝" width="160" height="40" onclick="$('payment_alipay').checked = true;selPayment();" />
                                                </label>
                                                <!--[if $EnableMany]-->
                                                <input id="payment_alipay" class="radio" type="radio" name="payment" checked="checked" onclick="selPayment()" value="1" />
                                                <label for="payment_alipay" class="title">支付宝</label>
                                                <!--[else]-->
                                                    <input type="hidden" name="payment"  value="1" />
                                                <!--[/if]-->
                                            </li>
                                         <!--[/if]-->
                                         <!--[if $EnableTenpay]-->
                                            <li id="litenpay">
                                                <label for="payment_tenpay" class="image">
                                                    <img src="$root/max-assets/payment/tenpay.png" alt="财付通" width="160" height="40" onclick="$('payment_tenpay').checked = true;selPayment();" />
                                                </label>
                                                <!--[if $EnableMany]-->
                                                <input id="payment_tenpay" class="radio" type="radio" name="payment" onclick="selPayment()" value="2" />
                                                <label for="payment_tenpay" class="title">财付通</label>
                                                <!--[else]-->
                                                <input type="hidden" name="payment"  value="2" />
                                                <!--[/if]-->
                                            </li>
                                         <!--[/if]-->
                                         <!--[if $Enable99Bill]-->
                                            <li id="li99bill">
                                                <label for="payment_99bill" class="image">
                                                    <img src="$root/max-assets/payment/99bill.png" alt="快钱" width="160" height="40" onclick="$('payment_99bill').checked = true;selPayment();" />
                                                </label>
                                                <!--[if $EnableMany]-->
                                                <input id="payment_99bill" class="radio" type="radio" name="payment" onclick="selPayment()" value="3" />
                                                <label for="payment_99bill" class="title">快钱</label>
                                                <!--[else]-->
                                                <input type="hidden" name="payment"  value="3" />
                                                <!--[/if]-->
                                            </li>
                                          <!--[/if]-->
                                        </ul>
                                    </div>
                                </div>
                                <script type="text/javascript">
                                 function selPayment() {
                                     var varPayment = document.getElementsByName("payment");
                                     for (i = 0; i < varPayment.length; i++) {
                                         if (varPayment[i].checked) {
                                             switch (varPayment[i].value) {
                                                 case "1":
                                                     setCssClass("lialipay", "selected");
                                                     setCssClass("litenpay", "");
                                                     setCssClass("li99bill", "");
                                                     $('paytype').innerHTML = '(你选择了支付宝支付)';
                                                     break;
                                                 case "2":
                                                     setCssClass("lialipay", "");
                                                     setCssClass("litenpay", "selected");
                                                     setCssClass("li99bill", "");
                                                     $('paytype').innerHTML = '(你选择了财富通支付)';
                                                     break;
                                                 case "3":
                                                     setCssClass("lialipay", "");
                                                     setCssClass("litenpay", "");
                                                     setCssClass("li99bill", "selected");
                                                     $('paytype').innerHTML = '(你选择了快钱支付)';
                                                     break;
                                                 default:
                                                     break;
                                             }
                                         }
                                     }
                                 }

                                 function setCssClass(id, className) {
                                     var element = document.getElementById(id);
                                     if (element != null)
                                         element.className = className;   
                                 }
                                 
                                </script>
                                <div class="formrow formrow-action">
                                    <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" name="confirmResult" id="confirmResult" onclick="return checkInput();  " value="确认充值" /></span></span>
                                </div>
                            </div>
                            </form>
                            
                            <div class="formhelper">
                                <h3>友情提示</h3>
                                <ul>
                                    <!--[if $EnableAlipay]-->
                                    <li>如您暂无支付宝，请注册 <a href="https://www.alipay.com/user/reg_select.htm" target="_blank">https://www.alipay.com/user/reg_select.htm</a></li>
                                    <!--[/if]-->
                                    <!--[if $EnableTenpay]-->
                                    <li>如您暂无财付通，请注册 <a href="https://www.tenpay.com/zft/register_1.shtml" target="_blank">https://www.tenpay.com/zft/register_1.shtml</a></li>
                                    <!--[/if]-->
                                    <!--[if $Enable99Bill]-->
                                    <li>如您暂无快钱，请注册 <a href="https://www.99bill.com/website/signup/websignup.htm" target="_blank">https://www.99bill.com/website/signup/websignup.htm</a></li>
                                    <!--[/if]-->
                                </ul>
                            </div>
                            
                        </div>
                    </div>
                    <!--#include file="_sidebar_setting.aspx"-->
                </div>
                <!--#include file="../_inc/_round_bottom.aspx"-->
            </div>
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>

<div class="dialog" id="infoPanel" style="z-index:51;display:none;">
    <div class="dialog-inner">
        <div class="dialog-content" style="width:400px;">
            <div class="clearfix dialogbody">
                <div class="dialogconfirm">
                    <h3>请在新打开的页面上完成付款.</h3>
                    <p>付款完成前请不要关闭此窗口. 完成付款后请根据您的情况点击下面的按钮.</p>
                </div>
            </div>
            <div class="clearfix dialogfoot">
                <a class="button button-highlight" href="$url(my/paylog)?state=1"><span>已完成充值</span></a>
                <a class="button" href="$url(my/pay)"><span>重新选择付款方式</span></a>
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
    function showMask() {
        var bk = new background();
        var panel = $("infoPanel");
        setVisible(panel, 1);
        moveToCenter(panel);
    }

    function checkInput() {
        var messages = {
        //<!--[loop $pair in $ErrorMessages]-->
            '$pair.Key': ['$pair.Value'],
        //<!--[/loop]-->
            '-1':['','']
        };
        var pointType = $('currentPointType').value;
        var pavValue = $('payvalue').value;
        if (pavValue == '' || minvalues[pointType] > pavValue) {
            $('error').style.display = '';
            $('error').innerHTML = messages[pointType][0];
            location.href = '#error';
            setButtonDisable('confirmResult', false);
            return false;
        }
        else {
            $('error').style.display = 'none';
            return true;
        }
    }

    var moneys = {};
    var points = {};
    var minvalues = {};
    //<!--[loop $rule in $Rules]-->
    //<!--[if $rule.enable]-->
    moneys['$rule.userpointtype'] = $rule.money;
    points['$rule.userpointtype'] = $rule.point;
    minvalues['$rule.userpointtype'] = $rule.minvalue;
    //<!--[/if]-->
    //<!--[/loop]-->
    function setMustPay() {
        var type = $('currentPointType').value;
        var payvalue = $('payvalue').value;
        $('minvalue').innerHTML = minvalues[type];
        if (payvalue == '') {
            $('mustpay').value = '0';
            return;
        }
        var pay = moneys[type] * payvalue / points[type];
        var payStr = pay + '';
        var dotIndex = payStr.indexOf('.');
        if (dotIndex > 0) {
            var temp = payStr.substring(dotIndex + 1);
            if (temp.length > 2) {
                var t = parseInt(temp.substring(0, 2)) + 1;
                pay = parseInt(payStr.substring(0, dotIndex)) + t / 100;
            }
        }
        $('mustpay').innerHTML = pay;
    }
    setMustPay();

    //<!--[if $Points.count>1]-->

    var units = {};
    var remain = {};
    //<!--[loop $point in $Points]-->
    units['$point.type'] = '$point.UnitName';
    remain['$point.type'] = '$GetPoint($point.type)';
    //<!--[/loop]-->

    function changePointType(obj) {
        var type = obj.value;
        var text = obj.options[obj.selectedIndex].text;
        $('pointname').innerHTML = text;
        $('pointname2').innerHTML = text;
        $('pointunit').innerHTML = units[type];
        $('remainvalue').innerHTML = remain[type];
        $('currentPointType').value = type;
        $('minvalue').innerHTML = minvalues[type];
        setMustPay();
    }

    changePointType($('pointtype'));
    //<!--[/if]-->
</script>
</body>
</html>
