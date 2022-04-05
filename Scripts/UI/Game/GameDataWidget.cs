using System;
using Gamla.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

public class GameDataWidget : MonoBehaviour
{
    private event Action onPlayGame;
        
    [SerializeField] private Button _play;
    [SerializeField] private Text _gameName;
    [SerializeField] private AvatarComponent _gameLogo;

    public void Start()
    {
        _play.onClick.RemoveAllListeners();
        _play.onClick.AddListener(()=> onPlayGame?.Invoke());
    }

    public void Init(GameAppInfo game)
    {
        _gameName.text = game.name;
        _gameLogo.Load("https://gamla.io" + game.image);
        onPlayGame += () =>
        {
#if UNITY_ANDROID
            Application.OpenURL(game.android_url);
#endif
            
#if UNITY_IPHONE
            Application.OpenURL(game.apple_url);
#endif
        };
    }
    
}
