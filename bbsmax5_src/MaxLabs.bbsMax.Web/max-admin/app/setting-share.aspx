<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>分享相关设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>设置分享功能</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
        <table>
			<tr>
				<th>
					<h4>是否开启分享功能</h4>
					<p><input type="radio" name="EnableShareFunction" id="EnableShareFunction1" value="True" $_form.checked('EnableShareFunction','True',$ShareSettings.EnableShareFunction == true) /> <label for="EnableShareFunction1">开启</label></p>
					<p><input type="radio" name="EnableShareFunction" id="EnableShareFunction2" value="False" $_form.checked('EnableShareFunction','False',$ShareSettings.EnableShareFunction == false) /> <label for="EnableShareFunction2">关闭</label></p>
				</th>
				<td>关闭分享功能将使所有网站用户在网站上看不到任何关于分享功能的链接，也不能使用任何关于分享的功能。</td>
			</tr>
			<!--[error name="HotDays"]-->
            <!--[include src="../_error_.aspx"/]-->
            <!--[/error]-->
		    <tr>
			    <th>
			        <h4>最近热门显示多少天内的分享</h4>
				    <input type="text" class="text number" name="HotDays" value="$_form.text('HotDays',$ShareSettings.HotDays)" />天
			    </th>
			    <td>&nbsp;</td>
		    </tr>
		    <tr>
			    <th>  
			        <h4>热门排序依据</h4>
				    <p><input type="radio" name="HotShareSortType" id="HotShareSortType1" value="ShareCount" $_form.Checked("HotShareSortType","ShareCount",$ShareSettings.HotShareSortType==HotShareSortType.ShareCount) /> <label for="HotShareSortType1">被分享数</label></p>
				    <p><input type="radio" name="HotShareSortType" id="HotShareSortType2" value="AgreeAndOpposeCount" $_form.Checked("HotShareSortType","AgreeAndOpposeCount",  $ShareSettings.HotShareSortType==HotShareSortType.AgreeAndOpposeCount) /> <label for="HotShareSortType2">分享的支持数加反对数</label></p>
			    </th>   
			    <td>&nbsp;</td>
		    </tr>
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
