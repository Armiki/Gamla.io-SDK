using System;

namespace Gamla.Logic
{
    public interface ITickProvider
    {
        /// <summary>
        /// Rise after new tick 
        /// </summary>
        event Action OnTick;
    }
}