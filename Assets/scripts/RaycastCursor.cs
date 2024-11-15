using UnityEngine;
using UnityEngine.XR;

public class RaycastCursor : MonoBehaviour
{
    public LayerMask interactableLayer; // Capa para los objetos con los que el raycast puede interactuar
    public float raycastDistance = 10.0f; // Distancia del rayo
    public Transform rightHandController; // El controlador de la mano derecha (asigna esto en el inspector)

    private LineRenderer lineRenderer; // Para visualizar el rayo
    private InputDevice rightHandDevice;
    
    void Start()
    {
        // Inicializar el LineRenderer si se quiere visualizar el rayo
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = true;

        // Obtener el dispositivo del controlador derecho
        rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    void Update()
    {
        CastRayFromController();
    }

    void CastRayFromController()
    {
        // Crear el rayo desde el controlador
        Ray ray = new Ray(rightHandController.position, rightHandController.forward);
        RaycastHit hit;

        // Visualizar el rayo
        lineRenderer.SetPosition(0, rightHandController.position);
        lineRenderer.SetPosition(1, rightHandController.position + rightHandController.forward * raycastDistance);

        // Lanzar el raycast
        if (Physics.Raycast(ray, out hit, raycastDistance, interactableLayer))
        {
            // El raycast ha golpeado algo en la capa interactuable
            Debug.Log("Golpeó: " + hit.collider.name);

            // Simula una interacción (por ejemplo, cuando presionas el botón "gatillo" del controlador)
            bool triggerPressed;
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed) && triggerPressed)
            {
                Debug.Log("Interacción con: " + hit.collider.name);
                InteractWithObject(hit.collider.gameObject);
            }
        }
    }

    // Función para manejar la interacción con el objeto
    void InteractWithObject(GameObject obj)
    {
        // Aquí puedes definir lo que pasa cuando el rayo interactúa con un objeto
        // Por ejemplo, mostrar un menú, activar un botón, etc.
        Debug.Log("Interacción con el objeto: " + obj.name);
    }
}
