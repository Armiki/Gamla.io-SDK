using System;
using System.Collections;

namespace Gamla.Logic
{
    public interface ISessionTickProvider : ITickProvider
    {
        event EventHandler OnSessionTick;
    }

    public interface IHomeThreadHelper : ISessionTickProvider
    {

        void Execute(Action<object> method, object parameters);
        void Execute(Action<object> method);

        ICoroutineState ExecuteCoroutine(IEnumerator enumerator, Action onFinish);
        ICoroutineState ExecuteCoroutine(IEnumerator enumerator);
        void ExecuteUnhadledCoroutine(IEnumerator enumerator);
        void SetUnhadledCoroutineDummy(HomeThreadHelper dummy);

        void TickTack();
        void EndSession();
        void StartSession();

        void Clean();
    }

    public interface ICoroutineInstruction
    {
        bool ready { get; }
    }

    public interface ICoroutineResult<T> : ICoroutineInstruction
    {
        T result { get; }
    }

    public interface ICancelable
    {
        bool canceled { get; }
        void Cancel();
    }

    public interface IWaitWithTimeout : ICoroutineInstruction
    {
        bool timeout { get; }
    }

    public interface ICoroutineState : ICoroutineInstruction, ICancelable
    {
    }
}