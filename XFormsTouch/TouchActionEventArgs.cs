using System;
using Xamarin.Forms;

namespace XFormsTouch
{
    /// <summary>
    /// Event arguments related to touch actions.
    /// </summary>
    public sealed class TouchActionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TouchActionEventArgs"/> class.
        /// </summary>
        /// <param name="id">The touch ID.</param>
        /// <param name="type">The touch action type.</param>
        /// <param name="location">The touch location.</param>
        /// <param name="isInContact">Whether or not the touch is in contact.</param>
        public TouchActionEventArgs(long id, TouchActionType type, Point location, bool isInContact)
        {
            Id = id;
            Type = type;
            Location = location;
            IsInContact = isInContact;
        }

        /// <summary>
        /// Gets the touch ID.
        /// </summary>
        /// <value>The touch ID.</value>
        public long Id { get; }

        /// <summary>
        /// Gets the touch action type.
        /// </summary>
        /// <value>The touch action type.</value>
        public TouchActionType Type { get; }

        /// <summary>
        /// Gets the touch location.
        /// </summary>
        /// <value>The location.</value>
        public Point Location { get; }

        /// <summary>
        /// Gets a value indicating whether the touch is in contact.
        /// </summary>
        /// <value><c>true</c> if the touch is in contact, <c>false</c> otherwise.</value>
        public bool IsInContact { get; }
    }
}
