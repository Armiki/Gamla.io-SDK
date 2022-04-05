using UnityEngine;
using UnityEngine.UI;
using Gamla.Scripts.UI.Main;

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