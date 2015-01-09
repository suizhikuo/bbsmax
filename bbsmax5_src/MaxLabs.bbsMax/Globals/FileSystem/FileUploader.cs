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
using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Security.Cryptography;

namespace MaxLabs.bbsMax.FileSystem
{
    public class FileManagerUploadPolicy : FileUploadPolicy
    {
        public FileManagerUploadPolicy(string tempFilePath):base(true)
        {
            m_TempFilePath = tempFilePath;
        }

        private string m_TempFilePath;

        private MD5 m_UploadFileMD5;

        public long UploadFileSize
        {
            get;
            private set;
        }

        public string UploadFileName
        {
            get;
            private set;
        }

        public string UploadFileMD5
        {
            get;
            private set;
        }

        public override Stream CreateFileStream(string uploadFileName)
        {
            UploadFileName = uploadFileName;

            FileStream stream = new FileStream(m_TempFilePath, FileMode.CreateNew);

            m_UploadFileMD5 =  MD5CryptoServiceProvider.Create();

            return new CryptoStream(stream, m_UploadFileMD5, CryptoStreamMode.Write);
        }

        public override void DisposeFileStream(long uploadFileSize, Stream stream)
        {
            UploadFileSize = uploadFileSize;

            base.DisposeFileStream(uploadFileSize, stream);

            string md5Code = System.BitConverter.ToString(m_UploadFileMD5.Hash);

            UploadFileMD5 = md5Code.Replace("-", "");
        }
    }

    public abstract class FileUploadPolicy
    {
        public FileUploadPolicy(bool useFormDataInjection)
        {
            UseFormDataInjection = useFormDataInjection;
        }

        public bool UseFormDataInjection
        {
            get;
            private set;
        }

        public virtual void InvalidContentType(string contentType)
        {
        }

        public abstract Stream CreateFileStream(string uploadFileName);

        public virtual void DisposeFileStream(long uploadFileSize, Stream stream)
        {
            stream.Close();
            stream.Dispose();
        }

        public virtual void InjectFormData(byte[] formData)
        {
            HttpRequest request = HttpContext.Current.Request;

            BindingFlags flags1 = BindingFlags.NonPublic | BindingFlags.Instance;

            Type type1 = request.GetType();

            FieldInfo info1 = type1.GetField("_rawContent", flags1);
            FieldInfo info2 = type1.GetField("_contentLength", flags1);

            if (info1 != null && info2 != null)
            {
                Assembly webAssembly = Assembly.GetAssembly(typeof(HttpRequest));

                Type typeofHttpRawUploadedContent = webAssembly.GetType("System.Web.HttpRawUploadedContent");

                object[] argList = new object[] { formData.Length + 1024, formData.Length };

                CultureInfo currCulture = CultureInfo.CurrentCulture;

                object httpRawUploadedContent = Activator.CreateInstance(
                    typeofHttpRawUploadedContent,
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    argList,
                    currCulture,
                    null
                );

                Type contentType = httpRawUploadedContent.GetType();

                FieldInfo dataField = contentType.GetField("_data", flags1);
                dataField.SetValue(httpRawUploadedContent, formData);

                FieldInfo lengthField = contentType.GetField("_length", flags1);
                lengthField.SetValue(httpRawUploadedContent, formData.Length);

                FieldInfo completeField = contentType.GetField("_completed", flags1);
                completeField.SetValue(httpRawUploadedContent, true);

                FieldInfo fileThresholdField = contentType.GetField("_fileThreshold", flags1);
                fileThresholdField.SetValue(httpRawUploadedContent, formData.Length + 1024);

                info1.SetValue(request, httpRawUploadedContent);
                info2.SetValue(request, formData.Length);
            }
        }
    }

    public class FileUploader : IDisposable
    {
        private FileUploader()
        {
        }

        public FileUploader(FileUploadPolicy fileUploadPolicy)
        {
            if (fileUploadPolicy == null)
                throw new ArgumentNullException("fileUploadPolicy");

            FileUploadPolicy = fileUploadPolicy;
        }

