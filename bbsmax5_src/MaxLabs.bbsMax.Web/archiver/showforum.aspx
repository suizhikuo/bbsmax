<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="zh-cn" xml:lang="zh-cn">
<head>
<title>$PageTitle - 简约版 v5</title>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="keywords" content="$MetaKeywords" />
<meta name="description" content="$MetaDescription" />
<meta name="copyright" content="bbsmax.com" />
<meta name="generator" content="$Version" />
<link rel="shortcut icon" type="image/x-ico" href="$root/max-assets/images/favicon.ico" />
<link rel="stylesheet" type="text/css" href="$root/max-assets/style/simple.css" />
<script type="text/javascript">window.onerror=function(){return true}</script>
</head>
<body id="bbsmax" class="printable">
    <div class="header">
        <h1><a href="$url(archiver/default)">$BbsName</a></h1>
    </div>

<!--[if $errorMessage != ""]-->
    <div class="box center">$errorMessage</div>
<!--[else]-->

    <div class="box printtools">
        <ul>
        <li><a class="viewall" href="$url($CodeName/list-1)">查看完整版</a></li>
        </ul>
    </div>

    <div class="box">
    <div class="box-main">
        <p>$Navigation</p>
        <!--[loop $subforum in $forum.SubForumsForList]-->
        <p><a href="$url(archiver/$subforum.codeName/list-1)">$subforum.ForumName</a> <span class="status">(今日$subforum.TodayPosts)</span></p>
        <!--[/loop]-->
        <!--[if $threadList.Count == 0]-->
        <p>本版块还未发布任何主题.</p>
        <!--[else]-->
        <p class="paginationlite">页码: <!--[pager name="ThreadListPager" skin="_pager_.aspx"]--></p>
        <ol class="entry-list">
        <!--[loop $thread in $threadList]-->
            <li>
            $GetArchiverThreadLink($thread,@"<a class=""heading"" href=""{0}"">{1}</a>")
            - <span class="author">
            <!--[if $thread.PostUserID!=0]-->
            $thread.PostUsername
            <!--[else]-->
                <!--[if $thread.PostUsername!=""]-->
                游客:$thread.PostUsername
                <!--[else]-->
                游客:匿名
                <!--[/if]-->
            <!--[/if]-->
            </span>
            <span class="date">发表于 $outputFriendlyDateTime($thread.CreateDate)</span>
            <span class="status">({=$thread.TotalReplies}回复 / {=$thread.TotalViews}点击)</span>
            </li>
        <!--[/loop]-->
        </ol>
        <p class="paginationlite">页码: <!--[pager name="ThreadListPager" skin="_pager_.aspx"]--></p>
        <!--[/if]-->
    </div>
    </div>
<!--[/if]-->

    <div class="footer">
        Powered by $Version &copy; 2002-2010 maxLab Inc.
        <p>Processed in $ProcessTime seconds, $QueryTimes Queries. GMT $_if($My.TimeZone>0,'+')$My.TimeZone, $usernow </p>
    </div>
</body>
</html>
