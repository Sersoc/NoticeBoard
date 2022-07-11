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
using System.Data;

namespace NoticeBoard
{
    /// <summary>
    /// MainPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainPage : Page
    {
        string connectionString = $"Server=192.168.0.102,Port:3306;Database=board;Uid=dwsmartict;Pwd=P@ssw0rd!@#$;Allow User Variables=True";
        int listCnt = 0; //전체 row 개수를 10으로 나누어 page가 몇개인지 저장
        int curPageNum = 1; // 현재 페이지
        int totalPageBatchCnt = 0; //전체 페이지를 5개로 묶은 갯수 저장
        int curBatchPos = 1; //현재 페이지가 5개씩 묶은 것 중에서 몇번 째에 있는지 저장하는 변수
        string findString = "";
        bool isFindList = false;

        public MainPage()
        {
            InitializeComponent();

            listCnt = GetRowCnt() / 10;
            if (GetRowCnt() % 10 != 0)
                listCnt++;

            totalPageBatchCnt = listCnt / 5;
            if (listCnt % 5 != 0)
                totalPageBatchCnt++;

            ShowPageSelBtn();
            ShowList();
            txb_page_btn_1.Foreground = Brushes.Blue;
        }

        /// <summary>
        /// db에 저장된 전체 글 개수 구하기
        /// </summary>
        /// <returns>전체 행 개수</returns>
        private int GetRowCnt()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string sql = "select count(*) cnt from writedb";

                    MySqlCommand command = new MySqlCommand(sql, connection);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
                catch (MySqlException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    return -1;
                }
            }
        }

        /// <summary>
        /// 페이징 쿼리를 이용해 한번에 10개씩만 출력되게 보여줌
        /// </summary>
        private void ShowList()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string sql = $"select (@num:=@num+1) as no, title, writer, writertime, bno from writedb, (select @num:={curPageNum * 10 - 10}) orders limit 10 offset {Convert.ToString(curPageNum * 10 - 10)}";

                    MySqlCommand command = new MySqlCommand(sql, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataSet ds = new DataSet();

                    adapter.Fill(ds, "DataBinding");
                    dgTable.DataContext = ds;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    return;
                }
            }
        }

        private void VisibleButton(params Button[] buttons)
        {
            foreach (Button button in buttons)
            {
                button.Visibility = Visibility.Visible;
            }
        }

        private void CollapseButton(params Button[] buttons)
        {
            foreach (Button button in buttons)
            {
                button.Visibility = Visibility.Collapsed;
            }
        }

        private void TextBlockTextChange(params TextBlock[] textBlocks)
        {
            for (int i = 0; i < textBlocks.Length; i++)
            {
                textBlocks[i].Text = Convert.ToString(curBatchPos * 5 - (4 - i));
            }
        }

        /// <summary>
        /// 페이지 묶음에서 5개보다 적거나 같은 경우 디스플레이
        /// </summary>
        /// <param name="val"></param>
        private void BtnVisible_1(int val)
        {
            switch (val)
            {
                case 1:
                    VisibleButton(btn_page_1);
                    CollapseButton(btn_page_2, btn_page_3, btn_page_4, btn_page_5);
                    TextBlockTextChange(txb_page_btn_1);
                    break;
                case 2:
                    VisibleButton(btn_page_1, btn_page_2);
                    CollapseButton(btn_page_3, btn_page_4, btn_page_5);
                    TextBlockTextChange(txb_page_btn_1, txb_page_btn_2);
                    break;
                case 3:
                    VisibleButton(btn_page_1, btn_page_2, btn_page_3);
                    CollapseButton(btn_page_4, btn_page_5);
                    TextBlockTextChange(txb_page_btn_1, txb_page_btn_2, txb_page_btn_3);
                    break;
                case 4:
                    VisibleButton(btn_page_1, btn_page_2, btn_page_3, btn_page_4);
                    CollapseButton(btn_page_5);
                    TextBlockTextChange(txb_page_btn_1, txb_page_btn_2, txb_page_btn_3, txb_page_btn_4);
                    break;
                case 5:
                    VisibleButton(btn_page_1, btn_page_2, btn_page_3, btn_page_4, btn_page_5);
                    TextBlockTextChange(txb_page_btn_1, txb_page_btn_2, txb_page_btn_3, txb_page_btn_4, txb_page_btn_5);
                    break;
            }
        }

        private void ShowPageSelBtn()
        {

            if (curBatchPos == 1) //제일 첫 페이지 묶음 위치
            {
                if (listCnt <= 5)
                {
                    BtnVisible_1(listCnt);  //페이지가 5개 이하일 때
                }
                else if (listCnt > 5)
                {
                    VisibleButton(btn_page_1, btn_page_2, btn_page_3, btn_page_4, btn_page_5);
                    TextBlockTextChange(txb_page_btn_1, txb_page_btn_2, txb_page_btn_3, txb_page_btn_4, txb_page_btn_5);
                    btn_next_page.Visibility = Visibility.Visible;
                }
                btn_pre_page.Visibility = Visibility.Hidden;
            }
            else if (curBatchPos == totalPageBatchCnt) // 마지막 페이지 묶음
            {
                if (listCnt % 5 != 0)
                {
                    BtnVisible_1(listCnt % 5 != 0 ? listCnt % 5 : listCnt % 5 + 1); //마지막 페이지 묶음에서 5개 보다 페이지 수가 적을 경우
                }
                else
                {
                    VisibleButton(btn_page_1, btn_page_2, btn_page_3, btn_page_4, btn_page_5);
                    TextBlockTextChange(txb_page_btn_1, txb_page_btn_2, txb_page_btn_3, txb_page_btn_4, txb_page_btn_5);
                }
                btn_next_page.Visibility = Visibility.Hidden;
                btn_pre_page.Visibility = Visibility.Visible;
            }
            else
            {
                VisibleButton(btn_page_1, btn_page_2, btn_page_3, btn_page_4, btn_page_5);
                TextBlockTextChange(txb_page_btn_1, txb_page_btn_2, txb_page_btn_3, txb_page_btn_4, txb_page_btn_5);
                btn_pre_page.Visibility = Visibility.Visible;
                btn_next_page.Visibility = Visibility.Visible;
            }
        }

        private void ChageColor(TextBlock textBlock)
        {
            txb_page_btn_1.Foreground = Brushes.Black;
            txb_page_btn_2.Foreground = Brushes.Black;
            txb_page_btn_3.Foreground = Brushes.Black;
            txb_page_btn_4.Foreground = Brushes.Black;
            txb_page_btn_5.Foreground = Brushes.Black;

            textBlock.Foreground = Brushes.Blue;
        }
        /// <summary>
        /// 페이지 숫자 버튼 또는 화살표 버튼을 눌렀을 때 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn == null)
                return;

            switch (btn.Name)
            {
                case "btn_page_1":
                    curPageNum = Convert.ToInt32(txb_page_btn_1.Text);
                    ChageColor(txb_page_btn_1);
                    break;
                case "btn_page_2":
                    curPageNum = Convert.ToInt32(txb_page_btn_2.Text);
                    ChageColor(txb_page_btn_2);
                    break;
                case "btn_page_3":
                    curPageNum = Convert.ToInt32(txb_page_btn_3.Text);
                    ChageColor(txb_page_btn_3);
                    break;
                case "btn_page_4":
                    curPageNum = Convert.ToInt32(txb_page_btn_4.Text);
                    ChageColor(txb_page_btn_4);
                    break;
                case "btn_page_5":
                    curPageNum = Convert.ToInt32(txb_page_btn_5.Text);
                    ChageColor(txb_page_btn_5);
                    break;
                case "btn_pre_page":
                    {
                        int mod = curPageNum % 5 == 0 ? 4 : curPageNum % 5 - 1;
                        curPageNum = curPageNum - 5 - mod; //현재 페이지에서 5를 빼고 현재 페이지의 넘버에서 가장 처음 넘버로 맞추기 위해 mod 를 계산해서 뺴줌 ex) 10  - 5 - (mod=)4 = 1, ex)9-5-(mod=)3
                        curBatchPos = curPageNum % 5 != 0 ? curPageNum / 5 + 1 : curPageNum / 5; //전체 페이지를 5개로 나누었을 때 몇번째 있는지 위치 연산
                        ChageColor(txb_page_btn_1);
                        ShowPageSelBtn();
                    }
                    break;
                case "btn_next_page":
                    {
                        int mod = curPageNum % 5 == 0 ? 4 : curPageNum % 5 - 1;
                        curPageNum = curPageNum + 5 - mod;
                        curBatchPos = curPageNum % 5 != 0 ? curPageNum / 5 + 1 : curPageNum / 5;
                        ChageColor(txb_page_btn_1);
                        ShowPageSelBtn();
                    }
                    break;
            }
            if (isFindList)
                ShowFindList();
            else
                ShowList();
        }

        private void ShowFindList()
        {
            string sql;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    if (cbxFindMode.Text == "작성자")
                        sql = $"select (@num:=@num+1) as no, title, writer, writertime, bno from writedb, (select @num:={curPageNum * 10 - 10}) t where writer='{findString}' order by bno limit 10 offset {Convert.ToString(curPageNum * 10 - 10)}";
                    else
                        sql = $"select (@num:=@num+1) as no, title, writer, writertime, bno from writedb, (select @num:={curPageNum * 10 - 10}) t where title like '%{findString}%' order by bno limit 10 offset {Convert.ToString(curPageNum * 10 - 10)}";
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "DataBinding");
                    dgTable.DataContext = ds;
                }
                catch (Exception e)
                {
                    MessageBox.Show("ShowFindList");
                    return;
                }
            }
        }

        private int GetFindRowCnt()
        {
            string sql;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    if (cbxFindMode.Text == "작성자")
                        sql = $"select count(*) cnt from writedb where writer='{findString}'";
                    else
                        sql = $"select count(*) cnt from writedb where title like '%{findString}%'";

                    MySqlCommand command = new MySqlCommand(sql, connection);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return -1;
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            findString = "";
            listCnt = GetRowCnt() / 10;
            if (GetRowCnt() % 10 != 0)
                listCnt++;

            totalPageBatchCnt = listCnt / 5;
            if (listCnt % 5 != 0)
                totalPageBatchCnt++;

            isFindList = false;
            ShowList();
            ShowPageSelBtn();
            txb_page_btn_1.Foreground = Brushes.Blue;
            btnBack.Visibility = Visibility.Hidden;
            btnRefresh.Visibility = Visibility.Visible;
            tbxPageName.Text = "게시판";
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            if (cbxFindMode.SelectedItem == null)
                return;

            findString = txbFind.Text;

            listCnt = GetFindRowCnt() / 10;
            if (GetFindRowCnt() % 10 != 0)
                listCnt++;

            if (listCnt == 0)
            {
                MessageBox.Show("없습니다.");
                listCnt = GetRowCnt() / 10;
                if (GetRowCnt() % 10 != 0)
                    listCnt++;
                return;
            }

            totalPageBatchCnt = listCnt / 5;
            if (listCnt % 5 != 0)
                totalPageBatchCnt++;
            curBatchPos = 1;
            curPageNum = 1;

            ShowFindList();
            ShowPageSelBtn();
            isFindList = true;
            txb_page_btn_1.Foreground = Brushes.Blue;
            txbFind.Text = "";
            btnBack.Visibility = Visibility.Visible;
            btnRefresh.Visibility = Visibility.Hidden;
            tbxPageName.Text = "검색";
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            listCnt = GetRowCnt() / 10;
            if (GetRowCnt() % 10 != 0)
                listCnt++;

            totalPageBatchCnt = listCnt / 5;
            if (listCnt % 5 != 0)
                totalPageBatchCnt++;

            ShowPageSelBtn();
            ShowList();
            txb_page_btn_1.Foreground = Brushes.Blue;

            btnBack.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// 게시글 페이지로 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            DataRowView drv = (DataRowView)dgTable.SelectedItem;

            PostPage PostPage = new PostPage(Convert.ToInt32(drv.Row[4]));

            NavigationService.Navigate(PostPage);
        }

        /// <summary>
        /// 글쓰기 페이지로 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            WritePage writePage = new WritePage();

            NavigationService.Navigate(writePage);
        }
    }
}
