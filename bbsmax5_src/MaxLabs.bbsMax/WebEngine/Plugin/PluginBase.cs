//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MaxLabs.WebEngine.Plugin
{
    /// <summary>
    /// 插件基类
    /// </summary>
	public abstract class PluginBase : Page
	{
        public string Name
        {
            get;
            internal set;
        }

        public bool Enable
        {
            get;
            internal set;
        }

        public virtual string DisplayName
        {
            get { return Name; }
        }

        public virtual string Description
        {
            get { return string.Empty; }
        }

		/// <summary>
		/// 为当前 HTTP 请求获取 System.Web.HttpRequest 对象。
		/// </summary>
        public new HttpRequest Request
        {
            get 
            {
                return Context.Request;
            }
        }

		/// <summary>
		/// 为当前 HTTP 响应获取 System.Web.HttpResponse 对象。
		/// </summary>
        public new HttpResponse Response
        {
            get
            {
                return Context.Response;
            }
        }

		/// <summary>
		/// 为当前 HTTP 请求获取 System.Web.HttpApplicationState 对象。
		/// </summary>
        public new HttpApplicationState Application
        {
            get
            {
                return Context.Application;
            }
        }

		/// <summary>
		/// 当前请求中的表单数据
		/// </summary>
        public new NameValueCollection Form
        {
            get
            {
                return Context.Request.Form;
            }
        }

		/// <summary>
		/// 用于在一次Http请求调用的不同方法之间传递数据
		/// </summary>
        public new IDictionary Items
        {
            get
            {
                return Context.Items;
            }
        }

        /// <summary>
        /// 重写此方法以实现插件的安装过程
        /// </summary>
		public virtual void Install() { }

        /// <summary>
        /// 重写此方法以实现插件的卸载过程
        /// </summary>
		public virtual void Uninstall() { }

        public bool Initialized { get; private set; }

        internal void DoInitialize()
        {
            if (Initialized == false)
            {
                Initialize();

                Initialized = true;
            }
        }

        /// <summary>
        /// 重写此方法以实现插件的初始化过程
        /// </summary>
        public abstract void Initialize();

        public Hashtable ActionHandlers = new Hashtable();

        public void Add<Action>(ActionHandler<Action> handler)
        {
            Add<Action>(handler, 1);
        }

        public void Add<Action>(ActionHandler<Action> handler, int priority)
        {
            Type actionType = typeof(Action);

            ActionHandlerInfoCollection<Action> handlers = ActionHandlers[actionType] as ActionHandlerInfoCollection<Action>;

            if (handlers == null)
            {
                handlers = new ActionHandlerInfoCollection<Action>();
                ActionHandlers.Add(actionType, handlers);
            }
            
            handlers.Add(new ActionHandlerInfo<Action>(handler, priority));
        }
	}

    /// <summary>
    /// 插件容器
    /// </summary>
    public class PluginCollection : Collection<PluginBase>
    {
    }
}