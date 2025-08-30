using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

// This class lives throughout the entire lifetime of the application and takes care of reloading the game scene
// when the game is won or lost.
public static class Game
{
    public static event Action OnGameLost;
    public static event Action OnGameWon; 
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        OnGameLost = null;
        OnGameWon = null;
        
        InputSystem.actions.Enable();
    }

    public static void ReportGameLost()
    {
        OnGameLost?.Invoke();
        ReloadAsync().Run();
    }
    
    public static void ReportGameWon()
    {
        OnGameWon?.Invoke();
        ReloadAsync().Run();
    }

    private static async Awaitable ReloadAsync()
    {
        OnGameLost = null;
        OnGameWon = null;
        
        Scene gameScene = SceneManager.GetActiveScene();
        string gameSceneName = gameScene.name;
        
        // Load the transition scene
        await SceneManager.LoadSceneAsync("TransitionScene", LoadSceneMode.Additive);
        Scene transitionScene = SceneManager.GetSceneByName("TransitionScene");
        
        RuntimeManager.PlayOneShot("event:/GameStop");
        
        Transition transition = Object.FindFirstObjectByType<Transition>();
        await transition.FadeOut();
        
        // Reload game scene
        await SceneManager.UnloadSceneAsync(gameScene);
        await SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);
        gameScene = SceneManager.GetSceneByName(gameSceneName);
        SceneManager.SetActiveScene(gameScene);
        
        await transition.FadeIn();
        await SceneManager.UnloadSceneAsync(transitionScene);
    }
}