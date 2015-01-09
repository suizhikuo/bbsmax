<!--[DialogMaster title="定时操作列表" width="500"]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<form action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="timescopetype">选择类型</label></h3>
            <div class="form-enter">
                <select id="timescopetype" name="timescopetype">
                    <option value="0" $_form.selected("timescopetype","0",true)>自定义某一天</option>
                    <option value="1" $_form.selected("timescopetype","1")>每天</option>
                    <option value="2" $_form.selected("timescopetype","2")>每周</option>
                    <option value="3" $_form.selected("timescopetype","3")>每月</option>
                </select>
            </div>
        </div>
        <div class="formrow" id="defineday">
            <h3 class="label"><label for="definedate">指定日期</label></h3>
            <div class="form-enter">
                <span class="datepicker">
                    <input id="definedate" name="definedate" class="text" type="text" value='$_form.text("definedate")' />
                    <a title="选择日期" id="A0" href="javascript:void(0)">选择日期</a>
                </span>
            </div>
        </div>
        <div class="formrow" id="week" style="display:none;">
            <h3 class="label">选择星期</h3>
            <div class="form-enter">
                <ul class="clearfix optionlist optionlist-lite">
                    <li><input type="checkbox" id="week1" name="week" value="1" /> <label for="week1">周一</label></li>
                    <li><input type="checkbox" id="week2" name="week" value="2" /> <label for="week2">周二</label></li>
                    <li><input type="checkbox" id="week3" name="week" value="3" /> <label for="week3">周三</label></li>
                    <li><input type="checkbox" id="week4" name="week" value="4" /> <label for="week4">周四</label></li>
                    <li><input type="checkbox" id="week5" name="week" value="5" /> <label for="week5">周五</label></li>
                    <li><input type="checkbox" id="week6" name="week" value="6" /> <label for="week6">周六</label></li>
                    <li><input type="checkbox" id="week7" name="week" value="0" /> <label for="week7">周日</label></li>
                </ul>
            </div>
        </div>
        <div class="formrow" id="month" style="display:none;">
            <h3 class="label">选择日期</h3>
            <div class="form-enter">
                <ul class="clearfix optionlist optionlist-lite">
                    <li><input type="checkbox" id="month1" name="month" value="1" /> <label for="month1">1号</label>    </li>
                    <li><input type="checkbox" id="month2" name="month" value="2" /> <label for="month2">2号</label>    </li>
                    <li><input type="checkbox" id="month3" name="month" value="3" /> <label for="month3">3号</label>    </li>
                    <li><input type="checkbox" id="month4" name="month" value="4" /> <label for="month4">4号</label>    </li>
                    <li><input type="checkbox" id="month5" name="month" value="5" /> <label for="month5">5号</label>    </li>
                    <li><input type="checkbox" id="month6" name="month" value="6" /> <label for="month6">6号</label>    </li>
                    <li><input type="checkbox" id="month7" name="month" value="7" /> <label for="month7">7号</label>    </li>
                    <li><input type="checkbox" id="month8" name="month" value="8" /> <label for="month8">8号</label>    </li>
                    <li><input type="checkbox" id="month9" name="month" value="9" /> <label for="month9">9号</label>    </li>
                    <li><input type="checkbox" id="month10" name="month" value="10" /> <label for="month10">10号</label></li>
                    <li><input type="checkbox" id="month11" name="month" value="11" /> <label for="month11">11号</label></li>
                    <li><input type="checkbox" id="month12" name="month" value="12" /> <label for="month12">12号</label></li>
                    <li><input type="checkbox" id="month13" name="month" value="13" /> <label for="month13">13号</label></li>
                    <li><input type="checkbox" id="month14" name="month" value="14" /> <label for="month14">14号</label></li>
                    <li><input type="checkbox" id="month15" name="month" value="15" /> <label for="month15">15号</label></li>
                    <li><input type="checkbox" id="month16" name="month" value="16" /> <label for="month16">16号</label></li>
                    <li><input type="checkbox" id="month17" name="month" value="17" /> <label for="month17">17号</label></li>
                    <li><input type="checkbox" id="month18" name="month" value="18" /> <label for="month18">18号</label></li>
                    <li><input type="checkbox" id="month19" name="month" value="19" /> <label for="month19">19号</label></li>
                    <li><input type="checkbox" id="month20" name="month" value="20" /> <label for="month20">20号</label></li>
                    <li><input type="checkbox" id="month21" name="month" value="21" /> <label for="month21">21号</label></li>
                    <li><input type="checkbox" id="month22" name="month" value="22" /> <label for="month22">22号</label></li>
                    <li><input type="checkbox" id="month23" name="month" value="23" /> <label for="month23">23号</label></li>
                    <li><input type="checkbox" id="month24" name="month" value="24" /> <label for="month24">24号</label></li>
                    <li><input type="checkbox" id="month25" name="month" value="25" /> <label for="month25">25号</label></li>
                    <li><input type="checkbox" id="month26" name="month" value="26" /> <label for="month26">26号</label></li>
                    <li><input type="checkbox" id="month27" name="month" value="27" /> <label for="month27">27号</label></li>
                    <li><input type="checkbox" id="month28" name="month" value="28" /> <label for="month28">28号</label></li>
                    <li><input type="checkbox" id="month29" name="month" value="29" /> <label for="month29">29号</label></li>
                    <li><input type="checkbox" id="month30" name="month" value="30" /> <label for="month30">30号</label></li>
                    <li><input type="checkbox" id="month31" name="month" value="31" /> <label for="month31">31号</label></li>
                </ul>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="timespan">时间范围</label></h3>
            <div class="form-enter">
                <input class="text" id="timespan" name="timespan" type="text" />
            </div>
            <div class="form-note">(时间范围格式HH:MM-HH:MM 如:10点20到12点为 <strong>10:20-12:00</strong>,为空则表示范围为全天,从0点到24点)</div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <input type="hidden" name="type" value="$_form.text('type','$_get.type')" />
    <button class="button button-highlight" type="submit" name="add" accesskey="a" title="添加"><span>添加(<u>A</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<script type="text/javascript">
initDisplay('timescopetype', [
 { value: '0', display: true, id: 'defineday' }
, { value: '0', display: false, id: 'week' }
, { value: '0', display: false, id: 'month' }
, { value: '1', display: false, id: 'defineday' }
, { value: '1', display: false, id: 'week' }
, { value: '1', display: false, id: 'month' }
, { value: '2', display: false, id: 'defineday' }
, { value: '2', display: true, id: 'week' }
, { value: '2', display: false, id: 'month' }
, { value: '3', display: false, id: 'defineday' }
, { value: '3', display: false, id: 'week' }
, { value: '3', display: true, id: 'month' }
]);
initDatePicker('definedate', 'A0');
</script>
<!--[/place]-->
<!--[/DialogMaster]--> 