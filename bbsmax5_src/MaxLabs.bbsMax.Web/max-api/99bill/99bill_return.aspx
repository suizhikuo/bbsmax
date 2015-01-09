<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="99bill_return.aspx.cs" Inherits="MaxLabs.bbsMax.Web.max_api._99bill._9bill_return" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>99bill</title>
</head>
<body>
    <%--以下报告给快钱处理结果，并提供将要重定向的地址--%>
    <result><%= rtnOk %></result><redirecturl><%= rtnUrl %></redirecturl>
</body>
</html>
