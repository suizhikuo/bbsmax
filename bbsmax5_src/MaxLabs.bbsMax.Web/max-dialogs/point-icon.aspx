<!--[DialogMaster title="积分等级图标" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[include src="_error_.ascx" /]-->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">操作成功!</div>
<!--[/success]-->

<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="pointvalue">初级所需$CurrentUserPoint.Name</label></h3>
                <div class="form-enter">
                    <input name="pointValue" id="pointvalue" type="text" class="text" value="$_form.text('Pointvalue',$out($PointIcon.pointvalue))" />
                    <!--[error name="pointValue"]-->
                    <span class="form-tip tip-error">$message</span>
                    <!--[/error]-->
                </div>
                <div class="form-note">一个初级图标所需$CurrentUserPoint.Name</div>
            </div>
          <div class="formrow">
            <h3 class="label"><label for="iconCount">升级所需图标个数</label></h3>
                <div class="form-enter">
                <input name="iconCount" id="iconCount" type="text" class="text" value="$_form.text('iconCount',$out($PointIcon.iconCount))" />
                <!--[error name="iconCount"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
                </div>
                <div class="form-note">上升一级所需当前图标个数</div>
          </div>
          <div class="formrow">
            <h3 class="label"><label for="icons">图标</label></h3>
                <div class="form-enter">
                <textarea id="icons" name="icons" cols="50" rows="3">$_form.text('icons',$out($PointIcons))</textarea>
                <!--[error name="icons"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
                </div>
                <div class="form-note">图标请上传到 max-assets/icon-point 目录下, 这里请填写图标文件名称, 1行1个, 越上面的图标表示的等级越高.</div>
          </div>
    </div>
</div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="savepointicon" accesskey="s" title="保存设置"><span>保存设置(<u>S</u>)</span></button>
    <button class="button" type="submit" name="deletepointicon" title="删除设置" onclick="panel.close();"><span>删除设置</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
