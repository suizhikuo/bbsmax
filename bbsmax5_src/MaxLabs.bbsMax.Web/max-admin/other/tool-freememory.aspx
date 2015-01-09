<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>内存整理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>内存整理</h3>
	<div class="Help">
	    <ol>
	    <li>本功能并不是整理整台服务器的内存，本功能只整理BBSMAX占用的内存。</li>
	    <li>本功能不会导致数据丢失。</li>
	    <li>整理内存的过程服务器cpu会有轻微上浮。</li>
	    </ol>
	    <p><strong>适用情况：</strong></p>
	    <ol>
	    <li>bbsmax内存占用过大。</li>
	    </ol>
	</div>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	    <table>
	        <tr>
	            <th>
	                <h4>内存整理</h4>
	                <input type="checkbox" value="true" name="isread" id="isread" />
	                <label for="isread">我已经仔细阅读以上说明，确认整理</label>
	            </th>
	            <td>&nbsp;</td>
	        </tr>
	        <tr class="nohover">
	            <th>
	                <input class="button" type="submit" name="free" value="立即整理内存" />
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
