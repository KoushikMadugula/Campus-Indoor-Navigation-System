namespace Campus_Indoor_Navigation_System;

using Google.Cloud.Firestore;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Plugin.MauiWifiManager;
using Map = Microsoft.Maui.Controls.Maps.Map;

public partial class MapPage : ContentPage
{
    public MapPage()
    {
        InitializeComponent();


        //setting cambrian college address

        var initialLocation = new Location(46.5291410, -80.9407060); // cambrian coordinates
        MapSpan mapSpan = MapSpan.FromCenterAndRadius(initialLocation, Distance.FromMeters(10));
        map.MoveToRegion(mapSpan);



        //_currentPin = new Pin
        //{
        //    Label = "Cambrian College",
        //    Type = PinType.Place,
        //    Location = new Location(46.5291410, -80.9407060)
        //};
       // map.Pins.Add(_currentPin);
    }

    private class LocationDetails
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<WiFiDetails> WiFi { get; set; } = new List<WiFiDetails>();
    }

    private class WiFiDetails
    {
        public string Ssid { get; set; }
        public string Bssid { get; set; }
        public int Level { get; set; }
    }

    private async void OnMapClicked(object sender, MapClickedEventArgs e)
        {

        List<Plugin.MauiWifiManager.Abstractions.NetworkData> scanResults = new List<Plugin.MauiWifiManager.Abstractions.NetworkData>();

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            PermissionStatus status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted || DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                scanResults = await CrossWifiManager.Current.ScanWifiNetworks();
            }
            else
            {
                await DisplayAlert("No location permission", "Please provide location permission", "OK");
            }
        });





        LocationDetails location = new LocationDetails
        {
            Latitude = e.Location.Latitude,
            Longitude = e.Location.Longitude,
            WiFi = new List<WiFiDetails>()
        };

  

        foreach (var accessPoint in scanResults)
        {
            dynamic nativeObject = accessPoint.NativeObject;

            location.WiFi.Add(new WiFiDetails()
            {
                Bssid = accessPoint.Bssid.ToString(),
                Ssid = accessPoint.Ssid,
                Level = nativeObject.Level
            });
        }



        var pin = new Pin
        {
            Label = "New Pin",
            Location = e.Location,
            Type = PinType.Place
        };
        map.Pins.Add(pin);




        //FirestoreDb db = FirestoreDb.Create("mapmycampus-425914");

        // Create a document with a random ID in the "users" collection.
        //CollectionReference collection = db.Collection("locations");
        //DocumentReference document = await collection.AddAsync(location);


    }

}
