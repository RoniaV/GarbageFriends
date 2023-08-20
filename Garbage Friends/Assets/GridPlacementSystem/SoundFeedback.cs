using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundFeedback : MonoBehaviour
{
    [SerializeField] AudioClip stopPlacementSound, placeSound, pickSound,
        removeSound, wrongPlacementSound, rotateSound;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.StopPlacement:
                audioSource.PlayOneShot(stopPlacementSound);
                break;
            case SoundType.Place:
                audioSource.PlayOneShot(placeSound);
                break;
            case SoundType.Pick:
                audioSource.PlayOneShot(pickSound);
                break;
            case SoundType.Remove:
                audioSource.PlayOneShot(removeSound);
                break;
            case SoundType.WrongPlacement:
                audioSource.PlayOneShot(wrongPlacementSound);
                break;
            case SoundType.RotateSound:
                audioSource.PlayOneShot(rotateSound);
                break;
            default:
                break;
        }
    }
}

public enum SoundType
{
    StopPlacement,
    Place,
    Pick,
    Remove,
    WrongPlacement,
    RotateSound
}
