<!--[DialogMaster title="$Category.name" subtitle="$advert.title" width="650"]-->
<!--[place id="body"]-->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">广告添加成功.</div>
<!--[/success]-->
<form id="form1" method="post" action="$_form.action">
<div class="clearfix dialogbody">
    <div class="clearfix dialogcolumnlayout">
        <div class="dialogcolumnlayout-content">
            <div class="dialogcolumnlayout-content-inner">
                <div class="scroller" style="height:350px;">
                    <div class="dialogform dialogform-horizontal">
                        <div class="formrow">
                            <h3 class="label"><label for="index">序号</label></h3>
                            <div class="form-enter">
                                <input id="index" type="text" value="$_form.text('index',$Advert.index)" name="index" class="text number" />
                            </div>
                        </div>
                        <div class="formrow">
                            <h3 class="label"><label for="title">标题</label></h3>
                            <div class="form-enter">
                                <input id="title" type="text" value="$_form.text('title',$Advert.title)" name="title" class="text longtext" />
                            </div>
                        </div>
                        <div class="formrow">
                            <h3 class="label"><label for="Available">是否启用广告</label></h3>
                            <div class="form-enter">
                                <input type="checkbox" $_form.checked('Available','true',$advert.Available) name="Available" value="true" id="Available" />
                                <label for="Available">启用</label>
                            </div>
                        </div>
                        <div class="formrow">
                            <h3 class="label">投放日期</h3>
                            <div class="form-enter">
                                <span class="datepicker">
                                    <input type="text" class="text number" name="begindate" id="begindate" value="$_form.text('begindate',$AdBeginDate)" />
                                    <a title="选择日期" id="d_s" href="#">选择日期</a>
                                </span> -
                                <span class="datepicker">
                                    <input type="text" class="text number" name="enddate" id="enddate" value="$_form.text('enddate', $adenddate)" />
                                    <a title="选择日期" id="d_e" href="javascript:void(0)">选择日期</a>
                                </span>
                            </div>
                        </div>
                        <input type="hidden" name="position" value="$Advert.position" />
                        <!--div id="positionselect" class="formrow">
                            <h3 class="label"><label for="adposition">投放位置</label></h3>
                            <div class="form-enter">
                                <select name="position" disabled="disabled" id="adposition">
                                    <option value="Top" $_form.selected('position','Top',$Advert.position==ADPosition.Top)>帖子上方</option>
                                    <option value="Bottom"  $_form.selected('position','Bottom',$Advert.position==ADPosition.Bottom)>帖子下方</option>
                                    <option value="Right"  $_form.selected('position','Right',$Advert.position==ADPosition.Right)>帖子右边</option>
                                </select>
                            </div>
                        </div-->
                        <input type="hidden" name="position" value="$ADPosition" />

                        <div id="showFloor" class="formrow">
                            <h3 class="label">显示楼层</h3>
                            <div class="form-enter">
                                <!--[loop 1 to $floorCount with $i]-->
                                <input type="checkbox" name="floor" $_form.checked('floor','{=$i-1}',$AdInFloor($i-1)) id="floor{=$i-1}" value="{=$i-1}"/>
                                <label for="floor{=$i-1}">#$i</label>&nbsp;
                                <!--[/loop]-->
                                <input type="checkbox" name="floorlast" $_form.checked('floorlast','-2',$AdInFloor(-2)) id="floor_2" value="-2"/>
                                <label for="floor_2">#最底楼</label>&nbsp;

                                <input type="checkbox" name="floorAll" $_form.checked('floor','-1',$AdInFloor(-1)) id="floor-1" value="-1"/>
                                <label for="floor-1">全部</label>&nbsp;
                                <!--[error name="floor"]--><span class="form-tip tip-error">请选择楼层</span><!--[/error]-->
                            </div>
                        </div>
                        <div class="formrow">
                            <h3 class="label"><label for="adtype">广告形式</label></h3>
                            <div class="form-enter">
                                <select name="adtype" id="adtype">
                                    <option value="Code" $_form.selected('adtype','Code',$Advert.adtype==ADType.Code)>HTML代码</option>
                                    <option value="Text" $_form.selected('adtype','Text',$Advert.adtype==ADType.Text)>文字链接</option>
                                    <option value="Image" $_form.selected('adtype','Image',$Advert.adtype==ADType.Image)>图片</option>
                                    <option value="Flash" $_form.selected('adtype','Flash',$Advert.adtype==ADType.Flash)>Flash</option>
                                </select>
                            </div>
                        </div>
                        <div id="image">
                        <div class="formrow">
                            <h3 class="label"><label for="imgsrc">图片地址 <span class="request" title="必填项">*</span></label></h3>
                            <div class="form-enter">
                                <!--
                                <span class="imagepicker">
                                -->
                                    <input type="text" class="text longtext" id="imgsrc" name="imagesrc" value="$_form.text('imagesrc', $advert.resourcehref)" />
                                <!--
                                    <a title="选择图片" href="javascript:void(browseImage('Assets_AdvertIcon','imgsrc'));">选择图片</a>
                                </span>
                                -->
                                <!--[error name="src"]--><span class="form-tip tip-error">图片地址不能为空</span><!--[/error]-->
                            </div>
                        </div>
                        <div class="formrow">
                            <h3 class="label"><label for="imghref">广告地址</label></h3>
                            <div class="form-enter">
                                <input type="text" class="text longtext" id="imghref" name="imagehref" value="$advert.href" />
                                <!--[error name="href"]--><span class="form-tip tip-error">广告地址不能为空</span><!--[/error]-->
                            </div>
                        </div>
                        <div class="formrow">
                            <h3 class="label"><label for="alt">图片提示</label></h3>
                            <div class="form-enter">
                                <input type="text" name="alt" id="alt" class="text" value="$_form.text('alt',$advert.text)" />
                            </div>
                        </div>
                        <div class="formrow">
                            <h3 class="label"><label for="imagewidth">图片宽度</label></h3>
                            <div class="form-enter">
                                <input type="text" class="text number" name="imagewidth" id="imagewidth" value="$_form.text('imagewidth',$advert.width)" /> px
                            </div>
                        </div>
                        <div class="formrow">
                            <h3 class="label"><label for="imageheight">图片高度</label></h3>
                            <div class="form-enter">
                                <input type="text" class="text" name="imageheight" id="imageheight" value="$_form.text('imageheight', $advert.height)" /> px
                            </div>
                        </div>
                        </div>
                        <div id="flash">
                        <div class="formrow">
                            <h3 class="label"><label for="flashsrc">Flash地址 <span class="request" title="必填项">*</span></label></h3>
                            <div class="form-enter">
                                <input type="text" class="text longtext" id="flashsrc" name="flashsrc" value="$_form.text('flashsrc',$advert.resourcehref)" />
                                <!--[error name="src"]--><span class="form-tip tip-error">Flash地址不能为空</span><!--[/error]-->
                            </div>
                        </div>
                        <div class="formrow">
                            <h3 class="label"><label for="flashheight">Flash高度<span class="request" title="必填项">*</span></label></h3>
                            <div class="form-enter">
                                <input type="text" class="text number" name="flashheight" id="flashheight" value="$_form.text('flashheight',$advert.height)" /> px
                                <!--[error name="height"]--><span class="form-tip tip-error">Flash高度不能为空</span><!--[/error]-->
                            </div>
                        </div>
                        <div class="formrow">
                            <h3 class="label"><label for="flashwidth">Flash宽度<span class="request" title="必填项">*</span></label></h3>
                            <div class="form-enter">
                                <input type="text" class="text number" name="flashwidth" id="flashwidth" value="$_form.text('flashwidth',$advert.width)" /> px
                                <!--[error name="width"]--><span class="form-tip tip-error">Flash宽度不能为空</span><!--[/error]-->
                            </div>
                        </div>
                        </div>
                        <div id="link">
                        <div class="formrow">
                            <h3 class="label"><label for="text">链接文字 <span class="request" title="必填项">*</span></label></h3>
                            <div class="form-enter">
                                <input type="text" name="text" id="text" class="text longtext" value="$_form.text('text',$advert.text)" />
                                <!--[error name="text"]--><span class="form-tip tip-error">广告链接文字不能为空</span><!--[/error]-->
                            </div>
                        </div>
                        <div class="formrow">
                            <h3 class="label"><label for="href">广告地址 <span class="request" title="必填项">*</span></label></h3>
                            <div class="form-enter">
                                <input type="text" class="text longtext" id="href" name="href" value="$_form.text('href',$advert.href)" />
                                <!--[error name="href"]--><span class="form-tip tip-error">广告地址不能为空</span><!--[/error]-->
                            </div>
                        </div>
                        <div class="formrow">
                            <h3 class="label"><label for="linkcolor">文字颜色</label></h3>
                            <div class="form-enter">
                                <span class="colorpicker">
                                    <input type="text" id="linkcolor" name="color" class="text" value="$_form.text('color',$advert.color)" />
                                    <span id="c_t"><a title="选择颜色"  href="#">选择颜色</a></span>
                                </span>
                            </div>
                        </div>
                        <div class="formrow">
                            <h3 class="label"><label for="fontsize">字体大小</label></h3>
                            <div class="form-enter">
                                <input type="text" name="fontsize" id="fontsize" class="text number" value="$_form.text('fontsize',$advert.FontSize)" /> px
                            </div>
                        </div>
                        </div>
                        <div id="htmlcode" class="formrow">
                            <h3 class="label"><label for="code">HTML代码 <span class="request" title="必填项">*</span></label></h3>
                            <div class="form-enter">
                                <textarea name="code" id="code" cols="50" rows="5">$_form.text('code',$advert.codeM)</textarea>
                                <!--[error name="code"]--><span class="form-tip tip-error">广告代码不能为空</span><!--[/error]-->
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
        <div class="dialogcolumnlayout-sidebar">
            <div class="dialogcolumnlayout-sidebar-inner">
                <div class="dialogform">
                    <div class="formrow">
                        <h3 class="label"><label for="targets">投放页面</label></h3>
                        <div class="form-enter">
                            <select multiple="multiple" style="width:150px;height:300px;" name="targets" id="targets">
                                <option value="all" $_if($HasTarget("all"),'selected="selected"')>全部</option>
                                <!--[loop $p in $Category.CommonPages]-->
                                <option value="$p.target" $_if($HasTarget($p.target),'selected="selected"')>$p.name</option>
                                <!--[/loop]-->
                                <!--[if $Category.ShowInForum]-->
                                    <!--[loop $forum in $forumList with $i]-->
                                        <!--[if $forum.parentid>0]-->
                                <option value="$ForumPrefix$forum.id" $_if($HasTarget("$ForumPrefix$forum.id"),'selected="selected"')>$forumSpliter[$i]$forum.ForumName</option>
                                        <!--[else]-->
                                <optgroup label="$forumSpliter[$i]$forum.ForumName">&nbsp;</optgroup>
                                        <!--[/if]-->
                                    <!--[/loop]-->
                                <!--[/if]-->
                            </select>
                        </div>
                        <div class="form-note">投放范围(按住Ctrl键多选)</div>
                    </div>
                </div>

            </div>
        </div>
    </div>
    
    </div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="save" accesskey="s" title="保存"><span>保存(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<script type="text/javascript">
initDisplay("adtype", [
     { value:"Code",          display:true,    id:'htmlcode' }
    ,{ value:"Code",          display:false,   id:'image' }
    ,{ value:"Code",          display:false,   id:'flash' }
    ,{ value:"Code",          display:false,   id:'link' }
    
    ,{ value:"Text",          display:false,   id:'htmlcode' }
    ,{ value:"Text",          display:false,   id:'image' }
    ,{ value:"Text",          display:false,   id:'flash' }
    ,{ value:"Text",          display:true,    id:'link' }
    
    ,{ value:"Image",         display:false,   id:'htmlcode' }
    ,{ value:"Image",         display:true,    id:'image' }
    ,{ value:"Image",         display:false,   id:'flash' }
    ,{ value:"Image",         display:false,   id:'link' }
    
    ,{ value:"Flash",         display:false,   id:'htmlcode' }
    ,{ value:"Flash",         display:false,   id:'image' }
    ,{ value:"Flash",         display:true,    id:'flash' }
    ,{ value:"Flash",         display:false,   id:'link' }

]);

//$('positionselect').style.display=-4 == $Category.id ? '':'none';
$('showFloor').style.display=-4 == $Category.id ? '':'none';
initColorSelector("linkcolor",'c_t');
if ($("floor-1")) {
    new checkboxList("floor", "floor-1");
}


//try{
//parent.addAD(ad);

//}
//catch(e)
//{
//    throw e;
//}
//<!--[if !$IsEditMode]-->
clearForm();
function clearForm()
{
    $('title').value="";
    $('code').value="";
    $('text').value="";
    $('imgsrc').value="";
    $('imghref').value="";
    $('alt').value="";
    $('flashsrc').value="";
    $('href').value="";    
}
//<!--[/if]-->



    initDatePicker('begindate','d_s');
    initDatePicker('enddate','d_e');

</script>
<!--[/place]-->
<!--[/dialogmaster]-->