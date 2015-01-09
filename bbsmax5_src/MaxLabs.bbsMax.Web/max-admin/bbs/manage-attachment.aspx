<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>附件管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
function deleteAttachment(id)
{
    removeElement($('item_'+id));
}
</script>
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $IsOwner == false]-->
<div class="Tip Tip-permission">由于权限限制, 你可能无法管理部分用户组的附件数据. 此处不会列出这些数据.灰色的版块是您没有权限管理的版块;只有创始人才能选择全部版块</div>
<!--[/if]-->
<div class="Content">
    <h3>附件管理</h3>
    <div class="SearchTable">
    <form action="$_form.action" method="post">
    <table>
    <tr>
	    <td><label for="forum">版块</label></td>
	    <td colspan="3">
	    <select name="ForumID">
		    <option value="">所有版块</option>
		    <!--[loop $tempForum in $Forums with $i]-->
		    <option value="$tempForum.ForumID" <!--[if $HasPermission($tempForum) == false]--> style="color:Red"<!--[/if]-->  $_form.selected("ForumID","$tempForum.ForumID","$out($Filter.ForumID)")>&nbsp;&nbsp;&nbsp;&nbsp;$ForumSeparators[$i]|--$tempForum.ForumName</option>
		    <!--[/loop]-->
		</select>
	    </td>
	</tr>
	<tr>
	    <td><label for="username">用户名</label></td>
	    <td><input class="text" id="username" name="username" type="text" value="$Filter.Username" /></td>
	    <td><label for="UserID">作者ID</label></td>
	    <td><input class="text" id="UserID" name="UserID" type="text" value="$Filter.UserID" /></td>
	</tr>
	<tr>
	    <td><label for="keyword">文件名关键字</label></td>
	    <td><input class="text" id="keyword" name="keyword" type="text" value="$Filter.keyword" /></td>
	    <td><label for="filetype">文件类型<label></td>
	    <td>
	        <input class="text" id="filetype" name="filetype" type="text" value="$Filter.filetype" />
	        <span class="desc">(如:"gif")</span>
	    </td>
	</tr>
	<tr>
	    <td><label>下载次数</label></td>
	    <td><input name="MinTotalDownload" class="text" style="width:6em;" type="text" value="$Filter.MinTotalDownload" /> ~ <input name="MaxTotalDownload" class="text" style="width:6em;" type="text" value="$Filter.MaxTotalDownload" /></td>
	    <td><label>文件大小</label></td>
	    <td><input name="MinFileSize" class="text" style="width:6em;" type="text" value="$Filter.MinFileSize" />
	    <select name="MinFileSizeUnit">
                <option value="K" $_form.selected("MinFileSizeUnit","K",$Filter.MinFileSizeUnit)>KB</option>
                <option value="M" $_form.selected("MinFileSizeUnit","M",$Filter.MinFileSizeUnit)>MB</option>
                <option value="B" $_form.selected("MinFileSizeUnit","B",$Filter.MinFileSizeUnit)>B</option>
                <option value="G" $_form.selected("MinFileSizeUnit","G",$Filter.MinFileSizeUnit)>GB</option>
            </select>
	     ~ <input name="MaxFileSize" class="text" style="width:6em;" type="text" value="$Filter.MaxFileSize" />
	     <select name="MaxFileSizeUnit">
                <option value="K" $_form.selected("MaxFileSizeUnit","K",$Filter.MaxFileSizeUnit)>KB</option>
                <option value="M" $_form.selected("MaxFileSizeUnit","M",$Filter.MaxFileSizeUnit)>MB</option>
                <option value="B" $_form.selected("MaxFileSizeUnit","B",$Filter.MaxFileSizeUnit)>B</option>
                <option value="G" $_form.selected("MaxFileSizeUnit","G",$Filter.MaxFileSizeUnit)>GB</option>
            </select>
	     </td>
	</tr>
	
	<tr>
	    <td><label>价格</label></td>
	    <td><input name="MinPrice" class="text" style="width:6em;" type="text" value="$Filter.MinPrice" /> ~ <input name="MaxPrice" class="text" style="width:6em;" type="text" value="$Filter.MaxPrice" /></td>
	</tr>
	
	<tr>
	    <td>上传时间</td>
	    <td colspan="3">
	        <input name="begindate" id="begindate" class="text" style="width:6em;" type="text" value="$Filter.BeginDate" />
	        <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
	        ~
	        <input name="enddate" id="enddate" class="text" style="width:6em;" type="text" value="$Filter.EndDate" /> 
	        <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
	        <span class="desc">(时间格式: YYYY-MM-DD)</span>
	    </td>
	</tr>
	<tr>
	    <td>结果排序</td>
	    <td>
            <select name="order">
                <option value="AttachmentID" $_form.selected("order","AttachmentID",$Filter.Order)>发表时间</option>
                <option value="TotalDownload" $_form.selected("order","TotalDownload",$Filter.Order)>下载次数</option>
                <option value="FileSize" $_form.selected("order","FileSize",$Filter.Order)>附件大小</option>
                <option value="Price" $_form.selected("order","Price",$Filter.Order)>价格</option>
            </select>
            <select name="isdesc">
                <option value="true" $_form.selected("isdesc","true",$Filter.IsDesc.ToString())>按降序排列</option>
                <option value="false" $_form.selected("isdesc","false",$Filter.IsDesc.ToString())>按升序排列</option>
            </select>
        </td>
        <td>每页显示数</td>
        <td>
            <select name="pagesize">
                <option value="10" $_form.selected("pagesize","10",$Filter.PageSize)>10</option>
                <option value="20" $_form.selected("pagesize","20",$Filter.PageSize)>20</option>
                <option value="50" $_form.selected("pagesize","50",$Filter.PageSize)>50</option>
                <option value="100" $_form.selected("pagesize","100",$Filter.PageSize)>100</option>
                <option value="200" $_form.selected("pagesize","200",$Filter.PageSize)>200</option>
                <option value="500" $_form.selected("pagesize","500",$Filter.PageSize)>500</option>
            </select>
	    </td>
	</tr>
	<tr>
	    <td>&nbsp;</td>
	    <td colspan="3">
            <input type="submit" name="search" class="button" value="搜索" />
	    </td>
	</tr>
	</table>
	</form>
	</div>

    <form action="$_form.action" method="post" name="attachmentlistForm" id="attachmentlistForm">
        <div class="DataTable">
        <h4>附件 <span class="counts">总数: $TotalCount</span></h4>
        <!--[if $TotalCount > 0]-->
        <table>
            <thead>
                <tr>
                    <th class="CheckBoxHold">&nbsp;</th>
                    <th>文件名</th>
                    <th style="width:100px;">作者</th>
                    <th style="width:100px;">所在主题</th>
                    <th style="width:100px;">大小</th>
                    <th style="width:100px;">价格</th>
                    <th style="width:100px;">下载次数</th>
                    <th style="width:100px;">上传时间</th>
                    <th style="width:50px;">操作</th>
                </tr>
            </thead>
            <tbody>
        <!--[/if]-->
    <!--[loop $attachment in $AttachmentList]-->
                <tr id="item_$attachment.attachmentID">
                    <td><input name="attachmentids" type="checkbox" value="$Attachment.AttachmentID" /></td>
                    <td><a href="$GetAttachmentUrl($Attachment.AttachmentID)" target="_blank">$Attachment.FileName</a></td>
                    <td><a href="$url(space/$Attachment.UserID)" target="_blank">$GetUserName($Attachment.PostID)</a></td>
                    <td><a href="$GetThreadUrl($Attachment.PostID)" target="_blank">$GetThreadSubject($Attachment.PostID)</a></td>
                    <td>$GetFileSize($Attachment.FileSize)</td>
                    <td>$Attachment.Price</td>
                    <td>$Attachment.TotalDownloads</td>
                    <td>$Attachment.CreateDate</td>
                    <td><a href="$dialog/attachment-delete.aspx?attachmentID=$attachment.AttachmentID&forumID=$GetForumID($attachment.PostID)" onclick="return openDialog(this.href,this,deleteAttachment);">删除</a></td>
                </tr>
    <!--[/loop]-->
    <!--[if $TotalCount > 0]-->
            </tbody>
        </table>
        <div class="Actions">
            <input type="checkbox" id="selectAll" /> <label for="selectAll">全选</label>
            <input type="submit" class="button" name="deletechecked" onclick="if(confirm('确认删除选中的数据吗?')==false)return false;" value="删除选中" />
            <input type="submit" class="button" name="deletesearched" onclick="if(confirm('确认删除搜索到的数据吗?')==false)return false;" value="删除搜索到的数据" />
        </div>
        <script type="text/javascript">
            new checkboxList( 'attachmentids', 'selectAll');
        </script>
        <!--[AdminPager Count="$TotalCount" PageSize="$PageSize" /]-->
    <!--[else]-->
        <div class="NoData">未搜索到附件.</div>
    <!--[/if]-->
        </div>
    </form>
</div>
<script type="text/javascript">

    initDatePicker('begindate','A0');
    initDatePicker('enddate','A1');
</script>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
