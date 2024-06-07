using Campus_Indoor_Navigation_System.viewModel;

namespace Campus_Indoor_Navigation_System
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage(MainPageViewModel mainPageViewModel)
        {
           InitializeComponent();
            BindingContext = mainPageViewModel;
        }

        private void NavigateFloorOne(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FloorOne());
        }

        private void NavigateFloorTwo(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FloorTwo());
        }

        private void NavigateFloorThree(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FloorThree());
        }

        private void NavigateFloorFour(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FloorFour());
        }

        private void NavigateWifiPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new WifiPage());
        }
    }

}
