using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamla.Data;
using Gamla.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gamla.Logic
{
    public class GameController : MonoBehaviour
    {
        private const long RewardNotifId = 23;
        private const long TournamentEndNotifId = 7;
        private const long LeagueNotifId = 5;
        
        private bool _isInMatch = false;
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
                UIMapController.OpenLoading();
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
                if (!_isInMatch && _notifications.Count == 0)
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
                if (!_isInMatch && _notifications.Count > 0)
                {
                    for(int i = _notifications.Count - 1; i >= 0; i --)
                    {
                        if (_notifications[i].notification_id == RewardNotifId)
                        {
                            var notif = _notifications[i];
                            List<Currency> reward = Currency.parse(notif.long_text);
                            UIMapController.OpenRewardWindow(_notifications[i].id, reward);
                            _notifications.RemoveAt(i);
                        }
                        else if (_notifications[i].notification_id == TournamentEndNotifId)
                        {
                            var notif = _notifications[i];
                            ServerTournamentEndModel model = JsonUtility.FromJson<ServerTournamentEndModel>(notif.long_text);
                            if (model == null) {
                                Debug.LogError($"Failed parse TournamentEndModel: {notif.long_text}");
                                ShowSimpleNotification(i);
                                continue;
                            }
                            UIMapController.OpenTournamentEndWindow(_notifications[i].id, model);
                            _notifications.RemoveAt(i);
                        }
                        else if (_notifications[i].notification_id == LeagueNotifId)
                        {
                            var notif = _notifications[i];
                            ServerLeagueEndModel model = JsonUtility.FromJson<ServerLeagueEndModel>(notif.long_text);
                            if (model == null) {
                                Debug.LogError($"Failed parse LeagueEndModel: {notif.long_text}");
                                ShowSimpleNotification(i);
                                continue;
                            }
                            UIMapController.OpenLeagueEndWindow(_notifications[i].id, model, false);
                            UIMapController.OpenLeagueEndWindow(_notifications[i].id, model, true);
                            _notifications.RemoveAt(i);
                        }
                    }
                    UIMapController.TryShowingPendingWindow();
                    
                    for (int i = _notifications.Count - 1; i >= 0; i--)
                    {
                        if (IsSimpleNotification(_notifications[i].notification_id))
                        {
                            ShowSimpleNotification(i);
                        }
                        yield return new WaitForSecondsRealtime(2f);
                    }
                }
                yield return new WaitForSecondsRealtime(1f);
            }
        }

        bool IsSimpleNotification(long id)
        {
            return id != LeagueNotifId && id != TournamentEndNotifId && id != RewardNotifId;
        }

        void ShowSimpleNotification(int index)
        {
            if (index < 0 || index >= _notifications.Count) {
                Debug.LogError($"Wrong index notification index:{index}");
                return;
            }
            
            UIMapController.OpenNotification(_notifications[index]);
            _notifications.RemoveAt(index);
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
            if (LocalState.currentMatch != null)
                ServerCommand.GameFinish(score);
            if (LocalState.currentTournament != null)
            {
                var tournament = LocalState.tournaments.Find(t => t.id == LocalState.currentTournament.id);

                if (tournament != null) {
                    var match = tournament.matches.Find(m =>
                        m.players.Any(p => p.user_id == LocalState.currentUser.uid && string.IsNullOrEmpty(p.score)));
                    if (match != null) {
                        var player = match.players.Find(p =>
                            p.user_id == LocalState.currentUser.uid && string.IsNullOrEmpty(p.score));
                        if (player != null) {
                            player.score = score.ToString();
                        }
                    }
                }

                foreach (var match in  LocalState.currentTournament.matches) {
                    foreach (var p in match.players) {
                        if (p.user_id == LocalState.currentUser.uid) {
                            p.score = score.ToString();
                        }
                    }
                }
                
                ServerCommand.GameFinishTournament(score);
                ServerCommand.GetTournaments(null);
            }

            _isInMatch = false;
        }

        private void Update()
        {
            EventManager.DelayEvent.Update();
        }
    }
}