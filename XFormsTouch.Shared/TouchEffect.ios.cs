using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("XFormsTouch")]
[assembly: ExportEffect(typeof(XFormsTouch.iOS.TouchEffectIOS), "TouchEffect")]

#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace XFormsTouch.iOS
#pragma warning restore SA1300 // Element should begin with upper-case letter
{
    /// <summary>
    /// The iOS implementation of the touch effect.
    /// </summary>
    public class TouchEffectIOS : PlatformEffect
    {
        UIView view;
        TouchRecognizer touchRecognizer;

        /// <inheritdoc />
        protected override void OnAttached()
        {
            // Get the iOS UIView corresponding to the Element that the effect is attached to
            view = Control ?? Container;

            // Get access to the TouchEffect class in the .NET Standard library
            var effect = (TouchEffect)Element.Effects.FirstOrDefault(e => e is TouchEffect);

            if (effect != null && view != null)
            {
                // Create a TouchRecognizer for this UIView
                touchRecognizer = new TouchRecognizer(Element, view, effect);
                view.UserInteractionEnabled = true;
                view.AddGestureRecognizer(touchRecognizer);
            }
        }

        /// <inheritdoc />
        protected override void OnDetached()
        {
            if (touchRecognizer != null)
            {
                // Clean up the TouchRecognizer object
                touchRecognizer.Detach();

                // Remove the TouchRecognizer from the UIView
                view.RemoveGestureRecognizer(touchRecognizer);
            }
        }
    }
}
