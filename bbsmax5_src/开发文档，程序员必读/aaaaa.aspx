<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" action="$_form.action" method="post">  
    <h3>基本的变量函数调用</h3>
    <div style="width:{=$width}px">  <!-- 如果变量名和html代码混在一起时候 把变量用 {   }括起来 -->

        你好 $my.username <!--输出对象属性 -->
        <br />
        今天是$Weekday   <!-- 输出后台变量 -->                    
        <br />
        随便产生一个数字 $GetRndNumber(100,$rndmax)  <!-- 模板函数调用,分别传递常量和变量的参数 -->
    </div>

    <h3>模板里的判断、循环</h3>
    <div style="width:500px">
    <!--[if $rndmax > 5000]-->        <!-- if 判断 -->
         <div>好大的数字啊</div>
    <!--[else if $rndmax < 1000]-->   <!-- else if判断 -->
         <div>好小的数字</div>
    <!--[else]-->                     <!-- else 判断 -->
         <div>错误的数字</div>
    <!--[/if]-->                      <!-- 闭合else -->
    </div>
    </form>
</body>
</html>
