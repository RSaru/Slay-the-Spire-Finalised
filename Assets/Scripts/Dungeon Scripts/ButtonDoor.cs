using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public AudioClip doorOpenSound;
    public AudioSource audioSource;
    public GameObject doorObject;

    //opens the door by playing sound and destroying the door object
    public void OpenDoor()
    {
        if (audioSource != null && doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound);
        }

        if (doorObject != null)
        {
            Destroy(doorObject);
        }
    }
}
