//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using MaxLabs.bbsMax.Entities;
using System.Text.RegularExpressions;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class timing : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("add"))
            {
                AddTimeSpan();
            }


        }

        public void AddTimeSpan()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            int scopetype = _Request.Get<int>("timescopetype", Method.Post,0);
            string timespan = _Request.Get("timespan", Method.Post, string.Empty,false);
            DateTime definedate = _Request.Get<DateTime>("definedate", Method.Post, DateTime.MinValue);
            string week = _Request.Get("week", Method.Post,string.Empty,false);
            string month = _Request.Get("month", Method.Post, string.Empty,false);

            int type = _Request.Get<int>("type", Method.Post, 0);

            if (definedate == DateTime.MinValue&&scopetype==0)
            {
                msgDisplay.AddError("指定日期未填!");
                return;
            }

            if (week == string.Empty && scopetype == 2)
            {
                msgDisplay.AddError("未选择星期");
                return;
            }

            if (month == string.Empty && scopetype == 3)
            {
                msgDisplay.AddError("未选择日期");
                return;
            }

            DateTime? ts1=null;
            DateTime? ts2=null;
            ScopeBase scopebase ;

            if (TimeSpanIsInMatch(timespan, ref ts1,ref ts2))
            {
                scopebase = GetScopeTypeClass(scopetype, definedate, week, month, ts1, ts2);
                scopebase.OperetorName = My.Name;
                AddToListByType(scopebase, type);
                ShowSuccess("操作成功", scopebase);
            }
            else
            {
                msgDisplay.AddError("时间范围格式错误,请重新输入!");
            }
        } 
       
        private ScopeBase GetScopeTypeClass(int scopetype,DateTime definedate,string weekday,string monthday,DateTime? ts1,DateTime? ts2)
        { 
             switch (scopetype)
                { 
                 case 0:
                        return new DayScope(definedate,ts1,ts2);
                 case 1:
                        return new ScopeBase(ts1,ts2);
                 case 2:
                        return new WeekScope(weekday,ts1,ts2);
                 case 3:
                        return new MonthScope(monthday,ts1,ts2);
                 default:
                        return null;              
                }
        }


        private bool TimeSpanIsInMatch(string timespan,ref DateTime? ts1,ref DateTime? ts2)
        {
            
            string[] spanlist;

            //时间范围没填,表示全天
            if (timespan == string.Empty)
            {
                return true;
            }

            if (timespan.IndexOf("-") > 0)
            {
                spanlist = timespan.Split('-');
            }
            else
            {
                return false;
            }

            if (spanlist.Length != 2)
                return false;

            foreach (string str in spanlist)
            {
                if (Regex.IsMatch(str.Trim(), @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$"))
                {

                    if (ts1 == null)
                    {
                        ts1 = DateTime.Parse(str);
                        continue;
                    }

                    ts2 = DateTime.Parse(str);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void AddToListByType(ScopeBase scopebase,int type)
        {
            if(type==0)
            {
                AllSettings.Current.SiteSettings.ScopeList.Add(scopebase);
            }
            else
            {
                AllSettings.Current.RegisterSettings.ScopeList.Add(scopebase);
            }

        }

        protected override void CheckForumClosed()
        {
            if (My.IsManager==false)
            {
                base.CheckForumClosed();
            }
        }

    }
}