using UnityEngine;
using System;
using System.Collections;
using Gamla.Scripts.Logic;
using UnityEngine.UI;

namespace Gamla.Scripts.Common.UI
{
    public class GUIView : MonoBehaviour, IGUIView
    {
        public event Action<IGUIView> onShow;
        public event Action<IGUIView> onClosed;

        [SerializeField] private WindowMode _windowMode;
        [SerializeField] private AnimationPatternType _animationPatternType;
        [SerializeField] private GameObject _animationContent;
        [SerializeField] bool _fitSafeArea = true;

        [SerializeField] private Button _close;

        public WindowMode windowMode { get { return _windowMode; } }

        protected CanvasGroup _canvasGroup;
        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            if (_close != null)
            {
                _close.onClick.RemoveAllListeners();
                _close.onClick.AddListener(Close);
            }

            InitSafeArea();
            
            GamlaService.OnMatchStarted.Subscribe(OnMatchStart);
        }

        public virtual void OnEnable()
        {
            if (windowMode != WindowMode.None)
            {
                GamlaResourceManager.tabBar.gameObject.SetActive(windowMode == WindowMode.Screen);
                GamlaResourceManager.topBar.gameObject.SetActive(windowMode == WindowMode.Screen);
            }
        }

        public virtual void OnDestroy()
        {
            GamlaService.OnMatchStarted.Del(OnMatchStart);
        }

        public void ClosePublic()
        {
            Close();
        }
        
        protected void ForceClose()
        {
            onClosed?.Invoke(this);
            onClosed = null;
            GamlaResourceManager.safeAreaManager.IndentChange -= ChangeScreen;
            
            Destroy(gameObject);
        }

        protected virtual void Close()
        {
            onClosed?.Invoke(this);
            onClosed = null;
            GamlaResourceManager.safeAreaManager.IndentChange -= ChangeScreen;
            if (_windowMode == WindowMode.None)
            {
                Destroy(gameObject);
                return;
            }
            
            if (_windowMode == WindowMode.Dialog || _windowMode == WindowMode.FullDialog)
            {
                UIMapController.DestroyWindowBefore(name);
            }
            else
            {
                if(gameObject.activeInHierarchy)
                    StartCoroutine(CloseAfterAnim());
            }
        }
        
        void InitSafeArea() {
            if (!_fitSafeArea) {
                return;
            }
            GamlaResourceManager.safeAreaManager.IndentChange += ChangeScreen;
            ChangeScreen( GamlaResourceManager.safeAreaManager.Indent);
        }
        
        void ChangeScreen(int indentSize) {
            GamlaResourceManager.topBar.RefreshSafeZone(indentSize);
            GamlaResourceManager.tabBar.RefreshSafeZone(indentSize);
            if (this == null || gameObject == null) {
                return;
            }
            RefreshSafeZone(indentSize);
        }
        protected virtual void RefreshSafeZone(int indentSize)
        {
            var root = GetComponent<RectTransform>();
            root.localPosition = new Vector2(0, indentSize/2f);
            root.anchoredPosition = new Vector2(0, indentSize/2f);
            root.sizeDelta = new Vector2(0, indentSize);
        }
        
        IEnumerator CloseAfterAnim()
        {
            yield return new WaitForSecondsRealtime(2f);
            UIMapController.DestroyWindowBefore(name);
        }

        public void Show()
        {
            GamlaResourceManager.BackView = true;
            onShow?.Invoke(this);
            if (_animationContent != null)
            {
                switch (_animationPatternType)
                {
                    case AnimationPatternType.WinShiftBottom:
                        AnimationPattern.WindowShiftFromBottom(_animationContent.transform);
                        break;
                    case AnimationPatternType.WinShiftLeft:
                        AnimationPattern.WindowShiftFromLeft(_animationContent.transform);
                        break;
                    default:
                        break;
                }
              
            }
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            GamlaResourceManager.BackView = false;
        }

        void OnMatchStart(string s1, string s2, bool b)
        {
            ForceClose();
        }
    }
}