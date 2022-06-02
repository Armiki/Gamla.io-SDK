using System;
using System.Collections.Generic;
using System.Linq;
using Gamla.Data;
using Gamla.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
{
    public class NewGameWindow : GUIView
    {
        public const float TitleHeight = 10;
        
        public event Action<BattleInfo> onPlayGameClick;
        
        //[SerializeField] private RectTransform _tournamentTitle;
        [SerializeField] private RectTransform _hardPvpTitle;
        [SerializeField] private RectTransform _softPvpTitle;
        
        //[SerializeField] private LayoutElement _tournamentLayoutElement;
        [SerializeField] private LayoutElement _hardPvpLayoutElement;
        [SerializeField] private LayoutElement _softPvpLayoutElement;
        
        //[SerializeField] private RectTransform _tournamentContent;
        [SerializeField] private RectTransform _hardPvpContent;
        [SerializeField] private RectTransform _softPvpContent;
        
        [SerializeField] private RectTransform _allContent;

        [SerializeField] private Button[] _filtersBtn;
        
        [SerializeField] private RecolorItem[] _filtersRecolor;
        [SerializeField] private RecolorItem[] _filtersRecolorTxt;
        
        [SerializeField] private NewGameWidget _battlePrefab;
        private List<NewGameWidget> _battleWidgets = new List<NewGameWidget>();

        public void Start()
        {
            foreach (var btn in _filtersBtn)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    FilterClick(btn.gameObject);
                });
            }
        }

        public void Init(List<BattleInfo> battles)
        {
            SetSimpleData(LocalState.newBattleMatches);
            FilterClick(_filtersBtn[0].gameObject);
            Show();
            
            ServerCommand.GetPossibleMatchesBet(realBattles =>
            {
                SetSimpleData(realBattles);
            });
        }

        public override void OnEnable()
        {
            base.OnEnable();
            EnableButtons(true);
        }
        
        public void EnableButtons(bool enable)
        {
            foreach (var button in _battleWidgets)
            {
                button.SetInteractible(enable);
            }
        }

        void SetSimpleData(List<BattleInfo> battles)
        {
            foreach (var existTournament in _battleWidgets)
            {
                if (existTournament != null) {
                    Destroy(existTournament.gameObject);
                }
            }

            _battleWidgets.Clear();

            var tournaments = battles.Where(x => x.type == BattleType.Tournament).ToList();
            var hardBattles = battles.Where(x => x.type == BattleType.HardPVP).ToList();
            var softBattles = battles.Where(x => x.type == BattleType.SoftPVP).ToList();

            //var t = FillContent(_tournamentTitle.gameObject, _tournamentContent, _tournamentLayoutElement, tournaments);
            if (_hardPvpTitle == null || _softPvpTitle == null || _allContent == null)
            {
                return;
            }

            var h = FillContent(_hardPvpTitle.gameObject, _hardPvpContent, _hardPvpLayoutElement, hardBattles);
            var s = FillContent(_softPvpTitle.gameObject, _softPvpContent, _softPvpLayoutElement, softBattles);

            _allContent.sizeDelta = new Vector2(_allContent.sizeDelta.x, //t + _tournamentTitle.sizeDelta.y + 30 +
                                                                         h + _hardPvpTitle.sizeDelta.y + 30 +
                                                                         s + _softPvpTitle.sizeDelta.y + 30);
        }

        float FillContent(GameObject title, RectTransform content, LayoutElement layout, List<BattleInfo> filteredBattles)
        {
            if (!filteredBattles.Any())
            {
                title.SetActive(false);
                content.gameObject.SetActive(false);
                return 0;
            }
            title.SetActive(true);
            content.gameObject.SetActive(true);
            var height = (filteredBattles.Count * _battlePrefab.rect.sizeDelta.y + filteredBattles.Count * 30);
            content.sizeDelta = new Vector2(content.sizeDelta.x, height);
            layout.preferredHeight = height;
            List<TutorialElement> _elements = null;
            
            foreach (var data in filteredBattles)
            {
                var item = Instantiate(_battlePrefab, content);
                item.Init(data);
                item.onPlayGame += () =>
                {
                    EnableButtons(false);
                    onPlayGameClick?.Invoke(data);
                };
                _battleWidgets.Add(item);

                if (TutorialManager.Instance.CurrentStepName == "tutorial-1-2" && data.type == BattleType.SoftPVP && data.entry.type == CurrencyType.Z &&
                    data.win.type == CurrencyType.Z && data.entry.amount == 1.0f)
                {
                    var element = item.gameObject.GetComponent<TutorialElement>();
                    element.TutorialStepName = "tutorial-1-2";
                    if (_elements == null)
                    {
                        _elements = new List<TutorialElement>(2);
                    }
                    _elements.Add(element);
                }
            }

            if (_elements != null && _elements.Count > 0)
            {
                TutorialManager.Instance.TryActivateStepWithElements(_elements);
            }
            
            return height;
        }
        
        void FilterClick(GameObject switchFilter)
        {
            for (int i = 0; i < _filtersBtn.Length; i++)
            {
                if (switchFilter == _filtersBtn[i].gameObject)
                {
                    _filtersRecolor[i].recolorFilter = "filterOnColor";
                    _filtersRecolor[i].Recolor();
                    _filtersRecolorTxt[i].recolorFilter = "textColorSecondary";
                    _filtersRecolorTxt[i].Recolor();
                }
                else
                {
                    _filtersRecolor[i].recolorFilter = "filterOffColor";
                    _filtersRecolor[i].Recolor();
                    _filtersRecolorTxt[i].recolorFilter = "textColorTertiary";
                    _filtersRecolorTxt[i].Recolor();
                }
            }
        }
    }
}