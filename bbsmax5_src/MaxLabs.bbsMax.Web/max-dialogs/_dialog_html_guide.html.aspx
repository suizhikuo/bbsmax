<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>对话框</title>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<link rel="stylesheet" type="text/css" href="$Root/max-templates/default/styles/common.css" />
<link rel="stylesheet" type="text/css" href="$Root/max-templates/default/styles/dialog.css" />
</head>
<body>

<div class="dialog" style="position:absolute;top:10px;left:10px;"><!--对话框位置 -->
    <div class="dialog-inner">
    <div class="dialogcontent" style="width:500px;"><!--对话框宽度 -->
    
        <div class="clearfix dialoghead">
            <h1 class="dialogtitle">Dialig Title</h1>
            
            <div class="dialogstatus">共有2张图片</div>
            
            <div class="dialogclose"><a href="#" accesskey="Q" title="关闭">关闭(<u>Q</u>)</a></div>
        </div>
        
        <div class="dialogmsg dialogmsg-error">Error</div>
        <div class="dialogmsg dialogmsg-alert">Alert</div>
        <div class="dialogmsg dialogmsg-success">Success</div>
        <div class="dialogmsg dialogmsg-protect">Protect</div>
        
        <div class="clearfix dialogtabwrap">
            <div class="dialogtab">
                <ul>
                    <li><a class="current" href="#"><span>Tab 1</span></a></li>
                    <li><a href="#"><span>Tab 2</span></a></li>
                </ul>
            </div>
            
            <div class="dialogadditem">
                <a href="#"><span>添加</span></a>
            </div>
            <!---->
            <div class="dialogsearch">
                <div class="dialogsearch-inner">
                    <input type="text" value="Search" />
                </div>
            </div>
        </div>
        
        <div class="clearfix dialogsubheadwrap">
            <div class="dialogback">
                <a href="#"><span>返回</span></a>
            </div>
            <h3 class="dialogsubhead">添加图标</h3>
        </div>
        <!---->
        <div class="clearfix dialogbody">

            <div class="dialogform dialogform-horizontal">
                <div class="formrow">
                    <h3 class="label"><label for="">文本</label> <em class="requisite" title="必填项">*</em></h3>
                    <div class="form-enter">
                        <input class="text" type="text" />
                        <span class="form-tip tip-error">Error Error Error Error Error Error Error</span>
                    </div>
                    <div class="form-note">表单提示</div>
                </div>
                <div class="formrow">
                    <h3 class="label"><label for="">图片</label></h3>
                    <div class="form-enter">
                        <span class="imagepicker">
                            <input class="text" type="text" value="$root/max-assets/icon" />
                            <a href="#">图片</a>
                        </span>
                        <span class="form-tip tip-alert">Alert Alert Alert Alert Alert Alert Alert</span>
                    </div>
                </div>
                <div class="formrow">
                    <h3 class="label"><label for="">颜色</label></h3>
                    <div class="form-enter">
                        <span class="colorpicker">
                            <input class="text" type="text" value="#FF8800" />
                            <a href="#" style="background-color:#ff8800;">颜色</a>
                        </span>
                        <span class="form-tip tip-success">Success Success Success Success Success Success Success</span>
                    </div>
                </div>
                <div class="formrow">
                    <h3 class="label"><label for="">日期</label></h3>
                    <div class="form-enter">
                        <span class="datepicker">
                            <input class="text" type="text" value="2010-04-23" />
                            <a href="#">日期</a>
                        </span>
                    </div>
                </div>
                <div class="formrow">
                    <h3 class="label"><label for="">日期</label></h3>
                    <div class="form-enter">
                        <span class="datepicker">
                            <input class="text" type="text" value="2010-04-23" />
                            <a href="#">日期</a>
                        </span> -
                        <span class="datepicker">
                            <input class="text" type="text" value="2010-04-23" />
                            <a href="#">日期</a>
                        </span>
                    </div>
                    <div class="form-enter">
                        <input type="checkbox" checked="checked />
                        <label for="">通知作者</label>
                    </div>
                </div>
            </div>
            
            <div class="dialogform">
                <div class="formrow">
                    <h3 class="label"><label for="">文本</label> <em class="requisite" title="必填项">*</em></h3>
                    <div class="form-enter">
                        <input class="text" type="text" />
                    </div>
                    <div class="form-enter">
                        <input type="checkbox" checked="checked />
                        <label for="">通知作者</label>
                    </div>
                </div>
                <div class="formrow">
                    <h3 class="label"><label for="">文本</label> <em class="requisite" title="必填项">*</em></h3>
                    <div class="form-enter">
                        <textarea cols="30" rows="5"></textarea>
                    </div>
                </div>
            </div>
            
            <div class="scroller" style="height:100px;"> <!-- 滚动框 默认不指定为200px -->
            </div>
            
            <div class="datatablewrap">
            <table class="datatable">
                <thead>
                    <tr>
                        <th>1</th>
                        <th>1</th>
                        <th>1</th>
                        <th>1</th>
                    </tr>
                </thead>
                <tbody>
                    <tr class="odd">
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                    </tr>
                    <tr class="even">
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                    </tr>
                    <tr class="odd">
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                    </tr>
                    <tr class="even">
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                    </tr><tr class="odd">
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                    </tr>
                    <tr class="even">
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                    </tr><tr class="odd">
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                    </tr>
                    <tr class="even">
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                    </tr><tr class="odd">
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                    </tr>
                </tbody>
            </table>
            </div>
            
            <div class="datatablewrap">
            <table class="datatable">
                <thead>
                    <tr>
                        <th>1</th>
                        <th>1</th>
                        <th>1</th>
                        <th>1</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                    </tr>
                    <tr class="datatable-rowerror">
                        <td colspan="4"><div class="dialogmsg dialogmsg-error">1</div></td>
                    </tr>
                    <tr class="datatable-rowerrorarrow">
                        <td><div class="errorarrow">1</div></td>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                    </tr>
                    <tr>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                        <td>1</td>
                    </tr>
                </tbody>
            </table>
            </div>

            <div class="clearfix dialogfluidform">
                <div class="formrow">
                    <h3 class="label"><label for="">指定要屏蔽的用户</label></h3>
                    <div class="form-enter">
                        <input class="text" type="text" />
                    </div>
                    <div class="form-note">输入用户名</div>
                </div>
                <div class="formrow">
                    <h3 class="label"><label for="">解除屏蔽日期</label></h3>
                    <div class="form-enter">
                        <span class="datepicker">
                            <input class="text" type="text" value="2010-04-23" />
                            <a href="#">日期</a>
                        </span>
                    </div>
                </div>
                <div class="formrow formrow-action">
                    <button class="button" type="button"><span>添加</span></button>
                </div>
            </div>
            
            <!-- 文件/附件 -->
            <div class="filelistview">
                <div class="filelistview-head">
                    <table class="filelist-table">
                        <tr>
                            <td class="filename">文件</td>
                            <td class="filesize">大小</td>
                            <td class="fileaddtime">时间</td>
                            <td class="fileaction">操作</td>
                        </tr>
                    </table>
                </div>
                <div class="filelist-nodata">
                    <p>当前目录没有TypeName文件.</p>
                </div>
            </div>
            
            <div class="filelistview">
                <div class="filelistview-head">
                    <table class="filelist-table">
                        <tr>
                            <td class="filename">文件</td>
                            <td class="filesize">大小</td>
                            <td class="fileaddtime">时间</td>
                            <td class="fileaction">操作</td>
                        </tr>
                    </table>
                </div>
                <div class="filelistview-wrap">
                    <table class="filelist-table">
                        <tr>
                            <td class="filename">
                                <div class="filename-wrap">
                                    <span style="padding-left:20px;background:url($Root/max-assets/icon/folder.gif) no-repeat 0 50%;"><a href="#">DiskFile.Name</a></span>
                                </div>
                            </td>
                            <td class="filesize">&nbsp;</td>
                            <td class="fileaddtime">04-20</td>
                            <td class="fileaction">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="filename">
                                <div class="filename-wrap">
                                    <span style="padding-left:20px;background:url($Root/max-assets/icon/folder.gif) no-repeat 0 50%;"><a href="#">DiskFile.Name</a></span>
                                </div>
                            </td>
                            <td class="filesize">&nbsp;</td>
                            <td class="fileaddtime">04-20</td>
                            <td class="fileaction">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="filename">
                                <div class="filename-wrap">
                                    <span style="padding-left:20px;background:url($Root/max-assets/icon-file/jpg.gif) no-repeat 0 50%;">DiskFile.Name.jpg</span>
                                </div>
                            </td>
                            <td class="filesize">100 kb</td>
                            <td class="fileaddtime">04-20</td>
                            <td class="fileaction">
                                <a href="#">插入图片</a>
                                <a href="#">插入链接</a>
                                <!--
                                <span class="tip">大小超出限制.</span>
                                -->
                            </td>
                        </tr>
                        <tr>
                            <td class="filename">
                                <div class="filename-wrap">
                                    <span style="padding-left:20px;background:url($Root/max-assets/icon-file/jpg.gif) no-repeat 0 50%;">DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDiskFile.Name.jpg</span>
                                </div>
                            </td>
                            <td class="filesize">100 kb</td>
                            <td class="fileaddtime">04-20</td>
                            <td class="fileaction">
                                <a href="#">插入图片</a>
                                <a href="#">插入链接</a>
                                <!--
                                <span class="tip">大小超出限制.</span>
                                -->
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            
            <!-- 相册/图片浏览 -->
            <div class="filethumbview">
                <ul class="clearfix filethumb-list">
                    <li class="thumbitem">
                        <a class="thumb-entry selected">
                            <span class="thumb">
                                <img src="Photo.ThumbSrc" alt="" onclick="" />&nbsp;
                            </span>
                            <span class="title">
                                Photo.Name
                            </span>
                        </a>
                    </li>
                    <li class="thumbitem">
                        <a class="thumb-entry">
                            <span class="thumb">
                                <img src="Photo.ThumbSrc" alt="" onclick="" />&nbsp;
                            </span>
                            <span class="title">
                                Photo.Name
                            </span>
                        </a>
                    </li>
                    <li class="thumbitem">
                        <a class="thumb-entry">
                            <span class="thumb">
                                <img src="Photo.ThumbSrc" alt="" onclick="" />&nbsp;
                            </span>
                            <span class="title">
                                Photo.Name
                            </span>
                        </a>
                    </li>
                    <li class="thumbitem">
                        <a class="thumb-entry">
                            <span class="thumb">
                                <img src="Photo.ThumbSrc" alt="" onclick="" />&nbsp;
                            </span>
                            <span class="title">
                                Photo.Name
                            </span>
                        </a>
                    </li>
                    <li class="thumbitem">
                        <a class="thumb-entry">
                            <span class="thumb">
                                <img src="Photo.ThumbSrc" alt="" onclick="" />&nbsp;
                            </span>
                            <span class="title">
                                Photo.Name
                            </span>
                        </a>
                    </li>
                    <li class="thumbitem">
                        <a class="thumb-entry">
                            <span class="thumb">
                                <img src="Photo.ThumbSrc" alt="" onclick="" />&nbsp;
                            </span>
                            <span class="title">
                                Photo.Name
                            </span>
                        </a>
                    </li>
                </ul>
            </div>
            
            <!-- 从相册插入/图片浏览 -->
            <div class="clearfix albuminsert-layout">
                    <table class="filelist-table">
                        <tr>
                            <td class="album">
                                <div class="albuminsert-dir">
                                    <div class="albuminsert-dir-inner">
                                        <ul>
                                            <li><a href="#">Album.Name</a></li>
                                        </ul>
                                    </div>
                                </div>
                            </td>
                            <td class="photo">
                                <div class="filethumbview">
                                    <ul class="clearfix filethumb-list">
                                        <li class="thumbitem">
                                            <a class="thumb-entry">
                                                <span class="thumb">
                                                    <img src="Photo.ThumbSrc" alt="" onclick="" />&nbsp;
                                                </span>
                                                <span class="title">
                                                    Photo.Name
                                                </span>
                                            </a>
                                        </li>
                                        <li class="thumbitem">
                                            <a class="thumb-entry">
                                                <span class="thumb">
                                                    <img src="Photo.ThumbSrc" alt="" onclick="" />&nbsp;
                                                </span>
                                                <span class="title">
                                                    Photo.Name
                                                </span>
                                            </a>
                                        </li>
                                        <li class="thumbitem">
                                            <a class="thumb-entry">
                                                <span class="thumb">
                                                    <img src="Photo.ThumbSrc" alt="" onclick="" />&nbsp;
                                                </span>
                                                <span class="title">
                                                    Photo.Name
                                                </span>
                                            </a>
                                        </li>
                                        <li class="thumbitem">
                                            <a class="thumb-entry">
                                                <span class="thumb">
                                                    <img src="Photo.ThumbSrc" alt="" onclick="" />&nbsp;
                                                </span>
                                                <span class="title">
                                                    Photo.Name
                                                </span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                            </td>
                        </tr>
                    </table>
            </div>
            
            <div class="clearfix pagination">
                <div class="pagination-inner">
                    <a href="#">1</a>
                    <a class="current" href="#">2</a>
                    <a href="#">3</a>
                    <a href="#">9</a>
                    <a href="#">...10</a>
                </div>
            </div>
            
        </div>
        <div class="clearfix dialogfoot">
            <button class="button button-highlight" type="button" accesskey="d" title="确认"><span>删除 9 (<u>D</u>)</span></button>
            <button class="button" type="submit" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
            <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
        </div>
    </div>
    </div>
