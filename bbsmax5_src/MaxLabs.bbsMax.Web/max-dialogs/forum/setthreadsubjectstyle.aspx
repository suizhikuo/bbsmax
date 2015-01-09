<!--[DialogMaster title="设置标题样式" subTitle="您共选择了 $ThreadList.count 个主题" width="400" ]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<form method="post" action="$_form.action">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="datatablewrap" style="height:100px;">
            <table class="datatable">
                <thead>
                    <tr>
                        <th>主题</th>
                        <th>作者</th>
                    </tr>
                </thead>
                <tbody>
                <!--[loop $thread in $ThreadList with $i]-->
                    <tr $_if($i%2==0,'class="odd"','class="even"')>
                        <td>$thread.subject</td>
                        <td>$thread.postusername</td>
                    </tr>
                <!--[/loop]-->
                </tbody>
            </table>
        </div>
        <div class="formrow">
            <h3 class="label">字体</h3>
            <div class="form-enter">
                <select name="highlight_family" id="Select1">
                    <option value="" $_form.selected("Select1",'',$fontFamilyValue=='')>默认字体</option>
                    <option value="Arial" $_form.selected("Select1",'',$fontFamilyValue=='Arial')>Arial</option>
                    <option value="宋体" $_form.selected("Select1",'',$fontFamilyValue=='宋体')>宋体</option>
                    <option value="隶书" $_form.selected("Select1",'',$fontFamilyValue=='隶书')>隶书</option>
                    <option value="楷体_GB2312" $_form.selected("Select1",'',$fontFamilyValue=='楷体_GB2312')>楷体</option>
                    <option value="黑体" $_form.selected("Select1",'',$fontFamilyValue=='黑体')>黑体</option>
                    <option value="幼圆" $_form.selected("Select1",'',$fontFamilyValue=='幼圆')>幼圆</option>
                    <option value="微软雅黑" $_form.selected("Select1",'',$fontFamilyValue=='微软雅黑')>微软雅黑</option>
                </select>
                <select name="highlight_size" id="Select2">
                    <option value="" $_form.selected("highlight_size",'',$fontSizeValue=='')>默认大小</option>
                    <option value="12" $_form.selected("highlight_size",'',$fontSizeValue=='12')>12px</option>
                    <option value="13" $_form.selected("highlight_size",'',$fontSizeValue=='13')>13px</option>
                    <option value="14" $_form.selected("highlight_size",'',$fontSizeValue=='14')>14px</option>
                    <option value="15" $_form.selected("highlight_size",'',$fontSizeValue=='15')>15px</option>
                    <option value="16" $_form.selected("highlight_size",'',$fontSizeValue=='16')>16px</option>
                    <option value="17" $_form.selected("highlight_size",'',$fontSizeValue=='17')>17px</option>
                    <option value="18" $_form.selected("highlight_size",'',$fontSizeValue=='18')>18px</option>
                    <option value="19" $_form.selected("highlight_size",'',$fontSizeValue=='19')>19px</option>
                    <option value="20" $_form.selected("highlight_size",'',$fontSizeValue=='20')>20px</option>
                    <option value="21" $_form.selected("highlight_size",'',$fontSizeValue=='21')>21px</option>
                    <option value="22" $_form.selected("highlight_size",'',$fontSizeValue=='22')>22px</option>
                    <option value="23" $_form.selected("highlight_size",'',$fontSizeValue=='23')>23px</option>
                    <option value="24" $_form.selected("highlight_size",'',$fontSizeValue=='24')>24px</option>
                    <option value="25" $_form.selected("highlight_size",'',$fontSizeValue=='25')>25px</option>
                    <option value="26" $_form.selected("highlight_size",'',$fontSizeValue=='26')>26px</option>
                    <option value="27" $_form.selected("highlight_size",'',$fontSizeValue=='27')>27px</option>
                    <option value="28" $_form.selected("highlight_size",'',$fontSizeValue=='28')>28px</option>
                    <option value="29" $_form.selected("highlight_size",'',$fontSizeValue=='29')>29px</option>
                    <option value="30" $_form.selected("highlight_size",'',$fontSizeValue=='30')>30px</option>
                    <option value="31" $_form.selected("highlight_size",'',$fontSizeValue=='31')>31px</option>
                    <option value="32" $_form.selected("highlight_size",'',$fontSizeValue=='32')>32px</option>
                    <option value="33" $_form.selected("highlight_size",'',$fontSizeValue=='33')>33px</option>
                    <option value="34" $_form.selected("highlight_size",'',$fontSizeValue=='34')>34px</option>
                    <option value="35" $_form.selected("highlight_size",'',$fontSizeValue=='35')>35px</option>
                    <option value="36" $_form.selected("highlight_size",'',$fontSizeValue=='36')>36px</option>
                </select>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">文字样式</h3>
            <div class="form-enter">
                <input type="checkbox" id="checkbox1" name="highlight_style" value="b" $_form.checked("Checkbox1",'',$isCheckedStyle_b)  />
                <label for="checkbox1" style="font-weight:bold;">粗体</label>
                <input type="checkbox" id="checkbox2" name="highlight_style" value="i" $_form.checked("Checkbox2",'',$isCheckedStyle_i) />
                <label for="checkbox2" style="font-style:italic;">斜体</label>
                <input type="checkbox" id="checkbox3" name="highlight_style" value="u" $_form.checked("Checkbox3",'',$isCheckedStyle_u) />
                <label for="checkbox3" style="text-decoration:underline;">下划线</label>
                <input type="checkbox" id="checkbox4" name="highlight_style" value="s" $_form.checked("Checkbox4",'',$isCheckedStyle_s) />
                <label for="checkbox4" style="text-decoration:line-through;">删除线</label>
            </div>
         </div>
         <div class="formrow">
            <h3 class="label">文字颜色</h3>
            <div class="form-enter">
                <input class="radio" type="radio" id="Radio1" name="highlight_color" value=""       $_form.checked("Radio1",'',$colorValue=='') /><label for="highlight_color_0">默认</label>&nbsp;
                <input class="radio" type="radio" id="Radio2" name="highlight_color" value="red"    $_form.checked("Radio2",'',$colorValue=='red') /><label for="highlight_color_red" style="color:Red">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio3" name="highlight_color" value="orange" $_form.checked("Radio3",'',$colorValue=='orange') /><label for="highlight_color_orange" style="color:orange">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio4" name="highlight_color" value="yellow" $_form.checked("Radio4",'',$colorValue=='yellow')  /><label for="highlight_color_yellow" style="color:yellow">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio5" name="highlight_color" value="green"  $_form.checked("Radio5",'',$colorValue=='green') /><label for="highlight_color_green" style="color:green">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio6" name="highlight_color" value="cyan"   $_form.checked("Radio6",'',$colorValue=='cyan') /><label for="highlight_color_cyan" style="color:cyan">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio7" name="highlight_color" value="blue"   $_form.checked("Radio7",'',$colorValue=='blue') /><label for="highlight_color_blue" style="color:blue">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio8" name="highlight_color" value="purple" $_form.checked("Radio8",'',$colorValue=='purple')  /><label for="highlight_color_purple" style="color:purple">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio9" name="highlight_color" value="gray"   $_form.checked("Radio9",'',$colorValue=='gray') /><label for="highlight_color_gray" style="color:gray">█</label>
                </div>
         </div>
         <div class="formrow">
            <h3 class="label">背景颜色</h3>
            <div class="form-enter">
                <input class="radio" type="radio" id="Radio10" name="highlight_bgcolor" value=""       $_form.checked("Radio10",'',$backgroundColorValue=='') /><label for="highlight_bgcolor_0">默认</label>&nbsp;
                <input class="radio" type="radio" id="Radio11" name="highlight_bgcolor" value="red"    $_form.checked("Radio11",'',$backgroundColorValue=='red')/><label for="highlight_bgcolor_red" style="color:Red">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio12" name="highlight_bgcolor" value="orange" $_form.checked("Radio12",'',$backgroundColorValue=='orange') /><label for="highlight_bgcolor_orange" style="color:orange">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio13" name="highlight_bgcolor" value="yellow" $_form.checked("Radio13",'',$backgroundColorValue=='yellow') /><label for="highlight_bgcolor_yellow" style="color:yellow">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio14" name="highlight_bgcolor" value="green"  $_form.checked("Radio14",'',$backgroundColorValue=='green') /><label for="highlight_bgcolor_green" style="color:green">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio15" name="highlight_bgcolor" value="cyan"   $_form.checked("Radio15",'',$backgroundColorValue=='cyan') /><label for="highlight_bgcolor_cyan" style="color:cyan">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio16" name="highlight_bgcolor" value="blue"   $_form.checked("Radio16",'',$backgroundColorValue=='blue') /><label for="highlight_bgcolor_blue" style="color:blue">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio17" name="highlight_bgcolor" value="purple" $_form.checked("Radio17",'',$backgroundColorValue=='purple') /><label for="highlight_bgcolor_purple" style="color:purple">█</label>&nbsp;
                <input class="radio" type="radio" id="Radio18" name="highlight_bgcolor" value="gray"   $_form.checked("Radio18",'',$backgroundColorValue=='gray')/><label for="highlight_bgcolor_gray" style="color:gray">█</label>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">有效时间</h3>
            <div class="form-enter">
                <input class="text number" onkeyup="value=value.replace(/[^\d]/g,'');" type="text" name="time" value="" />
                <select name="locktimetype" id="Select3">
                    <option value="1">小时</option>
                    <option value="2">分钟</option>
                    <option value="0">天</option>
                </select>
            </div>
            <div class="form-note">该时间过后，将自动取消标题样式，为空或者0则不自动取消，精确到3分钟.</div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="actionReasonSelect">操作理由</label></h3>
            <div class="form-enter">
                <select name="actionReasonSelect" id="actionReasonSelect" onchange="document.getElementsByName('actionReasonText')[0].value=this.value;">
                    <option value="">自定义</option>
                    <option value="灌水">灌水</option>
                    <option value="广告">广告</option>
                    <option value="奖励">奖励</option>
                    <option value="惩罚">惩罚</option>
                    <option value="好文章">好文章</option>
                    <option value="内容不符">内容不符</option>
                    <option value="重复发帖">重复发帖</option>
                </select>
                <input type="text" class="text" name="actionReasonText" />
            </div>
          </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="ok" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
    <input type="hidden" value="$_form.text('threadids','$threadIDString')" name="threadids" />