        private FileUploadPolicy FileUploadPolicy
        {
            get;
            set;
        }

        private HttpRequest m_Request;

        private HttpRequest Request
        {
            get
            {
                if (m_Request == null)
                {
                    m_Request = HttpContext.Current.Request;
                }

                return m_Request;
            }
        }

        private HttpWorkerRequest m_WorkerRequest;

        private HttpWorkerRequest WorkerRequest
        {
            get
            {
                if (m_WorkerRequest == null)
                {
                    IServiceProvider serviceProvider = (IServiceProvider)HttpContext.Current;
                    m_WorkerRequest = (HttpWorkerRequest)serviceProvider.GetService(typeof(HttpWorkerRequest));
                }

                return m_WorkerRequest;
            }
        }

        private int TotalReadedLength
        {
            get;
            set;
        }

        public void BeginUpload()
        {
            if (false == StringStartsWithIgnoreCase(Request.ContentType, "multipart/form-data"))
            {
                FileUploadPolicy.InvalidContentType(Request.ContentType);
                return;
            }

            m_FormData = new MemoryStream(1024);

            byte[] boundary = GetMultipartBoundary(Request.ContentType);

            byte[] preloaded = WorkerRequest.GetPreloadedEntityBody();

            if (preloaded != null)
            {
                ParseData(preloaded, 0, preloaded.Length, boundary);

                TotalReadedLength += preloaded.Length;
            }

            int loopCounter = 0;

            if (!WorkerRequest.IsEntireEntityBodyIsPreloaded())
            {
                byte[] buffer = new byte[10240];

                while (WorkerRequest.IsClientConnected() && TotalReadedLength < Request.ContentLength)
                {
                    int readedLength = WorkerRequest.ReadEntityBody(buffer, buffer.Length);

                    if (readedLength <= 0)
                        break;

                    TotalReadedLength += readedLength;

                    ParseData(buffer, 0, readedLength, boundary);

                    if (loopCounter == 5)
                    {
                        loopCounter = 0;
                        Thread.Sleep(1);
                    }
                    else
                    {
                        loopCounter += 1;
                    }
                }
            }

            if (FileUploadPolicy.UseFormDataInjection)
            {
                byte[] formData = new byte[m_FormData.Position];

                Buffer.BlockCopy(m_FormData.GetBuffer(), 0, formData, 0, formData.Length);

                FileUploadPolicy.InjectFormData(formData);
            }
        }

        private const byte LOOK_FOR_BOUNDARY = 0;
        private const byte LOOK_FOR_DOUBLE_NEWLINE = 1;
        private const byte LOOK_FOR_FILE_CONTENT = 2;
        private const byte LOOK_FOR_FILE_END = 4;

        private byte m_Status = LOOK_FOR_BOUNDARY;

        private int m_SearchStartIndex = 0;
        private int m_LastBoundaryIndex = 0;

        private MemoryStream m_FormData = null;

        private Stream m_CurrentFileStream = null;

        private long m_CurrentFileSize = 0;
        private string m_CurrentFileFormName = null;
        private string m_CurrentFileRealName = null;

