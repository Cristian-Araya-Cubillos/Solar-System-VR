using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Video360Controller : MonoBehaviour
{
    // Variables públicas para asignar desde el Inspector
    public Button[] playButtons;          // Array de botones que el usuario puede presionar
    public string[] sceneNames;           // Array de nombres de escenas a cargar

    private int currentIndex = -1;        // Variable para almacenar el índice del video actual

    void Start()
    {
        // Asignar la función OnPlayButtonPressed a cada botón
        for (int i = 0; i < playButtons.Length; i++)
        {
            int index = i; // Variable para capturar el índice en el bucle
            playButtons[i].onClick.AddListener(() => OnPlayButtonPressed(index));
        }
    }

    void OnPlayButtonPressed(int index)
    {
        // Cargar la siguiente escena según el índice del video
        if (currentIndex >= 0 && currentIndex < sceneNames.Length)
        {
            SceneManager.LoadScene(sceneNames[currentIndex]);
        }
    }
}
