<!--[DialogMaster title="添加链接" width="600"]-->
<!--[place id="body"]-->
<!--[include src="_error_.ascx" /]-->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">操作成功</div>
<!--[/success]-->

<form id="dialogForum" action="$_form.action" method="post">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="parent">父级菜单</label></h3>
            <div class="form-enter">
                <select id="parent" name="parent">
                <option value="0">顶级</option>
                <!--[loop $item in $NvigationItems]-->
                <!--[if $item.parentid==0]-->
                <option value="$item.id" $_if($item.id==$parentID,'selected="selected"','')>$item.name</option>
                <!--[/if]-->
                <!--[/loop]-->
                </select>
            </div>
        </div>
        
        <div class="clearfix addnavigator">
            <div class="item">
                <a class="add-button" href="javascript:;" onclick="dt.insertRow();getlastnewid();">添加自定义链接</a>
            </div>
            <div class="item">
                <a class="add-button" href="javascript:;" id="internalbutton">加内部链接</a>
            </div>
            <div class="item">
                <a class="add-button" href="javascript:;" id="forumbutton">添加版块链接</a>
            </div>
        </div>

        <div class="datatablewrap" style="height:300px;">
            <table class="datatable" id="linktable">
                <thead>
                    <tr>
                        <td>排序</td>
                        <td>类型</td>
                        <td>名称</td>
                        <td>地址</td>
                        <td>新窗口打开</td>
                        <td>登陆后可见</td>
                        <td>操作</td>
                    </tr>
                </thead>
                <tbody>
                    <!--[loop $item in $TempNavigations with $i]-->
                    <!--[error line="$i"]-->
                    <tr class="datatable-rowerror" id="error_$i">
                        <td colspan="8"><div class="dialogmsg dialogmsg-error">$Messages</div></td>
                    </tr>
                    <tr class="datatable-rowerrorarrow" id="errorArrow_$i">
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td><!--[if $HasError("name")]--><div class="errorarrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                        <td><!--[if $HasError("url")]--><div class="errorarrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                    <!--[/error]-->
                    <tr id="row_$i">
                        <td>
                            <input type="hidden" name="old_id" value="$i" />
                            <input type="text" class="text" name="old_sortorder_$i" value="$item.SortOrder" style="width:2em;" />
                        </td>
                        <td>
                            <!--[if $item.Type == NavigationType.Internal]-->
                            <input type="hidden" name="old_type_$i" id="old_type_$i" value="1" />
                            <input type="hidden" name="old_urlinfo_$i" id="old_urlinfo_$i" value="$item.urlinfo" />
                            内部链接
                            <!--[else if $item.Type == NavigationType.Custom]-->
                            <input type="hidden" name="old_type_$i" id="old_type_$i" value="0" />
                            自定义链接
                            <!--[else]-->
                            <input type="hidden" name="old_type_$i" id="old_type_$i" value="2" />
                            <input type="hidden" name="old_urlinfo_$i" id="old_urlinfo_$i" value="$item.urlinfo" />
                            板块链接
                            <!--[/if]-->
                        </td>
                        <td><input type="text" class="text" name="old_name_$i" id="old_name_$i" value="$item.name" style="width:10em;" /></td>
                        <td>
                            <!--[if $item.Type == NavigationType.Custom]-->
                            <input type="text" class="text" name="old_url_$i" id="old_url_$i" value="$item.urlinfo" style="width:10em;" />
                            <!--[else if $item.Type == NavigationType.Internal]-->
                            <input type="text" class="text" name="old_url_$i" id="old_url_$i" value="内部地址(key:$item.urlinfo)" style="width:10em;" disabled="disabled" />
                            <!--[else]-->
                            <input type="text" class="text" name="old_url_$i" id="old_url_$i" value="版块地址(forumid:$item.urlinfo)" style="width:10em;" disabled="disabled" />
                            <!--[/if]-->
                        </td>
                        <td style="text-align:center;">
                            <input type="checkbox" name="old_newwindow_$i" $_if($item.NewWindow,'checked="checked"','') />
                        </td>
                        <td style="text-align:center;">
                            <input type="checkbox" name="old_logincansee_$i" $_if($item.OnlyLoginCanSee,'checked="checked"','') />
                        </td>
                        <td><a href="javascript:;" onclick="cancelCheck($i,true);removeElement($('row_$i'));">取消</a></td>
                    </tr>
                    <!--[/loop]-->
                    <tr id="newrow">
                        <td>
                            <input type="hidden" name="newid" value="{0}" />
                            <input type="text" class="text" name="sortorder_{0}" id="sortorder_{0}" value="" style="width:2em;" />
                        </td>
                        <td>
                            <input type="hidden" name="type_{0}" id="type_{0}" value="0" />
                            <span id="ntype_{0}">自定义链接</span>
                        </td>
                        <td><input type="text" class="text" name="name_{0}" id="name_{0}" value="" style="width:10em;" /></td>
                        <td>
                            <input type="hidden" name="urlinfo_{0}" id="urlinfo_{0}" value="" />
                            <input type="text" class="text" name="url_{0}" id="url_{0}" value="" style="width:10em;" />
                        </td>
                        <td style="text-align:center;">
                            <input type="checkbox" name="newwindow_{0}" id="newwindow_{0}" value="1" />
                        </td>
                        <td style="text-align:center;">
                            <input type="checkbox" name="logincansee_{0}" id="logincansee_{0}" value="1" />
                        </td>
                        <td><a href="javascript:;" id="deleteRow{0}" onclick="cancelCheck('{0}',false)">取消</a></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" accesskey="s" name="save" title="保存"><span>保存(<u>S</u>)</span></button>
    <button class="button" type="button" accesskey="c" onclick="panel.close();" title="取消"><span>取消(<u>C</u>)</span></button>
