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
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class PointLog:IPrimaryKey<long>,IFillSimpleUser
    {

        public PointLog() {
            this.Points = new int[8];
            this.CurrentPoints = new int[8];
        }

        public PointLog( DataReaderWrap wrap )
        :this()
        {
            this.LogID = wrap.Get<long>("LogID");
            this.UserID = wrap.Get<int>("UserID");
            this.OperateID = wrap.Get<int>("OperateID");
            this.Remarks = wrap.Get<string>("Remarks");
            this.Points[0] = wrap.Get<int>("Point0");
            this.Points[1] = wrap.Get<int>("Point1");
            this.Points[2] = wrap.Get<int>("Point2");
            this.Points[3] = wrap.Get<int>("Point3");
            this.Points[4] = wrap.Get<int>("Point4");
            this.Points[5] = wrap.Get<int>("Point5");
            this.Points[6] = wrap.Get<int>("Point6");
            this.Points[7] = wrap.Get<int>("Point7");

            this.CurrentPoints[0] = wrap.Get<int>("Current0");
            this.CurrentPoints[1] = wrap.Get<int>("Current1");
            this.CurrentPoints[2] = wrap.Get<int>("Current2");
            this.CurrentPoints[3] = wrap.Get<int>("Current3");
            this.CurrentPoints[4] = wrap.Get<int>("Current4");
            this.CurrentPoints[5] = wrap.Get<int>("Current5");
            this.CurrentPoints[6] = wrap.Get<int>("Current6");
            this.CurrentPoints[7] = wrap.Get<int>("Current7");
            this.CreateTime = wrap.Get<DateTime>("CreateTime");
        }

        public long LogID { get; set; }

        public int UserID { get; set; }

        public int OperateID { get; set; }

        public string Remarks { get; set; }

        public int[] Points { get; private set; }

        public int[] CurrentPoints { get; private set; }

        public DateTime CreateTime { get; set; }

        #region IPrimaryKey<long> Members

        public long GetKey()
        {
            return LogID;
        }

        #endregion

        #region IFillSimpleUser Members

        public int GetUserIDForFill()
        {
            return this.UserID;
        }

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(this.UserID);
            }
        }

        #endregion
    }

    public class PointLogCollection : EntityCollectionBase<long, PointLog>
    {
        public PointLogCollection() { }

        public PointLogCollection( DataReaderWrap wrap ) {
            while (wrap.Next)
            {
                Add(new PointLog(wrap));
            }
        }
    }
}