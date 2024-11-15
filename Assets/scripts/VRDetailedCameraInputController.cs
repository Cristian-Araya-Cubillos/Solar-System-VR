using UnityEngine;

public class VRDetailedCameraInputController : MonoBehaviour
{
    public GameObject raycastOriginObject; // Objeto desde donde se lanza el raycast (puede ser un controlador de Oculus).
    public LayerMask interactionLayerMask; // Capa para detectar objetos interactivos.

    private double x = 30.0f;
    private double y = 20.0f;

    private bool isRotating;

    public float xSpeed = 10.0f;
    public float ySpeed = 10.0f;

    Quaternion rotation;

    [HideInInspector]
    public Vector3 position;

    float distance = 150;
    float zoomSpeed = 5;

    LineRenderer currentSelectedLineRenderer = null;

    // Usar para la inicialización
    void Start()
    {
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    // Update es llamado una vez por frame
    void LateUpdate()
    {
        // Acceder al componente Camera del objeto padre
        Camera cameraComponent = GetComponentInParent<Camera>();
        
        if (cameraComponent != null && cameraComponent.isActiveAndEnabled)
        {
            SetTheCameraPositionBasedOnInput();
            ObjectInfo();
        }
        else
        {
            Debug.LogWarning("La cámara del objeto padre no está activa o no se encontró.");
        }
    }


    void SetTheCameraPositionBasedOnInput()
    {
        var orbitSpeedInDaysPerSecond = ConfigManager.instance?.orbitSpeedInDaysPerSecond != null
            ? ConfigManager.instance.orbitSpeedInDaysPerSecond
            : DefaultValues.orbitSpeedInDaysPerSecond;

        rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler((float)y, (float)x, 0), Time.deltaTime * orbitSpeedInDaysPerSecond);

        position = rotation * new Vector3(0.0f, 0.0f, -distance);

        transform.rotation = rotation;
        transform.position = position;

        x = transform.eulerAngles.y;
        y = transform.eulerAngles.x;

        // Control de cámara usando el Oculus Touch
        if (OVRInput.Get(OVRInput.Touch.PrimaryTouchpad))
        {
            Vector2 touchpad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
            x += touchpad.x * xSpeed;
            y -= touchpad.y * ySpeed;
        }

        // Zoom utilizando el Oculus Touch (desplazamiento del pulgar)
        if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y > 0 && !ZoomLevelReached())
        {
            distance -= OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y * zoomSpeed;
        }
        else if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y < 0)
        {
            distance -= OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y * zoomSpeed;
        }
    }

    // Obtenemos información del objeto seleccionado mediante un Raycast desde el controlador del Oculus
    void ObjectInfo()
    {
        if (raycastOriginObject == null)
        {
            Debug.LogWarning("Raycast origin object is not assigned.");
            return;
        }

        Ray ray = new Ray(raycastOriginObject.transform.position, raycastOriginObject.transform.forward);
        var objectsHighlighted = Physics.RaycastAll(ray, 500, interactionLayerMask);
        var raycastHit = Physics.Raycast(ray, out RaycastHit hit, 500, interactionLayerMask);

        if (raycastHit)
        {
            SoundManager.instance.PlayHoverEffectSound(true);

            var objectHighlighted = objectsHighlighted[0].collider.transform;
            var selectedObject = PlanetManager.instance.GetPlanet(objectHighlighted.name);

            // Requisito especial para el Sol
            if (selectedObject && selectedObject.name.Equals("Sun"))
            {
                UIManager.instance.SetHighlightedObjectName(objectHighlighted.name);
                UIManager.instance.SetLabelNextToObject(objectHighlighted.name);

                // Cambiar a vista detallada del planeta cuando se presione el botón A del Oculus
                if (OVRInput.GetDown(OVRInput.Button.One))
                {
                    UIManager.instance.SwitchToDetailedPlanetView(selectedObject.name);
                }

                return;
            }

            if (selectedObject)
            {
                var followOrbitComponent = selectedObject.GetComponent<FollowOrbit>();
                var orbitToFollow = followOrbitComponent != null ? followOrbitComponent.orbitToFollow : null;

                if (orbitToFollow)
                {
                    var lineRenderer = GameObject.Find(orbitToFollow.name);

                    if (lineRenderer)
                    {
                        currentSelectedLineRenderer = lineRenderer.GetComponent<LineRenderer>();
                        lineRenderer.GetComponent<LineRenderer>().startWidth = 0.45f;
                        lineRenderer.GetComponent<LineRenderer>().endWidth = 0.45f;

                        // Establecer el nombre del objeto en el UI
                        UIManager.instance.SetHighlightedObjectName(objectHighlighted.name);
                        UIManager.instance.SetLabelNextToObject(objectHighlighted.name);
                    }
                }
            }

            // Cambiar a vista detallada del planeta cuando se presione el botón A del Oculus
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                UIManager.instance.SwitchToDetailedPlanetView(selectedObject.name);
                UIManager.instance.DisablePlanetLabel();
            }
        }
        else
        {
            SoundManager.instance.PlayHoverEffectSound(false);

            // Restaurar el estado predeterminado del LineRenderer
            if (currentSelectedLineRenderer != null)
            {
                currentSelectedLineRenderer.startWidth = 0.25f;
                currentSelectedLineRenderer.endWidth = 0.25f;

                UIManager.instance.DisablePlanetLabel();
            }
        }
    }

    // Verificar si el nivel de zoom se ha alcanzado
    private bool ZoomLevelReached()
    {
        if (Vector3.Distance(this.GetComponent<Camera>().transform.position, PlanetManager.instance.GetPlanet("Sun").transform.position) <= DefaultValues.minimumDistanceForOverviewCamera)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
