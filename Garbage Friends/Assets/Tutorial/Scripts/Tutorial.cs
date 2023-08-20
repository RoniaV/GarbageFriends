using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TextDisplayer textDisplayer;
    [SerializeField] StartCameraMovement startCameraMovement;
    [SerializeField] PlacementSystem placementSystem;
    [SerializeField] SavedObject savedObject;
    [SerializeField] SaveObjectByTime saveObjectByTime;
    [SerializeField] Image placeObjectButtonImage;
    [SerializeField] GameObject placeObjectButton;
    [SerializeField] GameObject moveObjectButton;
    [SerializeField] PlaceSavedObjectButton placeSavedObjectButton;
    [Header("Texts")]
    [SerializeField] TextToDisplay startText;
    [SerializeField] TextToDisplay midText;
    [SerializeField] TextToDisplay endText;
    [Header("Icons")]
    [SerializeField] Sprite defaultIcon;
    [SerializeField] Sprite tutorialIcon;
    [Header("Music Settings")]
    [SerializeField] RadioSystem radio;
    [SerializeField] float tutorialVolume = 0.1f;
    [SerializeField] float originalVolume = 0.3f;
    [Header("First Objects ID")]
    [SerializeField] List<int> objectsID = new List<int>();

    private int index = 0;
    private bool randomObjectsEnds = false;
    private int savedObjectID = -1;

    void Start()
    {
        saveObjectByTime.enabled = false;

        startCameraMovement.OnCameraMovementEnd += StartTutorial;
    }

    private void StartTutorial()
    {
        startCameraMovement.OnCameraMovementEnd -= StartTutorial;

        int b = PlayerPrefs.GetInt("Tutorial");
        bool tutorialMade = b == 1;

        if (!tutorialMade)
        {
            placeObjectButtonImage.sprite = tutorialIcon;
            savedObject.PlaySound = false;
            placeSavedObjectButton.PlaySound = false;
            placeObjectButton.SetActive(false);
            moveObjectButton.SetActive(false);

            radio.SetOriginalVolume(tutorialVolume);
            textDisplayer.DisplayText(startText);
            textDisplayer.OnEndDisplaying += GiveFirstObject;
        }
        else
            saveObjectByTime.enabled = true;
    }

    private void GiveFirstObject()
    {
        textDisplayer.OnEndDisplaying -= GiveFirstObject;
        placeObjectButton.SetActive(true);
        moveObjectButton.SetActive(true);

        if (index < objectsID.Count)
        {
            savedObject.SaveObjectByID(objectsID[index]);
            index++;

            placementSystem.OnObjectPlaced += GiveObject;
            placementSystem.OnObjectErased += GiveObject;
        }
    }

    private void GiveObject(int ID)
    {
        if (ID != objectsID[index - 1])
            return;


        if (index < objectsID.Count)
        {
            savedObject.SaveObjectByID(objectsID[index]);
            index++;
        }
        else
        {
            placementSystem.OnObjectPlaced -= GiveObject;
            placementSystem.OnObjectErased -= GiveObject;

            MidTextDisplay();
        }
    }

    private void MidTextDisplay()
    {
        placeObjectButton.SetActive(false);
        moveObjectButton.SetActive(false);

        textDisplayer.DisplayText(midText);
        textDisplayer.OnEndDisplaying += GiveRandomObjects;
    }

    private void GiveRandomObjects()
    {
        textDisplayer.OnEndDisplaying -= GiveRandomObjects;

        savedObject.PlaySound = true;
        placeSavedObjectButton.PlaySound = true;
        placeObjectButton.SetActive(true);
        moveObjectButton.SetActive(true);
        placeObjectButtonImage.sprite = defaultIcon;
        radio.SetOriginalVolume(originalVolume);

        saveObjectByTime.PlayTutorialTimes();
        saveObjectByTime.enabled = true;

        saveObjectByTime.OnEndPlayingTutorial += EndRandomObjects;
        savedObject.OnObjectSaved += GetLastSavedObject;
        placementSystem.OnObjectPlaced += DisplayEndText;
        placementSystem.OnObjectErased += DisplayEndText;
    }

    private void GetLastSavedObject(int ID)
    {
        savedObjectID = ID;
    }

    private void EndRandomObjects()
    {
        randomObjectsEnds = true;

        saveObjectByTime.enabled = false;
        saveObjectByTime.OnEndPlayingTutorial -= EndRandomObjects;
    }

    private void DisplayEndText(int ID)
    {
        if (ID != savedObjectID || !randomObjectsEnds)
            return;

        savedObject.OnObjectSaved -= GetLastSavedObject;
        placementSystem.OnObjectPlaced -= DisplayEndText;
        placementSystem.OnObjectErased -= DisplayEndText;

        placeObjectButton.SetActive(false);
        moveObjectButton.SetActive(false);
        radio.SetOriginalVolume(tutorialVolume);

        textDisplayer.DisplayText(endText);
        textDisplayer.OnEndDisplaying += EndTutorial;
    }

    private void EndTutorial()
    {
        textDisplayer.OnEndDisplaying -= EndTutorial;

        placeObjectButton.SetActive(true);
        moveObjectButton.SetActive(true);
        saveObjectByTime.enabled = true;
        radio.SetOriginalVolume(originalVolume);

        PlayerPrefs.SetInt("Tutorial", 1);
    }

    void OnDisable()
    {
        int b = PlayerPrefs.GetInt("Tutorial");
        bool tutorialMade = b == 1;

        if (!tutorialMade)
            PlayerPrefs.DeleteAll();
    }
}
