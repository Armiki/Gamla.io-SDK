#if UNITY_EDITOR
using Gamla.Core;
using GamlaSDK.Scripts;
using UnityEditor;
using UnityEngine;

namespace Gamla.Editor
{
    public class GamlaUtilityWindow : EditorWindow
    {
        private static readonly string sessionKey = "Gamla.GamlaUtilityWindow.HasBeenShown";
        private static GamlaConfigObject clientConfig;

        [InitializeOnLoadMethod]
        static void InitShowAtStartup()
        {
            if (SessionState.GetBool(sessionKey, false))
                return;

            SessionState.SetBool(sessionKey, true);
            EditorApplication.update += ShowAtStartup;
        }
        
        static void ShowAtStartup()
        {
            EditorApplication.update -= ShowAtStartup;

            if(!Application.isPlaying)
                Init();
        }
        
        [MenuItem("Window/GamlaSettings")]
        public static void Init()
        {
            clientConfig = ScriptableObjectUtility.Find<GamlaConfigObject>();
            if (clientConfig == null)
            {
                clientConfig = ScriptableObjectUtility.Create<GamlaConfigObject>("GamlaConfig");
            }
            
            var window = GetWindow<GamlaUtilityWindow>(true);

            window.titleContent = new GUIContent("Gamla Settings");

            window.maxSize = new Vector2(400, 620);
            window.minSize = window.maxSize;
        }
        
        void OnGUI()
        {
            OnHeaderGUI();

            OnBodyGUI();
            
            OnFooterGUI();
        }

        private void OnDestroy()
        {
            AssetDatabase.SaveAssets();
        }

        void OnHeaderGUI()
        {
            DrawHeader("GAMLA.io");
            
            var headerStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };

            GUILayout.Label($"SDK version {ClientManager.version}", headerStyle);
            
            Rect line = GUILayoutUtility.GetRect(position.width, 1);
            EditorGUI.DrawRect(line, Color.black);
            GUILayout.Space(10);

            if (clientConfig == null || clientConfig.GamlaID == 0)
            {
                GUILayout.Space(10);
                GUILayout.Label("Set your Game ID !!", EditorStyles.boldLabel);
                GUILayout.Space(10);
            }
            
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("GameID:", EditorStyles.boldLabel);
                string gameId = clientConfig.GamlaID + "";
                gameId = GUILayout.TextField(gameId);
                if(int.TryParse(gameId, out int result))
                {
                    AssetDatabase.StartAssetEditing();
                    clientConfig.GamlaID = result;
                    AssetDatabase.StopAssetEditing();
                    EditorUtility.SetDirty(clientConfig);
                }
            }

            GUILayout.FlexibleSpace();
        }

        void OnBodyGUI()
        {
            
        }
        
        void OnFooterGUI()
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label("Thank you for choosing Gamla.io!", EditorStyles.boldLabel);
            
            Rect line = GUILayoutUtility.GetRect(position.width, 1);

            EditorGUI.DrawRect(line, Color.black);

            GUILayout.Space(10);
            if (GUILayout.Button("Close"))
            {
                AssetDatabase.SaveAssets();
                Close();
            }
            GUILayout.Space(10);
        }
        
        void DrawHeader(string text)
        {
            Rect line = GUILayoutUtility.GetRect(position.width, 0);

            line.height = 22;

            EditorGUI.DrawRect(line, new Color(0.2f, 0.2f, 0.2f));

            GUILayout.Space(1);
            GUILayout.Label(text, EditorStyles.boldLabel);

            GUILayout.Space(8);
        }
    }
}
#endif