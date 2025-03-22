using UnityEngine;
using UnityEngine.UI;

public class MusicControlUI : MonoBehaviour
{
    [SerializeField] private MusicManager musicManager; // Müzik yöneticisini bağlayın.
    [SerializeField] private Button playButton; // Müzik başlatma butonu
    [SerializeField] private Button stopButton; // Müzik durdurma butonu
    [SerializeField] private Button pauseButton; // Müzik duraklatma/başlatma butonu

    private void Start()
    {
        //playButton.onClick.AddListener(musicManager.PlayMusic);
        //stopButton.onClick.AddListener(musicManager.StopMusic);
        //pauseButton.onClick.AddListener(musicManager.TogglePauseMusic);
    }
}
