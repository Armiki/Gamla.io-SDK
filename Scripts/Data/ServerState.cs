using System;
using System.Collections.Generic;
using Gamla.Logic;
using JetBrains.Annotations;
using UnityEngine;

namespace Gamla.Data
{
    public class ServerState
    {

    }
    
    
    [Serializable]
    public class ServerLoginModel
    {
        public int game_id;
        public string login_type;
        public string email;
        public string password;
        public ServerLoginPush push_token;
        public string version;
        public string version_sdk;
        public ServerCommand.LocationModel geo;
    }

    [Serializable]
    public class ServerLoginPush
    {
        public string device_id;
        public string token;
    }

    [Serializable]
    public class ResponseModel<T>
    {
        public string status;
        public T body;
        public ErrorModel error;
    }

    [Serializable]
    public class ErrorModel
    {
        public int code;
        public string message;
        public string error;
    }

    [Serializable]
    public class TokenClear
    {
        public string token;
    }

    [Serializable]
    public class EmptyModel
    {
    }

    [SerializeField]
    public class BooleanModel
    {
        public bool value;
        
        public BooleanModel(){}

        public BooleanModel(bool v)
        {
            value = v;
        }
    }

    [Serializable]
    public class ServerSignUpModel
    {
        public int game_id;
        public string nickname;
        public string email;
        public string password;
        public string password_confirmation;
        public ServerLoginPush push_token;
    }

    [Serializable]
    public class ServerPublicProfile
    {
        public long id;
        public string nickname;
        public string image; //url
        public string name;
        public string surname;
        public string flag;
        public long count_all_win;
        public long count_all_lose;
        public long win_in_row;
        public string referral_code;
        public ServerProfileExp experience;
        public List<ServerTrophiesModel> trophies;
    }

    [Serializable]
    public class ServerPublicUser
    {
        public long id;
        public string nickname;
        public string image; //url
        public string name;
        public string surname;
        public long trophie;
        public string score;
        public string flag;
        
        public ServerPublicUser(){}

        public ServerPublicUser(List<ServerPlayerMatch> users, bool isMe)
        {
            if (users == null || users.Count == 0)
            {
                nickname = "wait";
                return;
            }

            foreach (var user in users)
            {
                if (user.user_id != LocalState.currentUser.uid && !isMe)
                {
                    id = user.user_id;
                    nickname = user.nickname;
                    trophie = 0;
                    score = user.score;
                    image = user.image;
                }
                
                if (user.user_id == LocalState.currentUser.uid && isMe)
                {
                    id = user.user_id;
                    nickname = user.nickname;
                    trophie = 0;
                    score = user.score;
                    image = user.image;
                }
            }
        }

        public bool IsEmpty()
        {
            return id == 0 && string.IsNullOrEmpty(nickname);
        }
    }
    
    [Serializable]
    public class ServerMatchUser
    {
        public long id;
        public string nickname;
        public long user_id; //url
        public int score;
        public string image;
    }
    
    [Serializable]
    public class CardHolderModel
    {
        public string platform;
        public string data;
        public string holder;
    }

    [Serializable]
    public class CardHolderList
    {
        public List<CardHolderModel> list = new List<CardHolderModel>();
    }

    [Serializable]
    public class ServerProfileExpModel
    {
        public long game_id;
        public long points;
        public int level;
        public long pointsToNextLevel;
    }

    [Serializable]
    public class ServerProfileExp
    {
        public ServerProfileExpModel gamla;
        public List<ServerProfileExpModel> games;
    }

    [Serializable]
    public class ServerProfileBalances
    {
        public float hard_total;
        public float hard_bonus;
        public float hard;
        public float soft;
        public float bonus;
        public float tickets;
    }

    [Serializable]
    public class ServerProfile
    {
        public long id;
        public string nickname;
        public string image; //url
        public string name;
        public string surname;
        public string email;
        public string address;
        public string flag;
        public int notify;
        public string phone;
        public ServerProfileBalances balances;
        public ServerProfileExp experience;
        public string email_verified_at;
        public string remember_token;
        public string created_at;
        public string updated_at;
        public string daily_bonus;
        public string hours_bonus;
        public long timer_daily_bonus;
        public long timer_hours_bonus;
        public long count_all_win;
        public long count_all_lose;
        public long win_in_row = 0;
        public string referral_code;
        public string referral_link;
        public string birthday;
    }

    [Serializable]
    public class ServerLeague
    {
        public long id;
        public string name;
        public string image;
        public string end_at;
        public long awards_count;
        public long award_usd;
        public long award_tickets;
        public long award_z;
        public List<ServerMedal> gold_medals = new List<ServerMedal>();
        public List<ServerMedal> silver_medals = new List<ServerMedal>();
    }

    [Serializable]
    public class ServerMedal
    {
        public long id;
        public long user_id;
        public int gold_count;
        public int silver_count;
        public ServerPublicUser user_info;
        public ServerAward award;
    }

