using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Gamla.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Gamla.Logic
{
    public class ClientManager : MonoBehaviour
    {
        public static bool isInited = false;
        public static int gameId = 0;
        public static readonly string version = "0.9.21";
        private readonly string _uri = "https://gamla.io/api";

        private static ClientManager _instance;
        private static bool isConnected = false;


        private void Awake()
        {
            gameId = Resources.Load<GamlaConfigObject>("GamlaConfig").GamlaID;
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            #if UNITY_EDITOR
            if (gameId == 0)
            {
                Debug.LogError("You must set game id in settings window (Window/GamlaSettings)");
            }
            #endif

            _instance = this;
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            GamlaResourceManager.NetChecker.OnCheckFinished += OpenAfterCheckConnection;
            GamlaResourceManager.NetChecker.StartConnectionCheck();
            GamlaService.MatchEnd.Subscribe(score =>
            {
                if(!isConnected)
                    UIMapController.OpenSimpleErrorWindow("NO INTERNET CONNECTION", () => GamlaService.Open.Push(LocalState.pushToken));
            });
        }

        private void OpenAfterCheckConnection(NetChecker.Status status)
        {
            if (status == NetChecker.Status.CONNECTED)
            {
                isConnected = true;
            }
            else
            {
                if (isConnected && isInited && !LocalState.isInMatch)
                {
                    UIMapController.OpenSimpleErrorWindow("NO INTERNET CONNECTION", () => GamlaService.Open.Push(LocalState.pushToken));
                }
                isConnected = false;
            }
        }

        public static void InvokeEvent<T>(string eventName, string data, Action<T> resultJson,
            Action<ErrorModel> errorJson)
        {
            if(!isConnected) return;
            _instance.StartCoroutine(_instance.ApiPost(eventName, data, resultJson, errorJson));
        }

        public static void InvokeEvent<T>(string token, string eventName, string data, Action<T> resultJson,
            Action<ErrorModel> errorJson)
        {
            if(!isConnected) return;
            _instance.StartCoroutine(_instance.ApiPost(eventName, data, resultJson, errorJson, token));
        }

        public static void GetData<T>(string token, string eventName, Action<T> resultJson,
            Action<ErrorModel> errorJson)
        {
            if(!isConnected) return;
            _instance.StartCoroutine(_instance.ApiGet(eventName, resultJson, errorJson, token));
        }


        public static void PutData<T>(string eventName, string token, string data, Action<T> resultJson,
            Action<ErrorModel> errorJson = null)
        {
            if(!isConnected) return;
            _instance.StartCoroutine(_instance.ApiPut(eventName, token, data, resultJson, errorJson));
        }

        IEnumerator ApiPost<T>(string eventName, string data, Action<T> resultJson, Action<ErrorModel> errorJson = null,
            string token = "")
        {
            UnityWebRequest www = UnityWebRequest.Post($"{_uri}/{eventName}", data);
            if (token != String.Empty)
            {
                www.SetRequestHeader("Authorization", "Bearer " + token);
            }

            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            if (data != "")
                www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));

            string log = data.Length >= 1000 ? "" : data;
            Debug.Log(www.uri.OriginalString + " | " + log);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string resultText = www.downloadHandler.text;

                Debug.Log($"[GAMLA] {www.url} \n {resultText}");

                ParseResponse(resultText, resultJson, errorJson);
            }
            else
            {
                Debug.LogError($"[GAMLA] {eventName} {www.error} {www.downloadHandler.text}");

                string resultText = www.downloadHandler.text;
                if (string.IsNullOrEmpty(resultText))
                {
                    UIMapController.OpenSimpleErrorWindow(www.error);
                }
                else
                {
                    ParseResponse(resultText, resultJson, errorJson);
                }

            }
        }



        IEnumerator ApiPut<T>(string eventName, string token, string data, Action<T> resultJson,
            Action<ErrorModel> errorJson = null)
        {
            //string json = data.ToJsonString();

            UnityWebRequest www = UnityWebRequest.Put($"{_uri}/{eventName}", data);
            www.SetRequestHeader("Authorization", "Bearer " + token);
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            if (data.Length > 0)
                www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string resultText = www.downloadHandler.text;

                Debug.Log($"[GAMLA PUT] {www.url} \n {resultText}");

                ParseResponse(resultText, resultJson, errorJson);
            }
            else
            {
                Debug.LogError($"[GAMLA] {eventName} {www.error}");

                string resultText = www.downloadHandler.text;
                if (resultText == "")
                {
                    UIMapController.OpenSimpleErrorWindow(www.error);
                }
                else
                {
                    ParseResponse(resultText, resultJson, errorJson);
                }

            }
        }

        IEnumerator ApiGet<T>(string eventName, Action<T> resultJson, Action<ErrorModel> errorJson = null,
            string token = "", string data = "")
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            UnityWebRequest www = UnityWebRequest.Get($"{_uri}/{eventName}");
            if (token != String.Empty)
            {
                www.SetRequestHeader("Authorization", "Bearer " + token);
            }

            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            if (data.Length > 0)
                www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));

            yield return www.SendWebRequest();

            watch.Stop();
            Debug.Log($"request time {eventName} :: {watch.ElapsedMilliseconds} ms");
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                string resultText = www.downloadHandler.text;

                Debug.Log($"[GAMLA] {www.url} \n {resultText}");
                ParseResponse(resultText, resultJson, errorJson);
            }
            else
            {
                Debug.LogError($"[GAMLA] {www.uri.OriginalString} : {www.error}");

                string resultText = www.downloadHandler.text;
                if (resultText == "")
                {
                    UIMapController.OpenSimpleErrorWindow(www.error);
                }
                else
                {
                    ParseResponse(resultText, resultJson, errorJson);
                }

            }
        }

        private void ParseResponse<T>(string resultText, Action<T> resultData, Action<ErrorModel> error)
        {
            try
            {
                ResponseModel<T> response = JsonUtility.FromJson<ResponseModel<T>>(resultText);
                if (response.status == "ok")
                    resultData.Invoke(response.body);
                else
                {
                    if (response.error.message == "")
                    {
                        response.error.message = resultText;
                    }

                    error?.Invoke(response.error);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                UIMapController.OpenSimpleErrorWindow(e.Message);
            }
        }


        public static void SaveRematchInfo(string matchId)
        {
            PlayerPrefs.SetInt("rematch_" + matchId, 1);
            PlayerPrefs.Save();
        }

        public static void SaveMatchScore(string data)
        {
            var source = PlayerPrefs.GetString("matches", "");
            Matches matches = JsonUtility.FromJson<Matches>(source);
            if (matches == null) {
                matches = new Matches();
            }
            matches.datas.Add(data);
            PlayerPrefs.SetString("matches", JsonUtility.ToJson(matches));
            PlayerPrefs.Save();
        }

        public static void RemoveMatchScore(string data)
        {
            var source = PlayerPrefs.GetString("matches", "");
            Matches matches = JsonUtility.FromJson<Matches>(source);
            if (matches == null) {
                return;
            }
            matches.datas.Remove(data);
            PlayerPrefs.SetString("matches", JsonUtility.ToJson(matches));
            PlayerPrefs.Save();
        }

        public static List<string> GetMatchScore()
        {
            var source = PlayerPrefs.GetString("matches", "");
            Matches matches = JsonUtility.FromJson<Matches>(source);
            if (matches == null)
            {
                matches = new Matches();
                PlayerPrefs.SetString("matches", JsonUtility.ToJson(matches));
            }
            return matches.datas;
        }

        public static void SaveTempMatchScore(int score)
        {
            long matchId = LocalState.currentMatch != null ? LocalState.currentMatch.match.id : 0;
            string data = JsonUtility.ToJson(new ServerMatchResult()
            {
                match_id = matchId,
                score = score
            });
            
            PlayerPrefs.SetString("temp", data);
            PlayerPrefs.Save();
        }

        public static string GetTempMatch()
        {
            return PlayerPrefs.GetString("temp", "");
        }

        public static void ClearTempMatch()
        {
            PlayerPrefs.SetString("temp", "");
            PlayerPrefs.Save();
        }

        public static void UpdateMatchesLazy()
        {
            _instance.Invoke("UpdateMatches", 2);
        }

        void UpdateMatches()
        {
            ServerCommand.GetOrUpdateMatches();
        }
    }

    [Serializable]
    public class TestUserName
    {
        public string nickname;
    }

    [Serializable]
    public class Matches
    {
        public List<string> datas = new List<string>();
    }
}