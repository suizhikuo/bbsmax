//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Web;
//using System.IO;
//using System.Security.Cryptography;

//namespace MaxLabs.bbsMax.FileSystem
//{
//    /// <summary>
//    /// 提供用于ASP.NET的文件分块上传功能
//    /// </summary>
//    public class Uploader : IDisposable, TempUploadFile
//    {

//        #region 字段
//        //private bool m_EnableMD5;
//        private byte[] m_MD5Hash;

//        private string m_ContentType;
//        private long m_FileSize = -1;
//        private long m_TotalReceived;
//        private long? m_RequestLength;
//        private long m_TotalFileReceived;
//        private ReceiveBufferObject m_ReceiveBuffer;
//        private HttpWorkerRequest m_CurrentWorkerRequest;
//        private FileStream m_FileStream;

//        public delegate bool UploadOnProcess(object sender, EventArgs args);
//        #endregion

//        #region 属性
//        /// <summary>
//        /// 当前Http请求
//        /// </summary>
//        public HttpWorkerRequest CurrentWorkerRequest
//        {
//            get
//            {
//                if (m_CurrentWorkerRequest == null)
//                {
//                    IServiceProvider serviceProvider = (IServiceProvider)HttpContext.Current;
//                    m_CurrentWorkerRequest = (HttpWorkerRequest)serviceProvider.GetService(typeof(HttpWorkerRequest));
//                }

//                return m_CurrentWorkerRequest;
//            }
//        }

//        /// <summary>
//        /// 最后一次接收的数据块
//        /// </summary>
//        public ReceiveBufferObject ReceiveBuffer
//        {
//            get { return m_ReceiveBuffer; }
//            set { m_ReceiveBuffer = value; }
//        }

//        /// <summary>
//        /// 请求的总长度
//        /// </summary>
//        public long RequestLength
//        {
//            get
//            {
//                if (m_RequestLength == null || m_RequestLength <= 0)
//                {
//                    string requestHeader = CurrentWorkerRequest.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentLength);

//                    if (requestHeader != null)
//                    {
//                        try
//                        {
//                            m_RequestLength = long.Parse(requestHeader, System.Globalization.CultureInfo.InvariantCulture);
//                        }
//                        catch
//                        {
//                        }
//                    }
//                    else if (CurrentWorkerRequest.IsEntireEntityBodyIsPreloaded())
//                    {
//                        byte[] preloadedEntityBody = CurrentWorkerRequest.GetPreloadedEntityBody();

//                        if (preloadedEntityBody != null)
//                            m_RequestLength = preloadedEntityBody.Length;
//                    }

//                    if (m_RequestLength == null)
//                        m_RequestLength = 0;
//                }

//                return m_RequestLength.Value;
//            }
//        }

//        /// <summary>
//        /// 已接收的数据长度
//        /// </summary>
//        public long TotalReceived
//        {
//            get { return m_TotalReceived; }
//            private set { m_TotalReceived = value; }
//        }

//        /// <summary>
//        /// 文件长度
//        /// </summary>
//        public long FileSize
//        {
//            get
//            {
//                if (m_FileSize == -1)
//                    return RequestLength;

//                return m_FileSize;
//            }

//            private set { m_FileSize = value; }
//        }

//        /// <summary>
//        /// 已接收到的文件长度
//        /// </summary>
//        public long TotalFileReceived
//        {
//            get { return m_TotalFileReceived; }
//            private set { m_TotalFileReceived = value; }
//        }

//        ///// <summary>
//        ///// 是否计算文件的MD5哈希
//        ///// </summary>
//        //public bool EnableMD5
//        //{
//        //    get { return m_EnableMD5; }
//        //    set { m_EnableMD5 = value; }
//        //}

//        /// <summary>
//        /// 获取文件的MD5哈希，要获取此值必须在开始上传前设置EnableMD5属性为true
//        /// </summary>
//        public byte[] MD5Hash
//        {
//            get { return m_MD5Hash; }
//            private set { m_MD5Hash = value; }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public string ContentType
//        {
//            get { return m_ContentType; }
//            private set { m_ContentType = value; }
//        }

//        private bool m_IsSwfUploader = false;

