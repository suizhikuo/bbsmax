
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>Passport设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
   <div class="Help">
    Passport客户端设置页面仅允许创始人进行设置，其他任何用户均没有权限。
    </div>

	<h3>Passport设置</h3>
	<form id="passportform" action="$_form.action" method="post">
	<div class="FormTable">
	<table style="margin-bottom:1px;">
		<tr>
			<th>
			    <h4>开启Passport对接</h4>
				<p><input type="radio" name="enablepassport" id="enablepassport1" value="true" $_form.checked('enablepassport','true',$passportclientsettings.enablepassport) /> <label for="enablepassport1">开启</label></p>
				<p><input type="radio" name="enablepassport" id="enablepassport2" value="false" $_form.checked('enablepassport','false',$passportclientsettings.enablepassport == false) /> <label for="enablepassport2">关闭</label></p>
			</th>
			<td>
                开启Passport对接功能后， 用户注册、登录、登出等功能将跳转到Passport服务器&nbsp;
            </td>
		</tr>
		<tbody id="passportclientsettings">
		<!--[error name="passportroot"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th><h4>Passport服务器地址</h4>
            <input type="text" class="text" name="passportroot" value="$_form.text('passportroot',$passportclientsettings.passportroot)" />
            <input type="hidden" name="accesskey" value="$passportclientsettings.accesskey" />
            <input type="hidden" name="clientID" value="$passportclientsettings.clientid" />
			</th><td>如果开启Passport对接， 保存设置前请<a href="javascript:void(openTest());">测试服务器连接</a>， 务必保证服务器通讯正常<br />
			请填写完整的URL地址，比如:http://passport.bbsmax.com 或者 https://passport.bbsmax.com
			</td>
		</tr>
		<tr>
        <th>
        <h4>Possport通讯超时时间</h4>
        <input type="text" style="width:40px;" class="text" name="passporttimeout" value="$_form.text('passporttimeout',$passportclientsettings.passporttimeout)" />秒
        </th>
        <td>&nbsp;</td>
		</tr>
        <tr>
        <th>
        <h4>注册Passport客户端</h4>
        <input id="registerClient1" type="radio" name="registerClient" $_form.checked('registerclient','true') value="true" />
        <label for="registerClient1">向Passport服务器注册数据同步客户端</label>
            <br />
        <input id="registerClient2" type="radio" name="registerClient" $_form.checked('registerclient','false',true) value="false" />
        <label for="registerClient2">已经注册过客户端，不需要重新注册</label>
        </th>
        <td>如果之前有注册过，请不要重复注册。</td>
		</tr>
		</tbody>
		<tbody id="registion">
		<tr>
        <th>
        <h4>Possport创始人用户名</h4>
        <input type="text" class="text" name="passportOwnerUsername" value="$_form.text('passportOwnerUsername')" />
        </th>
        <td>&nbsp;</td>
		</tr>
		<tr>
        <th>
        <h4>Possport创始人密码</h4>
        <input type="password" class="text" name="passportOwnerPassword" value="$_form.text('passportOwnerPassword')" />
        </th>
        <td>&nbsp;</td>
		</tr>

		<tr>
        <th colspan="2"><font color="red">注意：注册Passport客户端需要重启当前论坛应用程序！</font></th>
		</tr>

		</tbody>
	</table>
    <table>
		<tr>
			<th>
			<input type="submit" value="保存设置" class="button"  name="savesetting" />
			<input type="button" id="btnTest" value="测试" class="button" onclick="openTest()" />
			</th>
			<td>&nbsp;</td>
		</tr>
	</table>
</div>
	</form>
</div>

<script type="text/javascript">
    function openTest() {
        var url = "$dialog/passport-test.aspx";
        postToDialog({ formId: "passportform", url: url });
    }

    initDisplay("enablepassport", [
     { value: "true", display: true, id: 'passportclientsettings' }
    , { value: "false", display: false, id: 'passportclientsettings' }
    , { value: "false", display: false, id: "btnTest" }
    , { value: "true", display: true, id: "btnTest" }
    ]);

     initDisplay("registerClient", [
     { value: "true", display: true, id: 'registion' }
    , { value: "false", display: false, id: 'registion' }
    ]);
</script>

<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
