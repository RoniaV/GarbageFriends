using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public enum FurnitureType
{
    Furniture,
    Floor,
    Wall
}

public class FurnitureDatabaseTool : MonoBehaviour
{
    [SerializeField] ObjectsDatabaseSO database;
    [SerializeField] GameObject orginalPrefab;
    [SerializeField] string directoryPath;
    [Header("New Furniture Data")]
    [SerializeField] string name;
    [SerializeField] FurnitureType type;
    [SerializeField] Vector3Int size;
    [SerializeField] Sprite image;
    [SerializeField] bool canBeRotated;
    [Header("Prefab Fix")]
    [SerializeField] GameObject[] furniturePrefabs;

    //[MenuItem("Window/Tools/FurnitureDatabaseWindow")]
    //public static void ShowWindow()
    //{
    //    EditorWindow.GetWindow(typeof(FurnitureDatabaseTool));
    //}
    //
    //void OnGUI() { }

#if UNITY_EDITOR

    public void AddNewFurnitureToDatabase()
    {
        GameObject furniturePrefab = CreateFurniturePrefab();
        GameObject rotatedFurniturePrefab = null;

        if (canBeRotated)
            rotatedFurniturePrefab = CreateFurniturePrefab(true);

        ObjectData furnitureData = new ObjectData(
            name,
            GetID(type),
            size,
            furniturePrefab,
            image,
            canBeRotated,
            rotatedFurniturePrefab);

        database.objectsData.Add(furnitureData);
    }

    private GameObject CreateFurniturePrefab(bool rotated = false)
    {
        GameObject newFurniture = Instantiate(orginalPrefab);

        newFurniture.name = name;
        SpriteRenderer spriteRenderer = newFurniture.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = image;
        spriteRenderer.flipX = rotated;

        // Create folder Prefabs and set the path as within the Prefabs folder,
        // and name it as the GameObject's name with the .Prefab format
        //if (!Directory.Exists("Assets/Prefabs"))
        //    AssetDatabase.CreateFolder("Assets", "Prefabs");
        string localPath;

        if (!rotated)
            localPath = directoryPath + "/" + name + ".prefab";
        else
            localPath = directoryPath + "/" + name + "_R" + ".prefab";

        // Make sure the file name is unique, in case an existing Prefab has the same name.
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        // Create the new Prefab and log whether Prefab was saved successfully.
        bool prefabSuccess;
        GameObject newFurniturePrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(newFurniture, localPath, InteractionMode.UserAction, out prefabSuccess);
        if (prefabSuccess == true)
            Debug.Log("Prefab was created successfully");
        else
            Debug.Log("Prefab failed to create " + prefabSuccess);

        DestroyImmediate(newFurniture);

        return newFurniturePrefab;
    }

    private int GetID(FurnitureType type)
    {
        int id = -1;

        switch(type)
        {
            case FurnitureType.Furniture:
                id = 200;
                break;
            case FurnitureType.Wall:
                id = 100;
                break;
            case FurnitureType.Floor:
                id = 0;
                break;
        }

        for(int i = 0; i < database.objectsData.Count; i++) 
        {
            switch (type) 
            {
                case FurnitureType.Furniture:
                    if (database.objectsData[i].ID >= 200 && database.objectsData[i].ID >= id)
                        id = database.objectsData[i].ID + 1;
                    break;
                case FurnitureType.Floor:
                    if (database.objectsData[i].ID < 100 && database.objectsData[i].ID >= id)
                        id = database.objectsData[i].ID + 1;
                    break;
                case FurnitureType.Wall:
                    if (database.objectsData[i].ID >= 100 && database.objectsData[i].ID < 200 && database.objectsData[i].ID >= id)
                        id = database.objectsData[i].ID + 1;
                    break;
            }
        }

        return id;
    }

    public void FixAllPrefabs()
    {
        foreach(GameObject prefab in furniturePrefabs)
        {
            GameObject sprite = prefab.transform.GetChild(0).gameObject;
            sprite.AddComponent<LookToCamera>();
            sprite.transform.localPosition = Vector3.zero;
        }
    }

#endif
}
