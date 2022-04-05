using System.Collections;
using System.Collections.Generic;
using System;

namespace Gamla.Scripts.Common
{
    public class HomeThread : IHomeThreadHelper
    {
        private class MethodToExecute
        {
            public Action<object> callback { get; set; }
            public object parameters { get; set; }
        }

        class Coroutine
        {
            public ICoroutineInstruction state;
            public IEnumerator enumerator;
            public ICoroutineState dependencyCoroutine;
            public Action onFinish;
            public bool shouldTerminate;
            public bool shouldRemove;
        }

        class CoroutineState : ICoroutineState
        {
            public bool ready { get; private set; }

            public bool canceled
            {
                get { return _coroutine.shouldTerminate; }
            }

            Coroutine _coroutine;

            public CoroutineState(Coroutine coroutine)
            {
                _coroutine = coroutine;
                coroutine.onFinish += onCoroutineFinish;
            }

            void onCoroutineFinish()
            {
                _coroutine.onFinish -= onCoroutineFinish;
                ready = true;
            }

            public void Cancel()
            {
                _coroutine.shouldTerminate = true;
            }
        }

        Queue<MethodToExecute> _callbacks = new Queue<MethodToExecute>();
        List<Coroutine> _coroutines = new List<Coroutine>();

        List<MethodToExecute> _methods = new List<MethodToExecute>();
        List<Coroutine> _executeCoroutines = new List<Coroutine>();

        bool _hasCoroutinesToRemove;
        HomeThreadHelper _unhandledCoroutineDummy;
        bool _inSession;

        public event Action OnTick;

        public void TickTack()
        {
            if (OnTick != null)
            {
                OnTick();
            }

            if (_inSession && OnSessionTick != null)
            {
                OnSessionTick(this, EventArgs.Empty);
            }

            if (_callbacks.Count > 0)
            {
                ProcessTasks();
            }

            if (_coroutines.Count > 0)
            {
                ProcessCoroutines();
            }
        }

        public void EndSession()
        {
            _inSession = false;
        }

        public void StartSession()
        {
            _inSession = true;
        }

        public void Clean()
        {
            lock (_coroutines)
            {
                _coroutines.Clear();
            }

            lock (_callbacks)
            {
                _callbacks.Clear();
            }

            _hasCoroutinesToRemove = false;
        }

        public event EventHandler OnSessionTick;

        public void Execute(Action<object> method, object parameters)
        {
            lock (_callbacks)
            {
                var methodToExecute = new MethodToExecute {callback = method, parameters = parameters};
                _callbacks.Enqueue(methodToExecute);
            }
        }

        public void Execute(Action<object> method)
        {
            Execute(method, null);
        }

        public ICoroutineState ExecuteCoroutine(IEnumerator enumerator)
        {
            return ExecuteCoroutine(enumerator, null);
        }

        public ICoroutineState ExecuteCoroutine(IEnumerator enumerator, Action onFinish)
        {
            var coroutine = new Coroutine
            {
                enumerator = enumerator,
                onFinish = onFinish
            };

            var state = new CoroutineState(coroutine);

            lock (_coroutines)
            {
                _coroutines.Add(coroutine);
            }

            return state;
        }

        public void ExecuteUnhadledCoroutine(IEnumerator enumerator)
        {
            _unhandledCoroutineDummy.StartCoroutine(enumerator);
        }

        public void SetUnhadledCoroutineDummy(HomeThreadHelper dummy)
        {
            _unhandledCoroutineDummy = dummy;
        }

        public void StopCoroutine()
        {
            lock (_coroutines)
            {
//			_coroutines.Add (coroutine);
            }
        }

        void ProcessTasks()
        {
            _methods.Clear();

            lock (_callbacks)
            {
                while (_callbacks.Count > 0)
                {
                    var method = _callbacks.Dequeue();
                    _methods.Add(method);
                }
            }

            if (_methods.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _methods.Count; i++)
            {
                var method = _methods[i];
                try
                {
                    method.callback(method.parameters);
                }
                catch (Exception e)
                {

                }
            }
        }

        void ProcessCoroutines()
        {
            if (_executeCoroutines.Count > 0)
            {
                _executeCoroutines.Clear();
            }

            lock (_coroutines)
            {
                for (int i = 0; i < _coroutines.Count; i++)
                {
                    _executeCoroutines.Add(_coroutines[i]);
                }
            }

            for (var i = 0; i < _executeCoroutines.Count; i += 1)
            {
                var coroutine = _executeCoroutines[i];

                if (!coroutine.shouldTerminate && CoroutineStep(coroutine))
                {
                    //++i;
                }
                else
                {
                    coroutine.shouldRemove = true;
                    _hasCoroutinesToRemove = true;
                    //_coroutines.RemoveAt(i);
                    try
                    {
                        if (coroutine.onFinish != null)
                        {
                            coroutine.onFinish();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            if (_hasCoroutinesToRemove)
            {
                lock (_coroutines)
                {
                    for (var i = _executeCoroutines.Count - 1; i >= 0; i -= 1)
                    {
                        var coroutine = _executeCoroutines[i];
                        if (coroutine.shouldRemove)
                        {
                            _coroutines.RemoveAt(i);
                        }
                    }

                    _hasCoroutinesToRemove = false;
                }
            }
        }

        bool CoroutineStep(Coroutine coroutine)
        {
            var state = coroutine.state;

            if (coroutine.dependencyCoroutine != null && !coroutine.dependencyCoroutine.ready)
            {
                return true;
            }

            if (state != null && !state.ready)
            {
                return true;
            }

            if (!coroutine.enumerator.MoveNext())
            {
                return false;
            }

            coroutine.state = coroutine.enumerator.Current as ICoroutineInstruction;

            if (coroutine.state == null)
            {
                var dependencyEnumerator = coroutine.enumerator.Current as IEnumerator;
                if (dependencyEnumerator != null)
                {
                    coroutine.dependencyCoroutine = ExecuteCoroutine(dependencyEnumerator);
                }
            }

            return true;
        }
    }
}