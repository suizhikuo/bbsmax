<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/publish.css" />
<link rel="stylesheet" type="text/css" href="$root/max-assets/kindeditor/skins/editor5.css" />
<script src="$root/max-assets/javascript/max-post.js" type="text/javascript"></script>
<script type="text/javascript" src="$url(handler/emotelib)?userid=$CurrentPostUserID&action=post&targetid=$PostID" defer="defer"></script>
<script type="text/javascript">
var deleteTempFileUrl = '$url(handler/DeleteTempFile)';
var attachUrl = "$url(handler/down)?action=attach";
</script>
</head>
<body>
<div class="container section-publish">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main rightsidebar">
        <div class="clearfix main-inner">
            <div class="publishcaption">
                <h3 class="publishcaption-title"><span>发帖</span></h3>
            </div>
            
            <div class="content">
                <!--#include file="../_inc/_round_top.aspx"-->
                <div class="clearfix content-inner">
                    <form id="formpost" method="post" enctype="multipart/form-data" action="$_form.action">
                    <div class="content-main">
                        <div class="content-main-inner">
                            
                            <!--[if $IsCreateThread]-->
                            <div class="publishtype-list">
                                <!--[ajaxpanel id="ap_topictype"]-->
                                <ul class="clearfix">
                                    <!--[if $CanCreateThread]-->
                                    <li><a <!--[if $action == "thread"]-->class="checked"<!--[/if]--> href="$url($codename/post)?action=thread" onclick="return ajaxRender(this.href, '*');">主题</a></li>
                                    <!--[/if]-->
                                    <!--[if $CanCreatePoll]-->
                                    <li><a <!--[if $action == "poll"]-->class="checked"<!--[/if]--> href="$url($codename/post)?action=poll" onclick="return ajaxRender(this.href, '*');">投票</a></li>
                                    <!--[/if]-->
                                    <!--[if $CanCreateQuestion]-->
                                    <li><a <!--[if $action == "question"]-->class="checked"<!--[/if]--> href="$url($codename/post)?action=question" onclick="return ajaxRender(this.href, '*');">提问</a></li>
                                    <!--[/if]-->
                                    <!--[if $CanCreatePolemize]-->
                                    <li><a <!--[if $action == "polemize"]-->class="checked"<!--[/if]--> href="$url($codename/post)?action=polemize" onclick="return ajaxRender(this.href, '*');">辩论</a></li>
                                    <!--[/if]-->
                                </ul>
                                <!--[/ajaxpanel]-->
                            </div>
                            <!--[/if]-->
                            
                            <!--[ajaxpanel id="ap_review" idonly="true"]-->
                            <!--[if $review]-->
                            <div id="reviewPanel" class="clearfix formcaption publishformcaption">
                                <h3 class="formcaption-title">帖子预览 <a href="$url($codename/post)?action=$action&Review=false&threadID=$threadID&postID=$postID" onclick="return cancellReviewButton(this.href);">[取消预览]</a></h3>
                            </div>
                            <div class="publishpreview">
                                <div class="post-heading">
                                    <h1>$reviewSubject</h1>
                                </div>
                                <div class="clearfix post-content">$reviewContent</div>
                            </div>
                            <!--[/if]-->
                            <!--[/ajaxpanel]-->
                            
                            <div class="formgroup publishform" id="topicdatas">
                                <!--[ajaxpanel id="ap_error" idonly="true"]-->
                                <!--[unnamederror]-->
                                <div id="errormsg" class="errormsg">$Message</div>
                                <script type="text/javascript">
                                    location.href = '#errormsg';
                                </script>
                                <!--[/unnamederror]-->
                                <!--[/ajaxpanel]-->
                                <!--[if $IsShowNoUpdateSortOrder]-->
                                <div class="alertmsg">由于该主题太长时间未被回复, 您的回复不会将该主题顶上去.</div>
                                <!--[/if]-->
                                
                                <!--[if $IsLogin==false && $EnableGuestNickName]-->
                                <div class="formrow publishform-guestname">
                                    <h3 class="label"><label for="guestNickName">昵称</label></h3>
                                    <div class="form-enter">
                                        <input type="text" class="text" name="guestNickName" id="guestNickName" value="$_form.text("guestNickName")" />
                                    </div>
                                </div>
                                <!--[/if]-->
                                
                                <div class="formrow publishform-title">
                                    <h3 class="label"><label for="title">标题</label></h3>
                                    <div class="clearfix form-enter">
                                        <!--[if $EnablePostIcon]-->
                                        <div class="topic-moodicon">
                                            <input id="postID" name="postID" type="hidden" value="$_form.text("postID","$postID")" />
                                            <input type="hidden" name="postIcon" value="$_form.text("postIcon","$PostIconID")" />
                                            <a id="posticon-trigger" class="dropdown-trigger" href="javascript:;" title="选择帖子图标">
                                                <img src="$root/max-assets/icon-post/icon0.gif" alt="" height="19" width="19" id="post_icon" />
                                            </a>
                                        </div>
                                        <div id="posticon-list" class="dropdownmenu-wrap topic-moodicon-list" style="display:none;">
                                            <div class="dropdownmenu">
                                                <div class="clearfix dropdownmenu-list">
                                                    <a href="#"><img src="$root/max-assets/icon-post/icon0.gif" alt="0" onclick="iconSelected(this);return false;" /></a>
                                                    <!--[loop $PostIcon in $PostIcons]-->
                                                    <a href="#"><img src="$postIcon.IconUrl" alt="$postIcon.IconID" onclick="iconSelected(this);return false;" /></a>
                                                    <!--[/loop]-->
                                                </div>
                                            </div>
                                        </div>
                                        <!--[/if]-->
                                        <!--[if $IsReply == false && $ThreadCatalogs.Count > 0 && $Forum.ThreadCatalogStatus != ThreadCatalogStatus.DisEnable]-->
                                        <div class="topic-category">
                                            <input type="hidden" name="threadCatalogs" value="0" />
                                            <a id="threadcatalog-trigger" class="dropdown-trigger" href="javascript:;" title="选择帖子分类">
                                                <span id="catalogname">选择分类</span>
                                            </a>
                                        </div>
                                        <div id="threadcatalog-list" class="dropdownmenu-wrap topic-category-list" style="display:none;">
                                            <div class="dropdownmenu">
                                                <ul class="dropdownmenu-list">
                                                    <!--[if $Forum.ThreadCatalogStatus!=ThreadCatalogStatus.EnableAndMust]-->
                                                    <li><a id="catalog_0" href="javascript:;" onclick="return catalogSelected(0);">不选</a></li>
                                                    <!--[/if]-->
                                                    <!--[loop $catalog in $ThreadCatalogs]-->
                                                    <li><a id="catalog_$catalog.threadcatalogid" href="javascript:;" onclick="return catalogSelected($catalog.threadcatalogid);" title="$ProcessCatelogName($catalog.threadcatalogname)" <!--[if $selectCatalogID == $catalog.threadcatalogid]-->class="checked"<!--[/if]-->>$catalog.threadcatalogname</a></li>
                                                    <!--[/loop]-->
                                                </ul>
                                            </div>
                                        </div>
                                        <script type="text/javascript">
                                        new popup('threadcatalog-list','threadcatalog-trigger',false);
                                        catalogSelected($selectCatalogID);
                                        function catalogSelected(id) {

                                            var obj = $('catalog_'+id);
                                            if(obj==null)
                                                id = 0;
                                            document.getElementsByName('threadCatalogs')[0].value=id;
                                            if (id == 0) {
                                                $('catalogname').innerHTML = "选择分类";
                                            }
                                            else {
                                                $('catalogname').innerHTML=obj.title;
                                            }
                                            
                                            $('threadcatalog-list').style.display='none';
                                            return false;
                                        }
                                        </script>
                                        <!--[/if]-->
                                        <div class="topic-title">
                                            <input type="text" class="text" name="subject" id="subject" value="$_form.text("subject","$Subject")" />
                                        </div>
                                    </div>
                                </div>
                                
                                <!--[if $IsReply && $Thread.ThreadType == ThreadType.Polemize]-->
                                <div class="formrow">
                                    <h3 class="label">选择您的立场</h3>
                                    <div class="clearfix form-enter">
                                        <input name="viewPointType" id="viewPointType1" type="radio" value="2" $_form.checked("viewPointType","2") />
                                        <label for="viewPointType1">正方</label>
                                        <input name="viewPointType" id="viewPointType3" type="radio" value="4" $_form.checked("viewPointType","4") />
                                        <label for="viewPointType3">反方</label>
                                        <input name="viewPointType" id="viewPointType2" type="radio" value="3" $_form.checked("viewPointType","3") />
                                        <label for="viewPointType2">中立</label>
                                    </div>
                                </div>
                                <!--[/if]-->
                                
                           <!--[ajaxpanel id="ap_topicdatas"]-->
                                <input type="hidden" name="postaction" value="$action" />
                                <!--[if $isShowPollOptions]-->
                                <div class="formrow publishform-pollitem">
                                    <h3 class="label"><label for="vote">投票项</label> <span class="form-note">(一行一个选项, 最多可使用$MaxPollItemCount项,允许使用“[img]”)</span></h3>
                                    <div class="textarea-wrap">
                                        <input type="hidden" name="pollItemCount" id="pollItemCount" value="2" />
                                        <textarea cols="40" rows="4" id="vote" name="vote" <!--[if $IsEditThread]-->disabled="disabled"<!--[/if]-->>$_form.text("vote",$PollItemString)</textarea>
                                    </div>
                                </div>
                                <div class="clearfix combineformrow">
                                    <div class="combineformrow-col-1">
                                        <div class="formrow publishform-polloptional">
                                            <h3 class="label">投票类型</h3>
                                            <div class="clearfix form-enter">
                                                <%--
                                                <ul class="optionlist">
                                                    <li>
                                                        <input type="radio" name="optional" id="optional_1" />
                                                        <label for="optional_1">单项选择投票</label>
                                                    </li>
                                                    <li>
                                                        <input type="radio" name="optional" id="optional_2" />
                                                        <label for="optional_2">多项选择投票</label>
                                                        <div class="publishform-polloptional-min">
                                                            <label for="optional_min">至少必选项数目</label>
                                                            <input type="text" class="text number" name="optional_min" id="optional_min" />
                                                        </div>
                                                        <div class="publishform-polloptional-max">
                                                            <label for="voteMultiple">最多可选项数目</label>
                                                            <input type="text" class="text number" id="voteMultiple" name="voteMultiple" onkeyup="value=value.replace(/[^\d]/g,'');" value="$PollMultiple" <!--[if $IsEditThread]-->disabled="disabled"<!--[/if]-->/>
                                                        </div>
                                                    </li>
                                                </ul>
                                                --%>
                                                <label for="voteMultiple">最多可选项数目</label>
                                                <input type="text" class="text number" id="voteMultiple" name="voteMultiple" onkeyup="value=value.replace(/[^\d]/g,'');" value="$PollMultiple" <!--[if $IsEditThread]-->disabled="disabled"<!--[/if]-->/>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="combineformrow-col-2">
                                        <div class="formrow publishform-datetime">
                                            <h3 class="label"><label for="expiresDays">有效时间</label> <span class="form-note">(最多$time$timeUnit, 0或空为允许的最大值)</span></h3>
                                            <div class="clearfix form-enter">
                                                <!--[if $IsShowPollExpiresDate]-->
                                                    <!--[if $Poll.ExpiresDate == DateTime.MaxValue]-->
                                                无限期
                                                    <!--[else]-->
                                                $Poll.ExpiresDate
                                                    <!--[/if]-->
                                                <!--[else]-->
                                                <input name="expiresDays" id="expiresDays" onkeyup="value=value.replace(/[^\d]/g,'');" value="$time" type="text" class="text number" />
                                                $timeUnit
                                                <!--[/if]-->
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <!--[else if $isShowQuestionOptions]-->
                                <div class="clearfix combineformrow">
                                    <div class="combineformrow-col-1">
                                        <div class="formrow publishform-askaward">
                                            <h3 class="label">悬赏设置 <span class="form-note">将扣除<span id="realReward"><!--[if $IsEditThread]-->$realReward<!--[else]-->0<!--[/if]--></span>$QuestionRewardPoint.UnitName, $QuestionRewardScope</span></h3>
                                            <div class="form-enter">
                                                <label for="askaward">奖励$QuestionRewardPoint.Name</label>
                                                <input type="text" class="text number" name="reward" id="reward" onkeyup="value=value.replace(/[^\d]/g,'');if(value==0)value='';setRealReward($rewardTax)"  <!--[if $IsEditThread]-->value="$question.Reward" disabled="disabled"<!--[else]-->value="$_form.text("reward")"<!--[/if]--> /> $QuestionRewardPoint.UnitName
                                                您目前有$QuestionRewardPoint.Name: $My.GetPoint($QuestionRewardPoint.Type)$QuestionRewardPoint.UnitName.
                                            </div>
                                            
                                        </div>
                                        <script type="text/javascript">
                                            function setRealReward(rewardTax) {
                                            var reward=document.getElementsByName("reward")[0];
                                            if (reward.value=="") {
                                                $("realReward").innerHTML="0";
                                                return ;
                                            }
                                            if(reward.value.indexOf("0")==0)
                                                reward.value=reward.value.substring(1,reward.value.length-1);
                                            var realReward=parseInt(reward.value)+reward.value*rewardTax;
                                            var dotIndex=realReward.toString().indexOf(".");
                                            if(dotIndex>-1) {
                                                realReward=parseInt(realReward.toString().substring(0,dotIndex));
                                                realReward=realReward+1;
                                            }
                                            $("realReward").innerHTML=realReward;
                                        }
                                        </script>
                                    </div>
                                    <div class="combineformrow-col-2">
                                        <div class="formrow publishform-datetime">
                                            <h3 class="label"><label for="expiresDays">有效时间</label> <span class="form-note">(最多$time$timeUnit, 0或空为允许的最大值)</span></h3>
                                            <div class="clearfix form-enter">
                                                <!--[if $IsEditThread]-->
                                                    <!--[if $question.ExpiresDate == DateTime.MaxValue]-->
                                                无限期
                                                    <!--[else]-->
                                                $outputDateTime($question.ExpiresDate)
                                                    <!--[/if]-->
                                                <!--[else]-->
                                                <input name="expiresDays" id="expiresDays" type="text" class="text number" onkeyup="value=value.replace(/[^\d]/g,'');if(value==0)value='';" value="$time" />
                                                $timeUnit
                                                <!--[/if]-->
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <!--[else if $isShowPolemizeOptions]-->
                                <div class="formrow publishform-debate">
                                    <div class="debate-right">
                                        <div class="debate-right-inner">
                                            <h3 class="label"><label for="debate_right">正方观点</label></h3>
                                            <div class="textarea-wrap">
                                                <textarea cols="40" rows="5" id="agreeViewPoint" name="agreeViewPoint" <!--[if $IsEditThread]-->disabled="disabled"<!--[/if]-->><!--[if $polemize == null]-->$_form.text("agreeViewPoint")<!--[else]-->$polemize.AgreeViewPoint<!--[/if]--></textarea>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="debate-left">
                                        <div class="debate-left-inner">
                                            <h3 class="label"><label for="debate_left">反方观点</label></h3>
                                            <div class="textarea-wrap">
                                                <textarea cols="40" rows="5" id="againstViewPoint" name="againstViewPoint" <!--[if $IsEditThread]-->disabled="disabled"<!--[/if]-->><!--[if $polemize == null]-->$_form.text("againstViewPoint")<!--[else]-->$polemize.AgainstViewPoint<!--[/if]--></textarea>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="formrow publishform-datetime">
                                    <h3 class="label"><label for="expiresDays">有效时间</label> <span class="form-note">(最多$time$timeUnit,0或空为允许的最大值)</span></h3>
                                    <div class="clearfix form-enter">
                                        <!--[if $IsEditThread]-->
                                            <!--[if $polemize.ExpiresDate == DateTime.MaxValue]-->
                                        无限期 
                                            <!--[else]-->
                                        $outputDateTime($polemize.ExpiresDate)
                                            <!--[/if]-->
                                        <!--[else]-->
                                        <input name="expiresDays" id="expiresDays" onkeyup="value=value.replace(/[^\d]/g,'');if(value==0)value='';" value="$time" type="text" class="text number" />
                                        $timeUnit 
                                        <!--[/if]-->
                                    </div>
                                </div>
                                <!--[/if]-->
                            <!--[/ajaxpanel]-->
                                
                                <div id="editorMain" class="formrow publishform-editor">
                                    <!--[if $AllowHTML || $AllowMaxcode]-->
                                        <!--[load src="../_inc/_editor_.aspx" height="400" id="editor_content" value="$Content" useMaxCode="$UseMaxCode" WyswygMode="$WyswygMode" Video="$AllowVideo" Audio="$AllowAudio" Flash="$AllowAudio"  Emoticons="$AllowEmoticon" /]-->
                                    <!--[else]-->
                                    <div class="textarea-wrap">
                                        <textarea name="editor_content" id="editor_content" cols="90" rows="10">$_form.text("editor_content","$Content")</textarea>
                                    </div>
                                    <!--[/if]-->
                                </div>
                                
                                <%--
                                <div class="formrow publishform-tag">
                                    <h3 class="label"><label for="tag">Tag <span class="form-note">(使用空格隔开多个标签)</span></label></h3>
                                    <div class="form-enter">
                                        <input type="text" class="text" name="tag" id="tag" />
                                    </div>
                                    <div class="tag-suggest">
                                        <strong>推荐标签:</strong>
                                        <a href="#">a</a> <a href="#">a</a> <a href="#">a</a> <a href="#">a</a> 
                                    </div>
                                </div>
                                --%>
                                
                                <div id="attachListContainer" class="formrow" style="display:none">
                                    <h3 class="label">附件列表 
                                    <span class="form-note">共有附件 
                                    <span id="attachCount">0</span> 个
                                    <!--[if $MaxAttachmentCountInDay>0]-->, 您当天还可以上传 {=$MaxAttachmentCountInDay - $UsedAttachmentCount} 个附件<!--[/if]-->
                                    <!--[if $CanSellAttachment]-->,$SellAttachmentPointScope (实际收益需扣除<font color="red">$TradeRate</font>的交易税)<!--[if $SellAttachmentDays>0]-->,如果是收费附件$SellAttachmentDaysInfo后将自动变为免费<!--[/if]--><!--[/if]-->
                                    </span> 
                                    </h3>
                                    <div class="attachmentlist">
                                        <div class="attachmentlist-head">
                                            <table class="attachmentlist-table">
                                                <tr>
                                                    <td class="fileicon">&nbsp;</td>
                                                    <td class="filename">文件</td>
                                                    <td class="filesize">大小</td>
                                                    <td class="fileprice">价格($SellAttachmentPoint.Name)</td>
                                                    <td class="fileaction">操作</td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div class="attachmentlist-wrap" style="height:auto;" id="attachInnerList">
                                            <table class="attachmentlist-table" id="attachList">
                                                <tr id="attachTemplate" style="display:none;">
                                                    <td class="fileicon">
                                                        {icon}
                                                        <input type="hidden" name="attachIndex" value="{index}" />
                                                        <input type="hidden" name="attachid_{index}" value="{id}" />
                                                        <input type="hidden" name="attachtype_{index}" value="{type}" />
                                                    </td>
                                                    <td class="filename">
                                                        <input name="filename_{type}_{id}" type="text" class="text" value="{filename}" />.{extname}
                                                        <input type="hidden" value="{extname}" name="extname_{type}_{id}" />
                                                    </td>
                                                    <td class="filesize">
                                                        {filesize}
                                                    </td>
                                                    <td class="fileprice">
                                                        <input name="price_{type}_{id}" type="text" class="text number" value="{price}" <!--[if $CanSellAttachment == false]-->disabled="disabled"<!--[/if]--> />
                                                    </td>
                                                    <td class="fileaction">
                                                        <a style=" display:none;" id="insertMedia_{unique}" href="javascript:void(insertAttachment('{unique}',false));">插入{mediatype}</a>
                                                        <a href="javascript:void(insertAttachment('{unique}',true));">插入下载连接</a>
                                                        <a href="javascript:void(removeAttachment('{unique}'));">删除</a>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                </div>

                             <!--[ajaxpanel id="ap_buttons"]-->
                                <!--[if $IsCreateThread || $IsEditThread]-->
                                    <!--[ValidateCode actionType="CreateTopic"]-->
                                <div class="formrow">
                                    <h3 class="label"><label for="$inputName">验证码</label> <span class="form-note">$tip</span></h3>
                                    <div class="form-enter">
                                        <input type="text" class="text validcode" name="$inputName" id="$inputName" onfocus ="showVCode(this,'$imageurl');" autocomplete="off" $_if($disableIme,'style="ime-mode:disabled;"') />
                                    </div>
                                </div>
                                    <!--[/ValidateCode]-->
                                <!--[else if $IsReply || $IsEditPost]-->
                                    <!--[ValidateCode actionType="ReplyTopic"]-->
                                <div class="formrow">
                                    <h3 class="label"><label for="$inputName">验证码</label> <span class="form-note">$tip</span></h3>
                                    <div class="form-enter">
                                        <input type="text" class="text validcode" name="$inputName" id="$inputName" onfocus ="showVCode(this,'$imageurl');" autocomplete="off" $_if($disableIme,'style="ime-mode:disabled;"') />
                                    </div>
                                </div>
                                    <!--[/ValidateCode]-->
                                <!--[/if]-->
                                
                                <div class="formrow formrow-action">
                                    <span class="minbtn-wrap minbtn-highlight"><span class="btn"><input class="button" type="submit" id="postButton" onclick="return clickPostButton();" value="确认发布" /></span></span>
                                    <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" id="reviewButton" name="reviewButton" onclick="return clickReviewButton();" value="预览" /></span></span>
                                    <!--[if $isReply]-->
                                    <input type="checkbox" name="tolastpage" id="tolastpage" value="1" $_form.checked('tolastpage','1',$forumSetting.ReplyReturnThreadLastPage) /><label for="tolastpage">回帖后跳转到最后一页</label>
                                    <!--[/if]-->
                                    <span id="ajaxsending" class="formrowsubmit-tip-ajax" style="display:none;">正在发布新的主题...</span>
                                    <span class="formrowsubmit-tip">[直接按 Ctrl+Enter 可完成发布]</span>
                                    <!--[if $canautosave]-->&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span id="tipautosave" class="formrowsubmit-tip" style="display:none;">[将在10秒后自动保存帖子临时内容]</span><!--[/if]-->
                                </div>
                            <!--[/ajaxpanel]-->
                            </div>
                        </div>
                    </div>
                    
                    <div class="content-sub" id="topicoptions">
                        <div class="content-sub-inner">
                        <!--[ajaxpanel id="ap_topicoptions"]-->
                            <!--[if $IsShowSellThread]-->
                            <div class="formgroup publishoption-extra">
                                <div class="formrow publishform-topicprice">
                                    <h3 class="label">出售帖子</h3>
                                    <div class="form-enter">
                                        <label for="price">$SellPostPoint.Name</label>
                                        <input type="text" class="text number" name="price" onkeyup="value=value.replace(/[^\d]/g,'');" id="price" <!--[if $IsEditThread]-->  value="$_form.text('price','$thread.Price')"<!--[else]--> value="$_form.text('price')"<!--[/if]--> /> $SellPostPoint.UnitName 
                                    </div>
                                    <div class="form-note">
                                        $SellPostPointScope (实际收益需扣除<font color="red">$TradeRate</font>的交易税)
                                        <!--[if $SellThreadDays>0]-->,如果是收费主题$SellThreadDaysInfo后将自动变为免费<!--[/if]-->
                                    </div>
                                </div>
                            </div>
                            <!--[else if $isShowPollOptions]-->
                            <div class="formgroup publishoption-extra">
                                <div class="formrow publishform-pollresult">
                                    <div class="form-enter">
                                        <!--[if $IsEditThread]-->
                                        <input type="checkbox" disabled="disabled" name="cbNoEyeable" id="cbNoEyeable" <!--[if $poll.AlwaysEyeable==false]--> checked="checked"<!--[/if]--> />
                                        <!--[else]-->
                                        <input type="checkbox" name="cbNoEyeable" id="cbNoEyeable" />
                                        <!--[/if]-->
                                        <label for="cbNoEyeable">投票后结果可见</label>
                                    </div>
                                </div>
                            </div>
                            <!--[else if $isShowQuestionOptions]-->
                            <div class="formgroup publishoption-extra">
                                <div class="formrow publishform-askawardpost">
                                    <div class="form-enter">
                                        <label for="optional_max">奖励回复数</label>
                                        <input type="text" class="text number" name="rewardCount" onkeyup="value=value.replace(/[^\d]/g,'');if(value==0)value='';" <!--[if $IsEditThread]--> value="$question.RewardCount" disabled="disabled" <!--[else]--> value="1" <!--[/if]--> />
                                    </div>
                                </div>
                                <div class="formrow publishform-askresult">
                                    <div class="form-enter">
                                        <input name="notEyeable" id="notEyeable" type="checkbox" <!--[if $IsEditThread && $question.AlwaysEyeable == false]--> checked="checked" disabled="disabled" <!--[/if]--> /> 
                                        <label for="notEyeable">回复后才能查看他人回复</label>
                                    </div>
                                </div>
                            </div>
                            <!--[/if]-->
                        <!--[/ajaxpanel]-->
                            <div class="formgroup publishoption">
                                <div class="formrow">
                                    <div class="clearfix form-enter">
                                        <ul class="optionlist">
                                            <!--[if $AllowHTML]-->
                                            <li>
                                                <input autocomplete="off" id="enableHtml" name="contentFormat" type="radio" onclick="KE.util.setCurrentMode('editor_content','html')" value="enableHtml" $_form.checked("contentFormat","enableHtml",$ContentFormatValue) /> 
                                                <label for="enableHtml">使用HTML代码</label>
                                            </li>
                                            <!--[else]-->
                                            <li>
                                                <input autocomplete="off" id="enableHtml" type="radio" disabled="disabled" />
                                                <label for="enableHtml">使用HTML代码</label>
                                            </li>
                                            <!--[/if]-->
                                            <!--[if $AllowMaxcode]-->
                                            <li>
                                                <input autocomplete="off" id="enableMaxCode" name="contentFormat" type="radio" onclick="KE.util.setCurrentMode('editor_content','ubb')" value="enableMaxCode" $_form.checked("contentFormat","enableMaxCode",$ContentFormatValue) /> 
                                                <label for="enableMaxCode">使用MaxCode标签</label>
                                            </li>
                                            <!--[else]-->
                                            <li>
                                                <input autocomplete="off" id="enableMaxCode" type="radio" disabled="disabled" />
                                                <label for="enableMaxCode">使用MaxCode标签</label>
                                            </li>
                                            <!--[/if]-->
                                        </ul>
                                    </div>
                                </div>
                                <div class="formrow">
                                    <div class="clearfix form-enter">
                                        <ul class="optionlist">
                                            <li>
                                                <!--[if $AllowEmoticon]-->
                                                <input id="enableEmoticons" name="enableItem" type="checkbox" value="enableEmoticons" $_form.checked("enableItem","enableEmoticons",true) />
                                                <label for="enableEmoticons">使用表情</label>
                                                <!--[else]--> 
                                                <input id="enableEmoticons" name="enableItem" type="checkbox" value="enableEmoticons" disabled="disabled" /> 
                                                <label for="enableEmoticons">使用表情</label>
                                                <!--[/if]--> 
                                            </li>
                                            <li>
                                                <!--[if $ShowSignatureInPost]-->
                                                <input id="enableSignature" name="enableItem" type="checkbox" value="enableSignature" $_form.checked("enableItem","enableSignature",true) /> 
                                                <label for="enableSignature">使用个人签名</label>
                                                <!--[else]-->
                                                <input id="enableSignature" name="enableItem" type="checkbox" value="enableSignature" disabled="disabled" />
                                                <label for="enableSignature">使用个人签名</label>
                                                <!--[/if]-->
                                            </li>
                                            <li>
                                                <!--[if $PostEnableReplyNotice]-->
                                                <input id="enableReplyNotice" name="enableItem" type="checkbox" value="enableReplyNotice" $_form.checked("enableItem","enableReplyNotice",true) />
                                                <label for="enableReplyNotice">接收新回复通知</label>
                                                <!--[else]--> 
                                                <input id="enableReplyNotice" name="enableItem" type="checkbox" value="enableReplyNotice" disabled="disabled" />
                                                <label for="enableReplyNotice">接收新回复通知</label>
                                                <!--[/if]-->
                                            </li>
                                            <!--[if $HasEditPermission]-->
                                            <li>
                                                <input id="recodeEditLog" name="recodeEditLog" type="checkbox" value="true" checked="checked" />
                                                <label for="recodeEditLog">保留编辑痕迹</label>
                                            </li>
                                            <!--[/if]-->
                                        </ul>
                                    </div>
                                </div>
                                <!--[if $IsCreateThread]-->
                                    <!--[if $CanLockThread || $CanStickyThread || $CanGlobalStickyThread]-->
                                <div class="formrow">
                                    <div class="clearfix form-enter">
                                        <ul class="optionlist">
                                        <!--[if $CanLockThread]-->
                                            <li>
                                                <input name="cbLockThread" id="cbLockThread" type="checkbox" value="true" $_form.checked("cbLockThread","true") />
                                                <label for="cbLockThread">帖子锁定</label>
                                            </li>
                                        <!--[/if]-->
                                        <!--[if $CanStickyThread]-->
                                            <li>
                                                <input name="cbStickyThread" id="cbStickyThread" type="checkbox" value="true" $_form.checked("cbStickyThread","true") />
                                                <label for="cbStickyThread">帖子固顶</label>
                                            </li>
                                        <!--[/if]-->
                                        <!--[if $CanGlobalStickyThread]-->
                                            <li>
                                                <input name="cbGlobalStickyThread" id="cbGlobalStickyThread" type="checkbox" value="true" $_form.checked("cbGlobalStickyThread","true") />
                                                <label for="cbGlobalStickyThread">帖子总固顶</label>
                                            </li>
                                        <!--[/if]-->
                                        </ul>
                                    </div>
                                </div>
                                    <!--[/if]-->
                                <!--[/if]-->
                            </div>
                        </div>
                    </div>
                    
                    <div class="clear">&nbsp;</div>
                    </form>
                </div>
                <!--#include file="../_inc/_round_bottom.aspx"-->
            </div>
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>
<!--[ajaxpanel id="post_success" ajaxonly="true"]-->
<div class="dialog" id="messagePanel" style="display:none; position:absolute;">
    <div class="dialog-inner">
        <div class="dialogcontent publishtip">
            <div class="clearfix publishtip-info">
                <!--[if $IsPostSuccess]-->
                <h3><span class="info-success">.</span>$PostMessage</h3>
                
                <!--[else if $IsPostAlert]-->
                <h3><span class="info-alert">.</span>$PostMessage</h3>
                <!--[/if]-->
            </div>
            <div class="publishtip-link">
                <ul>
                    <!--[loop $link in $JumpLinks]-->
                    <li><a href="$link.Link">$link.Text</a></li>
                    <!--[/loop]-->
                </ul>
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
    //<!--[if $IsPostSuccess || $IsPostAlert]-->
    if (window.enableAutosave) {
        window.enableAutosave = 0;
        deleteTempdata();
    }
    window.setTimeout("location.replace('$PostReturnUrl')", 3000);
    var p = $('messagePanel');
    setVisible(p, 1);
    moveToCenter(p);
    setStyle(p, { zIndex: 999 });
    var b = new background();
    //<!--[/if]-->
    var btns = $('postButton');
    btns.disabled = false;
    setVisible($('ajaxsending'), 0);
