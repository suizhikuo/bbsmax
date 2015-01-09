<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<title>BBSMAX V5</title>
<!--#include file="_htmlmeta.aspx"-->
</head>
<body>
<div class="container">
    <!--#include file="_header.aspx"-->
    
    <div>
        <a href="list.aspx">默认版块</a>
        <a href="publish.aspx">发帖</a>
    </div>
    
    <section class="main hfeed viewtopic">
        <article class="hentry">
            <div class="vcard author">
                <a class="url" href="#">
                    <img class="photo" src="../max-assets/avatar/avatar_48.gif" alt="" width="48" height="48">
                    <span class="fn nickname">Author</span>
                </a>
            </div>
            <div class="post-info">
                <a class="post-author" href="#">AuthorAuthor</a>
                <span class="pulished">2010-08-11</span>
                <span class="post-number">#1</span>
            </div>
            <h1 class="entry-title">主题标题主题标题TOPIC HEADING</h1>
            
            <!-- #include file="_postalert.aspx" -->
            
            <div class="entry-content">
                <p>Lorem ipsum dolor sit amet consectetuer porta pulvinar sem dis adipiscing. Proin risus pretium nec velit felis laoreet urna dui vel magna. Egestas et nunc Suspendisse Ut lacinia pretium Fusce eros id pellentesque. </p>
                <p>Eget tortor magna laoreet pede egestas elit semper tincidunt pharetra velit. Elit pretium orci eget ac consequat Phasellus pulvinar orci sodales ut. Nam turpis semper urna vel wisi Vivamus egestas tempus Cum iaculis.</p>
                <p>搞个调查,开启gzip压缩网页,压缩比可以达到70%以上,很明显的会加快网速的,很显然会加强用户体验度,但是有人讲开启了这个会影响百度收录甚至权重,对google到是没影响,google技术好嘛,能完全识别.大家知道了,有些信息网上都人云亦云,不知真假</p>
            </div>

            <!-- #include file="_postelements.aspx" -->

        </article>
        <article class="hentry">
            <div class="vcard author">
                <a class="url" href="#">
                    <img class="photo" src="../max-assets/avatar/avatar_48.gif" alt="" width="48" height="48">
                    <span class="fn nickname">Author</span>
                </a>
            </div>
            <div class="post-info">
                <a class="post-author" href="#">Author</a>
                <span class="pulished">2010-08-11</span>
                <span class="post-number">#1</span>
            </div>
            <h1 class="entry-title">提问帖</h1>
            
            <!-- #include file="_postalert.aspx" -->

            <div class="askpost-endtime">
                <span class="label">截止时间:</span>
                <span class="date">无限期</span>
            </div>

            <div class="askpost-reward">
                <p>
                    悬赏QuestionPoint.Name: <strong class="value">questionThread.Reward</strong> QuestionPoint.UnitName
                </p>
                <p>
                    最多奖励给<strong class="value">questionThread.RewardCount</strong>个回复
                </p>
            </div>
            
            <div class="entry-content">
                <p>Lorem ipsum dolor sit amet consectetuer porta pulvinar sem dis adipiscing. Proin risus pretium nec velit felis laoreet urna dui vel magna. Egestas et nunc Suspendisse Ut lacinia pretium Fusce eros id pellentesque. </p>
            </div>

            <!-- #include file="_postelements.aspx" -->

        </article>
        
        <article class="hentry">
            <div class="vcard author">
                <a class="url" href="#">
                    <img class="photo" src="../max-assets/avatar/avatar_48.gif" alt="" width="48" height="48">
                    <span class="fn nickname">Author</span>
                </a>
            </div>
            <div class="post-info">
                <a class="post-author" href="#">Author</a>
                <span class="pulished">2010-08-11</span>
                <span class="post-number">#1</span>
            </div>
            <h1 class="entry-title">投票帖</h1>

            <!-- include file="_postalert.aspx" -->

            <div class="pollpost-type">
                <strong>投票项</strong> (VoteType)
            </div>
            <div class="pollpost-stats">
                共 XXX 人参与, 总票数 XXX
            </div>
            <div class="pollpost-endtime">
                <span class="label">截止时间</span>
                <span class="date">无限期</span>
            </div>

            <ol class="pollentry">
                <li>
                    <span class="checkbox">
                        <input type="checkbox" name="pollItem" id="poll_loopIndex" value="pollItem.ItemID" _form.checked("pollItem",?pollItem.ItemID)>
                    </span>
                    <label class="pollchart pollchart-style" for="poll_loopIndex">
                        <span class="chart-figure"><span class="chart-index" style="width:50%;">50%</span></span>
                    </label>
                    <label class="title" for="poll_loopIndex">pollItem.ItemName <span class="chart-status">50% (XXX票)</span></label>
                </li>
            </ol>

            <div class="entry-content">
                <p>Lorem ipsum dolor sit amet consectetuer porta pulvinar sem dis adipiscing.</p>
            </div>

        </article>

        <article class="hentry">
            <div class="vcard author">
                <a class="url" href="#">
                    <img class="photo" src="../max-assets/avatar/avatar_48.gif" alt="" width="48" height="48">
                    <span class="fn nickname">Author</span>
                </a>
            </div>
            <div class="post-info">
                <a class="post-author" href="#">Author</a>
                <span class="pulished">2010-08-11</span>
                <span class="post-number">#2</span>
            </div>
            <div class="entry-content">
                <p>回复 Lorem ipsum dolor sit amet consectetuer porta pulvinar sem dis adipiscing. Proin risus pretium nec velit felis laoreet urna dui vel magna. </p>
            </div>
        </article>

        <article class="hentry">
            <div class="vcard author">
                <a class="url" href="#">
                    <img class="photo" src="../max-assets/avatar/avatar_48.gif" alt="" width="48" height="48">
                    <span class="fn nickname">Author</span>
                </a>

                <span>正方</span>

            </div>
            <div class="post-info">
                <a class="post-author" href="#">Author</a>
                <span class="pulished">2010-08-11</span>
                <span class="post-number">#3</span>
            </div>
            <div class="entry-content">
                <p>辩论回复</p>
            </div>
        </article>

        <article class="hentry">
            <div class="vcard author">
                <a class="url" href="#">
                    <img class="photo" src="../max-assets/avatar/avatar_48.gif" alt="" width="48" height="48">
                    <span class="fn nickname">Author</span>
                </a>

                <span>答案</span>

            </div>
            <div class="post-info">
                <a class="post-author" href="#">Author</a>
                <span class="pulished">2010-08-11</span>
                <span class="post-number">#4</span>
            </div>
            <div class="entry-content">
                <p>问答回复</p>
            </div>
        </article>




        
        
        <div class="pagination">
            <a href="#">上一页</a>
            <a href="#">1</a> <a href="#">2</a> <a href="#">3</a> <a href="#">4</a>
            <a href="#">...122</a>
            <a href="#">下一页</a>
        </div>
    </section>

    <!--#include file="_footer.aspx"-->
</div>
</body>
</html>