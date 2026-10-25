using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private float loadingDuration = 5f;
    [SerializeField] private string fallbackSceneName = "MainMenu";

    private void Start()
    {
        if (progressSlider == null)
            progressSlider = GetComponentInChildren<Slider>();

        if (progressSlider == null)
        {
            Debug.LogError("LoadingSceneManager requires a Slider reference.", this);
            return;
        }

        StartCoroutine(LoadTargetScene());
    }

    private IEnumerator LoadTargetScene()
    {
        string targetSceneName = global::SceneManager.PendingSceneName;
        if (string.IsNullOrWhiteSpace(targetSceneName))
            targetSceneName = fallbackSceneName;

        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError(
                $"Scene '{targetSceneName}' cannot be loaded. " +
                "Make sure it has been added to Build Settings.",
                this);
            yield break;
        }

        progressSlider.minValue = 0f;
        progressSlider.maxValue = 1f;
        progressSlider.value = 0f;

        AsyncOperation sceneLoad = UnitySceneManager.LoadSceneAsync(targetSceneName);
        if (sceneLoad == null)
        {
            Debug.LogError($"Could not start loading scene '{targetSceneName}'.", this);
            yield break;
        }

        sceneLoad.allowSceneActivation = false;

        float duration = Mathf.Max(0.01f, loadingDuration);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            progressSlider.value = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        progressSlider.value = 1f;
        while (!sceneLoad.isDone && sceneLoad.progress < 0.9f)
            yield return null;

        sceneLoad.allowSceneActivation = true;
    }
}
