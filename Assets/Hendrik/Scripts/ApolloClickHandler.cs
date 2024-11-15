using UnityEngine;
using UnityEngine.SceneManagement;

public class ApolloClickHandler : MonoBehaviour
{
    public Camera mainCamera; // Asigna la cámara principal en el Inspector
    public string sceneName; // Nombre de la escena a cargar
    public AudioSource audioSource; // Asigna el AudioSource en el Inspector
    private bool isSceneChanging = false; // Controla si el cambio de escena ya ha comenzado

    private void Update()
    {
        // Si ya estamos cambiando la escena, no permitir más interacciones
        if (isSceneChanging)
            return;

        // Verifica si el usuario hace clic con el botón izquierdo del mouse
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // Crea un rayo desde la posición del mouse
            RaycastHit hit;

            // Si el rayo colisiona con el objeto (Apollo 1), ejecuta el código
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject) // Verifica que el objeto colisionado es el Apollo 1
                {
                    // Iniciar la rutina para reproducir el sonido y cambiar de escena
                    StartCoroutine(PlaySoundAndChangeScene());
                }
            }
        }
    }

    // Corrutina que reproduce el sonido y espera 3 segundos antes de cambiar de escena
    private System.Collections.IEnumerator PlaySoundAndChangeScene()
    {
        isSceneChanging = true; // Marca que el cambio de escena ha comenzado
        
        // Reproducir el sonido si hay un AudioSource asignado
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Esperar la duración del sonido antes de cambiar la escena, o al menos 3 segundos si es más corto
        float delay = audioSource != null ? Mathf.Max(3f, audioSource.clip.length) : 3f;
        yield return new WaitForSeconds(delay);

        // Cambiar a la nueva escena
        SceneManager.LoadScene(sceneName);
    }
}
