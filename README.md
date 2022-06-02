# Gamla.io-SDK
Gamla.io - Gamers community with skill base games



##Unity3D Integration
1. Get your AppID(GameID) from developer.gamla.io
2. Open UnityProjectFolder/Packages/manifest.json
3. Add new dependencies:
   ####"io.gamla.sdk": "https://github.com/Armiki/Gamla.io-SDK.git"
4. Open Unity Project in Editor
5. In auto opened Gamla Settings Window enter your AppID(GameID)


##API function
####Open Gamla UI:
GamlaSDK auto inited when project started. You can now launch it to take over control of your interface. Typically, GamlaSDK is launched from your game's start screen. Specifically, we recommend that your game's start screen has a button to launch GamlaSDK when it is clicked.

    GamlaService.Open.Push("firebase_push_token");
    
####Listen Match start event:
Called when a player chooses a tournament and the match countdown expires
    
    GamlaService.OnMatchStarted.Subscribe((s1,s2,b1) => matchId(string), matchData(json), isTournament);


####Submit Score:
You must push updeted score after changed score in gameplay or gameUI 

    GamlaService.UpdateMatchScore.Push(score);

####GamePlay match end and finall send score:
To ensure that both asynchronous and synchronous gameplay proceeds optimally, the score for the match should be submitted immediately using MatchEnd when the match ends, regardless of any remaining player input necessary to reach the match results screen. The score submission should be handled in the background without any explicit action by the player necessary to proceed.

    GamlaService.MatchEnd.Push(score);

####Return to Gamla UI:
After match end and final score sended

    GamlaService.ReturnToGamla.Push();
    