using System.Collections.Generic;

namespace Gamla.Logic
{
	public class BaseRepository< TData, TKey > : IRepository< TData, TKey >
	{
		#region MyRegion
		private readonly Dictionary< TKey, TData > _itemsDictionary = new Dictionary< TKey, TData >();
		#endregion

		#region Public Members
		public virtual IEnumerable< TData > GetAll()
		{
			return _itemsDictionary.Values;
		}

		public virtual void AddItem( TData data, TKey key )
		{
			//check exist key
			if( _itemsDictionary.ContainsKey( key ) )
			{
				throw new ItemAlreadyExistException( string.Format( @"Item for key: {0} already exist in repo", key ) );
			}

			//add item
			_itemsDictionary.Add( key, data );
		}

		public virtual void DeleteItem( TKey key )
		{
			//check exist key
			if( !_itemsDictionary.ContainsKey( key ) )
			{
				throw new ItemNotFoundExcetion( string.Format( @"Item for key: {0} not exist in repo", key ) );
			}

			//remove item
			_itemsDictionary.Remove( key );
		}

		public virtual TData GetItem( TKey key )
		{
			//check exist key
			if( !_itemsDictionary.TryGetValue( key, out var result ) )
			{
				throw new ItemNotFoundExcetion( string.Format( @"Item for key: {0} not exist in repo", key ) );
			}

			return result;
		}
		#endregion
	}
}   
