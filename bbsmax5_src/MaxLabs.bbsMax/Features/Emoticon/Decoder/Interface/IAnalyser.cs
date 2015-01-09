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
using MaxLabs.bbsMax.Entities;
using System.IO;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Emoticons
{
    public interface IAnalyser
    {
        int EmoticonCount { get; }
        int TotalEmoticonSize { get; }
        int MaxEmoticonSize { get; }
        Dictionary<string, List<EmoticonItem>> GetGroupedEmoticons();
    }

    //表情分析器
    public class AnalyserFactory
    {
        public IAnalyser BuildAnalyser(string filename)
        {
            string filePostfix = Path.GetExtension(filename);

            switch (filePostfix.ToLower())
            {
                case ".cfc":
                    return new CFCAnalyser(filename);
                case ".eip":
                    return new EIPAnalyser(filename);
            }
            return null;
        }

        public IAnalyser BuildAnalyser(Stream stream, EmoticonPackType type)
        {
            switch (type)
            {
                case EmoticonPackType.CFC:
                    return new CFCAnalyser(stream);
                case EmoticonPackType.EIP:
                    return new EIPAnalyser(stream);
            }
            return null;
        }

        private static AnalyserFactory m_instance;
        public static AnalyserFactory Instance
        {
            get
            {
                return m_instance;
            }
        }

        static AnalyserFactory()
        {
            m_instance = new AnalyserFactory();
        }

        private AnalyserFactory() { }
    }

    //表情打包器
    public class EmoticonPackerFactory
    {
        private EmoticonPackerFactory() { }

        static EmoticonPackerFactory instance;
        static EmoticonPackerFactory() {
            instance = new EmoticonPackerFactory();
        }

        public object BuildEmoticonPackcer( EmoticonPackType packType)
        {
            switch (packType)
            {
                case EmoticonPackType.CFC:
                    return null;
                case EmoticonPackType.EIP:
                    return null;
            }

            return null;
        }

        public static EmoticonPackerFactory Instance { get { return instance; } }
    }
}