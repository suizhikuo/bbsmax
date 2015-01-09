<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>passport服务设置</title>
<!--[include src="../_htmlhead_.aspx" /]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
   <div class="Help">
    Passport服务端设置页面仅允许创始人进行设置，其他任何用户均没有权限。
    </div>
	<h3>passport服务设置</h3>
	<!--[if !$Restarting]-->
	<!--[if $EnableService]-->
	<div class="PageHeading">
	 <h3>客户端管理 </h3>
	<div class="ActionsBar">
        <a href="$dialog/passport/passport-editclient.aspx" onclick="return openDialog(this.href,refresh)"><span>添加客户端</span></a>
    </div>
    </div>
    <!--[/if]-->
	<form action="$_form.action" method="post">
	<div class="FormTable">
	<table style="margin-bottom:1px;">
		<tr>
			<th>
			    <h4>开启Passport服务</h4>
			    <input type="radio" name="EnablePassportService" value="true" $_form.checked("EnablePassportService","true",$PassportServerSettings.EnablePassportService) id="EnablePassportService1" /><label for="EnablePassportService1">开启</label>
                <input type="radio" name="EnablePassportService" value="false" $_form.checked("EnablePassportService","false",!$PassportServerSettings.EnablePassportService) id="EnablePassportService2" /><label for="EnablePassportService2">关闭</label>
			</th>
			<td>&nbsp;</td>
		</tr>
    </table>
    <!--[if $EnableService]-->
    <table id="linktable">
        <thead>
        <tr>
            <td style="width:50px;">ID</td>
            <td>名称</td>
            <td>URL</td>
            <td>接口文件路径</td>
            <td style="width:90px;">通信密钥</td>
            <td>反向通知数</td>
            <td>最后通讯时间</td>
            <td>反向通知</td>
            <td>延迟（ms）</td>
            <td>状态</td>
            <td style="width:100px">可用操作</td>
        </tr>
        </thead>
        <tbody>
        <!--[loop $driver in $DriverList]-->
        <tr id="client_$driver.client.clientID">
            <td> $driver.client.clientid </td>
            <td> $driver.client.name </td>
            <td><a href="$driver.client.url" target="_blank">$driver.client.url</a> </td>
            <td> $driver.client.apifilepath </td>
            <td> $driver.client.accesskey </td>
            <td> $driver.CurrentInstructCount </td>
            <td> $outputdatetime($driver.LastConnectTime) </td>
            <td>
            <!--[loop $temp in $driver.client.InstructTypes with $i]-->
            <!--[if $i>0]-->
            ,
            <!--[/if]-->
            $temp
            <!--[/loop]-->
            </td>
            <td>$_if($driver.AverageTime>0, $driver.AverageTime.ToString() ,"-")</td>
            <td>
            <!--[if !$driver.IsDisposed]-->
            <span style="color:green">正在通讯</span>
            <!--[else]-->
            <span style="color:Red">已断开</span>
            <!--[/if]-->
            </td>
            <td><a href="$dialog/passport/passport-deleteclient.aspx?clientid=$driver.client.clientID"  onclick="return openDialog(this.href,function(r){if(r)removeElement($('client_'+r));})" >删除</a>
            <a href="$dialog/passport/passport-editclient.aspx?clientID=$driver.client.clientID" onclick="return openDialog(this.href,refresh)">编辑</a>
            </td>
        </tr>
        <!--[/loop]-->
        </tbody>
        </table>
    <!--[/if]-->
    <table>
		<tr>
			<th>
			<input type="submit" value="保存设置" class="button" name="savesetting" />
			</th>
			<td>&nbsp;&nbsp;保存设置后，将会重启当前应用程序！&nbsp; </td>
		</tr> 
	</table>
	</div>
	</form>
	<!--[else]-->
	<div style="font-size:large; color:#FF4377;">
	正在重启当前应用程序，将在<span id="refreshtime">5</span>秒钟后自动刷新页面
	</div>
	<!--[/if]-->
</div>
<script type="text/javascript">
    function generateKey() {
        var chars = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];
        var result = "";
        for (var i = 0; i < 12; i++) {
            var index = Math.round(chars.length * Math.random());
            result += chars[index];
        }
        return result;
    }

    //<!--[if $Restarting]-->
    var reloadSec = 5;
    var label = $("refreshtime");
    var th;
    function reloadPage() {

        reloadSec--;
        if (reloadSec <= 0) {
            refresh();
            window.clearInterval(th);
        }
        else {
            label.innerHTML = reloadSec;
        }
    }

    th = window.setInterval("reloadPage()", 1000);
    //<!--[/if]-->
</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
