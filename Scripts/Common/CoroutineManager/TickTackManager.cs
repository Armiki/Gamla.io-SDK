using System.Collections.Generic;

namespace Gamla.Logic
{
	public static class TickTackManager
	{
		static List<ITicktackable> items = new List<ITicktackable> ();
		static List<ITicktackable> lateUpdateitems = new List<ITicktackable> ();

		public static void Register (ITicktackable ticktackable, bool isLateUpdate = false)
		{
            if (isLateUpdate) {
                lateUpdateitems.Add(ticktackable);
            } else {
                items.Add (ticktackable);       
            }
        }
        
		public static void Unregister (ITicktackable ticktackable)
		{
			items.Remove (ticktackable);
            lateUpdateitems.Remove(ticktackable);
		}

		public static void LateTickTack ()
		{
			for (int i = 0; i < lateUpdateitems.Count; ++i) {
                lateUpdateitems[i].TickTack ();
			}
		}
        
        public static void TickTack ()
        {
            for (int i = 0; i < items.Count; ++i) {
                items[i].TickTack ();
            }
        }
	}
}
