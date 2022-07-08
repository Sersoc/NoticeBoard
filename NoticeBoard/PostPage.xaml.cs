using MySql.Data.MySqlClient;
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
using log4net;
namespace NoticeBoard
{
    
    public partial class PostPage : Page
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WritePage));
        int index;
        string connStr = "Server=localhost;Database=test;Uid=root;Pwd=P@ssw0rd!@#$;";
        string password;
        bool correction = false;
        public PostPage()
        {

            InitializeComponent();
            System.Diagnostics.Trace.WriteLine("hi");
            Log.Info("=========start PostPage===========");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/MainPage.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (password == tbxPassword.Text)
            {
                DbConnect(($"DELETE FROM writedb WHERE bno = {index}"));
                Log.Info($"=========DELETE {index}column===========");
            }
        }

        private void CorrectionClick(object sender, RoutedEventArgs e)
        {

            if (password == tbxPassword.Text && correction == false)
            {
                tbxMainText.IsReadOnly = false;
                correction = true;

            }
            else if (password == tbxPassword.Text && correction == true)
            {
                DbConnect(($"UPDATE  writedb SET content = '{tbxMainText.Text}'"));
                Log.Info($"=========UPDATE {index}column===========");
                tbxMainText.IsReadOnly = true;
                correction = false;
            }
        }
        private void DbConnect(string dbCmd)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(dbCmd, conn);
            cmd.ExecuteNonQuery();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                index = Convert.ToInt32(tbxNo.Text);
                conn.Open();
                string sql = $"SELECT * FROM writedb WHERE bno = {index}";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    tbTitle.Text = dr["title"].ToString();
                    tbWriter.Text = dr["writer"].ToString();
                    tbxMainText.Text = dr["content"].ToString();
                    password = dr["password"].ToString();
                    tbDateTime.Text = "작성일 " + dr["writetime"].ToString();
                }
                dr.Close();
                Log.Info($"=========SELECT {index}column===========");
            }
        }

    }
}
