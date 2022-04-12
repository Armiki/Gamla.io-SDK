namespace Gamla.Logic
{
	public interface ITickTackManager : ITicktackable
	{
		void Register (ITicktackable ticktackable, bool isLateUpdate = false);
		void Unregister (ITicktackable ticktackable);
        void LateTickTack();
    }
}