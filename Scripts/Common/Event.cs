using System.Collections.Generic;
using System;

namespace Gamla.Core
{
    public class EventTrigger
    {

        private readonly List<Action> _callbacks = new List<Action>();

        public void Subscribe(Action callback)
        {
            _callbacks.Add(callback);
        }

        public void Del(Action callback)
        {
            _callbacks.Remove(callback);
        }

        public void Push()
        {
            for (int i = 0; i < _callbacks.Count; i++)
                if (_callbacks[i] != null)
                    _callbacks[i]();
        }
    }

    public class EventTrigger<T1>
    {

        private readonly List<Action<T1>> _callbacks = new List<Action<T1>>();

        public void Subscribe(Action<T1> callback)
        {
            _callbacks.Add(callback);
        }

        public void Del(Action<T1> callback)
        {
            _callbacks.Remove(callback);
        }

        public void Push(T1 eventData)
        {
            for (int i = 0; i < _callbacks.Count; i++)
                if (_callbacks[i] != null)
                    _callbacks[i](eventData);
        }
    }

    public class EventTrigger<T1, T2>
    {

        private readonly List<Action<T1, T2>> _callbacks = new List<Action<T1, T2>>();

        public void Subscribe(Action<T1, T2> callback)
        {
            _callbacks.Add(callback);
        }

        public void Del(Action<T1, T2> callback)
        {
            _callbacks.Remove(callback);
        }

        public void Push(T1 eventDataT1, T2 eventDataT2)
        {
            for (int i = 0; i < _callbacks.Count; i++)
                if (_callbacks[i] != null)
                    _callbacks[i](eventDataT1, eventDataT2);
        }
    }

    public class EventTrigger<T1, T2, T3>
    {

        private readonly List<Action<T1, T2, T3>> _callbacks = new List<Action<T1, T2, T3>>();

        public void Subscribe(Action<T1, T2, T3> callback)
        {
            _callbacks.Add(callback);
        }

        public void Del(Action<T1, T2, T3> callback)
        {
            _callbacks.Remove(callback);
        }

        public void Push(T1 eventDataT1, T2 eventDataT2, T3 eventDataT3)
        {
            for (int i = 0; i < _callbacks.Count; i++)
                if (_callbacks[i] != null)
                    _callbacks[i](eventDataT1, eventDataT2, eventDataT3);
        }
    }

    public class EventTrigger<T1, T2, T3, T4>
    {

        private readonly List<Action<T1, T2, T3, T4>> _callbacks = new List<Action<T1, T2, T3, T4>>();

        public void Subscribe(Action<T1, T2, T3, T4> callback)
        {
            _callbacks.Add(callback);
        }

        public void Del(Action<T1, T2, T3, T4> callback)
        {
            _callbacks.Remove(callback);
        }

        public void Push(T1 eventDataT1, T2 eventDataT2, T3 eventDataT3, T4 eventDataT4)
        {
            for (int i = 0; i < _callbacks.Count; i++)
                if (_callbacks[i] != null)
                    _callbacks[i](eventDataT1, eventDataT2, eventDataT3, eventDataT4);
        }
    }
}