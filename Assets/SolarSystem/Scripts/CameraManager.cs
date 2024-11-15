using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject Camera;  // La cámara única que se moverá
    public GameObject MainCamera;     // El primer objeto al que la cámara puede ir
    public GameObject OrbitCamera;     // El segundo objeto al que la cámara puede ir

    public GameObject sunLigh;
    public static CameraManager instance = null;

    // Variable para saber en qué modo de cámara estamos (puede ser 0 para MainCamera, 1 para OrbitCamera)
    public ConstantValues.CameraMode cameraMode;

    private float transitionSpeed = 10f;  // Velocidad de transición entre las dos posiciones

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        sunLigh.SetActive(false);
        Camera.SetActive(true);
        PlanetManager.instance.DisableSunFlare("Sun");
    }

    // Este método se llama para mover la cámara a la posición de MainCamera
    public void SwitchToMainCamera()
    {
        cameraMode = ConstantValues.CameraMode.Detailed;
        LightManager.instance.ToggleOverviewLight(cameraMode);
        sunLigh.SetActive(true);
        ToggleDetailedCameraInputController(true);  // Activar el controlador de la cámara detallada
        StartCoroutine(MoveCameraToPosition(MainCamera.transform.position));
    }

    // Este método se llama para mover la cámara a la posición de OrbitCamera
    public void SwitchToOrbitCamera()
    {
        cameraMode = ConstantValues.CameraMode.Overview;
        LightManager.instance.ToggleOverviewLight(cameraMode);
        sunLigh.SetActive(false);
        ToggleDetailedCameraInputController(false);  // Desactivar el controlador de la cámara detallada
        StartCoroutine(MoveCameraToPosition(OrbitCamera.transform.position));
    }

    // Método que mueve la cámara entre las posiciones de los dos objetos
    private IEnumerator MoveCameraToPosition(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = Camera.transform.position;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * transitionSpeed;
            Camera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            yield return null;
        }

        Camera.transform.position = targetPosition;  // Asegura que la cámara esté exactamente en la posición objetivo
    }

    // Mantener la función para habilitar/deshabilitar el controlador de entrada de la cámara detallada
    private void ToggleDetailedCameraInputController(bool enabled)
    {
        Camera.GetComponent<DetailedCameraInputController>().enabled = enabled;
    }
}
