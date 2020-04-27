using SQLiteWPF.NetworkPortCommunication;
using System.Windows;
using System.Windows.Controls;

namespace SQLiteWPF.View.网口
{
    /// <summary>
    /// Client.xaml 的交互逻辑
    /// </summary>
    public partial class Client : Page
    {
        ClientProgram clientProgram = new ClientProgram();
        ReceiveAsynchronously receiveAsynchronously = new ReceiveAsynchronously();
        ReceiveSynchronously receiveSynchronously = new ReceiveSynchronously();
        Yibu yibu = new Yibu();
        public Client()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageLog.Text = "";
            clientProgram.RealtimeData.MessageLog = "";
            clientProgram.RealtimeData.MessageLog2 = "";
            receiveSynchronously.RealtimeData.MessageLog = "";
            receiveSynchronously.RealtimeData.MessageLog2 = "";
            receiveAsynchronously.RealtimeData.MessageLog = "";
            receiveAsynchronously.RealtimeData.MessageLog2 = "";
            yibu.RealtimeData.MessageLog = "";
            yibu.RealtimeData.MessageLog2 = "";
        }

        private void MessageLog_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.MessageLog.ScrollToEnd();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            DataContext = clientProgram;
            clientProgram.ClientProgramTest();
        }
        private void Button_Click1(object sender, RoutedEventArgs e)
        {
           DataContext = receiveSynchronously;
            receiveSynchronously.ReceiveSynchronouslyTest();
        }
        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            yibu = new Yibu();
            DataContext = yibu;
           
            //receiveAsynchronously.ReceiveAsynchronouslyTest();

        }
    }
}
