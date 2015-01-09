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
                                <h3 class="pagecaption-title"><span>积分兑换</span></h3>
                            </div>
                            <div class="formcaption">
                                <p>你可以进行各种不同积分类型之间的兑换, 兑换需要扣除一定的兑换税率.</p>
                            </div>
                            
                            <form action="$_form.action" method="post">
                            <!--[success]-->
                            <div class="successmsg">你已成功完成积分兑换.</div>
                            <!--[/success]-->
                            <!--[unnamederror]-->
                            <div class="errormsg">$Message</div>
                            <!--[/unnamederror]-->
                            <!--[error name="pointvalue"]-->
                            <div class="errormsg">$message</div>
                            <!--[/error]-->
                            <!--[error name="pointtype"]-->
                            <div class="errormsg">$message</div>
                            <!--[/error]-->
                            <!--[error name="targetpointtype"]-->
                            <div class="errormsg">$message</div>
                            <!--[/error]-->
                            
                            <div class="formgroup">
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
                                <div class="formrow pointexchange">
                                    <table>
                                        <tr>
                                            <td class="cell-label">积分类型</td>
                                            <td class="cell-enter">
                                                <select name="pointtype" onchange="pointTypeChange(this.value)">
                                                    <option value="">选择积分类型</option>
                                                    <!--[loop $UserPoint in $CanExchangePoints]-->
                                                    <option value="$UserPoint.Type" $_form.selected('pointtype',$UserPoint.Type)>$UserPoint.Name</option>
                                                    <!--[/loop]-->
                                                </select>
                                            </td>
                                            <td class="cell-arrow" rowspan="3">&nbsp;</td>
                                            <td class="cell-enter">
                                                <select name="targetpointtype" onchange="targetPointTypeChange(this)">
                                                    <option value="">选择积分类型</option>
                                                </select>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="cell-label">积分兑换</td>
                                            <td class="cell-enter">
                                                <input class="text number" type="text" name="pointvalue" value="$_form.text('pointvalue')" onkeyup="value=value.replace(/[^\d]/g,'');if(value==0)value='';setCanExchangeValue();" />
                                            </td>
                                            <td class="cell-enter">
                                                <input id="canexchangevalue" type="text" class="text number" disabled="disabled"  value="" />
                                                <input type="hidden" name="targetpointtypeselectvalue" value="$_form.text('targetpointtypeselectvalue')" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="cell-label">兑换后金额</td>
                                            <td class="cell-enter"><span class="numeric" id="rpoints1">-</span></td>
                                            <td class="cell-enter"><span class="numeric" id="rpoints2">-</span></td>
                                        </tr>
                                    </table>
                                    <p class="form-note">
                                        兑换公式: 目标积分 = 起始积分 &divide; 兑换比例 &times; (1 - 兑换税率) <br />
                                        结果数值, 其小数点后面的数字将会无条件舍去.
                                    </p>
                                    
                                    <div class="pointnote">
                                        兑换比例
                                        <span id="proportion" class="numeric">-</span>;
                                        起始积分必须剩余
                                        <span id="remaining" class="numeric">-</span>;
                                        兑换税率
                                        <span id="taxrate" class="numeric">-</span>;
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
                                    <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" name="exchangepoint" value="确定" /></span></span>
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
//<!--[loop $UserPoint in $CanExchangePoints with $i]-->
    '$UserPoint.Type':[
    //<!--[loop $pointRule in $GetPointExchangeRules($UserPoint.Type) with $n]-->
        ['$pointRule.TargetPointType','$pointRule.TargetUserPoint.Name','$GetExchangeProportion($pointRule.PointType,$pointRule.TargetPointType)','$pointRule.MinRemainingValue','$pointRule.TaxRate']
        //<!--[if $isEnd($UserPoint.Type,$n) == false]-->
        ,
        //<!--[/if]-->
    //<!--[/loop]-->
    ]
    //<!--[if $CanExchangePoints.Count != {=$i+1}]-->
    ,
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
pointTypeChange(document.getElementsByName('pointtype')[0].value);
setTargetPointType();
targetPointTypeChange(document.getElementsByName('targetpointtype')[0]);

function pointTypeChange(pointType)
{
    setCanExchangeValue();
    var target = document.getElementsByName('targetpointtype')[0];
    for(var i = 0;i<target.options.length; i++)
    {
        if(target.options[i].value!='')
        {
            target.remove(i);
            i = 0;
        }
    }
        
    if(pointType == ''|| target.value == '')
    {
        document.getElementById('proportion').innerHTML='';
        document.getElementById('remaining').innerHTML='';
        document.getElementById('taxrate').innerHTML='';
    }
    if(pointType == '')
        return;
    
    for(var i = 0;i<points[pointType].length;i++)
    {
        var varItem = new Option(points[pointType][i][1],points[pointType][i][0]);
        target[target.options.length]=varItem;
    }

}
function targetPointTypeChange(targetPoint)
{
    setCanExchangeValue();
    if(targetPoint.value == '')
    {
        document.getElementById('proportion').innerHTML='';
        document.getElementById('remaining').innerHTML='';
        document.getElementById('taxrate').innerHTML='';
        document.getElementsByName('targetpointtypeselectvalue')[0].value = '';
        return;
    }
    var pointType = document.getElementsByName('pointtype')[0].value;

    if(pointType == '')
        return;
    for(var i = 0;i<points[pointType].length;i++)
    {
        if(points[pointType][i][0]==targetPoint.value)
        {
            document.getElementById('proportion').innerHTML=points[pointType][i][2];
            document.getElementById('remaining').innerHTML=points[pointType][i][3];
            document.getElementById('taxrate').innerHTML=points[pointType][i][4]+'%';
            document.getElementsByName('targetpointtypeselectvalue')[0].value = targetPoint.value;
        }
    }
}
function setTargetPointType()
{
    var targetPointTypeValue = document.getElementsByName('targetpointtypeselectvalue')[0].value;
    if(targetPointTypeValue!='')
     document.getElementsByName('targetpointtype')[0].value = targetPointTypeValue;
}
function setCanExchangeValue()
{
    var pointvalue = document.getElementsByName('pointvalue')[0].value;
    var pointType = document.getElementsByName('pointtype')[0].value;
    var targetPointType = document.getElementsByName('targetpointtype')[0].value;
    var rpoints1 = document.getElementById('rpoints1');
    var rpoints2 = document.getElementById('rpoints2');
    var span = document.getElementById('canexchangevalue');
    if(pointvalue == '' || pointType == '' || targetPointType == '')
    {
        span.value='';
        return;
    }
    
    for(var i = 0;i<points[pointType].length;i++)
    {
        if(points[pointType][i][0]==targetPointType)
        {
            var proportion = points[pointType][i][2];
            var p1 = proportion.split(':')[0];
            var p2 = proportion.split(':')[1];
            var taxRate = points[pointType][i][4];
            
            var temp = (pointvalue * p2 * (100 - taxRate)) / (p1 * 100);
            if(temp < 1)
                temp = 0;
            else
            {
                var index = String(temp).indexOf('.');
                if(index >=0)
                    temp = String(temp).substring(0,index);
            }
            span.value = temp;
            rpoints1.innerHTML = (userpoints[pointType] - pointvalue).toString();
            rpoints2.innerHTML = (userpoints[targetPointType] + parseInt(temp)).toString();
            break;
        }
     }
}
</script>
</div>
</body>
</html>
