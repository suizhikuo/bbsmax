<!--[DialogMaster title="公告编辑 - $_if($Announcement.Announcementid==0,'添加新公告',$Announcement.subject)" width="600"]-->
<!--[place id="body"]-->
<!--[success]-->
<div class="dialogmsg dialogmsg-success" id="successmsg">公告添加成功</div>
<!--[/success]-->
<form id="form1" method="post" action="$_form.action">
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="subject">标题</label></h3>
            <div class="form-enter">
                <input id="subject" type="text" maxlength="200" value="$_form.text('subject',$Announcement.subject)" name="subject" class="text longtext" />
                <!--[error name="subject"]--><span class="form-tip tip-error">公告标题不能为空</span><!--[/error]-->
            </div>
        </div>
          <div class="formrow">
            <h3 class="label"><label for="sortorder">序号</label></h3>
                <div class="form-enter">
                    <input id="sortorder" maxlength="3" type="text" value="$_form.text('sortorder',$Announcement.sortorder)" name="sortorder" class="text number" />
                </div>
          </div>
          <div class="formrow">
            <h3 class="label"><label for="begindate">起始时间</label></h3>
                <div class="form-enter">
                    <span class="datepicker">
                        <input maxlength="50"  type="text" class="text" id="begindate" name="begindate" value="$_form.text('begindate',$AnnBeginDate)" />
                        <a title="选择日期" href="javascript:void(0);" id="A0">选择日期</a>
                    </span>
                </div>
          </div>
          <div class="formrow">
            <h3 class="label"><label for="enddate">结束时间</label></h3>
                <div class="form-enter">
                    <span class="datepicker">
                        <input maxlength="50" type="text" class="text" id="enddate" name="enddate" value="$_form.text('enddate', $annenddate)" />
                        <a title="选择日期" href="javascript:void(0);" id="A1">选择日期</a>
                    </span>
                </div>
          </div>
          <div class="formrow">
            <h3 class="label"><label for="AnnouncementType">公告形式</label></h3>
                <div class="form-enter">
                    <select name="AnnouncementType" id="AnnouncementType">
                    <option value="text" $_form.selected('AnnouncementType','text',$Announcement.Announcementtype==AnnouncementType.Text)>文本</option>
                    <option value="urllink" $_form.selected('AnnouncementType','urllink',$Announcement.Announcementtype==AnnouncementType.UrlLink)>链接</option>
                    </select>
                </div>
          </div>
          <div class="formrow" id="htmlcode">
            <h3 class="label"><label for="content">公告内容</label></h3>
                <div class="form-enter">
                    <div class="htmleditorwrap">
                    <textarea name="content" id="content" style="height:100px; width:400px;">$_form.text('content',$Announcement.content)</textarea>
                    </div>
                    <!--[error name="content"]--><span class="form-tip tip-error">公告内容不能为空</span><!--[/error]-->
                </div>
          </div>
          <div class="formrow" id="urllink">
            <h3 class="label"><label for="url">公告地址</label></h3>
                <div class="form-enter">
                    <input type="text" maxlength="200px" class="text" name="url" id="url" value="$_form.text('url',$_if($IsLink,$Announcement.content))" />
                  <!--[error name="content"]--><span class="form-tip tip-error">公告地址不能为空</span><!--[/error]-->
                </div>
          </div>
    </div>
</div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="save" accesskey="s" title="保存"><span>保存(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
    <script type="text/javascript">
        var _thePanel = currentPanel;
        loadScript("$root/max-assets/nicedit/nicEdit.js", null, function() {
            var _editor = initMiniEditor("content");
            _thePanel.onSubmit = function() {
                _editor.saveContent();
                return true;
            } 
        });

    initDisplay("AnnouncementType", [
         { value:"text",        display:true,       id:'htmlcode' }
        ,{ value:"text",        display:false,      id:'urllink' }
        ,{ value:"urllink",     display:true,       id:'urllink' }
        ,{ value:"urllink",     display:false,      id:'htmlcode' }

    ]);

    initDatePicker('begindate', 'A0');
    initDatePicker('enddate', 'A1');
    </script>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->