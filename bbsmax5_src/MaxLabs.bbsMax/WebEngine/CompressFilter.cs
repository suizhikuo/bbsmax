//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

using System.IO.Compression;
using System.Web;

namespace MaxLabs.WebEngine
{
    /// <summary>
    /// The base of anything you want to latch onto the Filter property of a <see cref="System.Web.HttpResponse"/>
    /// object.
    /// </summary>
    /// <remarks>
    /// <p></p>These are generally used with <see cref="HttpModule"/> but you could really use them in
    /// other HttpModules.  This is a general, write-only stream that writes to some underlying stream.  When implementing
    /// a real class, you have to override void Write(byte[], int offset, int count).  Your work will be performed there.
    /// </remarks>
    public abstract class HttpOutputFilter : Stream
    {

        private Stream _sink;

        /// <summary>
        /// Subclasses need to call this on contruction to setup the underlying stream
        /// </summary>
        /// <param name="baseStream">The stream we're wrapping up in a filter</param>
        protected HttpOutputFilter(Stream baseStream)
        {
            _sink = baseStream;
        }

        /// <summary>
        /// Allow subclasses access to the underlying stream
        /// </summary>
        protected Stream BaseStream
        {
            get { return _sink; }
        }

        /// <summary>
        /// False.  These are write-only streams
        /// </summary>
        public override bool CanRead
        {
            get { return false; }
        }

        /// <summary>
        /// False.  These are write-only streams
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// True.  You can write to the stream.  May change if you call Close or Dispose
        /// </summary>
        public override bool CanWrite
        {
            get { return _sink.CanWrite; }
        }

        /// <summary>
        /// Not supported.  Throws an exception saying so.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown.  Always.</exception>
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Not supported.  Throws an exception saying so.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown.  Always.</exception>
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Not supported.  Throws an exception saying so.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown.  Always.</exception>
        public override long Seek(long offset, System.IO.SeekOrigin direction)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.  Throws an exception saying so.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown.  Always.</exception>
        public override void SetLength(long length)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Closes this Filter and the underlying stream.
        /// </summary>
        /// <remarks>
        /// If you override, call up to this method in your implementation.
        /// </remarks>
        public override void Close()
        {
            _sink.Close();
        }

        /// <summary>
        /// Fluses this Filter and the underlying stream.
        /// </summary>
        /// <remarks>
        /// If you override, call up to this method in your implementation.
        /// </remarks>
        public override void Flush()
        {
            _sink.Flush();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="buffer">The buffer to write into.</param>
        /// <param name="offset">The offset on the buffer to write into</param>
        /// <param name="count">The number of bytes to write.  Must be less than buffer.Length</param>
        /// <returns>An int telling you how many bytes were written</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

    }

    /// <summary>
    /// Base for any HttpFilter that performing compression
    /// </summary>
    /// <remarks>
    /// When implementing this class, you need to implement a <see cref="HttpOutputFilter"/>
    /// along with a <see cref="CompressingFilter.ContentEncoding"/>.  The latter corresponds to a 
    /// content coding (see http://www.w3.org/Protocols/rfc2616/rfc2616-sec3.html#sec3.5)
    /// that your implementation will support.
    /// </remarks>
    public abstract class CompressingFilter : HttpOutputFilter
    {

        private bool hasWrittenHeaders = false;
        private bool alwaysUseBaseStream = false;

        /// <summary>
        /// Protected constructor that sets up the underlying stream we're compressing into
        /// </summary>
        /// <param name="baseStream">The stream we're wrapping up</param>
        /// <param name="compressionLevel">The level of compression to use when compressing the content</param>
        protected CompressingFilter(Stream baseStream)
            : base(baseStream)
        {
        }

        /// <summary>
        /// The name of the content-encoding that's being implemented
        /// </summary>
        /// <remarks>
        /// See http://www.w3.org/Protocols/rfc2616/rfc2616-sec3.html#sec3.5 for more
        /// details on content codings.
        /// </remarks>
        public abstract string ContentEncoding { get; }

        /// <summary>
        /// Keeps track of whether or not we're written the compression headers
        /// </summary>
        public bool HasWrittenHeaders
        {
            get { return hasWrittenHeaders; }
            set { hasWrittenHeaders = value; }
        }

        public bool AlwaysUseBaseStream
        {
            get { return alwaysUseBaseStream; }
        }

        /// <summary>
        /// Writes out the compression-related headers.  Subclasses should call this once before writing to the output stream.
        /// </summary>
        protected void WriteHeaders()
        {
            // this is dangerous.  if Response.End is called before the filter is used, directly or indirectly,
            // the content will not pass through the filter.  However, this header will still be appended.  
            // Look for handling cases in PreRequestSendHeaders and Pre
            try
            {
                HttpContext.Current.Response.AppendHeader("Content-Encoding", this.ContentEncoding);
                //HttpContext.Current.Response.AppendHeader("X-Compressed-By", "HttpCompress");
            }
            catch
            {
                alwaysUseBaseStream = true;
            }
            hasWrittenHeaders = true;
        }


    }

