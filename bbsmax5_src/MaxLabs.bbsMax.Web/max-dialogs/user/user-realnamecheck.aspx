<!--[DialogMaster title="设置实名认证状态 - $AuthenticUser.user.username" width="400"]-->
<!--[place id="body"]-->

<!--[if $DetectFromAPI]-->
    <!--[else if $DetectState==1]-->
<div class="dialogmsg dialogmsg-error">验证失败: 名字和身份证号码不匹配.</div>
    <!--[else if $DetectState==2]-->
<div class="dialogmsg dialogmsg-error">验证失败: 无此身份证号码.</div>
    <!--[else if $DetectState==3]-->
<div class="dialogmsg dialogmsg-error">验证失败: 链接远程服务器时发生错误.</div>
    <!--[/if]-->
<!--[/if]-->

<form action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">姓名</h3>
            <div class="form-enter">
                $AuthenticUser.realname
                <span style="color:Red;">(该用户$_if($AuthenticUser.Verified,"已通过","未通过")实名认证)</span>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">身份证号码</h3>
            <div class="form-enter">$AuthenticUser.Idnumber</div>
        </div>
        <!--[if $AuthenticUser.HasUploadFile]-->
        <div class="formrow">
            <h3 class="label">用户上传的图片-正面</h3>
            <div class="form-enter">
                <img alt="" src="$dialog/user/manage-realnameattach.aspx?userid=$AuthenticUser.UserID&face=true" height="120" width="180" />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">用户上传的图片-背面</h3>
            <div class="form-enter">
                <img alt="" src="$dialog/user/manage-realnameattach.aspx?userid=$AuthenticUser.UserID&face=false"  height="120px" width="180px"/>
            </div>
        </div>
        <!--[/if]-->

        <!--[if $CanAutoDetect && ! $DetectFromAPI && $AuthenticUser.IsDetect==false]-->
        <!--[else]-->
            <!--[if $AuthenticUser.hasphoto]-->
        <div class="formrow">
            <h3 class="label">公安部返回照片</h3>
            <div class="form-enter">
                <!--[loop $p in $AuthenticUser.Photolist]-->
                <p><img src="$p" alt="" /></p>
                <!--[/loop]-->
            </div>
        </div>
            <!--[/if]-->
        <!--[/if]-->
        <div class="formrow">
            <h3 class="label"><label for="signature">备注</label></h3>
            <div class="form-enter">
                <input type="text" name="remark" value="$AuthenticUser.remark" />
            </div>
            <div class="form-enter">
                <input type="checkbox" name="sendnotify" value="True" checked="checked" id="max_realname_sendnotify"/>
                <label for="max_realname_sendnotify">发送操作通知给该用户</label>
            </div>
            <div class="form-note">备注或未通过认证原因，200字以内</div>
        </div>
    </div>
</div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <!--[if $CanAutoDetect]-->
    <!--[if !$DetectFromAPI && $AuthenticUser.IsDetect==false]-->
    <button class="button button-highlight" type="submit" name="autodetect"><span>从公安部验证</span></button>
    <!--[else]-->
    <button class="button button-highlight" type="submit" name="autodetect"><span>重新验证</span></button>
    <!--[/if]-->
    <!--[/if]-->
    <button class="button button-highlight" type="submit" name="checked" accesskey="y"><span>通过验证(<u>Y</u>)</span></button>
    <button class="button" type="submit" name="unchecked" accesskey="n"><span>不通过验证(<u>N</u>)</span></button>
    <button class="button" type="button" accesskey="c" onclick="panel.close();"><span>关闭(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/DialogMaster]-->