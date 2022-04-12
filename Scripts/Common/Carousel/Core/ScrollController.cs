using System;
using System.Collections.Generic;
using System.Linq;
using Gamla.Collections;
using UnityEngine;

namespace Gamla.UI.Carousel
{
	public sealed class ScrollController : IScrollController
	{
		#region Private Fields
		private int _endIndex = -1;
		private int _startIndex;

		private readonly IScrollElementsHolder _elementsHolder;
		private readonly IScrollElementFactory _elementsFactory;
		private readonly FluentNode< ScrollNode > _startNode = new FluentNode< ScrollNode >( null );
		#endregion

		#region Constructors
		public ScrollController( IScrollElementsHolder elementsHolder, IScrollElementFactory elementsFactory )
		{
			_elementsHolder = elementsHolder ?? throw new ArgumentNullException( nameof(elementsHolder) );
			_elementsFactory = elementsFactory ?? throw new ArgumentNullException( nameof(elementsFactory) );

			//add handlers for holders events
			_elementsHolder.OnElementSizeChanged += ( sender, args ) => ChangeElement( args );
			_elementsHolder.OnHolderChanged += HandleHolderChanged;
        }
        #endregion

		#region Public Events
		public event EventHandler< ScrollControllerChangedEventArgs > OnSizeChanged = delegate { };
		public event EventHandler<ScrollControllerChangedEventArgs> OnPositionCompensation = delegate { };
		public event EventHandler< ScrollControllerChangedEventArgs > OnPositionChanged = delegate { };
		public event EventHandler<ScrollControllerChangedEventArgs> OnStartItemChanged = delegate { };
		#endregion

		#region Public Properties
		public float Position { get; private set; }
		public float WindowSize { get; private set; }
		#endregion

		#region Public Members
		public void SetPosition( float position )
		{
			////check same position set
			//if( Mathf.Approximately( position, Position ) )
			//{
			//	return;
			//}

			//no init with size -> no actions
			if( !HasElements )
			{
				return;
			}

			int newStart = _elementsHolder.FindElementOnPosition( position );
			int newEnd = _elementsHolder.FindElementOnPosition( position + WindowSize );

			//update position
			float tempPosition = Position;
			Position = position;

			if( Position < tempPosition )
			{
				int updateCount = Math.Max( 0, newEnd - _startIndex + 1 );
				int addCount = newEnd < _startIndex ? newEnd - newStart + 1 : _startIndex - newStart;
				int removeCount = newEnd >= _startIndex ? _endIndex - newEnd : ElementsCount;
				AddUpdateRemove( 0, newStart, addCount, updateCount, removeCount );
			}
			else
			{
				int startAdd = Math.Max( _endIndex + 1, newStart );
				int addCount = newEnd - startAdd + 1;
				int removeCount = newStart <= _endIndex ? newStart - _startIndex : ElementsCount;
				int updateCount = Math.Max( 0, _endIndex - newStart + 1 );
				RemoveUpdateAdd( 0, removeCount, updateCount, startAdd, addCount );
			}

			if (_startIndex != newStart) {
				OnStartItemChanged(this, new ScrollControllerChangedEventArgs(_startIndex, newStart));
			}

			//set index
			_endIndex = newEnd;
			_startIndex = newStart;

			//rise event
			OnPositionChanged( this, new ScrollControllerChangedEventArgs( tempPosition, Position ) );
		}

		public void SetWindowSize( float windowSize )
		{
			//check range
			if( windowSize < 0 )
			{
				throw new ArgumentOutOfRangeException( nameof(windowSize), @"Cannot be less then zero" );
			}

			//check same size set
			if( Mathf.Approximately( windowSize, WindowSize ) )
			{
				return;
			}

			//resize
			if( _elementsHolder.TotalCount != 0 )
			{
				int newIndex = _elementsHolder.FindElementOnPosition( Position + windowSize );
				int indexDiff = Math.Abs( newIndex - _endIndex );

                if (indexDiff != 0) {
                    if (windowSize > WindowSize) {
                        int skipCount = Math.Max(_endIndex + 1 - _startIndex, 0);
                        RemoveUpdateAdd(skipCount, 0, 0, _endIndex + 1, indexDiff);
                    } else {
                        int skipCount = newIndex - _startIndex + 1;
                        AddUpdateRemove(skipCount, 0, 0, 0, indexDiff);
                    }
                }

				//set
				_endIndex = newIndex;
			}
		
			//update size
			float oldSize = WindowSize;
			WindowSize = windowSize;

			//rise event about change size
			OnSizeChanged( this, new ScrollControllerChangedEventArgs( oldSize, WindowSize ) );
		}

