using System;

namespace Gamla.Logic
{
	public sealed class CannotDisposeItemException : Exception
	{
#region Constructors
		public CannotDisposeItemException( string message, Exception innerException ) : base( message, innerException )
		{
		}

		public CannotDisposeItemException( string message ) : base( message )
		{
		}
#endregion
	}
}