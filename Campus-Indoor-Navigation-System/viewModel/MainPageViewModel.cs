using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Campus_Indoor_Navigation_System.viewModel
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IMap map;
        private readonly IGeolocation geolocation;
        private readonly IConnectivity connectivity;
        public MainPageViewModel(IMap map, IGeolocation geolocation, IConnectivity connectivity)
        {
            this.map = map;
            this.geolocation = geolocation;
            this.connectivity = connectivity;
        }
        [RelayCommand]
        public async Task CheckLocation()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("OOPS", "You need internet connection for this", "Ok");
                return;
            }

            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("OOPS", "You need internet connection for this", "Ok");
                return;
            }

            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location == null) {
                location = await Geolocation.GetLocationAsync(
                    new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Best,
                        Timeout = TimeSpan.FromSeconds(30),
                        RequestFullAccuracy = true
                    });
            }

            if (location == null) {
                await Shell.Current.DisplayAlert("OOPS", "Sorry we couldn't get your location", "OK");
                return;
            }

#if ANDROID
            location.Latitude = 46.5291410;
            location.Longitude = -80.9407060;
 #endif

            //open map and use the location values
            await map.OpenAsync(location.Latitude, location.Longitude, new MapLaunchOptions
            {
                Name = "my current location",
                NavigationMode = NavigationMode.None
            });
                
        }
    }
}
