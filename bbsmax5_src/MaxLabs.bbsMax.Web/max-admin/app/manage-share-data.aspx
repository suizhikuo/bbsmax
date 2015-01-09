<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>收藏和分享管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $HasNoPermissionManageRole]-->
<div class="Tip Tip-permission">由于权限限制，你无法管理“$NoPermissionManageRoleNames”用户组的收藏和分享。此处不会列出这些数据。</div>
<!--[/if]-->
<div class="Content">
    <h3><!--[if $_get.type == "share"]-->分享<!--[else]-->收藏<!--[/if]-->管理</h3>
    <form action="$_form.action" method="post" name="share">
    <div class="SearchTable">
        <table>
        <tr>
            <td><label for="userid">作者UID</label></td>
            <td><input class="text" id="userid" name="userid" type="text" value="$filter.userid" /></td>
            <td><label for="username">作者名</label></td>
            <td><input class="text" id="username" name="username" type="text" value="$filter.username" /></td>
        </tr>
        <tr>
            <td><label for="shareid"><!--[if $_get.type == "share"]-->分享<!--[else]-->收藏<!--[/if]-->ID</label></td>
            <td><input class="text" type="text" id="shareid" name="shareid" value="$filter.shareid" /></td>
            <!--[if $_get.type == "share"]-->
            <td>公开性质</td>
            <td>
                <select name="privacytype">
                <option value="">不限制</option>
                <option value="AllVisible" $_form.selected('privacytype','AllVisible',$filter.PrivacyType.ToString())>全站用户可见</option>
                <option value="FriendVisible" $_form.selected('privacytype','FriendVisible',$filter.PrivacyType.ToString())>好友可见</option>
                </select>
            </td>
            <!--[else]-->
            <td></td>
            <!--[/if]-->
        </tr>
        <tr>
            <td>类型</td>
            <td colspan="3">
                <select name="ShareType">
                <option value="All" $_form.selected('ShareType','All',$filter.ShareType.ToString())>全部</option>
                <option value="URL" $_form.selected('ShareType','URL',$filter.ShareType.ToString())>网址</option>
                <option value="Video" $_form.selected('ShareType','Video',$filter.ShareType.ToString())>视频</option>
                <option value="Music" $_form.selected('ShareType','Music',$filter.ShareType.ToString())>音乐</option>
                <option value="Flash" $_form.selected('ShareType','Flash',$filter.ShareType.ToString())>Flash</option>
                <option value="Blog" $_form.selected('ShareType','Blog',$filter.ShareType.ToString())>日志</option>
                <option value="Album" $_form.selected('ShareType','Album',$filter.ShareType.ToString())>相册</option>
                <option value="Picture" $_form.selected('ShareType','Picture',$filter.ShareType.ToString())>图片</option>
                <option value="Forum" $_form.selected('ShareType','Forum',$filter.ShareType.ToString())>群组</option>
                <option value="Topic" $_form.selected('ShareType','Topic',$filter.ShareType.ToString())>话题</option>
                <option value="User" $_form.selected('ShareType','User',$filter.ShareType.ToString())>用户</option>
                <option value="Tag" $_form.selected('ShareType','Tag',$filter.ShareType.ToString())>TAG</option>
                </select>
            </td>
        </tr>
        <tr>
            <td>发布时间</td>
            <td colspan="3">
            <input name="begindate" id="begindate" value="$filter.begindate" class="text" style="width:6em;" type="text" />
            <a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            ~ <input name="enddate" id="enddate" value="$filter.enddate" class="text" style="width:6em;" type="text" />
            <a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
            <span class="desc">(时间格式: YYYY-MM-DD)</span>
            </td>
        </tr>
        <tr>
            <td>结果排序</td>
            <td>
                <select name="Order">
                <option value="ID" $_form.selected('Order','ID',$filter.order)>发布时间</option>
                </select>
                <select name="IsDesc">
                <option value="true" $_form.selected('IsDesc','true',$filter.isDesc)>按降序排列</option>
                <option value="false" $_form.selected('IsDesc','false',$filter.isDesc)>按升序排列</option>
                </select>
            </td>
            <td>每页显示数</td>
            <td>
                <select name="pagesize">
                <option value="10" $_form.selected('pagesize','10',$filter.pagesize))>10</option>
                <option value="20" $_form.selected('pagesize','20',$filter.pagesize))>20</option>
                <option value="50" $_form.selected('pagesize','50',$filter.pagesize))>50</option>
                <option value="100" $_form.selected('pagesize','100',$filter.pagesize))>100</option>
                <option value="200" $_form.selected('pagesize','200',$filter.pagesize))>200</option>
                <option value="500" $_form.selected('pagesize','500',$filter.pagesize))>500</option>
                </select>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td colspan="3">
            <input type="submit" class="button" name="searchshares" value="搜索" />
            </td>
        </tr>
        </table>
	</div>

    <div class="DataTable">
        <h4><!--[if $_get.type == "share"]-->分享<!--[else]-->收藏<!--[/if]--> <span class="counts">总数: $ShareTotalCount</span></h4>
        <!--[if $ShareTotalCount > 0]-->
        <div class="Actions">
            <input name="checkAll" id="selectAll_2" type="checkbox"  />
            <label for="selectAll_2">全选</label>
            <input value="1" id="Checkbox2" name="updatePoint" checked="checked" type="checkbox" />
            <label for="updatePoint">更新用户积分</label>
            <input type="submit" class="button" name="deletechecked" onclick="return confirm('确认要删除吗?删除后不可恢复!');"  value="删除选中" />
            <input type="submit" class="button" name="deleteallsearch" onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" value="删除所有搜索结果" />
            <script type="text/javascript">
               new checkboxList('shareids', 'selectAll');
            </script>
        </div>
        <table>
            <tbody>
        <!--[/if]-->
    <!--[loop $Share in $ShareList]-->
            <tr id="share_$share.id">
		        <td class="CheckBoxHold"><input value="$Share.ID" name="shareids" type="checkbox"/></td>
		        <td>
		            <div>
		            $Share.User.NameLink <!--[if $_get.type == "share"]-->分享<!--[else]-->收藏<!--[/if]-->了 $Share.TypeName
		            <span style="color:#999;">$OutputDateTime($Share.CreateDate)</span>
		            </div>
		            <div>
		            <!--[if $Share.Type == ShareType.Video]-->
                    <a href="$Share.Video.URL" target="_blank">$Share.Video.URL</a>
                    <!--[else if $Share.Type == ShareType.Music]-->
                    <a href="$Share.Content" target="_blank">$Share.Content</a>
                    <!--[else if $Share.Type == ShareType.Flash]-->
                    <a href="$Share.Content" target="_blank">$Share.Content</a>
                    <!--[else if $Share.Type == ShareType.URL]-->
                    <a href="$Share.Content" target="_blank">$Share.Content</a>
                    <!--[else]-->
                    $Share.Content
                    <!--[/if]-->
		            </div>
		        </td>
		        <td>
                    <a href="$dialog/share-delete.aspx?shareid=$Share.ID$_if($_get.type=='share','','&isfav=1')" onclick="return openDialog(this.href,this, function(result){delElement($('share_$share.id'))})">删除</a>
		        </td>
            </tr>
    <!--[/loop]-->
    <!--[if $ShareTotalCount > 0]-->
            </tbody>
        </table>
        <div class="Actions">
            <input name="checkAll" id="selectAll" type="checkbox"  />
            <label for="selectAll">全选</label>
            <input value="1" id="updatePoint" name="updatePoint" checked="checked" type="checkbox" />
            <label for="updatePoint">更新用户积分</label>
            <input type="submit" class="button" name="deletechecked" onclick="return confirm('确认要删除吗?删除后不可恢复!');" value="删除选中" />
            <input type="submit" class="button" name="deleteallsearch" onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" value="删除所有搜索结果" />
            <script type="text/javascript">
               new checkboxList('shareids', 'selectAll');
               new checkboxList('shareids', 'selectAll_2');
            </script>
        </div>
        <!--[AdminPager Count="$ShareTotalCount" PageSize="$Filter.PageSize" /]-->
    <!--[else]-->
        <div class="NoData">未搜索到符合条件的<!--[if $_get.type == "share"]-->分享<!--[else]-->收藏<!--[/if]-->.</div>
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