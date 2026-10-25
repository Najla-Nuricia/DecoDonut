using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameplaySceneName = "Gameplay";
    [SerializeField] private string loadingSceneName = "Loading";

    public static string PendingSceneName { get; private set; }

    public void LoadMainMenu()
    {
        LoadSceneWithLoading(mainMenuSceneName);
    }

    public void LoadGameplay()
    {
        LoadSceneWithLoading(gameplaySceneName);
    }

    public void ReloadCurrentScene()
    {
        LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string sceneName)
    {
        if (!CanLoadScene(sceneName))
            return;

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    private void LoadSceneWithLoading(string sceneName)
    {
        if (!CanLoadScene(sceneName))
            return;

        if (!CanLoadScene(loadingSceneName))
        {
            Debug.LogError(
                $"Loading scene '{loadingSceneName}' cannot be loaded. " +
                "Make sure it has been added to Build Settings.",
                this);
            return;
        }

        PendingSceneName = sceneName;
        UnityEngine.SceneManagement.SceneManager.LoadScene(loadingSceneName);
    }

    private bool CanLoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("Nama scene tidak boleh kosong.", this);
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"Scene '{sceneName}' tidak dapat dimuat. Pastikan scene sudah ditambahkan ke Build Settings.",
                this);
            return false;
        }

        return true;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
