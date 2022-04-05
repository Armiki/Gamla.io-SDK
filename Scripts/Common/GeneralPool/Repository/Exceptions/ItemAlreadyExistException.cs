using System;


namespace Gamla.Scripts.Common.Repository
{
	public sealed class ItemAlreadyExistException : Exception
	{
		#region Constructors
		public ItemAlreadyExistException( string message ) : base( message )
		{
		}

		public ItemAlreadyExistException( string message, Exception innerException ) : base( message, innerException )
		{
		}
		#endregion
	}
}
