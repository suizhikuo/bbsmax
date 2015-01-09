<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="archives" title="$bbsName" href="$FullAppRoot/archiver/" />
<link rel="stylesheet" type="text/css" href="$skin/styles/forum.css" />
<!--[if $ShowLoginDialog]-->
<script type="text/javascript">
    addPageEndEvent(
    function() { window.setTimeout(function() { openDialog('$dialog/login.aspx?loginreferrer={=LoginReferrer.ViewAttachImage}', refresh); }, 1000); }
    );
</script>
<!--[/if]-->