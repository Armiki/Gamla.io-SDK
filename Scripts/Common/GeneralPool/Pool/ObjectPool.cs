using System;
using System.Collections.Generic;
using Gamla.Scripts.Common.GeneralPool;

namespace Gamla.Scripts.Common.GeneralPool
{
	public sealed class ObjectPool< TObject > : IObjectPool< TObject > where TObject : class
	{
#region Private Fields
		private bool _disposed;
		private readonly Stack< TObject > _freeObjectsStack = new Stack< TObject >();

		private readonly IObjectFactory _factory;
		private readonly IObjectDisposer< TObject > _objectDisposer;
		private readonly IPoolObjectInitializer< TObject > _objectInitializer;
#endregion

#region Constructors
		public ObjectPool( IObjectFactory factory, IObjectDisposer< TObject > objectDisposer, IPoolObjectInitializer< TObject > objectInitializer )
		{
			_factory = factory ?? throw new ArgumentNullException( nameof(factory) );
			_objectDisposer = objectDisposer ?? throw new ArgumentNullException( nameof(objectDisposer) );
			_objectInitializer = objectInitializer ?? throw new ArgumentNullException( nameof(objectInitializer) );
		}
#endregion

#region Public Members
		public void Dispose()
		{
			//clear all available items
			SetAvalibleObjectsCount( 0 );
			_disposed = true;
		}

		public TObject GetItem()
		{
			CheckDisposed();

			//construct or get object from pool or create new if no free
            bool newlyCreated = false;
            TObject result = null;
            if (_freeObjectsStack.Count != 0) {
                 result = _freeObjectsStack.Pop();
            } else {
                result = _factory.CreateObject<TObject>();
                newlyCreated = true;
            }

			//init object
			_objectInitializer.UseObject( result, newlyCreated);
			return result;
		}

		public void PutItem( TObject item )
		{
			CheckDisposed();

			//check item
			if( item == null )
			{
				throw new ArgumentNullException( nameof(item) );
			}

			_objectInitializer.ReleaseObject( item );
			_freeObjectsStack.Push( item );
		}

		public void SetAvalibleObjectsCount( uint count )
		{
			CheckDisposed();

			//check same set
			if( _freeObjectsStack.Count == count )
			{
				return;
			}

			//change number of elements
			if( _freeObjectsStack.Count < count )
			{
				for( int i = _freeObjectsStack.Count; i != count; i++ )
				{
					TObject newObjecy = _factory.CreateObject< TObject >();
					_freeObjectsStack.Push( newObjecy );
				}
			}
			else
			{
				int deleteCount = (int)count - _freeObjectsStack.Count;
				for( int i = 0; i != deleteCount; i++ )
				{
					//dispose items
					TObject first = _freeObjectsStack.Pop();
					_objectDisposer.DisposeItem( first );
				}
			}
		}

		public int AvalibleItems => _freeObjectsStack.Count;
#endregion

#region Private Members
		/// <summary>
		/// Check if pool is disposed -> throw exception on public methods
		/// </summary>
		private void CheckDisposed()
		{
			if( _disposed )
			{
				throw new ObjectDisposedException( @"Pool is disposed" );
			}
		}
#endregion
	}
}