//        /// <summary>
//        /// 是否是使用SwfUploader上传
//        /// </summary>
//        public bool IsSwfUploader
//        {
//            get { return m_IsSwfUploader; }
//            set { m_IsSwfUploader = value; }
//        }


//        #endregion

//        /// <summary>
//        /// 开始文件上传
//        /// </summary>
//        /// <param name="filePath">文件保存路径</param>
//        public bool BeginUpload(string filePath, UploadOnProcess onProcess)
//        {

//            #region 初始数据

//            //TODO:外部要可设置编码
//            byte[] newlineBytes = Encoding.UTF8.GetBytes("\r\n");
//            byte[] endLineBytes = Encoding.UTF8.GetBytes("--");

//            int lineIndex1 = -2;
//            int lineIndex2 = -2;
//            int lineIndex3 = -2;
//            int lineIndex4 = -2;
//            int lineIndex5 = -2;
//            int lineIndex6 = -2;
//            int lineIndex7 = -2;
//            int lineIndex8 = -2;

//            bool parseHeaderDone = false;

//            #endregion

//            byte[] preloadedData = CurrentWorkerRequest.GetPreloadedEntityBody();

//            //有可能会有一部分预读数据特别是文件小的时候，这时候得把预读数据写入缓冲区，等候处理
//            if (preloadedData != null)
//            {
//                this.ReceiveBuffer = new ReceiveBufferObject(preloadedData.Length, preloadedData.Length, preloadedData);
//                TotalReceived += this.ReceiveBuffer.LastTimeReceive;
//            }

//            do
//            {
//                if (this.ReceiveBuffer != null)
//                {
//                    #region 读头4行表单信息内容

//                    if (lineIndex1 == -2)
//                        lineIndex1 = ByteHelper.SearchBytesInArray(newlineBytes, this.ReceiveBuffer.Data, 0);

//                    if (lineIndex1 != -1)
//                    {
//                        if (lineIndex2 == -2)
//                            lineIndex2 = ByteHelper.SearchBytesInArray(newlineBytes, this.ReceiveBuffer.Data, lineIndex1 + newlineBytes.Length);

//                        if (lineIndex2 != -1)
//                        {
//                            if (lineIndex3 == -2)
//                                lineIndex3 = ByteHelper.SearchBytesInArray(newlineBytes, this.ReceiveBuffer.Data, lineIndex2 + newlineBytes.Length);

//                            if (lineIndex3 != -1)
//                            {
//                                if (lineIndex4 == -2)
//                                    lineIndex4 = ByteHelper.SearchBytesInArray(newlineBytes, this.ReceiveBuffer.Data, lineIndex3 + newlineBytes.Length);

//                                if (lineIndex4 != -1)
//                                {
//                                    if (!IsSwfUploader)
//                                    {
//                                        //直到找到第四个换行
//                                        parseHeaderDone = true;
//                                    }
//                                    else
//                                    {
//                                        if (lineIndex5 == -2)
//                                            lineIndex5 = ByteHelper.SearchBytesInArray(newlineBytes, this.ReceiveBuffer.Data, lineIndex4 + newlineBytes.Length);

//                                        if (lineIndex5 != -1)
//                                        {
//                                            if (lineIndex6 == -2)
//                                                lineIndex6 = ByteHelper.SearchBytesInArray(newlineBytes, this.ReceiveBuffer.Data, lineIndex5 + newlineBytes.Length);

//                                            if (lineIndex6 != -1)
//                                            {
//                                                if (lineIndex7 == -2)
//                                                    lineIndex7 = ByteHelper.SearchBytesInArray(newlineBytes, this.ReceiveBuffer.Data, lineIndex6 + newlineBytes.Length);

//                                                if (lineIndex7 != -1)
//                                                {
//                                                    if (lineIndex8 == -2)
//                                                        lineIndex8 = ByteHelper.SearchBytesInArray(newlineBytes, this.ReceiveBuffer.Data, lineIndex7 + newlineBytes.Length);

//                                                    if (lineIndex8 != -1)
//                                                    {
//                                                        parseHeaderDone = true;
//                                                    }
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }

//                    #endregion
//                }

//                if (parseHeaderDone == false)
//                {
//                    ReadDataToBuffer(true);

//                    //非常不可能发生，但做下处理比较保险
//                    if (ReceiveBuffer == null || ReceiveBuffer.LastTimeReceive == 0)
//                        return false;
//                }
//            } while (parseHeaderDone == false);

//            if (!IsSwfUploader)
//            {
//                //实际文件大小 = 请求长度 - 表单头部 - 表单尾部 - 表单项本身的最后换行
//                FileSize = RequestLength - (lineIndex4 + newlineBytes.Length) - newlineBytes.Length - (lineIndex1 + endLineBytes.Length + newlineBytes.Length);
//            }
//            else
//            {
//                //实际文件大小 = 请求长度 - 表单头部 - 表单头部 - 表单项本身的最后换行 - Flash第三个表单项内容 - 表单项本身的最后换行 - 表单尾部
//                FileSize = RequestLength - (lineIndex8 + newlineBytes.Length) - newlineBytes.Length - (lineIndex1 + newlineBytes.Length) - 61 - (lineIndex1 + endLineBytes.Length + newlineBytes.Length);
//            }

//            if (onProcess != null)
//                if (!onProcess(this, null))
//                    return false;

//            //如果文件是空的直接返回
//            if (FileSize == 0)
//            {
//                File.Create(filePath).Close();
//                return true;
//            }

//            string contentTypeRaw;

//            if (!IsSwfUploader)
//                contentTypeRaw = Encoding.UTF8.GetString(ReceiveBuffer.Data, lineIndex2, lineIndex3 - lineIndex2);
//            else
//                contentTypeRaw = Encoding.UTF8.GetString(ReceiveBuffer.Data, lineIndex6, lineIndex7 - lineIndex6);


//            //"Content-Type:"长度是13,还要算上换行
//            ContentType = contentTypeRaw.Substring(16).Trim();

//            using (m_FileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
//            {
//                HashAlgorithm algorithm = HashAlgorithm.Create("MD5");
//                using (CryptoStream cryptoStream = new CryptoStream(m_FileStream, algorithm, CryptoStreamMode.Write))
//                {

//                    #region 处理已经读取的文件内容
//                    //表单信息结束位置 = 最后一行最后一个字母位置 + 换行字符长度
//                    int headEnd = lineIndex4 + newlineBytes.Length;

//                    if (IsSwfUploader)
//                        headEnd = lineIndex8 + newlineBytes.Length;

//                    //如果表单信息结束位置比总的接收数据长度小，说明存在已读取的文件内容
//                    if (headEnd < this.TotalReceived - 1)
//                    {
//                        long dataLength = this.TotalReceived - headEnd;

//                        //有可能一次就读完了，这时候缓冲区中会有多余的表单数据
//                        if (this.FileSize < dataLength)
//                            dataLength = this.FileSize;

//                        //用文件数据替换缓冲区中的数据
//                        byte[] newBuffer = new byte[dataLength];

//                        for (int i = 0; i < dataLength; i++)
//                        {
//                            newBuffer[i] = this.ReceiveBuffer.Data[i + headEnd];
//                        }

//                        this.ReceiveBuffer = new ReceiveBufferObject(newBuffer.Length, newBuffer.Length, newBuffer);

//                        TotalFileReceived += dataLength;

//                        cryptoStream.Write(this.ReceiveBuffer.Data, 0, this.ReceiveBuffer.Size);

//                        if (TotalFileReceived == FileSize)
//                            m_MD5Hash = algorithm.Hash;

//                        //触发事件，告诉外部已经读取一块内容
//                        if (onProcess != null)
//                            if (!onProcess(this, null))
//                                return false;
//                    }
//                    #endregion

//                    do
//                    {
//                        ReadDataToBuffer(false);

//                        if (this.ReceiveBuffer != null)
//                        {
//                            int dataLength = ReceiveBuffer.LastTimeReceive;

//                            if (TotalFileReceived + dataLength > FileSize)
//                                dataLength = (int)(FileSize - TotalFileReceived);

//                            TotalFileReceived += dataLength;

