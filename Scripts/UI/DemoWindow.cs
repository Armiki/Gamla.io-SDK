using System;
using System.Linq;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Data;
using Gamla.Scripts.UI.Main;
using UnityEngine;
using UnityEngine.UI;
using Gamla.Scripts.UI.Profile;
using Gamla.Scripts.UI.Store;
using Gamla.Scripts.UI.Game;
using Gamla.Scripts.UI.Ladder;
using Gamla.Scripts.Logic;

namespace Project.Scripts.UI
{
    public class DemoWindow : GUIView
    {
        [SerializeField] private Dropdown _selectColor;
        [SerializeField] private Button _mainWindowBtn;
        [Header("Match end result")]
        [SerializeField] private Button _winWindowBtn;
        [SerializeField] private Button _loseWindowBtn;
        [SerializeField] private Button _lowBalanceeBtn;
        [SerializeField] private Button _lowSoftActiveDailyBtn;
        [SerializeField] private Button _lowSoftInActiveDailyBtn;
        [Header("Warnings")]
        [SerializeField] private Dropdown _selectWarning;
        [SerializeField] private Button _simpleWarning;
        [SerializeField] private Dropdown _selectInfo;
        [SerializeField] private Button _simpleInfo;
        [SerializeField] private Button _regionWarning;
        [SerializeField] private Button _internetWarning;

        [SerializeField] private Button _searchingOpponentsBtn;
        [SerializeField] private Button _ladderWindowBtn;
        [SerializeField] private Button _helpWindowBtn;
        [SerializeField] private Button _profileWindowBtn;
        [SerializeField] private Button _storeWindowBtn;
        [SerializeField] private Button _accountBalanceWindowBtn;

        [SerializeField] private Button _infoPanelWindow;
        [SerializeField] private Button _chooseLangWindow;

        [Header("SignUp")]
        [SerializeField] private Button _loadingWindowBtn;
        [SerializeField] private Button _signUpWindowBtn;
        [SerializeField] private Button _signUpAccountWindowBtn;

        private GUIWarningType _guiWarningType = (GUIWarningType)0;
        private GUIInfoType _guiInfoType = (GUIInfoType)0;

        public void Start()
        {
            _mainWindowBtn.onClick.RemoveAllListeners();
            _mainWindowBtn.onClick.AddListener(OpenMainWindow);
            
            _winWindowBtn.onClick.RemoveAllListeners();
            _winWindowBtn.onClick.AddListener(OpenWinWindow);
            _loseWindowBtn.onClick.RemoveAllListeners();
            _loseWindowBtn.onClick.AddListener(OpenLostWindow);
            _lowBalanceeBtn.onClick.RemoveAllListeners();
            _lowBalanceeBtn.onClick.AddListener(OpenLowBalanceWindow);
            _lowSoftActiveDailyBtn.onClick.RemoveAllListeners();
            _lowSoftActiveDailyBtn.onClick.AddListener(OpenLowSoftActiveRewardWindow);
            _lowSoftInActiveDailyBtn.onClick.RemoveAllListeners();
            _lowSoftInActiveDailyBtn.onClick.AddListener(OpenLowSoftInactiveRewardWindow);
            
            _simpleWarning.onClick.RemoveAllListeners();
            _simpleWarning.onClick.AddListener(OpenSimpleWarningWindow);

            _simpleInfo.onClick.RemoveAllListeners();
            _simpleInfo.onClick.AddListener(OpenSimpleInfoWindow);

            _selectColor.ClearOptions();
            _selectColor.AddOptions(GUIConstants.colorTemplate.colors.Select(x => x.name).ToList());

            _selectColor.onValueChanged.AddListener(delegate
            {
                DropdownValueChanged(_selectColor);
            });
            LocalState.selectColorTemplate = GUIConstants.colorTemplate.colors[0].name;


            _selectWarning.ClearOptions();
            _selectWarning.AddOptions(Enum.GetNames(typeof(GUIWarningType)).ToList());

            _selectWarning.onValueChanged.AddListener(delegate
            {
                ChooseWarningChanged(_selectWarning);
            });

            _selectInfo.ClearOptions();
            _selectInfo.AddOptions(Enum.GetNames(typeof(GUIInfoType)).ToList());

            _selectInfo.onValueChanged.AddListener(delegate
            {
                ChooseInfoChanged(_selectInfo);
            });

            _searchingOpponentsBtn.onClick.RemoveAllListeners();
            _searchingOpponentsBtn.onClick.AddListener(OpenSearchingOpponentWindow);

            _ladderWindowBtn.onClick.RemoveAllListeners();
            _ladderWindowBtn.onClick.AddListener(OpenLadderWindow);

            _helpWindowBtn.onClick.RemoveAllListeners();
            _helpWindowBtn.onClick.AddListener(OpenHelpWindow);

            _profileWindowBtn.onClick.RemoveAllListeners();
            _profileWindowBtn.onClick.AddListener(OpenProfileWindow);

            _storeWindowBtn.onClick.RemoveAllListeners();
            _storeWindowBtn.onClick.AddListener(OpenStoreWindow);

            _accountBalanceWindowBtn.onClick.RemoveAllListeners();
            _accountBalanceWindowBtn.onClick.AddListener(OpenAccountBalanceWindow);

            _signUpWindowBtn.onClick.RemoveAllListeners();
            _signUpWindowBtn.onClick.AddListener(OpenSignUpWindow);
            _signUpAccountWindowBtn.onClick.RemoveAllListeners();
            _signUpAccountWindowBtn.onClick.AddListener(OpenSignUpAccountWindow);
            _loadingWindowBtn.onClick.RemoveAllListeners();
            _loadingWindowBtn.onClick.AddListener(OpenLoadingWindow);
            _infoPanelWindow.onClick.RemoveAllListeners();
            _infoPanelWindow.onClick.AddListener(OpenInfoPanelWindow);
            _chooseLangWindow.onClick.RemoveAllListeners();
            _chooseLangWindow.onClick.AddListener(OpenChooseLangWindow);
        }
        
