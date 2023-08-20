using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectsDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectsData;

    public ObjectData GetObjectDataWithID(int id)
    {
        ObjectData objectData = null;

        int index = objectsData.FindIndex(data => data.ID == id);

        if (index > -1)
            objectData = objectsData[index];
        else
            Debug.Log($"No ObjectData found with ID{id}");

        return objectData;
    }
}

[Serializable]
public class ObjectData
{
    public ObjectData(string name, int iD, Vector3Int size, GameObject prefab, Sprite icon, bool canBeRotated, GameObject rotatedPrefab)
    {
        Name = name;
        ID = iD;
        Size = size;
        Prefab = prefab;
        Icon = icon;
        CanBeRotated = canBeRotated;
        RotatedPrefab = rotatedPrefab;
    }


    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public Vector3Int Size { get; private set; } = Vector3Int.one;
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
    [field: SerializeField]
    public Sprite Icon { get; private set; }
    [field: SerializeField]
    public bool CanBeRotated { get; private set; }
    [field: SerializeField]
    public bool CanBeOnTop { get; private set; }
    [field: SerializeField]
    public bool CantBeDeleted { get; private set; }
    [field: SerializeField]
    public GameObject RotatedPrefab { get; private set; }

    public GameObject GetRotatedPrefab(bool isRotated)
    {
        GameObject rotatedPrefab = Prefab;

        if (CanBeRotated)
            rotatedPrefab = isRotated ?
                RotatedPrefab :
                Prefab;
        else
            rotatedPrefab = Prefab;

        return rotatedPrefab;
    }

    public Vector3Int GetRotatedSize(bool isRotated)
    {
        Vector3Int rotatedSize = Size;

        if (CanBeRotated)
            rotatedSize = isRotated ?
            new Vector3Int(Size.z, Size.y, Size.x) :
            Size;
        else
            rotatedSize = Size;

        return rotatedSize;
    }
}