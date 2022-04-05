namespace Gamla.Scripts.Common.Carousel.ScrollElementsFactories
{
    public class GridLineParams
    {
        public float horizontalSpacing;
        public float verticalSpacing;

        public const int DefaultMaxGridLineSize = 3;
        public const int DefaultHorizontalSpacing = 24;
        public const int DefaultVerticalSpacing = 30;
        
        static readonly GridLineParams _default = new GridLineParams()
        {
            horizontalSpacing = DefaultHorizontalSpacing,
            verticalSpacing = DefaultVerticalSpacing
        };

        public static GridLineParams @default => _default;
    }
}