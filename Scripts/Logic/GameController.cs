using System;
using System.Collections;
using System.Collections.Generic;
using Gamla.Scripts.Common;
using Gamla.Scripts.Data;
using Gamla.Scripts.UI.Main;
using GamlaSDK.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gamla.Scripts.Logic
{
    public class GameController : MonoBehaviour
    {
        private bool _isInMatch = true;
        private List<ServerNotification> _notifications = new List<ServerNotification>();
        
        
        public void Start()
        {
            Application.targetFrameRate = 60;
            
            GamlaService.Open.Subscribe(Open);
            GamlaService.OnMatchStarted.Subscribe(OnMatchStarted);
            GamlaService.MatchEnd.Subscribe(OnMatchEnd);
            GamlaService.ReturnToGamla.Subscribe(() =>
            {
                if(LocalState.currentMatch != null)
                    ServerCommand.GetMatchInfoAndOpenView(LocalState.currentMatch.match.id);
                ServerCommand.GetOrUpdateProfile(LocalState.token);
                GamlaResourceManager.tabBar.SelectPlay();
                Application.targetFrameRate = 60;
                _isInMatch = false;
            });
            Debug.Log($"simple test random: {GamlaRandom.Range(0, 10)}");
            
            RemoteResourceManager.Init();
            
            HomeThreadHelper.homeThread.ExecuteCoroutine(SendNotification());
            HomeThreadHelper.homeThread.ExecuteCoroutine(CheckNotification());
        }

        void Open(string pushToken)
        {
            LocalState.pushToken = pushToken;
            try
            {
                LocalizationManager.Init(PlayerPrefs.GetString("locale", "english"), () => { Load(); });
                if (EventSystem.current == null)
                {
                    Debug.LogWarning("EventSystem not find! Create some one");
                    var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            Application.targetFrameRate = 60;
        }

        void Load()
        {
            TutorialManager.Instance.InitTutorialBlocker();
            
            var window = UIMapController.OpenLoading();
            UIMapController.SubscribeBar();

            if (PlayerPrefs.HasKey("email"))
            {
                ServerCommand.SendLogin(PlayerPrefs.GetString("email"), PlayerPrefs.GetString("password"), false);
                Debug.Log("auto login");
                window.ChangeProgress(0.8f);
            }
            else
            {
                HomeThreadHelper.homeThread.ExecuteCoroutine(Step1(window, 0.1f));
            }

        }

        IEnumerator Step1(LoadingWindow loading, float amount)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            loading.ChangeProgress(amount);
            HomeThreadHelper.homeThread.ExecuteCoroutine(Step2(loading, 0.8f));
        }

        IEnumerator Step2(LoadingWindow loading, float amount)
        {
            yield return new WaitForSecondsRealtime(2f);
            loading.ChangeProgress(amount);
            HomeThreadHelper.homeThread.ExecuteCoroutine(Step3(loading, 1f));
        }

        IEnumerator Step3(LoadingWindow loading, float amount)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            LoadComplete();
        }
        
        IEnumerator CheckNotification()
        {
            while (true)
            {
                if (!_isInMatch || _notifications.Count == 0)
                {
                    ServerCommand.GetNotification(result =>
                    {
                        _notifications = result.FindAll(notif => notif.read == 0);
                    });
                }
                yield return new WaitForSecondsRealtime(10f);
            }
        }

        IEnumerator SendNotification()
        {
            while (true)
            {
                if (_notifications.Count > 0)
                {
                    for(int i = _notifications.Count - 1; i >= 0; i --)
                    {
                        if (_notifications[i].notification_id == 23)
                        {
                            var notif = _notifications[i];
                            List<Currency> reward = Currency.parse(notif.long_text);
                            UIMapController.OpenRewardWindow(reward);
                        }
                        else if (_notifications[i].notification_id == 7)
                        {
                            var notif = _notifications[i];
                            ServerTournamentEndModel model = JsonUtility.FromJson<ServerTournamentEndModel>(notif.long_text);
                            UIMapController.OpenTournamentEndWindow(model);
                        }
                        else if (_notifications[i].notification_id == 5)
                        {
                            var notif = _notifications[i];
                            ServerLeagueEndModel model = JsonUtility.FromJson<ServerLeagueEndModel>(notif.long_text);
                            UIMapController.OpenLeagueEndWindow(model, false);
                            UIMapController.OpenLeagueEndWindow(model, true);
                        }
                        else
                        {
                            UIMapController.OpenNotification(_notifications[i]);
                            _notifications.RemoveAt(i);
                        }
                        yield return new WaitForSecondsRealtime(2f);
                    }
                }
                yield return new WaitForSecondsRealtime(1f);
            }
        }

        void LoadComplete()
        {
            UIMapController.OpenSignUp();
        }

        private void OnMatchStarted(string matchId, string data, bool isTournament)
        {
            _isInMatch = true;
        }

        private void OnMatchEnd(int score)
        {
            if(LocalState.currentMatch != null)
                ServerCommand.GameFinish(score);
            if(LocalState.currentTournament != null)
                ServerCommand.GameFinishTournament(score);

            _isInMatch = false;
        }

        private void Update()
        {
            EventManager.DelayEvent.Update();
        }
    }
}