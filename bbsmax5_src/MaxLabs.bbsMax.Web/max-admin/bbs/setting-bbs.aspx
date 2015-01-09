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
	    
		<tr>
			<th>  
			    <h4>全局置顶主题的排序方式</h4>
				<p><input type="radio" name="GloableStickSortType" id="GloableStickSortType1" value="StickDate" $_form.Checked("GloableStickSortType","StickDate",$BbsSettings.GloableStickSortType==StickSortType.StickDate) /> <label for="GloableStickSortType1">按置顶时间</label></p>
				<p><input type="radio" name="GloableStickSortType" id="GloableStickSortType2" value="LastReplyDate" $_form.Checked("GloableStickSortType","LastReplyDate",$BbsSettings.GloableStickSortType ==StickSortType.LastReplyDate) /> <label for="GloableStickSortType2">按最后回复时间</label></p>
			</th>   
			<td>更改此设置,仅对新置顶的主题生效</td>
		</tr>
		<tr>
			<th>  
			    <h4>版块置顶主题的排序方式</h4>
				<p><input type="radio" name="StickSortType" id="StickSortType1" value="StickDate" $_form.Checked("StickSortType","StickDate",$BbsSettings.StickSortType==StickSortType.StickDate) /> <label for="StickSortType1">按置顶时间</label></p>
				<p><input type="radio" name="StickSortType" id="StickSortType2" value="LastReplyDate" $_form.Checked("StickSortType","LastReplyDate",$BbsSettings.StickSortType ==StickSortType.LastReplyDate) /> <label for="StickSortType2">按最后回复时间</label></p>
			</th>   
			<td>更改此设置,仅对新置顶的主题生效</td>
		</tr>
        <!--[error name="ThreadsPageSize"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>主题列表页每页显示主题数</h4>
				<input type="text" class="text number" name="ThreadsPageSize" value="$_form.text('ThreadsPageSize',$BbsSettings.ThreadsPageSize)" />
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="PostsPageSize"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>主题页每页显示回复数</h4>
				<input type="text" class="text number" name="PostsPageSize" value="$_form.text('PostsPageSize',$BbsSettings.PostsPageSize)" />
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="HotThreadRequireReplies"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>热门主题的判断依据</h4>
				<input type="text" class="text number" name="HotThreadRequireReplies" value="$_form.text('HotThreadRequireReplies',$BbsSettings.HotThreadRequireReplies)" />
			</th>
			<td>
			回复数达到该值就为热门主题
			</td>
		</tr>
        <!--[error name="NewThreadTime"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>多久时间内发表的主题为最新主题</h4>
				<input type="text" class="text number" name="NewThreadTime" value="$_form.text('NewThreadTime',$BbsSettings.NewThreadTime)" />秒
			</th>
			<td>
			最新主题将在主题附近显示“New”图标
			</td>
		</tr>
        <!--[error name="NewThreadPageCount"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>最新主题显示页数</h4>
				<input type="text" class="text number" name="NewThreadPageCount" value="$_form.text('NewThreadPageCount',$BbsSettings.NewThreadPageCount)" />
			</th>
			<td>&nbsp;</td>
		</tr>
		
        <!--[error name="RssShowThreadCount"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>Rss中显示主题个数</h4>
				<input type="text" class="text number" name="RssShowThreadCount" value="$_form.text('RssShowThreadCount',$BbsSettings.RssShowThreadCount)" />
			</th>
			<td>&nbsp;</td>
		</tr>
		
        <!--[error name="DisplaySubforumsInIndexpage"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>  
			    <h4>是否在首页显示子版块</h4>
				<p><input type="radio" name="DisplaySubforumsInIndexpage" id="DisplaySubforumsInIndexpage1" value="true" $_form.Checked("DisplaySubforumsInIndexpage","true",$BbsSettings.DisplaySubforumsInIndexpage) /> <label for="DisplaySubforumsInIndexpage1">是</label></p>
				<p><input type="radio" name="DisplaySubforumsInIndexpage" id="DisplaySubforumsInIndexpage2" value="false" $_form.Checked("DisplaySubforumsInIndexpage","false",  !$BbsSettings.DisplaySubforumsInIndexpage) /> <label for="DisplaySubforumsInIndexpage2">否</label></p>
			</th>   
			<td>&nbsp;</td>
		</tr>
        <!--[error name="DisplaySideBar"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>  
			    <h4>首页是否显示侧边栏</h4>
				<p><input type="radio" name="DisplaySideBar" id="DisplaySideBar1" value="true" $_form.Checked("DisplaySideBar","true",$BbsSettings.DisplaySideBar) /> <label for="DisplaySideBar1">是</label></p>
				<p><input type="radio" name="DisplaySideBar" id="DisplaySideBar2" value="false" $_form.Checked("DisplaySideBar","false",  !$BbsSettings.DisplaySideBar) /> <label for="DisplaySideBar2">否</label></p>
			</th>   
			<td>&nbsp;</td>
		</tr>
        <!--[error name="DisplayAvatar"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>  
			    <h4>帖子中是否显示头像</h4>
				<p><input type="radio" name="DisplayAvatar" id="DisplayAvatar1" value="true" $_form.Checked("DisplayAvatar","true",$BbsSettings.DisplayAvatar) /> <label for="DisplayAvatar1">是</label></p>
				<p><input type="radio" name="DisplayAvatar" id="DisplayAvatar2" value="false" $_form.Checked("DisplayAvatar","false",  !$BbsSettings.DisplayAvatar) /> <label for="DisplayAvatar2">否</label></p>
			</th>   
			<td>&nbsp;</td>
		</tr>
        <!--[error name="EnableGuestNickName"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>  
			    <h4>游客发帖允许使用昵称</h4>
				<p><input type="radio" name="EnableGuestNickName" id="EnableGuestNickName1" value="true" $_form.Checked("EnableGuestNickName","true",$BbsSettings.EnableGuestNickName) /> <label for="EnableGuestNickName1">是</label></p>
				<p><input type="radio" name="EnableGuestNickName" id="EnableGuestNickName2" value="false" $_form.Checked("EnableGuestNickName","false",  !$BbsSettings.EnableGuestNickName) /> <label for="EnableGuestNickName2">否</label></p>
			</th>   
			<td>要设置游客能否发帖子请<a href="$admin/bbs/manage-forum-detail.aspx?action=editusepermission&forumid=0">编辑版块的使用权限</a></td>
		</tr>
		<!--[error name="DefaultTextMode"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>  
			    <h4>发帖时编辑器默认选中格式</h4>
				<p><input type="radio" name="DefaultTextMode" id="DefaultTextMode1" value="true" $_form.Checked("DefaultTextMode","true",$BbsSettings.DefaultTextMode) /> <label for="DefaultTextMode1">可视化</label></p>
				<p><input type="radio" name="DefaultTextMode" id="DefaultTextMode2" value="false" $_form.Checked("DefaultTextMode","false",  !$BbsSettings.DefaultTextMode) /> <label for="DefaultTextMode2">源代码</label></p>
			</th>   
			<td>&nbsp;</td>
		</tr>
        <!--[error name="AllowQuicklyCreateThread"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>  
			    <h4>开启快速发主题</h4>
				<p><input type="radio" name="AllowQuicklyCreateThread" id="AllowQuicklyCreateThread1" value="true" $_form.Checked("AllowQuicklyCreateThread","true",$BbsSettings.AllowQuicklyCreateThread) /> <label for="AllowQuicklyCreateThread1">是</label></p>
				<p><input type="radio" name="AllowQuicklyCreateThread" id="AllowQuicklyCreateThread2" value="false" $_form.Checked("AllowQuicklyCreateThread","false",  !$BbsSettings.AllowQuicklyCreateThread) /> <label for="AllowQuicklyCreateThread2">否</label></p>
			</th>   
			<td>&nbsp;</td>
		</tr>
        <!--[error name="AllowQuicklyReply"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>  
			    <h4>开启快速回复</h4>
				<p><input type="radio" name="AllowQuicklyReply" id="AllowQuicklyReply1" value="true" $_form.Checked("AllowQuicklyReply","true",$BbsSettings.AllowQuicklyReply) /> <label for="AllowQuicklyReply1">是</label></p>
				<p><input type="radio" name="AllowQuicklyReply" id="AllowQuicklyReply2" value="false" $_form.Checked("AllowQuicklyReply","false",  !$BbsSettings.AllowQuicklyReply) /> <label for="AllowQuicklyReply2">否</label></p>
			</th>   
			<td>&nbsp;</td>
		</tr>
        <!--[error name="ShowMarksCount"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>帖子中显示评分个数</h4>
				<input type="text" class="text number" name="ShowMarksCount" value="$_form.text('ShowMarksCount',$BbsSettings.ShowMarksCount)" />
			</th>
			<td>帖子中只显示最新的前几个评分</td>
		</tr>
        <!--[error name="MaxPollItemCount"]-->
        <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>允许最大的投票项个数</h4>
				<input type="text" class="text number" name="MaxPollItemCount" value="$_form.text('MaxPollItemCount',$BbsSettings.MaxPollItemCount)" />
			</th>
			<td>允许最大的投票选项个数(不能大于30)</td>
		</tr>
    </table>
    <table class="multiColumns" style="margin-bottom:1px;">
		<!--[loop $item in $BbsSettings.DisplaySignature with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$BbsSettings.DisplaySignature.Count" name="DisplaySignature"  title="帖子中显示签名" description="是否在帖子中显示签名" /]-->
	    <!--[/loop]-->
	</table>
	
	<table style="margin-bottom:1px;">
		<tr>
			<th>  
			    <h4>主题中含有图片附件时弹出登陆框</h4>
				<input type="radio" name="EnableShowLoginDialog" id="EnableShowLoginDialog1" value="true" $_form.Checked("EnableShowLoginDialog","true",$BbsSettings.EnableShowLoginDialog) /> <label for="EnableShowLoginDialog1">是</label>
				<input type="radio" name="EnableShowLoginDialog" id="EnableShowLoginDialog2" value="false" $_form.Checked("EnableShowLoginDialog","false",  !$BbsSettings.EnableShowLoginDialog) /> <label for="EnableShowLoginDialog2">否</label>
			</th>   
			<td>如果用户未登陆,并且该主题内容中含有图片附件时,是否弹出登陆对话框</td>
		</tr>
	</table>
    <table class="multiColumns" style="margin-bottom:1px;">
		
	    <!--[loop $item in $BbsSettings.MaxAttachmentCountInDay with $index]-->
        <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$BbsSettings.MaxAttachmentCountInDay.Count" name="MaxAttachmentCountInDay" type="int" textboxwidth="4" title="每天可以上传附件个数" description="每天可以上传附件个数，为0则不限制" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $BbsSettings.MaxTotalAttachmentsSizeInDay with $index]-->
        <!--[load src="../_exceptableitem_filesize_.ascx" index="$index" item="$item" itemCount="$BbsSettings.MaxTotalAttachmentsSizeInDay.Count" name="MaxTotalAttachmentsSizeInDay" title="每天允许上传附件最大大小" description="每天允许上传附件最大大小，为0则不限制" /]-->
	    <!--[/loop]-->
    </table>
    <table>
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
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
