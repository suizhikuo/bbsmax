<!--[DialogMaster title="主题分类($Forum.ForumName)" width="500" ]-->
<!--[place id="body"]-->
<form id="form" method="post" action="$_form.action">
<!--[success]-->
<div class="dialogmsg dialogmsg-success">操作成功</div>
<!--[/success]-->
<!--[include src="_error_.ascx" /]-->

<div class="clearfix dialogbody">
    <div class="dialogform">
        <div class="formrow">
            <h3 class="label">启用主题分类</h3>
            <div class="form-enter">
                <input id="threadCateMustUsed" type="radio" name="threadCategorySet" value="EnableAndMust" $_form.checked("threadCategorySet","EnableAndMust","$out($forum.ThreadCatalogStatus)") />
                <label for="threadCateMustUsed">启用(用户必选)</label> 
                <input id="threadCateUsed" type="radio" name="threadCategorySet" value="Enable" $_form.checked("threadCategorySet","Enable","$out($forum.ThreadCatalogStatus)") />
                <label for="threadCateUsed">启用(用户可选)</label> 
                <input id="threadCateDonotUsed" type="radio" name="threadCategorySet" value="DisEnable" $_form.checked("threadCategorySet","DisEnable","$out($forum.ThreadCatalogStatus)") />
                <label for="threadCateDonotUsed">禁用</label>
            </div>
        </div>
        <div class="formrow" id="div_catagory">
            <div class="form-enter">
                <div class="datatablewrap" id="divlist" style="height:200px;">
                <table class="datatable" id="table1">
                <thead>
                <tr>
                    <th>排序</th>
                    <th>分类名称(支持HTML)</th>
                    <th>操作</th>
                </tr>
                </thead>
                <tbody>
                <!--[loop $forumThreadCatalog in $ForumThreadCatalogList with $i]-->
                <!--[error line="$i"]-->
                <tr class="datatable-rowerror" id="error$i">
                    <td colspan="3"><div class="dialogmsg dialogmsg-error">$Messages</div></td>
                </tr>
                <tr class="datatable-rowerrorarrow" id="errorarray$i">
                    <td><!--[if $HasError("sortorder")]--><div class="errorarrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                    <td><!--[if $HasError("threadcatalogname")]--><div class="errorarrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                    <td>&nbsp;</td>
                </tr>
                <!--[/error]-->
                <!--[if $forumThreadCatalog.IsNew]-->
                <tr id="row-$i">
                    <td>
                        <input type="text" name="new_sortOrder_$i" onkeyup="value=value.replace(/[^\d]/g,'')" class="text" style="width:2em;"  value="$forumThreadCatalog.SortOrder" />
                        <input type="hidden" name="newcatagories" value="$i" />
                    </td>
                    <td>
                        <select name="new_threadCatagories_$i" onchange="setThreadCatalogName('new_threadCatagories_$i','new_threadcatalogname_$i')">
                        <option value="">新建</option>
                        <!--[loop $tempThreadCatalog in $ThreadCatalogList]-->
                        <option value="$tempThreadCatalog.ThreadCatalogID" $_form.selected("new_threadCatagories_$i","$tempThreadCatalog.ThreadCatalogID")>$GetName($tempThreadCatalog.ThreadCatalogName)</option>
                        <!--[/loop]-->
                        </select>
                        <input type="text" class="text" name="new_threadcatalogname_$i" value="$forumThreadCatalog.ThreadCatalog.ThreadCatalogName" />
                    </td>
                    <td>
                        <a href="javascript:void(cancelNewrow('$i'))">取消</a>
                    </td>
                </tr>
                <!--[else]-->
                <tr>
                    <td>
                        <input type="text" class="text" name="sortorder_$i"  onkeyup="value=value.replace(/[^\d]/g,'')" style="width:2em;" value="$_form.text("sortorder_$i","$forumThreadCatalog.SortOrder")" />
                        <input type="hidden" name="catagories" value="$i" />
                    </td>
                    <td>
                        <select name="threadCatagories_$i" onchange="setThreadCatalogName('threadCatagories_$i','threadCatalogName_$i')">
                        <option value="">新建</option>
                        <!--[loop $tempThreadCatalog in $ThreadCatalogList]-->
                        <option value="$tempThreadCatalog.ThreadCatalogID" $_form.selected("threadCatagories_$i","$tempThreadCatalog.ThreadCatalogID","$forumThreadCatalog.ThreadCatalogID")>$GetName($tempThreadCatalog.ThreadCatalogName)</option>
                        <!--[/loop]-->
                        </select>
                        <input type="text" class="text" name="threadCatalogName_$i" value="$_form.text("threadCatalogName_$i","$forumThreadCatalog.ThreadCatalog.ThreadCatalogName")" />
                    </td>
                    <td>
                        <a href="$dialog/threadcategories.aspx?action=delete&forumID=$Forum.ForumID&threadCatalogID=$forumThreadCatalog.ThreadCatalogID&isdialog=1">删除</a>
                    </td>
                </tr>
                <!--[/if]-->
                <!--[/loop]-->
                <tr id="newrow">
                    <td>
                        <input type="text" name="new_sortOrder_{0}" onkeyup="value=value.replace(/[^\d]/g,'')" class="text" style="width:2em;"  value="" />
                        <input type="hidden" name="newcatagories" value="{0}" />
                    </td>
                    <td>
                        <select name="new_threadCatagories_{0}" onchange="setThreadCatalogName('new_threadCatagories_{0}','new_threadcatalogname_{0}')">
                        <option value="">新建</option>
                        <!--[loop $tempThreadCatalog in $ThreadCatalogList]-->
                        <option value="$tempThreadCatalog.ThreadCatalogID">$tempThreadCatalog.ThreadCatalogName</option>
                        <!--[/loop]-->
                        </select>
                        <input type="text" class="text" name="new_threadcatalogname_{0}" value="" />
                    </td>
                    <td>
                        <a href="javascript:;" id="deleteRow{0}">取消</a>
                    </td>
                </tr>
                </tbody>
                </table>
                </div>
            </div>
            <div class="form-note" id="div_note">排序数字越小, 表示主题分类排序越靠前.</div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="savecatagories" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
    <button class="button" id="addnew" accesskey="a" title="添加分类" onclick="dt.insertRow();$('divlist').scrollTop=65535;" type="button" ><span>添加分类(<u>A</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();" type="button" ><span>取消(<u>C</u>)</span></button>
</div>
</form>

<script type="text/javascript">
    var created = $_if($IsChanged, "true", "false");
var catalogNames={};
//<!--[loop $tempThreadCatalog in $ThreadCatalogList]-->
catalogNames['$tempThreadCatalog.ThreadCatalogID']='$ToJsString($tempThreadCatalog.ThreadCatalogName)';
//<!--[/loop]-->

var dt=new  DynamicTable("table1","newcatagories");

function setThreadCatalogName(selectName,textName) {

    var selectObj = document.getElementsByName(selectName)[0];
    
    for(var i = 0;i<selectObj.options.length;i++)
    {
        if(selectObj.options[i].selected)
        {
            if(selectObj.options[i].value == '')
                document.getElementsByName(textName)[0].value = '';
            else
            {
                document.getElementsByName(textName)[0].value = catalogNames[selectObj.options[i].value];
            }
        }
    }
}


result = true;

initDisplay('threadCategorySet',[
 { value: 'EnableAndMust', display: true, id: 'div_catagory' }
, { value: 'Enable', display: true, id: 'div_catagory' }
, { value: 'DisEnable', display: false, id: 'div_catagory' }

, { value: 'EnableAndMust', display: true, id: 'div_note' }
, { value: 'Enable', display: true, id: 'div_note' }
, { value: 'DisEnable', display: false, id: 'div_note' }

,{value:'EnableAndMust',display:true, id:'addnew'}
,{value:'Enable',display:true, id:'addnew'}
,{value:'DisEnable',display:false, id:'addnew'}
])
</script>
<!--[/place]-->
<!--[/dialogmaster]-->