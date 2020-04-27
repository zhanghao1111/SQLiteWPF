using SQLiteWPF.NetworkPortCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SQLiteWPF.View.网口
{
    /// <summary>
    /// Service.xaml 的交互逻辑
    /// </summary>
    public partial class Service : Page
    {
        ServiceProgram service = new ServiceProgram();
        public Service()
        {
            InitializeComponent();
            DataContext = service;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            service.ServiceTest();
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            service.RealtimeData.MessageLog = "";
            service.RealtimeData.MessageLog2 = "";
            MessageLog.Text = "";
        }

        private void MessageLog_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.MessageLog.ScrollToEnd();
        }
    }
}
