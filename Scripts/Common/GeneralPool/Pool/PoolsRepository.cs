using System;
using System.Collections;
using System.Collections.Generic;

namespace Gamla.Logic
{
	public sealed class PoolsRepository<TKey> : IPoolsRepository<TKey>
	{
#region Private Fields
		//TODO: replace to base interface after make non generic base version
		private readonly Dictionary< TKey, object > _poolsMap = new Dictionary< TKey, object >();
#endregion

#region Public Members
		public void AddPool< TPoolObject >( TKey key, IObjectPool< TPoolObject > pool ) where TPoolObject : class
		{
			//check parameters
			if( pool == null )
			{
				throw new ArgumentNullException( nameof(pool) );
			}

			//check duplication of pool
			if( _poolsMap.ContainsKey( key ) )
			{
				throw new ItemAlreadyExistException( string.Format( @"pool with id:{0} already exist", pool ) );
			}

			//add pool
			_poolsMap.Add( key, pool );
		}

		public IObjectPool< TPoolObject > GetPool< TPoolObject >( TKey key ) where TPoolObject : class
		{
			//check exist in pool
			if( !_poolsMap.TryGetValue( key, out var poolObject ) )
			{
				throw new ItemNotFoundExcetion( string.Format( @"pool with key: {0} not exist", key ) );
			}

			//try cast to result pool
			if( !( poolObject is IObjectPool< TPoolObject > castedPool ) )
			{
				throw new ItemNotFoundExcetion( string.Format( @"Wrong pool type: expected {0} bu was {1}", typeof( IObjectPool< TPoolObject > ), poolObject.GetType() ) );
			}

			return castedPool;
		}

		public bool HasInPool< TPoolObject >( TKey key ) where TPoolObject : class
		{
			//check exist in map
			if( !_poolsMap.TryGetValue( key, out var poolObject ) )
			{
				return false;
			}

			//check type
			return poolObject is IObjectPool< TPoolObject >;
		}

		public void RemovePool( TKey key )
		{
			if( !_poolsMap.Remove( key ) )
			{
				throw new ItemNotFoundExcetion( string.Format( @"pool with key: {0} not exist", key ) );
			}
		}

		public IEnumerable GetAll()
		{
			return _poolsMap.Values;
		}

		public void Clear()
		{
			_poolsMap.Clear();
		}
#endregion
	}
}