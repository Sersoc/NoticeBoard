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
using MySql.Data.MySqlClient;
using log4net;
namespace NoticeBoard
{
    /// <summary>
    /// WritePage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WritePage : Page
    {
    
        private static readonly ILog Log = LogManager.GetLogger(typeof(WritePage));
        public WritePage()
        {
            InitializeComponent();
            Log.Info("=========start WritePage===========");     
        }

        private void CancleClick(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/MainPage.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }
        
        private void UploadClick(object sender, RoutedEventArgs e)
        {
            //DB연결 정보 저장
            string connStr = "Server=localhost;Database=test;Uid=root;Pwd=P@ssw0rd!@#$;";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            //sql문(INSERT)
            string dbCmd = ($"INSERT INTO writedb (writer,password,title,content) VALUES ('{tbxWriter.Text}','{tbxPassword.Text}', '{tbxTitle.Text}', '{tbxMainText.Text}')" );
            //sql문 실행
            MySqlCommand cmd = new MySqlCommand(dbCmd, conn);
            //쿼리 실행
            cmd.ExecuteNonQuery();
            Log.Info("=========INSERT Content===========");
            Uri uri = new Uri("/MainPage.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);

        }



  
    }
}
