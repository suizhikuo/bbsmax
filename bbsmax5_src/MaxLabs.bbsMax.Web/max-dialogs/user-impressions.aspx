<!--[DialogMaster width="550"]-->
<!--[place id="body"]-->
<div class="clearfix dialoghead" id="dialogTitleBar_$PanelID">
    <h3 class="dialogtitle">$user.username的资料</h3>
    <div class="dialogclose"><a href="javascript:void(panel.close());" accesskey="Q" title="关闭">关闭(<u>Q</u>)</a></div>
    <script type="text/javascript">
        maxDragObject(currentPanel.panel, $('dialogTitleBar_$PanelID'));
    </script>
</div>
<!--[unnamederror]-->
<div class="dialogmsg dialogmsg-error">$message</div>
<!--[/unnamederror]-->
<div class="clearfix dialogbody dialoguserprofile">
    <div class="dialoguser-sidebar">
        <div class="dialoguser-avatar">
            <a href="$url(space/$user.ID)" target="_blank"><img src="$user.avatarpath" alt="" width="48" height="48" /></a>
        </div>
        <ul class="dialoguser-menu">
            <li><a class="icon-profile" href="$dialog/user-profiles.aspx?uid=$user.id">个人资料</a></li>
            <li><a class="current icon-impression" href="$dialog/user-impressions.aspx?uid=$user.id">好友印象</a></li>
        </ul>
    </div>
    <div class="dialoguser-content">
        <div class="dialoguserimpression">
            <!--[if $IsShowImpressionInput == false]-->
            <div class="clearfix dialoguserimpression-head">
                <p class="impression-title">大家对 $User.name 的印象是:</p>
                <!--[if $VisitorIsFriend && $CanImpression]-->
                <p class="impression-action"><a href="$dialog/user-impressions.aspx?imp=1&uid=$UserID">我要对<!--[if $user.Gender == Gender.Female]-->她<!--[else]-->他<!--[/if]-->进行描述</a></p>
                <!--[/if]-->
            </div>
            <!--[/if]-->
            <div class="clearfix dialoguserimpression-body">
                <script type="text/javascript">
                    function submitImpressionForm() {
                        //ajaxSubmit('ImpressionForm', 'CreateImpression', 'sp_impression', null, null, true);
                    }
                    function submitImpression(text) {
                        $('ImpressionText').value = text;
                        //$('ImpressionSubmit').onclick();
                        //submitImpressionForm();
                    }
                </script>
            <!--[if $IsShowImpressionInput && $VisitorIsFriend && $CanImpression]-->
                <form action="$dialog/user-impressions.aspx?uid=$UserID&isdialog=1" method="post" id="ImpressionForm">
                <div class="dialogform impressionform">
                    <!--[unnamederror form="ImpressionForum"]-->
                    <div class="errormsg">$message</div>
                    <!--[/unnamederror]-->
                    <div class="formrow impressionform-input">
                        <h3 class="label"><label for="ImpressionText">输入您对$User.Name的印象</label></h3>
                        <div class="form-enter">
                            <input class="text longtext" type="text" name="Text" id="ImpressionText" onkeyup="textCounter(this,'impressionlimit',100)" />
                        </div>
                        <div class="form-note"><span class="impression-textlimit" id="impressionlimit">100</span></div>
                        <button class="button button-highlight" id="ImpressionSubmit" type="submit" title="提交" name="CreateImpression"><span>提交</span></button>
                    </div>
                    <!--[if $ImpressionTypeList.count != 0]-->
                    <div class="formrow impressionform-suggest">
                        <h3 class="label">供您参考的印象词:</h3>
                        <div class="form-enter">
                            <!--[loop $ImpressionType in $ImpressionTypeList with $i]-->
                            <a href="#" onclick="submitImpression('$ImpressionType.Text');return false;">{=$ImpressionType.Text}</a>
                            <!--[/loop]-->
                        </div>
                    </div>
                    <!--[/if]-->
                </div>
                </form>
            <!--[else]-->
                <!--[if $ImpressionList.Count == 0]-->
                <div class="nodata">
                    <!--[if $UserID == $MyUserID]-->
                    你还没有收到任何印象描述
                    <!--[else]-->
                    当前没有任何印象描述。
                    <!--[/if]-->
                </div>
                <!--[else]-->
                <script type="text/javascript">
                    function submitImpressionDeleteForm(typeid) {
                        if (confirm('确认要删除所选的好友印象吗？')) {
                            var proxy = document.createElement('input');
                            proxy.value = typeid.toString();
                            proxy.name = 'TypeID';
                            proxy.type = 'hidden';
                            $('ImpressionDeleteForm').appendChild(proxy);
                            //ajaxRender('$dialog/user-impressions.aspx?uid=$UserID&typeid='+typeid, 'ap_impression', null);
                            //ajaxSubmit('ImpressionDeleteForm', 'DeleteImpression', 'sp_impression', null, null, true);
                            //location.href = '$dialog/user-impressions.aspx?uid=$UserID&isdialog=1&typeid='+typeid;
                        }
                        return false;
                    }
                </script>
                <form action="$dialog/user-impressions.aspx?uid=$UserID&isdialog=1" method="post" id="ImpressionDeleteForm">
                    <div class="impression-word">
                        <!--[loop $Impression in $ImpressionList with $i]-->
                        <a href="javascript:;" class="imp-{=$i%10+1}" title="被这样描述了{=$Impression.Count}次" onclick="return false;">
                            $Impression.Text
                        </a>
                        <!--[if $User.UserID == $MyUserID]-->
                        <span><a href="$dialog/user-impressions.aspx?uid=$UserID&isdialog=1&typeid=$Impression.TypeID" onclick="if(confirm('确认要删除所选的好友印象吗?')){panel.loadPage(this.href);} return false;">删除</a></span>
                        <!--[/if]-->
                        <!--[/loop]-->
                    </div>
                </form>
                <!--[/if]-->
                <!--[if $ImpressionRecordList != null && $ImpressionRecordList.count > 0]-->
                <div class="impression-log">
                    <ul>
                        <!--[loop $record in $ImpressionRecordList with $i]-->
                        <li $_if($i%2==0,'class="odd"')><a class="fn" href="$url(space/$record.User.id)">$record.User.Name</a> <span  class="date">$OutputFriendlyDate($record.CreateDate)描述了:</span> $record.Text</li>
                        <!--[/loop]-->
                    </ul>
                </div>
                <!--[/if]-->
                <!--[if $VisitorIsFriend && !$CanImpression]-->
                <div class="impression-note">
                    您对他的印象描述 "$LastImpressionRecord.Text", $NextImpressionTime后才能继续描述。
                </div>
                <!--[/if]-->
            <!--[/if]-->
            </div>
        </div>
    </div>
</div>
<!--[if $IsShowImpressionInput && $VisitorIsFriend && $CanImpression]-->
<div class="clearfix dialogfoot">
    <a class="button" href="$dialog/user-impressions.aspx?imp=0&uid=$UserID"><span>取消</span></a>
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭(<u>C</u>)</span></button>
</div>
<!--[else]-->
<div class="clearfix dialogfoot">
    <button class="button" type="reset" accesskey="c" title="关闭" onclick="panel.close();"><span>关闭(<u>C</u>)</span></button>
</div>
<!--[/if]-->
<!--[/place]-->
<!--[/dialogmaster]-->