    /// <summary>
    /// This is a little filter to support HTTP compression using GZip
    /// </summary>
    public class GZipFilter : CompressingFilter
    {

        /// <summary>
        /// compression stream member
        /// has to be a member as we can only have one instance of the
        /// actual filter class
        /// </summary>
        private GZipStream m_stream = null;

        /// <summary>
        /// Primary constructor.  Need to pass in a stream to wrap up with gzip.
        /// </summary>
        /// <param name="baseStream">The stream to wrap in gzip.  Must have CanWrite.</param>
        public GZipFilter(Stream baseStream)
            : base(baseStream)
        {
            m_stream = new GZipStream(baseStream, CompressionMode.Compress);
        }

        /// <summary>
        /// Write content to the stream and have it compressed using gzip.
        /// </summary>
        /// <param name="buffer">The bytes to write</param>
        /// <param name="offset">The offset into the buffer to start reading bytes</param>
        /// <param name="count">The number of bytes to write</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!HasWrittenHeaders)
                WriteHeaders();

            if (AlwaysUseBaseStream)
                BaseStream.Write(buffer, offset, count);
            else
                m_stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// The Http name of this encoding.  Here, gzip.
        /// </summary>
        public override string ContentEncoding
        {
            get { return "gzip"; }
        }

        /// <summary>
        /// Closes this Filter and calls the base class implementation.
        /// </summary>
        public override void Close()
        {
            m_stream.Close(); // this will close the gzip stream along with the underlying stream
            // no need for call to base.Close() here.
        }

        /// <summary>
        /// Flushes the stream out to underlying storage
        /// </summary>
        public override void Flush()
        {
            m_stream.Flush();
        }

    }

    /// <summary>
    /// Summary description for DeflateFilter.
    /// </summary>
    public class DeflateFilter : CompressingFilter
    {
        /// <summary>
        /// compression stream member
        /// has to be a member as we can only have one instance of the
        /// actual filter class
        /// </summary>
        private DeflateStream m_stream = null;

        /// <summary>
        /// Full constructor that allows you to set the wrapped stream and the level of compression
        /// </summary>
        /// <param name="baseStream">The stream to wrap up with the deflate algorithm</param>
        /// <param name="compressionLevel">The level of compression to use</param>
        public DeflateFilter(Stream baseStream)
            : base(baseStream)
        {
            m_stream = new DeflateStream(baseStream, CompressionMode.Compress);
        }

        /// <summary>
        /// Write out bytes to the underlying stream after compressing them using deflate
        /// </summary>
        /// <param name="buffer">The array of bytes to write</param>
        /// <param name="offset">The offset into the supplied buffer to start</param>
        /// <param name="count">The number of bytes to write</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!HasWrittenHeaders)
                WriteHeaders();

            if (AlwaysUseBaseStream)
                BaseStream.Write(buffer, offset, count);
            else
                m_stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Return the Http name for this encoding.  Here, deflate.
        /// </summary>
        public override string ContentEncoding
        {
            get { return "deflate"; }
        }

        /// <summary>
        /// Closes this Filter and calls the base class implementation.
        /// </summary>
        public override void Close()
        {
            m_stream.Close();
        }

        /// <summary>
        /// Flushes that the filter out to underlying storage
        /// </summary>
        public override void Flush()
        {
            m_stream.Flush();
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class JsFilter : HttpOutputFilter
    {
        private List<byte> m_Buffer = new List<byte>();
        public JsFilter(Stream stream)
            :base(stream)
        {
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer.Length == count)
                m_Buffer.AddRange(buffer);
            else
            {
                for (int i = 0; i < count; i++)
                {
                    m_Buffer.Add(buffer[i]);
                }
            }
        }
        public override void Close()
        {
            byte[] buffer = new byte[m_Buffer.Count];

            m_Buffer.CopyTo(buffer);

            string str = System.Text.Encoding.UTF8.GetString(buffer);

            object outputParm = System.Web.HttpContext.Current.Items["jsFilter-param"];

            if (outputParm == null)
            {
                str = string.Concat(@"
document.write('", MaxLabs.bbsMax.StringUtil.ToJavaScriptString(str), @"');
");
            }
            else
            {
                str = string.Concat(@"
document.getElementById('" + outputParm.ToString(), "').innerHTML = '", MaxLabs.bbsMax.StringUtil.ToJavaScriptString(str), @"';
");
            }


            byte[] result = System.Text.Encoding.UTF8.GetBytes(str);

            BaseStream.Write(result, 0, result.Length);
            base.Close();
        }
    }
}