using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RadioSystem : MonoBehaviour
{
    public bool isOn { get; private set; }

    [SerializeField] AudioClip[] songs;
    [SerializeField] AudioSource ambientSound;
    [SerializeField] bool startOn;
    [Header("Animation Settings")]
    [SerializeField] Animator buttonAnimator;
    [SerializeField] string boolName = "On";
    [Header("Sound Settings")]
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip turnOnSound;
    [SerializeField] AudioClip turnOffSound;
    [Header("Volume Settings")]
    [SerializeField] float originalVolume = 0.1f;
    [SerializeField] float editModeVolume = 0.05f;
    [Header("Ambient Settings")]
    [SerializeField] AudioSource ambientSource;
    [SerializeField] float ambientOriginalVolume = 0.06f;
    [SerializeField] float ambientEditModeVolume = 0.15f;

    private AudioSource audioSource;

    private List<AudioClip> playList = new List<AudioClip>();
    private int playlistIndex = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        audioSource.loop = true;
        audioSource.volume = originalVolume;
        ambientSource.volume = ambientOriginalVolume;

        if (startOn)
            TurnOn();
        else
            TurnOff();
    }

    void Update()
    {
        buttonAnimator.SetBool(boolName, isOn);
    }

    public void ChangeState()
    {
        if (isOn)
            TurnOff();
        else
            TurnOn();
    }

    public void PlayAmbientSound()
    {
        audioSource.volume = editModeVolume;
        ambientSource.volume = ambientEditModeVolume;
    }

    public void StopAmbientSound()
    {
        audioSource.volume = originalVolume;
        ambientSource.volume = ambientOriginalVolume;
    }

    public void SetOriginalVolume(float volume)
    {
        originalVolume = volume;
        audioSource.volume = originalVolume;
    }

    private void TurnOn()
    {
        isOn = true;

        soundSource.PlayOneShot(turnOnSound);
        PlayNextSong();
    }

    private void TurnOff()
    {
        isOn = false;

        soundSource.PlayOneShot(turnOffSound);
        audioSource.Stop();
    }

    private void PlayNextSong()
    {
        playlistIndex++;

        if (playlistIndex >= playList.Count)
        {
            playList = GetPlayList();
            playlistIndex = 0;
        }

        audioSource.Stop();
        audioSource.clip = playList[playlistIndex];
        audioSource.Play();
    }

    private List<AudioClip> GetPlayList()
    {
        List<AudioClip> returnedPlayList = new List<AudioClip>();

        for(int i = 0; i < songs.Length; i++) 
        {
            returnedPlayList.Add(songs[i]);
        }

        returnedPlayList.Shuffle();

        return returnedPlayList;
    }
}

static class MyExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
