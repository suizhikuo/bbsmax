<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/search.css" />
</head>
<body>
<div class="container section-search">
<!--#include file="_inc/_head.aspx"-->
    <div id="main" class="main">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <div id="searchDiv" class="content-main-inner">
                            <form id="search" action="$_form.action" method="post" onsubmit="return clickButton()">
                            <div class="panel searchform">
                                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                                    <h3 class="panel-title"><span>搜索</span></h3>
                                </div></div></div>
                                <div class="panel-body">
                                    <div class="clearfix searchform-normal">
                                        <div class="searchform-enter">
                                            <input class="text searchform-input" id="SearchText" name="SearchText" type="text" value="$searchString" />
                                            <select class="searchform-select" name="mode">
                                                <option value="1" $_form.selected("mode","1",{=$mode == SearchMode.Subject})>主题标题</option>
                                                <!--[if $CanSearchTopicContent]-->
                                                <option value="6" $_form.selected("mode","6",{=$mode == SearchMode.TopicContent})>主题内容</option>
                                                <!--[/if]-->
                                                <!--[if $CanSearchAllPost]-->
                                                <option value="2" $_form.selected("mode","2",{=$mode == SearchMode.Content})>帖子内容</option>
                                                <!--[/if]-->
                                                <!--[if $CanSearchUserTopic]-->
                                                <option value="0" $_form.selected("mode","0",{=$mode == SearchMode.UserThread})>用户主题</option>
                                                <!--[/if]-->
                                                <!--[if $CanSearchUserPost]-->
                                                <option value="4" $_form.selected("mode","4",{=$mode == SearchMode.UserPost})>用户帖子</option>
                                                <!--[/if]-->
                                            </select>
                                        </div>
                                        <div class="searchform-submit">
                                            <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" id="SearchSubmit" name="SearchSubmit" value="搜索" /></span></span>
                                        </div>
                                        <div class="searchform-toggle" <!--[if $isShowResult == false]-->style="display:none"<!--[/if]-->>
                                            <a href="#" onclick="javascript:$('moreoption').style.display='';this.style.display='none';return false;">高级搜索</a>
                                        </div>
                                        <div class="searchform-tip">
                                            关键字超出20个字符将自动截断. 按作者查询为精确查询, 必须输入作者的用户名全称.
                                        </div>
                                    </div>
                                    <div id="moreoption" style="display:none" class="formgroup searchform-advance">
                                        <div class="formrow">
                                            <h3 class="label"><label>发布时间</label></h3>
                                            <div class="form-enter">
                                                <select name="searchtime" id="searchtime">
                                                    <option value="0" selected="selected">所有时间</option>
                                                    <option value="1">1天</option>
                                                    <option value="2">2天</option>
                                                    <option value="3">3天</option>
                                                    <option value="7">1周</option>
                                                    <option value="30">1个月</option>
                                                    <option value="90">3个月</option>
                                                    <option value="180">半年</option>
                                                    <option value="365">1年</option>
                                                </select>
                                                <input type="radio" name="isbefore" value="false" id="isbefore1" checked="checked" />
                                                <label for="isbefore1">之内</label>
                                                <input name="isbefore" type="radio" value="true" id="isbefore2" />
                                                <label for="isbefore2">之前</label>
                                            </div>
                                        </div>
                                        <div class="formrow">
                                            <h3 class="label"><label>结果排序</label></h3>
                                            <div class="form-enter">
                                                <input type="radio" name="isdesc" id="isdesc1" value="true" checked="checked" /><label for="isdesc1">降序</label>
                                                <input name="isdesc" type="radio" id="isdesc2" value="false" /><label for="isdesc2">升序</label>
                                            </div>
                                        </div>
                                        <div class="formrow">
                                            <h3 class="label"><label>版块范围</label></h3>
                                            <div class="form-enter">
                                                <select class="forumselect" name="forumIDs" multiple="true">
                                                    <option value="0" selected="selected">选择所有版块</option>
                                                    <!--[loop $forum in $forums with $i]-->
                                                    <option value="$forum.ForumID">|$ForumSeparators[$i]$forum.ForumNameText</option>
                                                    <!--[/loop]-->
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="search-loader" id="loadingDiv" style="display:none;">
                                        <span>正在生成搜索结果, 请稍候.</span>
                                    </div>
                                    <div class="page-message" id="errorDiv" style="display:none;">
                                        <span id="errorMessage">搜索出错了!</span>
                                    </div>
                                </div>
                                <div class="panel-foot"><div><div>-</div></div></div>
                            </div>
                            </form>
                            <!--[ajaxpanel id="ap_data" idonly="true"]--> 
                            <!--[if $isShowResult]-->
                            <div id="dataDiv">
                            <form id="datalist" action="$_form.action" method="post">
                            <div class="panel searchresult">
                                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                                    <h3 class="panel-title"><span>搜索到相关结果 <strong class="count">$TotalCount</strong> 个</span></h3>
                                </div></div></div>
                                <div class="panel-body">
                                    <table class="searchresult-table">
                                        <!--[if $threadList.Count>0]-->
                                        <thead>
                                            <tr>
                                                <td class="icon">&nbsp;</td>
                                                <td class="title">主题</td>
                                                <td class="cate">所属板块</td>
                                                <td class="author">作者/发布时间</td>
                                                <td class="last">最后更新</td>
                                                <td class="stats">查看/回复</td>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <!--[loop $thread in $threadList]-->
                                            <tr>
                                                <td class="icon">
                                                <!--[if $thread.ThreadStatus == ThreadStatus.GlobalSticky]-->
                                                <img src="$skin/images/icons/topic_pinned.gif" title="总置顶主题" alt="[总置顶主题]" />
                                                <!--[else if $thread.ThreadStatus == ThreadStatus.Sticky]-->
                                                <img src="$skin/images/icons/topic_sticky.gif" title="置顶主题" alt="[置顶主题]" />
                                                <!--[else if $thread.IsLocked]-->
                                                <img src="$skin/images/icons/topic_lock.gif" title="锁定主题" alt="[锁定主题]" />
                                                <!--[else if $thread.TotalReplies >= $HotThreadRequireReplies]-->
                                                <img src="$skin/images/icons/topic_hot.gif" title="热门主题" alt="[热门主题]" />
                                                <!--[else]-->
                                                <img src="$skin/images/icons/topic_normal.gif" title="主题" alt="[主题]" />
                                                <!--[/if]-->
                                                </td>
                                                <td class="title">
                                                    <div class="title-wrap">
                                                        <!--[if $IsShowCheckBox($thread)]-->
                                                        <input type="checkbox" name="threadIDs" value="$thread.ThreadID" />
                                                        <!--[/if]-->
                                                        <a href="$url($thread.forum.codename/$thread.ThreadTypeString-$thread.ThreadID-1-1)?SearchText=$searchText" target="_blank">
                                                        $HighlightFormatter($thread.SubjectText)
                                                        </a>
                                                        <!--[if $thread.ThreadType == ThreadType.Poll]-->
                                                        <img src="$root/max-assets/icon/poll.gif" alt="[投票帖]" title="投票帖" />
                                                        <!--[else if $thread.ThreadType == ThreadType.Question]-->
                                                        <img src="$root/max-assets/icon/ask.gif" alt="[问答帖]" title="问答帖" />
                                                        <!--[else if $thread.ThreadType == ThreadType.Polemize]-->
                                                        <img src="$root/max-assets/icon/polemize.gif" alt="[辩论帖]" title="辩论帖" />
                                                        <!--[/if]-->
                                                        <!--[if $thread.IsValued]-->
                                                        <img src="$root/max-assets/icon/diamond_blue.gif" alt="[精华帖子]" title="精华帖子" />
                                                        <!--[/if]-->
                                                    </div>
                                                </td>
                                                <td class="cate"><a href="$url($thread.forum.CodeName/list-1)" target="_blank">$thread.Forum.ForumNameText</a></td>
                                                <td class="author"><a href="$url(space/$thread.PostUserID)" target="_blank">$thread.PostUsername</a>$outputfriendlydatetime($thread.CreateDate)</td>
                                                <td class="last">
                                                    <a href="$url(space/$thread.LastReplyUserID)" target="_blank">$thread.LastReplyUsername</a>
                                                    <a href="$GetThreadLastPageUrl($thread)">$outputfriendlydatetime($thread.UpdateDate)</a>
                                                </td>
                                                <td class="stats">$thread.TotalViews / $thread.TotalReplies</td>
                                            </tr>
                                            <!--[/loop]-->
                                        </tbody>
                                            <!--[if $IsShowActionButton]-->
                                        <tbody>
                                            <tr>
                                                <td class="icon">&nbsp;</td>
                                                <td colspan="5">
                                                    <input type="checkbox" id="listSelectAllThreads" />全选
                                                    <script type="text/javascript">
                                                        var l = new checkboxList("threadIDs", "listSelectAllThreads");
                                                    </script>
                                                    $GetModeratorActionLinks(@"<a href=""javascript:managerCheck('{0}','threadIDs')"">{1}</a>&nbsp;&nbsp;")
                                                </td>
                                            </tr>
                                        </tbody>
                                            <!--[/if]-->
                                        <!--[else if $postList.Count>0]-->
                                        <thead>
                                            <tr>
                                                <td class="icon">&nbsp;</td>
                                                <td class="title">帖子</td>
                                                <td class="cate">所属板块</td>
                                                <td class="author">作者/发布时间</td>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        <!--[loop $post in $postList]--> 
                                        <tr>
                                            <td class="icon">
                                                <img src="$skin/images/icons/topic_normal.gif" title="帖子" alt="[帖子]" />
                                            </td>
                                            <td class="title">
                                                <div class="title-wrap">
                                                    <!--[if $post.PostType == PostType.ThreadContent]-->
                                                        <!--[if $mode == SearchMode.TopicContent]-->
                                                            <!--[if $IsShowCheckBox($GetThread($post.ThreadID))]-->
                                                            <input type="checkbox" name="threadIDs" value="$post.ThreadID" />
                                                            <!--[/if]-->
                                                        <!--[/if]-->
                                                    主题:
                                                    <!--[else]-->
                                                        <!--[if $AllowManagePost($post)]-->
                                                        <input type="checkbox" name="postIDs" id="postID_$post.PostID" value="$post.PostID" />
                                                        <!--[/if]-->
                                                    回复:
                                                    <!--[/if]-->
                                                    <a href="$GetSearchThreadUrl($post)" target="_blank">
                                                        $HighlightFormatter($post.SubjectText)
                                                    </a>
                                                    <br />
                                                    <a href="$GetSearchThreadUrl($post)" target="_blank">
                                                        <!--[if $post.IsShielded]-->
                                                            该帖子已被屏蔽
                                                            <!--[if $IsAlwaysViewShieldContents($post)]-->
                                                            ，但您仍有查看权限<br />
                                                            $HighlightFormatter($post.ContentForSearch)
                                                            <!--[/if]-->
                                                        <!--[else if $IsShieldedUser($post)]-->
                                                            该用户已被屏蔽
                                                            <!--[if $IsAlwaysViewShieldContents($post)]-->
                                                            ，但您仍有查看权限<br />
                                                            $HighlightFormatter($post.ContentForSearch)
                                                            <!--[/if]-->
                                                        <!--[else if $post.PostType == PostType.ThreadContent]-->
                                                            <!--[if $CanViewThread($post.ForumID) == false]-->
                                                                <!--[if $IsLogin == false]-->(您是游客)<!--[/if]-->您没有权限查看该主题内容
                                                            <!--[else if $CanViewValuedThread($post) == false]-->
                                                                <!--[if IsLogin == false]-->(您是游客)<!--[/if]-->您没有权限查看该精华主题内容
                                                            <!--[else if $HasBuyed($post) == false]-->
                                                                该帖需要购买才能查看
                                                                <!--[if $IsAlwaysViewContents($post)]-->
                                                                ，但您仍有查看权限<br />
                                                                $HighlightFormatter($post.ContentForSearch)
                                                                <!--[/if]-->
                                                            <!--[else]-->
                                                            $HighlightFormatter($post.ContentForSearch)
                                                            <!--[/if]-->
                                                        <!--[else if $CanViewPost($post) == false]-->
                                                            <!--[if $IsLogin == false]-->(您是游客)<!--[/if]-->您没有权限查看该回复内容
                                                        <!--[else]-->
                                                        $HighlightFormatter($post.ContentForSearch)
                                                        <!--[/if]-->
                                                    </a>
                                                </div>
                                            </td>
                                            <td class="cate"><a href="$url($post.forum.CodeName/list-1)" target="_blank">$post.Forum.ForumNameText</a></td>
                                            <td class="author"><a href="$url(space/$post.UserID)" target="_blank">$post.Username</a>$outputfriendlydatetime($post.CreateDate)</td>
                                        </tr>
                                        <!--[/loop]-->  
                                        </tbody> 
                                            <!--[if $IsShowActionButton]-->
                                        <tbody>
                                            <tr>
                                                <td class="icon">&nbsp;</td>
                                                <td colspan="3">
                                                    <!--[if $mode == SearchMode.TopicContent]-->
                                                    <input type="checkbox" id="listSelectAllThreads2" />全选
                                                    <script type="text/javascript">
                                                        var l = new checkboxList("threadIDs", "listSelectAllThreads2");
                                                    </script>
                                                    $GetModeratorActionLinks(@"<a href=""javascript:managerCheck('{0}','threadIDs')"">{1}</a>&nbsp;&nbsp;")
                                                    <!--[else]-->
                                                    <input type="checkbox" id="listSelectAllPosts" />全选
                                                    <script type="text/javascript">
                                                        var l = new checkboxList("postIDs", "listSelectAllPosts");
                                                    </script>
                                                    $GetPostActionLinks(@"<a href=""javascript:managerCheck('{0}','postIDs')"">{1}</a>&nbsp;&nbsp;")
                                                    <!--[/if]-->
                                                </td>
                                            </tr>
                                        </tbody>
                                            <!--[/if]-->
                                        <!--[else]-->
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <div class="nodata">未搜索到任何结果.</div>
                                                </td>
                                            </tr>
                                        </tbody>
                                        <!--[/if]-->
                                    </table>
                                </div>
                                <div class="panel-foot"><div><div>-</div></div></div>
                            </div>
                            </form>
                            <!--[pager name="list" ajaxpanelID="ap_data" skin="_inc/_pager_app_ajax.aspx"]-->
                            </div>
                            <!--[/if]-->
                            <!--[/ajaxpanel]-->
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>
<!--#include file="_inc/_foot.aspx"-->
</div>
<script type="text/javascript">
    //<!--[if $isShowResult]-->
    $('moreoption').style.display = 'none';
    //<!--[else]-->
    $('moreoption').style.display = '';
    //<!--[/if]-->
    function managerCheck(url, objName) {
        var obj = document.getElementsByName(objName);
        for (var i = 0, len = obj.length; i < len; i++) {
            if (obj[i].checked) {
                postdata(url);
                return;
                break;
            }
        }
        if (objName == "postIDs")
            showAlert('请至少选择一个回复');
        else
            showAlert('请至少选择一个主题');
    }
    var postdata = function (url) {
        postToDialog({ formId: "datalist", url: url, callback: refresh });
    }

    onEnterSubmit("SearchText", "SearchSubmit");


    function clickButton() {
        $('loadingDiv').style.display = '';
        $('errorDiv').style.display = 'none';
        if ($('dataDiv'))
            $('dataDiv').style.display = 'none';
        $('SearchSubmit').disabled = 'disabled';
        $('moreoption').style.display = 'none';
        ajaxSubmit('search', 'SearchSubmit', 'ap_data', function (result) {
            $('SearchSubmit').disabled = 'disabled';
            if (result != null) {
                if (result.iserror) {
                    $('loadingDiv').style.display = 'none';
                    $('errorMessage').innerHTML = result.message;
                    $('errorDiv').style.display = '';
                }
                else if (result.issuccess) {
                    $('loadingDiv').style.display = 'none';
                    $('errorDiv').style.display = 'none';
                    $('dataDiv').style.display = '';
                    tempSearchTime = searchTime;
                    closeSearchTime();
                    //location.href = '$SearchUrl?searchid=' + result.message;
                }
            }
        }, null, true);
        return false;
    }

    var searchTime = $SearchTime;
    var tempSearchTime = searchTime;
    function closeSearchTime() {
        var searchButton = document.getElementById('SearchSubmit');
        if (tempSearchTime == 0) {
            searchButton.disabled = '';
            searchButton.parentNode.parentNode.className = 'minbtn-wrap';
            searchButton.value = '搜索';
        } else {
            searchButton.disabled = 'disabled';
            searchButton.parentNode.parentNode.className = 'minbtn-wrap minbtn-disable';
            searchButton.value = '搜索 (' + tempSearchTime + 's)';
            tempSearchTime--;
            window.setTimeout("closeSearchTime()", 1000);
        }
    }
</script>
</body>
</html>
