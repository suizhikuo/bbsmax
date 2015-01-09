<script type="text/javascript">
function enableControl( container, enable)
{   var childs;
    if( typeof( container)=="string")
        childs=$(container).childNodes;
    else
        childs=container.childNodes;
        
    if(childs.length)
    {   
        for(var i=0;i<childs.length;i++)
        {

            var tagName=childs[i].nodeName.toLowerCase();
            if(tagName=="input"||tagName=="select" || tagName=="textarea" )
            {
                childs[i].disabled=!enable;
               
            }
            else
            {
                enableControl( childs[i], enable);
            }
        }
    }    
}
</script>
    <!--[include src="../_setting_msg_.aspx"/]-->
    <!--[if $success]-->
    <div class="Tip Tip-success">操作成功</div>
    <!--[/if]-->
    <div class="FormTable">
        <!--[if $forumID==0]-->
        <div class="minitip minitip-alert">
            当前设置的是版块的全局设置，如果需要具体设置某个版块请编辑具体版块.
            <!--[if $IsForumPage == false]-->
            <a href="$admin/bbs/manage-forum.aspx">点此处进入版块列表</a>
            <!--[/if]-->
        </div>
        <!--[else]-->
        <table style="margin-bottom:1px;">
        <tr class="nohover">
            <th>
                <h4>当前版块设置是</h4>
                <p>
                <input type="radio" id="custom" name="inheritType" value="False" onclick="inheritChange()" $_form.Checked("inheritType","False","{=$ForumSetting.ForumID==$ForumID}") />
                <label for="custom">自定义</label>
                </p>
                <p>
                <input type="radio" id="inherit" name="inheritType" value="True" onclick="inheritChange()"  $_form.Checked("inheritType","True","{=$ForumSetting.ForumID!=$ForumID}") />
                <label for="inherit">继承上级</label>
                <!--[if $ForumID!=$ForumSetting.ForumID]-->
                    <!--[if $ForumSetting.ForumID == 0]-->
                    (继承至版块全局设置 <a href="$admin/bbs/manage-forum-detail.aspx?action=editsetting&forumid=0">编辑</a>)
                    <!--[else]-->
                    (继承至版块：$ForumSettingForum.ForumName <a href="$admin/bbs/manage-forum-detail.aspx?action=editsetting&forumid=$ForumSetting.ForumID">编辑</a>)
                    <!--[/if] -->
                <!--[/if]-->
                </p>
                <input id="except_canedit" name="except_canedit" value="$_form.text('except_canedit','1')" type="hidden" />
            </th>
            <td>
                如果是继承至上级，你将不能进行编辑当前版块的设置和删除例外，如果要编辑删除请选择自定义并保存(保存之后才能进行删除操作)或者编辑所继承版块的设置.
            </td>
        </tr>
        </table>
        <!--[/if]-->
        <div id="p1">
        
        <table style="margin-bottom:1px;">
        <tr>
	        <th>
	            <div class="itemtitle">
	                <strong>允许游客进入版块</strong>
	            </div>
	            <p>
      	            <input type="radio" id="allowGuestVisitForum1" name="allowGuestVisitForum" value="true"  $_form.checked("allowGuestVisitForum","true",$ForumSetting.AllowGuestVisitForum)  /> <label for="allowGuestVisitForum1">是</label>
                    <input type="radio" id="allowGuestVisitForum2" name="allowGuestVisitForum" value="false" $_form.checked("allowGuestVisitForum","false",$ForumSetting.AllowGuestVisitForum==false) /> <label for="allowGuestVisitForum2">否</label>
                </p>	    
	        </th>
	        <td>
	            <p>是否允许游客进入版块</p>
        	    
	            <p>
	                <input type="checkbox" name="allowGuestVisitForum_aplyallnode" id="allowGuestVisitForum_aplyallnode" value="true"  />
	                <label for="allowGuestVisitForum_aplyallnode">应用到所有版块</label>
	            </p>
        	    
	        </td>
        </tr>
        </table>
        
        <!--[loop $item in $ForumSetting.VisitForum with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.VisitForum.Count" name="VisitForum"  hasnode="true" nodeName="版块" title="允许注册用户进入版块" description="是否允许注册用户进入版块" /]-->
	    <!--[/loop]--> 

        <table style="margin-bottom:1px;">
        <tr>
	        <th>
	            <div class="itemtitle">
	                <strong>游客是否可以看到该版块</strong>
	            </div>
	            <p>
      	            <input type="radio" id="displayInListForGuest1" name="displayInListForGuest" value="true"  $_form.checked("displayInListForGuest","true",$ForumSetting.AllowGuestVisitForum)  /> <label for="displayInListForGuest1">是</label>
                    <input type="radio" id="displayInListForGuest2" name="displayInListForGuest" value="false" $_form.checked("displayInListForGuest","false",$ForumSetting.AllowGuestVisitForum==false) /> <label for="displayInListForGuest2">否</label>
                </p>	    
	        </th>
	        <td>
	            <p>游客是否可以看到该版块</p>
        	    
	            <p>
	                <input type="checkbox" name="displayInListForGuest_aplyallnode" id="displayInListForGuest_aplyallnode" value="true"  />
	                <label for="displayInListForGuest_aplyallnode">应用到所有版块</label>
	            </p>
        	    
	        </td>
        </tr>
        </table>

        <!--[loop $item in $ForumSetting.DisplayInList with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.DisplayInList.Count" name="DisplayInList"  hasnode="true" nodeName="版块" title="注册用户是否可以看到该版块" description="注册用户是否可以看到该版块" /]-->
	    <!--[/loop]-->


        <table style="margin-bottom:1px;">
        <tr>
	        <th>
	            <div class="itemtitle">
	                <strong>主题默认排序</strong>
	            </div>
	            <p>
      	            <input type="radio" id="ThreadSortField1" name="ThreadSortField" value="LastReplyDate"  $_form.checked("ThreadSortField","LastReplyDate",$ForumSetting.DefaultThreadSortField)  /> <label for="ThreadSortField1">按回复时间</label>
                    <input type="radio" id="ThreadSortField2" name="ThreadSortField" value="CreateDate" $_form.checked("ThreadSortField","CreateDate",$ForumSetting.DefaultThreadSortField) /> <label for="ThreadSortField2">按发表时间</label>
                </p>	    
	        </th>
	        <td>
	            <p>主题的默认排列顺序</p>
        	    
	            <p>
	                <input type="checkbox" name="ThreadSortField_aplyallnode" id="ThreadSortField_aplyallnode" value="true"  />
	                <label for="ThreadSortField_aplyallnode">应用到所有版块</label>
	            </p>
        	    
	        </td>
        </tr>
        </table>


	    <!--[loop $item in $ForumSetting.CreateThreadNeedApprove with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.CreateThreadNeedApprove.Count" name="CreateThreadNeedApprove" hasnode="true" nodeName="版块" title="发表主题需要审核" description="发表主题需要审核" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $ForumSetting.ReplyNeedApprove with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.ReplyNeedApprove.Count" name="ReplyNeedApprove" hasnode="true" nodeName="版块" title="发表回复需要审核" description="发表回复需要审核" /]-->
	    <!--[/loop]-->
	    
        <!--[loop $item in $ForumSetting.PostSubjectLengths with $index]-->
        <!--[load src="../_exceptableitem_int32scope_.ascx" index="$index" item="$item" itemCount="$ForumSetting.PostSubjectLengths.Count" name="PostSubjectLengths" hasnode="true" nodeName="版块" title="标题字数限制" description="发表主题时，标题长度必须在这个范围之内" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $ForumSetting.PostContentLengths with $index]-->
        <!--[load src="../_exceptableitem_int32scope_.ascx" index="$index" item="$item" itemCount="$ForumSetting.PostContentLengths.Count" name="PostContentLengths" hasnode="true" nodeName="版块" title="内容字数限制" description="发表帖子时，帖子内容长度必须在这个范围之内" /]-->
	    <!--[/loop]-->
	    
	    
        <table style="margin-bottom:1px;">
        <tr>
	        <th>
	            <div class="itemtitle">
	                <strong>允许使用hidden标签</strong>
	            </div>
	            <p>
      	            <input type="radio" id="enableHiddenTag_true" name="enableHiddenTag" value="true" $_form.checked("enableHiddenTag","true",$ForumSetting.enableHiddenTag)  /> <label for="enableHiddenTag_true">是</label>
                    <input type="radio" id="enableHiddenTag_false" name="enableHiddenTag" value="false" $_form.checked("enableHiddenTag","false",$ForumSetting.enableHiddenTag==false) /> <label for="enableHiddenTag_false">否</label>
                </p>	    
	        </th>
	        <td>
	            <p>是否可以在帖子中使用hidden标签</p>
        	    
	            <p>
	                <input type="checkbox" name="enableHiddenTag_aplyallnode" id="enableHiddenTag_aplyallnode" value="true"  />
	                <label for="enableHiddenTag_aplyallnode">应用到所有版块</label>
	            </p>
        	    
	        </td>
        </tr>
        </table>
        
        
        <table style="margin-bottom:1px;">
        <tr>
	        <th>
	            <div class="itemtitle">
	                <strong>回帖后默认跳转到主题的最后一页</strong>
	            </div>
	            <p>
      	            <input type="radio" id="ReplyReturnThreadLastPage1" name="ReplyReturnThreadLastPage" value="true" $_form.checked("ReplyReturnThreadLastPage","true",$ForumSetting.ReplyReturnThreadLastPage)  /> <label for="ReplyReturnThreadLastPage1">是</label>
                    <input type="radio" id="ReplyReturnThreadLastPage2" name="ReplyReturnThreadLastPage" value="false" $_form.checked("ReplyReturnThreadLastPage","false",$ForumSetting.ReplyReturnThreadLastPage==false) /> <label for="ReplyReturnThreadLastPage2">否</label>
                </p>	    
	        </th>
	        <td>
	            <p>回帖后默认跳转到主题的最后一页,如果选择了否则跳转到当前浏览的页面</p>
        	    
	            <p>
	                <input type="checkbox" name="ReplyReturnThreadLastPage_aplyallnode" id="ReplyReturnThreadLastPage_aplyallnode" value="true"  />
	                <label for="ReplyReturnThreadLastPage_aplyallnode">应用到所有版块</label>
	            </p>
        	    
	        </td>
        </tr>
        </table>
        
	    
	    <table style="margin-bottom:1px;">
        <tr>
	        <th>
	            <div class="itemtitle">
	                <strong>开启主题评级</strong>
	            </div>
	            <p>
      	            <input type="radio" id="enableRank_true" name="enableThreadRank" value="true" $_form.checked("enableThreadRank","true",$ForumSetting.enableThreadRank)  /> <label for="enableRank_true">是</label>
                    <input type="radio" id="enableRank_false" name="enableThreadRank" value="false" $_form.checked("enableThreadRank","false",$ForumSetting.enableThreadRank==false) /> <label for="enableRank_fale">否</label>
                </p>	    
	        </th>
	        <td>
	            <p>是否开启主题评级功能</p>
        	    
	            <p>
	                <input type="checkbox" name="enableThreadRank_aplyallnode" id="enableThreadRank_aplyallnode" value="true"  />
	                <label for="enableThreadRank_aplyallnode">应用到所有版块</label>
	            </p>
        	    
	        </td>
        </tr>
        </table>
	    
	    <!--[loop $item in $ForumSetting.EnableSellThread with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.EnableSellThread.Count" name="EnableSellThread" hasnode="true" nodeName="版块" title="允许出售主题" description="允许出售主题" /]-->
	    <!--[/loop]-->
	    
	    
	    <table style="margin-bottom:1px;">
	    <!--[error name = "SellThreadDays"]-->
        <tr class="nohover" id="">
            <td colspan="2" class="Message">
                <div class="Tip Tip-error">$Message</div>
                <div class="TipArrow">&nbsp;</div>
            </td>
        </tr>
        <!--[/error]-->
        <tr>
	        <th>
	            <div class="itemtitle">
	                <strong>出售主题持续时间</strong>
	            </div>
	            <p>
	                <input type="text" class="text number" name="SellThreadDaysValue" value="$_form.text('SellThreadDays',$GetTimeVale($ForumSetting.SellThreadDays))" />
	                <select name="SellThreadDaysUnit">
	                <option value="Day" $_form.selected("SellThreadDaysUnit","Day","$GetTimeUnit($ForumSetting.SellThreadDays)")>天</option>
	                <option value="Hour" $_form.selected("SellThreadDaysUnit","Hour","$GetTimeUnit($ForumSetting.SellThreadDays)")>小时</option>
	                <option value="Minute" $_form.selected("SellThreadDaysUnit","Minute","$GetTimeUnit($ForumSetting.SellThreadDays)")>分钟</option>
	                <option value="Second" $_form.selected("SellThreadDaysUnit","Second","$GetTimeUnit($ForumSetting.SellThreadDays)")>秒钟</option>
	                </select>
	            </p>	    
	        </th>
	        <td>
	            <p>如果是出售的主题，超出此时间将自动变为免费，为0则永久不免费</p>
        	    
	            <p>
	                <input type="checkbox" name="SellThreadDays_aplyallnode" id="SellThreadDays_aplyallnode" value="true"  />
	                <label for="SellThreadDays_aplyallnode">应用到所有版块</label>
	            </p>
        	    
	        </td>
        </tr>
        </table>
	    
	    <!--[loop $item in $ForumSetting.EnableSellAttachment with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.EnableSellAttachment.Count" name="EnableSellAttachment" hasnode="true" nodeName="版块" title="允许出售附件" description="允许出售附件" /]-->
	    <!--[/loop]-->
	    
	    
	     
	    <table style="margin-bottom:1px;">
	    <!--[error name = "SellAttachmentDays"]-->
        <tr class="nohover">
            <td colspan="2" class="Message">
                <div class="Tip Tip-error">$Message</div>
                <div class="TipArrow">&nbsp;</div>
            </td>
        </tr>
        <!--[/error]-->
        <tr>
	        <th>
	            <div class="itemtitle">
	                <strong>出售附件持续时间</strong>
	            </div>
	            <p>
	                <input type="text" class="text number" name="SellAttachmentDaysValue" value="$_form.text('SellAttachmentDaysValue',$GetTimeVale($ForumSetting.SellAttachmentDays))" />
	                <select name="SellAttachmentDaysUnit">
	                <option value="Day" $_form.selected("SellAttachmentDaysUnit","Day","$GetTimeUnit($ForumSetting.SellAttachmentDays)")>天</option>
	                <option value="Hour" $_form.selected("SellAttachmentDaysUnit","Hour","$GetTimeUnit($ForumSetting.SellAttachmentDays)")>小时</option>
	                <option value="Minute" $_form.selected("SellAttachmentDaysUnit","Minute","$GetTimeUnit($ForumSetting.SellAttachmentDays)")>分钟</option>
	                <option value="Second" $_form.selected("SellAttachmentDaysUnit","Second","$GetTimeUnit($ForumSetting.SellAttachmentDays)")>秒钟</option>
	                </select>
	            </p>	    
	        </th>
	        <td>
	            <p>如果是附件的主题，超出此时间将自动变为免费，为0则永久不免费</p>
        	    
	            <p>
	                <input type="checkbox" name="SellAttachmentDays_aplyallnode" id="SellAttachmentDays_aplyallnode" value="true"  />
	                <label for="SellAttachmentDays_aplyallnode">应用到所有版块</label>
	            </p>
        	    
	        </td>
        </tr>
        </table>
	    
	    <!--[loop $item in $ForumSetting.PolemizeValidDays with $index]-->
        <!--[load src="../_exceptableitem_second_.ascx" index="$index" item="$item" itemCount="$ForumSetting.PolemizeValidDays.Count" name="PolemizeValidDays" hasnode="true" nodeName="版块" title="辩论持续时间" description="超出此时间辩论将自动关闭" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $ForumSetting.PollValidDays with $index]-->
        <!--[load src="../_exceptableitem_second_.ascx" index="$index" item="$item" itemCount="$ForumSetting.PollValidDays.Count" name="PollValidDays" hasnode="true" nodeName="版块" title="投票持续时间" description="超出此时间投票将自动关闭" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $ForumSetting.QuestionValidDays with $index]-->
        <!--[load src="../_exceptableitem_second_.ascx" index="$index" item="$item" itemCount="$ForumSetting.QuestionValidDays.Count" name="QuestionValidDays" hasnode="true" nodeName="版块" title="问题帖持续时间" description="超出此时间将自动结帖" /]-->
	    <!--[/loop]-->
    	
	    <!--[loop $item in $ForumSetting.CreatePostIntervals with $index]-->
        <!--[load src="../_exceptableitem_second_.ascx" index="$index" item="$item" itemCount="$ForumSetting.CreatePostIntervals.Count" name="CreatePostIntervals" hasnode="true" nodeName="版块" title="发帖子时间间隔" description="用户发新帖子后需隔多久才能继续发帖子，为0则不限制" /]-->
	    <!--[/loop]-->
    	
	    <!--[loop $item in $ForumSetting.RecycleOwnThreadsIntervals with $index]-->
        <!--[load src="../_exceptableitem_second_.ascx" index="$index" item="$item" itemCount="$ForumSetting.RecycleOwnThreadsIntervals.Count" name="RecycleOwnThreadsIntervals" hasnode="true" nodeName="版块" title="多久以内可以回收自己的主题" description="用户发主题后多久以内才允许回收该主题，为0则不限制" /]-->
	    <!--[/loop]-->
    	
	    <!--[loop $item in $ForumSetting.DeleteOwnThreadsIntervals with $index]-->
        <!--[load src="../_exceptableitem_second_.ascx" index="$index" item="$item" itemCount="$ForumSetting.DeleteOwnThreadsIntervals.Count" name="DeleteOwnThreadsIntervals" hasnode="true" nodeName="版块" title="多久以内可以删除自己的主题" description="用户发主题后多久以内才允许删除该主题，为0则不限制" /]-->
	    <!--[/loop]-->
    	
    	
	    <!--[loop $item in $ForumSetting.UpdateOwnPostIntervals with $index]-->
        <!--[load src="../_exceptableitem_second_.ascx" index="$index" item="$item" itemCount="$ForumSetting.UpdateOwnPostIntervals.Count" name="UpdateOwnPostIntervals" hasnode="true" nodeName="版块" title="多久以内可以编辑自己的帖子" description="用户发帖后多久以内才允许编辑该帖子，为0则不限制" /]-->
	    <!--[/loop]-->
    	
	    <!--[loop $item in $ForumSetting.UpdateThreadSortOrderIntervals with $index]-->
        <!--[load src="../_exceptableitem_second_.ascx" index="$index" item="$item" itemCount="$ForumSetting.UpdateThreadSortOrderIntervals.Count" name="UpdateThreadSortOrderIntervals" hasnode="true" nodeName="版块" title="多久没回复的主题回复后主题不会被顶上去" description="从主题的最后一次回复开始算，经过了此时间再回复，主题不会被顶上去，为0则不限制" /]-->
	    <!--[/loop]-->
    	
	    <!--[loop $item in $ForumSetting.AllowAttachment with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.AllowAttachment.Count" name="AllowAttachment" hasnode="true" nodeName="版块" title="允许上传附件" description="允许上传附件" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.AllowFileExtensions with $index]-->
        <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$ForumSetting.AllowFileExtensions.Count" name="AllowFileExtensions" hasnode="true" nodeName="版块" type="ExtensionList" textboxheight="3" title="允许上传的附件类型" description="填写文件扩展名不包括点,用逗号“,”分隔,“*”表示允许所有类型,如“<span style=\"color:red\">jpg,gif,rar</span>”" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.MaxSignleAttachmentSize with $index]-->
        <!--[load src="../_exceptableitem_filesize_.ascx" index="$index" item="$item" itemCount="$ForumSetting.MaxSignleAttachmentSize.Count" name="MaxSignleAttachmentSize" hasnode="true" nodeName="版块" title="单个附件最大大小" description="单个附件最大大小，为0则不限制" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.AllowImageAttachment with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.AllowImageAttachment.Count" name="AllowImageAttachment" hasnode="true" nodeName="版块" title="允许显示图片类型的附件" description="是否允许图片类型的附件在帖子中以图片显示，如果不允许将按一般附件类型显示" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.MaxTopicAttachmentCount with $index]-->
        <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$ForumSetting.MaxTopicAttachmentCount.Count" name="MaxTopicAttachmentCount" hasnode="true" nodeName="版块" type="int" textboxwidth="4" title="主题最大附件个数" description="发表主题时允许上传附件的最大个数，为0则不限制(<span style=\"color:red\">注意,如果大于“每天可以上传附件个数”则要相应的修改“<a href=\"setting-bbs.aspx\">每天可以上传附件个数</a>”</span>)" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.MaxPostAttachmentCount with $index]-->
        <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$ForumSetting.MaxPostAttachmentCount.Count" name="MaxPostAttachmentCount" hasnode="true" nodeName="版块" type="int" textboxwidth="4" title="帖子最大附件个数" description="发表回复时允许上传附件的最大个数， 为0则不限制(<span style=\"color:red\">注意,如果大于“每天可以上传附件个数”则要相应的修改“<a href=\"setting-bbs.aspx\">每天可以上传附件个数</a>”</span>)" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.ShowSignatureInThread with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.ShowSignatureInThread.Count" name="ShowSignatureInThread" hasnode="true" nodeName="版块" title="主题允许使用签名" description="发表主题时是否允许用户勾选使用签名" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.ShowSignatureInPost with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.ShowSignatureInPost.Count" name="ShowSignatureInPost" hasnode="true" nodeName="版块" title="回复允许使用签名" description="发表回复时是否允许用户勾选使用签名" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.CreatePostAllowEmoticon with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.CreatePostAllowEmoticon.Count" name="CreatePostAllowEmoticon" hasnode="true" nodeName="版块" title="允许使用表情" description="发帖时是否允许使用表情" /]-->
	    <!--[/loop]-->
	    
	    
	    <!--[loop $item in $ForumSetting.CreatePostAllowHTML with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.CreatePostAllowHTML.Count" name="CreatePostAllowHTML" hasnode="true" nodeName="版块" title="允许使用HTML" description="发帖时是否允许使用HTML" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.CreatePostAllowMaxcode with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.CreatePostAllowMaxcode.Count" name="CreatePostAllowMaxcode" hasnode="true" nodeName="版块" title="允许使用UBB" description="发帖时是否允许使用UBB" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.CreatePostAllowImageTag with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.CreatePostAllowImageTag.Count" name="CreatePostAllowImageTag" hasnode="true" nodeName="版块" title="允许使用img标签" description="发帖时是否允许使用img标签" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.CreatePostAllowFlashTag with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.CreatePostAllowFlashTag.Count" name="CreatePostAllowFlashTag" hasnode="true" nodeName="版块" title="允许使用Flash标签" description="发帖时是否允许使用Flash标签" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.CreatePostAllowAudioTag with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.CreatePostAllowAudioTag.Count" name="CreatePostAllowAudioTag" hasnode="true" nodeName="版块" title="允许使用Audio标签" description="发帖时允许使用Audio标签" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.CreatePostAllowVideoTag with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.CreatePostAllowVideoTag.Count" name="CreatePostAllowVideoTag" hasnode="true" nodeName="版块" title="允许使用Video标签" description="发帖时是否允许使用Video标签" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.CreatePostAllowTableTag with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.CreatePostAllowTableTag.Count" name="CreatePostAllowTableTag" hasnode="true" nodeName="版块" title="允许使用Table标签" description="发帖时是否允许使用Table标签" /]-->
	    <!--[/loop]-->
	    
	    <!--[loop $item in $ForumSetting.CreatePostAllowUrlTag with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumSetting.CreatePostAllowUrlTag.Count" name="CreatePostAllowUrlTag" hasnode="true" nodeName="版块" title="允许使用Url标签" description="发帖时是否允许使用Url标签" /]-->
	    <!--[/loop]-->
	    </div>
	    <table class="multiColumns">
	    <tr class="nohover">
		    <td style="width:15em;">&nbsp;</td>
		    <td><input type="submit" value="保存设置" class="button" name="saveforumsettings" /></td>
	    </tr>
	    </table>
    </div>
    
    <!--[if $ForumID!=0]-->
    <script type="text/javascript">
    inheritChange();
    function inheritChange()
    {
        if($("custom").checked)
        {
            $("except_canedit").value="1";
            enableControl("p1",true);
        }
        else
        {
            $("except_canedit").value="0";
            enableControl("p1",false);
        }
    }
    </script>
    <!--[/if]-->
