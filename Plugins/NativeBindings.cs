public class NativeBindings
{
#if UNITY_IPHONE
        [DllImport ("__Internal")]
        public static extern string GetSettingsURL();

        [DllImport ("__Internal")]
        public static extern void OpenSettings();
#endif
}
