using System;
using System.Collections.Generic;
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
        List<NetworkData> networkDataModel = new List<NetworkData>();
        public Command InfoCommand { get; }
        public Command ConnectCommand { get; }
        private GeolocationRequest geoRequest;
        private CancellationTokenSource cts;
        private Timer timer;

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
                    networkDataModel.Clear();
                    foreach (var item in response)
                    {
                        networkDataModel.Add(new NetworkData()
                        {
                            StausId = item.StausId,
                            IpAddress = (int)item.IpAddress,
                            Bssid = item.Bssid,
                            Ssid = item.Ssid,
                            GatewayAddress = item.GatewayAddress,
                            NativeObject = item.NativeObject
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
            cts = new CancellationTokenSource();
            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                MainThread.BeginInvokeOnMainThread(GetCurrentLocation);
                return true;
            });
        }

        private async void GetCurrentLocation()
        {
            try
            {
                var location = await Geolocation.GetLocationAsync(geoRequest, cts.Token);
                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                    await GetWifiList();
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
            timer = new Timer(async (state) =>
                await MainThread.InvokeOnMainThreadAsync(async () => await GetWifiList()),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(5));
        }
    }
}
