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
<script type="text/javascript">window.onerror=function(){return true}</script>
<title>$PageTitle - 简约版 v5</title>
</head>
<body id="bbsmax" class="printable">
    <div class="header">
        <h1>$BbsName</h1>
    </div>

    <div class="box printtools">
        <ul class="multicolumns2">
        <li class="left"><a class="viewall" href="$url(default)">查看完整版</a></li>
        <li class="right"><a class="print" href="javascript:void(0);" onclick="window.print(); return false">打印本页</a></li>
        </ul>
    </div>

    <div class="box">
    <div class="box-main">
        <div class="entry-list">
        <!--[loop $catalog in $Catalogs]-->
            <dl>
                <dt>$catalog.ForumName</dt>
                <!--[loop $forum in $catalog.SubForumsForList]-->
                <dd><a href="$url(archiver/$forum.codeName/list-1)">$forum.ForumName</a> <span class="status">(今日$forum.TodayPosts)</span></dd>
                <!--[/loop]-->
            </dl>
        <!--[/loop]-->
        </div>
    </div>
    </div>

    <div class="footer">
        Powered by $Version &copy; 2001-2010 maxLab Inc.
        <p>Processed in $ProcessTime seconds, $QueryTimes Queries. GMT $_if($My.TimeZone>0,'+')$My.TimeZone, $usernow </p>
    </div>

</body>
</html>
