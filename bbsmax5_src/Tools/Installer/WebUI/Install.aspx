<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" ValidateRequest="false" CodeBehind="Install.aspx.cs" ResponseEncoding="utf-8" Inherits="Max.WebUI.Install" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>bbsMax5.0安装向导</title>
<link href="<%=Self%>?fileName=max_install.css" rel="stylesheet" type="text/css" />
</head>
<body>
  <script type="text/javascript">
function oAjax () {
    this.req = null;
    this.url = '';
    this.content = '';
    this.type = 'text';
    this.encode = '';
    this.asyn = true;
    this.action = 'get';
    this.error = false;
}
oAjax.prototype.init = function () {
    if (window.XMLHttpRequest) {
        this.req = new XMLHttpRequest();
    }
    else if (window.ActiveXObject) {

        try {
            this.req = new ActiveXObject("Msxml2.XMLHTTP");
        }
        catch (e) {
            try {
                this.req = new ActiveXObject("Microsoft.XMLHTTP");
            }
            catch(e) {
                this.req = false;
            }
        }
    }
    var self = this;
    if (this.req) {
        this.req.onreadystatechange = function () {self.listener()};
    }
};
oAjax.prototype.listener = function () {
    if (this.req.readyState == 4) {
	    this.callback(this.req.status, this.req.responseText);
    }
};
oAjax.prototype.send = function (url) {
    this.init();

    url = this.url = url || this.url || '';
    this.content = !!this.content ? this.content : '';
    this.encode = this.encode ? this.encode.toLowerCase() : '';
    this.asyn = typeof(this.asyn) == 'undefined' ? true : !!this.asyn;
    this.action = (typeof(this.action) == 'undefined' || this.action == 'get') ? 'Get' : 'Post';
    this.error = typeof(this.error) == 'undefined' ? false : !!this.error;
   
    this.req.open(this.action, url, this.asyn);
    this.req.setRequestHeader('Connection', 'close');
    this.req.setRequestHeader('Accept-Encoding', 'gzip, deflate');
    this.req.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded' + (this.encode ? ';charset=' + this.encode : ''));
    this.req.send(this.content);
};
oAjax.prototype.callback = function (content) {

};

oAjax.prototype.abort = function () {
    this.req.abort();
};
oAjax.prototype.halt = function (description) {
    this.error && alert(description);
};

getContent.stop = false;
function getContent () {
    if (getContent.stop) return;

    var ajax = new oAjax();
    ajax.url = '<%=Self%>?ajaxstr=getstate&rnd=' + Math.random();
    ajax.callback = function (status, content) {
        if (status == 200) {
            callback(content);
        }
        else {
            location.reload(true);
        }
        setTimeout(getContent, 1500);
    }
    ajax.send();
}

function _get(id) {
    return document.getElementById(id);
}
function completed(value) {
    if (value == "取  消") {
        if (confirm("您确定要退出安装？")) window.top.close();
    } else {
        if (_get('message') && _get('message').innerHTML != '') {
            alert('重要提示：请确认您已经仔细阅读红色的警告并已经操作完毕，否则可能存在安全隐患或运行出错！');
            if (confirm('您真的已经仔细阅读红色的警告并已经操作完毕了吗？没有请点“取消”并仔细查看红色警告！')) location.href = "./";
        }
        else {
            location.href = "./";
        }
    }
}

function checkPassword() {
    var confirmMsg;
    if (_get('RabSetupMode_0').checked) {
        if (_get('siteName') && _get('siteName').value == '') { alert('网站名称不能为空！'); return false; }
        if (_get('siteUrl') && _get('siteUrl').value == '') { alert('网站URL不能为空！'); return false; }
        if (_get('bbsName') && _get('bbsName').value == '') { alert('论坛名称不能为空！'); return false; }
        if (_get('AdminName') && _get('AdminName').value == '') { alert('管理员帐号不能为空！'); return false; }
        if (_get('AdminPassword') && _get('AdminPassword').value == '') { alert('管理员密码不能为空！'); return false; }

        var pwd = _get('<%=AdminPassword.ClientID%>').value;
        var cpwd = _get('ConfirmPassword').value;
        if (pwd != cpwd) {
            alert('确认密码/密码不一致，请重新填写！！！');
            return false;
        }

        if (!confirm('确认要全新安装吗？\r\n\r\n如果当前数据库已经存在数据，请务必备份。'))
            return false;
    }
    else {
        if (!confirm('确认开始安装吗？\r\n\r\n此操作开始后不可恢复，并将删除旧版本的所有目录及文件（包括模板），请一定保证先备份文件和数据库！'))
            return false;
    }
    return true;
}
    
