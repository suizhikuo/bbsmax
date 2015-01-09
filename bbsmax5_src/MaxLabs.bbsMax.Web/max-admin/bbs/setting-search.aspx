<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>论坛功能设置</title>
<!--[include src="../_htmlhead_.aspx" /]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>设置论坛功能</h3>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	<table style="margin-bottom:1px;">
        <!--[error name="LikeStatement"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>搜索方式 </h4>
				<input type="radio" name="SearchType" id="SearchType1" value="LikeStatement" $_form.checked('SearchType','LikeStatement',$SearchSettings.SearchType==SearchType.LikeStatement) />
				<label for="SearchType1">采用 like 方式搜索（适合帖子量少于10万的网站）</label><br />
                <input type="radio" name="SearchType" id="SearchType2" value="FullTextIndex" $_form.checked('SearchType','FullTextIndex',$SearchSettings.SearchType==SearchType.FullTextIndex) />
                <label for="SearchType2">SQL全文检索（需要数据库服务器支持， 适合帖子量较多的网站）</label>
			</th>
			<td>
			
			</td>
		</tr>
        <!--[error name="SearchPageSize"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
			<th>
			    <h4>搜索结果页大小 </h4>
				<input type="text" class="text" name="SearchPageSize" value="$_form.text('SearchPageSize',$SearchSettings.SearchPageSize)" />
			</th>
			<td>
			
			</td>
		</tr>
        <!--[error name="HighlightColor"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>搜索到的关键字颜色</h4>
				<input type="text" class="text" id="HighlightColor" name="HighlightColor" value="$_form.text('HighlightColor',$SearchSettings.HighlightColor)" />
                <a title="选择颜色" class="selector-color" id="A0" href="#"><img src="$Root/max-assets/images/color.gif" alt="选择颜色" /></a>

			</th>
			<td>
			
			</td>
		</tr>
		 </table>
    <table class="multiColumns" style="margin-bottom:1px;">
		<!--[loop $item in $SearchSettings.EnableSearch with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$SearchSettings.EnableSearch.Count" name="EnableSearch"  title="允许注册用户搜索论坛" description="是否允许注册用户搜索论坛，如果关闭搜索，则以下对注册用户的设置无效" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $SearchSettings.CanSearchTopicContent with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$SearchSettings.CanSearchTopicContent.Count" name="CanSearchTopicContent"  title="允许注册用户搜索主题内容" description="是否允许注册用户搜索主题内容" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $SearchSettings.CanSearchAllPost with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$SearchSettings.CanSearchAllPost.Count" name="CanSearchAllPost"  title="允许注册用搜索户帖子内容" description="是否允许注册用户搜索帖子内容" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $SearchSettings.CanSearchUserTopic with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$SearchSettings.CanSearchUserTopic.Count" name="CanSearchUserTopic"  title="允许注册用户搜索指定用户的主题" description="是否允许注册用户搜索指定用户的主题" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $SearchSettings.CanSearchUserPost with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$SearchSettings.CanSearchUserPost.Count" name="CanSearchUserPost"  title="允许注册用户搜索指定用户帖子" description="是否允许注册用户搜索指定用户帖子" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $SearchSettings.MaxResultCount with $index]-->
        <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$SearchSettings.MaxResultCount.Count" name="MaxResultCount" type="int" textboxwidth="4" title="注册用户的最大搜索结果" description="返回搜索结果的最大条数，越小性能越好，请填写大于0的数字" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $SearchSettings.SearchTime with $index]-->
        <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$SearchSettings.SearchTime.Count" name="SearchTime" type="int" textboxwidth="4" title="注册用户的搜索时间间隔" description="每两次搜索的时间间隔，单位秒，0为不限制" /]-->
	    <!--[/loop]-->
    </table>
    <table style="margin-bottom:1px;">
        <!--[error name="EnableGuestSearch"]-->
        <!--[pre-include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>允许游客搜索论坛 </h4>
				<input type="radio" name="EnableGuestSearch" id="EnableGuestSearch1" value="true" $_form.checked('EnableGuestSearch','true',$SearchSettings.EnableGuestSearch) />
				<label for="EnableGuestSearch1">是</label><br />
                <input type="radio" name="EnableGuestSearch" id="EnableGuestSearch2" value="false" $_form.checked('EnableGuestSearch','false',$SearchSettings.EnableGuestSearch==false) />
                <label for="EnableGuestSearch2">否</label>
			</th>
			<td>
			是否允许游客搜索论坛，如果关闭搜索，则以下对游客的设置无效
			</td>
		</tr>
		<tr>
			<th>
			    <h4>允许游客搜索主题内容 </h4>
				<input type="radio" name="GuestCanSearchTopicContent" id="GuestCanSearchTopicContent1" value="true" $_form.checked('GuestCanSearchTopicContent','true',$SearchSettings.GuestCanSearchTopicContent) />
				<label for="GuestCanSearchTopicContent1">是</label><br />
                <input type="radio" name="GuestCanSearchTopicContent" id="GuestCanSearchTopicContent2" value="false" $_form.checked('GuestCanSearchTopicContent','false',$SearchSettings.GuestCanSearchTopicContent==false) />
                <label for="GuestCanSearchTopicContent2">否</label>
			</th>
			<td>
			是否允许游客用户搜索主题内容
			</td>
		</tr>
		<tr>
			<th>
			    <h4>允许游客搜索户帖子内容 </h4>
				<input type="radio" name="GuestCanSearchAllPost" id="GuestCanSearchAllPost1" value="true" $_form.checked('GuestCanSearchAllPost','true',$SearchSettings.GuestCanSearchAllPost) />
				<label for="GuestCanSearchAllPost1">是</label><br />
                <input type="radio" name="GuestCanSearchAllPost" id="GuestCanSearchAllPost2" value="false" $_form.checked('GuestCanSearchAllPost','false',$SearchSettings.GuestCanSearchAllPost==false) />
                <label for="GuestCanSearchAllPost2">否</label>
			</th>
			<td>
			是否允许游客搜索户帖子内容
			</td>
		</tr>
		<tr>
			<th>
			    <h4>允许游客搜索指定用户的主题 </h4>
				<input type="radio" name="GuestCanSearchUserTopic" id="GuestCanSearchUserTopic1" value="true" $_form.checked('GuestCanSearchUserTopic','true',$SearchSettings.GuestCanSearchUserTopic) />
				<label for="GuestCanSearchUserTopic1">是</label><br />
                <input type="radio" name="GuestCanSearchUserTopic" id="GuestCanSearchUserTopic2" value="false" $_form.checked('GuestCanSearchUserTopic','false',$SearchSettings.GuestCanSearchUserTopic==false) />
                <label for="GuestCanSearchUserTopic2">否</label>
			</th>
			<td>
			是否允许游客搜索指定用户的主题
			</td>
		</tr>
		<tr>
			<th>
			    <h4>允许游客搜索指定用户的帖子 </h4>
				<input type="radio" name="GuestCanSearchUserPost" id="GuestCanSearchUserPost1" value="true" $_form.checked('GuestCanSearchUserPost','true',$SearchSettings.GuestCanSearchUserPost) />
				<label for="GuestCanSearchUserPost1">是</label><br />
                <input type="radio" name="GuestCanSearchUserPost" id="GuestCanSearchUserPost2" value="false" $_form.checked('GuestCanSearchUserPost','false',$SearchSettings.GuestCanSearchUserPost==false) />
                <label for="GuestCanSearchUserPost2">否</label>
			</th>
			<td>
			是否允许游客搜索指定用户的帖子
			</td>
		</tr>
		<!--[error name="GuestMaxResultCount"]-->
        <!--[pre-include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
			<th>
			    <h4>游客的最大搜索结果</h4>
				<input type="text" class="text" name="GuestMaxResultCount" value="$_form.text('GuestMaxResultCount',$SearchSettings.GuestMaxResultCount)" />
			</th>
			<td>
			返回搜索结果的最大条数，越小性能越好，请填写大于0的数字
			</td>
		</tr>
		<!--[error name="GuestSearchTime"]-->
        <!--[pre-include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
			<th>
			    <h4>游客的搜索时间间隔 </h4>
				<input type="text" class="text" name="GuestSearchTime" value="$_form.text('GuestSearchTime',$SearchSettings.GuestSearchTime)" />
			</th>
			<td>
			每两次搜索的时间间隔，单位秒，0为不限制
			</td>
		</tr>
    </table>
    <table>
		<tr>
			<th>
			<input type="submit" value="保存设置" class="button" name="savesetting" />
			</th>
			<td>&nbsp;</td>
		</tr> 
	</table>
	</table>
	</div>
	</form>
</div>
<script type="text/javascript">
initColorSelector('HighlightColor','A0');
</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
