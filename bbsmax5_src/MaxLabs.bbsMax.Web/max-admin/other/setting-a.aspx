<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>广告设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>广告设置</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
        <table>
            <!--[error name="EnableAdverts"]-->
            <!--[include src="../_error_.aspx"/]-->
            <!--[/error]-->
            <tr>
                <th>
                    <h4>开启广告系统</h4>
                    <p>
                        <input type="radio" name="EnableAdverts" id="EnableAdverts1" value="true" $_form.checked('EnableAdverts','true',$AdSettings.EnableAdverts) />
                        <label for="EnableAdverts1">开启</label>
                        <input type="radio" name="EnableAdverts" id="EnableAdverts2" value="false" $_form.checked('EnableAdverts','false',!$AdSettings.EnableAdverts) />
                        <label for="EnableAdverts2">关闭</label>
                     </p>
                </th>
                <td> 选择“关闭”后整个系统的广告将不会输出 </td>
            </tr>
            <!--[error name="EnableDefer"]-->
            <!--[include src="../_error_.aspx"/]-->
            <!--[/error]-->
            <tr>
                <th>
                    <h4>广告加载模式</h4>
                    <p><select name="EnableDefer">
                    <option value="false" $_form.selected('EnableDefer','false',!$AdSettings.EnableDefer)>
                    页面加载时直接输出广告
                    </option>
                    <option value="true" $_form.selected('EnableDefer','true',$AdSettings.EnableDefer)>
                    页面加载完成后再加载广告
                    </option>
                    </select> 
                    </p>
                </th>
                <td>使用 “页面加载完成后再加载广告” 时若客户端不支持JS脚本，广告将不能正常显示。 但可以一定程度的提高页面的加载速度。</td>
            </tr>
            <tr class="nohover">
                <th colspan="2">
                    <h4>广告位管理</h4>
                    <ul class="adguidelist">
                        <li class="ad-banner-header"><a href="manage-a.aspx?categoryID=-1"><span>头部横幅($GetCount(-1,"None"))</span></a></li>
                        <li class="ad-banner-footer"><a href="manage-a.aspx?categoryID=-2"><span>底部横幅($GetCount(-2,"None"))</span></a></li>
                        <li class="ad-banner-leaderboard"><a href="manage-a.aspx?categoryID=-10"><span>顶部通栏($GetCount(-10,"None"))</span></a></li>
                        <li class="ad-squarepopup"><a href="manage-a.aspx?categoryID=-5"><span>漂浮广告($GetCount(-5,"None"))</span></a></li>
                        <li class="ad-skyscraper"><a href="manage-a.aspx?categoryID=-6"><span>对联广告($GetCount(-6,"None"))</span></a></li>
                        <li class="ad-text"><a href="manage-a.aspx?categoryID=-3"><span>页内文字($GetCount(-3,"None"))</span></a></li>
                        <li class="ad-banner-forumcate"><a href="manage-a.aspx?categoryID=-8"><span>分类间广告($GetCount(-8,"None"))</span></a></li>
                        <li class="ad-text-posttop"><a href="manage-a.aspx?categoryID=-4&pos=top"><span>帖子顶部文字($GetCount(-4,"Top"))</span></a></li>
                        <li class="ad-text-postbottom"><a href="manage-a.aspx?categoryID=-4&pos=bottom"><span>帖子底部文字($GetCount(-4,"Bottom"))</span></a></li>
                        <li class="ad-banner-post"><a href="manage-a.aspx?categoryID=-4&pos=right"><span>主题内广告($GetCount(-4,"Right"))</span></a></li>
                        <li class="ad-banner-signature"><a href="manage-a.aspx?categoryID=-9"><span>签名内广告($GetCount(-9,"None"))</span></a></li>
                        <li class="ad-banner-postdivision"><a href="manage-a.aspx?categoryID=-7"><span>帖子间广告($GetCount(-7,"None"))</span></a></li>
                        <li class="ad-banner-topicdivision"><a href="manage-a.aspx?categoryID=-11"><span>置顶帖下方广告($GetCount(-11,"None"))</span></a></li>
                    </ul>
                </th>
            </tr>
            <tr>
                <th>
                    <input type="submit" value="保存设置" class="button" name="savesetting" />
                </th>
                <td>&nbsp;</td>
            </tr>
        </table>
    </div>
    </form>
</div>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
