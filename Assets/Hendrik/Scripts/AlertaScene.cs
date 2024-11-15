using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI; // Necesario para el manejo de UI

public class AlertaScene : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Asigna tu VideoPlayer en el Inspector
    public RawImage rawImage; // Asigna tu RawImage en el Inspector

    private void Start()
    {
        // Asegúrate de que el Raw Image esté oculto al inicio
        rawImage.gameObject.SetActive(false);

        // Asignar el método para ocultar el RawImage cuando termine el video
        videoPlayer.loopPointReached += OnVideoEnd;
    }
        
    // Método para ser llamado al presionar el botón
    public void PlayVideo()
    {
        // Inicia la corrutina para mostrar el video
        StartCoroutine(PlayVideoCoroutine());
    }

    private IEnumerator PlayVideoCoroutine()
    {
        // Asegúrate de que el Raw Image sea visible
        rawImage.gameObject.SetActive(true);

        // Reproduce el video
        videoPlayer.Play();

        // Esperar a que el video termine
        while (videoPlayer.isPlaying)
        {
            yield return null; // Espera cada frame hasta que el video termine
        }
    }

    // Método que se llama cuando el video termina
    private void OnVideoEnd(VideoPlayer vp)
    {
        // Ocultar el Raw Image cuando el video termina
        rawImage.gameObject.SetActive(false);
    }
}
