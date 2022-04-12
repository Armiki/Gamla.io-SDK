using System;

namespace Gamla.UI.Carousel
{
	public sealed class ElementsHolderChangedArgs : EventArgs
	{
		#region Public Members
		public readonly int Index;
		public readonly float ElementSize;
		public readonly ElementChangeReason Reason;
		#endregion

		#region Constructor
		public ElementsHolderChangedArgs( int index, float elementSize, ElementChangeReason reason )
		{
			Index = index;

			Reason = reason;
			ElementSize = elementSize;
		}
		#endregion
	}
}
