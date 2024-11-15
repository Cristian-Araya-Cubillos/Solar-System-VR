using UnityEngine;

public class PlanetRaycast : MonoBehaviour
{
    public LayerMask planetLayer; // Asegúrate de asignar los planetas a una capa
    public Transform rayOrigin; // Punto de inicio del rayo, puede ser la posición del controlador
    public float rayDistance = 100f;

    void Update()
    {
        RaycastHit hit;
        // Lanza el raycast desde el controlador
        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, rayDistance, planetLayer))
        {
            // Verifica si lo que golpea es un planeta
            if (hit.collider != null)
            {
                // Muestra el nombre del planeta al que apunta
                Debug.Log("Planeta seleccionado: " + hit.collider.gameObject.name);

                // Aquí puedes agregar lógica adicional, como seleccionar el planeta o ejecutar una acción
                if (Input.GetButtonDown("Fire1")) // O alguna otra acción que dispares con el controlador
                {
                    SelectPlanet(hit.collider.gameObject);
                }
            }
        }
    }

    void SelectPlanet(GameObject planet)
    {
        // Lógica para seleccionar el planeta o interactuar con él
        Debug.Log("Has seleccionado el planeta: " + planet.name);
        // Puedes implementar más funcionalidades aquí (abrir menú, cambiar escena, etc.)
    }
}
