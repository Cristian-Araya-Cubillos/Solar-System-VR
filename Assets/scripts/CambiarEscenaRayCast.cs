using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscenaRayCastOculus : MonoBehaviour
{
    [SerializeField] private float raycastDistance; // Longitud del raycast
    [SerializeField] private string[] nombresObjetos; // Array con los nombres de los objetos
    [SerializeField] private string[] nombresEscenas; // Array con los nombres de las escenas

    void Update()
    {
        // Detectar si se presionó el gatillo de cualquiera de los dos controles
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            RealizarRaycast();
        }
    }

    // Realiza el raycast y detecta el objeto en el que se apunta
    private void RealizarRaycast()
    {
        // Crear un raycast desde la cámara principal (usualmente el punto de vista del usuario)
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            // Obtener el nombre del objeto detectado
            string nombreObjeto = hit.collider.gameObject.name;
            Debug.Log("Has apuntado al objeto: " + nombreObjeto);

            // Buscar la escena asociada y cambiarla
            string nombreEscena = BuscarEscenaAsociada(nombreObjeto);
            if (!string.IsNullOrEmpty(nombreEscena))
            {
                CambiarEscenaPorNombre(nombreEscena);
            }
            else
            {
                Debug.LogError("No se encontró una escena asociada para el objeto: " + nombreObjeto);
            }
        }
        else
        {
            Debug.Log("No se detectó ningún objeto.");
        }
    }

    // Busca la escena asociada al nombre del objeto en el array
    private string BuscarEscenaAsociada(string nombreObjeto)
    {
        for (int i = 0; i < nombresObjetos.Length; i++)
        {
            if (nombresObjetos[i] == nombreObjeto)
            {
                return nombresEscenas[i];
            }
        }
        return null; // No se encontró una escena asociada
    }

    // Cambia a la escena especificada por nombre
    private void CambiarEscenaPorNombre(string nombreEscena)
    {
        if (Application.CanStreamedLevelBeLoaded(nombreEscena))
        {
            Debug.Log("Cambiando a la escena: " + nombreEscena);
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogError("La escena '" + nombreEscena + "' no existe o no está en los Build Settings.");
        }
    }
}
