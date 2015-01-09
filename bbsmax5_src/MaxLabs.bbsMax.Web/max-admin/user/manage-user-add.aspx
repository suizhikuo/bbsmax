<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>添加用户</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->

<!--[unnamederror]-->
<div class="Tip Tip-error">$Message</div>
<!--[/unnamederror]-->
<!--[success]-->
<div class="Tip Tip-success">添加用户成功</div>
<!--[/success]-->

<div class="Content">
    <div class="PageHeading">
        <h3>添加用户</h3>
        <div class="ActionsBar">
        <a href="$admin/user/manage-user.aspx" class="back"><span>返回用户管理</span></a>
        </div>
    </div>

    <form action="$_form.action" method="post">
    <!--[do:bbsmax:AdminCreateUser/]-->
    <div class="FormTable">
        <table>
            <tr>
                <th>
                <h4>用户名</h4>
                <input id="username" name="username" type="text" class="text" />
                </th>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <th>
                <h4>强制使用ID</h4>
                    <input id="userid" name="userid" type="text" maxlength="7" class="text number" onkeyup="value=value.replace(/[^\d]/g,'');" />
                    <a href="$dialog/user-notuseids.aspx" onclick="return openDialog(this.href, function(result){});">查看未被使用的UserID</a>
                </th>
                <td>只有当该用户ID不被占用时,不填则使用默认.</td>
            </tr>
            <tr>
                <th>
                <h4>密码</h4>
                <input name="password" id="password" type="text" class="text" />
                </th>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <th>
                <h4>Email</h4>
                <input id="email" name="email" type="text" class="text" />
                </th>
                <td>&nbsp;</td>
            </tr>
            
            <tr class="nohover">
                <th><input type="submit" class="button" name="addUser" value="添加" /></th>
                <td>&nbsp;</td>
            </tr>
        </table>
    </div>
    </form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
