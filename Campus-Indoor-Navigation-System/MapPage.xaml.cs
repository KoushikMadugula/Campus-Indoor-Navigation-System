namespace Campus_Indoor_Navigation_System;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
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

        Pin pin = new Pin
        {
            Label = "Cambrian College",
            Type = PinType.Place,
            Location = new Location(46.5291410, -80.9407060)
        };
        map.Pins.Add(pin);


    }
}