    [Serializable]
    public class ServerLeagues
    {
        public ServerResultPage<ServerLeague> leagues;
    }

    [Serializable]
    public class ServerPlayerMatch
    {
        public long id;
        public long user_id = -1;
        public long match_id;
        public string score;
        public string nickname = "wait user";
        public string image;
    }

    [Serializable]
    public class ServerMatchInfo
    {
        public long id;
        public long game_id;
        public long winner_id;
        public string status;
        public float bet;
        public int stage = 0;
        public int score;
        public string currency;
        public int players_count;
        public int draw_score;
        public long tournament_id;
        public List<ServerPlayerMatch> players;
        public string updated_at;
        //public ServerTournamentModel tournament;
        public MatchResultModel matchResult;
    }
    
    [Serializable]
    public class ServerMatchTournamentInfo
    {
        public long id;
        public long game_id;
        public long winner_id;
        public string status;
        public float bet;
        public int stage = 0;
        public int score;
        public string currency;
        public int players_count;
        public int draw_score;
        public long tournament_id;
        public List<ServerPlayerMatch> players;
        public string updated_at;
        public ServerTournamentModel tournament;
        public MatchResultModel matchResult;
    }

    [Serializable]
    public class MatchResultModel
    {
        public Currency bonus;
        public Currency reward;
        //public float commission;
        public int medals;
        public MatchResultExperience experience;
    }

    [Serializable]
    public class MatchResultExperience
    {
        public MatchResultExp gamla;
        public MatchResultExp game;
    }

    [Serializable]
    public class MatchResultExp
    {
        public int newPoints;
    }

    [Serializable]
    public class ServerMatches
    {
        public ServerResultPage<ServerMatchInfo> matches;
    }

    [Serializable]
    public class ServerStartMatchInfo
    {
        public long id;
        public string game_id;
        public string bet;
        public string currency;
    }
    
    [Serializable]
    public class PayPalLink
    {
        public string game_id;
        public float amount;
    }

    [Serializable]
    public class WithdrawModel
    {
        public float amount;
        public string payment_system;
        public string account_number;
    }
    
    [Serializable]
    public class PayPalLinkResult
    {
        public string payment_link;
    }

    [Serializable]
    public class GeoResult
    {
        public bool result = false;
    }

    [Serializable]
    public class ServerMatchSingleModel
    {
        public ServerMatchInfo match;
    }

    [Serializable]
    public class ServerMatchStart
    {
        public ServerMatchInfo match;
    }

    [Serializable]
    public class ServerMatchStartContainer
    {
        public ServerMatchStart newUserMatch;
    }

    [Serializable]
    public class ServerMatchResult
    {
        public long match_id;
        public long score;
    }

    [Serializable]
    public class ServerFriends
    {
        //"friends":[],"friends_in":[],"friends_out":[]
        public List<ServerPublicUser> friends = new List<ServerPublicUser>();
        public List<ServerPublicUser> friends_in = new List<ServerPublicUser>();
        public List<ServerPublicUser> friends_out = new List<ServerPublicUser>();
    }

    [Serializable]
    public class ServerChatPages
    {
        public ServerResultPage<ServerChatModel> chats;
    }

    [Serializable]
    public class ServerChatModel
    {
        public long id;
        public long owner_id;
        public List<ServerMember> members;
    }

    [Serializable]
    public class ServerNextChatModel
    {
        public ServerChatModel chat;
    }

    [Serializable]
    public class ServerMember
    {
        public long id;
        public long chat_id;
        public long user_id;
        public ServerPublicUser user_info;
    }
    
    [Serializable]
    public class ServerChatMessageModel
    {
        public ServerResultPage<ServerChatMessage> messages;
    }

    [Serializable]
    public class ServerChatMessage
    {
        public long id;
        public long chat_id;
        public long user_id;
        public string created_at;
        public string text;
        public ServerPublicUser user_info;
    }

    [Serializable]
    public class ServerPaymentTransactionPages
    {
        public ServerResultPage<ServerPaymentTransactionModel> all_payments;
        public ServerResultPage<ServerPaymentTransactionModel> usd_payments;
        public ServerResultPage<ServerPaymentTransactionModel> z_payments;
        public ServerResultPage<ServerPaymentTransactionModel> ticket_payments;
    }

    [Serializable]
    public class ServerPaymentTransactionModel
    {
        public long id;
        public string type;
        public long user_id;
        public float amount;
        public string currency;
        public string status;
        public string balance_id;
        public string payment_system;
        public string payment_account;
        public string comment;
        public long match_id;
        public string created_at;
    }

    [Serializable]
    public class ServerTrophies
    {
        public List<ServerTrophiesModel> trophies;
        public List<ServerTrophiesModel> trophies_to_get;
    }

    [Serializable]
    public class ServerTrophiesModel
    {
        public long id;
        public string name;
        public string key;
        public string image;
        public int count_actions;
        public ServerTrophiePivotModel pivot;
        public int actions_left;
        public bool IsComplite => pivot != null && (pivot.count_actions == count_actions);
        public int SortValue => IsComplite ? 2 : ((pivot != null && pivot.count_actions != 0) ? 1 : 0); 
    }

