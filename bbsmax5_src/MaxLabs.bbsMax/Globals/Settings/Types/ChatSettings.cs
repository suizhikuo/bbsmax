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
using MaxLabs.bbsMax.Enums;
using System.IO;
using System.Web;

namespace MaxLabs.bbsMax.Settings
{
    public class ChatSettings : SettingBase
    {

        private string _SoundFilePath;

        public ChatSettings()
        {
            SaveMessageDays = 60;
            EnableDefaultEmoticon = true;
            EnableUserEmoticon = false;
            DataClearMode = JobDataClearMode.Disabled;
            SaveMessageRows = 100000;
            ClearMassgeExecuteTime = 4;
            ClearNoReadMessage = false;
            this.MessageSoundSrc = string.Empty;
            this._SoundFilePath = Globals.GetPath(SystemDirecotry.Assets_Sounds);
            this.EnableChatFunction = true;
        }

        [SettingItem]
        public bool EnableChatFunction
        {
            get;
            set;
        }

        [SettingItem]
        public JobDataClearMode DataClearMode
        {
            get;
            set;
        }

        [SettingItem]
        public int SaveMessageRows
        {
            get;
            set;
        }

        /// <summary>
        /// 默认短消息声音
        /// </summary>
        [SettingItem]
        public string MessageSoundSrc { get; set; }

        public string MessageSound { get { if (HasMessageSound)  return UrlUtil.ResolveUrl(MessageSoundSrc); return string.Empty; } }

        /// <summary>
        /// 消息保存时间
        /// </summary>
        [SettingItem]
        public int SaveMessageDays
        {
            get;
            set;
        }

        /// <summary>
        /// 使用系统表情
        /// </summary>
        [SettingItem]
        public bool EnableDefaultEmoticon
        {
            get;
            set;
        }

        public bool ClearNoReadMessage
        {
            get;
            set;
        }

        /// <summary>
        /// 使用用户表情
        /// </summary>
        [SettingItem]
        public bool EnableUserEmoticon
        {
            get;
            set;
        }

        /// <summary>
        /// 是否设置了默认短消息声音
        /// </summary>
        public bool HasMessageSound
        {
            get
            {
                return !string.IsNullOrEmpty( this.MessageSoundSrc);
            }
        }

        /// <summary>
        /// 清除消息的任务执行时间
        /// </summary>
        [SettingItem]
        public int ClearMassgeExecuteTime
        {
            get;
            set;
        }

        //所以音频文件列表
        public List<SoundFileItem> SoundList
        {
            get
            {
                List<SoundFileItem> sounds = new List<SoundFileItem>();
                DirectoryInfo soundDir;
                if (!Directory.Exists(_SoundFilePath))
                {
                    soundDir = Directory.CreateDirectory(_SoundFilePath);
                }
                else
                {
                    soundDir = new DirectoryInfo(_SoundFilePath);
                }

                FileInfo[] files;
                string urlRoot = Globals.GetRelativeUrl(SystemDirecotry.Assets_Sounds);
                string[] postfixs = new string[] {"*.mp3","*.mid","*.wav","*.wma" };

                foreach (string ex in postfixs)
                {
                    files = soundDir.GetFiles(ex);
                    foreach (FileInfo f in files)
                    {
                        SoundFileItem item = new SoundFileItem(HttpUtility.HtmlEncode( f.Name), UrlUtil.JoinUrl(urlRoot, HttpUtility.UrlEncode(f.Name)));
                        sounds.Add(item);
                    }
                }
                return sounds;
            }
        }

        public string SoundFilePath
        {
            get
            {
                return _SoundFilePath;
            }
        }

        public struct SoundFileItem
        {
            string _fileName;
            string _url;
            public SoundFileItem(string filename, string url)
            {
                _fileName = filename;
                _url = url;
            }

            [JsonItem]
            public string FileName { get { return _fileName; } }
            [JsonItem]
            public string Url { get { return _url; } }
        }
    }
}