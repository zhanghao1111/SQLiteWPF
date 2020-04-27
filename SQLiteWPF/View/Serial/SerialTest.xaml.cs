using SQLiteWPF.串口;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SQLiteWPF.View.串口
{
    /// <summary>
    /// Serial.xaml 的交互逻辑
    /// </summary>
    public partial class Serial : Page
    {
        SerialTest serialTest = new SerialTest();
        public Serial()
        {
            InitializeComponent();
            DataContext = serialTest;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            serialTest.OpenSerial();
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            serialTest.CloseSerial();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            serialTest.MessageLog = "";
            MessageLog1.Text = " ";
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            this.MessageLog1.ScrollToEnd();
        }

        private void MenuItem_Click1(object sender, RoutedEventArgs e)
        {

        }

        private void TextChanged1(object sender, TextChangedEventArgs e)
        {

        }
         
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 byte[] byteArray = System.Text.Encoding.Default.GetBytes(MessageLog2.Text);
          
                serialTest.serialPort.Write(byteArray, 0, byteArray.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                 
            }
           
        }
    }
}
