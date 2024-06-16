using Microsoft.Maui.Controls;

namespace Campus_Indoor_Navigation_System
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //string sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "google-services.json");

            // Path in the app data directory
            //string destinationPath = Path.Combine(FileSystem.AppDataDirectory, "google-services.json");

            // Set the environment variable for Google Application Credentials
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", destinationPath);

            // Ensure the google-services.json file is copied to the AppDataDirectory
            //File.Copy(sourcePath, destinationPath, true);

            MainPage = new NavigationPage(new HomePage());

        }
    }
}
