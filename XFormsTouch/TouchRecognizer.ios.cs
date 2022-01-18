using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;

#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace XFormsTouch.iOS
#pragma warning restore SA1300 // Element should begin with upper-case letter
{
    class TouchRecognizer : UIGestureRecognizer
    {
        static readonly Dictionary<UIView, TouchRecognizer> ViewDictionary = new ();
        static readonly Dictionary<long, TouchRecognizer> IdToTouchDictionary = new ();
        readonly Element element;        // Forms element for firing events
        readonly UIView view;            // iOS UIView
        readonly TouchEffect touchEffect;
        bool capture;

        public TouchRecognizer(Element element, UIView view, TouchEffect touchEffect)
        {
            this.element = element;
            this.view = view;
            this.touchEffect = touchEffect;

            ViewDictionary.Add(view, this);
        }

        public void Detach()
        {
            ViewDictionary.Remove(view);
        }

        // touches = touches of interest; evt = all touches of type UITouch
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();
                FireEvent(this, id, TouchActionType.Pressed, touch, true);

                if (!IdToTouchDictionary.ContainsKey(id))
                {
                    IdToTouchDictionary.Add(id, this);
                }
            }

            // Save the setting of the Capture property
            capture = touchEffect.Capture;
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            foreach (var touch in touches.Cast<UITouch>())
            {
                var id = touch.Handle.ToInt64();

                if (capture)
                {
                    FireEvent(this, id, TouchActionType.Moved, touch, true);
                }
                else
                {
                    CheckForBoundaryHop(touch);

                    if (IdToTouchDictionary[id] != null)
                    {
                        FireEvent(IdToTouchDictionary[id], id, TouchActionType.Moved, touch, true);
                    }
                }
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            foreach (var touch in touches.Cast<UITouch>())
            {
                var id = touch.Handle.ToInt64();

                if (capture)
                {
                    FireEvent(this, id, TouchActionType.Released, touch, false);
                }
                else
                {
                    CheckForBoundaryHop(touch);

                    if (IdToTouchDictionary[id] != null)
                    {
                        FireEvent(IdToTouchDictionary[id], id, TouchActionType.Released, touch, false);
                    }
                }

                IdToTouchDictionary.Remove(id);
            }
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            foreach (var touch in touches.Cast<UITouch>())
            {
                var id = touch.Handle.ToInt64();

                if (capture)
                {
                    FireEvent(this, id, TouchActionType.Cancelled, touch, false);
                }
                else if (IdToTouchDictionary[id] != null)
                {
                    FireEvent(IdToTouchDictionary[id], id, TouchActionType.Cancelled, touch, false);
                }

                IdToTouchDictionary.Remove(id);
            }
        }

        void CheckForBoundaryHop(UITouch touch)
        {
            long id = touch.Handle.ToInt64();

            // TODO: Might require converting to a List for multiple hits
            TouchRecognizer recognizerHit = null;

            foreach (var view in ViewDictionary.Keys)
            {
                var location = touch.LocationInView(view);

                if (new CGRect(new CGPoint(), view.Frame.Size).Contains(location))
                {
                    recognizerHit = ViewDictionary[view];
                }
            }

            if (recognizerHit != IdToTouchDictionary[id])
            {
                if (IdToTouchDictionary[id] != null)
                {
                    FireEvent(IdToTouchDictionary[id], id, TouchActionType.Exited, touch, true);
                }

                if (recognizerHit != null)
                {
                    FireEvent(recognizerHit, id, TouchActionType.Entered, touch, true);
                }

                IdToTouchDictionary[id] = recognizerHit;
            }
        }

        void FireEvent(TouchRecognizer recognizer, long id, TouchActionType actionType, UITouch touch, bool isInContact)
        {
            // Convert touch location to Xamarin.Forms Point value
            var oldPoint = touch.LocationInView(recognizer.View);
            var newPoint = new Point(oldPoint.X, oldPoint.Y);

            // Get the method to call for firing events
            Action<Element, TouchActionEventArgs> onTouchAction = recognizer.touchEffect.OnTouchAction;

            // Call that method
            onTouchAction(recognizer.element, new TouchActionEventArgs(id, actionType, newPoint, isInContact));
        }
    }
}
