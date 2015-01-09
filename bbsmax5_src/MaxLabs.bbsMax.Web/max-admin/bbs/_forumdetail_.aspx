<!--[unnamederror]-->
<div class="Tip Tip-error">$Message</div>
<!--[/unnamederror]-->
<!--[success]-->
<div class="Tip Tip-success">操作成功</div>
<!--[/success]-->
<!--[if $IsSuccess]-->
<div class="Tip Tip-success">操作成功</div>
<!--[/if]-->
	<div class="FormTable">
	<table>
        <!--[error name="forumName"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>版块名称</h4>
			    <input type="text" class="text" name="forumName" value="$_form.text("forumName",$Encode($out($forum.Name)))" />
			</th>
			<td>
			    可以使用部分HTML标签, 例如加粗<code>&lt;b&gt; &lt;strong&gt;</code>,
			    斜体<code>&lt;i&gt; &lt;em&gt;</code>等.
			    特殊符号必须使用HTML字符实体, 例如<code class="red">(&lt; = &amp;lt;)
                (&gt; = &amp;gt;) (&amp; = &amp;amp;)</code>
			</td>
		</tr>
		<!--[error name="codeName"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>版块别名</h4>
			    <input type="text" class="text" name="codeName" value="$_form.text("codeName",$out($forum.CodeName))" />
			</th>
			<td>
			    用户版块URL地址. 只允许包含英文字母，数字，下划线，横杠
			</td>
		</tr>
        <!--[error name="sortorder"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>排序</h4>
			    <input type="text" class="text" name="sortorder" value="$_form.text("sortorder",$out($forum.SortOrder))" />
			</th>
			<td>
			    填写整数, 数字越小表示顺序越靠前.
			</td>
		</tr>
        <!--[error name="forumType"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>版块类型</h4>
			    <p>
			    <input id="forumtype_forum" type="radio" onclick="forumTypeChange()" name="forumType" value="Normal" checked="checked" <!--[if $DisabledForumType]-->disabled="disabled"<!--[/if]--> $_form.checked("forumType","Normal","$ForumType") />
			    <label for="forumtype_forum">版块</label>
			    </p>
			    <p>
			    <input id="forumtype_category" type="radio" onclick="forumTypeChange()" name="forumType" value="Catalog" <!--[if $DisabledCatalogItem || $DisabledForumType]-->disabled="disabled"<!--[/if]--> $_form.checked("forumType","Catalog","$ForumType") />
			    <label for="forumtype_category">分类</label>
			    </p>
			    <p>
			    <input id="forumtype_link" type="radio" onclick="forumTypeChange()" name="forumType" value="Link" <!--[if $DisabledForumType]-->disabled="disabled"<!--[/if]--> $_form.checked("forumType","Link","$ForumType") />
			    <label for="forumtype_link">链接</label>
			    </p>
			</th>
			<td>&nbsp;</td>
		</tr>
		<tbody id="trForumLink">
		<!--[error name="forumLink"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>链接地址</h4>
			    <input id="forumLink" name="forumLink" type="text" class="text" value="$_form.text("forumLink",$ForumLink)"  />
			</th>
			<td>链接地址必须包含<code>http://</code>或者<code>https://</code></td>
		</tr>
		<tr>
			<th>
			    <h4>链接打开方式</h4>
      	        <input type="radio" id="linktype1" name="linktype" value="false"  $_form.checked("linktype","false",$ForumExtendedAttribute.LinkOpenByNewWidow==false)  /> <label for="linktype1">本页面打开</label>
                <input type="radio" id="linktype2" name="linktype" value="true" $_form.checked("linktype","true",$ForumExtendedAttribute.LinkOpenByNewWidow) /> <label for="linktype2">新窗口打开</label>
			</th>
			<td>链接的打开方式</td>
		</tr>
		</tbody>
		<script type="text/javascript">
		    document.getElementById("trForumLink").style.display = 'none';
        </script>
		<tr>
			<th>
			<h4>分栏</h4>
			<select name="colSpan">
			<option value="0" $_form.selected("colSpan","0","$out($forum.columnSpan)")>不分栏</option>
			<option value="2" $_form.selected("colSpan","2","$out($forum.columnSpan)")>2栏</option>
			<option value="3" $_form.selected("colSpan","3","$out($forum.columnSpan)")>3栏</option>
			<option value="4" $_form.selected("colSpan","4","$out($forum.columnSpan)")>4栏</option>
			<option value="5" $_form.selected("colSpan","5","$out($forum.columnSpan)")>5栏</option>
			<option value="6" $_form.selected("colSpan","6","$out($forum.columnSpan)")>6栏</option>
			</select>
			</th>
			<td>子版块的显示方式.</td>
		</tr>
		<tbody id="tr_ParentForum">
		<tr>
			<th>
			<h4>所属版块</h4>
		    <select name="parentForum" <!--[if $isEdit]--> disabled="disabled"<!--[/if]-->>
		    <optgroup label="一级分类"></optgroup>
		    <!--[loop $tempForum in $Forums with $i]-->
		    <option value="$tempForum.ForumID"  $_form.selected("parentForum","$tempForum.ForumID","$ParentID")>&nbsp;&nbsp;&nbsp;&nbsp;$ForumSeparators[$i]|--$tempForum.ForumName</option>
		    <!--[/loop]-->
		    </select>
			</th>
			<td>&nbsp;</td>
		</tr>
		</tbody>
		<tbody id="moreOption">
        <!--[error name="description"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>版块说明</h4>
			<textarea name="description" id="description" cols="30" rows="6" style="height:200px;width:400px;">$_form.text("description",$out($forum.Description))</textarea>
			</th>
			<td>
			    <p>显示在版块标题下的简短说明文字. </p>
			</td>
		</tr>
        <!--[error name="readme"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
		<tr>
			<th>
			<h4>版块布告</h4>
		    <textarea name="readme" id="readme" cols="30" rows="6" style="height:200px;width:400px;">$_form.text("readme",$out($forum.Readme))</textarea>
			</th>
			<td>
			    <p>显示在主题列表页主题上方的文字, 适用于版块布告通知, 警告声明, 论坛规定, 详细说明等等.</p>
			</td>
		</tr>
        <!--[error name="logo"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->	
		<tr>
			<th>
			    <h4>版块图标</h4>
			    <input name="logo" type="text" class="text" id="logo" style="width:auto;" value="$_form.text("logo",$out($forum.LogoSrc))" />
			    <a title="选择图片" class="selector-image" href="javascript:void(browseImage('Assets_ForumLogo','logo'))"><img src="$Root/max-assets/images/image.gif" alt="" /></a>
			</th>
			<td>&nbsp;</td>
		</tr>
		<tr>
			<th>
			    <h4>版块界面风格</h4>
			    <select name="theme">
			    <option value="">默认风格</option>
			    </select>
			</th>
			<td>
			</td>
		</tr>
		<tr>
			<th>
			    <h4>版块访问密码</h4>
			    <input name="password" type="text" class="text" value="$_form.text("password",$ForumPassword)" />
			</th>
			<td>
			    留空表示不设置版块访问密码.
			</td>
		</tr>
		</tbody>
		</table>
        <table class="multiColumns" style="margin-bottom:1px;">
		<!--[loop $item in $ForumExtendedAttribute.SigninForumWithoutPassword with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$ForumExtendedAttribute.SigninForumWithoutPassword.Count" name="SigninForumWithoutPassword" hasnode="true" nodeName="下级版块" title="进入版块不需要密码" description="如果该版块需要密码，是否不需输入密码就可以进入版块" /]-->
	    <!--[/loop]--> 
	    </table>
	    <table>
	    <tr>
                <th>
                    <h4>浏览器标题栏附加文字</h4>
                    <input type="text" class="text" name="TitleAttach" value="$_form.text('TitleAttach',$ForumExtendedAttribute.TitleAttach)" />
                </th>
                <td>附加在HTML源码的TITLE标签内的文字, 特殊符号必须使用HTML字符实体. </td>
            </tr>
            <tr>
                <th>
                    <h4>HTML头部的Meta关键字列表</h4>
                    <input type="text" class="text" name="MetaKeywords" value="$_form.text('MetaKeywords',$ForumExtendedAttribute.MetaKeywords)" />
                </th>
                <td>META标签的keywords内容. 关键字之间使用 "," 分隔.</td>
            </tr>
            <tr>
                <th>
                    <h4>HTML头部的Meta站点描述信息</h4>
                    <input type="text" class="text" name="MetaDescription" value="$_form.text('MetaDescription',$ForumExtendedAttribute.MetaDescription)"  />
                </th>
                <td>META标签的description内容.</td>
            </tr>
	    </table>
        <table>
		<tr class="nohover">
		    <th>
		    <input type="submit" value="保存设置" name="saveforum" class="button" />
		    </th>
		    <td>&nbsp;</td>
	    </tr>
	</table>
	</div>
<script type="text/javascript" src="$root/max-assets/nicedit/nicEdit.js"></script>
<script type="text/javascript">
forumTypeChange();
function forumTypeChange()
{
    var forumType = document.getElementsByName('forumType');
    var forumTypeValue;
    for(var i=0;i<forumType.length;i++)
    {
        if(forumType[i].checked)
        {
            forumTypeValue = forumType[i].value;
            break;
        }
    }
    
    if(forumTypeValue == 'Link')
    {
        document.getElementById("trForumLink").style.display = '';
        document.getElementById("moreOption").style.display = 'none';
        $('tr_ParentForum').style.display = '';
    }
    else if(forumTypeValue == 'Catalog')
    {
        document.getElementById("trForumLink").style.display = 'none';
        document.getElementById("moreOption").style.display = 'none';
        $('tr_ParentForum').style.display = 'none';
    }
    else
    {
        document.getElementById("trForumLink").style.display = 'none';
        document.getElementById("moreOption").style.display = '';
        $('tr_ParentForum').style.display = '';
    }
}
addPageEndEvent( function(){initMiniEditor(editorToolBar.setting);} );
</script>