<!--/*最新回复|最新回复*/-->
<!--[page inherits="MaxLabs.bbsMax.Web.JsPageBase" /]-->

<!--[NewRepliedThreadList  count="10"]-->
<!--[if $threadList.Count == 0]-->
<li>暂时没有帖子</li>
<!--[else]-->
<!--[loop $thread in $threadList]-->
<li>
	<a href="$FullAppRoot$url($thread.Forum.CodeName/$thread.ThreadTypeString-$thread.ThreadID-1)" target="_blank">$GetThreadSubject($thread,10)</a>
</li>
<!--[/loop]-->
<!--[/if]-->


<!--[/NewRepliedThreadList]-->
