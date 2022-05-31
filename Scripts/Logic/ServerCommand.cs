using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamla.Data;
using Gamla.UI;
using UnityEngine;

namespace Gamla.Logic
{
    public static class ServerCommand
    {
        private static bool _isMatchUpdate = false;
        public static void SignUpAsGuest()
        {
            UIMapController.OpenLoading();
            var nick = Utils.GenerateRandomStr();
            var email = nick + "@guest.io";
            var pass = Utils.GenerateRandomStr();

            if (!PlayerPrefs.HasKey("email"))
            {
                SignUp(nick, email, pass);
                return;
            }
            
            string data = JsonUtility.ToJson(new ServerLoginModel()
            {
                game_id = ClientManager.gameId,
                email = email,
                password = pass,
                login_type = "guest",
                push_token = new ServerLoginPush()
                {
                    device_id = SystemInfo.deviceUniqueIdentifier,
                    token = LocalState.pushToken
                },
                version_sdk = ClientManager.version,
                version = Application.version,
                geo = LocationManager.GetCashedLocation()
            });

            ClientManager.InvokeEvent<TokenClear>("login", data,
                tokenClear =>
                {
                    HomeThreadHelper.homeThread.ExecuteCoroutine(new WaitForSecondsRealtime(0.5f), () =>
                    {
                        LoginProfile(tokenClear.token);
                    });
                    PlayerPrefs.SetString("email", email);
                    PlayerPrefs.SetString("password", pass);
                },
                e =>
                {
                    UIMapController.OpenSignUp();
                    UIMapController.OpenSimpleErrorWindow(e.message);
                });
        }

        private static string GameIDJson => $"?game_id={ClientManager.gameId}";
        
        public static void SendLogin(string email, string password, bool isNew)
        {
            var spinner = UIMapController.OpenValidateWindow(false);
            string data = JsonUtility.ToJson(new ServerLoginModel()
            {
                game_id = ClientManager.gameId,
                email = email,
                password = password,
                push_token = new ServerLoginPush()
                {
                    device_id = SystemInfo.deviceUniqueIdentifier,
                    token = LocalState.pushToken
                },
                version_sdk = ClientManager.version,
                version = Application.version,
                geo = LocationManager.GetCashedLocation()
            });

            ClientManager.InvokeEvent<TokenClear>("login", data,
                token =>
                {
                    PlayerPrefs.SetString("email", email);
                    PlayerPrefs.SetString("password", password);
                    LoginProfile(token.token);
                }, 
                e =>
            {

                spinner.CloseInvoke();
                PlayerPrefs.DeleteKey("email");
                PlayerPrefs.DeleteKey("password");
                UIMapController.OpenSimpleErrorWindow(e.message);
                UIMapController.OpenLogin();
            });
        }

        public static void SignUp(string name, string email, string password)
        {
            var spinner = UIMapController.OpenValidateWindow(false); 
            string data = JsonUtility.ToJson(new ServerSignUpModel()
            {
                game_id = ClientManager.gameId,
                //nickname = name,
                email = email,
                password = password,
                password_confirmation = password,
                push_token = new ServerLoginPush()
                {
                    device_id = SystemInfo.deviceUniqueIdentifier,
                    token = LocalState.pushToken
                }
            });

            ClientManager.InvokeEvent<TokenClear>("register", data, token =>
            {
                LoginProfile(token.token);
                PlayerPrefs.SetString("email", email);
                PlayerPrefs.SetString("password", password);
            }, e =>
            {
                UIMapController.OpenSignUp();
                PlayerPrefs.DeleteKey("email");
                PlayerPrefs.DeleteKey("password");
                UIMapController.OpenSimpleErrorWindow(e.message);
                spinner.ClosePublic();
            });
        }

        public static void RegisterGuest(string name, string email, string password)
        {
            ChangeEmail(email, false);
            ResetPassword(password, password);
        }
        
