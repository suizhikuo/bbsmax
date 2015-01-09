<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>全局动态</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->

<!--[SiteFeed feedID="$_get.feedID"]-->
<!--[if $hasAnyError]-->
<div class="Tip Tip-error">$errorMessage</div>
<!--[/if]-->

<!--[include src="../_setting_msg_.aspx"/]-->

<form action="$_form.action" method="post">
<div class="Content">
    <div class="Help">
    全局动态，就是会在站内任何一个成员的好友动态里面都会出现的动态，每个成员都能第一时间看到。站长可以灵活使用全局动态来发布一些公开的信息。
    </div>
    
	<div class="PageHeading">
	    <h3><!--[if $isEdit]-->编辑全局动态<!--[else]-->添加全局动态<!--[/if]--></h3>
	    <div class="ActionsBar">
	        <a class="back" href="$admin/global/manage-feed-sitefeedlist.aspx"><span>全局动态列表</span></a>
	    </div>
	</div>
	
	<div class="FormTable">
	<table>
        <!--[error name="title"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>动态标题 <span class="request" title="必填项">*</span></h4>
			<input type="text" class="text" name="title" value="$_form.text('title',$title)" />
			</th>
			<td>支持html.</td>
		</tr>
        <!--[error name="content"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>动态内容 <span class="request" title="必填项">*</span></h4>
			<textarea name="content" cols="30" rows="6">$_form.text('content',$content)</textarea>
			</th>
			<td>支持html.</td>
		</tr>
		<tr>
			<th>
			<h4>动态备注</h4>
			<input type="text" class="text" name="description" value="$_form.text('description',$description)" />
			</th>
			<td>支持html.</td>
		</tr>
		<tr>
			<th>
			<h4>发布日期时间</h4>
		    <input type="text" class="text" id="createdate" name="createdate" value="$_form.text('createdate',$createdate)" />
            <a class="selector-date" title="选择日期" href="javascript:void(0);" id="A0"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
			</th>
			<td>
			填写日期时间格式为 <strong>$DateTimeNow</strong>，为空为当前的日期和时间。 
            您也可以填写一个将来的日期和时间，那么这条动态会在这个将来的日期到来之前，一直显示在第一位。
			</td>
		</tr>
		<tr>
			<th>
			<h4>第1张图片地址</h4>
			<input type="text" class="text" name="image1" value="$_form.text('image1',$image1)" />
			</th>
			<td>&nbsp;</td>
		</tr>
		<tr>
			<th>
			<h4>第1张图片链接</h4>
			<input type="text" class="text" name="link1" value="$_form.text('link1',$link1)" />
			</th>
			<td>&nbsp;</td>
		</tr>
		
		<tr>
			<th>
			<h4>第2张图片地址</h4>
			<input type="text" class="text" name="image2" value="$_form.text('image2',$image2)" />
			</th>
			<td>&nbsp;</td>
		</tr>
		<tr>
			<th>
			<h4>第2张图片链接</h4>
			<input type="text" class="text" name="link2" value="$_form.text('link2',$link2)" />
			</th>
			<td>&nbsp;</td>
		</tr>

		<tr>
			<th>
			<h4>第3张图片地址</h4>
			<input type="text" class="text" name="image3" value="$_form.text('image3',$image3)" />
			</th>
			<td>&nbsp;</td>
		</tr>
		<tr>
			<th>
			<h4>第3张图片链接</h4>
			<input type="text" class="text" name="link3" value="$_form.text('link3',$link3)" />
			</th>
			<td>&nbsp;</td>
		</tr>
		<tr>
			<th>
			<h4>第4张图片地址</h4>
			<input type="text" class="text" name="image4" value="$_form.text('image4',$image4)" />
			</th>
			<td>&nbsp;</td>
		</tr>
		<tr>
			<th>
			<h4>第4张图片链接</h4>
			<input type="text" class="text" name="link4" value="$_form.text('link4',$link4)" />
			</th>
			<td>&nbsp;</td>
		</tr>
        <tr>
		    <th>
		    <input type="submit" value="保存" name="savesitefeed" class="button" />
		    </th>
		    <td>&nbsp;</td>
	    </tr>
    </table>
    </div>

</div>
</form>
<script type="text/javascript">

initDatePicker('createdate','A0')
</script>
<!--[/SiteFeed]-->
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>