</script>
<!--[/ajaxpanel]-->

<script type="text/javascript">
//begin 附件
    
    var alc = $("attachListContainer");
    var ail = $("attachInnerList");
    var alCounter=$("attachCount");
    var hasEditor = false;
    //<!--[if $AllowHTML || $AllowMaxcode]-->
        hasEditor = true;
    //<!--[/if]-->
    var attachs = {};
    var attachIndex = 0; 
    var attachCount = 0;
    function insertAttachment(id,islink) {
    var f;
    if( typeof(id)=="object"){
        f=id;
    }
    else{
            f = attachs[id];
        }
        var ext = /\.[^\.]+$/.exec(f.filename);
        ext = ext ? ext[0] : ""; 
        if(islink)    ext="";
        insertAttach(f.id, f.filename, ext, f.type);
    }
 
    function addAttachment(attach) {
        var unique = attach.id +"_"+ attach.type;
        if(attachs[unique]) return ;
        var lt = $("attachList").tBodies[0];
        var tc = $("attachTemplate").cells;
        var rowCount = lt.rows.length;
        var r = lt.insertRow(rowCount);
        var mt = "";
        
         var regF = /^(.+)\.([^.]+)$/;
            
            
            var fileName;
            var extName;
            var fi=regF.exec(attach.filename);
            if(fi)
            {
                fileName=fi[1];
                extName=fi[2];
            }
            else
            {
                fileName=attach.filename;
                extName="";
            }
            
            if(isImage(extName))
              mt="图片";
            else if(isAudio(extName))
              mt="音频";
            else if(isFlash(extName))
              mt="Flash";
            else if(isVideo(extName))
              mt="视频";

        attachs[unique] = attach;

        for (var i = 0; i < tc.length; i++) {
            var c = r.insertCell(i);
            c.className=tc[i].className;
            var html = tc[i].innerHTML;

           if(max.browser.isIE){
                html = html.replace(/\{filename\}/g, '"'+fileName+'"');
            }
            else{
                html = html.replace(/\{filename\}/g, fileName);
            }   
            html = html.replace(/\{extname\}/g, extName);
            html = html.replace(/\{filesize\}/g, getFileSize(attach.filesize));
            html = html.replace(/\{icon\}/g, "<img src=" + attach.icon + " />");
            html = html.replace(/\{id\}/g, attach.id);
            html = html.replace(/\{type\}/g,attach.type);
            html = html.replace(/\{index\}/g, attachIndex);
            html = html.replace(/\{price\}/g, attach.price?attach.price:0);
            html = html.replace(/\{unique\}/g, unique);
            html = html.replace(/\{mediatype\}/g, mt);


            c.innerHTML = html;
        }  

        if(attachCount>=5) ail.style.height="200px";
        if(mt)  $("insertMedia_"+ unique).style.display="";
        r.id = "att_" + unique;
        alc.style.display = '';
        attachIndex++;
        attachCount++;
        alCounter.innerHTML=attachCount.toString();
    }

    function removeAttachment(id) {
        var file = attachs[id];
        if(!file)  return ;
        
        function a(){
            var lt = $("attachList");
            if (lt.tBodies[0].rows.length == 2)
                alc.style.display = 'none';
            removeElement("att_" + id);
            attachs[id] = null;
            attachCount--;
            alCounter.innerHTML=attachCount.toString();
            if(attachCount<5) ail.style.height="auto";
        }
        if(file.id  <0){
            delAttach(file.id,function(r,m){if(r)a();else alert(m);});
        }
        else{
            a();
        }
    }
    
