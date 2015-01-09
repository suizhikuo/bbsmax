<div class="postwrap <!--[if $loopindex == 0]-->postwrap-first<!--[/if]-->" id="post_$post.PostID">
    <table class="posttable" <!--[if $post.PostID == $LastPostID]-->id="last"<!--[/if]-->>
        <tr>
            <td class="postauthor" rowspan="2">
                <!-- #include file="_postauthorinfo.aspx" -->
            </td>
            <td class="postmain">
                <div class="postmain-inner">
                    <div class="clearfix post-meta">
                        <p class="post-info">
                            发表于 $outputFriendlyDateTime($post.CreateDate)
                            <!--[if $LookUserID > 0]-->
                            - <a href="$url($codename/$thread.ThreadTypeString-$threadid-1)" class="post-filter">查看全部</a>
                            <!--[else if $post.UserID != 0]-->
                            - <a href="$url($codename/thread-$threadid-1)?userid=$post.UserID" class="post-filter">只看该用户</a>
                            <!--[/if]-->
                        </p>
                        <!--[if $IsShowPostIndexAlias]-->
                        <p class="post-number">$post.PostIndexAlias</p>
                        <!--[/if]-->
                        <!--[if $OutputIpPartCount > 0]-->
                        <p class="post-authorip">
                            <!--[if $CanShowIpArea]-->
                            <a onclick="return openDialog(this.href);" href="$dialog/ip.aspx?ip=$OutputIP($post.IpAddress)"><span><em>.</em>IP <code>$OutputIP($post.IpAddress)</code></span></a>
                            <!--[else]-->
                            <a href="javascript:void(0);"><span><em>.</em>IP <code>$OutputIP($post.IpAddress)</code></span></a>
                            <!--[/if]-->
                        </p>
                        <!--[/if]-->
                    </div>
                    
                    <!--[if $IsShowContent($post) && $CanSeeContent($post)]-->
                    <!--[if $post.IconID > 0 || $post.SubjectText != ""]-->
                    <div class="clearfix post-head">
                        <div class="post-heading">
                            <h1>
                                $GetIcon($post.IconID)
                                $Highlight($post.SubjectText)
                            </h1>
                        </div>
                    </div>
                    <!--[/if]-->
                    <!--[/if]-->
                    
                    <!--[if $HasInPostTopAD($loopindex,$IsLastPost($post))]-->
                    <div class="ad-text-post">
                        $InPostTopAD($loopindex,$IsLastPost($post))
                    </div>
                    <!--[/if]-->

                    <table class="postcontentwrap">
                        <tr>
                            <td class="postcontentwrap-main">
                    
                    <!--[if $Thread.ThreadType == ThreadType.Polemize]-->
                    <div class="clearfix dabatesign <!--[if $post.PostType == PostType.Polemize_Agree]--> dabatesign-right<!--[else if $post.PostType == PostType.Polemize_Against]--> dabatesign-left<!--[else if $post.PostType == PostType.Polemize_Neutral]--> dabatesign-neutral<!--[/if]-->">
                        <!--[if $post.PostType == PostType.Polemize_Agree]-->正方<!--[else if $post.PostType == PostType.Polemize_Against]-->反方<!--[else if $post.PostType == PostType.Polemize_Neutral]-->中立<!--[/if]-->
                    </div>
                    <!--[/if]-->
                    
                    <!-- #include file="_postalert.aspx" -->
                    
                    <!--[if $IsShowContent($post) && $CanSeeContent($post)]-->
                    <div class="clearfix post-content">
                        $Highlight($post.ContentText)
                    </div>
                    
                    <!--[if $post.LastEditorID > 0]-->
                    <div class="post-lastmodify">
                        本帖最后由 $post.LastEditor 于 $outputFriendlyDatetime($post.UpdateDate) 编辑
                    </div>
                    <!--[/if]-->
                    
                    <!--[if $IsShowPostGetReward($post.PostID)]-->
                    <div class="askpost-reward">
                        <p>
                            该帖得分: <strong class="value">$GetReward($post.PostID)</strong> $QuestionPoint.UnitName
                        </p>
                    </div>
                    <!--[/if]-->
                    
                    <!-- #include file="_postattachment.aspx" -->
                    <!-- #include file="_postscore.aspx" -->
                    
                    <!--[/if]-->
                        </td>
                        <!--[if $HasInPostRightAD($loopindex,$IsLastPost($post))]-->
                        <td class="postcontentwrap-ad">
                    <div class="ad-verticalbanner-post">
                        $InPostRightAD($loopindex,$IsLastPost($post))
                    </div>
                        </td>
                        <!--[/if]-->
                        </tr>
                    </table>
                    
                </div>
            </td>
        </tr>
        <tr>
            <td class="postmain-signature">
                <div class="clearfix postmain-signature-inner">
                    <!--[if $isShowSignature($post)]-->
                    <div class="post-signature">
                        $post.User.SignatureFormatted
                    </div>
                    <!--[/if]-->
                    <!--[if $HasInPostBottomAD($loopindex,$IsLastPost($post))]-->
                    <div class="ad-text-postbottom">
                        $InPostBottomAD($loopindex,$IsLastPost($post))
                    </div>
                    <!--[/if]-->
                    <!-- #include file="_postmanage.aspx" -->
                </div>
            </td>
        </tr>
        <tr>
            <td class="postauthor-tools">
                <!--[if $my.ismanager]-->
                <div class="clearfix postauthor-tools-inner">
                    <div class="post-tools">
                        <ul class="post-tools-list">
                            <li><a class="action-usermanage" href="javascript:void(0);" onclick="return openUserMenu(this,$post.UserID)"><span><em>.</em>管理用户</span></a></li>
                        </ul>
                    </div>
                </div>
                <!--[/if]-->
            </td>
            <td class="postmain-tools">
                <div class="clearfix postmain-tools-inner">
                    <!-- #include file="_postaction.aspx" -->
                </div>
            </td>
        </tr>
    </table>
</div>