        private void ParseData(byte[] buffer, int startIndex, int length, byte[] boundary)
        {
        ParseDataStart:
            if (startIndex == buffer.Length - 1)
                return;

            byte[] formBuffer = null;

            if (m_Status != LOOK_FOR_FILE_CONTENT)
            {
                m_FormData.Write(buffer, startIndex, length);

                formBuffer = m_FormData.GetBuffer();
            }

            int formDataLength = 0;

            switch (m_Status)
            {
                case LOOK_FOR_BOUNDARY:

                    formDataLength = (int)m_FormData.Position;

                    m_LastBoundaryIndex = SearchByteArray(formBuffer, boundary, m_SearchStartIndex, formDataLength - m_SearchStartIndex);

                    if (m_LastBoundaryIndex >= 0)
                    {
                        m_Status = LOOK_FOR_DOUBLE_NEWLINE;

                        m_SearchStartIndex = m_LastBoundaryIndex + boundary.Length;

                        goto LOOK_FOR_DOUBLE_NEWLINE;
                    }

                    break;

                case LOOK_FOR_DOUBLE_NEWLINE:

                LOOK_FOR_DOUBLE_NEWLINE:

                    formDataLength = (int)m_FormData.Position;

                if (formDataLength - m_SearchStartIndex >= 4)
                {
                    int doubleNewLineIndex = SearchDoubleNewLine(formBuffer, m_SearchStartIndex, formDataLength - m_SearchStartIndex);

                    if (doubleNewLineIndex >= 0)
                    {
                        string str = Request.ContentEncoding.GetString(formBuffer, m_SearchStartIndex, doubleNewLineIndex - m_SearchStartIndex);

                        bool isFile = false;

                        int index = str.IndexOf("Content-Disposition:", StringComparison.OrdinalIgnoreCase);

                        if (index >= 0)
                        {
                            index = index + 20;

                            m_CurrentFileFormName = ExtractValueFromContentDispositionHeader(str, index, "name");
                            m_CurrentFileRealName = ExtractValueFromContentDispositionHeader(str, index, "filename");

                            isFile = m_CurrentFileRealName != null;
                        }

                        if (isFile)
                        {
                            m_Status = LOOK_FOR_FILE_CONTENT;

                            m_FormData.Position = m_LastBoundaryIndex;

                            m_SearchStartIndex = m_LastBoundaryIndex;

                            if (m_CurrentFileStream != null)
                            {
                                m_CurrentFileSize = 0;

                                FileUploadPolicy.DisposeFileStream(m_CurrentFileSize, m_CurrentFileStream);
                            }

                            m_CurrentFileStream = FileUploadPolicy.CreateFileStream(m_CurrentFileRealName);
                        }
                        else
                        {
                            m_Status = LOOK_FOR_BOUNDARY;

                            m_FormData.Position = doubleNewLineIndex + 4;

                            m_SearchStartIndex = doubleNewLineIndex + 4;
                        }

                        if (formDataLength - doubleNewLineIndex - 4 > 0)
                        {
                            /* 以下改为非递归
                            ParseData(formBuffer, doubleNewLineIndex + 4, formDataLength - doubleNewLineIndex - 4, boundary);
                            */

                            buffer = formBuffer;
                            startIndex = doubleNewLineIndex + 4;
                            length = formDataLength - doubleNewLineIndex - 4;

                            goto ParseDataStart;
                        }
                    }
                    else
                    {
                        m_SearchStartIndex += 1;

                        goto LOOK_FOR_DOUBLE_NEWLINE;
                    }
                }

                break;

                case LOOK_FOR_FILE_CONTENT:

                formDataLength = (int)m_FormData.Position;

                int newLineIndex = Array.IndexOf<byte>(buffer, 0x0D, startIndex, length);

                if (newLineIndex >= 0)
                {
                    m_Status = LOOK_FOR_FILE_END;

                    int temp = newLineIndex - startIndex;

                    if (temp > 0)
                    {
                        m_CurrentFileSize += temp;

                        m_CurrentFileStream.Write(buffer, startIndex, temp);
                    }

                    m_SearchStartIndex = (int)m_FormData.Position;

                    /* 以下改为非递归
                    ParseData(buffer, newLineIndex, length - temp, boundary);
                    */

                    startIndex = newLineIndex;
                    length = length - temp;

                    goto ParseDataStart;
                }
                else
                {
                    m_CurrentFileSize += length;

                    m_CurrentFileStream.Write(buffer, startIndex, length);
                }

                break;

                case LOOK_FOR_FILE_END:

                formDataLength = (int)m_FormData.Position;

                if (formDataLength - m_SearchStartIndex >= boundary.Length + 2)
                {
                    int fileEndIndex = -1;

                    if (formBuffer[m_SearchStartIndex + 1] == 0x0A)
                    {
                        fileEndIndex = SearchByteArray(formBuffer, boundary, m_SearchStartIndex + 2, formDataLength - m_SearchStartIndex - 2);
                    }

                    if (fileEndIndex >= 0)
                    {
                        m_FormData.Position = m_SearchStartIndex;

                        if (fileEndIndex > m_SearchStartIndex + 2)
                        {
                            int size = fileEndIndex - 2 - m_SearchStartIndex;

                            m_CurrentFileSize += size;

                            m_CurrentFileStream.Write(formBuffer, m_SearchStartIndex, size);
                        }

                        FileUploadPolicy.DisposeFileStream(m_CurrentFileSize, m_CurrentFileStream);

                        m_CurrentFileStream = null;

                        m_Status = LOOK_FOR_DOUBLE_NEWLINE;

                        m_LastBoundaryIndex = fileEndIndex;

                        m_SearchStartIndex = fileEndIndex + boundary.Length;

                        /* 以下改为非递归
                        ParseData(formBuffer, fileEndIndex, formDataLength - fileEndIndex, boundary);
                        */

                        buffer = formBuffer;
                        startIndex = fileEndIndex;
                        length = formDataLength - fileEndIndex;

                        goto ParseDataStart;
                    }
                    else
                    {
                        m_Status = LOOK_FOR_FILE_CONTENT;

                        m_CurrentFileSize += 1;

                        m_CurrentFileStream.Write(formBuffer, m_SearchStartIndex, 1);

                        m_FormData.Position = m_LastBoundaryIndex;

                        /* 以下改为非递归
                        ParseData(formBuffer, m_SearchStartIndex + 1, formDataLength - m_SearchStartIndex - 1, boundary);
                        */

                        buffer = formBuffer;
                        startIndex = m_SearchStartIndex + 1;
                        length = formDataLength - m_SearchStartIndex - 1;

                        goto ParseDataStart;
                    }
                }

                break;
            }
        }

