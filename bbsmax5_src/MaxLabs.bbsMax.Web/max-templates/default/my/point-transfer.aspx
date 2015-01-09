<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
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
                                <h3 class="pagecaption-title"><span>积分转账</span></h3>
                            </div>
                            <div class="formcaption">
                                <p>你可以将你各种不同类型的积分, 划到任何用户的账下. 转账需要扣除一定的兑换税率.</p>
                            </div>
                            
                            <!--[success]-->
                            <div class="successmsg">积分转账成功</div>
                            <!--[/success]-->
                            <!--[unnamederror]-->
                            <div class="errormsg">$Message</div>
                            <!--[/unnamederror]-->
                            
                            <form action="$_form.action" method="post">
                            <div class="formgroup">
                                <div class="formrow">
                                    <label class="label">将积分转给</label>
                                    <div class="form-enter">
                                        <input type="text" class="text" name="targetusername" value="$_form.text('targetusername')" />
                                    </div>
                                    <!--[error name="targetusername"]-->
                                    <p class="form-tip tip-error">$message</p>
                                    <!--[/error]-->
                                </div>
                                <div class="formrow">
                                    <label class="label">当前余额</label>
                                    <div class="form-enter">
                                        <ul class="clearfix pointlist">
                                            <li>$GeneralPointName: <span class="numeric">$My.Points</span></li>
                                            <!--[EnabledUserPointList]-->
                                            <li>$Name: <span class="numeric">$My.ExtendedPoints[$PointID]</span>$UnitName</li>
                                            <!--[/EnabledUserPointList]-->
                                        </ul>
                                    </div>
                                </div>
                                <div class="formrow">
                                    <label class="label">积分类型</label>
                                    <div class="form-enter">
                                        <select name="pointtype" onchange="setPayValue()">
                                            <option value="">选择积分类型</option>
                                            <!--[loop $UserPoint in $CanTransferPoints]-->
                                            <option value="$UserPoint.Type" $_form.selected('pointtype',$UserPoint.Type)>$UserPoint.Name</option>
                                            <!--[/loop]-->
                                        </select>
                                    </div>
                                    <!--[error name="pointtype"]-->
                                    <div class="form-tip tip-error">$message</div>
                                    <!--[/error]-->
                                </div>
                                <div class="formrow">
                                    <label class="label">转账数量</label>
                                    <div class="form-enter">
                                        <input type="text" class="text number" name="pointvalue" value="$_form.text('pointvalue')" onkeyup="value=value.replace(/[^\d]/g,'');if(value==0)value='';setPayValue();"/>
                                    </div>
                                    <!--[error name="pointvalue"]-->
                                    <div class="form-tip tip-error">$message</div>
                                    <!--[/error]-->
                                    
                                    <div class="pointnote">
                                        交易之后必须至少剩余
                                        <span id="remaining" class="numeric">-</span>;
                                        交易税率
                                        <span id="taxrate" class="numeric">-</span>;
                                        实际将扣除
                                        <span id="point" class="numeric">-</span>;
                                        实际转账后剩余
                                        <span id="point2" class="numeric">-</span>;
                                    </div>
                                    
                                </div>
                                
                                <div class="formrow">
                                    <label class="label">密码确认</label>
                                    <div class="form-enter">
                                        <input class="text" type="password" name="password" value="" />
                                    </div>
                                    <!--[error name="password"]-->
                                    <p class="form-tip tip-error">$message</p>
                                    <!--[/error]-->
                                </div>
                                <div class="formrow formrow-action">
                                    <span class="minbtn-wrap"><span class="btn"><input type="submit" class="button" name="transferpoint" value="确定" /></span></span>
                                </div>
                            </div>
                            </form>
                            
                        </div>
                    </div>
                    <!--#include file="_sidebar_setting.aspx"-->
                </div>
                <!--#include file="../_inc/_round_bottom.aspx"-->
            </div>
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
<script type="text/javascript">
var points = 
{
//<!--[loop $rule in $TransferRules with $i]-->
//<!--[if $rule.CanTransfer]-->
    '$rule.PointType':['$rule.UserPoint.Name','$rule.UserPoint.UnitName','$Rule.MinRemainingValue','$Rule.TaxRate']
    //<!--[if $CanTransferPoints.Count != {=$i + 1}]-->
    ,
    //<!--[/if]-->
//<!--[/if]-->
//<!--[/loop]-->
};
var userpoints = 
{
//<!--[EnabledUserPointList]-->
'$type':$My.ExtendedPoints[$PointID],
//<!--[/EnabledUserPointList]-->
'nouse':0
};
setPayValue();
function setPayValue()
{
    var pointType = document.getElementsByName('pointtype')[0].value;
    var pointSpan = document.getElementById('point');
    var pointSpan2 = document.getElementById('point2');
    var remainingSpan = document.getElementById('remaining');
    var taxrateSpan = document.getElementById('taxrate');
    
    if(pointType == '')
    {
        pointSpan.innerHTML='';
        remainingSpan.innerHTML='';
        taxrateSpan.innerHTML='';
        return;
    }
    
    var minRemainingValue = points[pointType][2];
    var taxRate = points[pointType][3];
    
    remainingSpan.innerHTML = points[pointType][0] + minRemainingValue + points[pointType][1];
    taxrateSpan.innerHTML = taxRate + '%';
    
    var pointValue = document.getElementsByName('pointvalue')[0].value;
    if(pointValue == '')
        pointSpan.innerHTML='';
    
    var temp = pointValue * ((100 + Number(taxRate)) / 100);
    
    if(pointValue * (100 + Number(taxRate)) % 100 != 0)
        temp = temp + 1;
    
    pointValue = temp;
    var index = String(pointValue).indexOf('.');
    if(index >=0)
        pointValue = String(pointValue).substring(0,index);
    pointSpan.innerHTML = points[pointType][0] + pointValue + points[pointType][1];
    pointSpan2.innerHTML = points[pointType][0] + (userpoints[pointType] - parseInt(pointValue)) + points[pointType][1];
}
</script>
</div>
</body>
</html>
