using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveObjectByTime : MonoBehaviour
{
    public Counter counter { get; private set; }

    public event Action OnEndPlayingTutorial;

    [SerializeField] ObjectsDatabaseSO database;
    [SerializeField] SavedObject savedObject;
    [SerializeField] float[] minutesBeforeNewObject;
    [SerializeField] List<ObjectsPercentage> percentages = new List<ObjectsPercentage>();
    [Header("Tutorial")]
    [SerializeField] float[] tutorialTimes;

    private int minutesIndex = 0;
    private float lastCounterMinutes = 0;
    //Tutorial
    private bool playingTutorial = false;
    private int tutorialIndex = 0;

    void Update()
    {
        if(counter != null && counter.Finished)
        {
            AddRandomObject();

            if (playingTutorial)
            {
                tutorialIndex++;

                if (tutorialIndex >= tutorialTimes.Length)
                {
                    playingTutorial = false;
                    OnEndPlayingTutorial?.Invoke();
                    return;
                }

                StartCounter(tutorialTimes[tutorialIndex]);
            }
            else
            {
                minutesIndex++;
                if (minutesIndex >= minutesBeforeNewObject.Length)
                    minutesIndex = minutesBeforeNewObject.Length - 1;

                StartCounter(minutesBeforeNewObject[minutesIndex]);
            }
        }
    }

    void OnEnable()
    {
        foreach(ObjectsPercentage percentage in percentages)
            percentage.SetupPercentage();

        if (!playingTutorial)
            LoadFromLastCounter();
    }

    void OnDisable()
    {
        counter = null;
    }

    public void PlayTutorialTimes()
    {
        playingTutorial = true;
        StartCounter(tutorialTimes[tutorialIndex]);
    }

    private void AddRandomObject()
    {
        float randomValue = UnityEngine.Random.value;
        float percentagesTotal = 0;
        float percentageToAdd = 0;

        int selectedObjectID = -1;

        for(int i = 0; i < percentages.Count; i++) 
        {
            percentagesTotal += percentages[i].percentage;
        }

        for(int i = 0; i < percentages.Count; i ++) 
        {
            if ((percentages[i].percentage / percentagesTotal) + percentageToAdd >= randomValue)
            {
                selectedObjectID = percentages[i].GetRandomObjectID();
                break;
            }
            else
                percentageToAdd += percentages[i].percentage / percentagesTotal;
        }

        savedObject.SaveObjectByID(selectedObjectID);
    }

    private void StartCounter(float minutes)
    {
        float minutesToSeconds = minutes * 60f;

        counter = new Counter(minutesToSeconds, true);

        if (!playingTutorial)
            SaveCounter();
    }

    private bool CheckLastDatePassed()
    {
        bool passed = false;

        TimeSpan difference = DateTime.Now - LoadCounter();
        float differenceInMinutes = (float)difference.TotalMinutes;

        Debug.Log($"Difference in minutes: {differenceInMinutes}");

        if(differenceInMinutes > minutesBeforeNewObject[minutesIndex])
            passed = true;

        return passed;
    }

    private void LoadFromLastCounter()
    {
        TimeSpan difference = DateTime.Now - LoadCounter();
        float differenceInMinutes = (float)difference.TotalMinutes;

        Debug.Log($"Difference in minutes: {differenceInMinutes}");

        if (differenceInMinutes > 0.02f)
        {
            for (int i = 0; i < savedObject.MaxSavedObjects; i++)
            {
                Debug.Log($"Minutes Before New Object: {lastCounterMinutes}");
                if (differenceInMinutes > lastCounterMinutes)
                {
                    AddRandomObject();

                    Debug.Log("Time Passed");
                    differenceInMinutes -= lastCounterMinutes;

                    minutesIndex++;
                    if (minutesIndex >= minutesBeforeNewObject.Length)
                        minutesIndex = minutesBeforeNewObject.Length - 1;

                    lastCounterMinutes = minutesBeforeNewObject[minutesIndex];
                }
                else
                {
                    float newCounterTime = lastCounterMinutes - differenceInMinutes;
                    Debug.Log($"Time Not Passed. Start counter with: {newCounterTime} minutes");
                    StartCounter(newCounterTime);
                    return;
                }
            }
        }

        StartCounter(minutesBeforeNewObject[minutesIndex]);
    }

    private void SaveCounter()
    {
        int year = DateTime.Now.Year;
        int month = DateTime.Now.Month;
        int day = DateTime.Now.Day;
        int hour = DateTime.Now.Hour;
        int minute = DateTime.Now.Minute;
        int second = DateTime.Now.Second;

        PlayerPrefs.SetInt("Date" + SaveDataUtility.GetMemberName(() => year), year);
        PlayerPrefs.SetInt("Date" + SaveDataUtility.GetMemberName(() => month), month);
        PlayerPrefs.SetInt("Date" + SaveDataUtility.GetMemberName(() => day), day);
        PlayerPrefs.SetInt("Date" + SaveDataUtility.GetMemberName(() => hour), hour);
        PlayerPrefs.SetInt("Date" + SaveDataUtility.GetMemberName(() => minute), minute);
        PlayerPrefs.SetInt("Date" + SaveDataUtility.GetMemberName(() => second), second);

        PlayerPrefs.SetInt(SaveDataUtility.GetMemberName(() => minutesIndex), minutesIndex);

        lastCounterMinutes = counter.TimeToComplete / 60;
        PlayerPrefs.SetFloat(SaveDataUtility.GetMemberName(() => lastCounterMinutes), lastCounterMinutes);
    }

    private DateTime LoadCounter()
    {
        DateTime counterDateTime;

        int year = 0;
        int month = 0;
        int day = 0;
        int hour = 0;
        int minute = 0;
        int second = 0;

        year = PlayerPrefs.GetInt("Date" + SaveDataUtility.GetMemberName(() => year), year);
        month = PlayerPrefs.GetInt("Date" + SaveDataUtility.GetMemberName(() => month), month);
        day = PlayerPrefs.GetInt("Date" + SaveDataUtility.GetMemberName(() => day), day);
        hour = PlayerPrefs.GetInt("Date" + SaveDataUtility.GetMemberName(() => hour), hour);
        minute = PlayerPrefs.GetInt("Date" + SaveDataUtility.GetMemberName(() => minute), minute);
        second = PlayerPrefs.GetInt("Date" + SaveDataUtility.GetMemberName(() => second), second);

        if (year <= 0)
            return DateTime.Now;

        counterDateTime = new DateTime(year, month, day, hour, minute, second);

        minutesIndex = PlayerPrefs.GetInt(SaveDataUtility.GetMemberName(() => minutesIndex));
        lastCounterMinutes = PlayerPrefs.GetFloat(SaveDataUtility.GetMemberName(() => lastCounterMinutes));

        return counterDateTime;
    }
}

