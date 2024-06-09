using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Plugin.MauiWifiManager;

namespace Campus_Indoor_Navigation_System
{
    public partial class WifiPage : ContentPage
    {
        ObservableCollection<NetworkData> networkDataModel = new ObservableCollection<NetworkData>();
        public Command InfoCommand { get; }
        public Command ConnectCommand { get; }
        private GeolocationRequest geoRequest;
        private CancellationTokenSource cts;
        private Timer timerWiFiScan;
        private Timer timerGPSScan;

        public WifiPage()
        {
            InitializeComponent();
            BindingContext = this;
            InfoCommand = new Command<NetworkData>(ExecuteInfoCommand);
            ConnectCommand = new Command<NetworkData>(ExecuteConnectCommand);
            StartLocationUpdates();
            StartPeriodicScan();
        }

        private async void ExecuteConnectCommand(NetworkData model)
        {
            if (!string.IsNullOrWhiteSpace(model.SsidName))
            {
                var response = await DisplayPromptAsync("Connect " + model.SsidName, "Enter password to connect");
                if (!string.IsNullOrWhiteSpace(response) && response.Length >= 8)
                {
                    var status = await CrossWifiManager.Current.ConnectWifi(model.SsidName, response);
                }
            }
        }

        private async void ExecuteInfoCommand(NetworkData model)
        {
            var info = $"StatusId: {model.StausId}, " +
                   $"Ssid: {model.SsidName}, " +
                   $"IpAddress: {model.IpAddress}, " +
                   $"GatewayAddress: {model.GatewayAddress ?? "N/A"}, " +
                   $"NativeObject: {model.NativeObject}, " +
                   $"Bssid: {model.Bssid}";
            await DisplayAlert("Network info", info, "OK");
        }

        private async void ScanClicked(object sender, EventArgs e)
        {
            await GetWifiList();
        }

        private async Task GetWifiList()
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                PermissionStatus status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status == PermissionStatus.Granted || DeviceInfo.Current.Platform == DevicePlatform.WinUI)
                {
                    await Task.Delay(1000);
                    loading.IsRunning = true;
                    scanCollectionView.IsVisible = false;
                    var response = await CrossWifiManager.Current.ScanWifiNetworks();
                    //networkDataModel.Clear();
                    //foreach (var item in response)
                    //{
                    //    dynamic nativeObject = item.NativeObject;

                    //    networkDataModel.Add(new NetworkData()
                    //    {
                    //        StausId = item.StausId,
                    //        IpAddress = (int)item.IpAddress,
                    //        Bssid = item.Bssid,
                    //        Ssid = item.Ssid,
                    //        GatewayAddress = item.GatewayAddress,
                    //        NativeObject = item.NativeObject,
                    //        Level = nativeObject.Level,
                    //    });
                    //}

                    //

                    // Create a dictionary from the response for quick lookup
                    var responseDict = response.ToDictionary(item => item.Bssid, item => item);

                    // List to hold items to be removed
                    List<NetworkData> itemsToRemove = new List<NetworkData>();

                    foreach (var item in networkDataModel)
                    {
                        if (responseDict.ContainsKey(item.Bssid))
                        {
                            // If the item exists in the response, update the values
                            dynamic nativeObject = responseDict[item.Bssid].NativeObject;

                            item.StausId = responseDict[item.Bssid].StausId;
                            item.IpAddress = (int)responseDict[item.Bssid].IpAddress;
                            item.Ssid = responseDict[item.Bssid].Ssid; //DateTime.Now.ToString();
                            item.GatewayAddress = responseDict[item.Bssid].GatewayAddress;
                            item.NativeObject = responseDict[item.Bssid].NativeObject;
                            item.Level = nativeObject.Level;

                            // Remove the item from the response dictionary
                            responseDict.Remove(item.Bssid);
                        }
                        else
                        {
                            // If the item does not exist in the response, mark it for removal
                            itemsToRemove.Add(item);
                        }
                    }

                    // Remove items not found in the response
                    foreach (var item in itemsToRemove)
                    {
                        networkDataModel.Remove(item);
                    }

                    // Add new items from the response
                    foreach (var item in responseDict.Values)
                    {
                        dynamic nativeObject = item.NativeObject;

                        networkDataModel.Add(new NetworkData()
                        {
                            StausId = item.StausId,
                            IpAddress = (int)item.IpAddress,
                            Bssid = item.Bssid,
                            Ssid = item.Ssid,
                            GatewayAddress = item.GatewayAddress,
                            NativeObject = item.NativeObject,
                            Level = nativeObject.Level,
                        });
                    }


                    scanCollectionView.ItemsSource = networkDataModel;
                    loading.IsRunning = false;
                    scanCollectionView.IsVisible = true;
                }
                else
                {
                    await DisplayAlert("No location permission", "Please provide location permission", "OK");
                }
            });
        }

        private void StartLocationUpdates()
        {
            geoRequest = new GeolocationRequest(GeolocationAccuracy.Best);
            timerGPSScan = new Timer(async (state) =>
                await MainThread.InvokeOnMainThreadAsync(async () => await GetCurrentLocation()),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(1)); //This is one, but should be longer, but I don't know if it will run right away if we set it to 10.
        }

        private async Task GetCurrentLocation()
        {
            try
            {
                var location = await Geolocation.GetLocationAsync(geoRequest, cts.Token);
                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                    await GetWifiList(); //This might not be needed.
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void StopLocationUpdates()
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }

        private void StartPeriodicScan()
        {
            timerWiFiScan = new Timer(async (state) =>
                await MainThread.InvokeOnMainThreadAsync(async () => await GetWifiList()),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(3)); //This should be triggered when clicking to make a pin, not frequently like this.
            //// I MADE THIS VERY SMALL, SHOULD BE HIGHER FOR TESTING.
        }
    }
}



//On map tap, add a marker, perform a scan, and for every item found in the scan (use a foreach), perform the following (inside the foreach):
//
//Create a new record in the database with:
// - Date/Time
// - Latitude of the new pin on the google map.
// - Longitude of the new pin on the google map.
// - MAC Address (bssid) of scanned device.
// - Level of scanned device.

//Basically if your scan found 10 items, you would create 10 rows with the exact same Latitude and Longitude, but each would have a unique MAC Address and Level.

//When you perform the insert, you want to check if the row already exists by selecting WHERE Longitude =... AND Laitude =... AND MAC =... AND Level =..., if a row already exists, don't add a new one.

// IMPORTANT: Youb are collecting the GPS position, but that is not the Latitude and Longitude you would store, you would get the lat/lon to store from the NEW PIN tapped. The lat/long from the GPS can be used to generally center the google map when the page is loaded initially. As such, it does not need to re-check the gps regularly, once on page load is fine.
