﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Data;
using Gamla.Scripts.UI.Friends;
using Gamla.Scripts.UI.Game;
using Gamla.Scripts.UI.Ladder;
using Gamla.Scripts.UI.Main;
using Gamla.Scripts.UI.Profile;
using Gamla.Scripts.UI.Store;
using Gamla.Scripts.UI.Tournament;
using GamlaSDK.Scripts;
using GamlaSDK.Scripts.UI.Game;
using GamlaSDK.Scripts.UI.Tournament;
using UnityEngine;

namespace Gamla.Scripts.Logic
{
    public static class UIMapController
    {
        private static Stack<GUIView> _windowStack = new Stack<GUIView>();

        public static void CheckStack(GUIView openView)
        {
            if (openView.windowMode == WindowMode.None)
            {
                return;
            }

            if(openView.windowMode == WindowMode.Screen)
            {
                while(_windowStack.Count > 0)
                {
                    try
                    {
                        GameObject.Destroy(_windowStack.Pop().gameObject);
                    }
                    catch (Exception e){ }
                }
            }
            if (openView.windowMode == WindowMode.Full)
            {
                DestroyCurrentWindow();
            }

            if (openView.windowMode == WindowMode.FullDialog || openView.windowMode == WindowMode.Dialog)
            {
                if (_windowStack.Count > 0)
                {
                    GUIView current = _windowStack.Peek();
                    if(current != null) current.Hide();
                }
            }

            _windowStack.Push(openView);
        }

        public static void DestroyCurrentWindow()
        {
            if (_windowStack.Count > 0)
            {
                GameObject.Destroy(_windowStack.Pop()?.gameObject);
            }
        }
        
        public static void DestroyWindowBefore(string name)
        {
            if(GetWindow(name) == null) return;
            
            while (_windowStack.Count > 0)
            {
                var win = _windowStack.Peek();
                if (win.name.Contains(name) || name.Contains(win.name))
                {
                    DestroyCurrentWindow();
                    break;
                }
                DestroyCurrentWindow();
            }

            if (_windowStack.Count > 0)
            {
                _windowStack.Peek().Show();
            }
        }

        public static GUIView GetWindow(string name)
        {
            return _windowStack.FirstOrDefault(x => x.name.Contains(name));
        }

        public static LoadingWindow OpenLoading()
        {
            var window =
             GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/LoadingWindow"),
                 GamlaResourceManager.windowsContainer).GetComponent<LoadingWindow>();
            CheckStack(window);
            window.Show();
            return window;
        }

        public static void CloseExtraWindows()
        {
            var find = GamlaResourceManager.windowsContainer.Find("FinishWindow(Clone)");
            if (find != null) {
                find.GetComponent<FinishWindow>().ClosePublic();
            }
        }

        public static void CloseSpinner()
        {
            var spinner = GamlaResourceManager.windowsContainer.Find("ValidateWindow(Clone)");
            if (spinner != null) {
                spinner.GetComponent<ValidateWindow>().ClosePublic();
            }
        }
        
        public static void SubscribeBar()
        {
            EventManager.OnProfileUpdate.Subscribe(() => GamlaResourceManager.topBar.Init(LocalState.currentUser));
            GamlaService.OnMatchStarted.Subscribe((s, s1, arg3) =>
            {
                GamlaResourceManager.tabBar.gameObject.SetActive(false);
                GamlaResourceManager.topBar.gameObject.SetActive(false);
                GamlaResourceManager.BackView = false;
            });

            GamlaResourceManager.topBar.onProfileClick +=  GamlaResourceManager.tabBar.SelectProfile;
            GamlaResourceManager.topBar.onAddCurrencyClick += OpenTicketShop;
            
            GamlaResourceManager.tabBar.onGamesClick += OpenGameList;
            GamlaResourceManager.tabBar.onPlayClick += OpenGameMain;
            GamlaResourceManager.tabBar.onProfileClick += OpenProfile;
            GamlaResourceManager.tabBar.onTopClick += OpenLadder;
            GamlaResourceManager.tabBar.onStoreClick += OpenStore;
            
            EventManager.OnUserWidgetClick.Subscribe(OpenUserInfoWindow);
        }

