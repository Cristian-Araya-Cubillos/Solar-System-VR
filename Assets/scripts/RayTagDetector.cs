using UnityEngine;

public class RayTagDetector : MonoBehaviour
{
    public float maxDistance; // Distancia máxima del raycast
    public string targetTag;  // Tag del objeto a detectar

    void Update()
    {
        DetectRayTag();
    }

    void DetectRayTag()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Realizar el raycast
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Verificar si el objeto tiene el tag especificado
            if (hit.collider.CompareTag(targetTag))
            {
                Debug.Log("Apuntando a: " + hit.collider.name);

                // Obtener el componente PlanetInfo del objeto y ejecutar LoadTextToScrollBar
                //PlanetInfo planetInfo = hit.collider.GetComponent<PlanetInfo>();
                PlanetInfo planetInfo = GetComponent<PlanetInfo>();
                if (planetInfo != null)
                {
                    // Llamar a la función LoadTextToScrollBar con el nombre del objeto detectado
                    planetInfo.LoadTextToScrollBar(hit.collider.name);
                }
                else
                {
                    Debug.LogWarning("El objeto detectado no tiene el componente PlanetInfo.");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Visualizar el raycast en la escena
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * maxDistance);
    }
}
