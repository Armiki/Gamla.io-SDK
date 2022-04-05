using System.Collections.Generic;

namespace Gamla.Scripts.Data
{
    public static class LocalState
    {
        public static string selectColorTemplate = "";
        public static string token = "";
        public static string pushToken = "empty";

        public static UserInfo currentUser; // fill on start by method FillAccountUser / FillGuestUser / FillStoreAccountUser

        public static ServerMatchStart currentMatch;
        public static ServerTournamentModel currentTournament;
        public static List<BattleInfo> newBattleMatches = new List<BattleInfo>();

        public static ServerFriends friends = new ServerFriends();
        public static ServerChatPages chats = new ServerChatPages();
        public static List<ServerTournamentModel> tournaments = new List<ServerTournamentModel>();
        public static SaveBattleModel saveBattleModel = new SaveBattleModel();
        public static GameAppListResult gameApplist = new GameAppListResult();
        
        public static List<Pack> storePacks = new List<Pack>
        {
            new Pack
            {
                name = "hard_2",
                main = new Currency
                {
                    amount = 2,
                    type = CurrencyType.USD
                },
                bonus = new Currency
                {
                    amount = 0,
                    type = CurrencyType.USD
                }
            },
            new Pack
            {
                name = "hard_5",
                main = new Currency
                {
                    amount = 5,
                    type = CurrencyType.USD
                },
                bonus = new Currency
                {
                    amount = 0,
                    type = CurrencyType.USD
                }
            },
            new Pack
            {
                name = "hard_10",
                main = new Currency
                {
                    amount = 10,
                    type = CurrencyType.USD
                },
                bonus = new Currency
                {
                    amount = 2,
                    type = CurrencyType.USD
                }
            },
            new Pack
            {
                name = "hard_15",
                main = new Currency
                {
                    amount = 15,
                    type = CurrencyType.USD
                },
                bonus = new Currency
                {
                    amount = 4,
                    type = CurrencyType.USD
                }
            },
            new Pack
            {
                name = "soft_30",
                main = new Currency
                {
                    amount = 30,
                    type = CurrencyType.USD
                },
                bonus = new Currency
                {
                    amount = 9,
                    type = CurrencyType.USD
                }
            },
            new Pack
            {
                name = "soft_50",
                main = new Currency
                {
                    amount = 50,
                    type = CurrencyType.USD
                },
                bonus = new Currency
                {
                    amount = 15,
                    type = CurrencyType.USD
                }
            },
        };

        public static List<TicketShopItemModel> ticketShop = new List<TicketShopItemModel>();

        public static GameAppInfo gameAppInfo;
        public static GameInfo currentGame = new GameInfo();
    }
}