using System;

namespace Gamla.UI
{
    public interface IGUIView
    {
        public event Action<IGUIView> onShow;
        public event Action<IGUIView> onClosed;
    }

    public enum WindowMode
    {
        None,
        Screen,
        Dialog,
        Reward,
        FullDialog,
        Full
    }
}