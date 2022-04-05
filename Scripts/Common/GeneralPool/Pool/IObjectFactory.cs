namespace Gamla.Scripts.Common.GeneralPool
{
	public interface IObjectFactory
	{
#region Public Members
		/// <summary>
		/// Creates new object
		/// </summary>
		/// <returns>object ref</returns>
		TObject CreateObject< TObject >() where TObject : class;
#endregion
	}
}