using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Gamla.Scripts.Common;
using Gamla.Scripts.Logic;
using UnityEngine;
using UnityEngine.Networking;

namespace GamlaSDK.Scripts
{
    public class LocalizationManager
    {
        private static readonly string LOCALE_CONFIG_URL = "https://static-gurii.fra1.cdn.digitaloceanspaces.com/gamla/localeconfig.json";
        private static readonly string DEFAULT_LANGUAGE = "english";
        public static LocaleConfig Config;

        public static string CurrentLanguage { get; private set; }

        private static Dictionary<string, string> _textBd = new Dictionary<string, string>();

        private static IEnumerator LoadVersion(Action<bool> callback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(LOCALE_CONFIG_URL))
            {
                yield return webRequest.SendWebRequest();
                while(!webRequest.isDone) {}
                
                if (!string.IsNullOrEmpty(webRequest.error))
                {
                    UIMapController.OpenSimpleErrorWindow("NO INTERNET CONNECTION", () =>
                    {
                        Debug.Log("do re load config");
                    });
                    callback?.Invoke(false);
                    yield break;
                }

                string data = webRequest.downloadHandler.text;
                Debug.Log(data);
                LocaleConfig localeConfig = JsonUtility.FromJson<LocaleConfig>(data);
                Config = localeConfig;

                foreach (var localeConfigValue in localeConfig.Locales)
                {
                    var cacheFilePath = Path.Combine(GetPath(), localeConfigValue.Filename);
                    if (!File.Exists(cacheFilePath))
                    {
                        yield return HomeThreadHelper.homeThread.ExecuteCoroutine(LoadSaveContent(localeConfigValue.Url, localeConfigValue.Filename));
                    }
                }
                Debug.Log("locale inited");
                callback?.Invoke(true);
            }
        }
        
        private static IEnumerator LoadSaveContent(string url, string fileName)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();
                while (!webRequest.isDone)
                {
                    yield return new WaitForSeconds(0.5f);
                }

                yield return webRequest.downloadHandler.isDone;

                if (string.IsNullOrEmpty(webRequest.error))
                {
                    string result = webRequest.downloadHandler.text;
                    Debug.Log(result);
                    if (!string.IsNullOrEmpty(result))
                    {
                        var cacheFilePath = Path.Combine(GetPath(), fileName);
                        File.WriteAllText(cacheFilePath, result);
                    }
                }
                else
                {
                    Debug.LogError(webRequest.error);
                }
            }
        }
        

        public static void Init(string language, Action callback)
        {
            if (!Directory.Exists(GetPath())) 
                //Directory.Delete(GetPath(), true);
                Directory.CreateDirectory(GetPath());
            
            HomeThreadHelper.homeThread.ExecuteCoroutine(LoadVersion(b =>
            {
                ChangeLanguage(language);
                callback.Invoke();
            }));
        }

        public static void ChangeLanguage(string language)
        {
            if (Config.Locales.FindAll(v => v.Locale == language).Count == 0)
                language = DEFAULT_LANGUAGE;

            CurrentLanguage = language;

            UpdateLocale(CurrentLanguage);
        }
        
        private static void UpdateLocale(string language)
        {
            PlayerPrefs.SetString("locale", language);
            _textBd.Clear();
            var cacheFilePath = Path.Combine(GetPath(), Config.Locales.Find(v => v.Locale == language).Filename);
            if (File.Exists(cacheFilePath))
            {
                AddKeys(_textBd, File.ReadAllText(cacheFilePath));
            }
            EventManager.OnLanguageChange.Push();
        }

        private static void AddKeys(Dictionary<string, string> textBd, string localeJson)
        {
            LocaleData localeData = JsonUtility.FromJson<LocaleData>(localeJson);
            if (localeData != null)
            {
                foreach (var data in localeData.Dict)
                {
                    textBd[data.Key] = data.Value;
                }
            }
        }
        
        public static string Text(string code){
            if (code != null && _textBd != null && _textBd.TryGetValue(code, out var result)){
                return result;
            }
            
            return $"{code}";
        }

        private static string GetPath()
        {
#if UNITY_EDITOR
            return Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Library/GamlaCache");
#else
            return Path.Combine(Application.persistentDataPath, "_GamlaCache");
#endif
        }
    }
    
    [Serializable]
    public class LocaleData
    {
        public List<LocaleValue> Dict = new List<LocaleValue>();
    }

    [Serializable]
    public class LocaleValue
    {
        public string Key;
        public string Value;
    }
    
    [Serializable]
    public class LocaleConfig
    {
        public List<LocaleConfigValue> Locales = new List<LocaleConfigValue>();
    }
    
    [Serializable]
    public class LocaleConfigValue
    {
        public string Locale;
        public string Url;
        public string Filename;
    }
}