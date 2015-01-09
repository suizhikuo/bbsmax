<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="zh-cn" xml:lang="zh-cn">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="keywords" content="$MetaKeywords" />
<meta name="description" content="$MetaDescription" />
<meta name="copyright" content="bbsmax.com" />
<meta name="generator" content="$Version" />
<link rel="shortcut icon" type="image/x-ico" href="$root/max-assets/images/favicon.ico" />
<link rel="stylesheet" type="text/css" href="$root/max-assets/style/simple.css" />
<!--[if $IncludeBase64Js]-->
<script type="text/javascript">
    var root = '$Root';
</script>
<script type="text/javascript" src="$root/max-assets/javascript/base64.js"></script>
<!--[/if]-->
<script type="text/javascript">window.onerror=function(){return true}</script>
<title>$PageTitle - 简约版</title>
</head>
<body id="bbsmax" class="printable">
    <div class="header">
        <h1><a href="$url(archiver/default)">$BbsName</a></h1>
    </div>

<!--[if $errorMessage != "" && $errorMessage != null]-->
    <div class="box center">$errorMessage</div>
<!--[else]-->

    <div class="box printtools">
        <ul>
        <li><a class="viewall" href="$url($CodeName/thread-$Thread.ThreadID-1-1)">查看完整版</a></li>
        </ul>
    </div>

    <div class="box">
        <div class="box-main">
            <p>$Navigation</p>
            <p class="paginationlite">页码: <!--[pager name="PostListPager" skin="_pager_.aspx"]--></p>
        </div>
    </div>

    <!--[loop $reply in $replyList with $index]-->
    <!--[if $reply.PostIndex == 0 || $IsShowReplies]-->
    <div class="box entry">
        <div class="box-head">
            <h3>$reply.SubjectText </h3>
            <p> - 
            <span class="author">
            <!--[if $reply.UserID == 0]-->
            游客:<!--[if $reply.Username!=""]-->$reply.Username<!--[else]-->匿名<!--[/if]-->
            <!--[else]-->
            $reply.Username
            <!--[/if]-->
            </span> <span class="date">$outputFriendlyDateTime($reply.CreateDate)</span></p>
        </div>
        <div class="box-main entry-content">
        
                    <!--[if $thread.IsValued && $reply.PostIndex == 0 && $ViewValuedThread == false]-->
                    <p class="center red"><!--[if $myuserID == 0]-->(您是游客)<!--[/if]-->您没有查看精华帖子的权限</p>
                    <!--[else if $reply.PostIndex == 0 && $ViewThread == false]-->
                    <p class="center red"><!--[if $myuserID == 0]-->(您是游客)<!--[/if]-->您没有查看主题内容的权限</p>
                    <!--[else if $reply.PostIndex != 0 && $ViewReply == false]-->
                    <p class="center red"><!--[if $myuserID == 0]-->(您是游客)<!--[/if]-->您没有查看回复内容的权限</p>
                    <!--[else if $forum.IsShieldedUser($reply.UserID)]-->
                    <p class="center red">该用户发言被屏蔽<!--[if $IsShowContent($reply)]-->,但您仍有查看权限<!--[/if]--></p>
                    <!--[else if $reply.IsShielded == true]-->
                    <p class="center red">本帖有违规内容被屏蔽<!--[if $IsShowContent($reply)]-->,但您仍有查看权限<!--[/if]--></p>
                    <!--[else if $reply.PostIndex == 0 && $Thread.Price > 0]-->
                    <p class="center red">本帖需要购买才能查看
                        <!--[if $myuserID == 0]-->
                            ,您还未购买(请先登录)
                        <!--[else if $AlwaysViewContents || $Thread.PostUserID == $MyUserID]-->
                            ,但是您有权限查看
                        <!--[else if $Thread.IsBuyed($My)]-->
                            ,您已经购买,可以查看
                        <!--[else]-->
                            ,您还未购买
                        <!--[/if]-->
                    </p>
                    <!--[/if]-->

                    <!--[if $IsShowContent($reply) && $CanSeeContent($reply)]-->
                    $reply.ContentText
                    <!--[/if]-->
        

        </div>
    </div>
    <!--[else if $IsShowNoPermissionViewReply($index)]-->
        <!--[if $ViewReply]-->
        <div class="box center red">
            该帖为悬赏帖并且必须回复后才能查看他人回复
        </div>
        <!--[else]-->
        <div class="box center red">
            <!--[if $myuserID == 0]-->(您是游客)<!--[/if]-->您没有权限查看回复
        </div>
        <!--[/if]-->
    <!--[/if]-->
    <!--[/loop]-->

    <div class="box">
        <div class="box-main">
            <p class="paginationlite">页码: <!--[pager name="PostListPager" skin="_pager_.aspx"]--></p>
        </div>
    </div>
<!--[/if]-->

    <div class="footer">
        Powered by <a href="http://www.bbsmax.com/" target="_blank">$Version</a> &copy; 2002-2010 maxLab Inc.
        <p>Processed in $ProcessTime seconds, $QueryTimes Queries. GMT $_if($My.TimeZone>0,'+')$My.TimeZone, $usernow </p>
    </div>

</body>
</html>
