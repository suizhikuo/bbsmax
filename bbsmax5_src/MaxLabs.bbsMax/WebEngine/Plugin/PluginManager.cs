//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Web.Hosting;
using System.Collections;
using System.Configuration;
using System.Web.Compilation;
using System.Collections.Generic;
using MaxLabs.bbsMax;
using System.Reflection;

namespace MaxLabs.WebEngine.Plugin
{
    /// <summary>
    /// 插件管理器，为系统提供插件机制
    /// </summary>
    public static class PluginManager
    {
        private static PluginCollection s_Plugins = new PluginCollection();

        private static bool s_Initialized;
        private static object s_InitLocker = new object();

        /// <summary>
        /// 加载插件
        /// </summary>
        public static void Init()
        {
            if (s_Initialized)
                return;

            lock (s_InitLocker)
            {
                if (s_Initialized)
                    return;

                //TODO:读取设置

                string pluginFolder = Globals.GetPath(SystemDirecotry.Plugins);

                if (Directory.Exists(pluginFolder) == false)
                {
                    s_Initialized = true;
                    return;
                    //throw new Exception(String.Format(Resources.PluginManager_PluginFolderNotFound, pluginFolder));
                }

                PluginCollection plugins = new PluginCollection();

                foreach (string dir in Directory.GetDirectories(pluginFolder))
                {
                    string pluginName = Path.GetFileNameWithoutExtension(dir);

                    Type pluginType = null;

                    if (BuildManager.CodeAssemblies != null)
                    {
                        foreach (Assembly assembly in BuildManager.CodeAssemblies)
                        {
                            pluginType = assembly.GetType("MaxLabs.bbsMax.Web.plugins." + pluginName, false, true);

                            if (pluginType != null)
                                break;
                        }
                    }

                    if (pluginType == null)
                        pluginType = BuildManager.GetType("MaxLabs.bbsMax.Web.plugins." + pluginName, false);

                    if (pluginType != null)
                    {
                        PluginBase plugin = (PluginBase)Activator.CreateInstance(pluginType);

                        if (plugin != null)
                        {
                            plugin.DoInitialize();

                            plugin.Name = pluginName;

                            string pluginDisableFile = IOUtil.JoinPath(dir, "plugin_disabled");

                            plugin.Enable = File.Exists(pluginDisableFile) == false;

                            plugins.Add(plugin);
                        }
                    }
                    else
                    {
                        //TODO:记录下警告信息
                    }
                }

                s_Initialized = true;
                s_Plugins = plugins;
            }
        }

		public static PluginCollection Plugins
		{
			get { return s_Plugins; }
		}

        public static void DisablePlugin(string pluginName)
        {
            string pluginFolder = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Code), "max-plugins", pluginName);

            if (Directory.Exists(pluginFolder))
            {
                File.Create(IOUtil.JoinPath(pluginFolder, "plugin_disabled")).Close();

                foreach (PluginBase plugin in s_Plugins)
                {
                    if (plugin.Name == pluginName)
                        plugin.Enable = false;
                }
            }
        }

        public static void EnablePlugin(string pluginName)
        {
            string pluginDisableFile = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Code), "max-plugins", pluginName, "plugin_disabled");

            if (File.Exists(pluginDisableFile))
            {
                File.Delete(pluginDisableFile);

                foreach (PluginBase plugin in s_Plugins)
                {
                    if (plugin.Name == pluginName)
                    {
                        plugin.Enable = true;
                        plugin.DoInitialize();
                    }
                }
            }
        }

        public static ActionHandlerResult Invoke<Action>(Action args)
        {
            ActionHandlerResult result = null;

            Type actionType = typeof(Action);

            foreach (PluginBase plugin in s_Plugins)
            {
                if (plugin.Enable == false)
                    continue;

                ActionHandlerInfoCollection<Action> actionHandlerInfos = plugin.ActionHandlers[actionType] as ActionHandlerInfoCollection<Action>;

                if (actionHandlerInfos != null)
                {
                    //处理器已经按优先级排序，优先级值低的先被调用
                    foreach (ActionHandlerInfo<Action> handlerInfo in actionHandlerInfos)
                    {
                        result = handlerInfo.Handler(args);

                        //不冒泡就停止后续处理器的执行
                        if (result != null && result.Bubble == false)
                            return result;
                    }
                }
            }

            if (result == null)
                result = new ActionHandlerResult();

            return result;
        }
    }
}