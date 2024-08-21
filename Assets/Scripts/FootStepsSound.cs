using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepsSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundClips;  // Array of sound clips
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource component

    void Awake()
    {
        // Ensure the AudioSource component is attached
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component is missing from the GameObject.");
            }
        }

        // Ensure there are sound clips assigned
        if (soundClips.Length == 0)
        {
            Debug.LogError("No sound clips assigned to the soundClips array.");
        }
    }

    // This function can be called from the Animator event graph
    public void PlayRandomSound()
    {
        if (soundClips.Length > 0 && audioSource != null)
        {
            // Pick a random sound clip from the array
            int randomIndex = Random.Range(0, soundClips.Length);
            AudioClip clipToPlay = soundClips[randomIndex];

            // Play the selected sound clip
            audioSource.PlayOneShot(clipToPlay);
        }
    }
}