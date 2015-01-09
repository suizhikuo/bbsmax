<!--[page inherits="MaxLabs.bbsMax.Web.max_pages._master_dialog_" /]-->
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>$DialogTitle</title>
<link rel="stylesheet" type="text/css" href="~/style/common.css" />
<!--#include file="_htmlhead_.aspx"-->
<script type="text/javascript">
    var dialog = new Object();
    dialog.close = function() { window.history.go(-1); };
    dialog.resize = function() { };
</script>
</head>
<body>
<div class="container dialogspage">
    <!--[include src="../max-templates/default/_top_.ascx" /]-->
    <!--[include src="../max-templates/default/_head_.ascx" /]-->
    <div class="wrap">
        <h1 class="dialogspage-title">
        <!--[if $hastitle]-->
        $dialogTitle
        <!--[else]-->
        页面提示信息
        <!--[/if]-->
        </h1>
        <div class="dialogspage-content">
        <!--[MasterPagePlace id="body" /]-->
        </div>
    </div>
    <!--[include src="../max-templates/default/_foot_.ascx" /]-->
</div>
<script type="text/javascript">
page_end();
</script>
</body>
</html>