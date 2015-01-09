<!--[DialogMaster title="批量更新快捷方式" width="500"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->

<input type="hidden" name="emoticonids" value="$_form.text('emoticonids')" />
<div class="clearfix dialogbody">
    <div class="dialogform">
        <div class="formrow">
            <div class="form-enter">
                <div class="datatablewrap" id="container" style="height:250px;">
                    <table class="datatable">
                        <thead>
                            <tr>
                                <th>图片</th>
                                <th>快捷方式</th>
                            </tr>
                        </thead>
                        <tbody>
                            <!--[loop $emoticon in $EmoticonList]-->
                            <tr>
                                <td><img alt="" src="$emoticon.imageurl" onload="imageScale(this, 80, 60)" /></td>
                                <td><input type="text" class="text" value="$emoticon.shortcut" name="shortcut.$emoticon.emoticonid" style="width:15em;" /></td>
                            </tr>
                            <!--[/loop]-->
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="form-note">为了避免用户表情快捷方式对原有HTML内容的破坏，系统会自动在快捷方式前后加上 "{" 和 "}" </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="update" class="button" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->