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

namespace Max.Installs
{
    //���ȹ���
    public class Progress
    {
        static Progress()
        {
            Notset = new Progress(string.Empty, 0, 0);
            //Notset.isNotSet = true;
        }

        public Progress(string title,int percent,int step):this(title,percent,step,null)
        {
        }

        public Progress(string title, int percent, int step,string error)
        {
            this.title = title;
            this.percent = percent;
            this.step = step;
            this.error = error;
        }

        public static Progress Notset;
        //private bool isNotSet = false;

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
        }

        private int percent;
        public int Percent
        {
            get
            {
                return percent;
            }
        }

        private int step;
        public int Step
        {
            get
            {
                return step;
            }
        }

        private string error;
        public string Error
        {
            get
            {
                return this.error;
            }
        }
    }
}