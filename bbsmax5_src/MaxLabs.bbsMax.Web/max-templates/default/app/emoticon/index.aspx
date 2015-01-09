<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/app.css" />
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
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/emoticons.gif);">表情管理</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <div class="contentusedchart emoticonusedchart">
                                        <div class="chart-stat">
                                            已用$outputfilesize($UsedSpace)/总共$EmoticonSpaceSize
                                        </div>
                                        <div class="chart-graph">
                                            <div class="chart-used" style="width:{=$SpacePercent}%;"><div>&nbsp;</div></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            
                            <form action="$_form.action" id="form1" method="post">
                            <div class="clearfix workspace app-emoticon">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        
                                        <div class="apptools">
                                            <div class="clearfix apptools-inner">
                                                <a id="toolUploadEmoticons" href="$dialog/emoticon-user-import-batch.aspx?groupid=$currentgroup.groupid" class="btn" onclick="return openDialog(this.href,refresh)" title="上传表情"><span><img src="$Root/max-assets/icon/emoticons_upload.gif" alt="" width="16" height="16" />上传表情</span></a>
                                                <a id="toolUploadEmoticons" href="$dialog/emoticon-user-import.aspx?groupid=$currentgroup.groupid" class="btn" onclick="return openDialog(this.href,refresh)" title="导入表情"><span><img src="$Root/max-assets/icon/emoticons_import.gif" alt="" width="16" height="16" />导入表情</span></a>
                                                <div class="dropdowndock">
                                                <a id="toolExport" class="btn btn-dropdown" href="javascript:void(0);" title="导出/移动"><span><img src="$Root/max-assets/icon/emoticons_export.gif" alt="" width="16" height="16" />导出/移动</span></a>
                                                    <div class="dropdownmenu-wrap" id="export" style="display:none;">
                                                        <div class="dropdownmenu">
                                                            <ul class="dropdownmenu-list">
                                                                <li><a href="javascript:void(clickButton('exportGroup'))">导出分组</a></li>
                                                                <li><a href="javascript:void(clickButton('exportSelected'))">导出表情</a></li>
                                                                <li><a href="javascript:void(moveEmoticon())">移动表情</a></li>
                                                            </ul>
                                                        </div>
                                                    </div>
                                                </div>
                                                <span class="edge">|</span>
                                                <a id="toolUpdateShortcuts" class="btn" href="javascript:void(editShortcut())" title="更新快捷方式"><span><img src="$Root/max-assets/icon/emoticons_edit.gif" alt="" width="16" height="16" />更新快捷方式</span></a>
                                                <span class="edge">|</span>
                                                <a id="toolDelete" class="btn" href="javascript:void(deleteEmote())" title="删除"><span><img src="$Root/max-assets/icon/crossout.gif" alt="" width="16" height="16" />删除</span></a>
                                            </div>
                                        </div>
                                        
                                        <!--[if $EmoticonList.totalrecords>0]-->
                                        <div class="emoticonlist">
                                            <ul class="clearfix emoticon-list">
                                                <!--[loop $Emoticon in $EmoticonList]-->
                                                <li class="emoticonitem" id="li_$emoticon.emoticonid" onclick="selectEmoticon($Emoticon.EmoticonID)">
                                                    <div class="emoticon-entry" id="c_$emoticon.emoticonid" title="快捷方式:$Emoticon.ShortCut">
                                                        <div class="emoticon-thumb" title="快捷方式:$Emoticon.ShortCut">
                                                            <img src="$Emoticon.ImageUrl" onload="imageScale(this, 80, 60)" alt="" />&nbsp;
                                                        </div>
                                                        <div class="emoticon-name">
                                                            <span class="emoticon-checkbox">
                                                                <input type="checkbox" name="emoticonids" id="chk.$Emoticon.EmoticonID" onclick="selectEmoticon($Emoticon.EmoticonID)" value="$Emoticon.EmoticonID" />
                                                            </span>
                                                            <label for="emot_$Emoticon.EmoticonID">$Emoticon.ShortCut</label>
                                                        </div>
                                                    </div>
                                                </li>
                                                <!--[/loop]-->
                                            </ul>
                                        </div>
                                        <!--[pager name="pager1" skin="../../_inc/_pager_app.aspx" count="$EmoticonList.totalrecords" pagesize="$PageSize" ]-->
                                        <!--[else]-->
                                        <div class="nodata">
                                            当前分组暂无任何表情.
                                        </div>
                                        <!--[/if]-->
                                        
                                    </div>
                                </div>
                                <div class="workspace-sidebar">
                                    <div class="workspace-sidebar-inner">
                                        
                                        <div class="panel categorylist emoticongroup">
                                            <div class="panel-head">
                                                <h3 class="panel-title"><span>表情分组</span></h3>
                                            </div>
                                            <div class="panel-body">
                                                <!--[ajaxpanel id="catlist"]-->
                                                <ul class="category-list actioncount-2">
                                                    <!--[loop $group in $groupList]-->
                                                    <li $_if($CurrentGroup.groupid==$group.groupid,'class="current"')>
                                                        <div class="name">
                                                            <a href="$url(app/emoticon/index)?groupid=$Group.GroupID" >
                                                                $Group.GroupName
                                                                <em class="counts">($group.totalEmoticons)</em>
                                                            </a>
                                                        </div>
                                                        <div class="entry-action">
                                                            <a class="action-rename" title="重命名分组" href="$dialog/emoticon-user-renamegroup.aspx?groupid=$group.groupid" onclick="return openDialog(this.href,refresh)">重命名分组</a>
                                                            <a class="action-delete" title="删除分组" href="$dialog/emoticon-user-deletegroup.aspx?groupid=$group.groupid" onclick="return openDialog(this.href,refresh)">当前分组</a>
                                                        </div>
                                                        <div class="clear">&nbsp;</div>
                                                    </li>
                                                    <!--[/loop]-->
                                                </ul>
                                                <!--[/ajaxpanel]-->
                                                
                                                <div class="addcategory addemoticongroup" style="display:none" id="addcat-box">
                                                    <div class="formgroup appform">
                                                        <div class="formrow">
                                                            <div class="form-enter">
                                                                <input name="groupname" type="text" class="text" value="分组名称..." id="groupname" onfocus="this.value='';this.style.color='#000';" />
                                                            </div>
                                                        </div>
                                                        <div class="formrow formrow-action">
                                                            <span class="minbtn-wrap"><span class="btn"><input class="button" type="button" value="确认" onclick="ajaxSubmit('form1','addcategory','catlist',function(){$('groupname').value = ''; $('addcat-btn').style.display=''; $('addcat-box').style.display='none';},null,true); " /></span></span>
                                                            <a href="#" onclick="$('addcat-btn').style.display=''; $('addcat-box').style.display='none'; return false;">取消</a>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="addcategory-button" id="addcat-btn">
                                                    <a href="#" onclick="$('addcat-btn').style.display='none'; $('addcat-box').style.display=''; return false;">创建新分组</a>
                                                </div>
                                                
                                            </div>
                                        </div>
                                        
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
<script type="text/javascript">
var drop;
drop=new dropdown('export','toolExport',true);

var chkList=new checkboxList('emoticonids');

function deleteEmote()
{ clickButton("delete");
}

function deleteGroup()
{
   clickButton("deletegroup");
}

function moveEmoticon()
{
    if(chkList.selectCount&&chkList.selectCount()>0)
    {
        postToDialog({ formId: 'form1', url: '$dialog/emoticon-user-move.aspx?groupid=$currentgroup.groupid', callback: refresh });
    }
    else
    {
        showAlert("未选择任何表情");
    }
}

function editShortcut()
{
    if(chkList.selectCount&&chkList.selectCount()>0)
    {
        postToDialog({ formId: 'form1', url: '$dialog/emoticon-user-editshortcut.aspx?groupid=$currentgroup.groupid', callback: refresh });
    }
    else
    {
        showAlert("未选择任何表情");
    }
}

function selectEmoticon(id)
{
    var chk= $('chk.'+id)
    chk.checked=!chk.checked;
    $('li_' + id).className = chk.checked ? 'emoticonitem selected' : 'emoticonitem';
}

</script>
</div>
</body>
</html>
