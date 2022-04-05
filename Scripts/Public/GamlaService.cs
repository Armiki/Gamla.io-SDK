using Gamla.Core;

namespace Gamla
{

    public static class GamlaService
    {
        /// <summary>
        /// Push open event for start Gamla UI
        /// SEND: firebase push token(string) 
        /// </summary>
        public static EventTrigger<string> Open = new EventTrigger<string>();

        /// <summary>
        /// Subscribe to start match
        /// SUBSCRIBE: matchId(string), matchData(json) 
        /// </summary>
        public static EventTrigger<string, string, bool> OnMatchStarted = new EventTrigger<string, string, bool>();
        
        /// <summary>
        /// Push end match event
        /// SEND: matchId(string), score(int) bigger better
        /// </summary>
        public static EventTrigger<int> MatchEnd = new EventTrigger<int>();
        
        public static EventTrigger ReturnToGamla = new EventTrigger();
    }
}