using System;

namespace Gamla.Scripts.Common.Repository
{
	public sealed class ItemNotFoundExcetion : Exception
	{
		#region Constructors
		public ItemNotFoundExcetion( string message ) : base( message )
		{
		}

		public ItemNotFoundExcetion( string message, Exception innerException ) : base( message, innerException )
		{
		}
		#endregion
	}
}
