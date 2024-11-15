using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI; // Necesario para el manejo de UI

public class NewBehaviourScript : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Asigna tu VideoPlayer en el Inspector
    public RawImage rawImage; // Asigna tu RawImage en el Inspector
    public string sceneName; // Nombre de la escena a cargar

    private void Start()
    {
        // Asegúrate de que el Raw Image esté oculto al inicio
        rawImage.gameObject.SetActive(false);
    }
        
    // Método para ser llamado al presionar el botón
    public void PlayVideoAndLoadScene()
    {
        StartCoroutine(PlayVideoAndLoadSceneCoroutine());
    }

    private IEnumerator PlayVideoAndLoadSceneCoroutine()
    {
        // Asegúrate de que el Raw Image sea visible
        rawImage.gameObject.SetActive(true);

        // Reproduce el video
        videoPlayer.Play();

        yield return new WaitForSeconds(6f);

        // Carga la nueva escena
        SceneManager.LoadScene(sceneName);
    }

    // Método para ocultar el Raw Image después de que se reproduce el video
    private void HideVideo()
    {
        rawImage.gameObject.SetActive(false);
    }
}
