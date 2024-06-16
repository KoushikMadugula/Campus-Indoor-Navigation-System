namespace Campus_Indoor_Navigation_System
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void NavigateMainPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MainPage());
        }
    }
}