namespace Gamla.UI.Carousel
{
    public interface IUpdatableScrollElementsHolder : IScrollElementsHolder
    {
        void UpdateSize(float newSize, int index);
        void AddLast();
        void RemoveLast();
        void Insert(int index);
        void Remove(int index);
        void ClearAndUpdateSettings(float newExpectedSizeOfElement);
    }
}