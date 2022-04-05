using System;

namespace Gamla.Scripts.Common
{
    public interface IApplicationStatusService
    {
        void OnApplicationPause(bool pauseStatus);
        void OnApplicationFocus(bool hasFocus);
        void OnApplicationQuit();
        void OnApplicationSaveData();

        void OnApplicationGoingToBackground();

        event Action<bool, float> OnApplicationPauseEvent;
        event Action<bool> OnApplicationFocusEvent;
        event Action OnApplicationQuitEvent;
        event Action OnApplicationSaveTempDataEvent;

        event Action onApplicationGoingToBackground;
    }
}