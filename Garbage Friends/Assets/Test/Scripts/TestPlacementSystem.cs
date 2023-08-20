using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine;

public class TestPlacementSystem : MonoBehaviour
{
    //public int selectedObjectID;

    //#region Inspector
    //[Header("Components")]
    //[SerializeField] InputManager inputManager;
    //[SerializeField] Grid grid;
    //[SerializeField] ObjectsDatabaseSO database;
    //[SerializeField] GameObject gridVisualization;
    //[SerializeField] PreviewSystem preview;
    //[SerializeField] ObjectPlacer objectPlacer;
    //[SerializeField] SoundFeedback soundFeedback;
    //[SerializeField] SavedObject savedObject;

    //[Header("Floor Settings")]
    //[SerializeField] Vector3Int maxFloorGridPosition;
    //[SerializeField] Vector3Int minFloorGridPosition;
    //[SerializeField] int maxFloorID;

    //[Header("Wall Settings")]
    //[SerializeField] Vector3Int maxWallGridPosition;
    //[SerializeField] Vector3Int minWallGridPosition;
    //[SerializeField] int maxWallID;

    //[Header("Furniture Settings")]
    //[SerializeField] Vector3Int maxFurnitureGridPosition;
    //[SerializeField] Vector3Int minFurnitureGridPosition;

    //[Header("Debug")]
    //[SerializeField] List<PlacementData> furniturePlacementData;
    //#endregion

    //private GridData floorData, furnitureData, wallData;
    //private Vector3Int lastDetectedGridPosition = Vector3Int.zero;
    //private FurnitureLayers lastDetectedLayer = default(FurnitureLayers);
    //private bool placing = false;
    //private bool picking = false;
    //private Vector3Int pickedObjectPosition = Vector3Int.zero;
    //private int pickedObjectPlacedIndex = -1;
    //private GridData pickedObjectData = null;


    //private void Start()
    //{
    //    gridVisualization.SetActive(false);
    //    floorData = new GridData(minFloorGridPosition, maxFloorGridPosition);
    //    furnitureData = new GridData(minFurnitureGridPosition, maxFurnitureGridPosition);
    //    wallData = new GridData(minWallGridPosition, maxWallGridPosition);
    //    LoadGridData();
    //}

    //private void Update()
    //{
    //    furniturePlacementData.Clear();
    //    for (int i = 0; i < furnitureData.placedObjects.Count; i++)
    //    {
    //        furniturePlacementData.Add(furnitureData.placedObjects.ElementAt(i).Value);
    //    }


    //    MapInfo mouseMapInfo = inputManager.GetSelectedMapInfo();
    //    Vector3 mousePosition = mouseMapInfo.mapPosition;
    //    Vector3Int gridPosition = grid.WorldToCell(mousePosition);

    //    if (lastDetectedGridPosition != gridPosition)
    //    {
    //        if (placing)
    //        {
    //            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectID, mouseMapInfo.layer);

    //            preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    //        }
    //        else if (picking)
    //        {
    //            bool validity = CheckPickValidity(gridPosition, mouseMapInfo.layer);
    //            preview.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    //        }

    //        lastDetectedGridPosition = gridPosition;
    //        lastDetectedLayer = mouseMapInfo.layer;
    //    }
    //}

    //#region Public Methods
    //public void StartPlacement(int ID)
    //{
    //    inputManager.OnClicked += PlaceStructure;
    //    inputManager.OnExit += StopPlacement;

    //    selectedObjectID = ID;

    //    int selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
    //    if (selectedObjectIndex > -1)
    //    {
    //        placing = true;
    //        preview.StartShowingPlacementPreview(
    //            database.objectsData[selectedObjectIndex].Prefab,
    //            database.objectsData[selectedObjectIndex].Size);
    //    }
    //    else
    //    {
    //        EndPlacementState();
    //        throw new System.Exception($"No object with ID {ID}");
    //    }
    //}

    //public void StartMoving()
    //{
    //    inputManager.OnClicked += PickStructure;
    //    inputManager.OnExit += StopPlacement;

    //    pickedObjectPosition = Vector3Int.zero;
    //    pickedObjectPlacedIndex = -1;
    //    pickedObjectData = null;

    //    preview.StartShowingRemovePreview();
    //}

