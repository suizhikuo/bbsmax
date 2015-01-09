<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>关键字过滤设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript" src="$root/max-assets/javascript/base64.js"></script>
<script type="text/javascript">
    function beforeSubmit() {
        document.getElementById('BannedKeywords').value = encode64(document.getElementById('BannedKeywords_View').value);
        document.getElementById('ReplaceKeywords').value = encode64(document.getElementById('ReplaceKeywords_View').value);
        document.getElementById('ApprovedKeywords').value = encode64(document.getElementById('ApprovedKeywords_View').value);
        return true;
    }
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>设置关键字过滤</h3>
	<div class="FormTable">
	<form action="$_form.action" method="post" onsubmit="return beforeSubmit()">
	<table>
	
        <tr>
            <th>
                <h4>是否在屏蔽关键字提示信息中显示关键字内容</h4>
                <p><input type="radio" id="IsShowKeywordContent1" name="IsShowKeywordContent" value="true" $_form.checked('IsShowKeywordContent','true',$ContentKeywordSettings.IsShowKeywordContent) /><label for="IsShowKeywordContent1"> 显示</label></p>
                <p><input type="radio" id="IsShowKeywordContent2" name="IsShowKeywordContent" value="false" $_form.checked('IsShowKeywordContent','false',$ContentKeywordSettings.IsShowKeywordContent==false) /><label for="IsShowKeywordContent2">不显示</label></p>
            </th>
            <td></td>
        </tr>	
	
	
        <!--[error name="BannedKeywords"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
		    <h4>禁止关键字 (一行一个)</h4>
			<textarea cols="50" rows="10" id="BannedKeywords_View"></textarea>
			</th>
			<td>
			<p>禁止关键字</p>
            <p>每个关键字可以使用","，例如: "a,b"表示内容中同时存在a和b时,a和b才被禁止.</p>
            <p>如果发表的内容中包含此类别的关键字，将明确告知用户禁止发表.</p>
			</td>
		</tr>
        <!--[error name="ReplaceKeywords"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
		    <h4>替换关键字 (一行一个)</h4>
			<textarea cols="50" rows="10" id="ReplaceKeywords_View"></textarea>
			</th>
			<td>
            <p>关键字</p>
            <p>(格式: 替换前字符=替换后字符), 如果等号后面不填则默认用"*"替换.</p>
            <p>如果发表的内容中包含此类别的关键字，将可以发表，但相关关键字将被替换</p>
			</td>
		</tr>
        <!--[error name="ApprovedKeywords"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
		    <h4>审核关键字 (一行一个)</h4>
			<textarea cols="50" rows="10" id="ApprovedKeywords_View"></textarea>
			</th>
			<td>
			<p>关键字</p>
            <p>每个关键可以使用","，例如: "a,b"表示内容中同时存在a和b时,该内容需要管理员审核。</p>
            <p>如果发表的内容中包含此类别的关键字，内容将等到管理员审核后才能公开发表</p>
			</td>
		</tr>
		<tr>
			<th>
			
			<textarea id="BannedKeywords" name="BannedKeywords" style="display:none">$BannedKeywords</textarea>
			<textarea id="ReplaceKeywords" name="ReplaceKeywords" style="display:none">$ReplaceKeywords</textarea>
			<textarea id="ApprovedKeywords" name="ApprovedKeywords" style="display:none">$ApprovedKeywords</textarea>
            
			<script type="text/javascript">
			    document.getElementById('BannedKeywords_View').value = decode64(document.getElementById('BannedKeywords').value);
			    document.getElementById('ReplaceKeywords_View').value = decode64(document.getElementById('ReplaceKeywords').value);
			    document.getElementById('ApprovedKeywords_View').value = decode64(document.getElementById('ApprovedKeywords').value);
			</script>

		    <input type="submit" value="保存设置" name="savesetting" class="button" />
	        
			</th>
			<td>&nbsp;</td>
		</tr>
	</table>
</form>
	</div>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
