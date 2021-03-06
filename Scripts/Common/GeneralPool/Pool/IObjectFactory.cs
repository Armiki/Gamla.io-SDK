namespace Gamla.Logic
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