    //public void DeleteSelectedObject()
    //{
        
    //}

    //public void StopPlacement()
    //{
    //    EndPlacementState();
    //    EndPickState();

    //    if(picking)
    //    {
    //        picking = false;

    //        if (selectedObjectID > -1)
    //        {
    //            int databaseIndex = database.objectsData.FindIndex(data => data.ID == selectedObjectID);

    //            objectPlacer.PlaceObject(database.objectsData[databaseIndex].Prefab,
    //                grid.CellToWorld(pickedObjectPosition));

    //            pickedObjectData.AddObjectAt(pickedObjectPosition,
    //                database.objectsData[databaseIndex].Size,
    //                database.objectsData[databaseIndex].ID,
    //                pickedObjectPlacedIndex);
    //        }
    //    }
    //}
    //#endregion

    //#region Placement Methods
    //private void PlaceStructure()
    //{
    //    Debug.Log("Placement Action");

    //    MapInfo mouseMapInfo = inputManager.GetSelectedMapInfo();
    //    Vector3 mousePosition = mouseMapInfo.mapPosition;
    //    Vector3Int gridPosition = grid.WorldToCell(mousePosition);

    //    bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectID, mouseMapInfo.layer);
    //    if (placementValidity == false)
    //    {
    //        Debug.Log("Wrong placement");
    //        soundFeedback.PlaySound(SoundType.wrongPlacement);
    //        return;
    //    }
    //    soundFeedback.PlaySound(SoundType.Place);

    //    GridData selectedData = GetPlacementGridData(selectedObjectID, mouseMapInfo.layer);
    //    if (selectedData != null)
    //        return;

    //    int databaseIndex = database.objectsData.FindIndex(data => data.ID == selectedObjectID);

    //    int placedIndex = objectPlacer.PlaceObject(database.objectsData[databaseIndex].Prefab,
    //        grid.CellToWorld(gridPosition));

    //    selectedData.AddObjectAt(gridPosition,
    //        database.objectsData[databaseIndex].Size,
    //        database.objectsData[databaseIndex].ID,
    //        placedIndex);

    //    selectedObjectID = -1;
    //    SaveGridData();
    //    EndPlacementState();

