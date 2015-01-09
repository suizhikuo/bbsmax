<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>手机认证设置</title>
<!--[include src="../_htmlhead_.aspx" /]-->
    <style type="text/css">
        .style1
        {
            color: #0000CC;
        }
        .style2
        {
            color: black;
        }
        .style3
        {
            color: #CC0000;
        }
        .style4
        {
            color: #FF0000;
        }
    </style>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
<!--[if !$HasSMSPlugin]-->
    <div class="Help">
    没有找到短信发送插件， 无法开启手机验证功能！
    请在bbsmax.config的maxPlugins节点下加入您的短信发送插件配置，<br />
    如下：<br />
        <span class="style1">&lt;</span><span class="style3">add</span> 
        <span class="style4">name</span><span 
            class="style1">=<span class="style2">"</span>$PluginName</span><span 
            class="style2">"</span> <span class="style4">type</span><span class="style1">=<span class="style2">"</span>短信发送对象全路径名</span><span 
            class="style2">"</span> <span class="style4">assembly</span><span class="style1">=</span><span 
            class="style2">"</span><span 
            class="style1">程序集名称<span class="style2">"</span>/&gt; </span>
    <br />
    <br />
    您的短信发送插件必须实现<span class="style1">MaxLabs.bbsMax.ISMSSender</span>接口

    </div>
<!--[/if]-->
	<h3>设置认证功能</h3>
	<form action="$_form.action" method="post">
	<div class="FormTable">
        <table>
            <tr>
                 <th>
                    <h4>是否开启认证</h4>
                    <!--[if $HasSMSPlugin]-->
                    <p><input type="radio" name="open" id="open1" value="true" $_form.checked('open','true',($PhoneValidateSettings.Open==true)) /><label for="open1">是</label> </p>
                    <p><input type="radio" name="open" id="open2" value="false" $_form.checked('open','false', ($PhoneValidateSettings.Open==false)) /><label for="open2">否</label> </p>
                    <!--[else]-->
                    <p><input type="radio" name="open" id="Radio1" value="true" disabled="disabled" /><label for="Radio1">是</label> </p>
                    <p><input type="radio" name="open" id="Radio2" value="false" checked="checked" disabled="disabled" /><label for="Radio2">否</label></p>
                    <!--[/if]-->
                </th>
            </tr>
        </table>
    <table>
        <tr class="nohover">
            <th><input type="submit" value="保存设置" $_if(!$HasSMSPlugin,'disabled="disabled"') class="button" name="savesetting" /></th>
            <td>&nbsp;</td>
        </tr>
    </table>
        
    </div>
    </form>
 </div>   
<!--[include src="../_foot_.aspx"/]-->    
</body>
</html>
