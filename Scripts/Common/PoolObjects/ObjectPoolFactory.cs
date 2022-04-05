using System;
using UnityEngine;
using System.Collections.Generic;
using Gamla.Scripts.Common.GeneralPool;
using Gamla.Scripts.Logic;
using Object = UnityEngine.Object;

	public sealed class ObjectPoolFactory< TObjectType > : IObjectFactory, IObjectDisposer< TObjectType > 
	{
#region Private Members
		private readonly string _resourceId;
		private readonly string _resourcePath;
		private readonly Transform _creationPoint;

		private readonly Dictionary< object, GameObject > _createdObjectsMap = new Dictionary< object, GameObject >();
#endregion

#region  Constructors
		public ObjectPoolFactory(string resourceId, string resourcePath, Transform creationPoint)
		{
			//check parameters
			if( creationPoint == null )
			{
				throw new ArgumentNullException( @"creationPoint" );
			}

			if( string.IsNullOrEmpty( resourceId ) )
			{
				throw new ArgumentException( @"resourceId cannot be null or empty" );
			}

			//set parameters
			_resourceId = resourceId;
			_resourcePath = resourcePath;
			_creationPoint = creationPoint;
		}
#endregion

#region Public Members
		public TCreateObject CreateObject< TCreateObject >() where TCreateObject : class
		{
			//get prefab
			GameObject prefab;
			try{
				var path = _resourcePath + _resourceId;
				prefab = GamlaResourceManager.GamlaResources.GetResource(path);
			}
			catch( Exception exception )
			{
				throw new CannotCreateResourceException( string.Format( @"Cannot create resource with path {0} and id: {1}", _resourcePath, _resourceId ), exception );
			}
			
			//get component 
			var component = prefab.GetComponent< TCreateObject >();
			if( component == null )
			{
				throw new CannotCreateResourceException( string.Format( @"Component of type: {0} not exist on resources: {1}", typeof( TCreateObject ).Name, _resourceId ) );
			}
			
			//create object
			var newGameObject = Object.Instantiate( prefab, _creationPoint );
			newGameObject.name = _resourceId;
			var result = newGameObject.GetComponent<TCreateObject>();
			
			//add to map
			_createdObjectsMap.Add( result, newGameObject );
			return result;
		}

		public void DisposeItem( TObjectType objectRef )
		{
			//check pres
			if( objectRef == null )
			{
				throw new ArgumentNullException( @"objectRef" );
			}

			//try find game object for GUI component
			GameObject mappedObject;
			if( !_createdObjectsMap.TryGetValue( objectRef, out mappedObject ) )
			{
				throw new CannotDisposeItemException( string.Format( @"Object: {0} not created in Factory and cannot be destroyed", objectRef ) );
			}
			
			//destroy object not safe in EDITOR
			_createdObjectsMap.Remove( objectRef );
			Object.Destroy( mappedObject );
		}
#endregion
	}