function addDiskFile(r)
{

        if(!checkFile(r))
            return false;
        addAttachment(r);
    
    return true;
}

function checkAttachCount(addCount)
{
    if($MaxPostAttachmentCount>0 && attachCount + addCount > $MaxPostAttachmentCount)
    {
        //<!--[if $IsCreateThread || $IsEditThread]-->
        showAlert( '附件个数已经超过单个主题允许的最大附件个数: $MaxPostAttachmentCount');
        //<!--[else]-->
        showAlert('附件个数已经超过单个回复允许的最大附件个数: $MaxPostAttachmentCount');
        //<!--[/if]-->
        return false;
    }
    
    if($MaxAttachmentCountInDay>0 && attachCount + 1 > $MaxAttachmentCountInDay - $UsedAttachmentCount)
    {
        showAlert('附件个数已经超过您今天允许上传的最大附件个数: $MaxAttachmentCountInDay');
        return false;
    }
    return true;
}


function checkFile(file)
{
    var success = checkAttachCount(1);
    if(success == false)
        return;
    return checkHistoryAttach(file);
}

function addHistoryAttach(r)
{
        if(!checkHistoryAttach(r))
            return;
        insertAttachment(r);
}

function checkHistoryAttach(file)
{
    if(file.filesize > $SingleFileSize && $SingleFileSize!=0)
    {
        showAlert('附件大小超过当前版块允许的单个附件最大大小: $SingleFileSizeString');
        return false;
    }
    
    var t = file.filename.split('.');
    var fileExtName = '';
    if(t.length>1)
        fileExtName = t[t.length-1];
    
    var allowType = '$AllowFileType';
    
    if(allowType != '*.*')
    {
        if(fileExtName == '')
        {
            showAlert('不允许上传没有扩展名的文件');
            return false;
        }
        if(allowType.indexOf('*.' + fileExtName) == -1)
        {
            showAlert('不允许上传“' + fileExtName + '”类型的文件');
            return false;
        }
    }
    
    return true;
}

