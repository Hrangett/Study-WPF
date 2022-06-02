using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfNaverMovieFinder.models;

namespace WpfNaverMovieFinder
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// 
    /// 검색버튼 클릭 이벤트 핸들러
    /// 네이버 OpenAPI
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtSearchName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter) btnSearch_Click(sender, e);
        }

        private void btnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            stsResult.Content = string.Empty;
            if(string.IsNullOrEmpty(txtSearchName.Text))
            {
                stsResult.Content = "검색할 영화 명을 입력, 검색 버튼을 눌러주세요";
                //MessageBox.Show("검색할 영화 명을 입력, 검색 버튼을 눌러주세요");
                Commons.ShowMessageAsync("검색", "검색할 영화 명을 입력, 검색 버튼을 눌러주세요");
                return;
            }

            //검색시작
            //Commons.ShowMessageAsync("결과", $"{ txtSearchName.Text}");
            try
            {
                //오픈 API는 네터워크 끊기명 안댄다
                SearchNaverOpenApi(txtSearchName.Text);
                Commons.ShowMessageAsync("검색", "영화 검색 완료");
            }
            catch (System.Exception ex)
            {

                //throw;
            }
        }

        /// <summary>
        /// 네이버 실제 검색 매서드
        /// </summary>
        /// <param name="searchName"></param>
        private void SearchNaverOpenApi(string searchName)
        {
            string clientID = "2yGXXLKrze_D5Akq_Yo9";
            string clientSecret = "MRH6qDQGIX";
            string openAptUri = $"https://openapi.naver.com/v1/search/movie?start=1&display=30&query={searchName}";
            string result = string.Empty;

            WebRequest request;
            WebResponse response = null;
            Stream stream = null;
            StreamReader reader = null;

            //naver OpenApi로 요청 시작
            try
            {
                request = WebRequest.Create(openAptUri);
                request.Headers.Add("X-Naver-Client-Id",clientID);
                request.Headers.Add("X-Naver-Client-Secret", clientSecret);

                response = request.GetResponse();
                stream = response.GetResponseStream();
                reader = new StreamReader(stream);

                result = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //네트워크, DB, File은 접속 후 예외발생여부에 관계없이 무조건 Close해줘야 한다.
                reader.Close();
                stream.Close();
                response.Close();

            }

            var parsedJson = JObject.Parse(result);

            int total = Convert.ToInt32(parsedJson["total"]);
            int display = Convert.ToInt32(parsedJson["display"]);

            stsResult.Content = $"{total} 중 {display} 호출 성공";

            var items = parsedJson["items"];
            var json_array = (JArray)items;

            List<MovieItem> movieItems = new List<MovieItem>();

            foreach (var item in json_array)
            {
                MovieItem movie = new MovieItem(
                    Regex.Replace(item["title"].ToString(), @"<(.|\n)*?>", string.Empty),
                    //item["title"].ToString(),
                    item["link"].ToString(),
                    item["image"].ToString(),
                    item["subtitle"].ToString(),
                    item["pubDate"].ToString(),
                    item["director"].ToString().Replace("|",", "), 
                    item["actor"].ToString().Replace("|", ", "),
                    item["userRating"].ToString());

                movieItems.Add(movie);
            }
            this.DataContext = movieItems;
        }

        private void btnAddWatchList_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void btnViewWatchList_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void btnDelWatchList_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void btnWatchTrailer_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }


        /// <summary>
        /// 선택한 영화의 포스터 보이기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void grdResult_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            if (grdResult.SelectedItem is MovieItem)
            {
                var movie = grdResult.SelectedItem as MovieItem;
                if(string.IsNullOrEmpty(movie.Image))
                {
                    imgPoster.Source = new BitmapImage(new Uri("/resource/No_Picture.jpg", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    imgPoster.Source = new BitmapImage(new Uri(movie.Image, UriKind.RelativeOrAbsolute));

                }
            }
        }


        /// <summary>
        /// 네이버 여오하 웹브라우저 열기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNaverMovie_Click(object sender, RoutedEventArgs e)
        {
            if (grdResult.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("네이버 영화", "영화를 선택하세요");
                return;
            }

            if (grdResult.SelectedItems.Count > 1 )
            {
                Commons.ShowMessageAsync("네이버 영화", "영화를 하나만 선택하세요");
                return;
            }
            string linkUrl = (grdResult.SelectedItem as MovieItem).Link;
            Process.Start(linkUrl);
            
             



        }
    }
}
