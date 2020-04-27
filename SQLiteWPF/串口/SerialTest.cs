using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SQLiteWPF.串口
{
    class SerialTest : ViewModelBase
    {
        /// <summary>
        /// 输出调试信息
        /// </summary>
        private string messageLog;

        public string MessageLog
        {
            get { return messageLog; }
            set { messageLog = value; RaisePropertyChanged(() => MessageLog); }
        }

        private ObservableCollection<int> _Baud;
        /// <summary>
        /// 波特率
        /// </summary>
        public ObservableCollection<int> Baud
        {
            set
            {
                _Baud = value;
                RaisePropertyChanged(() => Baud);
            }
            get
            {
                return _Baud;
            }
        }
        public static ObservableCollection<string> _PortNum;
        /// <summary>
        /// 串口号
        /// </summary>
        public ObservableCollection<string> PortNum
        {
            set
            {
                _PortNum = value;
                RaisePropertyChanged(() => PortNum);
            }
            get
            {
                return _PortNum;
            }
        }


        private ObservableCollection<Parity> _ParityBit;
        /// <summary>
        /// 校验位
        /// </summary>
        public ObservableCollection<Parity> ParityBit
        {
            set
            {
                _ParityBit = value;
                RaisePropertyChanged(() => ParityBit);
            }
            get
            {
                return _ParityBit;
            }
        }


        private ObservableCollection<int> _DataBit;
        /// <summary>
        /// 数据位
        /// </summary>
        public ObservableCollection<int> DataBit
        {
            set
            {
                _DataBit = value;
                RaisePropertyChanged(() => DataBit);
            }
            get
            {
                return _DataBit;
            }
        }

        private ObservableCollection<StopBits> _StopBit;
        /// <summary>
        /// 停止位
        /// </summary>
        public ObservableCollection<StopBits> StopBit
        {
            set
            {
                _StopBit = value;
                RaisePropertyChanged(() => StopBit);
            }
            get
            {
                return _StopBit;
            }
        }
        public static int _SelectedIndexPortNum;
        int SelectedIndexLast;
        /// <summary>
        /// 选中的串口号索引
        /// </summary>
        public int SelectedIndexPortNum
        {
            get
            {
                return _SelectedIndexPortNum;
            }
            set
            {
                SelectedIndexLast = _SelectedIndexPortNum;
                _SelectedIndexPortNum = value;
                RaisePropertyChanged(() => SelectedIndexPortNum);
                if (value != -1 && value != SelectedIndexLast && SelectedIndexLast != -1)
                {
                  //  ReLink();
                }
            }
        }

        private int _SelectedIndexBaud;
        /// <summary>
        /// 选中的波特率索引
        /// </summary>
        public int SelectedIndexBaud
        {
            get
            {
                return _SelectedIndexBaud;
            }
            set
            {
                _SelectedIndexBaud = value;
                RaisePropertyChanged(() => SelectedIndexBaud);
                if (value != -1)
                {
                 //  ReLink();
                }
            }
        }

        private int _SelectedIndexDataBit;
        /// <summary>
        /// 选中的数据位索引
        /// </summary>
        public int SelectedIndexDataBit
        {
            get
            {
                return _SelectedIndexDataBit;
            }
            set
            {
                _SelectedIndexDataBit = value;
                RaisePropertyChanged(() => SelectedIndexDataBit);
                if (value != -1)
                {
                   // ReLink();
                }
            }
        }


        private int _SelectedIndexStopBit;
        /// <summary>
        /// 选中的停止位索引
        /// </summary>
        public int SelectedIndexStopBit
        {
            get
            {
                return _SelectedIndexStopBit;
            }
            set
            {
                _SelectedIndexStopBit = value;
                RaisePropertyChanged(() => SelectedIndexStopBit);
                if (value != -1)
                {
                  //  ReLink();
                }
            }
        }

        private int _SelectedIndexParityBit;
        /// <summary>
        /// 选中的校验位索引
        /// </summary>
        public int SelectedIndexParityBit
        {
            get
            {
                return _SelectedIndexParityBit;
            }
            set
            {
                _SelectedIndexParityBit = value;
                RaisePropertyChanged(() => SelectedIndexParityBit);
                if (value != -1)
                {
                    //ReLink();
                }
            }
        }
        /// <summary>
        /// 载入串口号
        /// </summary>
        public RelayCommand LoadPortNumber { get; private set; }
        public void ExecuteLoadPortNumber()
        {
            PortNum.Clear();
            foreach (string s in SerialPort.GetPortNames())
            {
                PortNum.Add(s);
            }
            SelectedIndexPortNum = SelectedIndexLast;
        }

        public SerialPort serialPort = new SerialPort();
        /// <summary>
        /// 打开串口
        /// </summary>
        public void OpenSerial()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    MessageBox.Show("串口" + serialPort.PortName.ToString() + "已打开!");
                    MessageLog += "串口" + serialPort.PortName.ToString() + "已打开!\r\n";
                }
                else
                {
                    serialPort.PortName = PortNum[SelectedIndexPortNum];
                    serialPort.BaudRate = Baud[SelectedIndexBaud];
                    serialPort.Parity = ParityBit[SelectedIndexParityBit];
                    serialPort.DataBits = DataBit[SelectedIndexDataBit];
                    serialPort.StopBits = StopBit[SelectedIndexStopBit];
                    serialPort.Open();
                    if (serialPort.IsOpen)
                    {
                        MessageLog += "打开串口" + PortNum[SelectedIndexPortNum].ToString() + "成功\r\n";
                    }
                    else
                    {
                        MessageLog += "打开串口" + PortNum[SelectedIndexPortNum].ToString() + "失败\r\n";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageLog += ex.ToString() + "\r\n";
            }
           
        }

        public void  CloseSerial()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                if (serialPort.IsOpen)
                {
                    MessageLog += "关闭串口" + PortNum[SelectedIndexPortNum].ToString() + "失败\r\n";
                }
                else
                    MessageLog += "关闭串口" + PortNum[SelectedIndexPortNum].ToString() + "成功\r\n";
            }
            else
                MessageLog += "当前无串口连接\r\n"; ;
        }

        /// <summary>
        /// 串口接收报文
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;

            int n = serialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
            sp.Read(buf, 0, n);//读取缓冲数据  

            string buff = System.Text.Encoding.Default.GetString(buf); //xx="中文"; 
            MessageLog += buff;
        }
        /// <summary>
        /// 发送函数
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void serialPort_DataSend(byte[] buffer, int offset, int count)
        {
            serialPort.Write(buffer,offset,count);
        }

        public SerialTest()
        {
            //方法
            LoadPortNumber = new RelayCommand(ExecuteLoadPortNumber);
            //串口接收报文
            serialPort.DataReceived += serialPort_DataReceived;

            #region 串口参数

            //波特率初始化
            Baud = new ObservableCollection<int>
            {
                1200,
                2400,
                4800,
                9600,
                38400,
                115200
            };
            //数据位初始化
            DataBit = new ObservableCollection<int>
            {
                8,
                9
            };
            //校验位初始化
            ParityBit = new ObservableCollection<Parity>
            {
                Parity.Even,
                Parity.Mark,
                Parity.None,
                Parity.Odd,
                Parity.Space
            };
            //停止位初始化 
            StopBit = new ObservableCollection<StopBits>
            {
                StopBits.None,
                StopBits.One,
                StopBits.OnePointFive,
                StopBits.Two
            };

            PortNum = new ObservableCollection<string>();
            foreach (string s in SerialPort.GetPortNames())
            {
                PortNum.Add(s);
            }
            _SelectedIndexPortNum = 0;
            _SelectedIndexBaud = 3;
            _SelectedIndexDataBit = 0;
            _SelectedIndexStopBit = 1;
            _SelectedIndexParityBit = 2;
            #endregion
        }
    }
}