//<!--[loop $theAttachment in $AttachList]-->
addAttachment({id:'$theAttachment.AttachmentID', filename:'$JsString($theAttachment.FileName)', filesize:$theAttachment.FileSize, price:$theAttachment.Price, icon:'$theAttachment.FileIcon',type:{=(int)$theAttachment.AttachType}});
//<!--[/loop]-->
//end 附件


//Begin 编辑器初始化参数设置

KE.g['editor_content'].action = "post";
KE.g['editor_content'].targetID = $PostID;
//end 编辑器初始化参数设置
 
    var forumID = $forum.forumid;
    var searchInfo = "$searchInfo";
    
    
    
//<!--[if $EnablePostIcon]-->
new popup('posticon-list','posticon-trigger',false);


initIconSelected ();
// 编辑状态，选择帖子图标
function initIconSelected () {
    var icons = $('posticon-list').getElementsByTagName('img');
    var value = document.getElementsByName('postIcon')[0].value;
    for (var i = 0, len = icons.length; i < len; i++) {
        if (icons[i].alt == value) {
            $('post_icon').src = icons[i].src;
            return;
        }
    }
}

function iconSelected (th) {
    $('post_icon').src = th.src;
    document.getElementsByName('postIcon')[0].value = th.alt;
    $('posticon-list').style.display='none';
    return false;
}
//<!--[/if]-->

