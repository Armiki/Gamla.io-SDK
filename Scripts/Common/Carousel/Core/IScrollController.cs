using System;
using System.Collections.Generic;

namespace Gamla.Scripts.Common.Carousel.Core
{
	public interface IScrollController
	{
		#region Public Events
		/// <summary>
		/// Rise after size changed
		/// </summary>
		event EventHandler< ScrollControllerChangedEventArgs > OnPositionChanged;

		/// <summary>
		/// Rise after window size changed
		/// </summary>
		event EventHandler< ScrollControllerChangedEventArgs > OnSizeChanged;
		
		event EventHandler< ScrollControllerChangedEventArgs > OnPositionCompensation;

		event EventHandler<ScrollControllerChangedEventArgs> OnStartItemChanged;
		#endregion

		#region Public Properties
		/// <summary>
		/// Current position of scroll
		/// </summary>
		float Position { get; }

		/// <summary>
		/// Current Window Size
		/// </summary>
		float WindowSize { get; }
		#endregion

		#region Public members
		/// <summary>
		/// Set absolute position of start window
		/// </summary>
		void SetPosition(float position);

		/// <summary>
		/// Set size of window for scroll controller [0: float.maxValue]
		/// </summary>
		void SetWindowSize( float windowSize );

		/// <summary>
		/// Reset controller to start state
		/// </summary>
		void Reset();

		/// <summary>
		/// Gets all scroll nodes
		/// </summary>
		IEnumerable< ScrollNode > GetScrollElements();
		#endregion
	}
}
