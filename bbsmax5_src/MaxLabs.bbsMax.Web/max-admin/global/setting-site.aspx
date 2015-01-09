<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>$_if($HasBBS,"设置论坛功能" ,"Passport站点设置" )</title>
<!--[include src="../_htmlhead_.aspx" /]-->
<script type="text/javascript" src="$root/max-assets/nicedit/nicEdit.js"></script>
<script type="text/javascript">
    initDisplay('ForumClosed', [
 { value: '0', display: false, id: 'CloseReason' }
, { value: '1', display: true, id: 'CloseReason' }
, { value: '2', display: true, id: 'CloseReason' }
, { value: '2', display: true, id: 'Timinglist' }
, { value: '0', display: false, id: 'Timinglist' }
, { value: '1', display: false, id: 'Timinglist' }
]);
 addPageEndEvent(function() { initMiniEditor('ForumCloseReason', editorToolBar.setting); });
   
    function insertScope(s) {
        showTable(true);
        var l = $("scope_list");
        var id = l.rows.length;
        var r = l.insertRow(id);

        r.id = "item_" + s.ID;
        
        id = 0;
        var c = r.insertCell(id++);
        c.innerHTML = s.Message;
        c=r.insertCell(id++);
        c.innerHTML = s.OperetorName;
        c=r.insertCell(id++);
        c.innerHTML = s.OperetingDatetime;
        c = r.insertCell(id++);
        c.innerHTML = String.format('<a href="$dialog/timing-delete.aspx?type=1&scopeid={0}" onclick="return openDialog(this.href,function(){ removeElement($(\'item_{0}\'))})">删除</a>', s.ID); 
    }

    function showTable(b) {
        var s = $('DataTable');
        var e = $('emptydata');
        if (b == true) {
            s.style.display = "";
            e.style.display = "none";
        }
        else {
            s.style.display = "none";
            e.style.display = "";
        }
    }

</script>

