using UnityEngine;
using UnityEngine.UI;

namespace Gamla.UI
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