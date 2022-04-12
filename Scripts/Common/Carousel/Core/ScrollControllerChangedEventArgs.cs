using System;

namespace Gamla.UI.Carousel
{
	public sealed class ScrollControllerChangedEventArgs : EventArgs
	{
		#region Constructors
		public ScrollControllerChangedEventArgs( float oldValue, float newValue )
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#endregion

		#region Public Fields
		/// <summary>
		/// Old Value
		/// </summary>
		public readonly float oldValue;

		/// <summary>
		/// New Value
		/// </summary>
		public readonly float newValue;
		#endregion
	}
}
