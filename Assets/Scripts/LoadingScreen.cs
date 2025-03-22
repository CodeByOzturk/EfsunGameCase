using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text progressText;
    public void Show()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
    }

    public void UpdateProgress(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
            progressText.text = $"{(progress * 100):0}%";
        }

    }
}
