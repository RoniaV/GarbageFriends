using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaceSavedObjectButton : MonoBehaviour
{
    public bool PlaySound = true;

    [SerializeField] Button button;
    [SerializeField] SavedObject savedObject;
    [SerializeField] SaveObjectByTime saveObjectByTime;
    [SerializeField] PlacementSystem placementSystem;
    [SerializeField] ObjectsDatabaseSO database;
    [SerializeField] Image objectIcon;
    [SerializeField] TextMeshProUGUI counterText;
    [Header("Animation Settings")]
    [SerializeField] Animator buttonAnimator;
    [SerializeField] string boolAnim;
    [Header("Sound Settings")]
    [SerializeField] AudioSource buttonAudioSource;
    [SerializeField] AudioClip clickButton;

    void OnEnable()
    {
        savedObject.OnObjectSaved += ObjectSaved;
        savedObject.OnObjectErased += ObjectRemoved;
    }

    void OnDisable()
    {
        savedObject.OnObjectSaved -= ObjectSaved;
        savedObject.OnObjectErased -= ObjectRemoved;
    }

    void Update()
    {
        counterText.enabled = !(savedObject.objectsID?.Count > 0);
        objectIcon.enabled = savedObject.objectsID?.Count > 0;
        buttonAnimator.SetBool(boolAnim, savedObject.objectsID?.Count > 0);

        counterText.text = GetCounterText();
    }

    private string GetCounterText()
    {
        string text = "";

        if(saveObjectByTime.counter != null)
        {
            float counterTimeInSeconds = saveObjectByTime.counter.TimeLeft;

            int minutes = Mathf.FloorToInt(counterTimeInSeconds / 60F);
            int seconds = Mathf.FloorToInt(counterTimeInSeconds - minutes * 60);

            string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);
            //text = "<mspace=4em>" + niceTime + "</mspace>";
            text = niceTime;
        }

        return text;
    }

    private void ObjectSaved(int ID)
    {
        button.onClick?.RemoveAllListeners();
        button.onClick?.AddListener(() => OnClick(ID));
        if (PlaySound)
            button.onClick?.AddListener(() => buttonAudioSource.PlayOneShot(clickButton));

        int selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        objectIcon.sprite = database.objectsData[selectedObjectIndex]?.Icon;
    }

    private void ObjectRemoved()
    {
        button.onClick?.RemoveAllListeners();

        if(savedObject.objectsID.Count > 0)
        {
            int ID = savedObject.objectsID[0];

            button.onClick?.AddListener(() => OnClick(ID));
            if (PlaySound)
                button.onClick?.AddListener(() => buttonAudioSource.PlayOneShot(clickButton));

            int selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
            objectIcon.sprite = database.objectsData[selectedObjectIndex].Icon;
        }
    }

    private void OnClick(int ID)
    {
        Debug.Log("Click Garbage button");
        button.onClick.RemoveAllListeners();
        savedObject.EraseSavedObject();
        placementSystem.StartPlacement(ID);
    }
}
