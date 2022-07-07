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

namespace NoticeBoard
{

    public partial class PostPage : Page
    {

        int index;
        string connStr = "Server=localhost;Database=test;Uid=root;Pwd=P@ssw0rd!@#$;";
        string password;
        bool correction = false;
        public PostPage()
        {

            InitializeComponent();

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/MainPage.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (password == tbxPassword.Text && index != null)
            {
                DbConnect(($"DELETE FROM post WHERE bno = {index}"));
              
            }
        }

        private void CorrectionClick(object sender, RoutedEventArgs e)
        {
            
            if (password == tbxPassword.Text && correction == false)
            {
                tbxMainText.IsReadOnly = false;
                correction = true;

            }
            else if(password == tbxPassword.Text && correction == true)
            {
                DbConnect(($"UPDATE  post SET maintext = '{tbxMainText.Text}'"));
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
                string sql = $"SELECT * FROM post WHERE bno = {index}";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Title.Text = rdr["title"].ToString();
                    Writer.Text = rdr["writer"].ToString();
                    tbxMainText.Text = rdr["maintext"].ToString();
                    password = rdr["pasw"].ToString();
                    DateTime.Text = "작성일 " + rdr["writetime"].ToString();
                }
                rdr.Close();
            }
        }
    }
}
