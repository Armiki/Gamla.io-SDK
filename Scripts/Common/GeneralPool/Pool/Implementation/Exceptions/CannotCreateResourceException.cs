using System;

namespace Gamla.Scripts.Common.GeneralPool
{
	public sealed class CannotCreateResourceException : Exception
	{
#region Constructors
		public CannotCreateResourceException( string message, Exception innerException ) : base( message, innerException )
		{
		}

		public CannotCreateResourceException( string message ) : base( message )
		{
		}
#endregion
	}
}