using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] ObjectsDatabaseSO database;
    [SerializeField] OrderInLayerComputer layerComputer;
    [Header("Debug List")]
    [SerializeField] List<GameObject> placedGameObjects = new();

    public int PlaceObject(ObjectData objectData, Vector3Int position, bool isRotated)
    {
        //Instantiate and position prefab
        GameObject newObject = Instantiate(objectData.GetRotatedPrefab(isRotated));
        newObject.transform.position = position;

        //Set Order in layer to gameObject SpriteRenderer
        SpriteRenderer spriteRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sortingOrder = 
            layerComputer.GetOrderInLayer(position, objectData.GetRotatedSize(isRotated));

        //Add to list and return index
        placedGameObjects.Add(newObject);
        return placedGameObjects.Count - 1;
    }

    public void PlacePlacementData(PlacementData placementData)
    {
        //Checks that current index is empty
        if (placementData.PlacedObjectIndex < placedGameObjects.Count &&
            placedGameObjects[placementData.PlacedObjectIndex] != null)
        {
            Debug.Log("Already Has an Object");
            return;
        }

        //Instantiate and position prefab
        Debug.Log($"Placing Object at Index {placementData.PlacedObjectIndex}");
        GameObject newObject = Instantiate(
            database.GetObjectDataWithID(placementData.ID).GetRotatedPrefab(placementData.IsRotated));
        newObject.transform.position = placementData.gridPosition;

        //Set order in layer
        SpriteRenderer spriteRenderer = newObject.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sortingOrder = layerComputer.GetOrderInLayer(placementData.gridPosition,
            database.GetObjectDataWithID(placementData.ID).GetRotatedSize(placementData.IsRotated));

        SetObjectOnList(placementData, newObject);
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex 
            || placedGameObjects[gameObjectIndex] == null)
            return;
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }


    private void SetObjectOnList(PlacementData placementData, GameObject instantiatedObject)
    {
        if (placementData.PlacedObjectIndex > placedGameObjects.Count - 1)
        {
            List<GameObject> fixedPlacedObjects = new List<GameObject>();

            for (int i = 0; i <= placementData.PlacedObjectIndex; i++)
            {
                fixedPlacedObjects.Add(null);
            }

            for (int i = 0; i < placedGameObjects.Count; i++)
            {
                if (placedGameObjects[i] != null)
                    fixedPlacedObjects[i] = placedGameObjects[i];
            }

            fixedPlacedObjects[placementData.PlacedObjectIndex] = instantiatedObject;

            placedGameObjects = new List<GameObject>(fixedPlacedObjects);
        }
        else
            placedGameObjects[placementData.PlacedObjectIndex] = instantiatedObject;
    }
}
