<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>友好URL设置</title>
<!--[include src="../_htmlhead_.aspx" /]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>友好URL设置</h3>
	<form action="$_form.action" method="post">
	<div class="Tip Tip-alert">
	    经常性更改URL路径格式, 会影响搜索引擎收录. 请慎重使用.
	</div>
	<div class="FormTable">
	<table>
        <!--[error name="UrlPattern"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>URL路径格式</h4>
			    <p><input type="radio" name="UrlFormat" id="UrlFormat1" value="Aspx" $_form.checked("UrlFormat", "Aspx", $FriendlyUrlSettings.UrlFormat == UrlFormat.Aspx) /> <label for="UrlFormat1">ASP.NET模式</label></p>
			    <p class="desc" style="padding-left:15px">(例: bbsmax.com/music/thread-1-1.aspx)</p>
			    <p><input type="radio" name="UrlFormat" id="UrlFormat2" value="Html" $_form.checked("UrlFormat", "Html", $FriendlyUrlSettings.UrlFormat == UrlFormat.Html) /> <label for="UrlFormat2">静态页模式</label></p>
			    <p class="desc" style="padding-left:15px">(例: bbsmax.com/music/thread-1-1.html)</p>
			    <p><input type="radio" name="UrlFormat" id="UrlFormat3" value="Query" $_form.checked("UrlFormat", "Query", $FriendlyUrlSettings.UrlFormat == UrlFormat.Query) /> <label for="UrlFormat3">参数模式</label></p>
			    <p class="desc" style="padding-left:15px">(例: bbsmax.com/?music/thread-1-1)</p>
			    <p><input type="radio" name="UrlFormat" id="UrlFormat4" value="Folder" $_form.checked("UrlFormat", "Folder", $FriendlyUrlSettings.UrlFormat == UrlFormat.Folder) /> <label for="UrlFormat4">无后缀模式</label></p>
			    <p class="desc" style="padding-left:15px">(例: bbsmax.com/music/thread-1-1)</p>
			</th>
			<td>
			    <p>左边灰色的表示当前服务器设置不支持此模式</p>
			    <p>参数模式适用于所有服务器，建议其他模式都无法使用的用户选择此模式.</p>
			    <p>ASP.NET模式适用于大部分服务器，推荐选择.</p>
			    <p>静态页模式需要在IIS中将 *.html 映射给asp.net处理.</p>
			    <p>无后缀模式需要在IIS中将 * 映射给asp.net处理.</p>
			</td>
		</tr>
		<tr class="nohover">
			<th>
			<input type="submit" value="保存设置" class="button" name="savesetting" />
			</th>
			<td>&nbsp;</td>
		</tr>
	</table>
	</div>
	</form>
</div>
<script>
var browser={
        isIE     : navigator.userAgent.toLowerCase().indexOf('msie',0)   >=0,
        isIE5    : navigator.userAgent.toLowerCase().indexOf('msie 5',0) >=0,
        isIE6    : navigator.userAgent.toLowerCase().indexOf('msie 6',0) >=0,
        isIE7    : navigator.userAgent.toLowerCase().indexOf('msie 7',0) >=0,
        isGecko  : navigator.userAgent.toLowerCase().indexOf('gecko',0)  >=0,
        isSafari : navigator.userAgent.toLowerCase().indexOf('safari',0) >=0,
        isOpera  : navigator.userAgent.toLowerCase().indexOf('opera',0)  >=0
    };

var ajax=new function()
{
    function getXmlHttp()
    {
        var xmlHttp=false;
        if(browser.isIE)
        {
            try 
            {
                xmlHttp = new ActiveXObject("Msxml2.XMLHTTP");
            } 
            catch (e) 
            {
                try 
                {
                    xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
                }
                catch (e2) 
                {
                    xmlHttp = false;
                }
            }
        }
        else
        {
            xmlHttp=new XMLHttpRequest();
        }
        if(xmlHttp)
        {
            return xmlHttp;
        }
        return false;
    };
   return{
     get : function(url,processFunc){
            var xmlhttp=getXmlHttp();
            xmlhttp.open("GET", url, true);
            xmlhttp.onreadystatechange = function(){
              if (xmlhttp.readyState == 4) 
                {
                    var result;
                    if(xmlhttp.responseText)
                        result = xmlhttp.responseText;
                    else
                        result = '';    
                    processFunc(xmlhttp.status,result);  
                }
            };
            xmlhttp.send(null);
     }
   }
}


ajax.get("checkurlmode.aspx?checkurlmode=1",function(s,r){
    if(r!='success')
        document.getElementById('UrlFormat1').disabled = true;
});
ajax.get("checkurlmode.html?checkurlmode=1",function(s,r){
    if(r!='success')
        document.getElementById('UrlFormat2').disabled = true;
});
ajax.get("checkurlmode?checkurlmode=1",function(s,r){
    if(r!='success')
        document.getElementById('UrlFormat4').disabled = true;
});
</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
