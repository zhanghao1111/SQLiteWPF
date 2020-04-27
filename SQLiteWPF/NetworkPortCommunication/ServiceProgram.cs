using GalaSoft.MvvmLight;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace SQLiteWPF.NetworkPortCommunication
{

    class ServiceProgram : ViewModelBase
    {
        private static byte[] result = new byte[1024];
        private static int myProt = 2000;   //端口
        static Socket serverSocket;
        static Socket clientSocket;

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

        public void ServiceTest()
        {
            //服务器IP地址
            IPAddress ip = IPAddress.Parse("192.168.137.1");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口
            serverSocket.Listen(10);                        //设定最多10个排队连接请求
            RealtimeData.MessageLog2 += "启动监听"+  serverSocket.LocalEndPoint.ToString()+"成功\r\n";
            //通过Clientsoket发送数据
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
        }

        /// <summary>
        /// 监听客户端连接
        /// </summary>

        private   void ListenClientConnect()
        {
            while (true)
            {
                clientSocket = serverSocket.Accept();
                //   clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));

                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);
            }
        }


        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="clientSocket"></param>
  
        private   void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    //通过clientSocket接收数据
                    int receiveNumber = myClientSocket.Receive(result);
                    RealtimeData.MessageLog2  += "接收客户端" + myClientSocket.RemoteEndPoint.ToString() + "    消息数量：" + receiveNumber.ToString() + "\r\n";
                   
                    myClientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));

                }
                catch (Exception ex)
                {
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    MessageBox.Show(ex.Message,"提示");
                    break;
                }
            }
        }
    }
}