        private static readonly byte[] s_DoubleNewLinePattern = new byte[] { 0x0D, 0x0A, 0x0D, 0x0A };

        private static string ExtractValueFromContentDispositionHeader(string l, int pos, string name)
        {
            string str = name + "=\"";

            int startIndex = CultureInfo.InvariantCulture.CompareInfo.IndexOf(l, str, pos, CompareOptions.IgnoreCase);

            if (startIndex < 0)
            {
                return null;
            }

            startIndex += str.Length;

            int index = l.IndexOf('"', startIndex);

            if (index < 0)
            {
                return null;
            }

            if (index == startIndex)
            {
                return string.Empty;
            }

            return l.Substring(startIndex, index - startIndex);
        }

        private static byte[] GetMultipartBoundary(string contentType)
        {
            int pos = contentType.IndexOf("boundary=");

            string boundary = "--" + contentType.Substring(pos + 9);

            return Encoding.ASCII.GetBytes(boundary.ToCharArray());
        }

        private static bool StringStartsWithIgnoreCase(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return false;

            if (s2.Length > s1.Length)
                return false;

            return (0 == string.Compare(s1, 0, s2, 0, s2.Length, StringComparison.OrdinalIgnoreCase));
        }

        private static int SearchDoubleNewLine(byte[] buffer, int startIndex, int length)
        {
            return SearchByteArray(buffer, s_DoubleNewLinePattern, startIndex, length);
        }

        private static int SearchByteArray(byte[] buffer, byte[] value, int startIndex, int length)
        {
            int lastIndex = startIndex + length;

            for (int i = startIndex; i < lastIndex; i++)
            {
                if (lastIndex - i < value.Length)
                    break;

                if (buffer[i] != value[0])
                    continue;

                int dataIndex = i;
                bool found = true;

                for (int j = 1; j < value.Length; j++)
                {
                    dataIndex++;

                    if (buffer[dataIndex] != value[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                    return i;
            }

            return -1;
        }

        public void Dispose()
        {
            if (m_FormData != null)
            {
                m_FormData.Dispose();
                m_FormData = null;
            }

            if (m_CurrentFileStream != null)
            {
                FileUploadPolicy.DisposeFileStream(m_CurrentFileSize, m_CurrentFileStream);

                m_CurrentFileStream = null;
            }
        }
    }
}