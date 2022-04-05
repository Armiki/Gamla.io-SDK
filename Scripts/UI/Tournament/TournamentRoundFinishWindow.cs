using System;
using System.Linq;
using Gamla.Scripts.Common.UI;
using Gamla.Scripts.Data;
using Gamla.Scripts.UI.Main;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Scripts.UI.Tournament
{
    public class TournamentRoundFinishWindow : GUIView
    {
        [SerializeField] private Text _roundNumTitle;
        [SerializeField] private Text _roundNumSub;
        
        [SerializeField] private UserProfileWidget _user1;
        [SerializeField] private Text _pointUser1;
        [SerializeField] private Text _place1;
        
        [SerializeField] private UserProfileWidget _user2;
        [SerializeField] private Text _pointUser2;
        [SerializeField] private Text _place2;
        
        [SerializeField] private Text _entryFee;
        [SerializeField] private Image _entryIcon;
        [SerializeField] private Text _matchId;
        [SerializeField] private Button _copy;
        
        [SerializeField] private Button _playNext;
        [SerializeField] private Button _share;

        public void Init()
        {
            _roundNumTitle.text = "Round {0} status";
            _roundNumSub.text = "Round {0}";
        }
    }
}