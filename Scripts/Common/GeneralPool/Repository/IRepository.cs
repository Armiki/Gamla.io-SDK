using System.Collections.Generic;

namespace Gamla.Logic
{
	public interface IRepository< TData, in TKey >
	{
		/// <summary>
		/// Get all items from repo
		/// </summary>
		/// <returns></returns>
		IEnumerable< TData > GetAll();

		/// <summary>
		/// Add item to repo
		/// </summary>
		void AddItem( TData data, TKey key );

		/// <summary>
		/// Delete item from collection
		/// </summary>
		/// <param name="key"></param>
		void DeleteItem( TKey key );

		/// <summary>
		/// Get item by key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		TData GetItem( TKey key );
	}
}
