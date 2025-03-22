using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Müzik için kullanılacak AudioSource bileşeni


    private void Start()
    {
        PlayMusic();
    }

    // Müziği başlatma fonksiyonu
    public void PlayMusic()
    {
        if (audioSource != null)
        {
            audioSource.loop = true; // Müzik tekrar çalsın
            audioSource.Play(); // Müziği çalmaya başla
        }
    }

    // Müziği durdurma fonksiyonu
    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Müziği duraklatma ve devam ettirme fonksiyonu
    public void TogglePauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause(); // Müziği duraklat
        }
        else
        {
            audioSource.UnPause(); // Müziği devam ettir
        }
    }
}
