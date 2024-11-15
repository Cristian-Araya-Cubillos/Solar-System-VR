using System.Collections.Generic;
using UnityEngine;

public class MoonAudioManager : MonoBehaviour
{
    // Array para los clips de audio
    public AudioClip firstAudio; // El primer audio que siempre se reproducirá
    public List<AudioClip> otherAudios; // Lista de otros audios
    private List<AudioClip> availableAudios; // Audios disponibles para reproducir aleatoriamente
    private AudioSource audioSource; // Componente AudioSource

    [Header("Configuración de tiempo")]
    public float audioPlayInterval = 300f; // Intervalo en segundos para reproducir otros audios

    private void Start()
    {
        // Inicializa el AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();

        // Reproduce el primer audio
        PlayFirstAudio();

        // Inicializa la lista de audios disponibles
        availableAudios = new List<AudioClip>(otherAudios);

        // Inicia la rutina para reproducir audios aleatorios
        StartCoroutine(PlayAudioRoutine());
    }

    private void PlayFirstAudio()
    {
        // Reproduce el primer audio
        audioSource.PlayOneShot(firstAudio);
    }

    private System.Collections.IEnumerator PlayAudioRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(audioPlayInterval); // Espera el intervalo especificado
            PlayRandomAudio(); // Reproduce un audio aleatorio
        }
    }

    public void PlayRandomAudio()
    {
        if (availableAudios.Count == 0)
        {
            // Si no hay audios disponibles, reinicia la lista
            availableAudios = new List<AudioClip>(otherAudios);
        }

        // Escoge un índice aleatorio
        int randomIndex = Random.Range(0, availableAudios.Count);
        AudioClip randomAudio = availableAudios[randomIndex];

        // Reproduce el audio seleccionado
        audioSource.PlayOneShot(randomAudio);

        // Elimina el audio reproducido de la lista
        availableAudios.RemoveAt(randomIndex);
    }
}
