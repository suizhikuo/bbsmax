<!--[if $post.AttachmentsForView.Count!=0 && $isShowAttachments($post)]-->
    <!--[if $ViewAttachment == false]-->
    <div class="post-attachment-tip">
        <!--[if IsLogin]-->
        您没有权限查看附件.
        <!--[else]-->
        您需要 <a target="_blank" href="/max-dialogs/login.aspx" onclick="return openDialog(this.href, refresh)">登录</a> 才可以查看或下载附件. 没有帐号, <a href="$url(register)">点击此处注册</a>.
        <!--[/if]-->
    </div>
    <!--[else]-->
    <div class="post-attachment">
        <h3 class="post-attachment-title"><span>帖子附件</span></h3>
        <ul class="post-attachment-list">
            <!--[loop $attachment in $post.AttachmentsForView]--> 
            <li class="clearfix">
                <div class="attachment-icon">
                    <img src="$attachment.FileIcon" alt="" />
                </div>
                <div class="attachment-name">
                    <!--[if $attachment.Price!=0 && $MyUserID!=$attachment.UserID && $attachment.IsBuyed($My) == false && $AlwaysViewContents==false && $IsOverSellAttachmentDays($attachment) == false]-->
                    $attachment.FileName
                    <!--[else]-->
                    <a href="$url(handler/down)?action=attach&id=$attachment.AttachmentID">$attachment.FileName</a> 
                    <!--[/if]-->
                    <span class="file-status">(大小:$OutputFileSize($attachment.FileSize),下载:$attachment.totalDownLoads)</span>
                </div>
                <!--[if $attachment.Price!=0]--> 
                <div class="attachment-price">
                    <span class="label">价格:</span>
                    <span class="value">$GetAttachmentPoint($post.UserID).Name:$attachment.Price $GetAttachmentPoint($post.UserID).UnitName</span>
                    <!--[if $MyUserID!=$attachment.UserID && $attachment.IsBuyed($My) == false]-->
                    <a href="$Dialog/attachment-buy.aspx?attachmentid=$attachment.AttachmentID" onclick="return openDialog(this.href, refresh)">[ 购买 ]</a>
                    <span style="color:Red">您还有$GetAttachmentPoint($post.UserID).Name:$My.GetPoint($GetAttachmentPoint($post.UserID).Type) $GetAttachmentPoint($post.UserID).UnitName</span>
                    <!--[if $IsShowChargePointLink($GetAttachmentPoint($post.UserID))]--><a href="$url(my/pay)" target="_blank">[ 充值 ]</a><!--[/if]-->
                        <!--[if $AlwaysViewContents]-->
                        <a href="$dialog/attachmentexchanges.aspx?attachmentid=$attachment.AttachmentID" onclick="return openDialog(this.href, null)">[ 购买记录 ]</a>
                        <!--[/if]-->
                    <!--[else]-->
                    <a href="$dialog/attachmentexchanges.aspx?attachmentid=$attachment.AttachmentID" onclick="return openDialog(this.href, null)">[ 购买记录 ]</a>
                    <!--[/if]-->
                    <!--[if $IsOverSellAttachmentDays($attachment)]-->
                    <span>(现已免费)</span>
                    <!--[/if]-->
                </div>
                <!--[/if]-->
                <!--[if $isShowAttachmentImage($attachment)]-->
                <div class="attachment-thumb">
                    <img src="$url(handler/down)?action=attach&mode=image&id=$attachment.attachmentID" onload="ImageLoaded(this)" alt="" />
                </div>
                <!--[/if]-->
            </li>
            <!--[/loop]-->
        </ul>
    </div>
    <!--[/if]-->
<!--[/if]-->
