//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_admin.other
{
    public partial class manage_prop_detail : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return BackendPermissions.Action.Manage_Prop;
            }
        }

        private Prop m_Prop;

        public Prop Prop
        {
            get { return m_Prop; }
        }

        private PropType m_PropType;

        public PropType PropType
        {
            get { return m_PropType; }
        }

        public string PropParamFormHtml
        {
            get { return m_PropType.GetPropParamFormHtml(Request, m_Prop.PropParam); }
        }

        private bool m_IsEdit;

        public bool IsEdit
        {
            get { return m_IsEdit; }
        }

        private RoleCollection m_AllRoleList;

        protected RoleCollection AllRoleList
        {
            get
            {
                if (m_AllRoleList == null)
                {
                    m_AllRoleList = AllSettings.Current.RoleSettings.GetRoles(Role.Everyone, Role.Guests);
                }
                return m_AllRoleList;
            }
        }

        protected string GetPointString(Prop prop, int index)
        {
            if (prop == null)
                return string.Empty;
            if (prop.BuyCondition == null)
                return string.Empty;

            if (index == -1)
            {
                if (prop.BuyCondition.TotalPoint == int.MinValue)
                    return string.Empty;
                else
                    return prop.BuyCondition.TotalPoint.ToString();
            }

            if (prop.BuyCondition.Points[index] == int.MinValue)
                return string.Empty;
            else
                return prop.BuyCondition.Points[index].ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int? id = _Request.Get<int>("id");

            m_IsEdit = id != null;

            if (m_IsEdit)
            {
                this.m_Prop = PropBO.Instance.GetPropByID(id.Value);

                this.m_PropType = PropBO.GetPropType(this.m_Prop.PropType);
            }
            else
            {
                this.m_Prop = new Prop();

                this.m_PropType = PropBO.GetPropType(_Request.Get("proptype"));
            }

            if (m_PropType == null)
                ShowError("道具类型不存在");

            if (_Request.IsClick("save"))
            {
                MessageDisplay md = CreateMessageDisplay(
                    "icon", "name", "description", 
                    "price", "pricetype", 
                    "packagesize", "totalnumber", "allowexchange", 
                    "autoreplenish", "replenishnumber", "replenishtimespan"
                );

                this.m_Prop.Icon = _Request.Get("icon");

                //默认为猪头卡的图片
                if (string.IsNullOrEmpty(this.m_Prop.Icon))
                {
                   this.m_Prop.Icon="~/max-assets/icon-prop/4.gif";
                }

                this.m_Prop.Name = _Request.Get("name");

                if (string.IsNullOrEmpty(this.m_Prop.Name))
                {
                    md.AddError("name", "道具名称不能为空");
                }

                this.m_Prop.Description = _Request.Get("description");

                if (string.IsNullOrEmpty(this.m_Prop.Description))
                {
                    md.AddError("description", "道具描述不能为空");
                }

                int? sortOrder = _Request.Get<int>("SortOrder");

                if(sortOrder == null)
                {
                    md.AddError("SortOrder", "道具排序必须填写");
                }
                else if(sortOrder.Value < 0)
                {
                    md.AddError("SortOrder", "道具排序必须大于或等于0");
                }
                else
                {
                    this.m_Prop.SortOrder = sortOrder.Value;
                }

                int? price = _Request.Get<int>("price");

                if (price == null)
                {
                    md.AddError("price", "道具价格必须填写");
                }
                else if (price.Value <= 0)
                    md.AddError("price", "道具价格必须大于0");
                else
                    this.m_Prop.Price = price.Value;

                if(m_IsEdit == false)
                    this.m_Prop.PropType = _Request.Get("proptype");

                this.m_Prop.PropParam = m_PropType.GetPropParam(Request);

                int? priceType = _Request.Get<int>("pricetype");

                if (priceType == null)
                {
                    md.AddError("pricetype", "道具售价类型不能为空");
                }
                else
                    this.m_Prop.PriceType = priceType.Value;

                int? packageSize = _Request.Get<int>("packagesize");

                if (packageSize == null)
                {
                    md.AddError("packagesize", "道具重量必须填写");
                }
                else
                    this.m_Prop.PackageSize = packageSize.Value;

                int? totalNumber = _Request.Get<int>("totalnumber");

                if (totalNumber == null)
                {
                    md.AddError("totalnumber", "道具总数必须填写");
                }
                else if(totalNumber <= 0)
                {
                    md.AddError("totalnumber", "道具总数必须大于0");
                }
                else
                    this.m_Prop.TotalNumber = totalNumber.Value;

                bool? allowExchange = _Request.Get<bool>("allowexchange");

                if (allowExchange == null)
                {
                    md.AddError("allowexchange", "道具是否允许出售和赠送必须设置");
                }
                else
                    this.m_Prop.AllowExchange = allowExchange.Value;

                bool? autoReplenish = _Request.Get<bool>("autoreplenish");

                if (autoReplenish == null)
                {
                    md.AddError("autoreplenish", "道具是否自动补货必须设置");
                }
                else
                    this.m_Prop.AutoReplenish = autoReplenish.Value;

                int? replenishLimit = _Request.Get<int>("ReplenishLimit");

                if(replenishLimit == null)
                {
                    md.AddError("ReplenishLimit", "道具补货阀值必须设置");
                }
                else if(replenishLimit.Value < 0)
                {
                    md.AddError("ReplenishLimit", "道具补货阀值必须大于等于0");
                }
                else
                    this.m_Prop.ReplenishLimit = replenishLimit.Value;

                int? replenishNumber = _Request.Get<int>("replenishnumber");

                if (replenishNumber == null)
                {
                    md.AddError("replenishnumber", "道具自动补货数量必须填写");
                }
                else
                    this.m_Prop.ReplenishNumber = replenishNumber.Value;
                
                int? replenishTimespan = _Request.Get<int>("replenishtimespan");

                if (replenishTimespan == null)
                {
                    md.AddError("replenishtimespan", "道具自动补货周期必须设置");
                }
                else
                    this.m_Prop.ReplenishTimeSpan = replenishTimespan.Value;

                BuyPropCondition condition = new BuyPropCondition();

                this.m_Prop.BuyCondition = condition;

                condition.UserGroupIDs = StringUtil.Split2<Guid>(_Request.Get("BuyCondition.groups", Method.Post, string.Empty));

                int? totalPoint = _Request.Get<int>("BuyCondition.totalPoint");

                if (totalPoint != null && totalPoint.Value > 0)
                    condition.TotalPoint = totalPoint.Value;

                UserPointCollection allPoints = AllSettings.Current.PointSettings.UserPoints;

                int[] points = new int[allPoints.Count];

                for (int i = 0; i < points.Length; i++)
                {
                    UserPoint point = allPoints[i];

                    if (point.Enable)
                    {
                        int? value = _Request.Get<int>("BuyCondition." + point.Type);

                        if (value != null)
                            points[i] = value.Value;
                        else
                            points[i] = 0;
                    }
                    else
                    {
                        points[i] = 0;
                    }
                }

                condition.Points = points;

                int? totalPosts = _Request.Get<int>("BuyCondition.totalPosts");

                if (totalPosts != null && totalPosts.Value > 0)
                    condition.TotalPosts = totalPosts.Value;

                int? onlineTime = _Request.Get<int>("BuyCondition.onlinetime");

                if (onlineTime != null && onlineTime.Value > 0)
                    condition.OnlineTime = onlineTime.Value;

                condition.ReleatedMissionIDs = StringUtil.Split2<int>(_Request.Get("BuyCondition.releatedmissionids", Method.Post, string.Empty));

                if (md.HasAnyError())
                    return;

                using (ErrorScope es = new ErrorScope())
                {
                    if(m_IsEdit)
                    {
                        PropBO.Instance.UpdateProp(
                            m_Prop.PropID,
                            m_Prop.Icon,
                            m_Prop.Name,
                            price.Value,
                            priceType.Value,
                            m_Prop.PropType,
                            m_Prop.PropParam,
                            m_Prop.Description,
                            packageSize.Value,
                            totalNumber.Value,
                            allowExchange.Value,
                            autoReplenish.Value,
                            replenishNumber.Value,
                            replenishTimespan.Value,
                            replenishLimit.Value,
                            condition,
                            sortOrder.Value
                        );
                    }
                    else
                    {
                        PropBO.Instance.CreateProp(
                            m_Prop.Icon,
                            m_Prop.Name,
                            price.Value,
                            priceType.Value,
                            m_Prop.PropType,
                            m_Prop.PropParam,
                            m_Prop.Description,
                            packageSize.Value,
                            totalNumber.Value,
                            allowExchange.Value,
                            autoReplenish.Value,
                            replenishNumber.Value,
                            replenishTimespan.Value,
                            replenishLimit.Value,
                            condition,
                            sortOrder.Value
                        );
                    }

                    if (es.HasError)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            md.AddError(error);
                        });
                    }
                    else
                        JumpTo("interactive/manage-prop.aspx?page=" + _Request.Get("page"));
                }
            }
        }
    }
}