using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfNaverMovieFinder.models;

namespace WpfNaverMovieFinder
{
    /// <summary>
    /// TrailerWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TrailerWindow : MetroWindow
    {
        List<YoutubeItem> youtubeItems;
        public TrailerWindow()
        {
            InitializeComponent();
        }

        public TrailerWindow(string movieName) : this() //  기본생성자 실행하면서 InitializeComponent() 실행하고 이 생성자 실행하라는 의미
        {
            lblMovieName.Content = $"{movieName} 예고편 "; // '매트릭스 : 리저렉션 예고편'
        }

        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            youtubeItems = new List<YoutubeItem>();
            SearchYoutubeApi();
        }

        private async void SearchYoutubeApi()
        {
            //비동기로 처리되어야함 :: 옆에서 영상 틀어지면서 검색 가능
            await LoadDataCollection();
            lsvYoutubeSearch.ItemsSource = youtubeItems;
        }

        private async Task LoadDataCollection()
        {
            var youtubeService = new YouTubeService(
                new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyCt9cLxFEACS0_YyktF3I7VvI5eEhRJdY8",
                    ApplicationName = this.GetType().ToString()
                });

            //요청
            var request = youtubeService.Search.List("snippet");
            request.Q = lblMovieName.Content.ToString();
            request.MaxResults = 10;

            //응답 :: 비동기, 구글의 Request 호출
            var response = await request.ExecuteAsync();

            //MessageBox.Show(response.ToString());   //잘 넘어오는지 확인.

            foreach(var item in response.Items)
            {
                if(item.Id.Kind.Equals("youtube#video"))
                {
                    YoutubeItem youtube = new YoutubeItem(
                    /*Title*/   item.Snippet.Title,
                    /*author*/  item.Snippet.ChannelTitle,
                    /*$"https://www.youtube-nocookie.com/embed/{item.Id.VideoId}"*/
                    /*URL*/     $"https://www.youtube.com/watch?v={item.Id.VideoId}");
                    
                    //섬네일 이미지
                    youtube.Thumbnail = new BitmapImage(new Uri(item.Snippet.Thumbnails.Default__.Url, UriKind.RelativeOrAbsolute));

                    youtubeItems.Add(youtube);
                }
            }

        }


        private void lsvYoutubeSearch_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(lsvYoutubeSearch.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("유투브", "예고편을 볼 영화를 선택하세요");
                return;
            }
            if (lsvYoutubeSearch.SelectedItems.Count > 1)
            {
                Commons.ShowMessageAsync("유투브", "예고편을 하나만 선택하세요");
                return;
            }
            if (lsvYoutubeSearch.SelectedItem is YoutubeItem)
            {
                var video = lsvYoutubeSearch.SelectedItem as YoutubeItem;
                brsYoutubeWatch.Address = video.URL;
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            brsYoutubeWatch.Address = String.Empty;
            brsYoutubeWatch.Dispose();  //리소스 해제
        }


    }
}
