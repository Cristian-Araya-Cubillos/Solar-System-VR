using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Para interactuar con el botón

public class CambiadorAutomatico : MonoBehaviour
{
    public Button changeSceneButton; // Asigna el botón desde el Inspector

    private void Start()
    {
    
    }

    public void intento(string Scena){
         SceneManager.LoadScene(Scena);
    }
}
