<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>百度SEO设置</title>
<!--[include src="../_htmlhead_.aspx" /]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>设置百度SEO</h3>
	<div class="Help">
    百度论坛收录协议即《互联网论坛收录开放协议》是百度网页搜索制定的论坛内容收录标准,
参见: <a href="http://www.baidu.com/search/pageop.htm" target="_blank">http://www.baidu.com/search/pageop.htm</a>
    </div>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	<table>

		<!--#include file="../_inc/_row_enable.aspx" id="Enable" checked="$BaiduPageOpJopSettings.Enable" title="SiteMap生成功能" note="&nbsp;" -->
		
		<!--#include file="../_inc/_row_textbox.aspx" id="FilePath" value="$BaiduPageOpJopSettings.FilePath" title="SiteMap生成的路径" note="SiteMap文件生成在该目录下(如果论坛目录不在根目录,则应填写网站根目录,此处默认为论坛根目录)" -->

		<tr>
			<th>
			    <h4>网站根目录写权限</h4>
			    <!--[if $HasWritePermission]-->
			    检查写权限通过  
			    <!--[else]-->
			    <span style="color:Red">没有写权限,SiteMap文件无法生成,请给予生成目录可写权限</span>
			    <!--[/if]-->
			</th>
			<td>如果没有写权限，SiteMap文件将无法生成</td>
		</tr>
		
		
		<!--#include file="../_inc/_row_numberbox.aspx" id="UpdateFrequency" value="$BaiduPageOpJopSettings.UpdateFrequency" title="SiteMap生成周期(小时)" note="不能大于50，sitemap 更新周期，以小时为单位。搜索引擎将遵照此周期访问该页面，使页面上的内容更及时地被百度 spider 发现" -->
        
        <!--#include file="../_inc/_row_textbox.aspx" id="Email" value="$BaiduPageOpJopSettings.Email" title="网站负责人Email" note="当有必要时,百度通过这个地址与您联系" -->

		<tr>
			<th>
			    <input type="submit" value="保存设置" class="button" name="savesetting" />
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