</div>

</form>
<script type="text/javascript">

 var divpop1=addElement("div");
 divpop1.className = "dropdownmenu-wrap navigatorlist";
 divpop1.id="internal_links";
 setVisible(divpop1,0);
 divpop1.innerHTML = '<div class="dropdownmenu">\
                        <div class="dropdownmenu-inner">\
                            <div class="clearfix addnavigator-title">\
                                <h3>请选择要添加内部链接</h3>\
                                <a class="close" href="javascript:;" onclick="setVisible($(\'internal_links\'),0)">关闭</a>\
                            </div>\
                            <div class="scroller">\
                                <ul class="clearfix">\
                                    <!--[loop $key in $InternalLinkKeys]-->\
                                    <li>\
                                        <input type="checkbox" id="internal_$key" value="$key" onclick="addInternalLink(this)" />\
                                        <label id="internalname_$key" for="internal_$key">$GetInternalName($key)</label>\
                                    </li>\
                                    <!--[/loop]-->\
                                </ul>\
                            </div>\
                        </div>\
                    </div>';

 var divpop2=addElement("div");
 divpop2.className = "dropdownmenu-wrap navigatorlist";
 divpop2.id="forum_links";
 setVisible(divpop2,0);
 divpop2.innerHTML = '<div class="dropdownmenu">\
                        <div class="dropdownmenu-inner">\
                            <div class="clearfix addnavigator-title">\
                                <h3>请选择要添加的论坛版块链接</h3>\
                                <a class="close" href="javascript:;" onclick="setVisible($(\'forum_links\'),0)">关闭</a>\
                            </div>\
                            <div class="scroller">\
                                <ul class="clearfix forumlist">\
                                    <!--[loop $forum in $forums with $i]-->\
                                    <li>\
                                        $ForumSeparators[$i]\
                                        <input type="checkbox" id="forum_$forum.forumid" value="$forum.forumid" onclick="addForumLink(this)" />\
                                        <label id="forumname_$forum.forumid" for="forum_$forum.forumid">$forum.name</label>\
                                    </li>\
                                    <!--[/loop]-->\
                                </ul>\
                            </div>\
                        </div>\
                    </div>'

    new popup("internal_links", "internalbutton");
    new popup("forum_links", "forumbutton");

    currentPanel.addCloseHandler(function () {
        removeElement($("internal_links"));
        removeElement($("forum_links"));

    });
    var dt = new DynamicTable("linktable", "newid");
    //<!--[if $TempNavigations.Count==0]-->
    dt.insertRow();
    //<!--[/if]-->
    function getlastnewid() {
        var idObjs = $$('newid');
        if (idObjs.length > 0) {
            var id = parseInt(idObjs[idObjs.length - 1].value);
            //alert(id);
            return id;
        }
        return 0;
    }
    var internalKeys = {};
    function addInternalLink(obj) {
        var key = obj.value;
        if (obj.checked == false) {
            var row = $('newrow-' + internalKeys[key]);
            if (row)
                removeElement(row);
            return;
        }
        var name = $('internalname_' + key).innerHTML;
        dt.insertRow();
        var id = getlastnewid();
        $('type_' + id).value = 1;
        $('ntype_' + id).innerHTML = '内部链接';
        $('name_' + id).value = name; 
        $('urlinfo_' + id).value = key;
        $('url_' + id).disabled = true;
        $('url_' + id).value = '内部地址(key:' + key + ')';
        internalKeys[key] = id;
    }

    var forumIDs = {};
    function addForumLink(obj) {
        var forumID = obj.value;
        if (obj.checked == false) {
            var row = $('newrow-' + forumIDs[forumID]);
            if (row)
                removeElement(row);
            return;
        }
        var name = $('forumname_' + forumID).innerHTML;
        dt.insertRow();
        var id = getlastnewid();
        $('type_' + id).value = 2;
        $('ntype_' + id).innerHTML = '版块链接';
        $('name_' + id).value = name;
        $('urlinfo_' + id).value = forumID;
        $('url_' + id).disabled = true;
        $('url_' + id).value = '版块地址(forumID:' + forumID + ')';
        forumIDs[forumID] = id;
    }
    function cancelCheck(tempid,isold) {
        var id = parseInt(tempid);

        if (isold) {
            var error = $('error_' + id);
            if (error)
                removeElement(error);
            var errorArrow = $('errorArrow_' + id);
            if (errorArrow)
                removeElement(errorArrow);
        }
        
        var type;
        if (isold) {
            type = $('old_type_' + id).value;
        }
        else
            type = $('type_' + id).value;
        if (type == 0)
            return;
        else if (type == 1) {
            var obj;
            if (isold)
                obj = $('internal_' + $('old_urlinfo_' + id).value);
            else
                obj = $('internal_' + $('urlinfo_' + id).value);
            
            if (obj)
                obj.checked = false;
        }
        else if (type == 2) {
            var obj;
            if (isold)
                obj = $('forum_' + $('old_urlinfo_' + id).value);
            else
                obj = $('forum_' + $('urlinfo_' + id).value);
            if (obj)
                obj.checked = false;
        }
    }
</script>
<!--[/place]-->
<!--[/DialogMaster]-->
