using System;
using System.Collections;
using Gamla.UI;
using UnityEngine;

namespace Gamla.Logic
{
    public class LocationManager : MonoBehaviour
    {
        private static LocationManager _instance;

        public static void FindLocation(Action<bool, LocationInfo> callback)
        {
#if UNITY_EDITOR
            callback?.Invoke(true, new LocationInfo());
            return;
#endif

            if (_instance == null)
            {
                _instance = MainCanvas.Canvas.gameObject.AddComponent<LocationManager>();
            }

            _instance.StartCoroutine(_instance.FindPosition(callback));
        }

        IEnumerator FindPosition(Action<bool, LocationInfo> callback)
        {
            Debug.Log("FindPosition step 0");
            // Starts the location service.
            Input.location.Start();

            // if (!Input.location.isEnabledByUser)
            // {
            //     Debug.Log("FindPosition invoke 0");
            //     callback?.Invoke(false, new LocationInfo());
            //     Input.location.Stop();
            //     yield break;
            // }

            Debug.Log("FindPosition step 1");
            // Waits until the location service initializes
            int maxWait = 10;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                Debug.Log("Input.location.status: " + Input.location.status);
                Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude +
                          " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy +
                          " " + Input.location.lastData.timestamp);
                maxWait--;
            }

            // If the service didn't initialize in 20 seconds this cancels location service use.
            if (maxWait < 1)
            {
                Debug.Log("Location Timed out");
                callback?.Invoke(false, Input.location.lastData);
                Input.location.Stop();
                yield break;
            }

            // If the connection failed this cancels location service use.
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("Location Unable to determine device location");
                callback?.Invoke(false, Input.location.lastData);
                Input.location.Stop();
                yield break;
            }
            else
            {
                callback?.Invoke(true, Input.location.lastData);
                // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
                Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude +
                          " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy +
                          " " + Input.location.lastData.timestamp);
            }

            Debug.Log("FindPosition step 3");
            // Stops the location service if there is no need to query location updates continuously.
            Input.location.Stop();
        }
    }
}

