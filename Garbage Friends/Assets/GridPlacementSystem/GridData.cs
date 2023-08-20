using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class GridData
{
    public Dictionary<Vector3Int, PlacementData> placedObjects { get; private set; } = new();

    int placedObjectsCount = 0;

    Vector3Int minGridSize;
    Vector3Int maxGridSize;

    public GridData() { }

    public GridData(Vector3Int minGridSize, Vector3Int maxGridSize)
    {
        this.minGridSize = minGridSize;
        this.maxGridSize = maxGridSize;
    }

    public void AddObjectAt(Vector3Int gridPosition,
                            Vector3Int objectSize,
                            int ID,
                            int placedObjectIndex,
                            bool isRotated)
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(gridPosition, positionsToOccupy, ID, placedObjectIndex, isRotated);

        if(gridPosition.y > 0)
        {
            PlacementData container = GetContainer(gridPosition);

            container?.placedInObjects.Add(data);

            if (container != null)
                data.container = container;
        }

        placedObjects[gridPosition] = data;
    }

    public void AddPlacementData(Vector3Int gridPosition,
        Vector3Int size,
        PlacementData data,
        bool isRotated)
    {
        //Si el meuble esta por encima del suelo, busca un container
        if (gridPosition.y > 0)
        {
            PlacementData container = GetContainer(gridPosition);

            //Añade el mueble al container
            container?.placedInObjects.Add(data);

            if (container != null)
            {
                Debug.Log($"Set container for object with ID:{data.ID}, at grid position {container.gridPosition} with key{placedObjects.ContainsKey(container.gridPosition)}");
                data.container = container;
            }
        }

        //Guardo la antigua posicion para luego calcular el offset de los objetos dentro
        Vector3Int oldGridPosition = data.gridPosition;

        //Ajusto los datos a la nueva posicion
        data.gridPosition = gridPosition;

        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, size);
        data.occupiedPositions = positionsToOccupy;

        placedObjects[gridPosition] = data;

        //Tengo que reposicionar todos los objetos que estan dentro de este
        for (int i = 0; i < data.placedInObjects.Count; i++)
        {
            //Calculo el offset de los muebles dentro con respecto al mueble
            Vector3Int offset = data.placedInObjects[i].gridPosition - oldGridPosition;

            if(isRotated != data.IsRotated)
            {
                Vector3Int rotatedOffset = offset;
                rotatedOffset.x = offset.z;
                rotatedOffset.z = offset.x;
                offset = rotatedOffset;
            }

            Vector3Int newGridPos = gridPosition + offset;

            data.placedInObjects[i].gridPosition = newGridPos;
            List<Vector3Int> inPositionsToOccupy = CalculatePositions(newGridPos, Vector3Int.one);
            data.placedInObjects[i].occupiedPositions = inPositionsToOccupy;
            data.placedInObjects[i].container = data;

            placedObjects[newGridPos] = data.placedInObjects[i];
        }

        data.IsRotated = isRotated;
    }

    public bool CanPlaceObejctAt(Vector3Int gridPosition, Vector3Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);

        //Check if positions to occupy are inside of the grid
        for (int a = 0; a < positionToOccupy.Count; a++)
        {
            if (
                positionToOccupy[a].x < minGridSize.x ||
                positionToOccupy[a].y < minGridSize.y ||
                positionToOccupy[a].z < minGridSize.z
                )
                return false;
            else if (
                positionToOccupy[a].x > maxGridSize.x ||
                positionToOccupy[a].y > maxGridSize.y ||
                positionToOccupy[a].z > maxGridSize.z
                )
                return false;
        }

        //Also check if there isn't other objects occuping the same positions
        for (int i = 0; i < placedObjects.Count; i++)
        {
            PlacementData actualData = placedObjects.ElementAt(i).Value;

            for(int a = 0; a < positionToOccupy.Count; a++)
            {
                for(int b = 0; b < actualData.occupiedPositions.Count; b++)
                {
                    if (positionToOccupy[a] == actualData.occupiedPositions[b])
                        return false;
                }
            }
        }

        return true;
    }

    //Sustituir los otros metodos "Get..." por el siguiente
    internal PlacementData GetPlacementData(Vector3Int gridPosition)
    {
        PlacementData returnedData = null;

        for (int i = 0; i < placedObjects.Count; i++)
        {
            PlacementData actualData = placedObjects.ElementAt(i).Value;
            Vector3Int actualKey = placedObjects.ElementAt(i).Key;

            for (int b = 0; b < actualData.occupiedPositions.Count; b++)
            {
                if (gridPosition == actualData.occupiedPositions[b])
                    returnedData = placedObjects[actualKey];
            }
        }

        return returnedData;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        Debug.Log($"Try to remove object at {gridPosition}");

        for (int i = 0; i < placedObjects.Count; i++)
        {
            PlacementData actualData = placedObjects.ElementAt(i).Value;
            Vector3Int actualKey = placedObjects.ElementAt(i).Key;

            for (int b = 0; b < actualData.occupiedPositions.Count; b++)
            {
                if (gridPosition == actualData.occupiedPositions[b])
                {
                    Debug.Log($"Object removed at {actualKey}");

                    //Tengo que eliminar del objeto que lo contiene, este objeto
                    actualData.container?.placedInObjects.Remove(actualData);

                    //Tengo que borrar de placedObjects tambien los objetos que esten dentro de este objeto
                    for(int e = 0; e < actualData.placedInObjects.Count; e++)
                    {
                        placedObjects.Remove(actualData.placedInObjects[e].gridPosition);
                    }

                    placedObjects.Remove(actualKey);
                }
            }
        }
    }

    public void SaveGridData(string keyName)
    {
        placedObjectsCount = placedObjects.Count;
        PlayerPrefs.SetInt(keyName + SaveDataUtility.GetMemberName(() => placedObjectsCount), placedObjectsCount);

        for(int i = 0; i < placedObjectsCount; i++) 
        {
            //Save dictionary key
            SaveDataUtility.SaveVector3Int(
                placedObjects.ElementAt(i).Key,
                keyName + SaveDataUtility.GetMemberName(() => placedObjects) + "key" + i
                );

            //Save dictionary value
            placedObjects.ElementAt(i).Value.SavePlacementData(
                keyName + SaveDataUtility.GetMemberName(() => placedObjects) + "value" + i);
        }
    }

    public void LoadGridData(string keyName) 
    {
        placedObjectsCount = PlayerPrefs.GetInt(keyName + SaveDataUtility.GetMemberName(() => placedObjectsCount));

        for(int i = 0; i < placedObjectsCount; i++) 
        {
            //Get dictionary key
            Vector3Int key = SaveDataUtility.LoadVector3Int(
                keyName + SaveDataUtility.GetMemberName(() => placedObjects) + "key" + i
                );

            //Get dictionary value
            PlacementData value = new PlacementData();
            value.LoadPlacementData(
                keyName + SaveDataUtility.GetMemberName(() => placedObjects) + "value" + i);

            placedObjects[key] = value;
        }

        for(int i = 0; i < placedObjectsCount; i++)
        {
            placedObjects.ElementAt(i).Value.LoadPlacedInAndContainer(
                keyName + SaveDataUtility.GetMemberName(() => placedObjects) + "value" + i,
                this);
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector3Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int z = 0; z < objectSize.z; z++)
            {
                for(int  y = 0; y < objectSize.y; y++)
                {
                    returnVal.Add(gridPosition + new Vector3Int(x, y, z));
                }
            }
        }
        return returnVal;
    }

    private PlacementData GetContainer(Vector3Int gridPosition)
    {
        Vector3Int containerGridPosition = gridPosition;
        containerGridPosition.y = 0;
        PlacementData container = GetPlacementData(containerGridPosition);
        return container;
    }
}

