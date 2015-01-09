<!--[DialogMaster title="结帖" subtitle="$question.SubjectText" width="800"]-->
<!--[place id="body"]-->

<!--[unnamederror]-->
<div class="dialogmsg dialogmsg-error">$Message</div>
<!--[/unnamederror]-->

<div class="clearfix dialogbody">
    
    <div style="margin-bottom:10px;">
    结帖
    (本问题奖励额为 <span style="color:Red">$tradePoint.Name $question.Reward $tradePoint.UnitName</span>
    最多只能奖励给 <span style="color:Red">$question.RewardCount</span> 个回复,
    对最佳答案的奖励不能低于对其它回复的奖励)
    </div>

    <div id="rewardposts" style="display:none;">
    <form id="questionform" name="questionform" method="post" enctype="multipart/form-data" action="$_form.action">
    <input type="hidden" name="itemcount" id="itemcount" value="0" />
    <div class="datatablewrap" style="height:120px;">
        <table class="datatable" id="listtable">
        <thead>
            <tr>
                <td>用户</td>
                <td>最佳答案</td>
                <td>奖励(填写整数)</td>
                <td>回复内容</td>
                <td>取消</td>
            </tr>
        </thead>
        <tbody>
        <tr style="display:none"></tr>
        </tbody>
        </table>
        <table style="display:none">
            <tr id="listitem">
                <td id="item_user"><a href="javascript:'[url]';" target="_blank">{name}</a></td>
                <td id="item_bestpost">
                    <input id="bestPostID_{postid}" name="bestPostID" value="{postid}" type="radio" />
                </td>
                <td id="item_reward">
                    <input id="reward_{postid}" name="reward_{postid}" onkeyup="value=value.replace(/[^\d]/g,'')" class="text" />
                </td>
                <td id="item_content">{content}</td>
                <td id="item_action">
                    <input type="hidden" name="postid" value="{postid}" />
                    <a href="javascript:void(deleteitem('{postid}'));" name="delete">取消</a>
                </td>
            </tr>
        </table>
    </div>
    <div style="margin-bottom:10px;">
        <button class="button button-highlight" type="submit" name="finalButton" id="finalButton"  accesskey="y" title="结帖"><span>结帖</span></button>
    </div>
    </form>
    </div>
    <!--[ajaxpanel id="ap_postlist2"]-->
    <!--[if $PostList.count != 0]-->
    <div class="datatablewrap" style="height:230px;">
        <table class="datatable">
            <thead>
                <tr>
                    <td>用户</td>
                    <td>奖励(请选中要奖励的帖子)</td>
                    <td>回复内容</td>
                </tr>
            </thead>
            <tbody>
            <!--[loop $post in $postlist]-->
                <tr>
                    <td><a href="$url(space/$post.UserID)" target="_blank">$post.Username</a></td>
                    <td>
                    <!--[if $post.UserID == $question.PostUserID]-->
                        <input type="checkbox" disabled="disabled" />
                    <!--[else]-->
                        <input type="checkbox" id="cbox_$post.postid" onclick="onClickCheckBox(this,$post.postid,'$url(space/$post.UserID)','$post.Username')"  <!--[if $Checked($post.postid)]-->checked="checked"<!--[/if]--> />
                    <!--[/if]-->
                    </td>
                    <td id="postContent_$post.postid">$post.contenttext</td>
                </tr>
            <!--[/loop]-->
            </tbody>
        </table>
    </div>
    <!--[else]-->
    <div class="nodata">
        当前还没有人回复, 不能结帖.
    </div>
    <!--[/if]-->
 
    <div id="pagenation" class="clearfix pagination">
        <div id="pagelist" class="pagination-inner">
        <!--[pager name="list" skin="_pagerajax.aspx" ajaxpanelID="ap_postlist2"]-->
        </div>
    </div>
    <!--[/ajaxpanel]-->
</div>