		public void Reset()
		{
			//remove all elements
			if( HasElements )
			{
				_startNode.Remove( _endIndex - _startIndex + 1, RemoveScrollComponent );
			}

			//reset to start position and index`s
			ClearIndex();

			Position = 0;
			WindowSize = 0;
		}
		
		/// <summary>
		/// Gets all scroll nodes
		/// </summary>
		public IEnumerable< ScrollNode > GetScrollElements()
		{
			return _startNode.NextNode != null ? _startNode.NextNode.GetElementsStartsFromThis() : Enumerable.Empty< ScrollNode >();
		}
		#endregion

		#region Private Properties
		/// <summary>
		/// True if has elements
		/// </summary>
		private bool HasElements => _endIndex >= 0;

		/// <summary>
		/// Current element count in controller
		/// </summary>
		private int ElementsCount => _endIndex - _startIndex + 1;
		#endregion

		#region Private Members
		/// <summary>
		/// Skip some elements then add one or zero elements then remove one or zero element then checnge indexes
		/// </summary>
		private void AddRemoveChangeIndex( int skipCount, int addIndex, bool needRemove, int indexChange )
		{
			FluentNode< ScrollNode > currentNode = _startNode.Skip( skipCount );

			//add new node
			if( addIndex >= 0 )
			{
				ScrollNode node = GetNodeForIndex( addIndex );
				currentNode = currentNode.Add( node );
			}

			//remove old
			if( needRemove )
			{
				currentNode = currentNode.Remove( 1, RemoveScrollComponent );
			}

			//update indexes if need
			if( indexChange != 0 )
			{
				foreach( ScrollNode node in currentNode.SkipSafe( 1 ).GetElementsStartsFromThis() )
				{
					if( node != null )
					{
						node.Index += indexChange;
					}
				}
			}
		}

		/// <summary>
		/// Remove elements then update then add
		/// </summary>
		private void RemoveUpdateAdd( int start, int removeCount, int updateCount, int addPos, int addCount )
		{
			IEnumerable< ScrollNode > newNodes = GenerateNewNodes( addPos, addCount );
			int skipCount = updateCount != 0 ? 1 : 0;
			_startNode
				.Skip( start )
				.Remove( removeCount, RemoveScrollComponent )
				.SkipSafe( skipCount )
				.Iterate( updateCount, UpdateScrollComponent )
				.AddRange( newNodes );
		}

		/// <summary>
		/// Add elements then update then remove
		/// </summary>
		private void AddUpdateRemove( int start, int addPos, int addCount, int updateCount, int removeCount )
		{
			IEnumerable< ScrollNode > newNodes = GenerateNewNodes( addPos, addCount );
			int skipCount = updateCount != 0 ? 1 : 0;
			_startNode
				.Skip( start )
				.AddRange( newNodes )
				.SkipSafe( skipCount )
				.Iterate( updateCount, UpdateScrollComponent, true )
				.Remove( removeCount, RemoveScrollComponent );
		}

		/// <summary>
		/// Change element size with SizeChangedEventArgs
		/// </summary>
		private void ChangeElement( SizeChangedEventArgs e )
		{
			//check no elements
			if( !HasElements )
			{
				return;
			}

			//update position + no change
			if( e.Index <= _startIndex ) //fixed bug when changing size of first visible element
			{
				if(!NearStartPosition(e.Index))
                    //no need compensation if we on start position
				{
					OnPositionCompensation?.Invoke(this, new ScrollControllerChangedEventArgs(e.OldSize, e.NewSize));
				}
				UpdatePosition( e.Delta );
			}
			else if( e.Index <= _endIndex )
			{
				int skipCount = e.Index - _startIndex + 1;
				int newEnd = _elementsHolder.FindElementOnPosition( Position + WindowSize );
				int updateCount = Math.Min( newEnd, _endIndex ) - e.Index;
				int indexDiff = Math.Abs( newEnd - _endIndex );

				if( e.Delta > 0 )
				{
					//remove elements from tail
					AddUpdateRemove( skipCount, 0, 0, updateCount, indexDiff );
				}
				else
				{
					//add elements to tail
					RemoveUpdateAdd( skipCount, 0, updateCount, _endIndex + 1, indexDiff );
				}

				//fixed bug when elemets adding/removing to window by resize
				_endIndex = newEnd;
			}
		}

