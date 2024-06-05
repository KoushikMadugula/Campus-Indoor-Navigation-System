using Microsoft.Maui.Controls;
using System;

namespace Campus_Indoor_Navigation_System;

public partial class FloorOne : ContentPage
{

    double currentScale = 1;
    double startScale = 1;
    double xOffset = 0;
    double yOffset = 0;

    public FloorOne()
	{
		InitializeComponent();
	}

    void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        if (e.Status == GestureStatus.Started)
        {
            // Store the current scale factor applied to the wrapped user interface element,
            // and zero the components for the center point of the translate transform.
            startScale = FloorOneImage.Scale;
            FloorOneImage.AnchorX = 0;
            FloorOneImage.AnchorY = 0;
        }
        else if (e.Status == GestureStatus.Running)
        {
            // Calculate the scale factor to be applied.
            currentScale = startScale * e.Scale;
            currentScale = Math.Max(1, currentScale);

            // Apply the scale factor.
            FloorOneImage.Scale = currentScale;
        }
        else if (e.Status == GestureStatus.Completed)
        {
            // Store the current translation applied during the scale operation.
            xOffset = FloorOneImage.TranslationX;
            yOffset = FloorOneImage.TranslationY;
        }
    }

    void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (e.StatusType == GestureStatus.Running)
        {
            // Translate the image.
            FloorOneImage.TranslationX = xOffset + e.TotalX;
            FloorOneImage.TranslationY = yOffset + e.TotalY;
        }
        else if (e.StatusType == GestureStatus.Completed)
        {
            // Store the translation applied during the pan operation.
            xOffset = FloorOneImage.TranslationX;
            yOffset = FloorOneImage.TranslationY;
        }
    }
}