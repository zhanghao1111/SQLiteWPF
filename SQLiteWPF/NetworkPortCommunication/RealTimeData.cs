using GalaSoft.MvvmLight;

namespace SQLiteWPF.NetworkPortCommunication
{
    public class RealTimeData : ViewModelBase
    {
        /// <summary>
        /// 输出调试信息1
        /// </summary>
        private string messageLog;

        public string MessageLog
        {
            get { return messageLog; }
            set { messageLog = value; RaisePropertyChanged(() => MessageLog); }
        }
        /// <summary>
        /// 输出调试信息2
        /// </summary>
        private string messageLog2;

        public string MessageLog2
        {
            get { return messageLog2; }
            set { messageLog2= value; RaisePropertyChanged(() => MessageLog2); }
        }
    }
}
