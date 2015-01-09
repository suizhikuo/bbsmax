<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>SEO基本设置</title>
<!--#include file="../_htmlhead_.aspx"-->
</head>
<body>
<!--#include file="../_head_.aspx" -->
<!--#include file="../_setting_msg_.aspx" -->
<div class="Content">
    <h3>SEO基本设置</h3>
    <div class="FormTable">
        <form action="$_form.action" method="post">
        <table>

<!--#include file="../_inc/_row_textbox.aspx" id="TitleAttach" value="$baseseosettings.TitleAttach" title="浏览器标题栏附加文字" note="附加在HTML源码的TITLE标签内的文字, 特殊符号必须使用HTML字符实体" -->

<!--#include file="../_inc/_row_textbox.aspx" id="MetaKeywords" value="$baseseosettings.MetaKeywords" title="Head标签内的Meta关键字列表" note="META标签的keywords内容. 关键字之间使用 &quot;,&quot; 分隔" -->

<!--#include file="../_inc/_row_textbox.aspx" id="MetaDescription" value="$baseseosettings.MetaDescription" title="Head标签内的Meta描述信息" note="META标签的description内容" -->

<!--#include file="../_inc/_row_textarea.aspx" id="OtherHeadMessage" value="$baseseosettings.OtherHeadMessage" title="Head标签内的自定义内容" note="自定义内容将出现在HTML源码的HEAD标签内, 必须使用合法的HTML标签(如meta, link, script, style), 并确保标签正确的结束.<br />例如：&lt;meta name=&quot;description&quot; content=&quot;世界顶级的asp.net论坛程序&quot; /&gt;" -->

<!--#include file="../_inc/_row_enable.aspx" id="TitleAddPageNumber" checked="$baseseosettings.TitleAddPageNumber" title="是否在浏览器标题中显示页码" note="在标题中显示页码对搜索引擎更加友好（推荐使用）" -->

<!--#include file="../_inc/_row_textbox.aspx" id="TitleAddPageNumberFormat" value="$baseseosettings.TitleAddPageNumberFormat" title="在浏览器标题中显示页码的格式" note="如 “第{0}页”则第一页显示“第1页”，“{0}”表示页码" -->

<!--#include file="../_inc/_row_enable.aspx" id="EncodePostUrl" checked="$baseseosettings.EncodePostUrl" title="加密帖子,签名中链接的URL" note="加密后，用户发表的帖子、签名中的链接将以脚本方式输出，搜索引擎蜘蛛将完全没有办法进入这些链接" -->

            <tr>
                <th><input type="submit" value="保存设置" class="button" name="savesetting" /></th>
                <td>&nbsp;</td>
            </tr>
        </table>
        </form>
    </div>
</div>
<!--#include file="../_foot_.aspx"-->
</body>
</html>
