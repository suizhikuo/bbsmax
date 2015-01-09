<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>实名认证设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
    initDisplay('EnablerealnameCheck',[
     {value : 'true'  , display : true ,  id : 'settings'}
    ,{value : 'false' , display : false , id : 'settings'}])
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>设置实名认证</h3>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	<table style="margin-bottom:1px;">
        <tr>
            <th>
                <h4>开启实名认证</h4>
                <p>
                <input type="radio" name="EnablerealnameCheck" id="EnableNameCheck1" value="true" $_form.checked('EnableNameCheck','true', $NameCheckSettings.EnableRealnameCheck ) />
                <label for="EnableNameCheck1">是</label>
                </p>
                <p>
                <input type="radio" name="EnablerealnameCheck" id="EnableNameCheck2" value="false" $_form.checked('EnableNameCheck','false',!$NameCheckSettings.EnableRealnameCheck) />
                <label for="EnableNameCheck2">否</label>
                </p>
            </th>
			<td>以下设置只有在开启实名机制后有效</td>
        </tr>
    </table>
    <div id="settings">
    <table style="margin-bottom:1px;">
        <!--[error name="CanChinese"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr id="namechar">
            <th>
                <h4>真实姓名允许类型</h4>
                <p>中文名 
                <input type="radio" name="CanChinese" id="CanChinese1" value="true" $_form.checked('CanChinese','true',$NameCheckSettings.CanChinese) />
                <label for="CanChinese1">是</label>
                <input type="radio" name="CanChinese" id="CanChinese2" value="false" $_form.checked('CanChinese','false',!$NameCheckSettings.CanChinese) />
                <label for="CanChinese2">否</label>
                </p>
                <p>英文名 
                <input type="radio" name="CanEnglish" id="CanEnglish1" value="true" $_form.checked('CanEnglish','true',$NameCheckSettings.CanEnglish) />
                <label for="CanEnglish1">是</label>
                <input type="radio" name="CanEnglish" id="CanEnglish2" value="false" $_form.checked('CanEnglish','false',!$NameCheckSettings.CanEnglish) />
                <label for="CanEnglish2">否</label>
                </p>
            </th>
			<td>&nbsp;</td>
        </tr>
        <tr>
            <th>
                <h4>需要上传身份证扫描件</h4>
                <p>
                <input type="radio" name="NeedIDCardFile" id="NeedIDCardFile1" value="true" $_form.checked('NeedIDCardFile','true',$NameCheckSettings.NeedIDCardFile) />
                <label for="NeedIDCardFile1">是</label>
                </p>
                <p>
                <input type="radio" name="NeedIDCardFile" id="NeedIDCardFile2" value="false"  $_form.checked('NeedIDCardFile','false',!$NameCheckSettings.NeedIDCardFile) />
                <label for="NeedIDCardFile2">否</label>
                </p>
            </th>
			<td></td>
        </tr>
        <tr>
            <th>
                <h4>身份证扫描件最大文件限制</h4>
                <p>
                <input type="text" class="text" name="MaxIDCardFileSize" value="$_form.text("MaxIDCardFileSize",$NameCheckSettings.MaxIDCardFileSize)" />
                </p>
            </th>
			<td>
                &nbsp;
            </td>
        </tr>
    </table>
    </div>
    <table>
		<tr>
			<th><input type="submit" value="保存设置" class="button" name="savesetting" /></th>
			<td>&nbsp;</td>
		</tr>
    </table>
    </div>
	</form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
