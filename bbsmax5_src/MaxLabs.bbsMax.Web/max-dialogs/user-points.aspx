<!--[DialogMaster title="修改用户积分" width="700"]-->
<!--[place id="body"]-->
<script type="text/javascript">
var pointCollection=new Array();
<!--[loop $point in $PointList]-->
pointCollection.push({name:"$point.name",maxValue:$point.maxvalue,minValue:$point.minValue,control:"point$point.index",index:$point.index});
<!--[/loop]-->
var err=false;
function generalPoint()
{
//    var value;
//    var expression = "$GeneralPointExpression";
//    for(var p in pointCollection)
//    {
//        expression=expression.replace("p"+(pointCollection[p].index+1),$(pointCollection[p].control).value);
//    }
//    try
//    {
//        err=false;
//        $('totalpoints').innerText=eval(expression);
//    }
//    catch(e)
//    {
//        err=true;
//        $('totalpoints').innerHTML="<font color=\"red\">错误的积分值</span>";
//    }
}

function checkPoints()
{
    for(var p in pointCollection)
    {
        if(pointCollection[p].minValue> parseInt($(pointCollection[p].control).value) || pointCollection[p].maxValue<parseInt($(pointCollection[p].control).value) )
        {
            showAlert("错误的积分值："+pointCollection[p].name);
            return false;
        }
    }
    if(err==true) return false;

    return true;
}
 </script>

<!--#include file="_error_.ascx" -->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">积分修改成功</div>
<!--[/success]-->

<!--#include file="_tab_userinfo_.aspx" tab="point" -->

<form id="form1" onsubmit="return checkPoints()" method="post" action="$_form.action">
<div class="clearfix dialogbody">
    <div class="scroller" style="height:300px;">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">总积分</h3>
            <div class="form-enter">
                <span id="totalpoints">$user.points</span> ($GeneralPointExpression)
            </div>
        </div>
        <!--[loop $point in $PointList]-->
        <div class="formrow">
            <h3 class="label"><label for="point$point.index">$point.name</label></h3>
            <div class="form-enter">
                <input id="point$point.index" onkeyup="generalPoint()" maxlength="10" name="point$point.index" type="text" class="text" value="$_form.text('point$point.index',$point.value)" />
                <!--[error name="realname"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
            <div class="form-note">(最小值: $point.MinValue 最大值:$point.MaxValue)</div>
        </div>
        <!--[/loop]-->
    </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" accesskey="s" title="保存" name="updatepoint"><span>保存(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->