var isKeyDown = false;
document.onkeydown = function(e) {
    e = e || window.event;
    if (e.keyCode == 13) {
        isKeyDown = true;
        var objs = document.getElementsByTagName('input');
        for (var s in objs) {
            if (objs[s].type == 'submit' && objs[s].getAttribute('focus') == 'true') {
                objs[s].click();
                break;
            }
        }
    }
}

function _submit(th) {
    if (isKeyDown) { return false; }
    return true;
}

function disable(obj) {
    setTimeout(function() { obj.disabled = true; }, 30);
}

function isShowAccountTextbox() {
    if (_get('ThirdIsWindows_0').checked) _get('thirdsql').style.visibility = 'inherit';
    else _get('thirdsql').style.visibility = 'hidden';
}

function switchSetupMode() {
    if (_get('RabSetupMode_0').checked) {
        _get('setupmode_new').style.display = '';
        if (_get('setupmode_upgrade')) _get('setupmode_upgrade').style.display = 'none';
    }
    else {
        _get('setupmode_new').style.display = 'none';
        if (_get('setupmode_upgrade')) _get('setupmode_upgrade').style.display = '';
    }
}

function setText(cid, text)
{
    if (!_get(cid)) return;
    _get(cid).innerHTML = text;
}

function progressOK(cid) {
    if (!_get(cid)) return;
    _get(cid).style.width = '0px';
    _get(cid + '_OK').width = 14;
    _get(cid + '_OK').height = 14;
}

    function progressLoading(cid) {
        var c_loading = _get(cid);
        if (!c_loading) return;
        if (c_loading.className != 'loader') {
            c_loading.className = 'loader';
            c_loading.style.width = '16px';
        }
        _get(cid + '_OK').width = 0;
    }

    function progressError(cid) {
        var c_error = _get(cid);
        if (!c_error) return;
        if (c_error.className != 'no') {
            c_error.className = 'no';
            c_error.style.width = '16px';
        }
        _get(cid + '_OK').width = 0;
    }

    function callback(c) {
        if (c && c.length > 0) {
            var cb = _get('completeall');
            var message = _get("message");

            var p;
            eval(c);
            getContent.stop = p.IsError || p.IsCompleted;
            if (p.message != '') { message.innerHTML = p.message + '<br />'; }
            if (!p.IsError) {
                switch (p.Step) {
                    case 1:
                        if (p.Percent < 100) { progressLoading('SetConfigImage'); }
                        else { progressOK('SetConfigImage'); progressLoading('DatabaseImage'); }
                        setText('DatabasePercent', '');
                        break;
                    case 2:
                        progressOK('SetConfigImage');
                        if (p.Percent < 100) { progressLoading('DatabaseImage'); setText('DatabasePercent', p.Percent + '%'); }
                        else{
                        progressOK('DatabaseImage'); setText('DatabasePercent', '');
                        //<% if (Max.Installs.Settings.Current.SetupMode == Max.Installs.SetupMode.New) {%>
                        progressLoading('CompletedImage');
                        //<% } else { %>
                        progressLoading('ConvertRolesImage');
                        //<% } %>
                        }
                        break;
                        
                    case 3:
                        progressOK('SetConfigImage');
                        progressOK('DatabaseImage');
                        setText('DatabasePercent', '');
                        if (p.Percent < 100) { progressLoading('ConvertRolesImage'); }
                        else { progressOK('ConvertRolesImage');progressLoading('ConvertAvatarImage'); }
                        break;

                    case 4:
                        progressOK('SetConfigImage');
                        progressOK('DatabaseImage');
                        progressOK('ConvertRolesImage');
                        setText('DatabasePercent', '');
                        if (p.Percent < 100) { progressLoading('ConvertAvatarImage'); setText('ConvertAvatarPercent', '已处理' + p.Percent + '%'); }
                        else { progressOK('ConvertAvatarImage'); setText('ConvertAvatarPercent', ''); progressLoading('MoveUserFilesImage'); }
                        break;

                    case 5:
                        progressOK('SetConfigImage');
                        progressOK('DatabaseImage');
                        progressOK('ConvertRolesImage');
                        progressOK('ConvertAvatarImage');
                        setText('DatabasePercent', '');
                        setText('ConvertAvatarPercent', '');
                        if (p.Percent < 100) { progressLoading('MoveUserFilesImage'); setText('ConvertDB', p.Percent + '% ' + p.title); }
                        else { progressOK('MoveUserFilesImage'); progressLoading('CompletedImage'); }
                        break;
                    
                    
                    case 6:
                        progressOK('SetConfigImage');
                        progressOK('DatabaseImage');
                        progressOK('ConvertRolesImage');
                        progressOK('ConvertAvatarImage');
                        progressOK('MoveUserFilesImage');
                        setText('DatabasePercent', '');
                        setText('ConvertAvatarPercent', '');
                        setText('ConvertDB', '');
                        if (p.Percent < 100) { progressLoading('CompletedImage'); }
                        else { progressOK('CompletedImage'); }
                        break;
                        
                        
                        
                    case 10000:
                        progressOK('SetConfigImage');
                        progressOK('DatabaseImage');
                        progressOK('ConvertRolesImage');
                        progressOK('ConvertAvatarImage');
                        progressOK('MoveUserFilesImage');
                        setText('DatabasePercent', '');
                        setText('ConvertAvatarPercent', '');
                        setText('ConvertDB', '');
                        progressOK('CompletedImage');
                        break;
                }
                if (p.IsCompleted) {
                        progressOK('SetConfigImage');
                        progressOK('DatabaseImage');
                        progressOK('ConvertRolesImage');
                        progressOK('ConvertAvatarImage');
                        progressOK('MoveUserFilesImage');
                        setText('DatabasePercent', '');
                        setText('ConvertAvatarPercent', '');
                        setText('ConvertDB', '');
                        progressOK('CompletedImage');
                    var pbl = _get("progressbar_last");
                    pbl && (pbl.style.width = '100%');
                    pbl && (pbl.innerHTML = '<span>100%</span>');
                    cb.value = "完  成";
                }
            }
            else {
                switch (p.Step) {
                    case 1:
                        progressError('SetConfigImage');
                        break;
                    case 2:
                        progressOK('SetConfigImage');
                        progressError('DatabaseImage');
                        break;
                    case 3:
                        progressOK('SetConfigImage');
                        progressOK('DatabaseImage');
                        progressError('ConvertRolesImage');
                        break;
                    case 4:
                        progressOK('SetConfigImage');
                        progressOK('DatabaseImage');
                        progressOK('ConvertRolesImage');
                        progressError('ConvertAvatarImage');
                        break;
                    case 5:
                        progressOK('SetConfigImage');
                        progressOK('DatabaseImage');
                        progressOK('ConvertRolesImage');
                        progressOK('ConvertAvatarImage');
                        progressError('MoveUserFilesImage');
                        break;
                    case 6:
                    default:
                        progressOK('SetConfigImage');
                        progressOK('DatabaseImage');
                        progressOK('ConvertRolesImage');
                        progressOK('ConvertAvatarImage');
                        progressOK('MoveUserFilesImage');
                        progressError('CompletedImage');
                        break;
                }
                alert(p.Error);
                cb.Value = "完  成";
            }
        }
    }

    function restrictstr(obj) {
        var str = obj.value;
        var len = str.replace(/[^\x00-\xff]/g, "**").length;
        if (len > 12) {
            obj.value = str.substring(0, str.length - 1);
            restrictstr(obj);
        }
        return;
    }
    </script>
    <form id="form1" runat="server" method="get" onsubmit="_submit(this)">
        <asp:Panel  ID="StepFirst" runat="server">
       <div id="content" class="container"> 
	       <div class="head">
    	 <h1>用户许可协议</h1>
         <p><span>1</span>/5</p>
          </div>  
	   	 <div id="progressbar" class="progressbar">
         <p style="width:0%;"><span style="right:-2em;">0%</span></p>
         </div>
           <div class="content">
	     
		     <h3 id="WelcomeTitle" runat="server">用户许可协议</h3>
		 <div class="license" id="WelcomeBody" runat="server">
			 1、本授权协议适用于 bbsmax 任何版本，bbsmax开发团队拥有对本授权协议的最终解释权和修改权。<br />
             <br />
             2、所有用户均可根据自己的需要对 bbsmax 进行修改。但无论何种情况，即：无论用途如何、是否经过修改或美化、修改程度如何，只要您使用 bbsmax 
             的任何整体或部分程序算法，都必须保留页脚处的 bbsmax 名称和 <a href="http://www.bbsmax.com">www.bbsmax.com</a> 
             的链接地址，且修改后的程序版权依然归 bbsmax开发团队所有。<br />
             <br />
             3、无论您从何处获得 bbsmax，只要未经商业授权，不得将本软件用于商业用途(企业网站或以盈利为目的经营性网站)，否则我们将保留追究的权力。有关 bbsmax 
             授权包含的服务范围，技术支持等，请参看 <a href="http://www.bbsmax.com">www.bbsmax.com</a>。对于违反以上条款，我们将依法追究其责任。<br />
             <br />
             4、您可以免费使用本程序用于非商业用途，并允许传播给其他人。但您不能将本软件出售或变相出售、搭售（包括但不限于出售其他产品赠送本软件等形式），这将被视为将本软件用于商业用途，需要经过我们的商业授权。<br />
             <br />
             <br />
             <strong>免责声明</strong><br />
             <br />
             1、利用本软件构建的网站的任何信息内容以及导致的任何版权纠纷和法律争议及后果，我们不承担任何责任。<br />
             <br />
             2、程序的使用(或无法再使用)中所有一般化、特殊化、偶然性或必然性的损坏（包括但不限于数据的丢失，自己或第三方所维护数据的不正确修改，和其他程序协作过程中程序的崩溃等）,我们不承担任何责任。<br />
               </div>
      </div>
           
          
		   <div class="operate">
              <input value="不同意" id="ToOut" type="button" onclick="window.top.close();" runat="server" />
              <asp:Button runat="server" Text="同  意" OnClientClick="return disable(this);" 
                   focus="true" ID="ToSecondNext" />
    		</div>
	  </div>
		</asp:Panel>
		
		<asp:Panel ID="StepSecond" runat="server">
		<div  class="container">
		<div class="head">
    	<h1>bbsmax 5.0 安装向导</h1>
          <p><span>2</span>/5</p>
        </div>
         <div class="progressbar">
            <p style="width:20%"><span>20%</span></p>
         </div>
			<div class="content">
    	   
    	     <h3>检查程序版本/目录权限</h3>
           <p><span id="SecondLastestApplicationImage" runat="server" class="space">.</span><asp:Literal ID="Request2" runat="server" Text="已经是最新版本的程序" ></asp:Literal></p><br />
           
           <% foreach (string result in CheckPermissionResult)
              { %>
           <p><%= result%></p>
           <% } %>
           
        </div>
			
		  <div class="operate">
		      <asp:Button runat="server" Text="上一步" OnClientClick="if(isKeyDown) return false;return disable(this);" ID="ToFirstPrev" />
              <asp:Button runat="server" Text="下一步" OnClientClick="return disable(this);" ID="ToThirdNext" focus="true" />
              <asp:Label ID="thirdNextInfo" runat="server" Visible="false" ForeColor="Red"></asp:Label>
	      </div>
	      </div>
		</asp:Panel>
		
		<asp:Panel ID="StepThird" runat="server">
		<div  class="container"> 	
		<div class="head">
    	<h1>bbsmax 5.0 安装向导</h1>
          <p><span>3</span>/5</p>
        </div>
         <div class="progressbar">
           <p style="width:40%"><span>40%</span></p>
        </div>
    <div class="content">
		<h3>配置安装模式</h3>
		<div id="checkconnect" style="position:absolute;display:none;height:100%;width:100%;top:0;left:0">
            <div style="position:absolute;height:100%;width:100%;background:#FFF;filter:progid:DXImageTransform.Microsoft.Alpha(opacity=50);-moz-opacity:0.5;opacity:0.5;"></div>
            <span style="position:absolute;background:#C00;color:#FFF;margin:5px;padding:6px 15px;left:50%;top:50%;margin-top:-1em;line-height:2;">
            正在测试连接...</span>
		</div>
        <div id="DataBase_Sqlite" runat="server">
        <h4>您安装的是bbsMax Sqlite数据库版</h4>
        <ul>
            <li class="px12">
            <p>以下是数据库文件路径(不能修改):</p>
            <asp:TextBox ID="bbsMaxFilePath" ReadOnly="true" runat="server" CssClass="text" Width="200px">\App_Data\bbsMax.config</asp:TextBox><br />
            <asp:TextBox ID="idMaxFilePath" ReadOnly="true" runat="server" CssClass="text" Width="200px">\App_Data\idMax.config</asp:TextBox>
            </li>
        </ul>
        </div>
	
        <div id="DataBase_SqlServer" runat="server" enableviewstate="false">
		<h4>SQL Server连接方式:</h4>
		<ul>
		    <li>
		        <span>数据库地址:</span>
		        <asp:TextBox ID="IdMaxServer" runat="server" CssClass="text" Width="260px" EnableViewState="true">(local)</asp:TextBox>
		        
		        <span>端口:</span>
		        <asp:TextBox ID="sqlServerPort" runat="server" CssClass="text" Width="50px" EnableViewState="true">1433</asp:TextBox>
		    </li>
		    <li>
                <asp:RadioButtonList ID="ThirdIsWindows" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                <asp:ListItem Text="  SQLServer验证 " Value="1"/>
                <asp:ListItem Text="  Windows验证 " Value="0" />
                </asp:RadioButtonList>
		    </li>
		</ul>
		<ul id="thirdsql" runat="server">
		    <li>
		        <span>数据库帐号:</span> <asp:TextBox ID="IdMaxUserID" Text="sa" runat="server" CssClass="text" Width="260px"></asp:TextBox>
		    </li>
		    <li>
		        <span>数据库密码:</span> <asp:TextBox ID="IdMaxPassword" runat="server" CssClass="text" Width="260px"></asp:TextBox>
		    </li>
		    <li>
		        <span>数据库名称:</span> <asp:TextBox ID="IdMaxDatabase" runat="server" Text="bbsmax" CssClass="text" Width="260px" EnableViewState="true"></asp:TextBox>
		        <asp:Button ID="CreateDatabase" runat="server" Text="创建数据库" />
		    </li>
		</ul>
		<script type="text/javascript">
		    isShowAccountTextbox();
		</script>
		<h4>商业版序列号（免费版仅供非盈利站点使用）:</h4>
		<ul>
		<li>
                <span>bbsmax序列号:</span>
                <asp:TextBox ID="Licence" Text="" runat="server" CssClass="text" Width="260px"></asp:TextBox> <a href="http://www.bbsmax.com/service.html" target="_blank">点击此处获得商业授权</a>
        </li>
		</ul>
		<h4>页面输出加速(gzip压缩):</h4>
		<ul>
            <li><span class="desc">如果您可以操作IIS，建议您关闭bbsmax实现的gzip压缩，改用IIS实现的gzip压缩以获得更高的性能。<br />
            如果您不知道如何设置本选项，请保留默认值</span></li>
            <li>
                <span>由bbsmax来实现页面gzip压缩:</span>
                <input id="DynamicCompress" name="DynamicCompress" type="checkbox" runat="server" checked="checked" />                
            </li>
            <li>
                <span>由bbsmax来实现js、css文件gzip压缩:</span>
                <input id="StaticCompress" name="StaticCompress" type="checkbox" runat="server" checked="checked" />
            </li>
		</ul>
	    </div>
    
        <div class="operate">
            <asp:Button runat="server" Text="上一步" OnClientClick="if(isKeyDown) return false;disable(this);" ID="ToSecondPrev" />
            <asp:Button runat="server" Text="下一步" focus="true" ID="ToFourthNext" 
            OnClientClick="_get('checkconnect').style.display='block';return disable(this);" 
            />
        </div>
        </div>
        
