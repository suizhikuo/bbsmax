<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>屏蔽搜索引擎</title>
<!--[include src="../_htmlhead_.aspx" /]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
<h3>屏蔽搜索引擎</h3>
	<form action="$_form.action" method="post">
	<div class="Tip Tip-alert">
	    屏蔽访问论坛次数过于频繁的搜索引擎,可以提高论坛性能.
	</div>
	<div class="FormTable">
	<table>
	   <tr>
	    <th>
	        <h4>是否屏蔽以下搜索引擎</h4>
	        <p><input type="radio" id="EnableShield1" name="EnableShield" value="true" $_form.checked('EnableShield','true',$ShieldSpiderSettings.EnableShield) /><label for="EnableShield1">是</label> </p>
	        <p><input type="radio" id="EnableShield2" name="EnableShield" value="false" $_form.checked('EnableShield','false',!$ShieldSpiderSettings.EnableShield)/><label for="EnableShield2">否</label> </p>
	    </th>
	    <td>
	    </td>
	   </tr>
	    <tr>
	        <th>
	            <h4>选择需要屏蔽的搜索引擎</h4>
                <!--[loop $item in $spiderlist]-->
	            <p><input type="checkbox"  name="BannedSpiders" value="$item.Name" $_form.checked("BannedSpiders", "$item.Name",$IsSpiderBanned($item))><label for="$item.Name">$item.Name</label></p>
                <!--[/loop]-->
	        </th>
	        <td>
	            <p>请谨慎选择需要屏蔽的搜索引擎.</p>
	            <p>一旦选择屏蔽,将不再会被所选搜索引擎收录.</p>
	        </td>
	    </tr>
	    <tr class="nohover">
			<th>
			<input type="submit" value="保存设置" class="button" name="savesetting" />
			</th>
			<td>&nbsp;</td>
		</tr>
    </table>
    </div>

    </form>
</div>  
<!--[include src="../_foot_.aspx"/]-->  
</body>
</html>
