1、 <input onclick="setControlState()" type="radio" id="m1" name="DataClearMode" value="Disabled"  $_form.Checked("DataClearMode","ClearByRows",$DataClearMode==JobDataClearMode.Disabled)  /><label for="m1">不清理历史数据</label>
<br />
2、 <input onclick="setControlState()" type="radio" id="m2" name="DataClearMode" value="ClearByDay"  $_form.Checked("DataClearMode","ClearByRows",$DataClearMode==JobDataClearMode.ClearByDay) /> <label for="m2">每天自动清理&nbsp;&nbsp;<input type="text" class="text" style="width:40px;" id="day1" value="$_form.text('SaveDays',$SaveDays)" name="SaveDays" onkeyup="$('day2').value=this.value" /> 天以前的数据</label>
<br />
3、 <input type="radio" onclick="setControlState()" id="m3" name="DataClearMode"  value="ClearByRows" $_form.Checked("DataClearMode","ClearByRows",$DataClearMode==JobDataClearMode.CombinMode || $DataClearMode==JobDataClearMode.ClearByRows) /><label for="m3">只保留最近<input type="text" class="text" style="width:40px;" value="$_form.text('SaveRows',$SaveRows)" name="SaveRows" /> 条数据
<input type="checkbox" id="chkCombin" onclick="setControlState()" value="true" name="isCombin"  $_form.Checked("DataClearMode","ClearByRows",$DataClearMode==JobDataClearMode.CombinMode) />
但
<input type="text" class="text" style="width:40px;" id="day2" value="$_form.text('SaveDays',$SaveDays)" name="SaveDays" onkeyup="$('day1').value=this.value" />天以内的除外
    </label>

<script type="text/javascript">
function setControlState()
{
    var c1= $("chkCombin");
    c1.disabled =!$("m3").checked;
    $("day1").disabled=c1.checked && !c1.disabled;
    $('day2').disabled=!$("day1").disabled;
}

setControlState();
</script>