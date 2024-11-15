using UnityEngine;

public class InputControllerVR : MonoBehaviour
{
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

    public Transform rightHandAnchor; // El ancla del controlador derecho

    // Use this for initialization
    void Start()
    {
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (this.GetComponent<Camera>().isActiveAndEnabled)
        {
            SetTheCameraPositionBasedOnInput();
            ObjectInfo();
        }
    }

    void SetTheCameraPositionBasedOnInput()
    {
        var orbitSpeedInDaysPerSecond = ConfigManager.instance?.orbitSpeedInDaysPerSecond != null ? ConfigManager.instance.orbitSpeedInDaysPerSecond : DefaultValues.orbitSpeedInDaysPerSecond;

        rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler((float)y, (float)x, 0), Time.deltaTime * orbitSpeedInDaysPerSecond);

        position = rotation * new Vector3(0.0f, 0.0f, -distance);

        transform.rotation = rotation;
        transform.position = position;

        x = transform.eulerAngles.y;
        y = transform.eulerAngles.x;

        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger)) // Botón del controlador
        {
            x += OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x * xSpeed;
            y -= OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y * ySpeed;
        }

        float scrollDelta = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;

        if (scrollDelta > 0 && !ZoomLevelReached())
        {
            distance -= scrollDelta * zoomSpeed;
        }
        else if (scrollDelta < 0)
        {
            distance -= scrollDelta * zoomSpeed;
        }
    }

    /// <summary>
    /// Detectar la información de los planetas usando el rayo del controlador VR
    /// </summary>
    void ObjectInfo()
    {
        Ray ray = new Ray(rightHandAnchor.position, rightHandAnchor.forward); // Ray desde el controlador derecho
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 500))
        {
            SoundManager.instance.PlayHoverEffectSound(true);

            var objectHighlighted = hit.collider.transform;
            var selectedObject = PlanetManager.instance.GetPlanet(objectHighlighted.name);

            if (selectedObject && selectedObject.name.Equals("Sun"))
            {
                UIManager.instance.SetHighlightedObjectName(objectHighlighted.name);
                UIManager.instance.SetLabelNextToObject(objectHighlighted.name);

                // Cambiar a vista detallada cuando se presiona el botón
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) // Detecta clic con el gatillo
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

                        UIManager.instance.SetHighlightedObjectName(objectHighlighted.name);
                        UIManager.instance.SetLabelNextToObject(objectHighlighted.name);
                    }
                }
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                UIManager.instance.SwitchToDetailedPlanetView(selectedObject.name);
                UIManager.instance.DisablePlanetLabel();
            }
        }
        else
        {
            SoundManager.instance.PlayHoverEffectSound(false);

            if (currentSelectedLineRenderer != null)
            {
                currentSelectedLineRenderer.startWidth = 0.25f;
                currentSelectedLineRenderer.endWidth = 0.25f;

                UIManager.instance.DisablePlanetLabel();
            }
        }
    }

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
