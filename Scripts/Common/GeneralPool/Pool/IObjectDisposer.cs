namespace Gamla.Logic
{
	public interface IObjectDisposer< in TObjectType >
	{
		/// <summary>
		/// Dispose used presenter
		/// </summary>
		/// <param name="presenter">presenter to dispose</param>
		void DisposeItem( TObjectType presenter );
	}
}