namespace Gamla.Scripts.Common.GeneralPool
{
	public interface IPoolObjectInitializer< in TObject > where TObject : class
	{
#region Public Members
		/// <summary>
		/// Start initialization for object after get from pool
		/// </summary>
		/// <param name="objectRef">reference to polled object</param>
		void UseObject( TObject objectRef, bool newlyCreated);

		/// <summary>
		/// Clear logic for object after return to pool
		/// </summary>
		/// <param name="objectRef">object reference</param>
		void ReleaseObject( TObject objectRef );
#endregion
	}
}