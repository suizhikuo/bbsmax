<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>缓存设置</title>
<!--[include src="../_htmlhead_.aspx" /]-->
<script type="text/javascript" src="$root/max-assets/nicedit/nicEdit.js"></script>

</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>缓存设置</h3>
	<div class="Help">
    <p>为了获得更高的性能，这里提供了缓存时间设置，通常缓存时间越大性能越好，但是数据就越可能会是过期的。</p>
    <p>这里只提供了本周,本日浏览最多主题，本周,本日在线最长用户的缓存时间设置。其它类似最新回复，今日热门等缓存均是实时更新，不需要设置缓存时间。</p>
    </div>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	<table style="margin-bottom:1px;">

        <!--[error name="weekPostCacheTime"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
			<th>
			    <h4>本周浏览最多的主题缓存时间</h4>
				<input type="text" class="text number" name="weekPostCacheTime" value="$_form.text('weekPostCacheTime',$CacheSettings.weekPostCacheTime)" />分钟
			</th>
			<td>
            
			</td>
		</tr>
        <!--[error name="dayPostCacheTime"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
			<th>
			    <h4>本日浏览最多的主题缓存时间</h4>
				<input type="text" class="text number" name="dayPostCacheTime" value="$_form.text('dayPostCacheTime',$CacheSettings.dayPostCacheTime)" />分钟
			</th>
			<td>
			</td>
		</tr>
        <!--[error name="WeekUserCacheTime"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
			<th>
			    <h4>本周在线最长的用户缓存时间</h4>
				<input type="text" class="text number" name="WeekUserCacheTime" value="$_form.text('WeekUserCacheTime',$CacheSettings.WeekUserCacheTime)" />分钟
			</th>
			<td>
			</td>
		</tr>
        
        <!--[error name="DayUserCacheTime"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
			<th>
			    <h4>本日在线最长的用户缓存时间</h4>
				<input type="text" class="text number" name="DayUserCacheTime" value="$_form.text('DayUserCacheTime',$CacheSettings.DayUserCacheTime)" />分钟
			</th>
			<td>
			</td>
		</tr>

        
	</table>
    <table>
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
