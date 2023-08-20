using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GUIType
{
    Game,
    Placement,
    Pick
}

public class GUIManager : MonoBehaviour
{
    [SerializeField] GUIType initialGUI;
    [Header("Panels")]
    [SerializeField] GameObject gameGUI;
    [SerializeField] GameObject placementGUI;
    [SerializeField] GameObject pickGUI;

    void Start()
    {
        SwitchGUI(initialGUI);
    }

    public void SwitchGUI(GUIType initialGUI)
    {
        switch(initialGUI)
        {
            case GUIType.Game:
                gameGUI.SetActive(true);
                placementGUI.SetActive(false);
                pickGUI.SetActive(false);
                break;
            case GUIType.Placement:
                placementGUI.SetActive(true);
                gameGUI.SetActive(false);
                pickGUI.SetActive(false);
                break;
            case GUIType.Pick:
                pickGUI.SetActive(true);
                gameGUI.SetActive(false);
                placementGUI.SetActive(false);
                break;
        }
    }
}