[Serializable]
public class ObjectsPercentage
{
    [field: SerializeField]
    public string name { get; private set; }
    [Range(0f, 1f)]
    public float percentage;
    [SerializeField] List<int> ObjectsID;

    private List<int> objectsID = new List<int>();
    private int objectsIDCount = 0;

    public int GetRandomObjectID()
    {
        if(objectsID.Count == 0)
            CreateList();

        int returnedID = objectsID[0];
        objectsID.RemoveAt(0);
        SaveObjectsID();

        return returnedID;
    }

    public void SetupPercentage()
    {
        LoadObjectsID();

        if (objectsID.Count == 0)
        {
            CreateList();
        }

        string listDebug = name + " actual list: ";
        for(int i = 0; i < objectsID.Count; i++) 
        {
            listDebug += objectsID[i].ToString() + ",";
        }
        Debug.Log(listDebug);
    }

    private void CreateList()
    {
        objectsID.Clear();
        for (int i = 0; i < ObjectsID.Count; i++)
        {
            objectsID.Add(ObjectsID[i]);
        }
        objectsID.Shuffle();

        SaveObjectsID();
    }

    private void SaveObjectsID()
    {
        objectsIDCount = objectsID.Count;
        PlayerPrefs.SetInt(name + SaveDataUtility.GetMemberName(() => objectsIDCount), objectsIDCount);

        for(int i = 0; i < objectsID.Count; i++)
        {
            PlayerPrefs.SetInt(
                name + SaveDataUtility.GetMemberName(() => objectsID) + i,
                objectsID[i]);
        }
    }

    private void LoadObjectsID()
    {
        List<int> objects = new List<int>();

        objectsIDCount = PlayerPrefs.GetInt(name + SaveDataUtility.GetMemberName(() => objectsIDCount));

        for(int i = 0; i < objectsIDCount; i++)
        {
            objects.Add(PlayerPrefs.GetInt(name + SaveDataUtility.GetMemberName(() => objectsID) + i));
        }

        objectsID = new List<int>(objects);
    }
}
