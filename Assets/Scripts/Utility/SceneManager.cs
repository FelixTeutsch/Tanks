using System;
using UnityEngine;

namespace Utility
{
    public enum EScene
    {
        MainMenu,
        PlayerAdd,
        Settings,
        Game,
        Shop,
        GameOver
    }

    public static class SceneManager
    {
        public static event Action<EScene> OnSceneLoaded;
        public static event Action<EScene> OnSceneUnloaded;

        public static void LoadScene(EScene scene)
        {
            Debug.Log($"Loading scene: {scene}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene.ToString());
            OnSceneLoaded?.Invoke(scene);
            Debug.Log($"Scene loaded: {scene}");
        }

        // appearntly not supported. Consider removing this method
        private static void LoadSceneAndUnloadPrevious(EScene scene)
        {
            Debug.Log($"Loading scene and unload previous scene: {scene}");
            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            Debug.Log($"Current scene: {currentScene.name}");
            LoadScene(scene);
            UnloadScene((EScene)Enum.Parse(typeof(EScene), currentScene.name));
            Debug.Log($"Scene loaded and previous scene unloaded: {scene}");
        }

        public static void ReloadCurrentScene()
        {
            Debug.Log("Reloading current scene");
            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            Debug.Log("Current scene: " + currentScene.name);
            LoadScene((EScene)Enum.Parse(typeof(EScene), currentScene.name));
            Debug.Log("Current scene reloaded");
        }

        private static void UnloadScene(EScene scene)
        {
            Debug.Log($"Unloading scene: {scene}");
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene.ToString());
            OnSceneUnloaded?.Invoke(scene);
            Debug.Log($"Scene unloaded: {scene}");
        }

        public static EScene GetCurrentScene()
        {
            Debug.Log("Getting name of current scene");
            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            Debug.Log($"Current scene: {currentScene}");
            var scene = (EScene)Enum.Parse(typeof(EScene), currentScene.name);
            Debug.Log($"Enum name of scene: {scene}");
            return scene;
        }
    }
}