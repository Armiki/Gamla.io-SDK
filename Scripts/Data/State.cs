using System;
using System.Collections.Generic;
using Gamla.Logic;

namespace Gamla.Data
{
    public enum Language
    {
        English,
        Russian
    }
    public enum BattleStatus
    {
        Win = 0,
        Lost = 1,
        Waiting = 2,
        Searching = 3,
        NoOpponent = 4
    }
    
    public enum BattleType
    {
        None = 0,
        Tournament = 1,
        HardPVP = 2,
        SoftPVP = 3
    }

    public enum CurrencyType
    {
        Z,
        USD,
        TICKETS
    }

    [Serializable]
    public class GameInfo
    {
        public string miniLogo;
        public string backLogo;
        public string name;

        public List<BattleInfo> battles = new List<BattleInfo>();
        public List<HistoryBattleInfo> history = new List<HistoryBattleInfo>();

        public ServerLeagues leagues;
        public List<LadderInfo> ladder;
        
        public GameInfo(){}

        public GameInfo(ServerMatches matches)
        {
            name = "GameName";
            battles = LocalState.currentGame.battles;
            ladder = new List<LadderInfo>();// MockData.currentGame.ladder;
            history =  HistoryBattleInfo.Convert(matches); //MockData.currentGame.history; //
        }
    }

    [Serializable]
    public class BattleInfo
    {
        public BattleType type;
        public Currency win;
        public Currency entry;
        public int trophie;
        public int exp;
        public int tickets;

        public int score;
        // for tournaments
        public int max_gamers;
    }

    [Serializable]
    public class HistoryBattleInfo
    {
        public string matchId;
        public BattleStatus status;

        public ServerPublicUser opponent;
        public ServerPublicUser me;
        public int timeLeft;
        public DateTime date;
        public int returnCount;

        public BattleInfo battleInfo;

        public static List<HistoryBattleInfo> Convert(ServerMatches matches)
        {
            List<HistoryBattleInfo> result = new List<HistoryBattleInfo>();
            foreach (var match in matches.matches.data)
            {
                if (match.game_id == ClientManager.gameId)
                {
                    var history = Convert(match);
                    result.Add(history);
                }
            }
            return result;
        }

        public static HistoryBattleInfo Convert(ServerMatchInfo match)
        {
            return new HistoryBattleInfo()
            {
                matchId = match.id + "",
                status = match.status == "finished" ? (match.winner_id == LocalState.currentUser.uid ? BattleStatus.Win : BattleStatus.Lost) : BattleStatus.Waiting,
                opponent = new ServerPublicUser(match.players, false),
                me = new ServerPublicUser(match.players, true),
                date = DateTime.Parse(match.updated_at),
                battleInfo = new BattleInfo()
                {
                    entry = new Currency(){type = match.currency == "Z" ? CurrencyType.Z : CurrencyType.USD, amount = match.bet },
                    exp = match.matchResult?.experience?.game?.newPoints ?? 0,
                    tickets = (int)match.matchResult.bonus.amount,
                    trophie = 2,// match.matchResult?.medals ?? 0,
                    win = new Currency(){type = match.currency == "Z" ? CurrencyType.Z : CurrencyType.USD, amount = (match.bet * 2) },
                }
            };
        }
    }

    [Serializable]
    public class UserInfo
    {
        public long uid;
        public string sessionId;
        public DateTime dateRegistration;
        
        public string name;
        public string avatar;
        public string avatarUrl;
        public string flagUrl;
        public InnerUserInfo innerUserInfo;

        public List<UserGames> games;
        //public int level;
        //public long exp_cur;
        //public long exp_next_level;

        public long countAllWins;
        public long countAllLoses;

        public long countTotalPlays => countAllLoses + countAllWins;
        
        public int trophie;

        public int userFlag; //bitwise
        public Wallet wallet;

        public bool emailNews;
        public bool guest;

        public DateTime dailyBonus;
        public DateTime hoursBonus;
        public long timer_daily_bonus;
        public long timer_hours_bonus;

        public string promoCode;
        public string promoCodeUrl;

        public List<AccountBalanceData> accountBalanceHistory;

        public UserInfo(){}
        
        public ServerPublicUser ToPublic()
        {
            return new ServerPublicUser()
            {
                id = uid,
                image = avatarUrl,
                name = innerUserInfo.firstName,
                nickname = name,
                surname = innerUserInfo.lastName
            };
        }
        
        public UserInfo(ServerProfile profile)
        {
            uid = profile.id;
            dateRegistration = DateTime.Parse(profile.created_at);
            name = profile.nickname;
            avatarUrl = profile.image;
            flagUrl = profile.flag;
            innerUserInfo = new InnerUserInfo()
            {
                address = profile.address,
                email = profile.email,
                firstName = profile.name,
                lastName = profile.surname,
                phone = profile.phone
            };
            games = UserGames.Convert(profile.experience);
            trophie = 10;
            wallet = new Wallet()
            {
                currencies = new List<Currency>()
                {
                    new Currency() {type = CurrencyType.USD, amount = profile.balances.hard_total},
                    new Currency() {type = CurrencyType.Z, amount = profile.balances.soft},
                    new Currency() {type = CurrencyType.TICKETS, amount = (int)profile.balances.bonus}
                },
                hardTotal = profile.balances.hard_total,
                hardBonus = profile.balances.hard_bonus,
                hard = profile.balances.hard,
                soft = profile.balances.soft,
                bonus = (int)profile.balances.bonus
            };
            
            dailyBonus = String.IsNullOrEmpty(profile.daily_bonus) ? DateTime.Now : DateTime.Parse(profile.daily_bonus);
            hoursBonus = String.IsNullOrEmpty(profile.hours_bonus) ? DateTime.Now : DateTime.Parse(profile.hours_bonus);
            timer_hours_bonus = profile.timer_hours_bonus;
            timer_daily_bonus = profile.timer_daily_bonus;
            accountBalanceHistory = new List<AccountBalanceData>();
            countAllWins = profile.count_all_win;
            countAllLoses = profile.count_all_lose;
            guest = innerUserInfo.email.Contains("guest.io");
            promoCode = profile.referral_code;
            promoCodeUrl = profile.referral_link;
        }
    }

