using System;

namespace Gamla.Scripts.Common
{
    public interface ITickProvider
    {
        /// <summary>
        /// Rise after new tick 
        /// </summary>
        event Action OnTick;
    }
}