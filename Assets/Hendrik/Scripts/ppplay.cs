using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ppplay : MonoBehaviour
{
    public string sceneName; // Asigna el nombre de la escena a cargar en el Inspector
    public float Sec;
    // Start is called before the first frame update
    void Start()
    {
        // Iniciar la corutina para cambiar de escena
        StartCoroutine(ChangeSceneAfterDelay());
    }

    private IEnumerator ChangeSceneAfterDelay()
    {
        // Esperar 6 segundos
        yield return new WaitForSeconds(Sec);

        // Cambiar a la nueva escena
        SceneManager.LoadScene(sceneName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
