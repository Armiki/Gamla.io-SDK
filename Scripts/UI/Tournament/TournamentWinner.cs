using UnityEngine;
using UnityEngine.UI;
using Gamla.Data;

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
        
        public void SetWinner(ServerPlayerMatch user)
        {
            _user.Init(user);
            _userPoints.text = user.score;
        }
    }
}