function postCheck() {

    var form = $('formpost');
    //<!--[if $IsCreateThread || $IsEditThread]-->
    if (form.subject.value == '') {
        showAlert('标题不能为空');
        return false;
    }
    //<!--[/if]-->
    
    KE.util.setData("editor_content");
    if (form.editor_content.value == '') {
        showAlert('内容不能为空');
        return false;
    }
    return true;
}

onCtrlEnter($("formpost"),clickPostButton);
//var isSending = 0;
function clickPostButton(){

    if(postCheck() == false)
        return false;

   var btns = $('postButton');
    if(btns.disabled == true)
        return;

   btns.disabled = true;

   var j =$('ajaxsending');
    setVisible(j,1);

    ajaxSubmit('formpost', 'postButton', 'ap_topicdatas,ap_buttons,ap_error,post_success', null, null, true);
    return false;
}

function clickReviewButton(){
    if(postCheck() == false)
    { 
        setButtonDisable("reviewButton",false);
        return false;
    }
        
    ajaxSubmit('formpost', 'reviewButton', 'ap_review', function(result){
        setButtonDisable("reviewButton",false);
        if(result!=null && result.iswarning){
            showAlert(result.message);
        }
        else{
            location.href='#reviewPanel';
        }
    }, null, true);
    
    return false;
}
function cancellReviewButton(url){
    ajaxRender(url,'ap_review');
    return false;
}

