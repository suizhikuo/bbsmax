<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>重新启动</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>重新启动</h3>
	<div class="Help">
	    <ol>
	    <li>本功能并不是重新启动服务器或者IIS，本功能只重启BBSMAX应用程序。</li>
	    <li>本功能不会导致任何数据丢失，但会清空在线列表。</li>
	    <li>和重新启动服务器的情况一样，在本页面重启后，每个页面的第一次访问速度较慢（2-3秒才能打开），这是因为asp.net页面首次运行需要编译。</li>
	    <li>和重新启动服务器的情况一样，刚重启后因为没有任何缓存，服务器cpu会有轻微上浮，稍后会慢慢恢复。</li>
	    </ol>
	    <p><strong>适用情况：</strong></p>
	    <ol>
	    <li>缓存数据不正确，例如发表帖子后帖子数统计并未增加。</li>
	    <li>人为修改了数据库中的值，但由于数据缓存，这些改变没有体现到程序当中。</li>
	    <li>保存了某些设置但并未生效。</li>
	    <li>其他不正常的情况均可以尝试重启。</li>
	    </ol>
	</div>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	    <table>
	        <tr>
	            <th>
	                <h4>重新启动应用程序</h4>
	                <input type="checkbox" value="true" name="isread" id="isread" />
	                <label for="isread">我已经仔细阅读以上说明，确认重启</label>
	            </th>
	            <td>&nbsp;</td>
	        </tr>
	        <tr class="nohover">
	            <th>
	                <input class="button" type="submit" name="restart" value="立即重新启动" />
	            </th>
	            <td>&nbsp;</td>
	        </tr>
	    </table>
	</div>
	</form>
</div>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
