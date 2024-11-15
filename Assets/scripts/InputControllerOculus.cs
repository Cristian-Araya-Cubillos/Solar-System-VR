using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;

public class InputControllerOculus : MonoBehaviour
{
    public Transform rightHand;  // Asigna la mano derecha del Oculus en el inspector
    public float raycastDistance = 500f;
    public LayerMask interactionLayer;  // Para determinar las capas con las que el raycast puede interactuar

    private OVRInput.Controller controller = OVRInput.Controller.RTouch;  // Controlador derecho de Oculus
    private Transform lastHitObject;  // Para almacenar el último objeto detectado por el raycast

    void Update()
    {
        // Llamamos a la función que lanza el raycast y maneja la interacción con los objetos
        HandleRaycastWithController();
    }

    // Maneja el raycast y la interacción a través del controlador derecho de Oculus
    void HandleRaycastWithController()
    {
        // Obtener la posición y dirección del controlador derecho
        Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(controller);
        Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(controller);
        Vector3 forwardDirection = controllerRotation * Vector3.forward;

        // Crear el rayo desde el controlador
        Ray ray = new Ray(controllerPosition, forwardDirection);
        RaycastHit hit;

        // Lanzar el raycast
        if (Physics.Raycast(ray, out hit, raycastDistance, interactionLayer))
        {
            // Si el raycast golpea un objeto interactuable
            Transform hitObject = hit.transform;
            UIManager.instance.SetHighlightedObjectName(hitObject.name);  // Mostrar el nombre del objeto en la UI
            UIManager.instance.SetLabelNextToObject(hitObject.name);  // Posicionar una etiqueta al lado del objeto

            // Verificar si el objeto detectado es un nuevo objeto
            if (lastHitObject != hitObject)
            {
                lastHitObject = hitObject;  // Actualizamos el último objeto golpeado
            }

            // Interactuar con el objeto cuando se presiona el gatillo del controlador
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                UIManager.instance.SwitchToDetailedPlanetView(hitObject.name);  // Cambiar a la vista detallada del planeta
            }
        }
        else
        {
            // Si no se golpea ningún objeto, desactivamos las etiquetas o información mostrada
            UIManager.instance.DisablePlanetLabel();
            lastHitObject = null;
        }
    }
}
