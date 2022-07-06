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

namespace NoticeBoard
{
    /// <summary>
    /// WritePage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WritePage : Page
    {
        public WritePage()
        {
            InitializeComponent();
        }

        private void CancleClick(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/MainPage.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void UploadClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