        public static void OpenNotification(ServerNotification notification)
        {
            var notif =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Widgets/NotificationWidget"),
                    GamlaResourceManager.windowsContainer).GetComponent<NotificationWidget>();
            notif.Init(notification);
        }

        public static void OpenSignUp()
        {
            var window =
            GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/SignUpWindow"),
                GamlaResourceManager.windowsContainer).GetComponent<SignUpWindow>();
            window.onSignUpClick += OpenAccountSignUp;
            window.onGuestClick += ServerCommand.SignUpAsGuest;
            window.onSignInClick += OpenLogin;
            CheckStack(window);
            window.Show();
        }
        

        static void OpenAccountSignUp()
        {
            var window =
                  GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/SignUpAccountWindow"),
                       GamlaResourceManager.windowsContainer).GetComponent<SignUpAccountWindow>();
            window.onSignUpAppleClick += SignUpStoreAccount;
            window.onSignUpGoogleClick += SignUpStoreAccount;
            window.onSignUpEmailClick += OpenSignUpEmail;
            CheckStack(window);
            window.Show();
        }

        static void SignUpStoreAccount()
        {
//            MockData.currentUser = MockData.FillStoreAccountUser();
//            EventManager.OnProfileUpdate.Publish();
//            GameResourceManager.tabBar.SelectPlay();
        }

        static void OpenSignUpEmail()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/SignUpEmailWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<SignUpEmailWindow>();
            CheckStack(window);
            window.onSignUpClick += ServerCommand.SignUp;
            window.onLogInClick += OpenLogin;
            window.Show();
        }
        
        public static void OpenLogin()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/LogInWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<LogInWindow>();
            CheckStack(window);
            window.onLogInClick += (x, y) =>
            {
                ServerCommand.SendLogin(x, y, false);
            };
            window.onSignUpClick += OpenSignUpEmail;
            window.onForgotPasswordClick += OpenForgotPassword;
            window.Show();
        }
        
        static void OpenForgotPassword()
        {
            Application.OpenURL("https://gamla.io/password/reset");
            return;
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ForgotPasswordWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<ForgotPasswordWindow>();
            CheckStack(window);
            window.onResetEmailClick += (email) => ServerCommand.SendResetLink(email, "", true);
            window.onResetPhoneClick += (phone) => ServerCommand.SendResetLink("", phone, true);
            window.Init();
            window.Show();
        }

       
        
        static void OpenResetPublishName()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ChangeProfileItemWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<ChangeProfileItemWindow>();
            CheckStack(window);
            window.ChangePublishName(LocalState.currentUser.name);
            window.onChangeProfileItem += ServerCommand.ResetPublishName;
            window.Show();
        }
        
        static void OpenTicketShop()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/TicketShopWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<TicketShopWindow>();
            CheckStack(window);
            
            window.Show();
        }
        
        static void OpenResetName()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ChangeProfileItemWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<ChangeProfileItemWindow>();
            CheckStack(window);
            window.ChangeName(LocalState.currentUser.innerUserInfo.firstName, LocalState.currentUser.innerUserInfo.lastName);
            window.onChangeProfileItem += ServerCommand.ResetName;
            window.Show();
        }
        
        static void OpenResetAddress()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ChangeAddressWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<ChangeAddressWindow>();
            CheckStack(window);
            window.ChangeAddress(LocalState.currentUser.innerUserInfo.address);
            window.onChangeAddress += (newAddress) =>
            {
                var editWindow = GetWindow("EditProfileWindow");
                if (editWindow != null)
                {
                    var editProfileWindow = (EditProfileWindow) editWindow;
                    if (editProfileWindow != null)
                    {
                        editProfileWindow.ChangeAddress(newAddress);
                    }
                }
            };
            window.Show();
        }
        
        static void OpenResetPhone()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ChangeProfileItemWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<ChangeProfileItemWindow>();
            CheckStack(window);
            window.ChangePhone(LocalState.currentUser.innerUserInfo.phone);
            window.onChangeProfileItem += ServerCommand.ResetPhone;
            window.Show();
        }
        
        public static void OpenResetPassword()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ChangeProfileItemWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<ChangeProfileItemWindow>();
            CheckStack(window);
            window.ChangePassword();
            window.onChangeProfileItem += ServerCommand.ResetPassword;
            window.Show();
        }

        public static void OpenConfirmCodeWindow(string email, string phone, bool sendCode)
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ConfirmCodeWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<ConfirmCodeWindow>();
            CheckStack(window);
            if (sendCode)
            {
                window.InitCodeState(email, phone);
                window.onCodePressed += ServerCommand.CheckCode;
            }
            else
            {
                window.InitEmailState();
            }
            window.onEmailCheck += ServerCommand.ChangeEmail;
            window.Show();
        }
        
        public static void OpenGameList()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/GameListWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<GameListWindow>();
            CheckStack(window);

            //OpenNotification("Hey! Hey! Play the game!");
        }
        
        public static void OpenGameMain()
        {
            var window =
               GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/GameMainWindow"),
                   GamlaResourceManager.windowsContainer).GetComponent<GameMainWindow>();
            window.onOpenBattleList += OpenBattleList;
            window.onOpenTournamentList += OpenTournamentList;
            CheckStack(window);
            window.Init(LocalState.currentGame);
            CloseSpinner();
            //OpenNotification("Hey! Hey! Play the game!");
        }

        static void OpenBattleList()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/NewGameWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<NewGameWindow>();
            window.onPlayGameClick += OpenSearchOpponents;
            CheckStack(window);
            window.Init(LocalState.currentGame.battles);
        }
        
        static void OpenTournamentList(ServerTournamentModel tournament)
        {
            //TournamentStartWindow
            bool isFirstTournament = !tournament.isJoined;// tournament.participants.Find(p => p.id == LocalState.currentUser.uid) == null;//.matches.Find(m => m.players != null && m.Any(p => p.user_id == LocalState.currentUser.uid)) == null;
            if (isFirstTournament)
            {
                var window =
                    GameObject.Instantiate(
                        GamlaResourceManager.GamlaResources.GetResource("Windows/TournamentStartWindow"),
                        GamlaResourceManager.windowsContainer).GetComponent<TournamentStartWindow>();
                window.Init(tournament, () =>
                {
                    ServerCommand.JoinTournament(tournament.id);
                });
                CheckStack(window);
                window.Show();
            }
            else
            {
                var window =
                    GameObject.Instantiate(
                        GamlaResourceManager.GamlaResources.GetResource("Windows/TournamentBoardWindow"),
                        GamlaResourceManager.windowsContainer).GetComponent<TournamentBoardWindow>();
                window.SetData(tournament);
                CheckStack(window);
                window.Show();
            }
        }

        static void OpenProfile()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ProfileWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<ProfileWindow>();
            window.onEditProfileClick += OpenEditProfile;
            window.onLanguageClick += OpenChooseLangWindow;
            window.onFriendsClick += OpenFriends;
            window.onAccountBalanceClick += OpenAccountBalanceWindow;
            window.onInfoClick += OpenInfoPanelWindow;
            window.onSignUpClick += OpenEditProfile;
            window.onShowMoreGameLvlsClick += OpenGameProgressList;
            window.onShowMoreTrophieClick += OpenTrophiesList;
            window.onCreateTournamentClick += OpenCreateTournamentWindow;
            window.onJoinTournamentClick += OpenJoinTournamentWindow;
            window.onAttachReferral += AttachReferralCodeWindow;
            window.Init(LocalState.currentGame, LocalState.currentUser);
            window.onRequestNewData += () => window.Init(LocalState.currentGame, LocalState.currentUser);
            window.onNotificationUpdate += ServerCommand.ResetNotification;
            CheckStack(window);
            window.Show();
        }

        static void OpenCreateTournamentWindow()
        {
            var level = LocalState.currentUser.games.Find(g => g.id == ClientManager.gameId);
            if (level == null || level.level < 2)
            {
                OpenSimpleErrorWindow("Need 2 game level!");
                return;
            }
            
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/CreateTournamentWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<CreateTournamentWindow>();
            window.onCreateTournament += (model, callback) => ServerCommand.CreateTournament(model, callback);
            CheckStack(window);
            window.Show();
        }
        
        static void OpenJoinTournamentWindow()
        {
            var curGame = LocalState.currentUser.games.Find(g => g.id == ClientManager.gameId);
            if (curGame == null || curGame.level < 2)
            {
                OpenSimpleErrorWindow("Need 2 game level!");
                return;
            }
            
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/PrivateTournamentCodeWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<PrivateTournamentCodeWindow>();
            window.onPromoCodeClick += ServerCommand.JoinPrivateTournament;
            CheckStack(window);
            window.Show();
        }
        
        static void AttachReferralCodeWindow()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/RedeemReferralCodeWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<PrivateTournamentCodeWindow>();
            window.onPromoCodeClick += ServerCommand.AttachReferral;
            CheckStack(window);
            window.Show();
        }

        static void OpenUserInfoWindow(ServerPublicUser user)
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/UserInfoWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<UserInfoWindow>();
            window.Init(user, LocalState.friends.friends.Any(f => f.id == user.id));
            window.invite += ServerCommand.FriendsAdd;
            window.sendMessage += ServerCommand.GetOrCreateFriendChat;
            window.sendMessage += l =>
            {
                CloseExtraWindows();
            };
            window.profile += l =>
            {
                //Todo: find better solution
                var find = GamlaResourceManager.windowsContainer.Find("FinishWindow(Clone)");
                if (find != null) {
                    find.GetComponent<FinishWindow>().ClosePublic();
                }
                
                window.Hide();
                OpenPublicProfile(l);
            };
            CheckStack(window);
            window.Show();
        }

        static void OpenFriends()
        {
            ServerCommand.GetOrUpdateFriends(LocalState.token);
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/FriendsWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<FriendsWindow>();
            window.InitFriends(LocalState.friends);
            window.onSelectFriend += ServerCommand.GetOrCreateFriendChat;
            CheckStack(window);
            window.Show();

            //EventManager.OnProfileUpdate.Subscribe(() => window.InitMessages(new List<string> {"test", "message"}));
            window.onClosed += (_) =>
            {
                EventManager.OnProfileUpdate.Del(null);
            };
        }
        
        public static void OpenChat(long chatId, List<ServerChatMessage> messages)
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ChatWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<ChatWindow>();
            window.InitMessages(chatId, messages);
            CheckStack(window);
            window.Show();

            //EventManager.OnProfileUpdate.Subscribe(() => window.InitMessages(new List<string> {"test", "message"}));
            window.onClosed += (_) =>
            {
                
            };
        }
        
        static void OpenGameProgressList()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/GameLevelsWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<GameLevelsWindow>();
            CheckStack(window);
            window.InitGameProgress(LocalState.currentUser.games);
        }
        
        static void OpenTrophiesList()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/TrophiesWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<TrophiesWindow>();
            CheckStack(window);
        }

        static void OpenEditProfile()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/EditProfileWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<EditProfileWindow>();
            CheckStack(window);
            window.Init(LocalState.currentUser);
            window.onSaveClick += ResetUserInnerInfo;
            window.onCancelSaveClick += (newData) =>
            {
                if (LocalState.currentUser != newData)
                {
                    OpenSimpleWarningWindow(GUIWarningType.NotChangePersonInfo, null,
                        () => { ResetUserInnerInfo(newData); });
                }
            };
            window.onAddressChangeClick += OpenResetAddress;
            window.onPasswordChangeClick += OpenResetPassword;
            window.onLogOutClick += () =>
            {
                OpenSimpleWarningWindow(GUIWarningType.LogOutAsk, null,ServerCommand.LogOut);
            };
            window.onRequestNewData += () => window.Init(LocalState.currentUser);
            window.Show();
        }

        static void ResetUserInnerInfo(UserInfo newUSer)
        {
            if (LocalState.currentUser.name != newUSer.name)
            {
                ServerCommand.ResetPublishName(newUSer.name, "");
            }
            
            if (LocalState.currentUser.innerUserInfo.firstName != newUSer.innerUserInfo.firstName ||
                LocalState.currentUser.innerUserInfo.lastName != newUSer.innerUserInfo.lastName)
            {
                ServerCommand.ResetName(newUSer.innerUserInfo.firstName, newUSer.innerUserInfo.lastName);
            }
            
            if (LocalState.currentUser.innerUserInfo.phone != newUSer.innerUserInfo.phone)
            {
                ServerCommand.ResetPhone(newUSer.innerUserInfo.phone, "");
            }
            
            if (LocalState.currentUser.innerUserInfo.email != newUSer.innerUserInfo.email)
            {
                ServerCommand.ChangeEmail(newUSer.innerUserInfo.email, false);
            }
            
            if (LocalState.currentUser.innerUserInfo.address != newUSer.innerUserInfo.address)
            {
                ServerCommand.ResetAddress(newUSer.innerUserInfo.address);
            }

            LocalState.currentUser = newUSer;
            OpenSimpleInfoWindow(GUIInfoType.ChangePersonInfoSuccess);
        }

        static void OpenLadder()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/LadderWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<LadderWindow>();
            CheckStack(window);
            window.Init(LocalState.currentGame, LocalState.currentUser);
        }
        
        static void OpenStore()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/StoreWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<StoreWindow>();
            window.Init(LocalState.currentGame, LocalState.currentUser.promoCode, LocalState.storePacks);
            window.onPromoCodeClick += OpenPromoCodeWindow;
            CheckStack(window);
            window.Show();
        }

       public static void OpenSearchOpponents(BattleInfo info)
       {
           var level = LocalState.currentUser.games.Find(g => g.id == ClientManager.gameId);
            if (info.entry.type == CurrencyType.USD && (level == null || level.level < 2))
            {
                OpenSimpleErrorWindow("Need 2 game level!");
                return;
            }
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/SearchingOpponentsWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<SearchingOpponentsWindow>();
            window.InitBattleInfo(info);
            window.SetCurrentUser(LocalState.currentUser);
            window.onPlayGameClick += ServerCommand.TryPlayGame;
            CheckStack(window);
            window.Show();
        }

        public static void OpenGame(BattleInfo info)
        {
            _windowStack.Clear();
            return;
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/GameScreen"),
                    GamlaResourceManager.windowsContainer).GetComponent<GameScreen>();
            //window.onGameFinish += ServerCommand.GameFinish;
            window.Init(info);
            CheckStack(window);
            window.Show();
        }

        public static void OpenGameFinish(HistoryBattleInfo info)
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/FinishWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<FinishWindow>();
            CheckStack(window);
            window.Init(info, LocalState.currentUser);
            window.onPlayAgain += () =>
            {
                OpenSimpleWarningWindow(GUIWarningType.RematchRequest, () => {}, () =>
                {
                    ServerCommand.CreateMatchRequest(info.battleInfo.entry.amount, info.battleInfo.entry.type.ToString(), info.opponent.id);
                    OpenNotification(new ServerNotification()
                    {
                        id = -1,
                        notification_id = -1,
                        short_text = "Rematch request sended"
                    });
                    window.ClosePublic();
                });
            };
            window.onPlay += () =>
            {
                LocalState.currentMatch = new ServerMatchStart()
                {
                    match = new ServerMatchInfo()
                    {
                        id = int.Parse(info.matchId)
                    }
                };
                LocalState.currentTournament = null;
                Clear();
                GamlaService.OnMatchStarted.Push(info.matchId + "", "", false);
            };
            window.onClosed += (_) =>
            {
                DestroyWindowBefore("GameScreen");
            };
        }
        
        static void OpenChooseLangWindow()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ChooseLangWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<ChooseLangWindow>();
            CheckStack(window);
            window.Show();
        }
        
        static void OpenAccountBalanceWindow()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/AccountBalanceWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<AccountBalanceWindow>();
            window.onWithdraClick += OpenWithdrawWindow;
            CheckStack(window);
            window.Init(LocalState.currentUser);
        }

        static void OpenWithdrawWindow()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/WithdrawWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<WithdrawWindow>();
            window.onAddWithdraClick += OpenAddWithdrawWindow;
            window.onWithdrawClick += ServerCommand.Withdraw;
            CheckStack(window);
            window.Show();
        }

        static void OpenAddWithdrawWindow()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/AddWithdrawWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<AddWithdrawWindow>();
            CheckStack(window);
            window.Show();
        }

        static void OpenInfoPanelWindow()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/InfoPanelWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<InfoPanelWindow>();
            window.onContactUsClick += () => { Application.OpenURL("https://gamla.io"); };// OpenHelpWindow;
            window.onResetTutorialClick += ResetTutorial;
            window.onFAQClick += () => { Application.OpenURL("https://gamla.io/faq"); };//OpenHelpWindow;
            window.onLegalNoteClick += () => { Application.OpenURL("https://gamla.io/privacy-policy"); };//OpenHelpWindow;
            CheckStack(window);
            window.Show();
        }
        
        static void OpenHelpWindow()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/HelpWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<HelpWindow>();
            CheckStack(window);
            window.Show();
        }

        static void ResetTutorial()
        {
            GamlaResourceManager.tabBar.SelectPlay();
            TutorialManager.Instance.ResetTutorial();
        }
        
        public static void OpenSimpleInfoWindow(GUIInfoType info, Action onClose = null)
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/InfoWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<InfoWindow>();
            window.onClosed += (_) => onClose?.Invoke();
            window.Init(info);
            CheckStack(window);
            window.Show();
        }
        
        public static void OpenSimpleInfoWindow(GUIInfoWinData info, Action onClose = null)
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/InfoWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<InfoWindow>();
            window.onClosed += (_) => onClose?.Invoke();
            window.Init(info);
            CheckStack(window);
            window.Show();
        }
        
        public static void OpenSimpleWarningWindow(GUIWarningType info, Action onClose = null, Action onAction = null)
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/WarningWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<WarningWindow>();
            window.onActionClick += () => onAction?.Invoke();
            window.onClosed += (_) => onClose?.Invoke();
            window.Init(info, onAction != null);
            CheckStack(window);
            window.Show();
        }
        
        public static void OpenSimpleErrorWindow(string error, Action onClose = null)
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/WarningWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<WarningWindow>();
            window.onClosed += (_) => onClose?.Invoke();
            window.Init(new GUIInfoWinData()
            {
                actionTitle = "",
                closeTitle = "Close",
                description = error,
                title = "ERROR"
            });
            CheckStack(window);
            window.Show();
        }
        
        public static void OpenPopupWindow(string title, string description, string closeTitle, string code, Action onClose = null)
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ReferralWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<WarningWindow>();
            window.onClosed += (_) => onClose?.Invoke();
            window.Init(new GUIInfoWinData()
            {
                actionTitle = "",
                closeTitle = closeTitle,
                description = description,
                title = title,
                logo = "smile_state_1"
            }, code);
            CheckStack(window);
            window.Show();
        }
        
        static void OpenPromoCodeWindow()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/PromocodeWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<PromocodeWindow>();
            CheckStack(window);
            window.onPromoCodeClick += (code) =>
            {
                if(code.Length == 6)
                    ServerCommand.AttachReferral(code);
                else
                    ServerCommand.CheckPromoCode(code);
            };
            window.Show();
        }

        public static ValidateWindow OpenValidateWindow()
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ValidateWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<ValidateWindow>();
            CheckStack(window);
            window.Show();
            return window;
        }
        
        public static void OpenRewardWindow(Pack pack)
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/RewardWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<RewardWindow>();
            CheckStack(window);
            var bonusCount = pack.bonus?.amount ?? 0;
            window.Init((pack.main.amount + bonusCount), pack.main.type);
            window.Show();
        }
        
        public static void OpenRewardWindow(List<Currency> rewards)
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/RewardWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<RewardWindow>();
            CheckStack(window);
            
            var bonusCount = rewards.Count > 0 ? rewards[0].amount : 0;
            window.Init(bonusCount, rewards.Count > 0 ? rewards[0].type : CurrencyType.Z);
            window.Show();
        }

        static void OpenPublicProfile(long id)
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/PublicProfileWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<PublicProfileWindow>();
            CheckStack(window);
            window.Init(id);
            window.invite += ServerCommand.FriendsAdd;
            window.sendMessage += ServerCommand.GetOrCreateFriendChat;
            window.sendMessage += l =>
            {
                CloseExtraWindows();
            };
            window.Show();
        }

        public static void OpenPreStarReward(List<HistoryBattleInfo> winBattles)
        {
            var window =
                GameObject.Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/HoldFinishWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<HoldFinishWindow>();
            CheckStack(window);
            window.Init(winBattles);
            window.Show();
        }

        public static void Clear()
        {
            while (_windowStack.Count > 0)
            {
                var win = _windowStack.Peek();
                win.Hide();
                DestroyCurrentWindow();
            }
        }
    }
}