    [Serializable]
    public class UserGames
    {
        public long id;
        public string name;
        public string logo;
        public int level;
        public long expCurLvl;
        public long expNextLvl;

        public static List<UserGames> Convert(ServerProfileExp profileExp)
        {
            var result = new List<UserGames>();
            result.Add(new UserGames
            {
                id = -1,
                name = "Gamla",
                level = profileExp.gamla.level,
                expCurLvl = profileExp.gamla.points,
                expNextLvl = profileExp.gamla.pointsToNextLevel
            });

            foreach (var game in profileExp.games)
            {
                result.Add(new UserGames
                {
                    id = game.game_id,
                    level = game.level,
                    expCurLvl = game.points,
                    expNextLvl = game.pointsToNextLevel
                });
            }
            return result;
        }
    }

    [Serializable]
    public class InnerUserInfo
    {
        public string password;
        public string email;
        public string firstName;
        public string lastName;
        public string address;
        public string phone;
    }

    [Serializable]
    public class LadderInfo
    {
        public bool isGoldLeague;
        public int place;
        public ServerPublicUser user;
        public float amount;
        public CurrencyType currency;

        public static List<LadderInfo> Convert(ServerLeagues leagues)
        {
            List<LadderInfo> result = new List<LadderInfo>();
            if (leagues.leagues.data.Count == 0) return result;

            int goldPlace = 1;
            foreach (ServerMedal medal in leagues.leagues.data[0].gold_medals)
            {
                medal.user_info.trophie = medal.gold_count;
                medal.user_info.id = medal.user_id;
                CurrencyType _type = CurrencyType.TICKETS;
                CurrencyType.TryParse(medal.award.currency, true, out _type);
                result.Add(new LadderInfo()
                {
                    amount = (float)Math.Round(medal.award.value, 2),
                    currency = _type,
                    place = goldPlace,
                    user = medal.user_info,
                    isGoldLeague = true
                });
                goldPlace++;
            }
            int silverPlace = 1;
            foreach (ServerMedal medal in leagues.leagues.data[0].silver_medals)
            {
                medal.user_info.trophie = medal.silver_count;
                medal.user_info.id = medal.user_id;
                CurrencyType _type = CurrencyType.TICKETS;
                CurrencyType.TryParse(medal.award.currency, true, out _type);
                result.Add(new LadderInfo()
                {
                    amount = (float)Math.Round(medal.award.value, 2),
                    currency = _type,
                    place = silverPlace,
                    user = medal.user_info,
                    isGoldLeague = false
                });
                silverPlace++;
            }


            return result;
        }
    }

    [Serializable]
    public class AccountBalanceData
    {
        public GameInfo game;
        public string date;
        public string status;
        public string comment;
        public string type;
        public string battleId;
        public Currency currency;

        public static List<AccountBalanceData> Convert(List<ServerPaymentTransactionModel> transactions)
        {
            List<AccountBalanceData> result = new List<AccountBalanceData>();
            foreach (var transaction in transactions)
            {
                var game = new GameInfo();
                game.name = transaction.type;
                AccountBalanceData data = new AccountBalanceData()
                {
                    game = game,
                    date = transaction.created_at,
                    status = transaction.status,
                    type = transaction.type,
                    comment = transaction.comment,
                    battleId = transaction.match_id + "",
                    currency = ConvertCurrency(transaction)
                };
                result.Add(data);
            }

            return result;
        }

        private static Currency ConvertCurrency(ServerPaymentTransactionModel transaction)
        {
            CurrencyType currencyType;
            Enum.TryParse(transaction.currency, out currencyType);
            return new Currency()
            {
                amount = transaction.amount,
                type = currencyType
            };
        }
    }

    [Serializable]
    public class Wallet
    {
        public List<Currency> currencies = new List<Currency>();
        public float hardTotal = 0;
        public float hardBonus = 0;
        public float hard = 0;
        public float soft = 0;
        public float bonus = 0;
    }
    
    [Serializable]
    public class Currency
    {
        public string currency;
        public CurrencyType type;
        public float amount;

        public static List<Currency> parse(string source)
        {
            string[] data = source.Split(';');
            List<Currency> result = new List<Currency>();
            foreach (var variable in data)
            {
                string[] vars = variable.Split(':');
                if(CurrencyType.TryParse(vars[0], true, out CurrencyType type))
                {
                    Currency currency = new Currency()
                    {
                        type = type,
                        amount = float.Parse(vars[1])
                    };
                    result.Add(currency);
                }
            }
            return result;
        }
    }

    [Serializable]
    public class Pack
    {
        public string name;
        public Currency main;
        public Currency bonus;
        
        public Pack(){}

        public Pack(ServerRewardModel rewardModel)
        {
            name = rewardModel.comment;
            main = new Currency(){type = rewardModel.currency == "Z" ? CurrencyType.Z : CurrencyType.USD, amount = rewardModel.amount };
        }
    }
}