        public static void LoginProfile(string token)
        {
            LocalState.token = token;
            LocalState.currentGame = new GameInfo();
            GetGameAppList();
            ClientManager.GetData<ServerProfile>(token, "profile", profile =>
            {
                PlayerPrefs.SetString("email", profile.email);
                LocalState.currentUser = new UserInfo(profile);
                EventManager.OnProfileUpdate.Push();
                GetOrUpdateFullMatches();
                GetOrUpdateMatches(result =>
                {
                    UIMapController.CloseSpinner();
                    if(!IsHaveBattleInPast())
                        GamlaResourceManager.tabBar.SelectPlay();

                });
                GetOrUpdateFriends(token);
            }, e =>
            {
                UIMapController.CloseSpinner();
                UIMapController.OpenSignUp();
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
            GetMyGame();
            GetShop();
        }

        public static void GetOrUpdateProfile(string token)
        {
            LocalState.token = token;
            ClientManager.GetData<ServerProfile>(token, "profile", profile =>
            {
                LocalState.currentUser = new UserInfo(profile);
                EventManager.OnProfileUpdate.Push();
                GetOrUpdateMatches();
                GetOrUpdateFriends(token);
                GetTournaments(null);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }

        private static bool IsHaveBattleInPast()
        {
            var battles = JsonUtility.FromJson<SaveBattleModel>(PlayerPrefs.GetString("battle_saves", String.Empty));
            if (battles == null)
                return false;
            Debug.Log("IsHaveBattleInPast: " + battles.battles.Count);
            
            bool result = false;
            List<HistoryBattleInfo> winBattles = new List<HistoryBattleInfo>();
            foreach (var battle in battles.battles)
            {
                var history = LocalState.battleHistoryList.Find(b => b.matchId == battle);
                if (history != null && history.status == BattleStatus.Win)
                {
                    winBattles.Add(history);
                    result = true;
                }
            }
            UIMapController.OpenPreStarReward(winBattles);
            return result;
        }

        public static void GetOrUpdateFullMatches()
        {
            if(_isMatchUpdate)
                return;
            ClientManager.GetData<ServerMatches>(LocalState.token, "matches" + GameIDJson, matches =>
            {
                LocalState.battleHistoryList = HistoryBattleInfo.Convert(matches);
                _isMatchUpdate = false;
                if(matches.matches.last_page > 1)
                    LoadNextMatchesPage(++matches.matches.current_page);
            }, e =>
            {
                _isMatchUpdate = false;
            });
        }

        public static void LoadNextMatchesPage(long page)
        {
            if(_isMatchUpdate)
                return;
            ClientManager.GetData<ServerMatches>(LocalState.token, $"matches{GameIDJson}&page={page}", matches =>
            {
                var _matches = HistoryBattleInfo.Convert(matches);
                foreach (var vMatch in _matches)
                {
                    var historyMatch = LocalState.battleHistoryList.Find(m => m.matchId == vMatch.matchId);
                    if (historyMatch == null)
                    {
                        LocalState.battleHistoryList.Add(vMatch);
                    }
                    else
                    {
                        historyMatch.Update(vMatch);
                    }
                }

                _isMatchUpdate = false;
                
                if(matches.matches.last_page < page)
                    LoadNextMatchesPage(++page);
            }, e =>
            {
                _isMatchUpdate = false;
            });
        }
        
        public static void GetOrUpdateMatches(Action<bool> callback = null)
        {
            if(_isMatchUpdate)
                return;
            
            ClientManager.GetData<ServerMatches>(LocalState.token, "matches" + GameIDJson, matches =>
            {
                var historyBattleInfos = HistoryBattleInfo.Convert(matches);
                foreach (var vMatch in historyBattleInfos)
                {
                    var historyMatch = LocalState.battleHistoryList.Find(m => m.matchId == vMatch.matchId);
                    if (historyMatch == null)
                    {
                        LocalState.battleHistoryList.Add(vMatch);
                    }
                    else
                    {
                        historyMatch.Update(vMatch);
                    }
                }
                if (callback == null)
                {
                    LocalState.saveBattleModel.battles.Clear();
                    foreach (var battle in LocalState.battleHistoryList)
                    {
                        if (battle.status == BattleStatus.Searching || battle.status == BattleStatus.Waiting)
                            LocalState.saveBattleModel.battles.Add(battle.matchId);
                    }
                    Debug.Log("SaveBattleInPast: " + LocalState.saveBattleModel.battles.Count);
                    string battleSaves = JsonUtility.ToJson(LocalState.saveBattleModel);
                    PlayerPrefs.SetString("battle_saves", battleSaves);
                    PlayerPrefs.Save();
                }
                GetOrUpdateLeagues(callback);

                if (!string.IsNullOrEmpty(ClientManager.GetTempMatch()))
                {
                    ClientManager.SaveMatchScore(ClientManager.GetTempMatch());
                    ClientManager.ClearTempMatch();
                }

                var matchScore = ClientManager.GetMatchScore();
                foreach (var data in matchScore)
                {
                    SendScore(data);
                }
                _isMatchUpdate = false;
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
                _isMatchUpdate = false;
                callback?.Invoke(false);
            });
            _isMatchUpdate = true;
            GetTournaments(null);
        }

        public static void GetOrUpdateLeagues(Action<bool> callback = null)
        {
            //EventManager.OnGameInfoUpdate.Push();
            ClientManager.GetData<ServerLeagues>(LocalState.token, "leagues" + GameIDJson, leagues =>
            {
                LocalState.currentGame.leagues = leagues;
                LocalState.currentGame.ladder = LadderInfo.Convert(leagues);
                EventManager.OnGameInfoUpdate.Push();
                callback?.Invoke(true);
                _isMatchUpdate = false;
            }, e =>
            {
                EventManager.OnGameInfoUpdate.Push();
                //UIMapController.OpenSimpleErrorWindow(e.message);
                _isMatchUpdate = false;
                callback?.Invoke(false);
            });
        }

        public static void GetMatchInfoAndOpenView(long matchId, bool repeat = true)
        {
            ClientManager.GetData<ServerMatchSingleModel>(LocalState.token, "matches/" + matchId, data =>
                {
                    if (repeat && (data.match.players.Count < 2 ||
                                    string.IsNullOrEmpty(data.match.players[0].score) ||
                                    string.IsNullOrEmpty(data.match.players[1].score)))
                    {
                        GetMatchInfoAndOpenView(matchId, false);
                    } else {
                        UIMapController.OpenGameFinish(HistoryBattleInfo.Convert(data.match));
                    }
                },
                e =>
                {
                    if (repeat) {
                        GetMatchInfoAndOpenView(matchId, false);
                    } else {
                        UIMapController.OpenSimpleErrorWindow(e.message);
                        _isMatchUpdate = false;
                    }
                });
        }

        public static void GetOrUpdateFriends(string token)
        {
            ClientManager.GetData<ServerFriends>(token, "friends", friends =>
            {
                LocalState.friends = friends;
                EventManager.OnFriendsUpdated.Push();
                ClientManager.GetData<ServerChatPages>(token, "chats", result =>
                {
                    LocalState.chats = result;

//                EventManager.OnGameInfoUpdate.Push();
                }, e =>
                {
                    UIMapController.OpenSimpleErrorWindow(e.message);
                });
                
//                EventManager.OnGameInfoUpdate.Push();
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }

        public static void GetPossibleMatchesBet(Action<List<BattleInfo>> callback)
        {
            ClientManager.GetData<ServerMatchBet>(LocalState.token, "matches/get/parameters", result =>
            {
                List<BattleInfo> realBattles = new List<BattleInfo>();
                if (result.currencies.Contains(CurrencyType.USD.ToString()))
                {
                    foreach (var b in result.bets)
                    {
                        realBattles.Add(new BattleInfo()
                        {
                            entry = new Currency(){amount = b, type = CurrencyType.USD},
                            exp = 1,
                            max_gamers = 2,
                            score = 2,
                            trophie = 1,
                            type = BattleType.HardPVP,
                            win = new Currency(){amount = (b * 2f) * 0.8f, type = CurrencyType.USD},
                        });
                    }
                }
                
                if (result.currencies.Contains(CurrencyType.Z.ToString()))
                {
                    foreach (var b in result.bets)
                    {
                        realBattles.Add(new BattleInfo()
                        {
                            entry = new Currency(){amount = b, type = CurrencyType.Z},
                            exp = 1,
                            max_gamers = 2,
                            score = 2,
                            trophie = 1,
                            type = BattleType.SoftPVP,
                            win = new Currency(){amount = (b * 2f), type = CurrencyType.Z},
                        });
                    }
                }

                LocalState.newBattleMatches = realBattles;
                callback.Invoke(realBattles);
            }, e => {});
        }
        
        public static void GetGameAppList()
        {
            ClientManager.GetData<GameAppListResult>(LocalState.token, "games", result =>
            {
                LocalState.gameApplist = result;
            }, e => {});
        }

        public static void GetGameInfo(string gameId, Action<GameAppInfo> callback)
        {
            ClientManager.GetData<GameAppResult>(LocalState.token, "games/" + gameId, result =>
            {
                callback?.Invoke(result.game);
            }, e =>
            {
                callback?.Invoke(null);
            });
        }

        public static void GetMyGame()
        {
            ClientManager.GetData<GameAppResult>(LocalState.token, "games/" + ClientManager.gameId, result =>
            {
                LocalState.gameAppInfo = result.game;
                EventManager.OnGameInfoUpdate.Push();
            }, e => {});
        }

        public static void LogOut()
        {
            ClientManager.InvokeEvent<EmptyModel>("logout", "", token =>
            {
                //UIMapController.OpenSignUp();
                LocalState.currentUser = null;

            }, e =>
            {
                //UIMapController.OpenSignUp();
                //UIMapController.OpenSimpleErrorWindow(e.message);
            });
            
            //UIMapController.OpenSignUp();
            UIMapController.OpenSignUp();
            PlayerPrefs.DeleteKey("email");
            PlayerPrefs.DeleteKey("password");
            PlayerPrefs.DeleteKey("battle_saves");
            LocalState.ClearState();
        }
        
        [Serializable]
        public class ResetPublishNameModel
        {
            public string nickname;
        }
        public static void ResetPublishName(string name, string fake)
        {
            string data = JsonUtility.ToJson(new ResetPublishNameModel()
            {
                nickname = name
            });

            ClientManager.PutData<EmptyModel>("profile", LocalState.token, data, result =>
            {
                GetOrUpdateProfile(LocalState.token);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }

        [Serializable]
        public class ResetNameModel
        {
            public string name;
            public string surname;
        }
        public static void ResetName(string firstName, string lastName)
        {
            string data = JsonUtility.ToJson(new ResetNameModel()
            {
                name = firstName,
                surname = lastName
            });

            ClientManager.PutData<EmptyModel>("profile", LocalState.token, data, result =>
            {
                GetOrUpdateProfile(LocalState.token);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }

        [Serializable]
        public class ResetAddressModel
        {
            public string address;
        }
        public static void ResetAddress(string address)
        {
            string data = JsonUtility.ToJson(new ResetAddressModel()
            {
                address = address
            });

            ClientManager.PutData<EmptyModel>("profile", LocalState.token, data, result =>
            {
                GetOrUpdateProfile(LocalState.token);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }

        [Serializable]
        public class ResetPhoneModel
        {
            public string phone;
        }
        public static void ResetPhone(string phone, string fake)
        {
            string data = JsonUtility.ToJson(new ResetPhoneModel()
            {
                phone = phone
            });
            
            ClientManager.PutData<EmptyModel>("profile", LocalState.token, data, result =>
            {
                GetOrUpdateProfile(LocalState.token);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }
        
        [Serializable]
        public class ResetBirthdayModel
        {
            public string birthday;
        }
        public static void SetBirthday(string birthday)
        {
            string data = JsonUtility.ToJson(new ResetBirthdayModel()
            {
                birthday = birthday
            });
            
            Debug.Log($"birthday {birthday}");
            ClientManager.PutData<EmptyModel>("profile", LocalState.token, data, result =>
            {
                GetOrUpdateProfile(LocalState.token);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }
        
        [Serializable]
        public class ResetNotificationModel
        {
            public int notify;
        }
        public static void ResetNotification(bool state)
        {
            string data = JsonUtility.ToJson(new ResetNotificationModel()
            {
                notify = state ? 1 : 0
            });
            
            ClientManager.PutData<EmptyModel>("profile", LocalState.token, data, result =>
            {
                GetOrUpdateProfile(LocalState.token);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }

[Serializable]
        public class ResetPasswordModel
        {
            public string password;
            public string new_password;
            public string new_confirm_password;
        }
        public static void ResetPassword(string password, string fake)
        {
            string data = JsonUtility.ToJson(new ResetPasswordModel()
            {
                password = PlayerPrefs.GetString("password"),
                new_password = password,
                new_confirm_password = password
            });
            ClientManager.PutData<EmptyModel>("profile", LocalState.token, data, result =>
            {
                PlayerPrefs.SetString("password", password);
                UIMapController.OpenSimpleInfoWindow(GUIInfoType.ResetPassSuccess);
                GetOrUpdateProfile(LocalState.token);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }
        
        [Serializable]
        public class ResetAvatarModel
        {
            public string image;
        }
        public static void ResetAvatar(string fileData)
        {
            string data = JsonUtility.ToJson(new ResetAvatarModel()
            {
                image = "data:image/jpeg;base64," + fileData
            });
            
            ClientManager.InvokeEvent<EmptyModel>(LocalState.token, "profile", data, result =>
            {
                GetOrUpdateProfile(LocalState.token);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }

        public static void CheckCode(string code)
        {
            //TODO: send to server
            UIMapController.OpenSimpleInfoWindow(GUIInfoType.ConfirmCodeSuccess);
        }

        public static void SendResetLink(string email, string phone, bool resetPassword)
        {
            // TODO: send to server
            UIMapController.OpenResetPassword();
            UIMapController.OpenSimpleInfoWindow(GUIInfoType.SendResetLinkSuccess);
        }

        [Serializable]
        public class ResetEmailModel
        {
            public string email;
        }

        public static void ChangeEmail(string email)
        { 
            ChangeEmail(email, true);
        }
        public static void ChangeEmail(string email, bool isShow)
        {
            string data = JsonUtility.ToJson(new ResetEmailModel()
            {
                email = email
            });
            ClientManager.PutData<EmptyModel>("profile", LocalState.token, data, result =>
            {
                PlayerPrefs.SetString("email", email);
                if(isShow)
                    UIMapController.OpenSimpleInfoWindow(GUIInfoType.ChangePersonInfoSuccess);
                GetOrUpdateProfile(LocalState.token);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }
        
        
        [Serializable]
        public class PromocodeModel
        {
            public string promocode;
        }
        
        [Serializable]
        public class PromoResulModel
        {
            public ServerPaymentTransactionModel payment;
        }

        public static void CheckPromoCode(string code)
        {
            string data = JsonUtility.ToJson(new PromocodeModel()
            {
                promocode = code
            });
            
            ClientManager.PutData<PromoResulModel>("promocodes/use", LocalState.token, data, result =>
            {
                UIMapController.OpenSimpleInfoWindow(new GUIInfoWinData {
                    title = "Success",
                    description = result.payment.comment,
                    closeTitle = "OK" });
                GetOrUpdateProfile(LocalState.token);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }

        public static void CheckGeoLocation(Action<bool> result)
        {
            LocationManager.FindLocation(((b, info) =>
            {
                if (!b)
                {
                    UIMapController.OpenSimpleWarningWindow(GUIWarningType.CheckRegion, () =>
                        {
                            result?.Invoke(false);
                        },
                        () =>
                        {
#if UNITY_IPHONE
                            Application.OpenURL(NativeBindings.GetSettingsURL());
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
                            using var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                            using AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                            string packageName = currentActivityObject.Call<string>("getPackageName");
                            using var uriClass = new AndroidJavaClass("android.net.Uri");
                            using AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null);
                            using var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject);
                            intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                            intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                            currentActivityObject.Call("startActivity", intentObject);
#endif
                        });
                }
                else
                {
                    SendCheckGeo(info, result);
                }
            }));

        }

        [Serializable]
        public class LocationModel
        {
            public float latitude;
            public float longitude;
            public float altitude;
            public float horizontalAccuracy;
            public float verticalAccuracy;
            public double timestamp;

            public LocationModel(){}
            public LocationModel(LocationInfo info)
            {
                latitude = info.latitude;
                longitude = info.longitude;
                altitude = info.altitude;
                horizontalAccuracy = info.horizontalAccuracy;
                verticalAccuracy = info.verticalAccuracy;
                timestamp = info.timestamp;
            }
        }
        
        [Serializable]
        public class LocationSendModel
        {
            public LocationModel geo;

            public LocationSendModel(LocationModel data)
            {
                geo = data;
            }
        }
        public static void SendCheckGeo(LocationInfo info, Action<bool> callback)
        {
            string data = JsonUtility.ToJson(new LocationSendModel(new LocationModel(info)));
            Debug.Log("SendCheckGeo" + data);
            ClientManager.InvokeEvent<GeoResult>(LocalState.token, "geo", data, result =>
            {
                //callback?.Invoke(true);
                callback?.Invoke(result is {result: true});
            }, e =>
            {
                callback?.Invoke(false);
            });
        }

        public static void AddPack(Pack pack, ValidateWindow window)
        {
            CheckGeoLocation(b =>
            {
                if (b)
                {
                    string data = JsonUtility.ToJson(new PayPalLink()
                    {
                        game_id = ClientManager.gameId + "",
                        amount = pack.main.amount
                    });
                    
                    ClientManager.InvokeEvent<PayPalLinkResult>(LocalState.token, "payments/deposit", data, match =>
                    {
                        Application.OpenURL(match.payment_link);
                        window.CloseInvoke();
                    }, e =>
                    {
                        window.CloseInvoke();
                        UIMapController.OpenSimpleErrorWindow(e.message + " " + e.error);
                    });
                }
                else
                {
                    window.CloseInvoke();
                    UIMapController.OpenSimpleWarningWindow(GUIWarningType.UnavalibleRegion);
                }
            });
        }

        public static void Withdraw(string platform, float value, string target)
        {
            string data = JsonUtility.ToJson(new WithdrawModel()
            {
                amount = value,
                payment_system = platform,
                account_number = target
            });
                    
            ClientManager.InvokeEvent<EmptyModel>(LocalState.token, "payments/withdrawal/add", data, match =>
            {
                GamlaResourceManager.tabBar.SelectPlay();
                UIMapController.OpenSimpleInfoWindow(GUIInfoType.TransferAmountSuccess);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message + " " + e.error);
            });
        }

        public static void CreateMatch(BattleInfo info, Action<ServerMatchStart> callback)
        {
            string data = JsonUtility.ToJson(new ServerStartMatchInfo()
            {
                game_id = ClientManager.gameId + "",
                bet = (int)info.entry.amount + "",
                currency = info.entry.type.ToString()
            });
            
            ClientManager.InvokeEvent<ServerMatchStartContainer>(LocalState.token, "matches/add", data, match =>
            {
                LocalState.currentMatch = match.newUserMatch;
                LocalState.currentTournament = null;
                //GamlaService.OnMatchStarted.Push(LocalState.currentMatch.match.id + "", "", false);
                //UIMapController.Clear();
                callback?.Invoke(match.newUserMatch);
            }, e =>
            {
                callback?.Invoke(null);
                UIMapController.Clear();
                GamlaResourceManager.tabBar.SelectPlay();
                UIMapController.OpenSimpleErrorWindow(e.message + " " + e.error);
            });
        }

        public static void TryPlayGame(BattleInfo info)
        {
            // string data = JsonUtility.ToJson(new ServerStartMatchInfo()
            // {
            //     game_id = ClientManager.gameId + "",
            //     bet = (int)info.entry.amount + "",
            //     currency = info.entry.type.ToString()
            // });
            //
            // ClientManager.InvokeEvent<ServerMatchStartContainer>(LocalState.token, "matches/add", data, match =>
            // {
            //     LocalState.currentMatch = match.newUserMatch;
            //     LocalState.currentTournament = null;
            //     GamlaService.OnMatchStarted.Push(LocalState.currentMatch.match.id + "", "", false);
            //     //UIMapController.Clear();
            // }, e =>
            // {
            //     UIMapController.Clear();
            //     GamlaResourceManager.tabBar.SelectPlay();
            //     UIMapController.OpenSimpleErrorWindow(e.message + " " + e.error);
            // });
            
            // LocalState.currentMatch = match.newUserMatch;
            // LocalState.currentTournament = null;
            if(LocalState.currentMatch != null)
                GamlaService.OnMatchStarted.Push(LocalState.currentMatch.match.id + "", "", false);
        }

        public static void TryPlayTournament(long matchId, ServerTournamentModel tournament)
        {
            LocalState.currentMatch = null;
            LocalState.currentTournament = tournament;
            GamlaService.OnMatchStarted.Push(matchId + "", "", false);
            //UIMapController.Clear();
            
//            ClientManager.InvokeEvent<ServerMatchStartContainer>(LocalState.token, "matches/add", data, match =>
//            {
//                LocalState.currentMatch = null;
//                LocalState.currentTournament = tournament;
//                GamlaService.OnMatchStarted.Push(LocalState.currentMatch.match.id + "", "", false);
//                UIMapController.Clear();
//            }, e =>
//            {
//                UIMapController.OpenSimpleErrorWindow(e.message);
//            });
        }

        public static void GameFinish(int score)
        {
            long matchId = LocalState.currentMatch != null ? LocalState.currentMatch.match.id : 0;
            string data = JsonUtility.ToJson(new ServerMatchResult()
            {
                match_id = matchId,
                score = score
            });

            var match = LocalState.battleHistoryList.Find(info => info.matchId == (matchId + ""));
            if (match != null)
            {
                match.me.score = score + "";
            }

            ClientManager.SaveMatchScore(data);
            
            ClientManager.InvokeEvent<EmptyModel>(LocalState.token, "matches/play", data, matches =>
            {
                ClientManager.RemoveMatchScore(data);
                ClientManager.UpdateMatchesLazy();
            }, e =>
            {
//                GameResourceManager.tabBar.SelectPlay();
                 UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }

        public static void SendScore(string data)
        {
            ClientManager.InvokeEvent<EmptyModel>(LocalState.token, "matches/play", data, matches =>
            {
                ClientManager.RemoveMatchScore(data);
                GetOrUpdateMatches();
            }, e =>
            {
                if (e.code == 500)
                {
                    ClientManager.RemoveMatchScore(data);
                }
                else
                {
                    UIMapController.OpenSimpleErrorWindow(e.message);
                }
            });
        }
        
        [Serializable]
        public class ServerTournamentResult
        {
            public long tournament_id;
            public long score;
        }
        
        public static void GameFinishTournament(int score)
        {
            string data = JsonUtility.ToJson(new ServerTournamentResult()
            {
                tournament_id = LocalState.currentTournament != null ? LocalState.currentTournament.id : 0,
                score = score
            });
            
            ClientManager.InvokeEvent<ServerMatchInfo>(LocalState.token, "tournaments/play", data, matches =>
            {

            }, e =>
            {
                Debug.LogError($"Error on finish turnir game: {e.message}");
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
            
        }

        [Serializable]
        class FriendsAddModel
        {
            public long friend_id;
        }
        public static void FriendsAdd(long friendId)
        {
            string data = JsonUtility.ToJson(new FriendsAddModel()
            {
                friend_id = friendId
            });
            
            ClientManager.InvokeEvent<EmptyModel>(LocalState.token, "friends/add", data, result =>
            {
                GetOrUpdateFriends(LocalState.token);
            }, e =>
            {

            });
        }

        [Serializable]
        class FriendsRequestModel
        {
            public long friend_id;
            public string status; // ['accepted', 'rejected']
        }
        public static void FriendsRequestAccept(long friendId)
        {
            string data = JsonUtility.ToJson(new FriendsRequestModel()
            {
                friend_id = friendId,
                status = "accepted"
            });
            ClientManager.PutData<EmptyModel>("friends/update", LocalState.token, data, result =>
            {
                CreateChat(friendId);
                GetOrUpdateFriends(LocalState.token);
            }, e =>
            {

            });
        }
        
        public static void FriendsRequestReject(long friendId)
        {
            string data = JsonUtility.ToJson(new FriendsRequestModel()
            {
                friend_id = friendId,
                status = "rejected"
            });
            ClientManager.PutData<EmptyModel>("friends/update", LocalState.token, data, result =>
            {
                GetOrUpdateFriends(LocalState.token);
            }, e =>
            {

            });
        }

        public static void CreateChat(long friendId, bool isNeedOpenUI = false)
        {
            string data = JsonUtility.ToJson(new FriendsAddModel()
            {
                friend_id = friendId
            });
            
            ClientManager.InvokeEvent<ServerNextChatModel>(LocalState.token, "chats/create", data, result =>
            {
                GetOrUpdateFriends(LocalState.token);
                UIMapController.OpenChat(result.chat.id, new List<ServerChatMessage>());
            }, e =>
            {

            });
        }

        public static void GetOrCreateFriendChat(long friendId)
        {
            if (LocalState.chats == null) return;
            
            var chat = (from chats in LocalState.chats.chats.data
                from member in chats.members
                where member.user_id == friendId
                select chats).FirstOrDefault();
            if (chat != null)
            {
                ClientManager.GetData<ServerChatMessageModel>(LocalState.token, "chats/messages/" + chat.id,
                    result =>
                    {
                        UIMapController.OpenChat(chat.id, result.messages.data);
                        EventManager.OnFriendChatLoad.Push(chat.id);
                    }, e => { });
            }
            else
            {
                CreateChat(friendId, true);
                EventManager.OnFriendChatLoad.Push(0);
            }
        }


        public static void GetUserPublic(long userId, Action<ServerPublicUser> callback)
        {
            ClientManager.GetData<ServerPublicUser>(LocalState.token, "users/" + userId, callback, e => { });
        }
        
        public static void GetPublicProfile(long userId, Action<ServerPublicProfile> callback)
        {
            ClientManager.GetData<ServerPublicProfile>(LocalState.token, "users/" + userId,
                callback, e => { });
        }
        
        public static void GetFriendChat(long chatId, Action<List<ServerChatMessage>> messages)
        {
            ClientManager.GetData<ServerChatMessageModel>(LocalState.token, "chats/messages/" + chatId,
                result =>
                {
                    messages.Invoke(result.messages.data);
                    EventManager.OnFriendChatLoad.Push(chatId);
                }, e => { });
        }

        [Serializable]
        class SendFriendChatModel
        {
            public string text;
        }
        public static void SendFriendChat(long chatId, string message)
        {
            string data = JsonUtility.ToJson(new SendFriendChatModel()
            {
                text = message
            });
            
            ClientManager.InvokeEvent<EmptyModel>(LocalState.token, "chats/send/"+chatId, data, result =>
            {
                GetOrUpdateFriends(LocalState.token);
            }, e =>
            {

            });
        }

        public static void GetBalanceTransaction(Action<ServerPaymentTransactionPages> callback)
        {
            ClientManager.GetData<ServerPaymentTransactionPages>(LocalState.token, "payments", result =>
            {
                callback?.Invoke(result);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }
        
        public static void GetTrophies(Action<ServerTrophies> callback)
        {
            ClientManager.GetData<ServerTrophies>(LocalState.token, "trophies", result =>
            {
                callback?.Invoke(result);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }
        
        public static void GetTournaments(Action<ServerTournament> callback)
        {
            ClientManager.GetData<ServerTournament>(LocalState.token, "tournaments" + GameIDJson, result =>
            {
                LocalState.tournaments.Clear();
                var all_tournaments = result.all_tournaments.data.FindAll(t => t.game_id == ClientManager.gameId);// && t.status != "finished" && t.status != "cancelled");
                var user_tournaments = result.user_tournaments.data.FindAll(t => t.game_id == ClientManager.gameId);// && t.status != "finished" && t.status != "cancelled");
                user_tournaments.ForEach(t => t.isMy = true);
                var participates = result.participate_tournaments.data.FindAll(t => t.game_id == ClientManager.gameId);// && t.status != "finished" && t.status != "cancelled");
                participates.ForEach(t => t.isJoined = true);
                
                LocalState.tournaments.AddRange(participates);
                foreach (var tournament in user_tournaments)
                {
                    if (LocalState.tournaments.All(t => t.id != tournament.id))
                    {
                        LocalState.tournaments.Add(tournament);
                    }
                }

                LocalState.tournaments.ForEach(t =>
                {
                    if (user_tournaments.Any(ut => ut.id == t.id))
                    {
                        t.isMy = true;
                    }
                });
                
                foreach (var tournament in all_tournaments)
                {
                    if (LocalState.tournaments.All(t => t.id != tournament.id))
                    {
                        LocalState.tournaments.Add(tournament);
                    }
                }
                
                callback?.Invoke(result);
                EventManager.OnTournamentsUpdated.Push();
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }

        public static void GetNotification(Action<List<ServerNotification>> callback)
        {
            if (!string.IsNullOrEmpty(LocalState.token))
            {
                ClientManager.GetData<List<ServerNotification>>(LocalState.token, "profile/notifications",
                    result => { callback?.Invoke(result); },
                    e => { });
            }
        }
        
        public static void ReadNotification(long id)
        {
            ClientManager.PutData<EmptyModel>( "profile/notification/" + id, LocalState.token, "empty", result => { }, e => {});
        }

        public static void CollectDailyReward(Action<ServerRewardContainer> callback)
        {
            ClientManager.PutData<ServerRewardContainer>("profile/dailyBonus", LocalState.token, "0", callback);
        }
        
        public static void CollectHourReward(Action<ServerRewardContainer> callback)
        {
            ClientManager.PutData<ServerRewardContainer>("profile/hoursBonus", LocalState.token, "0", callback);
        }

        [Serializable]
        public class JoinTournamentModel
        {
            public long tournament_id;
            public string private_code;
        }
        
        public static void JoinTournament(long id)
        {
            string data = JsonUtility.ToJson(new JoinTournamentModel()
            {
                tournament_id = id
            });
            
            ClientManager.InvokeEvent<ServerJoinTournament>(LocalState.token, "tournaments/join", data, result =>
                {
                    PlayTournament(result.match.tournament);
                }, error =>
            {
                UIMapController.OpenSimpleErrorWindow(error.message, UIMapController.OpenGameMain);
            });
        }
        public static void JoinPrivateTournament(string code)
        {
            string[] source = code.Split('p');
            string data = JsonUtility.ToJson(new JoinTournamentModel()
            {
                tournament_id = int.Parse(source[0]),
                private_code = code.Replace(source[0]+"p", "")
            });

            ClientManager.InvokeEvent<ServerJoinTournament>(LocalState.token, "tournaments/joinPrivate", data, result =>
            {
                PlayTournament(result.match.tournament);
            }, error =>
            {
                UIMapController.OpenSimpleErrorWindow(error.message);
            });
        }

        public static void PlayTournament(ServerTournamentModel tournament)
        {
            LocalState.currentMatch = null;
            LocalState.currentTournament = tournament;
            GamlaService.OnMatchStarted.Push(LocalState.currentTournament.id + "", "", true);
            //UIMapController.Clear();
        }

        public static void CreateTournament(ServerCreateTournamentModel model, Action<ServerCreateTournamentResult> callback)
        {
            model.game_id = ClientManager.gameId;
            model.autoplay_minutes = 240;
            model.autoplay_duration = 30;
//            DateTime time = DateTime.Now;
//            model.start_at = time.ToString("s");
//            model.end_at = time.AddDays(2).ToString("s");
//            model.private_code = Utils.GenerateRandomStr();


            string data = JsonUtility.ToJson(model);
            
            ClientManager.InvokeEvent<ServerCreateTournamentResult>(LocalState.token, "tournaments/addPrivate", data, result =>
            {
                result.tournament.isMy = true;
                LocalState.tournaments.Add(result.tournament);
                callback?.Invoke(result);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
                callback?.Invoke(null);
            });
        }

        public static void GetMatchRequests(Action<ServerRequestMatchesModel> callback)
        {
            ClientManager.GetData<ServerRequestMatchesModel>(LocalState.token, "matches/requests", result =>
            {
                callback?.Invoke(result);
            }, e =>
            {
                
            });
        }
        
        [Serializable]
        public class CreateMatchRequestModel
        {
            public long game_id = ClientManager.gameId;
            public float bet;
            public string currency;
            public string to_user;
        }
        
        [Serializable]
        public class MatchRequestModel
        {
            public long match_request_id;
        }

        public static void CreateMatchRequest(float bet, string currency, long toUser)
        {
            string data = JsonUtility.ToJson(new CreateMatchRequestModel()
            {
                bet = bet,
                currency = currency,
                to_user = toUser.ToString()
            });
            
            ClientManager.InvokeEvent<EmptyModel>(LocalState.token, "matches/requests/create", data, result => { }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }
        
        public static void RejectRequest(long requestId)
        {
            string data = JsonUtility.ToJson(new MatchRequestModel()
            {
                match_request_id = requestId
            });
            
            ClientManager.InvokeEvent<EmptyModel>(LocalState.token, "matches/requests/reject", data, result => { }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }

        public static void AcceptRequest(long requestId)
        {
            string data = JsonUtility.ToJson(new MatchRequestModel()
            {
                match_request_id = requestId
            });
            
            ClientManager.InvokeEvent<ServerRequestAccept>(LocalState.token, "matches/requests/accept", data, result =>
            {
                LocalState.currentMatch = new ServerMatchStart()
                {
                    match = new ServerMatchInfo()
                    {
                        id = result.payment.match_id
                    }
                };
                LocalState.currentTournament = null;
                //UIMapController.Clear();
                GamlaService.OnMatchStarted.Push(result.payment.match_id + "", "", false);
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }

        public class ReferralModel
        {
            public string referral_code;
        }

        public static void AttachReferral(string code)
        {
            string data = JsonUtility.ToJson(new ReferralModel()
            {
                referral_code = code
            });
            
            ClientManager.InvokeEvent<EmptyModel>(LocalState.token, "referral/attach", data, result =>
            {
                UIMapController.OpenSimpleInfoWindow(new GUIInfoWinData {
                    title = "Success",
                    description = "Referral code attached!",
                    closeTitle = "OK" });
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }
        
        public static void GetShop()
        {
            ClientManager.GetData<TicketShop>(LocalState.token, "shop/items", callback =>
            {
                LocalState.ticketShop = callback.items;
            }, e =>
            {
                UIMapController.OpenSimpleErrorWindow(e.message);
            });
        }

        public static void BuyItem(TicketShopItemModel item)
        {
            ClientManager.InvokeEvent<EmptyModel>(LocalState.token, $"shop/item/{item.id}/buy", "",
                resultJson =>
                {
                    Debug.Log($"Success buy {item.id}");
                    UIMapController.OpenNotification(new ServerNotification()
                    {
                        id = -1,
                        notification_id = -1,
                        short_text = $"{LocalizationManager.Text("gamla.window.successfulpayment.youbuy")} {item.name}"   
                    });
                    GetOrUpdateProfile(LocalState.token);
                },
                e =>
                {
                    UIMapController.OpenSimpleErrorWindow(e.message);
                });
        }
    }

}