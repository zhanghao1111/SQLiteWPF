using GalaSoft.MvvmLight;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace SQLiteWPF.NetworkPortCommunication
{
    /// <summary>
    /// 命令行 客户端程序:
    /// </summary>
    public class ClientProgram : ViewModelBase
    {

        private static byte[] result = new byte[1024];

        private static RealTimeData realTimeData = new RealTimeData();
        public RealTimeData RealtimeData
        {
            get { return realTimeData; }
            set
            {
                realTimeData = value;
                RaisePropertyChanged(() => RealtimeData);
            }
        }
        public ClientProgram()
        {
            RealtimeData = new RealTimeData();
        }
        public void ClientProgramTest()
        {

            //设定服务器IP地址  
            IPAddress ip = IPAddress.Parse("192.168.58.232");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {

                clientSocket.Connect(new IPEndPoint(ip, 2000)); //配置服务器IP与端口 
                RealtimeData.MessageLog += "网口测试：连接服务器成功";


            }
            catch (Exception e)
            {
                RealtimeData.MessageLog += "网口测试：连接服务器失败";
                return;
            }

            //通过 clientSocket 发送数据  

            try
            {
                Thread.Sleep(10);    //等待1秒钟  

                string sendMessage = "哈哈哈哈啊哈哈哈哈\r\n";
                RealtimeData.MessageLog += "网口测试：客户端发送此信息" + DateTime.Now + "\r\n";

                clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage));
                RealtimeData.MessageLog += "网口测试：向服务器发送消息" + sendMessage + "\r\n";

                //通过clientSocket接收数据
                int receiveLength = clientSocket.Receive(result);
                RealtimeData.MessageLog += "网口测试：接收服务器消息： " + Encoding.UTF8.GetString(result, 0, receiveLength) + "\r\n";
            }
            catch (Exception e)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }
    }
    /// <summary>
    /// 客户端程序:(同步接收模式)
    /// </summary>
    public class ReceiveSynchronously : ViewModelBase
    {

        private static RealTimeData realTimeData = new RealTimeData();
        public RealTimeData RealtimeData
        {
            get { return realTimeData; }
            set
            {
                realTimeData = value;
                RaisePropertyChanged(() => RealtimeData);
            }
        }
        public ReceiveSynchronously()
        {
            RealtimeData = new RealTimeData();
        }


        private static byte[] result = new byte[1024];

        public void ReceiveSynchronouslyTest()
        {
            //设定服务器IP地址  
            IPAddress ip = IPAddress.Parse("192.168.58.232");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 2000)); //配置服务器IP与端口  
                RealtimeData.MessageLog += "网口同步接收测试：连接服务器成功" + "\r\n";
            }
            catch
            {
                RealtimeData.MessageLog += "网口同步接收测试：连接服务器失败" + "\r\n";
                return;
            }

            Thread socket_send = new Thread(socket_send_callback);
            socket_send.Start(clientSocket);

            Thread socket_receive = new Thread(socket_receive_callback);
            socket_receive.Start(clientSocket);
        }

        void socket_send_callback(object clientSocket)
        {
            Socket client_socket = (Socket)clientSocket;

            try
            {
                while (true)
                {
                    Thread.Sleep(5000);    //等待1秒钟  
                    string sendMessage = "网口同步接收测试：client send Message Hellp" + DateTime.Now + "\r\n";
                    client_socket.Send(Encoding.ASCII.GetBytes(sendMessage));
                    RealtimeData.MessageLog += "网口同步接收测试：向服务器发送消息：" + sendMessage + "\r\n";
                }
            }
            catch
            {
                client_socket.Shutdown(SocketShutdown.Both);
                client_socket.Close();

            }

        }

        //通过 clientSocket 发送数据  
        void socket_receive_callback(object clientSocket)
        {
            Socket client_socket = (Socket)clientSocket;
            while (true)
            {


                int receiveLength = client_socket.Receive(result);//通过clientSocket接收数据  
                if (receiveLength != 0)
                {
                    RealtimeData.MessageLog += "网口同步接收测试：接收服务器消息：" + result.ToString() + "\r\n";
                }
            }
        }
    }
    /// <summary>
    /// 异步接收
    /// </summary>
    public class ReceiveAsynchronously : ViewModelBase
    {
        private static RealTimeData realTimeData = new RealTimeData();
        public RealTimeData RealtimeData
        {
            get { return realTimeData; }
            set
            {
                realTimeData = value;
                RaisePropertyChanged(() => RealtimeData);
            }
        }
        public ReceiveAsynchronously()
        {
            RealtimeData = new RealTimeData();
        }

        private static byte[] result = new byte[1024];
        static byte[] receive_buffer = new byte[1024];

        public void ReceiveAsynchronouslyTest()
        {
            //设定服务器IP地址  
            IPAddress ip = IPAddress.Parse("192.168.58.232");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 2000)); //配置服务器IP与端口  
                RealtimeData.MessageLog += "网口异步接收测试：连接服务器成功\r\n";
            }
            catch
            {
                RealtimeData.MessageLog += "网口异步接收测试：连接服务器失败\r\n";
                return;
            }

            Thread socket_send = new Thread(socket_send_callback);
            socket_send.Start(clientSocket);

            Thread socket_receive = new Thread(socket_receive_callback);
            socket_receive.Start(clientSocket);
        }

        void socket_send_callback(object clientSocket)
        {
            Socket client_socket = (Socket)clientSocket;

            try
            {
                while (true)
                {
                    Thread.Sleep(5000);    //等待1秒钟  
                    string sendMessage = "网口异步接收测试：发送的消息：咯咯咯咯咯咯咯" + DateTime.Now + "\r\n";
                    client_socket.Send(Encoding.ASCII.GetBytes(sendMessage));
                    RealtimeData.MessageLog += "网口异步接收测试：向服务器发送消息：" + sendMessage + "\r\n";
                }

            }
            catch
            {
                client_socket.Shutdown(SocketShutdown.Both);
                client_socket.Close();

            }
        }

        //通过 clientSocket 发送数据  
        void socket_receive_callback(object clientSocket)
        {
            Socket client_socket = (Socket)clientSocket;
            //  异步接收
            client_socket.BeginReceive(receive_buffer, 0, receive_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), client_socket);
        }

        void ReceiveCallback(IAsyncResult result)
        {
            Socket ts = (Socket)result.AsyncState;
            while (true)
            {
                Thread.Sleep(5000);
                //ts.EndReceive(result);
                //result.AsyncWaitHandle.Close();

                //清空数据，重新开始异步接收
                receive_buffer = new byte[receive_buffer.Length];
                ts.BeginReceive(receive_buffer, 0, receive_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), ts);
                RealtimeData.MessageLog += "网口异步接收测试：收到消息:" + receive_buffer[1].ToString() + "\r\n";

            }

        }

    }
    public class Yibu : ViewModelBase 
    {
        private static RealTimeData realTimeData = new RealTimeData();
        public RealTimeData RealtimeData
        {
            get { return realTimeData; }
            set
            {
                realTimeData = value;
                RaisePropertyChanged(() => RealtimeData);
            }
        }
        public Yibu()
        {
            RealtimeData = new RealTimeData();
            ResetT3Timeout();
            Thread workerThread = new Thread(ConnectSocket);
            // 启动线程
            workerThread.IsBackground = true;
            workerThread.Start();
        }
       
        /// <summary>
        /// 1970年1月1日当天时间
        /// </summary>
        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 从1970年1月1日到当前日期总毫秒数
        /// </summary>
        /// <returns></returns>
        public static long currentTimeMillis()
        {
            return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
        }

        /// <summary>
        /// 长期空闲状态下发送测试帧的超时，设定值为20s
        /// </summary>
        private UInt64 nextT1Timeout;

        /// <summary>
        /// 重置长期空闲状态下发送测试帧的超时(T3)
        /// </summary>
        private void ResetT3Timeout()
        {
            this.nextT1Timeout = (UInt64)currentTimeMillis() + (UInt64)(4 * 1000);
        }
        /// <summary>
        /// 连接套接字创建
        /// </summary>
        private void ConnectSocket()
        {
            bool running = true;
            IPEndPoint remoteEP;
            Socket socket;
            try
            {
                IPAddress ipAddress = IPAddress.Parse("192.168.58.232");
                //用指定的地址和端口号初始化 System.Net.IPEndPoint 类的新实例
                remoteEP = new IPEndPoint(ipAddress, 2000);
            }
            catch (Exception e)
            {
                RealtimeData.MessageLog +="--------------IP地址和端口号有误---------------\r\n";
                // wrong argument
                throw new Exception("SocketException:IP地址和端口号有误" + e.Message, new SocketException(87));
            }

            RealtimeData.MessageLog += "--------------IP地址和端口号初始化成功---------------\r\n";

            try
            {
                // 创建套接字.Create a TCP/IP  socket.
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception e)
            {
                RealtimeData.MessageLog += "--------------套接字创建失败---------------\r\n";
                // wrong argument
                throw new Exception("SocketException:套接字创建失败" + e.Message, new SocketException(87));
            }

            RealtimeData.MessageLog += "--------------套接字创建成功---------------\r\n";

            try
            {
                // 开始一个对远程主机连接的异步请求
                var result = socket.BeginConnect(remoteEP, null, null);
                RealtimeData.MessageLog += "--------------开始一个对远程主机连接的异步请求---------------\r\n";

                // 阻止当前线程，直到当前的 System.Threading.WaitHandle 收到信号为止，
                // 同时使用 32 位带符号整数指定时间间隔，并指定是否在等待之前退出同步域
                bool success = result.AsyncWaitHandle.WaitOne(1000, true);
                RealtimeData.MessageLog += "--------------阻止当前线程，直到当前的 System.Threading.WaitHandle 收到信号---------------\r\n";
                if (success)
                {
                    RealtimeData.MessageLog += "--------------结束挂起的异步连接请求---------------\r\n";
                    // 结束挂起的异步连接请求
                    socket.EndConnect(result);
                    running = true;
                }
                else
                {
                    // 关闭 System.Net.Sockets.Socket 连接并释放所有关联的资源
                    socket.Close();

                    RealtimeData.MessageLog += "--------------套接字关闭,连接时间超时---------------\r\n";

                    // Connection timed out.
                    throw new Exception("SocketException:套接字关闭,连接时间超时", new SocketException(10060));
                }
            }
            catch (Exception e)
            {
                RealtimeData.MessageLog += "--------------对远程主机连接的异步请求失败---------------\r\n";
 
                //throw new Exception("SocketException:对远程主机连接的异步请求失败。" + e.Message, new SocketException(10060));
            }

            //接收数据
            while (running)
            {
                try
                {
                    byte[] buffer = new byte[300];

                    int ret = socket.Receive(buffer);

                    if (ret == 0)
                    { 
                        running = false;
                        RealtimeData.MessageLog += "hahahahhahahahhhahahah" + ret.ToString() + "\r\n";

                    }
                    // 获取当前总毫秒数
                    UInt64 currentTime = (UInt64)currentTimeMillis();
                    if (currentTime > nextT1Timeout)
                    {
                        running = false;
                    }
                    else
                        ResetT3Timeout();
                    RealtimeData.MessageLog += "接收到消息数量：" + ret.ToString() + "\r\n";
                }
                catch (Exception ex)
                {
                    running = false;
                    MessageBox.Show(ex.Message ,"提示"); 
                }
                
 
            }

            // 关闭Socket连接并释放所有关联的资源
            RealtimeData.MessageLog += "关闭Socket连接并释放所有关联的资源";
            socket.Close();
            RealtimeData.MessageLog += "退出当前线程";
            System.Threading.Thread.CurrentThread.Abort();
        }
    }
}