</div>
</form>
<script type="text/javascript">
/*
    var styleSet = {};
    styleSet.I = parseInt($('txtI').value);
    styleSet.U = parseInt($('txtU').value);
    styleSet.B = parseInt($('txtB').value);
    styleSet.S = parseInt($('txtS').value);
    styleSet.color = parseInt($('txtColor').value);

    function setCheck(obj, chk) {
        var s = obj.style;
        if (chk) {
            s.borderStyle = 'solid';
            s.borderWidth = '1px';
            s.borderColor = '#aaaaaa';
            s.backgroundColor = 'white';
        }
        else {
            s.border = 'none';
            s.backgroundColor = '';
        }
    }

    function showColorBoard() {
        var c = document.getElementById("foreColor");
        var cp = document.getElementById('colorBoard');
        var s = cp.style;
        s.left = getLeft(c) + 'px';
        s.top = (getTop(c) + c.offsetHeight) + 'px';
        s.display = '';

    }
    function selColor(c) {
        styleSet.color = c;
        checkStyle();
        document.getElementById('colorBoard').style.display = "none";
    }

    function checkStyle() {
        var divB = document.getElementById("divB");
        var divI = document.getElementById("divI");
        var divU = document.getElementById("divU");
        var divS = document.getElementById("divS");
        document.getElementById("txtB").value = styleSet.B ? 'true' : "false";
        document.getElementById("txtI").value = styleSet.I ? 'true' : "false";
        document.getElementById("txtU").value = styleSet.U ? 'true' : "false";
        document.getElementById("txtS").value = styleSet.S ? 'true' : "false";
        document.getElementById("foreColor").style.backgroundColor = styleSet.color;

        setCheck(divB, styleSet.B);
        setCheck(divI, styleSet.I);
        setCheck(divU, styleSet.U);
        setCheck(divS, styleSet.S);
    }
    */
</script>
<!--[/place]-->
<!--[/DialogMaster]-->