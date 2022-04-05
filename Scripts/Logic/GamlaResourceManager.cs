using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Data;
using Gamla.Scripts.UI.Main;
using UnityEngine.UI;

namespace Gamla.Scripts.Logic
{
    public class GamlaResourceManager : MonoBehaviour
    {
        [SerializeField] private UISafeAreaManager _safeAreaManager;
        [SerializeField] private TopBar _topBar;
        [SerializeField] private TabBar _tabBar;
        [SerializeField] private Image _backWindow;
        [SerializeField] private List<Sprite> _trophies;
        [SerializeField] private Sprite _defaultTrophie;
        [SerializeField] private Texture _rematchIcon;
        [SerializeField] private List<Sprite> _smiles;
        
        public static Transform windowsContainer => GameObject.FindGameObjectWithTag("Windows").transform;
        public static Transform tutorialContainer => GameObject.FindGameObjectWithTag("Tutorial").transform;
        public static Transform dynamicCanvas => GameObject.FindGameObjectWithTag("DynamicCanvas").transform;
        public static Transform root;
        
        private static GamlaResourceManager _gamlaResources;
        public static GamlaResourceManager GamlaResources => _gamlaResources;

        public static TopBar topBar => _gamlaResources._topBar;
        public static TabBar tabBar => _gamlaResources._tabBar;
        public static UISafeAreaManager safeAreaManager => _gamlaResources._safeAreaManager;
        public static Texture RematchIcon => _gamlaResources._rematchIcon;

        public static bool BackView
        {
            get => _gamlaResources._backWindow.gameObject.activeSelf;
            set => _gamlaResources._backWindow.gameObject.SetActive(value);
        }

        public static Sprite GetTrophyIcon(long id)
        {
            foreach (var trophy in _gamlaResources._trophies)
            {
                if (trophy.name == "trophies_" + id) return trophy;
            }
            return _gamlaResources._defaultTrophie;
        }

        public static Sprite GetSmiles(string name)
        {
            foreach (var img in _gamlaResources._smiles)
            {
                if (img.name == name) return img;
            }
            return _gamlaResources._smiles[0];
        }


        public static Canvas canvas
        {
            get
            {
                if (_canvas == null) {
                    _canvas = dynamicCanvas.GetComponent<Canvas>();
                }

                return _canvas;
            }
        }

        static Canvas _canvas;

        public void Awake()
        {
            root = transform;
            if (_gamlaResources == null) {
                _gamlaResources = this;
            }
            
            LocalState.selectColorTemplate = GUIConstants.colorTemplate.colors.First(x => x.use).name;
            var customSprite = GUIConstants.colorTemplate.colors.First(x => x.use).gameView;
            if (customSprite != null)
            {
                _backWindow.sprite = customSprite;
            }
            else
            {
                _backWindow.color = GUIConstants.colorTemplate.colors.First(x => x.use).viewPrimaryColor;
            }
        }

        public GameObject GetResource(string path)
        {
            var item = Resources.Load<GameObject>(path);
            return item;
        }
    }
}