//-----------------以下是数据自动保存、恢复部分---------------------------
window.enableAutosave = $_if(CanAutoSave,"1","0");

//<!--[if $CanAutoSave]-->
var tempContentHash="";
function beginAutoSave(data){
var secAutosave = 10;
var autosaveurl="$url(handler/tempdata)?type=$TempDataType&action=save";
var getContentHash=function (cnt)
{
     var l = cnt.length;
     var sample = cnt.substr(0,5);
     sample += cnt.substr(l/2-3,5);
     sample += cnt.substr(l-5);
     return l +"_"+ sample.getHashCode();
}

if(data)
{
    tempContentHash=getContentHash(data);
}

var autosaveTimer = new timer(1000,function(){
if(!window.enableAutosave)
    return;



 var tipobj=$("tipautosave");

var currentContentFormat = hasEditor? KE.util.getCurrentMode("editor_content"): "text";
var tempdata= KE.util.getData("editor_content");
$("max_autosave_Data").value = tempdata;

 if(tempdata.length<30)  //30个字符以上启用自动保存功能
 {
    setVisible(tipobj,0);
    tipobj.innerHTML ="";
    return;
 }
 setVisible(tipobj,1);
if(secAutosave>0)
{
    
    tipobj.innerHTML=String.format( "[将在{0}秒后自动保存临时帖子内容]",secAutosave);
    secAutosave --;
    return;
}
else
{ 
    tipobj.innerHTML=String.format( "[正在提交临时内容到服务器]",secAutosave);
   secAutosave =10;
}

 var contentHash =getContentHash(tempdata);
 if( tempContentHash == contentHash ) //检查和最后一次保存的内容是否一致
    return; 

tempContentHash = contentHash;

 $("max_autosave_format").value=currentContentFormat;
 var postContent = getFormData($("autosaveForm"));
 ajaxPostData(autosaveurl,postContent,function(r){
 var isLost=false;
 if(r.indexOf("404 ")==0) isLost = true;
 if(r.indexOf("400 ")==0) isLost = true;
 if(r.indexOf("500 ")==0) isLost = true;
 if(r.indexOf("403 ")==0) isLost = true;

     if(isLost)
     {
        writeCookie("posttempdata",currentContentFormat +"|"+tempdata,240);
     }
     else{
        deleteCookie("posttempdata");
     }
 });
 });

 autosaveTimer.start();
 }
 //<!--[/if]-->

