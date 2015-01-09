<div class="clearfix rankinglayout rankinglayout-priceranking $_if($islogin == false,' rankinglayout-nosidebar ')">
    <div class="rankinglayout-content">
        <div class="rankinglayout-content-inner">
            <div class="panel userstablepanel userstable-pointshow">
                <div class="panel-head">
                    <h3 class="panel-title"><span>竞价排行</span></h3>
                </div>
                <div class="panel-body">
                    <table class="userstable">
                        <thead>
                            <tr>
                                <td class="order">&nbsp;</td>
                                <td class="avatar">&nbsp;</td>
                                <td class="user">用户</td>
                                <td class="price">竞价</td>
                                <td class="action">&nbsp;</td>
                            </tr>
                        </thead>
                        <tbody>
                        <!--[if $userList.count > 0]-->
                        <!--[loop $user in $userList with $i]-->
                            <tr $_if($user.UserID == $My.userID,'class="rankingtablerow-my"')>
                                <td class="order"><strong class="number">$Index</strong></td>
                                <td class="avatar">
                                <a href="$url(space/$user.UserID)?source=show" target="_blank">
                                <img src="$user.AvatarPath" alt="$user.Username" />
                                </a>
                                </td>
                                <td class="user">
                                    <div class="rankinguser-name">
                                        <a href="$url(space/$user.UserID)?source=show" target="_blank">$user.Username</a>
                                        <!--[if $user.Realname!=""]-->
                                        <span class="realname">($user.realname)</span>
                                        <!--[/if]-->
                                        <!--[if $User.Gender == Gender.Male]-->
                                        <img src="$root/max-assets/icon/male.gif" alt="" />
                                        <!--[else if $User.Gender == Gender.Female]-->
                                        <img src="$root/max-assets/icon/female.gif" alt="" />
                                        <!--[/if]-->
                                    </div>
                                    <!--[if $GetUserShowContent($user.UserID) != ""]-->
                                    <div class="rankinguser-speak">
                                        $GetUserShowContent($user.UserID)
                                    </div>
                                    <!--[/if]-->
                                    <!--[if $user.UserID != $My.userID && $islogin]-->
                                    <div class="rankinguser-action">
                                        <!--[if !$My.ismyfriend($user.UserID)]-->
                                        <a class="addnewfriend" href="$dialog/friend-tryadd.aspx?uid=$User.ID" onclick="return openDialog(this.href, function(result){});">加为好友</a>
                                        <!--[/if]-->
                                        <a class="<!--[if $user.isonline]-->chat-online<!--[else]-->chat-offline<!--[/if]-->" href="$dialog/chat.aspx?to=$User.ID" onclick="return openDialog(this.href, function(result){});"><!--[if $user.isonline]-->会话<!--[else]-->发消息<!--[/if]--></a>
                                    </div>
                                    <!--[/if]-->
                                </td>
                                <td class="price">$GetUserShowPoint($user.UserID)$AddPointName</td>
                                <td class="action">

                                </td>
                            </tr>
                        <!--[/loop]-->
                        <!--[else]-->
                            <tr>
                                <td class="nodata" colspan="5">目前还没有会员参与竞价排行.</td>
                            </tr>
                        <!--[/if]-->
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <!--[if $islogin]-->
    <div class="rankinglayout-sidebar">
        <div class="rankinglayout-sidebar-inner">
        <form action="$_form.action" method="post">
            <div class="panel round-tl joinrankingform">
                <div class="panel-body round-tr">
                    <div class="round-bl"><div class="clearfix round-br">
                    <!--[if $MyPointShowInfo==null]-->
                        <h3 class="formgroup-title">我也要上榜</h3>
                        <!--[if $my.GetPoint($AddPoint.Type) < $MinShowPoint]-->
                        <div class="alertmsg">您目前的$AddPointName为$my.GetPoint($AddPoint.Type)， $AddPointName少于竞价上榜的最低值$MinShowPoint，因此您无法参与竞价排行.</div>
                        <!--[else]-->
                        <!--[unnamederror]-->
                        <div class="errormsg">$message</div>
                        <!--[/unnamederror]-->
                        <!--[error name="price"]-->
                        <div class="errormsg">$message</div>
                        <!--[/error]-->
                        <!--[error name="pointcount"]-->
                        <div class="errormsg">$message</div>
                        <!--[/error]-->
                        <div class="formgroup">
                            <div class="formrow">
                                <h3 class="label"><label for="pointshowOath">上榜宣言</label></h3>
                                <div class="form-enter">
                                    <textarea rows="4" cols="16" name="pointshowOath" id="pointshowOath">$_form.text('pointshowOath')</textarea>
                                </div>
                                <div class="form-note">
                                    最多50个汉字, 文字将显示在榜单中.
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="price">每次点击单价</label></h3>
                                <div class="form-enter">
                                    <input class="text number" name="price" id="price" onkeyup="this.value=this.value.replace(/\D/,'');" value="$_form.text('price')" type="text" />$AddPointName
                                </div>
                                <div class="form-note">
                                    单价越高排名越靠前.
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="pointcount">充值</label></h3>
                                <div class="form-enter">
                                    <input class="text number" type="text" name="pointcount" id="pointcount" value="$_form.text('pointcount')"  onkeyup="this.value=this.value.replace(/\D/,'');" />$AddPointName
                                </div>
                                <div class="form-note">
                                    您当前还有: $my.GetPoint($AddPoint.Type)$AddPointName
                                </div>
                            </div>
                            <div class="formrow formrow-action">
                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="addPointShow" id="moodsubmit_btn" value="确认" class="button" /></span></span>
                            </div>
                        </div>
                        <!--[/if]-->
                    <!--[else]-->
                        <h3 class="formgroup-title">我的排名 $MyPointShowInfo.Rank</h3>                      
                        <!--[unnamederror]-->
                        <div class="errormsg">$message</div>
                        <!--[/unnamederror]-->
                        <!--[error name="price"]-->
                        <div class="errormsg">$message</div>
                        <!--[/error]-->
                        <!--[error name="pointcount"]-->
                        <div class="errormsg">$message</div>
                        <!--[/error]-->
                        <div class="formgroup">
                            <div class="formrow">
                                <h3 class="label"><label for="pointshowOath">上榜宣言</label></h3>
                                <div class="form-enter">
                                    <textarea rows="4" cols="16" name="pointshowOath" id="pointshowOath">$_form.text('pointshowOath', $MyPointShowInfo.Content)</textarea>
                                </div>
                                <div class="form-note">
                                    最多50个汉字, 文字将显示在榜单中.
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="price">单次点击价格</label></h3>
                                <div class="form-enter">
                                    <input class="text number" name="price" onkeyup="this.value=this.value.replace(/\D/,'');" id="price" type="text" value="$_form.text('price', $MyPointShowInfo.Price)" /> $AddPointName
                                </div>
                                <div class="form-note">
                                    单价越高排名越靠前.
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="pointadd">充值</label></h3>
                                <div class="form-enter">
                                    <input class="text number" type="text" name="pointadd" id="pointadd" onkeyup="this.value=this.value.replace(/\D/,'');" /> $AddPointName
                                </div>
                                <div class="form-note">
                                    您当前还有: $my.GetPoint($AddPoint.Type)$AddPointName <br />
                                    竞价账户剩余:$MyPointShowInfo.ShowPoints$AddPointName
                                </div>
                            </div>
                            <div class="formrow formrow-action">
                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="updatepointshow" id="updatepointshow" value="更新" class="button" /></span></span>
                            </div>
                        </div>
                        
                    <!--[/if]-->
                    </div></div>
                </div>
            </div>
            <%--
            <div class="panel round-tl joinrankingform">
                <div class="panel-body round-tr">
                    <div class="round-bl"><div class="clearfix round-br">
                        <h3 class="formgroup-title">帮助好友上榜</h3>
                        <div class="formgroup">
                            <div class="formrow">
                                <h3 class="label"><label for="">好友</label></h3>
                                <div class="form-enter">
                                    <input class="text" type="text" id="friendname" name="friendname" value="$_form.text('friendname')" />
                                </div>
                                <div class="dropdownmenu-wrap myfriends-dropdown" style="xxxxxxxxxxxxxxdisplay:none;">
                                    <div class="dropdownmenu">
                                        <ul class="dropdownmenu-list">
                                            <li><a href="#">Friend1</a></li>
                                            <li><a href="#">Friend1</a></li>
                                            <li><a href="#">Friend1</a></li>
                                            <li><a href="#">Friend1</a></li>
                                            <li><a href="#">Friend1</a></li>
                                            <li><a href="#">Friend1</a></li>
                                            <li><a href="#">Friend1</a></li>
                                            <li><a href="#">Friend1</a></li>
                                            <li><a href="#">Friend1</a></li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="">赠送竞价积分</label></h3>
                                <div class="form-enter">
                                    <input class="text number" type="text" /> 金钱
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="">每次点击扣除</label></h3>
                                <div class="form-enter">
                                    <input class="text number" type="text" /> 金钱
                                </div>
                                <div class="form-note">
                                    您当前金钱: $my.GetPoint($AddPoint.Type)
                                </div>
                            </div>
                            <div class="formrow formrow-action">
                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="addFriendPointShow" id="Submit1" value="确认" class="button" /></span></span>
                            </div>
                        </div>
                    </div></div>
                </div>
            </div>
            --%>
        </form>
        </div>
    </div>
    <!--[/if]-->
</div>
<!--[pager name="list" skin="../_inc/_pager_app.aspx"]-->