<script type="text/javascript">

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
            alert("");
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


ajax.get("checkurlmode.aspx?checkurlmode=1", function(s, r) {
        if (r != 'success') {
            document.getElementById('StaticCompress').disabled = true;
            document.getElementById('StaticCompress').checked = false;
        }
});
</script>
		</asp:Panel>
		
        <asp:Panel ID="StepFifth" runat="server">
            <div class="container">
		        <div class="head">
    	<h1>bbsmax 5.0 安装向导</h1>
        <p><span>4</span>/5</p>
        </div>
    
        <div class="progressbar">
        <p style="width:60%"><span>60%</span></p>
        </div>
     
    <div class="content">
        <h3>基本信息</h3>
        <ul>
            <li>
                <asp:RadioButtonList ID="RabSetupMode" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow">
                   <asp:ListItem Text=" 全新安装(严重警告：这将清空原有数据)" Value="0" />
                   <asp:ListItem Text=" " Value="1"  Selected="True"/>
                </asp:RadioButtonList>
            </li>
        </ul>
        <%if (RabSetupMode.Items[1].Enabled && settings != null) { %>
        <ul id="setupmode_upgrade" style="display:none">
	        <li><label class="fl">论坛名称:</label><%=settings.BBSName%></li>
            <li><label class="fl">网站名称:</label> <%=settings.SiteName%></li>
	        <li><label class="fl">网站首页:</label><%=settings.SiteUrl%></li>
	        <li><label class="fl">创始帐号:</label><%=settings.AdminName%></li>
	        <li><label class="fl">URL格式:</label><%=GetUrlFormat(settings.UrlFormat.ToString())%></li>
        </ul>
        <%}%>
        <ul id="setupmode_new" style="display:none">
	        <li><label class="fl">论坛名称:</label><input id="bbsName" type="text" class="text" size="26" style="width:260px;" runat="server" value="" /></li>
	        <li><label class="fl">网站名称:</label><input id="siteName" type="text" class="text" size="26" style="width:260px;" runat="server" value="" /></li>
	        <li><label class="fl">网站首页:</label><input id="siteUrl" type="text" class="text" size="26" style="width:260px;" runat="server" value="" /></li>
	        <li><label class="fl">创始帐号:</label><input id="AdminName" type="text" class="text" size="26" style="width:260px;" runat="server" value="admin" onkeyup="restrictstr(this);" onchange="restrictstr(this);" /></li>
	        <li><label class="fl">密　　码:</label><input id="AdminPassword" type="password" class="text" size="26" style="width:260px;" runat="server" /></li>			  
            <li><label class="fl">确认密码:</label><input id="ConfirmPassword" type="password" class="text" style="width:260px;" size="26" /></li>
            <li style="overflow:hidden;zoom:1;"><label class="fl">URL格式:</label>
                <div style="float:left;">
                    <p class="desc">灰色的表示您服务器的当前设置不支持</p>
                    <p>
                        <input type="radio" runat="server" name="UrlFormat" id="UrlFormat1" value="Aspx" />
                        <label for="UrlFormat1">ASP.NET模式</label>
                        <span class="desc">(例: bbsmax.com/music/thread-1-1.aspx)</span>
                    </p>
                    <p>
                        <input type="radio" runat="server" name="UrlFormat" id="UrlFormat2" value="Html" />
                        <label for="UrlFormat2">静态页模式</label>
                        <span class="desc">(例: bbsmax.com/music/thread-1-1.html)</span>
                    </p>
                    <p>
                        <input type="radio" runat="server" name="UrlFormat" id="UrlFormat3" value="Query" checked="true" />
                        <label for="UrlFormat3">参数模式</label>
                        <span class="desc">(例: bbsmax.com/?music/thread-1-1)</span>
                    </p>
                    <p>
                        <input type="radio" runat="server" name="UrlFormat" id="UrlFormat4" value="Folder" />
                        <label for="UrlFormat4">无后缀模式</label>
                        <span class="desc">(例: bbsmax.com/music/thread-1-1)</span>
                    </p>
                </div>
            </li>
        </ul>
    </div>
