using System;
using System.Collections;
using Gamla.Data;
using Gamla.UI;
using UnityEngine;

namespace Gamla.Logic
{
    public class FeatureValidationManager : MonoBehaviour
    {
        public const string BIRTH_DATE_PREFS_KEY = "birth_date";
        
        const int MIN_AGE_FEATURE = 17; 
        const int MIN_LEVEL_FEATURE = 2; 
        
        private static FeatureValidationManager _instance;

        public static void ValidateFeature(bool checkLevel, bool checkAge, bool checkRegion, Action<bool> result)
        {
            if (_instance == null)
            {
                _instance = MainCanvas.Canvas.gameObject.AddComponent<FeatureValidationManager>();
            }

            _instance.StartCoroutine(_instance.ValidateUserAccessProcess(checkLevel, checkAge, checkRegion, result));
        }

        IEnumerator ValidateUserAccessProcess(bool checkLevel, bool checkAge, bool checkRegion, Action<bool> result)
        {
            if (checkRegion)
            {
                bool locationReceived = false;
                bool locationResult = false;

                ServerCommand.CheckGeoLocation(b =>
                {
                    locationReceived = true;
                    locationResult = b;
                });
                
                while (!locationReceived)
                {
                    yield return new WaitForEndOfFrame();
                }

                if (!locationResult)
                {
                    UIMapController.OpenSimpleWarningWindow(GUIWarningType.UnavalibleRegion);
                    result(false);
                    yield break;
                }
            }

            if (checkAge)
            {
                //Todo: For debug
                //PlayerPrefs.DeleteKey(BIRTH_DATE_PREFS_KEY);
                //LocalState.currentUser.birth_date = -1;
                
                int birthday = -1;
                if (LocalState.currentUser.innerUserInfo.birthday != null)
                {
                    if (DateTime.TryParse(LocalState.currentUser.innerUserInfo.birthday, out var data))
                    {
                        birthday = DateTime.Now.Year - data.Year;
                    }
                }

                //int.TryParse(, out int birthday);
                if (birthday < 1)
                {
                    UIMapController.OpenBirthDateWindow((age) =>
                    {
                        birthday = age;
                        DateTime dateTime = DateTime.Now;
                        dateTime = dateTime.AddYears(-age);
                        LocalState.currentUser.innerUserInfo.birthday = dateTime.ToShortDateString();
                        ServerCommand.SetBirthday(dateTime.ToShortDateString());
                    });

                    while (birthday < 1)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }

                if (birthday < MIN_AGE_FEATURE)
                {
                    UIMapController.OpenSimpleWarningWindow(GUIWarningType.NotOldAnough);
                    result(false);
                    yield break;
                }
            }

            if (LocalState.currentUser.guest)
            {
                UIMapController.OpenSimpleWarningWindow(GUIWarningType.GuestsUnavailable, null,
                    () => UIMapController.OpenInGameSignUp());
                result(false);
                yield break;
            }

            if (checkLevel)
            {
                var level = LocalState.currentUser.games?.Find(g => g.id == ClientManager.gameId);
                if (level == null || level.level < MIN_LEVEL_FEATURE)
                {
                    UIMapController.OpenSimpleWarningWindow(GUIWarningType.LowLevel);
                    result(false);
                    yield break;
                }
            }

            result(true);
        }
    }
}
