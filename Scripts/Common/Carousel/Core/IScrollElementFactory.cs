namespace Gamla.Scripts.Common.Carousel.Core
{
    public interface IScrollElementFactory
    {
        #region Public Members
        /// <summary>
        /// Create new scroll element for index
        /// </summary>
        IScrollElement CreateElement(int index);

        /// <summary>
        /// Dispose element
        /// </summary>
        void DisposeElement(IScrollElement element);
        #endregion
    }
}