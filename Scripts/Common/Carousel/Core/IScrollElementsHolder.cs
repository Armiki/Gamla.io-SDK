using System;

namespace Gamla.Scripts.Common.Carousel.Core
{
	public interface IScrollElementsHolder
	{
		#region Public Events
		/// <summary>
		/// Rise after element size changed
		/// </summary>
		event EventHandler< SizeChangedEventArgs > OnElementSizeChanged;

		/// <summary>
		/// Rise after holder changed -> add/or remove elements
		/// </summary>
		event EventHandler< ElementsHolderChangedArgs > OnHolderChanged;
		#endregion

		#region Public Properties
		/// <summary>
		/// Elements total count
		/// </summary>
		int TotalCount { get; }
		#endregion

		#region Public members
		/// <summary>
		/// Get size of scroll element
		/// </summary>
		float GetElementSize( int index );

		/// <summary>
		/// Get absolute position of scroll element in holder
		/// </summary>
		float GetElementPosition( int index );

		/// <summary>
		/// Find index of element on position, -1 if none
		/// </summary>
		int FindElementOnPosition( float position );

		/// <summary>
		/// Clears all elements
		/// </summary>
		void Clear();
		#endregion
	}
}
