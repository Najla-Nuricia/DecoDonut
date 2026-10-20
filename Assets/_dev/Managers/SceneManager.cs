using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameplaySceneName = "Gameplay";

    public void LoadMainMenu()
    {
        LoadScene(mainMenuSceneName);
    }

    public void LoadGameplay()
    {
        LoadScene(gameplaySceneName);
    }

    public void ReloadCurrentScene()
    {
        LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("Nama scene tidak boleh kosong.", this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"Scene '{sceneName}' tidak dapat dimuat. Pastikan scene sudah ditambahkan ke Build Settings.",
                this);
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
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