    [Serializable]
    public class ServerTrophiePivotModel
    {
        public long user_id;
        public long trophy_id;
        public int count_actions;
    }
    
    [Serializable]
    public class ServerCreateTournamentModel
    {
        public string name;
        public long game_id;
        public int players_count;
        public int entry_cost;
        public string currency;
        public int autoplay_minutes;
        public int autoplay_duration;
//        public string private_code;
        
        //
//        public string start_at;
//        public string end_at;
//        public int awards_count;
//        public List<ServerTournamentAward> awards;
    }

    [Serializable]
    public class ServerTournamentAward
    {
        public int place;
        public float amount;
        public string currency;
    }

    [Serializable]
    public class ServerTournamentModel : MatchStory
    {
        public long id;
        public string name;
        public string status; //created | cancelled | finished
        public int players_count;
        public string start_at;
        public long game_id;
        public long owner_id;
        public float entry_cost;
        public string currency;
        public List<ServerPublicUser> participants;
        public List<ServerTournamentAward> awards;
        public List<ServerMatchInfo> matches;
        public bool isJoined;
        public bool isMy;
        public string private_code;
    }

    [Serializable]
    public class ServerTournamentEndModel
    {
        public long id;
        public string type;
        public string name;
        public string start_at;
        public string end_at;
        public long game_id;
        public List<ServerLeagueTournamentEndPlace> places;
    }
    
    [Serializable]
    public class ServerLeagueEndModel
    {
        public long id;
        public string type;
        public string name;
        public string start_at;
        public string end_at;
        public long game_id;
        public List<ServerLeagueTournamentEndPlace> places_gold;
        public List<ServerLeagueTournamentEndPlace> places_silver;
    }

    [Serializable]
    public class ServerLeagueTournamentEndPlace
    {
        public long place;
        public float reward_amount;
        public string reward_currency;
        public ServerPublicUser user;
        public string score;
    }

    [Serializable]
    public class ServerTournament
    {
        public ServerResultPage<ServerTournamentModel> all_tournaments;
        public ServerResultPage<ServerTournamentModel> participate_tournaments;
        public ServerResultPage<ServerTournamentModel> user_tournaments;
    }

    [Serializable]
    public class ServerRewardModel
    {
        public long id;
        public string currency;
        public float amount;
        public string comment;
    }

    [Serializable]
    public class ServerJoinTournament
    {
        public ServerMatchTournamentInfo match;
        public ServerPaymentTransactionModel payment;
    }
    
    [Serializable]
    public class ServerRewardContainer
    {
        public ServerRewardModel payment;
    }

    [Serializable]
    public class ServerNotification
    {
        public long id;
        public long notification_id;
        public string short_text;
        public string long_text;
        public int read;
        public string created_at;
    }

    [Serializable]
    public class ServerMatchBet
    {
        public float[] bets;
        public string[] currencies;
    }

    [Serializable]
    public class ServerAward
    {
        public float value;
        public string currency;
    }

    [Serializable]
    public class ServerRequestMatch
    {
        public long id;
        public long game_id;
        public float bet;
        public string currency;
        public long from_user;
        public long to_user;
        public string status; //waiting
    }

    [Serializable]
    public class ServerRequestAccept
    {
        public ServerRequestMatch match_request;
        public ServerPaymentTransactionModel payment;
    }
    
    [Serializable]
    public class ServerRequestMatchesModel
    {
        public ServerResultPage<ServerRequestMatch> from;
        public ServerResultPage<ServerRequestMatch> to;
    }

    [Serializable]
    public class ServerCreateTournamentResult
    {
        public ServerTournamentModel tournament;
    }

    [Serializable]
    public class GameAppListResult
    {
        public ServerResultPage<GameAppInfo> featured_games;
        public ServerResultPage<GameAppInfo> popular_games;
        public ServerResultPage<GameAppInfo> new_games;
    }

    [Serializable]
    public class GameAppResult
    {
        public GameAppInfo game;
    }
    
    [Serializable]
    public class SaveBattleModel
    {
        public List<string> battles = new List<string>();
    }
    
    [Serializable]
    public class TicketShopItemModel
    {
        public long id;
        public string name;
        public string image;
        public string imageUrl => $"https://gamla.io{image}";
        public string description;
        public float price;
        public string type;
        [CanBeNull] public string bonus_usd_amount;
    }

    [Serializable]
    public class TicketShop
    {
        public List<TicketShopItemModel> items;
    }
    
    [Serializable]
    public class GameAppInfo
    {
        public long id;
        public string name;
        public string image;
        public string[] images;
        public string description;
        public string full_description;
        public string android_url;
        public string apple_url;
    }
    
    [Serializable]
    public class ServerResultPage<T>
    {
        public List<T> data;
        public string first_page_url;
        public long from;
        public long current_page;
        public long last_page;
        public string last_page_url;
        public string path;
    }
}