//                            cryptoStream.Write(ReceiveBuffer.Data, 0, dataLength);

//                            m_MD5Hash = algorithm.Hash;

//                            if (onProcess != null)
//                                if (!onProcess(this, null))
//                                    return false;
//                        }

//                    } while (this.ReceiveBuffer != null);
//                }
//            }

//            //文件没传完，应该是客户端突然结束请求了
//            if (TotalFileReceived != FileSize)
//            {
//                File.Delete(filePath);
//                return false;
//            }

//            return true;
//        }

//        /// <summary>
//        /// 将数据读到缓存
//        /// </summary>
//        /// <param name="saveOldData">是否保留历史数据</param>
//        /// <returns>实际读取到的数据长度</returns>
//        private void ReadDataToBuffer(bool saveOldData)
//        {
//            long bufferSize = this.RequestLength - this.TotalReceived;

//            if (bufferSize <= 0)
//            {
//                if (saveOldData == false && this.ReceiveBuffer != null)
//                {
//                    this.ReceiveBuffer.Dispose();
//                    this.ReceiveBuffer = null;
//                }

//                return;
//            }

//            //TODO:外部要可设置,每次读取200K
//            if (bufferSize > 204800)
//                bufferSize = 204800;

//            byte[] buffer = new byte[bufferSize];

//            int received = CurrentWorkerRequest.ReadEntityBody(buffer, buffer.Length);

//            if (received == 0)
//            {
//                if (saveOldData == false && this.ReceiveBuffer != null)
//                {
//                    this.ReceiveBuffer.Dispose();
//                    this.ReceiveBuffer = null;
//                }

//                return;
//            }

//            if (saveOldData)
//            {
//                if (this.ReceiveBuffer != null)
//                {
//                    //TODO:需要略过缓冲区中无用的数据
//                    buffer = ByteHelper.MergeArrays(this.ReceiveBuffer.Data, buffer);
//                    this.ReceiveBuffer.Dispose();
//                    this.ReceiveBuffer = null;
//                }

//                this.ReceiveBuffer = new ReceiveBufferObject(buffer.Length, received, buffer);
//            }
//            else
//                this.ReceiveBuffer = new ReceiveBufferObject(received, received, buffer);

//            TotalReceived += received;
//        }

//        #region IDisposable Members

//        public void Dispose()
//        {
//            if (this.ReceiveBuffer != null)
//            {
//                this.ReceiveBuffer.Dispose();
//                this.ReceiveBuffer = null;
//            }
//        }

//        #endregion

//        /// <summary>
//        /// 数据接收缓冲区
//        /// </summary>
//        public class ReceiveBufferObject : IDisposable
//        {
//            private int m_Size;
//            private byte[] m_Data;
//            private int m_LastTimeReceive;

//            /// <summary>
//            /// 缓冲区中数据长度，通常等于最后一次接收数据的长度
//            /// 最后一次接收数据的长度不一定等于缓冲区字节数组长度，需要注意
//            /// 在以保存原有缓冲区数据模式读取请求内容时，数据长度会递增
//            /// </summary>
//            public int Size
//            {
//                get { return m_Size; }
//                private set { m_Size = value; }
//            }

//            /// <summary>
//            /// 最后一次接收到的数据长度，在以保存原有缓冲区数据模式读取请求内容时
//            /// 最后接收到的数据长度和缓冲区中数据长度是有区别的
//            /// </summary>
//            public int LastTimeReceive
//            {
//                get { return m_LastTimeReceive; }
//                private set { m_LastTimeReceive = value; }
//            }

//            public byte[] Data
//            {
//                get { return m_Data; }
//                private set { m_Data = value; }
//            }

//            public ReceiveBufferObject(int size, int lastTimeReceive, byte[] data)
//            {
//                m_Size = size;
//                m_Data = data;
//                m_LastTimeReceive = lastTimeReceive;
//            }

//            public void Dispose()
//            {
//                m_Data = null;
//            }
//        }

//        #region IUploadedFile 成员

//        public string MD5Code
//        {
//            get { return BitConverter.ToString(m_MD5Hash).Replace("-", ""); }
//        }

//        public void MoveTo(string savePath)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }
//}