using System;
namespace Gamla.Scripts.Common.Carousel.Core
{
	public sealed class SizeChangedEventArgs : EventArgs
	{
		#region Public Fields
		/// <summary>
		/// Old size of element
		/// </summary>
		public readonly float OldSize;

		/// <summary>
		/// New Size of element
		/// </summary>
		public readonly float NewSize;

		/// <summary>
		/// Delta of size = NewSize - OldSize
		/// </summary>
		public readonly float Delta;

		/// <summary>
		/// Index of changed Element
		/// </summary>
		public readonly int Index;
		#endregion

		#region Constructors
		public SizeChangedEventArgs( int index, float oldSize, float newSize )
		{
			//check arguments
			if( index < 0 )
			{
				throw new ArgumentOutOfRangeException( nameof(index), @"Cannot be less then zero" );
			}

			OldSize = oldSize;
			NewSize = newSize;
			Delta = newSize - oldSize;
			Index = index;
		}
		#endregion
	}
}