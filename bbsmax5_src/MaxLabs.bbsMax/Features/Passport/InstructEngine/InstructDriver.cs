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
using MaxLabs.bbsMax.Settings;
using System.Threading;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using System.Xml;
using System.Xml.Serialization;
using System.Web.Services.Protocols;
using System.Diagnostics;
using System.Web;
using MaxLabs.bbsMax.PassportClientInterface;
using ClientInstruct = MaxLabs.bbsMax.PassportClientInterface.Instrcut;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Passport
{
    public class InstructDriver : IDisposable
    {
        public bool IsDisposed { get; private set; }
        private int SendCount = 0;                              //已发生的条数（成功的）
        private double ElapsedTime;                             //总网络消耗时间，（毫秒）
        private Stopwatch watch;
        private ClientInterface ClientService;  //
        private int QueueSize = 1000;                           //队列大小
        private Thread thread;
        private bool HasDbData;                                //标记数据库是否有指令
        private bool IsOpenQueue = false;                      //队列是否已经打开？ 如果数据库有记录， 将要先处理数据库的记录， 才能打开队列
        private int Timeout = int.MaxValue;//86400;                           //客户端超时时间设置，默认1分钟（60s）   86400 是一天      
        private int SustaineLostInstructCount = 0;             //连续发送失败的次数计数器
        private object locker = new object();
        private bool IsStop = false;
        private DateTime LastSuccessTime;

        #region 构造函数
        internal InstructDriver(PassportClient client)
        {
            LogHelper.CreateLog(null, string.Format("ID为{0}的Passport客户端启动", client.ClientID), "passport.log");
            this.Client = client;
            this.InstructList = new Queue<Instruct>(this.QueueSize);
            watch = new Stopwatch();
            ClientService = new ClientInterface();
            ClientService.Url = UrlUtil.JoinUrl(Client.Url.Trim(), client.APIFilePath);
            ClientService.AllowAutoRedirect = true;



            LastSuccessTime = DateTime.Now;
            LoadDbInstruct();//载入数据库数据

            if (this.InstructList.Count>0)
            {
                this.thread = new Thread(ThreadStart);
                thread.IsBackground = true;
                thread.Start();
            }
            else
                this.IsOpenQueue = true; //没有数据库记录， 打开队列
        }

        #endregion

        #region 处理数据库冗余指令列表

        private void ThreadStart()
        {
            while (true)
            {
                if (IsStop) break; //完全终止

                int sleepInteral = 0;
                sleepInteral = IsDisposed ? 1000 :
                    InstructList.Count == 0 ? 100 : 0;

                if (sleepInteral > 0)
                {
                    Thread.Sleep(sleepInteral);
                }
                if (InstructList.Count == 0) continue;

                SendInstruct();
            }
        }

        private void LoadDbInstruct()
        {
            int lave;


            int currentsize = this.InstructList.Count;

            int loadCount = this.QueueSize - currentsize;

            if (loadCount == 0) return;

            List<Instruct> instructs = PassportBO.Instance.LoadInstruct(this.Client.ClientID, loadCount, out lave);

            foreach (Instruct ins in instructs)
            {
                currentsize++;
                if (currentsize > this.QueueSize)
                {
                    HasDbData = true;
                    break;
                }
                this.InstructList.Enqueue(ins);
            }
            HasDbData = lave > 0 && instructs.Count > 0;
        }
        #endregion

        /// <summary>
        /// 指令队列
        /// </summary>
        private Queue<Instruct> InstructList
        {
            get;
            set;
        }

        /// <summary>
        /// 如果客户端当掉，使得驱动停止， 通过这个接口可以重启命令驱动。
        /// </summary>
        public void Restart()
        {
            if (this.IsDisposed)
            {
                this.IsDisposed = false;
                if (this.thread == null && !this.thread.IsAlive)
                {
                    this.thread = new Thread(ThreadStart);
                    thread.Start();
                }
            }
        }

        public void Stop()
        {
            IsStop = true;
            if (this.thread != null && this.thread.IsAlive)
            {
                this.thread.Join();
            }
            this.InstructList.Clear();
        }

        /// <summary>
        /// 绑定的客户端
        /// </summary>
        public PassportClient Client
        {
            get;
            private set;
        }

        /// <summary>
        /// 返回平均耗时
        /// </summary>
        public double AverageTime
        {
            get
            {
                if (SendCount == 0) return 0;
                return Math.Round( ElapsedTime / SendCount,0);
            }
        }

        /// <summary>
        /// 当前队列指令数量
        /// </summary>
        public int CurrentInstructCount
        {
            get
            {
                return InstructList.Count;
            }
        }

        /// <summary>
        /// 最后通讯时间
        /// </summary>
        public DateTime LastConnectTime {

            get
            {
                return LastSuccessTime;
            }
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="ins"></param>
        private bool SendInstruct(Instruct[] ins, bool isSync)
        {
            if (IsDisposed)
                return false;

            watch.Reset();
            watch.Start();


            ClientInstruct[] instructs = new ClientInstruct[ins.Length];

            int i = 0;

            //转换
            foreach (Instruct instruct in ins)
            {
                ClientInstruct clientInstruct = new ClientInstruct();
                clientInstruct.InstructID = instruct.InstructID;
                if (instruct.Datas != null)
                {
                    clientInstruct.Datas = instruct.Datas.Trim('\0');
                }
                else
                {
                    clientInstruct.Datas = string.Empty;
                }
                clientInstruct.InstructType = (int)instruct.InstructType;
                clientInstruct.CreateDateTime = instruct.CreateDate;
                clientInstruct.TargetID = instruct.TargetID;
                instructs[i++] = clientInstruct;
            }

            try
            {
                ClientService.ReceiveInstruct(instructs);
            }
            catch (SoapException ex)//客户端异常， 跳过
            {
                LogHelper.CreateLog(ex, ex.Message+"（指令被跳过）", string.Format("passport_{0}_Exception_{1}.log", this.Client.ClientID,DateTime.Now.ToString("yyyyMMdd")));
                if (isSync)
                    throw;
            }
            catch (Exception ex)
            {
                LogHelper.CreateLog(ex, ex.Message, string.Format("passport_{0}_Exception_{1}.log", this.Client.ClientID, DateTime.Now.ToString("yyyyMMdd")));

                if (isSync)
                    throw;
                watch.Reset();
                return false;
            }

            SendCount++;
            watch.Stop();
            ElapsedTime += watch.Elapsed.TotalMilliseconds;
            if (watch.ElapsedMilliseconds > 500)
            {
                string msg =string.Concat( 
                    "InstructType = "+ins[0].InstructType,"\r\n"
                ,   "Time = ", watch.Elapsed.TotalSeconds, "\r\n"
                ,  " ClientID=", this.Client.ClientID,"\r\n");

                LogHelper.CreateLog(null, msg, string.Format("InstructCall_{0}.txt", DateTime.Now.ToString("yyyy-MM-dd")));
            }

            return true;
        }

        #region 数据库操作
        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="ins"></param>
        /// <param name="error"></param>
        private long WriteToDB(Instruct ins)
        {
            return PassportBO.Instance.WriteInstruct(this.Client.ClientID, ins.TargetID, ins.CreateDate, ins.InstructType, ins.Datas);
        }

        private void DeleteFromDB(Instruct ins)
        {
            PassportBO.Instance.DeleteFromDB(new long[] { ins.InstructID });
        }

        #endregion

        /// <summary>
        /// 从队列取一条指令， 并发送
        /// </summary>
        /// <param name="ins"></param>
        private void SendInstruct()
        {
            Instruct ins = this.InstructList.Peek();  //取指令 
            ins.SendTimes++;                          //尝试次数+1
            if (SendInstruct(new Instruct[] { ins }, false))                    //执行， 是否成功
            {
                this.InstructList.Dequeue();
                DeleteFromDB(ins);                    //吧这条指令从数据库删除

                if (InstructList.Count == 0)
                {
                    if (HasDbData)                        //如果数据库还有指令， 取数据库指令来发送
                    {
                        LoadDbInstruct();
                    }
                    else
                    {
                        IsOpenQueue = true;
                    }
                }
                LastSuccessTime = DateTime.Now;
                SustaineLostInstructCount = 0;
            }
            else
            {
                if ((DateTime.Now - LastSuccessTime).TotalSeconds > Timeout && SustaineLostInstructCount > 10) //判断最后成功时间和当前时间是否超过规定值，并且连续发送失败的次数是否达到10次
                {
                    this.Dispose(); //客户端严重超时，关闭
                }
                else
                {
                    Thread.Sleep(ins.SendTimes);
                }
                SustaineLostInstructCount++;
            }
        }

        /// <summary>
        /// 发送异步指令
        /// </summary>
        /// <param name="instruct"></param>
        private void SendInstructAsync(Instruct instruct)
        {
            instruct.InstructID = WriteToDB(instruct);  //先入库

            if (IsDisposed) return;                      //对象已经销毁

            if (!IsOpenQueue || HasDbData)
            {
                HasDbData = true;
                return;
            }

            if (this.InstructList.Count < this.QueueSize)
            {
                this.InstructList.Enqueue(instruct); //入列
                if (thread == null || !thread.IsAlive)
                {
                    lock (locker)
                    {
                        if (thread == null || !thread.IsAlive)
                        {
                            this.thread = new Thread(ThreadStart);
                            thread.IsBackground = true;
                            thread.Start();
                        }
                    }
                }
            }
            else
            {
                this.IsOpenQueue = false;
                HasDbData = true;
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            IsDisposed = true;
            this.InstructList.Clear();
        }

        #endregion

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="instruct"></param>
        public void AppendInstruct(Instruct instruct)
        {
            if (IsStop) return; //完全终止
            instruct.ClientID = this.Client.ClientID;

            if (!instruct.IsSync)
                SendInstructAsync(instruct);
            else
                SendInstruct(new Instruct[] { instruct }, true);
        }
    }
}