        void DropdownValueChanged(Dropdown change)
        {
            LocalState.selectColorTemplate = GUIConstants.colorTemplate.colors[change.value].name;
        }

        void ChooseWarningChanged(Dropdown change)
        {
            _guiWarningType = (GUIWarningType)change.value;
        }

        void ChooseInfoChanged(Dropdown change)
        {
            _guiInfoType = (GUIInfoType)change.value;
        }

        void OpenMainWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/GameMainWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<GameMainWindow>();
            window.Init(LocalState.currentGame);
        }
        
        void OpenWinWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/FinishWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<FinishWindow>();
            //window.Init(GameStatus.Win, MockData.current_game.simpleGame, MockData.current_user);
        }
        
        void OpenLostWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/FinishWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<FinishWindow>();
           // window.Init(GameStatus.Lost, MockData.current_game.simpleGame, MockData.current_user);
        }
        
        void OpenLowBalanceWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/BattleErrorWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<BattleErrorWindow>();
            window.Show();
            window.Init(false, true);
        }
        
        void OpenLowSoftActiveRewardWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/BattleErrorWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<BattleErrorWindow>();
            window.Show();
            window.Init(true, false);
        }
        
        void OpenLowSoftInactiveRewardWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/BattleErrorWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<BattleErrorWindow>();
            window.Show();
            window.Init(false, false);
        }
        
        void OpenSimpleWarningWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/WarningWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<WarningWindow>();
            window.Show();
            window.Init(_guiWarningType);
        }

        void OpenSimpleInfoWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/InfoWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<InfoWindow>();
            window.Show();
            window.Init(_guiInfoType);
        }

        void OpenSearchingOpponentWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/SearchingOpponentsWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<SearchingOpponentsWindow>();
            window.Show();
        }

        void OpenLadderWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/LadderWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<LadderWindow>();
            window.Init( LocalState.currentGame, LocalState.currentUser);
        }

        void OpenHelpWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/HelpWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<HelpWindow>();
            window.Show();
        }

        void OpenProfileWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ProfileWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<ProfileWindow>();
            window.Show();
        }

        void OpenStoreWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/StoreWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<StoreWindow>();
            window.Show();
        }

        void OpenLoadingWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/LoadingWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<LoadingWindow>();
            window.Show();
        }

        void OpenSignUpWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/SignUpWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<SignUpWindow>();
            window.Show();
        }

        void OpenSignUpAccountWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/SignUpAccountWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<SignUpAccountWindow>();
            window.Show();
        }

        void OpenAccountBalanceWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/AccountBalanceWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<AccountBalanceWindow>();
            //window.Init();
            
        }

        void OpenInfoPanelWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/InfoPanelWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<InfoPanelWindow>();
            window.Show();
        }

        void OpenChooseLangWindow()
        {
            var window =
                Instantiate(GamlaResourceManager.GamlaResources.GetResource("Windows/ChooseLangWindow"),
                    GamlaResourceManager.windowsContainer).GetComponent<ChooseLangWindow>();
            window.Show();
        }
    }
}