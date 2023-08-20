using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SavedObject : MonoBehaviour
{
    [HideInInspector]
    public bool PlaySound = true;

    public event Action<int> OnObjectSaved;
    public event Action OnObjectErased;

    public List<int> objectsID { get; private set; }
    public int MaxSavedObjects { get { return maxSavedObjects; } }

    [SerializeField] int maxSavedObjects;
    [SerializeField] AudioClip notificationSound;

    private AudioSource audioSource;

    void Awake()
    {
        objectsID = new List<int>();
        audioSource = GetComponent<AudioSource>();
    }

    public void SaveObjectByID(int ID)
    {
        if (objectsID.Count < maxSavedObjects)
        {
            Debug.Log($"Object saved with ID {ID}");
            if (PlaySound)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(notificationSound);
            }
            objectsID.Insert(0, ID);

            OnObjectSaved?.Invoke(ID);
        }
        else
            Debug.Log("Couldn't save object");
    }

    public void SaveObjectByID(string ID)
    {
        int number;

        bool isParsable = Int32.TryParse(ID, out number);

        if (isParsable)
        {
            SaveObjectByID(number);
        }
        else
            Console.WriteLine("Could not be parsed.");
    }

    public void EraseSavedObject()
    {
        Debug.Log("Saved Object Erased");
        objectsID.Remove(objectsID[0]);

        OnObjectErased?.Invoke();
    }
}
