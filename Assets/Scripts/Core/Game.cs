using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

// This class lives throughout the entire lifetime of the application and takes care of reloading the game scene
// when the game is won or lost.
public static class Game
{
    public static Level currentLevel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        InputSystem.actions.Enable();
    }

    public static void ReloadLevel()
    {
        ReloadAsync().Run();
    }

    private static async Awaitable ReloadAsync()
    {
        Scene gameScene = SceneManager.GetActiveScene();
        string gameSceneName = gameScene.name;
        
        // Load the transition scene
        await SceneManager.LoadSceneAsync("TransitionScene", LoadSceneMode.Additive);
        Scene transitionScene = SceneManager.GetSceneByName("TransitionScene");
        
        RuntimeManager.PlayOneShot("event:/Game Flow/ScreenFadeOut");
        
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