<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭(<u>C</u>)</span></button>
</div>
<script type="text/javascript">
    var listtable = $('listtable');
    var itemuser = fix($('item_user').innerHTML);
    var itemcontent = fix($('item_content').innerHTML);
    var itemreward = fix($('item_reward').innerHTML);
    var itembestpost = fix($('item_bestpost').innerHTML);
    var itemaction = fix($('item_action').innerHTML);

    function onClickCheckBox(obj, postid, url, name) {
        if (obj.checked)
            additem(postid, url, name, fix($('postContent_' + postid).innerHTML));
        else
            deleteitem(postid);
    }

    function additem(postid, spaceUrl, username, content, userid) {
        if (userid == $Question.PostUserID)
            return;

        var itemuserStr = itemuser.replace(/javascript:'\[url\]';/g, spaceUrl).replace(/\{name\}/g, username);
        
        var itemcontentStr = itemcontent.replace(/\{content\}/g, content);
        var itemrewardStr = itemreward.replace(/\{postid\}/g, postid);
        var itembestpostStr = itembestpost.replace(/\{postid\}/g, postid);
        var itemactionStr = itemaction.replace(/\{postid\}/g, postid);

        var row = listtable.insertRow(listtable.rows.length);
        row.id = 'list_item_' + postid;

        var cell1 = row.insertCell(0);
        var cell2 = row.insertCell(1);
        var cell3 = row.insertCell(2);
        var cell4 = row.insertCell(3);
        var cell5 = row.insertCell(4);
        cell1.innerHTML = itemuserStr;
        cell4.innerHTML = itemcontentStr;
        cell3.innerHTML = itemrewardStr;
        cell2.innerHTML = itembestpostStr;
        cell5.innerHTML = itemactionStr;

        var itemC = $('itemcount');
        itemC.value = parseInt(itemC.value) + 1;
        $('rewardposts').style.display = '';
        changeUrl();
    }
    function deleteitem(postid) {
        var l = $('list_item_' + postid);
        if (l)
            removeElement(l);
        var itemC = $('itemcount');
        itemC.value = parseInt(itemC.value) - 1;
        if (parseInt(itemC.value) < 1) {
            $('rewardposts').style.display = 'none';
        }
        var cbox = $('cbox_' + postid);
        if (cbox != null)
            cbox.checked = false;
        changeUrl();
    }
    function changeUrl() {
        var c = $('pagelist').childNodes;
        var postids = document.getElementsByName('postid');
        var postidString = '';
        for (var i = 0; i < postids.length; i++) {
            if (postids[i].value != '{postid}') {
                if (i != 0)
                    postidString += ',';
                postidString += postids[i].value;
            }
        }
        for (var i = 0; i < c.length; i++) {
            var a = c[i];
            if (a.nodeName.toLowerCase() == "a") {
                var url = a.href;
                var index = url.indexOf('postids');
                if (index > 1) {
                    url = url.substring(0, index - 1);
                    var temp = a.href.substring(index + 1);
                    a.href = url + '&postids=' + postidString;
                    if (temp.indexOf('&') > 0) {
                        a.href = a.href + temp.substring(temp.indexOf('&'));
                    }
                }
                else
                    a.href = url + '&postids=' + postidString;
            }
        }
    }
    function fix(str) {
        return str.replace(/value=\{([\w]+)\}/g, 'value="{$1}"');
    };
//<!--[unnamederror]-->
//<!--[loop $post in $postlist]-->
//<!--[if $Checked($post.postid)]-->
additem($post.postid, '$url(space/$post.UserID)','$post.Username', fix('$post.contenttext'));
//<!--[/if]-->
var rewardInput = $('reward_' + $post.postID);
if (rewardInput != null)
    rewardInput.value = '$GetRewardString($post.PostID)';
//<!--[/loop]-->
//<!--[if $BestPostID > 0]-->
$('bestPostID_' + $BestPostID).checked = true;
//<!--[/if]-->
//<!--[/unnamederror]-->
</script>
<!--[/place]-->
<!--[/DialogMaster]-->
 