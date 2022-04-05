using System;
using System.Collections.Generic;
using Gamla.Core;
using Gamla.Scripts.Common;
using Gamla.Scripts.Data;

namespace Gamla.Scripts.Logic
{
    public class EventManager
    {
        public static EventTrigger OnProfileUpdate = new EventTrigger();
        public static EventTrigger OnWalletUpdate = new EventTrigger();
        public static EventTrigger OnGameInfoUpdate = new EventTrigger();
        public static EventTrigger<long> OnFriendChatLoad = new EventTrigger<long>();
        public static EventTrigger<ServerPublicUser> OnUserWidgetClick = new EventTrigger<ServerPublicUser>();
        public static EventTrigger OnLanguageChange = new EventTrigger();

        public static DelayedEventManager DelayEvent = new DelayedEventManager();
    }
    
    public class DelayedEvent
    {
        public Action action;
        private long timeToGo;
        private bool isLoop;
        private long cooldown;

        public DelayedEvent(Action _action, int _cooldown = 1000, bool _isLoop = false)
        {
            action = _action;
            cooldown = _cooldown;
            isLoop = _isLoop;
            timeToGo = Utils.GetPlainUnixTime + cooldown;
        }
	
        public bool Invoke()
        {
            if (timeToGo <= Utils.GetPlainUnixTime)
            {
                if(action != null) action.Invoke();
                if(isLoop) timeToGo = Utils.GetPlainUnixTime + cooldown;

                return !isLoop;
            }

            return false;
        }
    }
    
    public class DelayedEventManager
    {
        private readonly Dictionary<string, DelayedEvent> Events = new Dictionary<string, DelayedEvent>();

        public void Add(string name, Action eventAction, int cooldown = 1000, bool isLoop = false)
        {
            if (Events.ContainsKey(name.ToLower()))
            {
                Events[name.ToLower()] = new DelayedEvent(eventAction, cooldown, isLoop);
            }
            else Events.Add(name.ToLower(), new DelayedEvent(eventAction, cooldown, isLoop));
        }

        public void Update()
        {
            List<string> keysToDestroy = new List<string>();
            foreach (var _event in Events)
            {
                if (_event.Value.Invoke()) keysToDestroy.Add(_event.Key);
            }

            foreach (var key in keysToDestroy)
            {
                Events.Remove(key);
            }
            keysToDestroy.Clear();
        }
    }
}