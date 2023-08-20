using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Text", menuName = "Scriptable Objects/Text To Display")]
public class TextToDisplay : ScriptableObject
{
    [SerializeField] string name;
    [TextArea(3, 10)]
    [SerializeField] string[] sentences;
    [Header("Custom Sound")]
    [Tooltip("Every AudioClip will be played once at the beggining of the sentence with the same index")]
    [SerializeField] AudioClip[] soundsLinkedWithSentences;


    public string Name { get => name; }
    public string[] Sentences { get => sentences; }
    public AudioClip[] SoundsLinkedWithSentences { get => soundsLinkedWithSentences; }
}
