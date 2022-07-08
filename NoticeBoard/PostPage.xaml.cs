using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using log4net;
namespace NoticeBoard
{
    /// <summary>
    /// 게시물 페이지
    /// </summary>
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
            Log.Info("=========start PostPage===========");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/MainPage.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (password == tbxPassword.Password)
            {
                //DELETE query
                DbConnect(($"DELETE FROM writedb WHERE bno = {index}"));
                Log.Info($"=========DELETE bno {index} column===========");
            }
        }

        private void CorrectionClick(object sender, RoutedEventArgs e)
        {

            if (password == tbxPassword.Password && correction == false)
            {
                tbxMainText.IsReadOnly = false;
                correction = true;

            }
            else if (password == tbxPassword.Password && correction == true)
            {
                //UPDATE query
                DbConnect(($"UPDATE  writedb SET content = '{tbxMainText.Text}' where bno = {index}"));
                Log.Info($"=========UPDATE bno {index} column===========");
                tbxMainText.IsReadOnly = true;
                correction = false;
            }
        }
        private void DbConnect(string dbCmd)
        {
            //DB로 query문 전송
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(dbCmd, conn);
            cmd.ExecuteNonQuery();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //DB table에서 데이터 불러오기
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                index = Convert.ToInt32(tbxNo.Text);
                conn.Open();
                string sql = $"SELECT title,writer,content,password,writetime FROM writedb WHERE bno = {index}";

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
                Log.Info($"=========SELECT bno {index} column===========");
            }
        }

    }
}
