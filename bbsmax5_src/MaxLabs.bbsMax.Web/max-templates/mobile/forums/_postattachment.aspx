<!--[if $post.AttachmentsForView.Count != 0 && $isShowAttachments($post)]-->
    <!--[if $ViewAttachment == false]-->
<div class="postattachment-tip">
    <!--[if IsLogin]-->
    您没有权限查看附件.
    <!--[else]-->
    您需要 <a href="$url(login)">登录</a> 才可以查看或下载附件.
    <!--[/if]-->
</div>
    <!--[else]-->
<div class="postattachment">
    <h3 class="title"><span>帖子附件</span></h3>
    <ul class="list">
        <!--[loop $attachment in $post.AttachmentsForView]-->
        <li>
            <!--[if $attachment.Price != 0 && $MyUserID != $attachment.UserID && $attachment.IsBuyed($My) == false]-->
            <a href="$Dialog/attachment-buy.aspx?attachmentid=$attachment.AttachmentID">
            <!--[else if $IsOverSellAttachmentDays($attachment) == false]-->
            <a href="#" onclick="alert('该附件已经过了出售日期. 如需下载请联系作者.'); return false;">
            <!--[else if $AlwaysViewContents == false]-->
            <a href="#" onclick="alert('你无法下载该附件.'); return false;">
            <!--[else]-->
            <a href="$url(handler/down)?action=attach&id=$attachment.AttachmentID">
            <!--[/if]-->
                <span class="filename">$attachment.FileName</span>
                <span class="status">(大小: $OutputFileSize($attachment.FileSize), 下载: $attachment.totalDownLoads)</span>
                <!--[if $attachment.Price!=0]-->
                <span class="price">价格: $GetAttachmentPoint($post.UserID).Name $attachment.Price $GetAttachmentPoint($post.UserID).UnitName <!--[if $IsOverSellAttachmentDays($attachment)]-->(现已免费)<!--[else]-->(点击立即购买)<!--[/if]--></span>
                <!--[/if]-->
            </a>
        </li>
        <!--[/loop]-->
    </ul>
</div>
    <!--[/if]-->
<!--[/if]-->
