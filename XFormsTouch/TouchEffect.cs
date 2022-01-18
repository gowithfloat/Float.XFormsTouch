using Xamarin.Forms;

namespace XFormsTouch
{
    /// <summary>
    /// A touch effect.
    /// </summary>
    public class TouchEffect : RoutingEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TouchEffect"/> class.
        /// </summary>
        public TouchEffect() : base("XFormsTouch.TouchEffect")
        {
        }

        /// <summary>
        /// The touch action handler.
        /// </summary>
        public event TouchActionEventHandler TouchAction;

        /// <summary>
        /// Gets or sets a value indicating whether to capture touches.
        /// </summary>
        /// <value><c>true</c> if touches should be captured, <c>false</c> otherwise.</value>
        public bool Capture { get; set; } = true;

        /// <summary>
        /// Invoked when a touch action occurs.
        /// </summary>
        /// <param name="element">The element that was touched.</param>
        /// <param name="args">The touch arguments.</param>
        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }
    }
}
