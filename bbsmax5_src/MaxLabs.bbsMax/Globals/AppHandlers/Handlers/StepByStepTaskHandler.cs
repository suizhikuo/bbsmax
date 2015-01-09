//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.StepByStepTasks;

namespace MaxLabs.bbsMax.AppHandlers
{
    public class StepByStepTaskHandler : IAppHandler
    {

        public IAppHandler CreateInstance()
        {
            return new StepByStepTaskHandler();
        }

        public string Name
        {
            get { return "steptask"; }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            int userID = UserBO.Instance.GetCurrentUserID();

            string taskIDText = context.Request.QueryString["TaskID"];

            string frameIndexText = context.Request.QueryString["i"];

            if (string.IsNullOrEmpty(taskIDText))
                return;

            Guid taskID;
            int frameIndex;

            try
            {
                taskID = new Guid(taskIDText);
                frameIndex = int.Parse(frameIndexText);
            }
            catch
            {
                return;
            }

            int percent;
            string title;

            if (TaskManager.ExecuteTaskStep(userID, taskID, out percent, out title))
            {
                context.Response.Write(string.Format(@"<script type=""text/javascript"">parent.updateTask({0}, {1}, '{2}');</script>", frameIndex, percent, title.Replace("'", "\'")));
            }
            else
            {
                context.Response.Write(string.Format(@"<script type=""text/javascript"">parent.finishTask({0}, '{1}');</script>", frameIndex, title.Replace("'", "\'")));
            }
        }

    }
}