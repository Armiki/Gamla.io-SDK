using UnityEngine;

namespace Gamla.Logic
{
	public class HomeThreadHelper : MonoBehaviour
	{
		public static HomeThread homeThread;
		IApplicationStatusService _applicationStatus;

		void Awake()
		{
			if (homeThread == null)
			{
				homeThread = new HomeThread();
				homeThread.SetUnhadledCoroutineDummy(this);
			}
		}

		void OnApplicationFocus(bool hasFocus)
		{/*
			_applicationStatus?.OnApplicationFocus(hasFocus);
			if (!hasFocus)
			{
				_applicationStatus?.OnApplicationSaveData();
			}*/
		}

		void OnApplicationPause(bool pauseStatus)
		{
			_applicationStatus?.OnApplicationPause(pauseStatus);
		}

		void OnApplicationQuit()
		{
			_applicationStatus?.OnApplicationSaveData();
			_applicationStatus?.OnApplicationGoingToBackground();
#if UNITY_EDITOR
			_applicationStatus?.OnApplicationQuit();
#endif
		}

		void Update()
		{
			homeThread.TickTack();
			TickTackManager.TickTack();
		}

		void LateUpdate()
		{
			TickTackManager.LateTickTack();
		}

		private void OnDestroy()
		{
			homeThread.Clean();
		}
	}
}