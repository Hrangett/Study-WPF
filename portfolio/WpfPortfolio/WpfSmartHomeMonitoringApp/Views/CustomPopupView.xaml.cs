using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace WpfSmartHomeMonitoringApp.Views
{
    /// <summary>
    /// CustomPopupView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CustomPopupView : MetroWindow
    {
        public class CustomPoppViewModel : Conductor<object>
        {

        }
        public CustomPopupView()
        {
            InitializeComponent();
        }
    }
}
