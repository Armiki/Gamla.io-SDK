using System;
using System.Collections.Generic;

namespace Gamla.Collections
{
	public sealed class FluentNode< T >
	{
		#region Public Fields
		/// <summary>
		/// The value of node
		/// </summary>
		public readonly T Value;
		#endregion

		#region Constructors
		public FluentNode(T value)
		{
			Value = value;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Next node reference
		/// </summary>
		public FluentNode< T > NextNode { get; private set; }
		#endregion

		#region Public Members
		/// <summary>
		/// Remove next node connection
		/// </summary>
		public void RemoveConnection()
		{
			NextNode = null;
		}

		/// <summary>
		/// Get all tail elements starts from this
		/// </summary>
		public IEnumerable< T > GetElementsStartsFromThis()
		{
			FluentNode< T > node = this;
			while( node != null )
			{
				yield return node.Value;
				node = node.NextNode;
			}
		}

		/// <summary>
		/// Get next count values starts from this node
		/// </summary>
		public FluentNode< T > Iterate( int count, Action< T > action, bool stay = false )
		{
			//check arguments
			if( action == null )
			{
				throw new ArgumentNullException(nameof(action));
			}

			//iterate
			FluentNode< T > node = IterateInternal( count, action, out int realCount, stay );

			//check count -> error
			if( realCount != count )
			{
				throw new ArgumentOutOfRangeException( nameof(count), $@"Not enough elements to iterate. Expect: {count} real: {realCount}" );
			}

			return node;
		}

		/// <summary>
		/// Get next count values starts from this node -> no exceptions
		/// </summary>
		public FluentNode< T > IterateSafe( int count, Action< T > action, bool stay = false  )
		{
			//check arguments
			if( action == null )
			{
				throw new ArgumentNullException(nameof(action));
			}

			return IterateInternal( count, action, out int _, stay );
		}

		/// <summary>
		/// Add element to tail
		/// </summary>
		public FluentNode< T > Add( T value )
		{
			var oldNode = NextNode;
			var newNode = new FluentNode< T >( value );
			NextNode = newNode;

			if( oldNode != null )
			{
				newNode.NextNode = oldNode;
			}

			return newNode;
		}

		/// <summary>
		/// Add elements to tail
		/// </summary>
		public FluentNode< T > AddRange( IEnumerable< T > values )
		{
			FluentNode< T > lastNode = this;
			foreach( T value in values )
			{
				lastNode = lastNode.Add( value );
			}

			return lastNode;
		}

		/// <summary>
		/// Remove count nodes starts from this
		/// </summary>
		/// <returns>same node</returns>
		public FluentNode< T > Remove( int count, Action<T> removeAction = null )
		{
			//try remove
			int removedCount = RemoveInternal( count, removeAction );

			//check count
			if( removedCount != count )
			{
				throw new ArgumentOutOfRangeException( nameof(count), $@"Not enough elements to remove. Expect: {count} real: {removedCount}" );
			}

			//return same node -> no position change after remove
			return this;
		}

		/// <summary>
		/// Remove elements after current. No exception with not enough
		/// </summary>
		public FluentNode< T > RemoveSafe( int count, Action<T> removeAction = null )
		{
			//try remove
			RemoveInternal( count, removeAction );

			//return same node -> no position change after remove
			return this;
		}

		/// <summary>
		/// Returns node with skip count nodes
		/// </summary>
		public FluentNode< T > Skip( int count )
		{
			//remove node
			FluentNode< T > node = IterateSafeHandle( count, null, out int elementsSkiped );

			//rise exception for not enough count
			if( elementsSkiped != count )
			{
				throw new ArgumentOutOfRangeException( nameof(count), $@"Not enough elements to skip. Expect: {count} real: {elementsSkiped}" );
			}

			return node;
		}

		/// <summary>
		/// Skip element with return last element if impossible. Return last if count > next elements count
		/// </summary>
		public FluentNode< T > SkipSafe( int count )
		{
			return IterateInternal( count, null, out _ );
		}
		#endregion

		#region Private Fields
		/// <summary>
		/// Check count with exception
		/// </summary>
		/// <param name="count"></param>
		private void CheckCount( int count )
		{
			if( count < 0 )
			{
				throw new ArgumentOutOfRangeException( nameof(count), @"Must be >= 0" );
			}
		}

		/// <summary>
		/// Remove elements from this node -> return same node always
		/// </summary>
		private int RemoveInternal( int count, Action< T > removeAction )
		{
			//check arguments
			CheckCount(count);

			//iterate
			if( NextNode == null )
			{
				return 0;
			}

			FluentNode< T > nodeAfterSkip = NextNode.IterateInternal( count, removeAction, out int successCount, out bool safe );

			//replace old chain
			NextNode = successCount == count && safe ? nodeAfterSkip : null;
			return successCount;
		}

		/// <summary>
		/// iterate nodes with action. If last node was null return count - 1
		/// </summary>
		private FluentNode< T > IterateSafeHandle( int count, Action< T > removeAction, out int successCount )
		{
			//skip nodes
			var iterateNode = IterateInternal( count, removeAction, out successCount, out bool safe );

			//check safe
			if (!safe)
			{
				successCount--;
			}

			return iterateNode;
		}

		/// <summary>
		/// iterate nodes with action
		/// </summary>
		private FluentNode< T > IterateInternal( int count, Action< T > action, out int successCount, bool stay = false )
		{
			return IterateInternal( count, action, out successCount, out bool _ , stay );
		}

		/// <summary>
		/// iterate nodes with action
		/// </summary>
		private FluentNode< T > IterateInternal( int count, Action< T > action, out int successCount, out bool tailSafe, bool stay = false )
		{
			//check arguments
			CheckCount( count );

			successCount = 0;
			FluentNode< T > node = this;
			FluentNode< T > lastNode = this;
			while( node != null && successCount != count )
			{
				//make action
				action?.Invoke( node.Value );

				//update node
				lastNode = node;
				node = node.NextNode;

				//update count
				successCount++;
			}

			//if stay -> stand on last node
			if( stay )
			{
				tailSafe = true;
				return lastNode;
			}

			//never null
			tailSafe = node != null;
			return node ?? lastNode;
		}
		#endregion
	}
}
