using System;

namespace Gamla.Logic
{
	public interface IObjectPool< TObject > : IDisposable where TObject : class
	{
		/// <summary>
		/// Get new item from pool
		/// </summary>
		TObject GetItem();

		/// <summary>
		/// Return item to pool
		/// </summary>
		void PutItem( TObject item );

		/// <summary>
		/// Set Available Objects count
		/// </summary>
		/// <param name="count">count of free objects to set</param>
		void SetAvalibleObjectsCount( uint count );

		/// <summary>
		/// Get free items in pool
		/// </summary>
		int AvalibleItems { get; }
	}
}