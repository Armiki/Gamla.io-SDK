using System;

namespace Gamla.Scripts.Common.Carousel.Core
{
	public sealed class ScrollNode
	{
		#region Public Fields
		/// <summary>
		/// Index of element in holder
		/// </summary>
		public int Index;

		/// <summary>
		/// Current Scroll Element
		/// </summary>
		public readonly IScrollElement ScrollElement;
		#endregion

		#region Constructor
		public ScrollNode(int index, IScrollElement scrollElement)
		{
			Index = index;
			ScrollElement = scrollElement ?? throw new ArgumentNullException();
		}
		#endregion
	}
}
