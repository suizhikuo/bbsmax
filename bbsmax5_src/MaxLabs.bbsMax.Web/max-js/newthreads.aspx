
<!--/*最新主题|最新主题*/-->
<!--[page inherits="MaxLabs.bbsMax.Web.JsPageBase" /]-->

<!--[NewThreadList  count="10"]-->
<!--[if $threadList.Count == 0]-->
    <li>暂时没有最新帖子</li>
<!--[else]-->
    <!--[loop $thread in $threadList]-->
        <li>
<a href="$FullAppRoot$url($thread.Forum.CodeName/$thread.ThreadTypeString-$thread.ThreadID-1)" target="_blank">$GetThreadSubject($thread,10)</a>
</li>
    <!--[/loop]-->
<!--[/if]-->


<!--[/NewThreadList]-->
