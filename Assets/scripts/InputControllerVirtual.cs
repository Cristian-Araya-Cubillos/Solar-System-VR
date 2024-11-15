using UnityEngine;

public class InputControllerVirtual : MonoBehaviour {

    private float x = 30.0f;
    private float y = 20.0f;

    public float xSpeed = 10.0f;
    public float ySpeed = 10.0f;

    Quaternion rotation;

    [HideInInspector]
    public Vector3 position;

    float distance = 150;
    float zoomSpeed = 5;

    LineRenderer currentSelectedLineRenderer = null;

    // Use this for initialization
    void Start () {
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }
    
    // Update is called once per frame
    void LateUpdate () {
        if (this.GetComponent<Camera>().isActiveAndEnabled)
        {
            SetTheCameraPositionBasedOnInput();
            ObjectInfo();
        }
    }

    void SetTheCameraPositionBasedOnInput()
    {
        var orbitSpeedInDaysPerSecond = ConfigManager.instance?.orbitSpeedInDaysPerSecond != null ? ConfigManager.instance.orbitSpeedInDaysPerSecond : DefaultValues.orbitSpeedInDaysPerSecond;

        rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(y, x, 0), Time.deltaTime * orbitSpeedInDaysPerSecond);

        position = rotation * new Vector3(0.0f, 0.0f, -distance);

        transform.rotation = rotation;
        transform.position = position;

        x += OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x * xSpeed;
        y -= OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y * ySpeed;

        distance -= OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y * zoomSpeed;

        if (OVRInput.Get(OVRInput.Button.Two))
        {
            // Additional action when Button Two (typically 'B' or 'Oculus Button') is pressed
        }
    }

    /// <summary>
    /// Performing action when planet is selected on overview camera mode
    /// </summary>
    void ObjectInfo()
    {
        Ray ray = CameraManager.instance.OrbitCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit[] objectsHighlighted = Physics.RaycastAll(ray, 500);
        bool raycast = Physics.Raycast(ray, out RaycastHit hit, 500, LayerMask.GetMask("UI"));
        
        if (raycast)
        {
            SoundManager.instance.PlayHoverEffectSound(true);

            Transform objectHighlighted = hit.collider.transform;
            GameObject selectedObject = PlanetManager.instance.GetPlanet(objectHighlighted.name);

            if (selectedObject && selectedObject.name.Equals("Sun"))
            {
                UIManager.instance.SetHighlightedObjectName(objectHighlighted.name);
                UIManager.instance.SetLabelNextToObject(objectHighlighted.name);

                // Switch to detailed camera view when clicked
                if (Input.GetMouseButtonUp(0))
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
                    // now we have an access to line renderer component
                    var lineRenderer = GameObject.Find(orbitToFollow.name);

                    if (lineRenderer)
                    {
                        currentSelectedLineRenderer = lineRenderer.GetComponent<LineRenderer>();
                        lineRenderer.GetComponent<LineRenderer>().startWidth = 0.45f;
                        lineRenderer.GetComponent<LineRenderer>().endWidth = 0.45f;

                        // set name on UI panel
                        UIManager.instance.SetHighlightedObjectName(objectHighlighted.name);
                        UIManager.instance.SetLabelNextToObject(objectHighlighted.name);
                    }
                }
            }

            // Switch to detailed camera view when clicked
            if (raycast && Input.GetMouseButtonUp(0))
            {
                UIManager.instance.SwitchToDetailedPlanetView(selectedObject.name);
                UIManager.instance.DisablePlanetLabel();
            }
        }
        else
        {
            SoundManager.instance.PlayHoverEffectSound(false);

            // set line renderer to default state
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
