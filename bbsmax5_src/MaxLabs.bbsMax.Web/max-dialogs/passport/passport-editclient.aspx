<!--[DialogMaster title="创建Passport客户端" width="580"]-->
<!--[place id="body"]-->
<!--[include src="../_error_.ascx" /]-->
<head>
    <style type="text/css">
        .style1
        {
            color: #FF3300;
        }
    </style>
</head>
<form id="form1" method="post" action="$_form.action">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="clientname">名称</label></h3>
            <div class="form-enter">
                <input id="clientname" type="text" maxlength="20" value="$_form.text('clientname',$client.name)" name="clientname" $_if($isEdit,'disabled="disabled"','') class="text" />
                <!--[error name="clientname"]--><span class="form-tip tip-error">$message</span><!--[/error]-->
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="url">网站URL</label></h3>
            <div class="form-enter">
                <input id="url" type="text"  style="width:250px" maxlength="500" value="$_form.text('url',$client.url)" name="url" $_if($isEdit,'disabled="disabled"','') class="text" />
                <!--[error name="url"]--><span class="form-tip tip-error">$message</span><!--[/error]-->
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="accesskey">通信密钥</label></h3>
            <div class="form-enter">
                <input maxlength="50" type="text" class="text" id="accesskey" name="accesskey" value="$_form.text('accesskey',$client.accesskey)" $_if($isEdit,'disabled="disabled"','') />
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="apifilepath">反向通知API文件路径</label></h3>
            <div class="form-enter">
                <input id="apifilepath" maxlength="200" style=" width:250px" type="text" value="$_form.text('apifilepath',$client.apifilepath)" name="apifilepath" $_if($isEdit,'disabled="disabled"','') class="text" />
                <!--[error name="apifilepath"]--><span class="form-tip tip-error">$message</span><!--[/error]-->
                <div class="formtip">用于接收Passport服务器反向通知的文件路径</div>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">反向通知</h3>
            <div class="datatablewrap" style="height:180px; background-color:White; border:solid 1px #777; padding:2px; line-height:110%">
            <div class="form-enter">
                    <!--[loop $temp in $Instructs]-->
                    <input type="checkbox" name="instructs" id="$temp" value="$temp" $_form.checked('instructs','$temp',{=$IsChecked($temp)}) />
                    <label for="$temp">$temp</label><br />
                    <!--[/loop]-->
                
            </div>
            </div> 
            <span>如不需要反向通知请选择"<span class="style1">Other</span>"</span>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="save" accesskey="s" title="保存"><span>保存(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->