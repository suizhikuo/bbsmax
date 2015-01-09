<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/setting.css" />
<link rel="stylesheet" type="text/css" href="$root/max-assets/kindeditor/skins/editor5.css" />
<script type="text/javascript">
    function sinput(s) {
        setStyle($("changename"), { display: s ? '' : 'none' });
        setStyle($("showname"), { display: s ? 'none' : '' });
        return false;
    }
</script>
</head>
<body>
<div class="container section-setting">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main">
        <div class="clearfix main-inner">
            <div class="content">
                <!--#include file="../_inc/_round_top.aspx"-->
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <div class="content-main-inner">

                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span>修改资料</span></h3>
                            </div>
                            <div class="formcaption">
                                <h3>个人资料</h3>
                                <p>填写完善的个人信息, 可以帮助用户更容易找到你.</p>
                            </div>
                            <!--[if $EmptyRequiredFieldList.Count > 0]-->
                            <div class="setting-message">
                                <h3>你有尚未填写的必填资料项或者必填项填写有误. 请认真完善各项必填资料.</h3>
                                <p>
                                    以下为你当前尚未完善的必填项:<br />
                                    <!--[loop $field in $EmptyRequiredFieldList with $i]-->
                                    $field<!--[if $i < $EmptyRequiredFieldList.Count - 1]-->, <!--[/if]-->
                                    <!--[/loop]-->
                                </p>
                            </div>
                            <!--[/if]-->
                            
                            <!--[if $_get.success == "1"]-->
                            <div class="successmsg" id="successmsg">你已成功更新个人资料.</div>
                            <!--[/if]-->
                            
                            <!--[unnamederror]-->
                            <div class="errormsg">$Message</div>
                            <!--[/unnamederror]-->
                            
                            <!--[UserEditForm]-->
                            <form action="$_form.action" method="post">
                            <div class="formgroup">
                                <div class="formrow">
                                    <label class="label" for="">用户名</label>
                                    <div class="form-enter">
                                        <input type="text" class="text" value="$My.Username" disabled="disabled" />
                                    </div>
                                </div>
                                <div class="formrow">
                                    <label class="label">性别</label>
                                    <div class="form-enter">
                                        <input type="radio" value="0" name="gender" $_form.checked('gender','0',$My.Gender==MaxLabs.bbsMax.Enums.Gender.NotSet) id="gender0" />
                                        <label for="gender0">保密</label>
                                        <input type="radio" value="1" name="gender" $_form.checked('gender','1',$My.Gender==MaxLabs.bbsMax.Enums.Gender.Male) id="gender1" />
                                        <label for="gender1">男</label>
                                        <input type="radio" value="2" name="gender" $_form.checked('gender','2',$My.Gender==MaxLabs.bbsMax.Enums.Gender.Female) id="gender2" />
                                        <label for="gender2">女</label>
                                    </div>
                                </div>
                                <div class="formrow">
                                    <label class="label" for="timezone">所在地时区</label>
                                    <div class="form-enter">
                                        <select name="timezone" id="timezone" style="width:15em;">
                                        <option value="-12.0" $_form.selected("timezone",'-12.0',$my.timezone==-12.0f)>(GMT -12:00) 安尼威土克、瓜甲兰</option>
                                        <option value="-11.0" $_form.selected("timezone",'-11.0',$my.timezone==-11.0f)>(GMT -11:00) 中途岛、萨摩亚群岛</option>
                                        <option value="-10.0" $_form.selected("timezone",'-10.0',$my.timezone==-10.0f)>(GMT -10:00) 夏威夷</option>
                                        <option value="-09.0" $_form.selected("timezone",'-09.0',$my.timezone==-9.0f)>(GMT -09:00) 阿拉斯加</option>
                                        <option value="-08.0" $_form.selected("timezone",'-08.0',$my.timezone==-8.0f)>(GMT -08:00) 太平洋时间（美国和加拿大），蒂华纳</option>
                                        <option value="-07.0" $_form.selected("timezone",'-07.0',$my.timezone==-7.0f)>(GMT -07:00) 山区时间(美加)、亚利桑那</option>
                                        <option value="-06.0" $_form.selected("timezone",'-06.0',$my.timezone==-6.0f)>(GMT -06:00) 中部时间（美国和加拿大），特古西加尔巴，萨斯喀彻温省，墨西哥城、塔克西卡帕</option>
                                        <option value="-05.0" $_form.selected("timezone",'-05.0',$my.timezone==-5.0f)>(GMT -05:00) 东部时间（美国和加拿大）、波哥大、利马、基多</option>
                                        <option value="-04.0" $_form.selected("timezone",'-04.0',$my.timezone==-4.0f)>(GMT -04:00) 大西洋时间（加拿大）委内瑞拉、拉巴斯</option>
                                        <option value="-03.5" $_form.selected("timezone",'-03.5',$my.timezone==-3.5f)>(GMT -03:30) 新岛(加拿大东岸) 纽芬兰</option>
                                        <option value="-03.0" $_form.selected("timezone",'-03.0',$my.timezone==-3.0f)>(GMT -03:00) 东南美洲 波西尼亚 布鲁诺斯爱丽斯、乔治城</option>
                                        <option value="-02.0" $_form.selected("timezone",'-02.0',$my.timezone==-2.0f)>(GMT -02:00) 大西洋中部</option>
                                        <option value="-01.0" $_form.selected("timezone",'-01.0',$my.timezone==-1.0f)>(GMT -01:00) 亚速尔群岛，佛得角群岛</option>
                                        <option value="000.0" $_form.selected("timezone",'000.0',$my.timezone==0.0f)>(GMT 0:00) 格林威治标准时间 ：伦敦，都柏林，爱丁堡，里斯本，卡萨布兰卡，蒙罗维亚，英国夏令</option>
                                        <option value="+01.0" $_form.selected("timezone",'+01.0',$my.timezone==1.0f)>(GMT +01:00) 荷兰，斯洛伐克，斯洛文尼亚，法国，保加利亚，波兰，克罗地亚</option>
                                        <option value="+02.0" $_form.selected("timezone",'+02.0',$my.timezone==2.0f)>(GMT +02:00) 布加勒斯特，哈拉雷，南非，比勒陀尼亚，埃及，赫尔辛基，里加，塔林，明斯克，以色列</option>
                                        <option value="+03.0" $_form.selected("timezone",'+03.0',$my.timezone==3.0f)>(GMT +03:00) 沙乌地阿拉伯、俄罗斯 利雅得，莫斯科，圣彼得堡，伏尔加格勒，内罗毕</option>
                                        <option value="+03.5" $_form.selected("timezone",'+03.5',$my.timezone==3.5f)>(GMT +03:30) 伊朗</option>
                                        <option value="+04.0" $_form.selected("timezone",'+04.0',$my.timezone==4.0f)>(GMT +04:00) 阿布扎比，马斯喀特，巴库、第比利斯、阿布达比、莫斯凯、塔布理斯、阿拉伯</option>
                                        <option value="+04.5" $_form.selected("timezone",'+04.5',$my.timezone==4.5f)>(GMT +04:30) 阿富汗</option>
                                        <option value="+05.0" $_form.selected("timezone",'+05.0',$my.timezone==5.0f)>(GMT +05:00) 西亚 : 叶卡特琳堡、卡拉奇、塔什干、伊斯兰马巴德、克洛奇、伊卡特林堡</option>
                                        <option value="+05.5" $_form.selected("timezone",'+05.5',$my.timezone==5.5f)>(GMT +05:30) 印度 : 孟买，加尔各答，马德拉斯，新德里</option>
                                        <option value="+06.0" $_form.selected("timezone",'+06.0',$my.timezone==6.0f)>(GMT +06:00) 中亚 : 阿拉木图、科伦坡、阿马提、达卡</option>
                                        <option value="+07.0" $_form.selected("timezone",'+07.0',$my.timezone==7.0f)>(GMT +07:00) 曼谷，河内，雅加达</option>
                                        <option value="+08.0" $_form.selected("timezone",'+08.0',$my.timezone==8.0f)>(GMT +08:00) 中国 : 北京，重庆，广州，上海，香港，台北，新加坡</option>
                                        <option value="+09.0" $_form.selected("timezone",'+09.0',$my.timezone==9.0f)>(GMT +09:00) 平壤，汉城，东京，大阪，札幌，雅库茨克</option>
                                        <option value="+09.5" $_form.selected("timezone",'+09.5',$my.timezone==9.5f)>(GMT +09:30) 澳洲中部</option>
                                        <option value="+10.0" $_form.selected("timezone",'+10.0',$my.timezone==10.0f)>(GMT +10:00) 西太平洋 : 席德尼、塔斯梅尼亚、关岛，莫尔兹比港，霍巴特，堪培拉，悉尼</option>
                                        <option value="+11.0" $_form.selected("timezone",'+11.0',$my.timezone==11.0f)>(GMT +11:00) 太平洋中部 : 马加丹，所罗门群岛，新喀里多尼亚</option>
                                        <option value="+12.0" $_form.selected("timezone",'+12.0',$my.timezone==12.0f)>(GMT +12:00) 纽芬兰 : 威灵顿、斐济，堪察加半岛，马绍尔群岛</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="formrow">
                                    <label class="label">生日</label>
                                    <div class="form-enter">
                                        <select id="birthyear" name="birthyear" style="width:6em;">
                                        <option value="0">-</option>
                                        <!--[loop $var0 in $Years]-->
                                        <option value="$var0" $_form.selected("birthyear","$var0",$my.birthday.year==$var0)>$var0</option>
                                        <!--[/loop]-->
                                        </select> 年
                                        <select id="birthmonth" name="birthmonth" style="width:4em;">
                                        <option value="0">-</option>
                                        <!--[loop $var1 in $months]-->
                                        <option value="$var1" $_form.selected("birthmonth","$var1",$my.birthday.month==$var1)>$var1</option>
                                        <!--[/loop]-->
                                        </select> 月
                                        <select id="birthday" name="birthday" style="width:4em;">
                                        <option value="0">-</option>
                                        <!--[loop $var2 in $days]-->
                                        <option value="$var2" $_form.selected("birthday","$var2",$my.birthday.day==$var2)>$var2</option>
                                        <!--[/loop]-->
                                        </select> 日
                                    </div>
                                </div>
                                <!--[ExtendedFieldList]-->
                                <!--[if $IsHidden == false]-->
                                <div class="formrow">
                                    <h3 class="label"><label>$Name</label> $_if($IsRequired, '<span class="require" title="必填项">*</span>')</h3>
                                    <div class="form-enter">
                                        <!--[load src="$fieldType.FrontendControlSrc" value="$_if($_post[$Key] != null, $HtmlEncode($_post[$Key]), $userValue)" field="$_this" /]-->
                                        <!--[if $DisplayType == ExtendedFieldDisplayType.AllVisible]-->
                                        (所有人可见)
                                        <!--[else if $DisplayType == ExtendedFieldDisplayType.FriendVisible]-->
                                        (仅您的好友可见)
                                        <!--[else if $DisplayType == ExtendedFieldDisplayType.SelfVisible]-->
                                        (仅您自己可见)
                                        <!--[else]-->
                                        <select name="{=$Key}_displaytype">
                                        <option value="0" $_form.selected("{=$Key}_displaytype","0",$PrivacyType)>所有人可见</option>
                                        <option value="1" $_form.selected("{=$Key}_displaytype","1",$PrivacyType)>仅好友可见</option>
                                        <option value="2" $_form.selected("{=$Key}_displaytype","2",$PrivacyType)>仅自己可见</option>
                                        </select>
                                        <!--[/if]-->
                                    </div>
                                    <!--[error name="$Key"]-->
                                    <p class="form-tip tip-error">$message</p>
                                    <!--[/error]-->
                                    <!--[if $Description != ""]-->
                                    <div class="form-note">$Description</div>
                                    <!--[/if]-->
                                </div>
                                <!--[/if]-->
                                <!--[/ExtendedFieldList]-->
                                <div class="formrow">
                                    <label class="label">个性签名</label>
                                    <div class="form-enter">
                                    <!--[if $ImportEditor]-->
                                        <!--[load src="../_inc/_editor_.aspx" width="500px" height="200"  usemaxcode="{=!$SignatureAllowHtml}" id="signature" mode="signature" value="$ParsedSignature" /]-->
                                        <script type="text/javascript">
                                            KE.g["signature"].panelSize = { width: 320, height: 200 };
                                        </script>
                                    <!--[else]-->
                                        <textarea name="signature" style="width:500px;height:200px;">$_form.text("signature",$ParsedSignature)</textarea>
                                    <!--[/if]-->
                                    </div>
                                </div>
                                <div class="formrow formrow-action">
                                    <span class="minbtn-wrap"><span class="btn"><input type="submit" value="提交" class="button" name="save" /></span></span>
                                </div>
                            </div>
                            </form>
                            <!--[/UserEditForm]-->
                            
                        </div>
                    </div>
                    <!--#include file="_sidebar_setting.aspx"-->
                </div>
                <!--#include file="../_inc/_round_bottom.aspx"-->
            </div>
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>
</body>
</html>