<script type="text/javascript">

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
            alert("");
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


ajax.get("checkurlmode.aspx?checkurlmode=1", function(s, r) {
    if (r != 'success') {
        document.getElementById('UrlFormat1').disabled = true;
    } else {
        document.getElementById('UrlFormat1').checked = true;
    }
});
ajax.get("checkurlmode.html?checkurlmode=1",function(s,r){
    if(r!='success')
    {
        document.getElementById('UrlFormat2').disabled = true;
    }
    else
        document.getElementById('UrlFormat2').checked = true;
});
ajax.get("checkurlmode?checkurlmode=1",function(s,r){
    if(r!='success')
        document.getElementById('UrlFormat4').disabled = true;
});
</script>
		     <script type="text/javascript">
		     switchSetupMode();
		     </script>
		        <div class="operate">
		     <asp:Button runat="server" OnClientClick="if(isKeyDown) return false;" Text="上一步" ID="ToThirdPrev" />
              <asp:Button runat="server" OnClientClick="return checkPassword();return disable(this);" focus="true" Text="下一步" ID="ToFifthNext" />
	      </div>	     
	      
	        </div>
		</asp:Panel>
			
		<asp:Panel ID="StepSixth" runat="server">

		<div class="container">
		<div class="head">
    	   <h1>bbsmax 5.0 安装向导</h1>
            <p><span>5</span>/5</p>
        </div>
    
         <div class="progressbar">
    <p id="progressbar_last" style="width:80%"><span>80%</span></p>
    </div>
    
    <div class="content">
    <h3>开始安装</h3>