		/// <summary>
		/// Handler for elements added or removed
		/// </summary>
		private void HandleHolderChanged(object sender, ElementsHolderChangedArgs args)
		{
			//check for not affecting changes
			if (_endIndex < args.Index && HasElements)
			{
				return;
			}

			bool notMiddle = _startIndex >= args.Index;

			if (notMiddle)
			{
				switch (args.Reason)
				{
					case ElementChangeReason.Added:
						if (!NearStartPosition(args.Index)) 
                            //no need compensation if we on start position
						{
							OnPositionCompensation?.Invoke(this,
								new ScrollControllerChangedEventArgs(0, args.ElementSize));
						}

						break;
					case ElementChangeReason.Removed:
						if (!NearStartPosition(args.Index)) 
                            //no need compensation if we on start position
						{
							OnPositionCompensation?.Invoke(this,
								new ScrollControllerChangedEventArgs(args.ElementSize, 0));
						}

						break;
					default:
						throw new ArgumentOutOfRangeException(args.Reason.ToString(), args.Reason, null);
				}
			}
		}

        bool NearStartPosition(int index)
        {
            float minDelta = 0.1f;
            return Mathf.Abs(GetElementPosition(index)) <= minDelta;
        }

		/// <summary>
		/// Init elements if not exist
		/// </summary>
		private void InitElements()
		{
			_startIndex = _elementsHolder.FindElementOnPosition( Position );
			_endIndex = _elementsHolder.FindElementOnPosition( Position + WindowSize );

			AddUpdateRemove( 0, _startIndex, _endIndex - _startIndex + 1, 0, 0 );
		}

		/// <summary>
		/// Update position with event
		/// </summary>
		/// <param name="delta">delta of position</param>
		private void UpdatePosition( float delta )
		{
			float oldPosition = Position;
			Position += delta;
			OnPositionChanged( this, new ScrollControllerChangedEventArgs( oldPosition, Position ) );
		}

		/// <summary>
		/// Create s new nodes for start position and count
		/// </summary>
		private IEnumerable< ScrollNode > GenerateNewNodes( int startPosition, int count )
		{
			var newNodesList = new List< ScrollNode >();
			for( int i = 0; i < count; i++ )
			{
				int index = startPosition + i;
				ScrollNode node = GetNodeForIndex( index );

				//add result
				newNodesList.Add( node );
			}

			return newNodesList;
		}

		/// <summary>
		/// Create scroll element on position
		/// </summary>
		private ScrollNode GetNodeForIndex( int index )
		{
			IScrollElement element = _elementsFactory.CreateElement( index );
			element.Position = GetElementPosition( index );

			return new ScrollNode( index, element );
		}

		/// <summary>
		/// Handler for remove scroll components
		/// </summary>
		private void RemoveScrollComponent( ScrollNode node )
		{
			_elementsFactory.DisposeElement( node.ScrollElement );
		}

		/// <summary>
		/// Handler for update scroll components
		/// </summary>
		private void UpdateScrollComponent( ScrollNode node )
		{
			node.ScrollElement.Position = GetElementPosition( node.Index );
		}

		/// <summary>
		/// Get relevant element position
		/// </summary>
		private float GetElementPosition( int index )
		{
			return _elementsHolder.GetElementPosition( index ) - Position;
		}

		/// <summary>
		/// Clear indexes
		/// </summary>
		private void ClearIndex()
		{
			_startIndex = 0;
			_endIndex = -1;
		}
		#endregion
	}
}