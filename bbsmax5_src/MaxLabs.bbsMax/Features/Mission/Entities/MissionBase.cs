//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;


using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine.Plugin;
using System.Collections;

namespace MaxLabs.bbsMax.Entities
{
    public abstract class MissionBase
    {
        /// <summary>
        /// 类型  任务的唯一标志
        /// </summary>
        public abstract string Type { get; }

        /// <summary>
        /// 如 "帖子类","头像类"
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 任务完成条件的表单对象名称,如没有请返回null
        /// </summary>
        public virtual List<string> InputNames { get { return null; } }


        /// <summary>
        /// 执行任务的步骤描述（支持HTML）,如不需要填写步骤请返回null
        /// </summary>
        public virtual List<string> StepDescriptions { get { return null; } }

        /// <summary>
        /// 是否有步骤描述
        /// </summary>
        public bool HaveStepDescriptions 
        {
            get
            {
                if (StepDescriptions != null && StepDescriptions.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 完成任务条件 的html文件路径 类似：Mission_Topic/html.aspx
        /// </summary>
        public abstract string HtmlPath { get; }


        /// <summary>
        /// 任务完成条件描述  用于前台 如不需要向用户描述 返回null
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual string GetFinishConditionDescription(StringTable values)
        {
            return null;
        }

        /// <summary>
        /// 检查值是否合法
        /// </summary>
        /// <param name="values"></param>
        /// <returns>返回结果</returns>
        public abstract Hashtable CheckValues(StringTable values);


        /// <summary>
        /// 获取任务完成百分比(如果任务失败了，返回任务失败前（即限定时间前）的完成百分比)
        /// </summary>
        /// <param name="cycleTimes">任务周期 单位秒 0为不是周期任务</param>
        /// <param name="beginDate">用户申请任务的时间</param>
        /// <param name="values">完成任务条件对应的值</param>
        /// <param name="isFail">是否失败了,通常是由于超时未完成照成任务失败</param>
        /// <returns></returns>
        public abstract double GetFinishPercent(Mission mission, int userID, int cycleTimes, DateTime beginDate, StringTable values,out bool isFail);

    }

   
}