<ul>
<li>
    <span class="space">
    <span ID="SetConfigImage" class="loader"></span>
    <img ID="SetConfigImage_OK" src="<%=Self%>?fileName=ok.gif" alt="" width="0" height="0" />
    </span>
    写入配置信息
</li>
<li>
    <span class="space">
    <span ID="DatabaseImage" class="space"></span>
    <img ID="DatabaseImage_OK" src="<%=Self%>?fileName=ok.gif" alt="" width="0" height="0" />
    </span>
    <% 
    if (Max.Installs.Settings.Current.SetupMode == Max.Installs.SetupMode.New)
    {
        Response.Write("建立数据库");
    }
    else if (Max.Installs.SetupManager.GetCurrentVersion() == Max.Installs.Settings.Version)
    {
        Response.Write("修复数据库(数据量多可能需要10分钟以上)");
    }
    else
    {
        Response.Write("升级数据库(数据量多可能需要10分钟以上)");
    }
    %>  <span id="DatabasePercent"></span>
</li>


    <% 
        if (Max.Installs.Settings.Current.SetupMode == Max.Installs.SetupMode.Update)
        {
        %>
<li>
    <span class="space">
    <span ID="ConvertRolesImage" class="space"></span>
    <img ID="ConvertRolesImage_OK" src="<%=Self%>?fileName=ok.gif" alt="" width="0" height="0" />
    </span>
    处理用户组和勋章数据
</li>
<li>
    <span class="space">
    <span ID="ConvertAvatarImage" class="space"></span>
    <img ID="ConvertAvatarImage_OK" src="<%=Self%>?fileName=ok.gif" alt="" width="0" height="0" />
    </span>
    处理头像数据  <span id="ConvertAvatarPercent"></span>
</li>
<li>
    <span class="space">
    <span ID="MoveUserFilesImage" class="space"></span>
    <img ID="MoveUserFilesImage_OK" src="<%=Self%>?fileName=ok.gif" alt="" width="0" height="0" />
    </span>
    正在做最后的数据库校对 <span id="ConvertDB"></span>
</li>
<% } %>


<li>
    <span class="space">
    <span ID="CompletedImage" class="space"></span>
    <img ID="CompletedImage_OK" src="<%=Self%>?fileName=ok.gif" alt="" width="0" height="0" />
    </span>
    完成安装
</li>
</ul>
    <div style="color:Red;position:relative;" id="message"></div>
    </div>
    <div class="operate">
        <asp:Button Text="取  消" focus="true" OnClientClick="completed(this.value);return false;" runat="server" id="completeall" />
    </div>
    </div>
    </asp:Panel>

    <div class="footer">&copy; Copyright 2002-2010, Max Labs. All Rights Reserved</div>
    </form>
</body>
</html>