<!--[DialogMaster title="向 $forUser.Username 打招呼" width="500"]-->
<!--[place id="body"]-->
<form id="form1" action="$_form.action" method="post">
<input type="hidden" name="uid" value="$UserID" />
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">选择动作</h3>
            <div class="form-enter">
                <table>
                    <tr>
                        <td>
                            <input type="radio" name="HailID" id="hail1" value="1" $_form.checked("HailID","1",true) />
                            <label for="hail1"><img src="$Root/max-assets/icon-hail/cyx.gif" alt="踩一下" />踩一下</label>
                        </td>
                        <td>
                            <input type="radio" name="HailID" id="hail2" value="2" $_form.checked("HailID","2") />
                            <label for="hail2"><img src="$Root/max-assets/icon-hail/wgs.gif" alt="握个手" />握个手</label>
                        </td>
                        <td>
                            <input type="radio" name="HailID" id="hail3" value="3" $_form.checked("HailID","3") />
                            <label for="hail3"><img src="$Root/max-assets/icon-hail/wx.gif" alt="微笑" />微笑</label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <input type="radio" name="HailID" id="hail4" value="4" $_form.checked("HailID","4") />
                            <label for="hail4"><img src="$Root/max-assets/icon-hail/jy.gif" alt="加油" />加油</label>
                        </td>
                        <td>
                            <input type="radio" name="HailID" id="hail5" value="5" $_form.checked("HailID","5") />
                            <label for="hail5"><img src="$Root/max-assets/icon-hail/pmy.gif" alt="抛媚眼" />抛媚眼</label>
                        </td>
                        <td>
                            <input type="radio" name="HailID" id="hail6" value="6" $_form.checked("HailID","6") />
                            <label for="hail6"><img src="$Root/max-assets/icon-hail/yb.gif" alt="拥抱" />拥抱</label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <input type="radio" name="HailID" id="hail7" value="7" $_form.checked("HailID","7") />
                            <label for="hail7"><img src="$Root/max-assets/icon-hail/fw.gif" alt="飞吻" />飞吻</label>
                        </td>
                        <td>
                            <input type="radio" name="HailID" id="hail8" value="8" $_form.checked("HailID","8") />
                            <label for="hail8"><img src="$Root/max-assets/icon-hail/nyy.gif" alt="挠痒痒" />挠痒痒</label>
                        </td>
                        <td>
                            <input type="radio" name="HailID" id="hail9" value="9" $_form.checked("HailID","9") />
                            <label for="hail9"><img src="$Root/max-assets/icon-hail/gyq.gif" alt="给一拳" />给一拳</label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <input type="radio" name="HailID" id="hail10" value="10" $_form.checked("HailID","10") />
                            <label for="hail10"><img src="$Root/max-assets/icon-hail/dyx.gif" alt="电一下" />电一下</label>
                        </td>
                        <td>
                            <input type="radio" name="HailID" id="hail11" value="11" $_form.checked("HailID","11") />
                            <label for="hail11"><img src="$Root/max-assets/icon-hail/yw.gif" alt="依偎" />依偎</label>
                        </td>
                        <td>
                            <input type="radio" name="HailID" id="hail12" value="12" $_form.checked("HailID","12") />
                            <label for="hail12"><img src="$Root/max-assets/icon-hail/ppjb.gif" alt="拍拍肩膀" />拍拍肩膀</label>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="note">内容</label></h3>
            <div class="form-enter">
                <textarea name="Note" id="note" cols="30" rows="3"></textarea>
            </div>
        </div>
        <!--[if $TheNotify==null]-->
        <!--[validateCode actionType="$validateActionName"]-->
        <div class="formrow">
        <h3 class="label"><label for="validatecode">验证码</label></h3>
            <div class="form-enter">
                <input type="text" class="text validcode" name="$inputName" $_if($disableIme,'style="ime-mode:disabled;"') id="validatecode" tabindex="4" autocomplete="off" />
                <span class="captcha">
                    <img alt="" src="$imageurl" title="看不清,点击刷新" onclick="this.src=this.src+'&rnd=' + Math.random();" />
                </span>
                <!--[error name="$inputName"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
            <div class="form-note">$tip</div>
        </div>
        <!--[/validateCode]-->
        <!--[/if]-->
    </div>
</div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="addhail" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->