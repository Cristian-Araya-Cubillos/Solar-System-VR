using UnityEngine;
using UnityEngine.XR;  // Asegúrate de tener la biblioteca de XR importada

/// <summary>
/// Detailed camera input controller for VR.
/// </summary>
public class DetailedCameraInputController : MonoBehaviour
{
    public float xSpeed = 40.0f;   // Velocidad de rotación en el eje X
    public float ySpeed = 40.0f;   // Velocidad de rotación en el eje Y
    public float zoomSpeed = 2.0f; // Velocidad del zoom

    private double x = 80.0f;
    private double y = 50.0f;

    Quaternion rotation;
    Vector3 position;

    private float distance;

    private bool isZoomingIn = false;
    private bool isZoomingOut = false;

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false; // Ocultar el cursor en VR

        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            SetPosition();
    }

    private void SetPosition()
    {
        rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler((float)y, (float)x, 0), Time.deltaTime * ConfigManager.instance.rotationSpeed);

        position = rotation * new Vector3(0.0f, 0.0f, -ConfigManager.instance.distance) + PlanetManager.instance.selectedPlanet.position;

        transform.rotation = rotation;
        transform.position = position;
    }

    void LateUpdate()
    {
        SetPosition();
        HandleVRInput(); // Manejar la entrada de VR
    }

    private void HandleVRInput()
    {
        // Obtén los controles del mando VR
        InputDevice leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Rotación con el joystick del mando derecho (movimiento en el mundo)
        Vector2 rightJoystickInput;
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightJoystickInput))
        {
            x += rightJoystickInput.x * xSpeed * Time.deltaTime;
            y -= rightJoystickInput.y * ySpeed * Time.deltaTime;
        }

        // Zoom con los botones Y y B (mando izquierdo)
        bool buttonYPressed, buttonBPressed;
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out buttonYPressed) &&
            leftHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out buttonBPressed))
        {
            if (buttonYPressed && !isZoomingIn)
            {
                isZoomingIn = true;
                ZoomInOut(1); // Aumentar zoom
            }
            else if (!buttonYPressed && isZoomingIn)
            {
                isZoomingIn = false;
            }

            if (buttonBPressed && !isZoomingOut)
            {
                isZoomingOut = true;
                ZoomInOut(-1); // Reducir zoom
            }
            else if (!buttonBPressed && isZoomingOut)
            {
                isZoomingOut = false;
            }
        }

        // Zoom con el joystick izquierdo (solo si está movido)
        Vector2 leftJoystickInput;
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftJoystickInput))
        {
            // Solo realizar zoom si hay movimiento vertical en el joystick izquierdo
            if (leftJoystickInput.y != 0)
            {
                ZoomInOut(leftJoystickInput.y);
            }
        }
    }

    private void ZoomInOut(float direction)
    {
        // Cambia la distancia basándote en la dirección del input
        ConfigManager.instance.distance -= direction * zoomSpeed * Time.deltaTime;

        // Limita el zoom para que no se aleje ni acerque demasiado
        ConfigManager.instance.distance = Mathf.Clamp(
            ConfigManager.instance.distance,
            ConfigManager.instance.minDistanceForSelectedPlanet / ConfigManager.instance.minDistanceForPlanets,
            ConfigManager.instance.maxDistanceForPlanets * ConfigManager.instance.minDistanceForSelectedPlanet);
    }
}