</div>

<div class="dialog" style="position:absolute;top:600px;left:550px;">
    <div class="dialog-inner">
        <div class="dialogloader"><span>载入中...</span></div>
    </div>
</div>

<div class="dialog" style="position:absolute;top:10px;left:550px;">
    <div class="dialog-inner">
        <div class="dialogcontent" style="width:400px;">
            <div class="dialoginfowrap">
                <div class="dialogclose"><a href="#" accesskey="Q" title="关闭">关闭(<u>Q</u>)</a></div>
                <div class="dialoginfo dialoginfo-error">
                    <h3>操作失败</h3>
                </div>
                <div class="clearfix dialogfoot">
                    <button class="button button-highlight" type="button" accesskey="c" title="关闭"><span>关闭(<u>C</u>)</span></button>
                </div>
            </div>
        </div>
        
    </div>
</div>

<div class="dialog" style="position:absolute;top:200px;left:550px;">
    <div class="dialog-inner">
        <div class="dialogcontent" style="width:400px;">
            <div class="dialoginfowrap">
                <div class="dialogclose"><a href="#" accesskey="Q" title="关闭">关闭(<u>Q</u>)</a></div>
                <div class="dialoginfo dialoginfo-alert">
                    <h3>操作失败</h3>
                    <p>5 秒后自动关闭</p>
                </div>
                <div class="clearfix dialogfoot">
                    <button class="button button-highlight" type="button" accesskey="c" title="关闭"><span>关闭(<u>C</u>)</span></button>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="dialog" style="position:absolute;top:400px;left:550px;">
    <div class="dialog-inner">
        <div class="dialogcontent" style="width:400px;">
            <div class="dialoginfowrap">
                <div class="dialogclose"><a href="#" accesskey="Q" title="关闭">关闭(<u>Q</u>)</a></div>
                <div class="dialoginfo dialoginfo-success">
                    <h3>操作失败</h3>
                    <p>5 秒后自动关闭</p>
                </div>
                <div class="clearfix dialogfoot">
                    <button class="button button-highlight" type="button" accesskey="c" title="关闭"><span>|关闭|</span></button>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="dialog" style="position:absolute;top:700px;left:550px;">
    <div class="dialog-inner">
        <div class="dialogcontent" style="width:400px;">
            <div class="clearfix dialoghead">
                <h1 class="dialogtitle">是否删除主题</h1>
                <div class="dialogstatus">共选择2个主题</div>
                <div class="dialogclose"><a href="#" accesskey="Q" title="关闭">关闭(<u>Q</u>)</a></div>
            </div>
            <div class="clearfix dialogbody">
                <div class="dialogconfirm">
                    <h3>您确定要删除该主题</h3>
                    <p>删除的主题将无法恢复.</p>
                </div>
            </div>
            <div class="clearfix dialogfoot">
                <button class="button button-highlight" type="submit" accesskey="y" title="确认"><span>确认(<u>Y</u>)</span></button>
                <button class="button" type="button" accesskey="c" title="取消"><span>取消(<u>C</u>)</span></button>
                <button class="button button-disable" type="button" accesskey="c" title="取消"><span>取消(<u>C</u>)</span></button>
            </div>
        </div>
    </div>
</div>

</body>
</html>
