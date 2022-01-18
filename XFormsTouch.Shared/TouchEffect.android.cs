using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("XFormsTouch")]
[assembly: ExportEffect(typeof(XFormsTouch.Droid.TouchEffectDroid), "TouchEffect")]

namespace XFormsTouch.Droid
{
    /// <summary>
    /// The Android implementation of the touch effect.
    /// </summary>
    public class TouchEffectDroid : PlatformEffect
    {
        static readonly Dictionary<Android.Views.View, TouchEffectDroid> ViewDictionary = new ();
        static readonly Dictionary<int, TouchEffectDroid> IdToEffectDictionary = new ();
        readonly int[] twoIntArray = new int[2];
        Android.Views.View view;
        Element formsElement;
        TouchEffect libTouchEffect;
        bool capture;
        Func<double, double> fromPixels;

        /// <inheritdoc />
        protected override void OnAttached()
        {
            view = Control ?? Container;

            var touchEffect = (TouchEffect)Element.Effects.FirstOrDefault(e => e is TouchEffect);

            if (touchEffect == null || view == null)
            {
                return;
            }

            ViewDictionary.Add(view, this);

            formsElement = Element;
            libTouchEffect = touchEffect;

            fromPixels = view.Context.FromPixels;
            view.Touch += OnTouch;
            (Element as Layout<Xamarin.Forms.View>)?.Children.ForEach(v => v.InputTransparent = true);
        }

        /// <inheritdoc />
        protected override void OnDetached()
        {
            try
            {
                if (ViewDictionary.ContainsKey(view))
                {
                    ViewDictionary.Remove(view);
                    view.Touch -= OnTouch;
                }
            }
            catch (ObjectDisposedException)
            {
                // TODO This Bug is fixed with XForms 3.5 or higher.
            }
        }

        void OnTouch(object sender, Android.Views.View.TouchEventArgs args)
        {
            // Two object common to all the events
            var senderView = sender as Android.Views.View;
            var motionEvent = args.Event;

            // Get the pointer index
            var pointerIndex = motionEvent.ActionIndex;

            // Get the id that identifies a finger over the course of its progress
            var id = motionEvent.GetPointerId(pointerIndex);

            senderView.GetLocationOnScreen(twoIntArray);

            var screenPointerCoords = new Point(
                twoIntArray[0] + motionEvent.GetX(pointerIndex),
                twoIntArray[1] + motionEvent.GetY(pointerIndex));

            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    FireEvent(this, id, TouchActionType.Pressed, screenPointerCoords, true);

                    IdToEffectDictionary.Add(id, this);

                    capture = libTouchEffect.Capture;
                    break;
                case MotionEventActions.Move:
                    // Multiple Move events are bundled, so handle them in a loop
                    for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                    {
                        id = motionEvent.GetPointerId(pointerIndex);

                        if (capture)
                        {
                            senderView.GetLocationOnScreen(twoIntArray);

                            screenPointerCoords = new Point(
                                twoIntArray[0] + motionEvent.GetX(pointerIndex),
                                twoIntArray[1] + motionEvent.GetY(pointerIndex));

                            FireEvent(this, id, TouchActionType.Moved, screenPointerCoords, true);
                        }
                        else
                        {
                            CheckForBoundaryHop(id, screenPointerCoords);

                            if (IdToEffectDictionary[id] != null)
                            {
                                FireEvent(IdToEffectDictionary[id], id, TouchActionType.Moved, screenPointerCoords, true);
                            }
                        }
                    }

                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                    if (capture)
                    {
                        FireEvent(this, id, TouchActionType.Released, screenPointerCoords, false);
                    }
                    else
                    {
                        CheckForBoundaryHop(id, screenPointerCoords);

                        if (IdToEffectDictionary[id] != null)
                        {
                            FireEvent(IdToEffectDictionary[id], id, TouchActionType.Released, screenPointerCoords, false);
                        }
                    }

                    IdToEffectDictionary.Remove(id);
                    break;
                case MotionEventActions.Cancel:
                    if (capture)
                    {
                        FireEvent(this, id, TouchActionType.Cancelled, screenPointerCoords, false);
                    }
                    else
                    {
                        if (IdToEffectDictionary[id] != null)
                        {
                            FireEvent(IdToEffectDictionary[id], id, TouchActionType.Cancelled, screenPointerCoords, false);
                        }
                    }

                    IdToEffectDictionary.Remove(id);
                    break;
            }
        }

        void CheckForBoundaryHop(int id, Point pointerLocation)
        {
            TouchEffectDroid touchEffectHit = null;

            foreach (var view in ViewDictionary.Keys)
            {
                // Get the view rectangle
                try
                {
                    view.GetLocationOnScreen(twoIntArray);
                }
                catch
                {
                    // System.ObjectDisposedException: Cannot access a disposed object.
                    continue;
                }

                var viewRect = new Rectangle(twoIntArray[0], twoIntArray[1], view.Width, view.Height);

                if (viewRect.Contains(pointerLocation))
                {
                    touchEffectHit = ViewDictionary[view];
                }
            }

            if (touchEffectHit != IdToEffectDictionary[id])
            {
                if (IdToEffectDictionary[id] != null)
                {
                    FireEvent(IdToEffectDictionary[id], id, TouchActionType.Exited, pointerLocation, true);
                }

                if (touchEffectHit != null)
                {
                    FireEvent(touchEffectHit, id, TouchActionType.Entered, pointerLocation, true);
                }

                IdToEffectDictionary[id] = touchEffectHit;
            }
        }

        void FireEvent(TouchEffectDroid touchEffect, int id, TouchActionType actionType, Point pointerLocation, bool isInContact)
        {
            // Get the method to call for firing events
            Action<Element, TouchActionEventArgs> onTouchAction = touchEffect.libTouchEffect.OnTouchAction;

            // Get the location of the pointer within the view
            touchEffect.view.GetLocationOnScreen(twoIntArray);
            var x = pointerLocation.X - twoIntArray[0];
            var y = pointerLocation.Y - twoIntArray[1];
            var point = new Point(fromPixels(x), fromPixels(y));

            // Call the method
            onTouchAction(touchEffect.formsElement, new TouchActionEventArgs(id, actionType, point, isInContact));
        }
    }
}
