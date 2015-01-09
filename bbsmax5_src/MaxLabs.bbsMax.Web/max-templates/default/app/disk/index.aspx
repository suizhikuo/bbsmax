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
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/folder_web.gif);">网络硬盘</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagebutton">
                                        <li><a class="uploadfile" href="$dialog/disk-upload.aspx?directoryid=$currentdirectory.directoryid" onclick="return openDialog(this.href,function(){location.replace('$url(app/disk/index)?directoryID=$currentdirectory.directoryid&OrderBy=CreateDate&desc=true&ViewMode=$ViewMode')})">上传文件</a></li>
                                    </ul>
                                    <div class="contentusedchart diskusedchart">
                                        <div class="chart-stat">
                                            已用$TotalUseSizesFormated/总共$TotalSizesFormated
                                        </div>
                                        <div class="chart-graph">
                                            <div class="chart-used" style="width:{=$TotalRemnantPercent}%;"><div>&nbsp;</div></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="clearfix workspace app-disk">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">

                                        <div class="apptools">
                                            <div class="clearfix apptools-inner">
                                                <a id="toolback" class="btn" href="$url(app/disk/index)?directoryID=$CurrentDirectory.ParentID&ViewMode=$ViewMode"><span><img src="$Root/max-assets/icon/folder_up.gif" alt="" width="16" height="16" />返回上级</span></a>
                                                <span class="edge">|</span>
                                                <a id="toolNewDir" class="btn" href="$dialog/disk-createdirectory.aspx?directoryid=$CurrentDirectory.Directoryid" onclick="return openDialog(this.href,function(r){location.replace(String.format('$url(app/disk/index)?directoryID={0}',r))})"><span><img src="$Root/max-assets/icon/folder_add.gif" alt="" width="16" height="16" />新建目录</span></a>
                                                <a id="toolReNameDiskDirectory" class="btn" href="$dialog/disk-rename.aspx?directoryid=$currentdirectory.directoryid" onclick="return postToDialog({formId:'form1',url:'$dialog/disk-rename.aspx?directoryid=$currentdirectory.directoryid',callback:refresh})"><span><img src="$Root/max-assets/icon/folder_edit.gif" alt="" width="16" height="16" />重命名</span></a>
                                                <a id="toolMove" class="btn" href="javascript:void(moveFiles());"><span><img src="$Root/max-assets/icon/file_export.gif" alt="" width="16" height="16" />移动</span></a>
                                                <a id="toolUploadFile" class="btn" href="$dialog/disk-upload.aspx?directoryid=$currentdirectory.directoryid" onclick="return openDialog(this.href,function(){location.replace('$url(app/disk/index)?directoryID=$currentdirectory.directoryid&OrderBy=CreateDate&desc=true&ViewMode=$ViewMode')})"><span><img src="$Root/max-assets/icon/file_up.gif" alt="" width="16" height="16" />上传</span></a>
                                                <span class="edge">|</span>
                                                <div class="dropdowndock">
                                                <a id="tool_select" class="btn btn-dropdown" href="javascript:void(0);" title="选择"><span><img src="$Root/max-assets/icon/select.gif" alt="" width="16" height="16" />选择</span></a>
                                                    <div class="dropdownmenu-wrap" id="drop_select" style="display:none;">
                                                        <div class="dropdownmenu">
                                                            <ul class="dropdownmenu-list">
                                                                <li><a href="javascript:void(selectAll())">全选</a></li>
                                                                <li><a href="javascript:void(selectReverse())">反选</a></li>
                                                            </ul>
                                                        </div>
                                                    </div>
                                                </div>
                                                <span class="edge">|</span>
                                                <a id="toolDelete" class="btn" href="javascript:void(deleteFiles());"><span><img src="$Root/max-assets/icon/crossout.gif" alt="" width="16" height="16" />删除</span></a>
                                                <div style="float:right;">
                                                <!--[if $ViewMode == FileViewMode.Thumbnail]-->
                                                <a class="btn" $_if($ViewMode== FileViewMode.List,'class="checked"') id="toolListView" href="$AttachQueryString('ViewMode=List')" title="切换到列表视图"><span><img src="$Root/max-assets/icon/view_thumb.gif" alt="" width="16" height="16" /></span></a>
                                                <!--[else]-->
                                                <a class="btn" $_if($ViewMode== FileViewMode.Thumbnail,'class="checked"') id="toolTnView" href="$AttachQueryString('ViewMode=Thumbnail')" title="切换到缩略图视图"><span><img src="$Root/max-assets/icon/view_list.gif" alt="" width="16" height="16" /></span></a>
                                                <!--[/if]-->
                                                <div class="dropdowndock">
                                                <a id="tool_sort" class="btn btn-dropdown" href="javascript:void(0);" title="文件排列方式"><span><img src="$Root/max-assets/icon/order_asc.gif" alt="" width="16" height="16" />排列</span></a>
                                                    <div class="dropdownmenu-wrap" id="drop_sort" style="display:none;">
                                                        <div class="dropdownmenu">
                                                            <ul class="dropdownmenu-list">
                                                                <li><a $_if($OrderBy==FileOrderBy.Name,'class="checked"') href="$AttachQueryString('OrderBy=Name')">名称</a></li>
                                                                <li><a $_if($OrderBy==FileOrderBy.Size,'class="checked"') href="$AttachQueryString('OrderBy=Size')">大小</a></li>
                                                                <li><a $_if($OrderBy==FileOrderBy.CreateDate,'class="checked"') href="$AttachQueryString('OrderBy=CreateDate')">创建时间</a></li>
                                                                <li><a $_if($OrderBy==FileOrderBy.Type,'class="checked"') href="$AttachQueryString('OrderBy=Type')">类型</a></li>
                                                                <li class="dropmenu-split">-</li>
                                                                <li><a $_if(!$isdesc,'class="checked"') href="$AttachQueryString('desc=false')">顺序</a></li>
                                                                <li><a $_if($isdesc,'class="checked"') href="$AttachQueryString('desc=true')">倒序</a></li>
                                                            </ul>
                                                        </div>
                                                    </div>
                                                </div>
                                                </div>
                                            </div>
                                        </div>
                                        
                                        <div class="clearfix diskwrapper">
                                            <div class="diskfile">
                                                <div class="diskfile-inner">
                                                   <form id="form1" action="$_form.action" method="post">
                                                    <!--[if $AllFiles.count > 0]-->
                                                    <!--[if $ViewMode == FileViewMode.Thumbnail]-->
                                                    <div class="diskfile-files diskfile-thumbview">
                                                        <ul class="clearfix diskfile-list">
                                                        <!--[loop $DiskFile in $PagedFiles with $i]-->
                                                            <!--[if !$DiskFile.isFile]-->
                                                            <li class="fileitem" id="li_diskDirectoryID_$DiskFile.ID">
                                                                <a href="$url(app/disk/index)?directoryID=$DiskFile.ID&ViewMode=$ViewMode" title="$DiskFile.Name">
                                                                    <!--[if $IsIE && $BrowserMajorVersion < 7]-->
                                                                    <span class="file-icon" style="filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='$skin/images/icons/diskicon_folder.png');">&nbsp;</span>
                                                                    <!--[else]-->
                                                                    <span class="file-icon" style="background-image:url($skin/images/icons/diskicon_folder.png);">&nbsp;</span>
                                                                    <!--[/if]-->
                                                                </a>
                                                                <label for="diskDirectoryID_$DiskFile.ID" class="file-name" title="$DiskFile.Name">
                                                                    <span class="file-checkbox">
                                                                        <input id="diskDirectoryID_$DiskFile.ID" name="diskDirectoryID" type="checkbox" value="$DiskFile.ID" class="checkbox" />
                                                                    </span>
                                                                    $DiskFile.Name
                                                                </label>
                                                            </li>
                                                            <!--[else]-->
                                                            <li class="fileitem" id="li_diskFileID_$i">
                                                                <a target="_blank" href="$url(handler/down)?action=disk&fileid=$DiskFile.ID" title="$DiskFile.Name &#13;创建时间:$outputdatetime($DiskFile.CreateDate)&#13;大小:$outputFileSize($DiskFile.Size)">
                                                                    <span class="file-icon" style="background-image:url($DiskFile.bigIcon);">&nbsp;</span>
                                                                </a>
                                                                <label for="diskFileID_$i" class="file-name" title="$DiskFile.Name">
                                                                    <span class="file-checkbox">
                                                                        <input name="diskFileID" id="diskFileID_$i" type="checkbox" value="$DiskFile.ID" class="checkbox" />
                                                                    </span>
                                                                    $DiskFile.Name
                                                                </label>
                                                            </li>
                                                            <!--[/if]-->
                                                            <!--[/loop]-->
                                                        </ul>
                                                    </div>
                                                    <!--[else]-->
                                                    <div class="diskfile-files diskfile-listview">
                                                        <ul class="cleafix diskfile-list">
                                                            <li class="fileitem fileitem-caption">
                                                                <div class="file-name"><p>文件名</p></div>
                                                                <div class="file-size"><p>大小</p></div>
                                                                <div class="file-date"><p>时间</p></div>
                                                            </li>
                                                        <!--[loop $DiskFile in $PagedFiles with $i]-->
                                                            <!--[if !$DiskFile.isfile]-->
                                                            <li class="fileitem" id="li_diskDirectoryID_$DiskFile.ID">
                                                                <div class="file-name">
                                                                    <p>
                                                                        <input name="diskDirectoryID" id="diskDirectoryID_$DiskFile.ID" type="checkbox" value="$DiskFile.ID" class="checkbox" />
                                                                        <img src="$Root/max-templates/default/images/icons/folder.gif" width="16" height="16" alt="" />
                                                                        <a href="$url(app/disk/index)?directoryID=$DiskFile.ID&ViewMode=$ViewMode">$DiskFile.Name</a>
                                                                    </p>
                                                                </div>
                                                                <div class="file-size"><p>-</p></div>
                                                                <div class="file-date"><p>$outputdatetime($DiskFile.CreateDate)</p></div>
                                                            </li>
                                                            <!--[else]-->
                                                            <li class="fileitem" id="li_diskFileID_$i">
                                                                <div class="file-name">
                                                                    <p>
                                                                        <input name="diskFileID" id="diskFileID_$i" type="checkbox" value="$DiskFile.ID" class="checkbox" />
                                                                        <!--[if $IsIE && $BrowserMajorVersion < 7 ]-->
                                                                        <b class="fix-iepng" style="filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src=$DiskFile.smallIcon)"><img src="$DiskFile.smallIcon" alt="" /></b>
                                                                        <!--[else]-->
                                                                        <img src="$DiskFile.smallIcon" alt="" />
                                                                        <!--[/if]-->
                                                                        <a target="_blank" href="$url(handler/down)?action=disk&fileid=$DiskFile.ID" title="$DiskFile.FileName">$DiskFile.Name</a>
                                                                    </p>
                                                                </div>
                                                                <div class="file-size"><p>$outputFilesize($DiskFile.Size)</p></div>
                                                                <div class="file-date"><p>$outputdatetime($DiskFile.CreateDate)</p></div>
                                                            </li>
                                                            <!--[/if]-->
                                                        <!--[/loop]-->
                                                        </ul>
                                                    </div>
                                                    <!--[/if]-->
                                                    <!--[pager name="pager1" skin="../../_inc/_pager_app.aspx" count="$AllFiles.count" pagesize="$PageSize" ]-->
                                                    
                                                    <!--[else]-->
                                                    <div class="nodata">
                                                        当前文件夹暂无任何文件.
                                                    </div>
                                                    <!--[/if]-->
                                                    </form>
                                                </div>
                                            </div>
                                            <div class="diskfloder">
                                                <div class="diskfolder-inner">
                                                    <ul class="diskfloder-menutree">
                                                        $GetDiskMenu("<em></em>",@"<li><a class=""{0}"" href=""{1}"">{2}</a></li>","","","current","",@"<span>{4}</span>","<span>根目录</span>")
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>
                                        
                                    </div>
                                </div>
                                <div class="workspace-sidebar">
                                    <div class="workspace-sidebar-inner">
                                        &nbsp;
                                    </div>
                                </div>
                            </div>
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
function deleteFiles()
{
    if(fileList.selectCount()==0)//&&dirList.selectCount()==0)
    {
        showAlert("没有选择任何文件或文件夹")
    }
    else
    {
        if(confirm("确定要删除这些文件或文件夹吗"))
        {
            clickButton("delete","form1");
        }
    }
}

function moveFiles()
{
    postToDialog({ formId: "form1", url: "$dialog/disk-move.aspx?directoryid=$currentdirectory.directoryid", callback: refresh });
}

var fileList = new checkboxList(['diskDirectoryID','diskFileID'],'selectall');
fileList.SetItemChangeHandler( function(e)
{
    $('li_' + e.id).className = e.checked ? 'fileitem selected' : 'fileitem';
}
 );
 
function selectAll()
{
    fileList.selectAll();
}

function selectReverse()
{
    fileList.reverse();
}

new dropdown('drop_sort','tool_sort',true);
new dropdown('drop_select','tool_select',true);
</script>
</div>
</body>
</html>
