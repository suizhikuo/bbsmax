<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/app.css" />
<link rel="stylesheet" type="text/css" href="$root/max-assets/kindeditor/skins/editor5.css" />
<script type="text/javascript">var root = '$Root'; var cookieDomain = '$CookieDomain'</script>
<script type="text/javascript" src="$root/max-assets/javascript/_source/max.js"></script>
<script src="$root/max-assets/javascript/max-lib.js" type="text/javascript"></script>
</head>
<body>
<div class="container">
<!--#include file="../../_inc/_head.aspx"-->
    <div id="main" class="main hasappsidebar section-app">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">

                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/blog.gif);">日志</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a $_if(SelectedEveryone, 'class="current" ' ) href="$url(app/blog/index)?view=everyone"><span>大家的日志</span></a></li>
                                        <li><a $_if(SelectedFriend, 'class="current" ' ) href="$url(app/blog/index)?view=friend"><span>好友的日志</span></a></li>
                                        <li><a $_if(SelectedMy, 'class="current" ' ) href="$url(app/blog/index)"><span>我的日志</span></a></li>
                                        <li><a $_if(SelectedCommented, 'class="current" ' ) href="$url(app/blog/index)?view=visited"><span>评论过的日志</span></a></li>
                                    </ul>
                                    <ul class="pagebutton">
                                        <li><a class="addblog" href="$url(app/blog/write)">发布新日志</a></li>
                                    </ul>
                                </div>
                            </div>
                            
                            <form method="post" action="$_form.action" enctype="multipart/form-data" id="theform">
                            <div class="clearfix workspace app-blog">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        
                                        <!--[unnamederror]-->
                                        <div class="errormsg">$message</div>
                                        <!--[/unnamederror]-->
                                        <!--[ValidateCode actionType="CreateBlogArticle"]-->
                                            <!--[error name="$inputName"]-->
                                        <div class="errormsg">$message</div>
                                            <!--[/error]-->
                                        <!--[/ValidateCode]-->
                                        
                                        <div class="formgroup appform blogwriteform">
                                            <div class="formrow">
                                                <h3 class="label"><label for="subject">文章标题</label></h3>
                                                <div class="form-enter">
                                                    <input type="text" class="text" id="subject" name="subject" value="$_form.text('subject', $Article.OriginalSubject)" size="60" />
                                                </div>
                                            </div>
                                            <div class="formrow">
                                                <h3 class="label"><label for="HtmlEditor">文章内容</label></h3>
                                                <div class="form-enter">
                                                     <!--[if $CanUseHtml || $CanUseUbb]-->
                                                     <!--[load src="../../_inc/_editor_.aspx" height="400" id="content" value="$Article.OriginalContent" useMaxCode="$CanUseUbb" Video="true" Audio="true" Flash="true"  Emoticons="true" mode="blog" /]-->
                                                     <!--[else]-->
                                                     <textarea class="blog-content" cols="50" rows="10" name="content" id="HtmlEditor">$_form.text("content", $Article.OriginalContent)</textarea>
                                                     <!--[/if]-->
                                                </div>
                                            </div>
                                            <div class="formrow" id="tagbox">
                                                <h3 class="label"><label for="tag">标签</label></h3>
                                                <div class="form-enter">
                                                    <input id="tag" class="text" type="text" value="$_form.text('tag', $ArticleTagList)" name="tag" />
                                                </div>
                                            </div>
                                            <!--[ValidateCode actionType="CreateBlogArticle"]-->
                                            <div class="formrow">
                                                <h3 class="label"><label for="$inputName">验证码</label> <span class="form-note">$tip</span></h3>
                                                <div class="form-enter">
                                                    <input type="text" class="text validcode" name="$inputName" id="$inputName" value="" onfocus="showVCode(this,'$imageurl');" autocomplete="off" $_if($disableIme,'style="ime-mode:disabled;"') />
                                                </div>
                                            </div>
                                            <!--[/ValidateCode]-->
                                            <div class="formrow formrow-action">
                                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="save" value="$_if(IsEditMode, '保存日志', '发布日志')" class="button" /></span></span>
                                            </div>
                                        </div>
                                        
                                    </div>
                                </div>
                                <div class="workspace-sidebar">
                                    <div class="workspace-sidebar-inner">
                                         
                                         <div class="formgroup appform blogoptionform">
                                            <div class="formrow">
                                                <h3 class="label"><label for="SelectBlogCategory">文章分类</label></h3>
                                                <div class="form-enter">
                                                    <!--[ajaxpanel id="catlist"]-->
                                                    <select name="category" id="SelectBlogCategory">
                                                        <option value="0" $_form.selected("category", "0", $Article.CategoryID == 0)>未分类</option>
                                                        <!--[loop $category in $CategoryList]-->
                                                        <option value="$Category.ID" $_form.selected("category", $Category.ID.ToString(), $Article.CategoryID == $Category.ID)>$Category.Name</option>
                                                        <!--[/loop]-->
                                                    </select>
                                                    <!--[/ajaxpanel]-->
                                                </div>
                                                <p class="form-note" id="catnew">
                                                    <a class="addcate" href="#" onclick="SwitchDisplay('catnew'); SwitchDisplay('catsubmit'); return SwitchDisplay('catinput')">新建分类</a>
                                                </p>
                                            </div>
                                            <script type="text/javascript">
                                                function SwitchDisplay(id) {
                                                    var div = document.getElementById(id);
                                                    if (div.style.display == 'none')
                                                        div.style.display = '';
                                                    else
                                                        div.style.display = 'none';
                                                    return false;
                                                }
                                            </script>
                                            <div class="formrow" id="catinput" style="display:none;">
                                                <div class="form-enter">
                                                    <input class="text" type="text" name="catname" />
                                                </div>
                                            </div>
                                            <div class="formrow formrow-action" id="catsubmit" style="display:none;">
                                                <span class="minbtn-wrap"><span class="btn"><a href="#" onclick="ajaxSubmit('theform','addcategory','catlist',null,null,true); SwitchDisplay('catnew'); SwitchDisplay('catsubmit'); return SwitchDisplay('catinput')" class="button">提交</a></span></span>
                                                <a href="#" onclick="SwitchDisplay('catnew'); SwitchDisplay('catsubmit'); return SwitchDisplay('catinput')">取消</a>
                                            </div>
                                            <div class="formrow">
                                                <h3 class="label">内容格式</h3>
                                                <div class="form-enter">
                                                    <input type="radio" name="format" id="formatubb" value="ubb" $_if($CanUseUbb && $CanUseHtml == false, 'checked="checked"') $_if($CanUseUbb == false, 'disabled="disabled"') />
                                                    <label for="formatubb">UBB</label>
                                                    <input type="radio" name="format" id="formathtml" value="html" $_if($CanUseHtml, 'checked="checked"') $_if($CanUseHtml == false, 'disabled="disabled"') />
                                                    <label for="formathtml">HTML</label>
                                                </div>
                                            </div>
                                            <div class="formrow">
                                                <h3 class="label"><label for="enablecomment">是否评论</label></h3>
                                                <div class="form-enter">
                                                    <input type="checkbox" name="enablecomment" id="enablecomment" value="true" $_form.checked("enablecomment","true",$Article.EnableComment == true) />
                                                    <label for="enablecomment">允许评论</label>
                                                </div>
                                            </div>
                                            <div class="formrow">
                                                <h3 class="label"><label for="privacytype">隐私设置</label></h3>
                                                <div class="form-enter">
                                                    <select id="privacytype" name="privacytype">
                                                        <option id="privacytype_AllVisible" value="AllVisible" $_form.selected("privacytype","AllVisible", $Article.PrivacyType.ToString())>全站用户可见</option>
                                                        <option id="privacytype_FriendVisible" value="FriendVisible" $_form.selected("privacytype","FriendVisible", $Article.PrivacyType.ToString())>好友可见</option>
                                                        <option id="privacytype_SelfVisible" value="SelfVisible" $_form.selected("privacytype","SelfVisible", $Article.PrivacyType.ToString())>仅自己可见</option>
                                                        <option id="privacytype_NeedPassword" value="NeedPassword" $_form.selected("privacytype","NeedPassword", $Article.PrivacyType.ToString())>凭密码查看</option>
                                                    </select>
                                                    <script type="text/javascript">
                                                        initDisplay("privacytype", [
                                                        { value: 'AllVisible', display: false, id: 'span_password' },
                                                        { value: 'AllVisible', display: true, id: 'tagbox' },
                                                        { value: 'FriendVisible', display: false, id: 'span_password' },
                                                        { value: 'FriendVisible', display: false, id: 'tagbox' },
                                                        { value: 'SelfVisible', display: false, id: 'span_password' },
                                                        { value: 'SelfVisible', display: false, id: 'tagbox' },
                                                        { value: 'NeedPassword', display: true, id: 'span_password' },
                                                        { value: 'NeedPassword', display: true, id: 'tagbox' }
                                                    ]);
                                                    </script>
                                                </div>
                                            </div>
                                            <div class="formrow" id="span_password">
                                                <h3 class="label"><label for="password">密码</label></h3>
                                                <div class="form-enter">
                                                    <input type="text" name="password" id="password" class="text" value="$_form.text('password','')" size="10" />
                                                </div>
                                            </div>
                                         </div>
                                         
                                         <!--[if $articlelist.count > 0]-->
                                         <div class="panel recentarticle">
                                            <div class="panel-head">
                                                <h3 class="panel-title"><span>编辑最近的日志</span></h3>
                                            </div>
                                            <div class="panel-body">
                                                <ul>
                                                    <!--[loop $article in $articlelist]-->
                                                    <li><a href="$url(app/blog/write)?id=$article.id">$article.subject</a></li>
                                                    <!--[/loop]-->
                                                </ul>
                                            </div>
                                        </div>
                                        <!--[/if]-->
                                         
                                    </div>
                                </div>
                            </div>
                            </form>
                            
                        </div>
                        <!--#include file="../../_inc/_round_bottom.aspx"-->
                    </div>
                </div>
            </div>
            <!--#include file="../../_inc/_sidebar_app.aspx"-->
        </div>
    </div>
<!--#include file="../../_inc/_foot.aspx"-->
</div>
</body>
</html>