    //    if (picking)
    //        StartMoving();
    //}

    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectID, FurnitureLayers mapLayer)
    //{
    //    GridData selectedData = GetPlacementGridData(selectedObjectID, mapLayer);

    //    if (selectedData == null)
    //        return false;

    //    int selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == selectedObjectID);

    //    return selectedData.CanPlaceObejctAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    //}

    //private GridData GetPlacementGridData(int selectedObjectID, FurnitureLayers mapLayer)
    //{
    //    GridData returnedData = null;

    //    if (selectedObjectID > 0 && selectedObjectID < maxFloorID &&
    //        mapLayer == FurnitureLayers.floor)
    //        returnedData = floorData;
    //    else if (selectedObjectID >= maxFloorID && selectedObjectID < maxWallID &&
    //        mapLayer == FurnitureLayers.rightWall || mapLayer == FurnitureLayers.leftWall)
    //        returnedData = wallData;
    //    else if (selectedObjectID >= maxWallID && mapLayer == FurnitureLayers.floor)
    //        returnedData = furnitureData;
    //    else
    //        returnedData = null;

    //    return returnedData;
    //}

    //private void EndPlacementState()
    //{
    //    placing = false;

    //    soundFeedback.PlaySound(SoundType.Click);
    //    gridVisualization.SetActive(false);

    //    if (!picking)
    //    {
    //        savedObject.SaveObjectByID(selectedObjectID);
    //        selectedObjectID = -1;
    //    }

    //    inputManager.OnClicked -= PlaceStructure;
    //    inputManager.OnExit -= StopPlacement;
    //    lastDetectedGridPosition = Vector3Int.zero;
    //    preview.StopShowingPreview();
    //}
    //#endregion

    //#region Pick Methods
    //private void PickStructure()
    //{
    //    Debug.Log("Pick Action");

    //    MapInfo mouseMapInfo = inputManager.GetSelectedMapInfo();
    //    Vector3 mousePosition = mouseMapInfo.mapPosition;
    //    Vector3Int gridPosition = grid.WorldToCell(mousePosition);

    //    GridData selectedData = GetPickGridData(gridPosition, mouseMapInfo.layer);

    //    if (selectedData.CanPlaceObejctAt(gridPosition, Vector2Int.one) == true)
    //        selectedData = null;

    //    if (selectedData == null)
    //    {
    //        Debug.Log("Wrong pick");
    //        soundFeedback.PlaySound(SoundType.wrongPlacement);
    //    }
    //    else
    //    {
    //        soundFeedback.PlaySound(SoundType.Pick);
    //        int placedObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
    //        int pickedObjectID = selectedData.GetObjectID(gridPosition);
    //        if (placedObjectIndex == -1)
    //            return;

    //        Debug.Log($"Picked object with ID {pickedObjectID}");
    //        selectedData.RemoveObjectAt(gridPosition);
    //        objectPlacer.RemoveObjectAt(placedObjectIndex);

    //        pickedObjectPosition = gridPosition;
    //        pickedObjectPlacedIndex = placedObjectIndex;
    //        pickedObjectData = selectedData;

    //        EndPickState();
    //        StartPlacement(pickedObjectID);
    //    }
    //}

    //private bool CheckPickValidity(Vector3Int gridPosition, FurnitureLayers mapLayer)
    //{
    //    GridData selectedData = GetPickGridData(gridPosition, mapLayer);

    //    if (selectedData == null)
    //        return false;

    //    return !(selectedData.CanPlaceObejctAt(gridPosition, Vector2Int.one));
    //}

    //private GridData GetPickGridData(Vector3Int gridPosition, FurnitureLayers mapLayer)
    //{
    //    GridData returnedData = null;

    //    switch (mapLayer)
    //    {
    //        case FurnitureLayers.floor:
    //            if (furnitureData.CanPlaceObejctAt(gridPosition, Vector2Int.one) == false)
    //                returnedData = furnitureData;
    //            else if (floorData.CanPlaceObejctAt(gridPosition, Vector2Int.one) == false)
    //                returnedData = floorData;
    //            break;
    //        case FurnitureLayers.rightWall:
    //            returnedData = wallData;
    //            break;
    //        case FurnitureLayers.leftWall:
    //            returnedData = wallData;
    //            break;
    //        default:
    //            returnedData = null;
    //            break;
    //    }

    //    return returnedData;
    //}

    //private void EndPickState()
    //{
    //    soundFeedback.PlaySound(SoundType.Click);
    //    gridVisualization.SetActive(false);

    //    inputManager.OnClicked -= PickStructure;
    //    inputManager.OnExit -= StopPlacement;
    //    lastDetectedGridPosition = Vector3Int.zero;
    //    preview.StopShowingPreview();
    //}
    //#endregion

    //#region Save, Load and Place Saved Grid System
    //public void SaveGridData()
    //{
    //    floorData.SaveGridData(SaveDataUtility.GetMemberName(() => floorData));
    //    furnitureData.SaveGridData(SaveDataUtility.GetMemberName(() => furnitureData));
    //    wallData.SaveGridData(SaveDataUtility.GetMemberName(() => wallData));
    //}

    //private void LoadGridData()
    //{
    //    floorData.LoadGridData(SaveDataUtility.GetMemberName(() => floorData));
    //    furnitureData.LoadGridData(SaveDataUtility.GetMemberName(() => furnitureData));
    //    wallData.LoadGridData(SaveDataUtility.GetMemberName(() => wallData));

    //    PlaceSavedGridData(floorData);
    //    PlaceSavedGridData(furnitureData);
    //    PlaceSavedGridData(wallData);
    //}

    //private void PlaceSavedGridData(GridData gridData)
    //{
    //    for (int i = 0; i < gridData.placedObjects.Count; i++)
    //    {
    //        int objectIndex = database.objectsData.FindIndex(
    //            data => data.ID == gridData.placedObjects.ElementAt(i).Value.ID
    //            );


    //        objectPlacer.PlaceObjectAtIndex(
    //            database.objectsData[objectIndex].Prefab,
    //            grid.CellToWorld(gridData.placedObjects.ElementAt(i).Key),
    //            gridData.placedObjects.ElementAt(i).Value.PlacedObjectIndex
    //            );
    //    }
    //}
    //#endregion
}