<script type="text/javascript">
    function showlink() {
            $("timing").style.display = "none";
    }
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>$_if($HasBBS,"设置论坛功能" ,"Passport站点设置" )</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
    <table style="margin-bottom:1px;">
        <!--[error name="ForumClosed"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr $_if( ! $HasBBS,'style="display:none;"')>
            <th>
                <h4>论坛功能</h4>
                <p><input type="radio" name="ForumClosed" id="ForumClosed1" value="0" $_form.Checked("ForumClosed","0",($SiteSettings.ForumClosed==ForumState.Open)) /> <label for="ForumClosed1">开放论坛</label></p>
                <p><input type="radio" name="ForumClosed" id="ForumClosed2" value="1" $_form.Checked("ForumClosed","1", ($SiteSettings.ForumClosed==ForumState.Closed)) /> <label for="ForumClosed2">关闭论坛</label></p>
                <p><input type="radio" name="ForumClosed" id="ForumClosed3" value="2" $_form.Checked("ForumClosed","2", ($SiteSettings.ForumClosed==ForumState.TimingClosed)) /> <label for="ForumClosed3">定时关闭论坛</label></p>
            </th>
            <td>
            设置“关闭论坛”将使论坛前台无法被访问，请谨慎使用。<br />
            当您要恢复论坛的正常访问时，可以再回到这里设置“开放论坛”。<br />
            如果您只想对论坛的访问或注册做限制，那么您可以尝试使用“<a href="$admin/global/setting-accesslimit.aspx" target="_blank">访问限制</a>”和“<a href="$admin/global/setting-registerlimit.aspx" target="_blank">注册限制</a>”功能。<br />
            当从其他模式切换到"定时关闭论坛"模式,进行添加删除操作,要点击"保存设置",才能生效.
            </td>
        </tr>
        <tbody id="Timinglist">
        <tr>
            <th colspan="2">
                <h4>定时关闭访问时间列表 <a id="timing" href="$dialog/timing-add.aspx?type=0" onclick="return openDialog(this.href,function( s ){insertScope(s);})" > 添加时间范围</a></h4>
                <div id="emptydata">当前没有指定定时范围</div>
                <table id="DataTable" class="DataTable">
                <thead>
                     <tr>
                       <td>定时范围 </td>
                       <td>操作者  </td>
                       <td>添加时间  </td>
                       <td>操作</td>
                    </tr>
                </thead>
                <tbody id="scope_list">
                <!--[loop $item in $ScopeItemList]-->
                <tr>
                   <td>$item.Message </td>
                   <td>$item.OperetorName </td>
                   <td>$item.OperetingDatetime </td>
                   <td><a href="$dialog/timing-delete.aspx?type=0&scopeid=$item.id" onclick="return openDialog(this.href,function(){ removeElement($('item_$item.id'))})" >删除</a></td>
                </tr>
                <!--[/loop]-->
                </tbody>
                </table>
                <!--[if $ListIsEmpty]-->
                <script type="text/javascript">showTable(false);</script>
                <!--[else]-->
                <script type="text/javascript">showTable(true);</script>
                <!--[/if]-->
            </th>
        </tr>
        </tbody>
        <!--[error name="ForumCloseReason"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr id="CloseReason">
            <th>
                <h4>论坛关闭原因</h4>
                <div class="htmleditorwrap">
                <textarea name="ForumCloseReason" id="ForumCloseReason" style="width:350px;height:150px;" cols="30" rows="6">$_form.text('ForumCloseReason',$SiteSettings.ForumCloseReason)</textarea>
                </div>
            </th>
            <td>
            在论坛关闭期间，您可以使用此设置，告知访问论坛的用户，论坛已经关闭和关闭原因等。
            </td>
        </tr>
        <tbody id="forumOpen">
        <!--[error name="SiteName"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>网站名称</h4>
                <input type="text" class="text" name="SiteName" value="$_form.text('SiteName',$SiteSettings.SiteName)" />
            </th>
            <td>
            </td>
        </tr>
        <!--[error name="SiteUrl"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>网站地址</h4>
                <input type="text" class="text" name="SiteUrl" value="$_form.text('SiteUrl',$SiteSettings.SiteUrl)" />
            </th>
            <td>
            </td>
        </tr>
        <!--[error name="BbsName"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr $_if( ! $HasBBS,'style="display:none;"')>
            <th>
                <h4>论坛名称</h4>
                <input type="text" class="text" name="BbsName" value="$_form.text('BbsName',$SiteSettings.BbsName)" />
            </th>
            <td>
            简洁并贴切主题的论坛名称能帮助你更好的推广论坛。
            </td>
        </tr>
        <!--[error name="ForumIcp"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>ICP备案信息</h4>
                <input type="text" class="text" name="ForumIcp" value="$_form.text('ForumIcp',$SiteSettings.ForumIcp)" />
            </th>
            <td>
            如果您还不了解什么是ICP备案，可以点击<a href="http://baike.baidu.com/view/33319.htm?func=retitle" target="_blank">此处</a>（链接到百度百科）了解更多。<br />
            您还可以点击<a href="http://www.miibeian.gov.cn/" target="_blank">此处</a>快速跳转到ICP备案网站。
            </td>
        </tr>
        <!--[error name="StatCode"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>第三方统计代码</h4>
                <textarea cols="50" rows="4" name="StatCode">$_form.text('StatCode',$SiteSettings.StatCode)</textarea>
            </th>
            <td>
            中国境内主流的第三方统计机构：<a href="http://www.cnzz.com/" target="_blank">cnzz</a>、<a href="http://www.51.la" target="_blank">51la</a>
            </td>
        </tr>
        <!--[error name="DefaultFeedType"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr $_if( ! $HasBBS,'style="display:none;"')>
            <th>
                <h4>用户中心首页默认显示 </h4>
                <p>
                <input type="radio" name="DefaultFeedType" id="DefaultFeedType1" value="AllUserFeed" $_form.checked('DefaultFeedType','AllUserFeed','$SiteSettings.DefaultFeedType') />
                <label for="DefaultFeedType1">全站动态</label>
                </p>
                <p>
                <input type="radio" name="DefaultFeedType" id="DefaultFeedType2" value="FriendFeed" $_form.checked('DefaultFeedType','FriendFeed','$SiteSettings.DefaultFeedType') />
                <label for="DefaultFeedType2">好友动态</label>
                </p>
                <p>
                <input type="radio" name="DefaultFeedType" id="DefaultFeedType3" value="MyFeed" $_form.checked('DefaultFeedType','MyFeed','$SiteSettings.DefaultFeedType') />
                <label for="DefaultFeedType3">个人动态</label>
                </p>
            </th>
            <td>&nbsp;</td>
        </tr>
        <!--[error name="DisplaySiteNameInNavigation"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr $_if( ! $HasBBS,'style="display:none;"')>
            <th>
                <h4>在导航栏显示站点名称 </h4>
                <p>
                <input type="radio" name="DisplaySiteNameInNavigation" id="DisplaySiteNameInNavigation1" value="true" $_form.checked('DisplaySiteNameInNavigation','true',$SiteSettings.DisplaySiteNameInNavigation) />
                <label for="DisplaySiteNameInNavigation1">是</label>
                </p>
                <p>
                <input type="radio" name="DisplaySiteNameInNavigation" id="DisplaySiteNameInNavigation2" value="false" $_form.checked('DisplaySiteNameInNavigation','false',!$SiteSettings.DisplaySiteNameInNavigation) />
                <label for="DisplaySiteNameInNavigation2">否</label>
                </p>
            </th>
            <td>&nbsp;</td>
        </tr>
        <%--
        <tr>
            <th>
                <h4>默认短消息声音</h4>
                <input type="text" class="text" name="DefaultMessageSound" value="$_form.text('DefaultMessageSound',$SiteSettings.DefaultMessageSound)" />
            </th>
            <td>&nbsp;</td>
        </tr>
        --%>
    </table>
    <!--[if $HasBBS]-->
    <!--[loop $item in $SiteSettings.ViewIPFields with $index]-->
    <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$SiteSettings.ViewIPFields.Count" name="ViewIPFields" type="int" textboxwidth="4" title="用户可查看的IP地址段数量" description="如果有权限屏蔽IP，则可以看到完整的IP地址" /]-->
    <!--[/loop]-->
    <!--[/if]-->
    <table>
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
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