var deleteTempdata=function(){
    var dataUrl = "$url(handler/tempdata)?type=$TempDataType&action=delete";
    ajaxRequest(dataUrl);
    deleteCookie("posttempdata");
};

var loadTempData = false;
var tempdata =readCookie("posttempdata");
var hasServerTempData = $_if(TempPostData!=null,"1","0");

if(tempdata)
{
    loadTempData = confirm("您本地有未发布的帖子内容， 是否恢复？");
}
else if(hasServerTempData)
{
    loadTempData = confirm("您有未发布的帖子内容， 是否恢复？");    
}
window.setTimeout(function(){
if(loadTempData)
{
    var setTempData  = function(data){
        var fi = data.indexOf("|");
        
           var f = "";
           if(fi && fi<10){
                f = data.substr(0,fi);
                data = data.substr(fi+1);
          }
          if(hasEditor){
            KE.util.setValue("editor_content",data,f);
          }else{
             $("editor_content").value=data;
          }

          deleteCookie("posttempdata");
          //<!--[if $CanAutoSave]-->
          beginAutoSave(data);
          //<!--[/if]-->
    }

    if(tempdata)
    {
        setTempData(tempdata);
    }
    else if(hasServerTempData)
    {
    var bg = new background();
    var div= addElement("div");
    div.innerHTML = "<h3>正在恢复数据，请稍候......</h3>";
    setStyle(div,{position:"absolute",zIndex:51,border:"solid 1px #cccccc",padding:"5px;", backgroundColor:"white"});
    moveToCenter(div);
        var dataUrl = "$url(handler/tempdata)?type=$TempDataType&action=get";
        ajaxRequest(dataUrl,function(r){ 
            setTempData(r);
             removeElement(div);
            bg.destroy();
        });
    }
}
else
{
    if(hasServerTempData || tempdata)
    {
        deleteTempdata();
    }
    //<!--[if $CanAutoSave]-->
    beginAutoSave();
    //<!--[/if]-->
}
},500);
</script>
<form id="autosaveForm" style="display:none;">
<input type="hidden" name="data" value="" id="max_autosave_Data" />
<input type="hidden" name="format" value=""id="max_autosave_format"/>
</form>
</body>
</html>