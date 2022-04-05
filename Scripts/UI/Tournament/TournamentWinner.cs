using System;
using System.Linq;
using Gamla.Scripts.Common.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gamla.Scripts.Data;
using Gamla.Scripts.UI.Main;
using UnityEngine.EventSystems;

namespace GamlaSDK.Scripts.UI.Game
{
    public class TournamentWinner : MonoBehaviour
    {
        [SerializeField] private UserProfileWidget _user;
        [SerializeField] private Text _userPoints;

        public void SetClear()
        {
            _user.Clear();
            _userPoints.text = "";
        }
    }
}