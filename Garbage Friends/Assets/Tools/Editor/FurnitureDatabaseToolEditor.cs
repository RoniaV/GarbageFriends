using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FurnitureDatabaseTool))]
public class FurnitureDatabaseToolEditor : Editor
{
    FurnitureDatabaseTool m_Tool;

    void OnEnable()
    {
        m_Tool = (FurnitureDatabaseTool)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Add Furniture to Database"))
            m_Tool.AddNewFurnitureToDatabase();

        if(GUILayout.Button("Fix Prefabs"))
            m_Tool.FixAllPrefabs();
    }
}
