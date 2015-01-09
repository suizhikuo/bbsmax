<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>举报管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<div class="Content">
    <h3>举报管理</h3>
	<div class="SearchTable">
    <form id="filter" action="$_form.action" method="post">
    <table>
        <tr>
            <td>举报类型</td>
            <td>
                <select name="Type">
                <option value="All" $_Form.selected('Type','All',$filter.Type==DenouncingType.All)>所有</option>
                <option value="Photo" $_Form.selected('Type','Photo',$filter.Type==DenouncingType.Photo)>相片</option>
                <option value="Blog" $_Form.selected('Type','Blog',$filter.Type==DenouncingType.Blog)>日志</option>
                <option value="Space" $_Form.selected('Type','Space',$filter.Type==DenouncingType.Space)>空间</option>
                <option value="Share" $_Form.selected('Type','Share',$filter.Type==DenouncingType.Share)>分享</option>
                <option value="Topic" $_Form.selected('Type','Topic',$filter.Type==DenouncingType.Topic)>主题</option>
                <option value="Reply" $_Form.selected('Type','Reply',$filter.Type==DenouncingType.Reply)>帖子</option>
                </select>
            </td>
            <td>举报状态</td>
            <td colspan="3">
                <select name="ReportState">
                <option value="2" $_Form.selected('ReportState','2',$filter.ReportState==2)>不限</option>
                <option value="1" $_Form.selected('ReportState','1',$filter.ReportState==1)>已忽略</option>
                <option value="0" $_Form.selected('ReportState','0',$filter.ReportState==0)>待处理</option>
                </select>
            </td>
        </tr>
        <tr>
            <td>结果排序</td>
            <td>
                <select name="IsDesc">
                <option value="True" $_Form.selected('IsDesc','True',$filter.IsDesc==true)>按举报时间降序排列</option>
                <option value="False" $_Form.selected('IsDesc','Flase',$filter.IsDesc==false)>按举报时间升序排列</option>
                </select>
            </td>
            <td>每页显示数</td>
            <td>
                <select name="PageSize">
                <option value="10" $_Form.selected('PageSize','10',$filter.PageSize==10)>10</option>
                <option value="20" $_Form.selected('PageSize','20',$filter.PageSize==20)>20</option>
                <option value="50" $_Form.selected('PageSize','50',$filter.PageSize==50)>50</option>
                <option value="100" $_Form.selected('PageSize','100',$filter.PageSize==100)>100</option>
                <option value="200" $_Form.selected('PageSize','200',$filter.PageSize==200)>200</option>
                <option value="500" $_Form.selected('PageSize','500',$filter.PageSize==500)>100</option>
                </select>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td colspan="3"><input type="submit" class="button" value="搜索" name="searchreport"/></td>
        </tr>
    </table>
    </form>
	</div>

	<div class="DataTable">
    <form id="reportResults" name="reportResults" action="$_form.action" method="post" accept-charset=">
        <h4>举报</h4>
        <!--[if $DenouncingTotalCount > 0]-->
        
        <table>
          <thead>
            <tr>
	            <td class="checkboxHold" style="width:24px;">&nbsp;</td>
              <td style="width:130px;"></td>
              <td style="width:200px"></td>
              <td></td>
            </tr>
          </thead>
          <tbody>
		      <!--[loop $Denouncing in $DenouncingList]-->
            <tr>
              <td valign="top"><input type="checkbox" name="denouncingIDs" value="$Denouncing.ID" /></td>
              <td valign="top">
                <p><a href="$IgnoreDenouncingUrl($Denouncing.ID)">忽略该举报</a></p>
                <!--[if $Denouncing.Type==DenouncingType.Photo]-->
                  <!--[if $CanDeletePhoto($Denouncing.TargetPhoto)]-->
                  <p><a href="$DeleteDenouncingUrl($Denouncing.ID)" onclick="return confirm('确认要删除吗?删除后不可恢复!');">删除该相片</a></p>
                  <!--[else]-->
                  <p><a href="$dialog/denouncing-sendnotify.aspx?tid=$denouncing.targetid&uid=$denouncing.targetuserid&type=$denouncing.type" onclick="return openDialog(this.href)">通知作者删除该相片</a></p>
                  <!--[/if]-->
                <!--[else if $Denouncing.Type==DenouncingType.Blog]-->
                  <!--[if $CanDeleteArticle($Denouncing.TargetArticle)]-->
                  <p><a href="$DeleteDenouncingUrl($Denouncing.ID)" onclick="return confirm('确认要删除吗?删除后不可恢复!');">删除该日志</a></p>
                  <!--[else]-->
                  <p><a href="$dialog/denouncing-sendnotify.aspx?tid=$denouncing.targetid&uid=$denouncing.targetuserid&type=$denouncing.type" onclick="return openDialog(this.href)">通知作者删除该日志</a></p>
                  <!--[/if]-->
                <!--[else if $Denouncing.Type==DenouncingType.Share]-->
                  <!--[if $CanDeleteShare($Denouncing.TargetShare)]-->
                  <p><a href="$DeleteDenouncingUrl($Denouncing.ID)" onclick="return confirm('确认要删除吗?删除后不可恢复!');">删除该分享</a></p>
                  <!--[else]-->
                  <p><a href="$dialog/denouncing-sendnotify.aspx?tid=$denouncing.targetid&uid=$denouncing.targetuserid&type=$denouncing.type" onclick="return openDialog(this.href)">通知作者删除该分享</a></p>
                  <!--[/if]-->
                <!--[else if $Denouncing.Type==DenouncingType.Space]-->
                  <!--[if $CanDeleteUser($Denouncing.TargetUser)]-->
                  <p><a href="$DeleteDenouncingUrl($Denouncing.ID)" onclick="return confirm('确认要删除吗?删除后不可恢复!');">删除该用户</a></p>
                  <!--[else]-->
                  <%--<p><a href="$dialog/denouncing-sendnotify.aspx?tid=$denouncing.targetid&uid=$denouncing.targetuserid&type=$denouncing.type" onclick="return openDialog(this.href)">通知作者删除该分享</a></p>--%>
                  <!--[/if]-->
                <!--[else if $Denouncing.Type==DenouncingType.Topic]-->
                  <!--[if $CanDeleteTopic($Denouncing.TargetTopic)]-->
                  <p><a href="$DeleteDenouncingUrl($Denouncing.ID)" onclick="return confirm('确认要删除吗?删除后不可恢复!');">删除该主题</a></p>
                  <!--[else]-->
                  <p><a href="$dialog/denouncing-sendnotify.aspx?tid=$denouncing.targetid&uid=$denouncing.targetuserid&type=$denouncing.type" onclick="return openDialog(this.href)">通知作者删除该主题</a></p>
                  <!--[/if]-->
                <!--[else if $Denouncing.Type==DenouncingType.Reply]-->
                  <!--[if $CanDeleteReply($Denouncing.TargetReply)]-->
                  <p><a href="$DeleteDenouncingUrl($Denouncing.ID)" onclick="return confirm('确认要删除吗?删除后不可恢复!');">删除该帖子</a></p>
                  <!--[else]-->
                  <p><a href="$dialog/denouncing-sendnotify.aspx?tid=$denouncing.targetid&uid=$denouncing.targetuserid&type=$denouncing.type" onclick="return openDialog(this.href)">通知作者删除该帖子</a></p>
                  <!--[/if]-->
                <!--[/if]-->
              </td>
              <td valign="top">
				        <ul>
				        <!--[loop $content in $Denouncing.ContentList]-->
					        <li>$content.User.PopupNameLink：$Content.Content ($Content.CreateDate)</li>
				        <!--[/loop]-->
				        </ul>
				      </td>
              <td>
                <!--[if $Denouncing.Type==DenouncingType.Photo]-->
                <a href="$url(app/album/photo)?id=$Denouncing.TargetID" target="_blank"><img src="$Denouncing.TargetPhoto.ThumbSrc" /></a>
                <!--[else if $Denouncing.Type==DenouncingType.Blog]-->
                <a href="$url(app/blog/view)?id=$Denouncing.TargetID" target="_blank">$Denouncing.TargetArticle.OriginalSubject</a>
                <!--[else if $Denouncing.Type==DenouncingType.Space]-->
                $Denouncing.TargetUser.AvatarLink $Denouncing.TargetUser.PopupNameLink
                <!--[else if $Denouncing.Type==DenouncingType.Share]-->
                      <!--[if $Denouncing.TargetShare.Type == ShareType.Video]-->
                      <div class="videoplayer">
                          <img src="$Root/max-assets/images/default_media.gif" alt="" title="点击播放" width="120" height="90" />
                          <a class="video-play" href="#" title="播放该视频" onclick="javascript:showFlash('$Root', '$Denouncing.TargetShare.Video.Domain', '$Denouncing.TargetShare.Video.VideoID', this.parentNode, '$Denouncing.TargetShare.usershareid');" >播放该视频</a>
                      </div>
                      <!--[else if $Denouncing.TargetShare.Type == ShareType.Music]-->
                      <div class="audioplayer">
                          <object id="audioplayer_$Denouncing.TargetShare.usershareid" height="24" width="290" data="/max-assets/flash/player.swf" type="application/x-shockwave-flash">
                              <param value="/max-assets/flash/player.swf" name="movie" />
                              <param value="autostart=no&bg=0xEBF3F8&leftbg=0x6B9FCE&lefticon=0xFFFFFF&rightbg=0x6B9FCE&rightbghover=0x357DCE&righticon=0xFFFFFF&righticonhover=0xFFFFFF&text=0x357DCE&slider=0x357DCE&track=0xFFFFFF&border=0xFFFFFF&loader=0xAF2910&soundFile=$Denouncing.TargetShare.Content" name="FlashVars" />
                              <param value="high" name="quality" />
                              <param value="false" name="menu" />
                              <param value="#ffffff" name="bgcolor" />
                          </object>
                      </div>
                      <!--[else]-->
                      <div class="contentwrapper">
                          <a href="$Denouncing.TargetShare.Content" target="_blank">$Denouncing.TargetShare.Content</a>
                      </div>
                      <!--[/if]-->
                      <!--[if $Denouncing.TargetShare.description != null && $Denouncing.TargetShare.description.length > 0]-->
                      <div class="description">
                          <div class="description-inner">
                              $Denouncing.TargetShare.description
                          </div>
                      </div>
                      <!--[/if]-->
                <!--[else if $Denouncing.Type==DenouncingType.Topic]-->
                    <!--[if $Denouncing.TargetTopic == null]-->
                    该主题已被删除
                    <!--[else]-->
                    <a href="$GetThreadUrl($Denouncing.TargetID)" target="_blank">
                    $Denouncing.TargetTopic.SubjectText
                    </a>
                    <!--[/if]-->
                <!--[else if $Denouncing.Type==DenouncingType.Reply]-->
                    <!--[if $Denouncing.TargetReply == null]-->
                    该帖子已被删除
                    <!--[else]-->
                    <a href="$GetPostThreadUrl($Denouncing.TargetID)?type=getpost&postid=$Denouncing.TargetID" target="_blank">查看内容</a><br />
                    $Denouncing.TargetReply.Content
                    <!--[/if]-->
                <!--[/if]-->
              </td>
				    </tr>
		      <!--[/loop]-->
		      </tbody>
        </table>
        
        <%-- 
        <table>
        <thead>
	        <tr>
	            <td class="checkboxHold" style="width:24px;">&nbsp;</td>
	            <td style="width:50px;">类型</td>
	            <td style="width:50px;">状态</td>
	            <td style="width:130px;">操作</td>
	            <td>举报</td>
	        </tr>
        </thead>
        <tbody>
        <!--[/if]-->
        
		<!--[loop $Denouncing in $DenouncingList]-->
            <tr>
                <td valign="top"><input type="checkbox" name="denouncingIDs" value="$Denouncing.ID" /></td>
                <td valign="top">
                <!--[if $Denouncing.Type==DenouncingType.Photo]-->相片
                <!--[else if $Denouncing.Type==DenouncingType.Blog]-->日志
                <!--[else if $Denouncing.Type==DenouncingType.Space]-->空间
                <!--[else if $Denouncing.Type==DenouncingType.Share]-->分享
                <!--[else if $Denouncing.Type==DenouncingType.Topic]-->主题
                <!--[else if $Denouncing.Type==DenouncingType.Reply]-->帖子
                <!--[/if]-->
				</td>
                <td valign="top">$_if($Denouncing.IsIgnore,"已忽略","待处理")</td>
                <td valign="top">
                    <!--[if $Denouncing.Type==DenouncingType.Photo]-->
                    <p><a href="$url(space/$Denouncing.TargetUserID/album/photo-$Denouncing.TargetID)" target="_blank">查看该相片</a></p>
                    <p><a href="$IgnoreDenouncingUrl($Denouncing.ID)">忽略该举报</a></p>
                    <p><a href="$DeleteDenouncingUrl($Denouncing.ID)" onclick="return confirm('确认要删除吗?删除后不可恢复!');">删除该相片和举报</a></p>
                    <!--[else if $Denouncing.Type==DenouncingType.Blog]-->
                    <p><a href="$url(space/$Denouncing.TargetUserID/blog/article-$Denouncing.TargetID)" target="_blank">查看该日志</a></p>
                    <p><a href="$IgnoreDenouncingUrl($Denouncing.ID)">忽略该举报</a></p>
                    <p><a href="$DeleteDenouncingUrl($Denouncing.ID)" onclick="return confirm('确认要删除吗?删除后不可恢复!');">删除该日志和举报</a></p>
                    <!--[else if $Denouncing.Type==DenouncingType.Share]-->
                    <p><a href="$url(space/$Denouncing.TargetUserID/share/share-$Denouncing.TargetID)" target="_blank">查看该分享</a></p>
                    <p><a href="$IgnoreDenouncingUrl($Denouncing.ID)">忽略该举报</a></p>
                    <p><a href="$DeleteDenouncingUrl($Denouncing.ID)" onclick="return confirm('确认要删除吗?删除后不可恢复!');">删除该分享和举报</a></p>
                    <!--[else if $Denouncing.Type==DenouncingType.Space]-->
                    <p><a href="$url(space/$Denouncing.TargetID)" target="_blank">点击查看该用户</a></p>
                    <p><a href="$IgnoreDenouncingUrl($Denouncing.ID)">忽略该举报</a></p>
                    <p><a href="$DeleteDenouncingUrl($Denouncing.ID)" onclick="return confirm('确认要删除吗?删除后不可恢复!');">删除该用户和举报</a></p>
                    <!--[else if $Denouncing.Type==DenouncingType.Topic]-->
                    <p><a href="$GetThreadUrl($Denouncing.TargetID)" target="_blank">查看该主题</a></p>
                    <p><a href="$IgnoreDenouncingUrl($Denouncing.ID)">忽略该举报</a></p>
                    <p><a href="$DeleteDenouncingUrl($Denouncing.ID)" onclick="return confirm('确认要删除吗?删除后不可恢复!');">删除该主题和举报</a></p>
                    <!--[else if $Denouncing.Type==DenouncingType.Reply]-->
                    <p><a href="$GetPostThreadUrl($Denouncing.TargetID)?type=getpost&postid=$Denouncing.TargetID" target="_blank">查看该帖子</a></p>
                    <p><a href="$IgnoreDenouncingUrl($Denouncing.ID)">忽略该举报</a></p>
                    <p><a href="$DeleteDenouncingUrl($Denouncing.ID)" onclick="return confirm('确认要删除吗?删除后不可恢复!');">删除该帖子和举报</a></p>
                    <!--[/if]-->
                </td>
                <td valign="top">
					<ul>
					<!--[loop $content in $Denouncing.ContentList]-->
						<li>$content.User.PopupNameLink：$Content.Content ($Content.CreateDate)</li>
					<!--[/loop]-->
					</ul>
                </td>
            </tr>
		<!--[/loop]-->
        <!--[if $DenouncingTotalCount > 0]-->
        </tbody>
        </table>
        --%>
        <div class="Actions">
            <input type="checkbox" name="checkAll" value="checkbox" id="checkAll"/>
            <label for="checkAll">全选</label>
            <input name="btn_ignore" class="button" onclick="return confirm('确认要忽略吗?忽略后不可恢复!');" type="submit" value="忽略选中的举报"/>
            <input name="btn_deleteboth" class="button"  onclick="return confirm('确认删除搜索到的数据吗?删除后不可恢复!!');" type="submit"  value="删除选中的举报内容" />
		            
			<script type="text/javascript">
			   new checkboxList( 'denouncingIDs', 'checkAll');
			</script>
        </div>
	    <!--[AdminPager Count="$DenouncingTotalCount" PageSize="$Filter.PageSize"/]-->
        <!--[else]-->
        <div class="NoData">未搜索到任何记录.</div>
        <!--[/if]-->
    </form>
    </div>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>