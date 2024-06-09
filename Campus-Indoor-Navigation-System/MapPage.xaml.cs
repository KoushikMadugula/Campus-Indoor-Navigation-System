namespace Campus_Indoor_Navigation_System;
using Map = Microsoft.Maui.Controls.Maps.Map;

public partial class MapPage : ContentPage
{
	public MapPage()
	{
		InitializeComponent();
		Map map = new Map
		{
			MapType = Microsoft.Maui.Maps.MapType.Street
		};
	}
}