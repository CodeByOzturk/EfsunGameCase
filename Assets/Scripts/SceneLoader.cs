using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UniRx;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private LoadingScreen _loadingScreen; // Inspector üzerinden atanacak
    private readonly ReactiveProperty<float> _progress = new(0f);

    [SerializeField] private string nextSceneName; // Yüklenmesi gereken sahne adı

    private void Start()
    {
        if (_loadingScreen == null)
        {
            Debug.LogError("LoadingScreen is not assigned in the Inspector!");
            return;
        }

        _loadingScreen.Show();
        _progress.Subscribe(progress => _loadingScreen.UpdateProgress(progress)).AddTo(this);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            LoadSceneAsync(nextSceneName).Forget();
        }
    }

    public async UniTask LoadSceneAsync(string sceneName)
    {
        float loadTime = 5f; // Toplam yükleme süresi (saniye)
        float startTime = Time.time;

        _loadingScreen.Show();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            // Geçen süreyi hesapla ve 5 saniyede %100 olacak şekilde ilerlet
            float elapsed = Time.time - startTime;
            float targetProgress = Mathf.Clamp01(elapsed / loadTime);

            _progress.Value = targetProgress; // Yükleme ekranında gösterilecek ilerleme

            if (elapsed >= loadTime)
            {
                asyncLoad.allowSceneActivation = true; // 5 saniye sonra sahneyi aç
            }

            await UniTask.Yield();
        }
    }

}
