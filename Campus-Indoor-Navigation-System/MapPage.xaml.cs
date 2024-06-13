namespace Campus_Indoor_Navigation_System;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

public partial class MapPage : ContentPage
{
    private Pin _currentPin;
    public MapPage()
    {
        InitializeComponent();


        //setting cambrian college address

        var initialLocation = new Location(46.5291410, -80.9407060); // cambrian coordinates
        MapSpan mapSpan = MapSpan.FromCenterAndRadius(initialLocation, Distance.FromMeters(10));
        map.MoveToRegion(mapSpan);


        _currentPin = new Pin
        {
            Label = "Cambrian College",
            Type = PinType.Place,
            Location = new Location(46.5291410, -80.9407060)
        };
        map.Pins.Add(_currentPin);
    }

    private void OnMapClicked(object sender, MapClickedEventArgs e)
        {
        // Remove the previous pin if it exists
        if (_currentPin != null)
        {
            map.Pins.Remove(_currentPin);
        }
        _currentPin = new Pin
            {
                Label = "New Pin",
                Location = e.Location,
                Type = PinType.Place
            };
            map.Pins.Add(_currentPin);
        }

    }
