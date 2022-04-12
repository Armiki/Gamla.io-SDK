namespace Gamla.Logic
{
	public interface IPoolFactory< in TKey, in TType > where TType : class
	{
#region Public Members
		/// <summary>
		/// Create pool with key
		/// </summary>
		/// <param name="key">key for pool</param>
		/// <returns>new pool reference</returns>
		IObjectPool< TObjectType > CreatePool< TObjectType >( TKey key ) where TObjectType : class, TType;
#endregion
	}
}