[Serializable]
public class PlacementData
{
    public Vector3Int gridPosition;
    public List<Vector3Int> occupiedPositions = new List<Vector3Int>();
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }
    public bool IsRotated;
    [SerializeReference] public List<PlacementData> placedInObjects = new List<PlacementData>();
    [SerializeReference] public PlacementData container;


    int occupiedPositionsCount = 0;
    int placedInObjectsCount = 0;

    public PlacementData() 
    {
        occupiedPositions = new List<Vector3Int>();
    }

    public PlacementData(Vector3Int gridPosition, List<Vector3Int> occupiedPositions,
        int iD, int placedObjectIndex, bool isRotated)
    {
        this.gridPosition = gridPosition;
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
        IsRotated = isRotated;
    }

    public void SavePlacementData(string keyName)
    {
        SaveDataUtility.SaveVector3Int(gridPosition, keyName + SaveDataUtility.GetMemberName(() => gridPosition));
        PlayerPrefs.SetInt(keyName + SaveDataUtility.GetMemberName(() => ID), ID);
        PlayerPrefs.SetInt(keyName + SaveDataUtility.GetMemberName(() => PlacedObjectIndex), PlacedObjectIndex);

        int r = IsRotated ? 1 : 0;
        PlayerPrefs.SetInt(keyName + SaveDataUtility.GetMemberName(() => IsRotated), r);

        occupiedPositionsCount = occupiedPositions.Count;
        PlayerPrefs.SetInt(
            keyName + SaveDataUtility.GetMemberName(() => occupiedPositionsCount),
            occupiedPositionsCount
            );

        for(int i = 0; i < occupiedPositions.Count; i++)
        {
            SaveDataUtility.SaveVector3Int(
                occupiedPositions[i],
                keyName + SaveDataUtility.GetMemberName(() => occupiedPositions) + i
                );
        }

        placedInObjectsCount = placedInObjects.Count;
        PlayerPrefs.SetInt(keyName + SaveDataUtility.GetMemberName(() => placedInObjectsCount),
            placedInObjectsCount);

        for(int e = 0; e < placedInObjectsCount; e++)
        {
            SaveDataUtility.SaveVector3Int(placedInObjects[e].gridPosition,
                keyName + SaveDataUtility.GetMemberName(() => placedInObjects) + e);
        }

        if (container != null)
            SaveDataUtility.SaveVector3Int(container.gridPosition,
                keyName + SaveDataUtility.GetMemberName(() => container));
    }

    public void LoadPlacementData(string keyName)
    {
        gridPosition = SaveDataUtility.LoadVector3Int(keyName + SaveDataUtility.GetMemberName(() => gridPosition));
        ID = PlayerPrefs.GetInt(keyName + SaveDataUtility.GetMemberName(() => ID));
        PlacedObjectIndex = PlayerPrefs.GetInt(keyName + SaveDataUtility.GetMemberName(() => PlacedObjectIndex));

        int r = PlayerPrefs.GetInt(keyName + SaveDataUtility.GetMemberName(() => IsRotated));
        IsRotated = r == 1 ? true : false;

        occupiedPositionsCount =
            PlayerPrefs.GetInt(keyName + SaveDataUtility.GetMemberName(() => occupiedPositionsCount));

        for(int i = 0; i < occupiedPositionsCount; i++) 
        {
            occupiedPositions.Add(
                SaveDataUtility.LoadVector3Int(keyName + SaveDataUtility.GetMemberName(() => occupiedPositions) + i)
                );
        }

        placedInObjectsCount =
            PlayerPrefs.GetInt(keyName + SaveDataUtility.GetMemberName(() => placedInObjectsCount));        
    }

    public void LoadPlacedInAndContainer(string keyName, GridData gridData)
    {
        for (int e = 0; e < placedInObjectsCount; e++)
        {
            Vector3Int objectKey = SaveDataUtility.LoadVector3Int(
                        keyName + SaveDataUtility.GetMemberName(() => placedInObjects) + e);

            if (gridData.placedObjects.ContainsKey(objectKey))
                placedInObjects.Add(
                    gridData.placedObjects[objectKey]);
            else
                Debug.Log($"Couldn't find any object with key {objectKey}");
        }

        Vector3Int containerKey =
            SaveDataUtility.LoadVector3Int(keyName + SaveDataUtility.GetMemberName(() => container));

        if (gridData.placedObjects.ContainsKey(containerKey))
        {
            if (containerKey != new Vector3Int(-13, -13, -13))
                container = gridData.placedObjects[containerKey];
        }
        else
            Debug.Log($"Couldn't find a container with key {containerKey}");
    }
}

public static class SaveDataUtility
{
    public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
    {
        MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static void SaveVector3Int(Vector3Int vector3Int, string keyName)
    {
        int x, y, z;
        x = vector3Int.x;
        y = vector3Int.y;
        z = vector3Int.z;

        PlayerPrefs.SetInt(keyName + "x", x);
        PlayerPrefs.SetInt(keyName + "y", y);
        PlayerPrefs.SetInt(keyName + "z", z);
    }

    public static Vector3Int LoadVector3Int(string keyName)
    {
        int x, y, z;
        Vector3Int result = new Vector3Int(-13, -13, -13);

        if(PlayerPrefs.HasKey(keyName + "x"))
        {
            x = PlayerPrefs.GetInt(keyName + "x");
            y = PlayerPrefs.GetInt(keyName + "y");
            z = PlayerPrefs.GetInt(keyName + "z");

            result = new Vector3Int(x, y, z);
        }

        return result;
    }
}