using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public event Action<int> OnObjectPlaced;
    public event Action<int> OnObjectErased;

    public int selectedObjectID { get; private set; }

    #region Inspector
    [Header("Components")]
    [SerializeField] InputManager inputManager;
    [SerializeField] Grid grid;
    [SerializeField] ObjectsDatabaseSO database;
    [SerializeField] GameObject gridVisualization;
    [SerializeField] PreviewSystem preview;
    [SerializeField] ObjectPlacer objectPlacer;
    [SerializeField] SoundFeedback soundFeedback;
    [SerializeField] SavedObject savedObject;
    [SerializeField] GUIManager guiManager;
    [SerializeField] PlacementUI placementUI;
    [SerializeField] RadioSystem radioSystem;

    [Header("Floor Settings")]
    [SerializeField] Vector3Int maxFloorGridPosition;
    [SerializeField] Vector3Int minFloorGridPosition;
    [SerializeField] int maxFloorID;

    [Header("Wall Settings")]
    [SerializeField] Vector3Int maxWallGridPosition;
    [SerializeField] Vector3Int minWallGridPosition;
    [SerializeField] int maxWallID;

    [Header("Furniture Settings")]
    [SerializeField] Vector3Int maxFurnitureGridPosition;
    [SerializeField] Vector3Int minFurnitureGridPosition;

    [Header("Debug")]
    [SerializeField] List<PlacementData> furniturePlacementData;
    #endregion

    private GridData floorData, furnitureData, wallData;

    private Vector3Int lastDetectedGridPosition = Vector3Int.zero;
    private FurnitureLayers lastDetectedLayer = default(FurnitureLayers);

    private bool placing = false;
    private bool picking = false;

    private PlacementData pickedPlacementData = null;
    private Vector3Int pickedObjectLastPosition = Vector3Int.zero;

    private GridData pickedObjectData = null;

    private bool selectedObjectIsRotated = false;


    private void Start()
    {
        gridVisualization.SetActive(false);
        floorData = new GridData(minFloorGridPosition, maxFloorGridPosition);
        furnitureData = new GridData(minFurnitureGridPosition, maxFurnitureGridPosition);
        wallData = new GridData(minWallGridPosition, maxWallGridPosition);
        LoadGridData();
        selectedObjectID = -1;
    }

    private void Update()
    {
        furniturePlacementData.Clear();
        for (int i = 0; i < furnitureData.placedObjects.Count; i++)
        {
            furniturePlacementData.Add(furnitureData.placedObjects.ElementAt(i).Value);
        }


        MapInfo mouseMapInfo = inputManager.GetSelectedMapInfo();
        Vector3 mousePosition = mouseMapInfo.mapPosition;
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedGridPosition != gridPosition)
        {
            if (placing)
            {
                bool isRotated = selectedObjectIsRotated;

                if (selectedObjectID >= maxFloorID && selectedObjectID < maxWallID)
                {
                    if (mouseMapInfo.layer == FurnitureLayers.rightWall)
                        selectedObjectIsRotated = false;
                    else if (mouseMapInfo.layer == FurnitureLayers.leftWall)
                        selectedObjectIsRotated = true;
                }

                if (isRotated == selectedObjectIsRotated)
                {
                    bool placementValidity = 
                        CheckPlacementValidity(gridPosition, selectedObjectID, mouseMapInfo.layer);

                    preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity, mouseMapInfo.layer);
                }
                else if(isRotated != selectedObjectIsRotated)
                {
                    preview.StopShowingPreview();

                    ObjectData objectData = database.GetObjectDataWithID(selectedObjectID);

                    preview.StartShowingPlacementPreview(objectData.GetRotatedSize(selectedObjectIsRotated));
                }
            }
            else if (picking)
            {
                bool validity = CheckPickValidity(gridPosition, mouseMapInfo.layer);
                preview.UpdatePosition(grid.CellToWorld(gridPosition), validity, mouseMapInfo.layer);
            }

            lastDetectedGridPosition = gridPosition;
            lastDetectedLayer = mouseMapInfo.layer;
        }
    }

    #region Public Methods
    public void StartPlacement(int ID)
    {
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;

        selectedObjectID = ID;

        ObjectData objectData = database.GetObjectDataWithID(selectedObjectID);

        if (objectData != null)
        {
            placing = true;

            preview.StartShowingPlacementPreview(objectData.GetRotatedSize(selectedObjectIsRotated));
            guiManager.SwitchGUI(GUIType.Placement);
            placementUI.SetObjectIcon(selectedObjectID, selectedObjectIsRotated);
            gridVisualization.SetActive(true);
            radioSystem.PlayAmbientSound();
        }
        else
            EndPlacementState();
    }

    public void StartMoving()
    {
        inputManager.OnClicked += PickStructure;
        inputManager.OnExit += StopPlacement;

        picking = true;
        pickedObjectLastPosition = Vector3Int.zero;
        pickedPlacementData = null;
        pickedObjectData = null;

        preview.StartShowingRemovePreview();
        guiManager.SwitchGUI(GUIType.Pick);
        gridVisualization.SetActive(true);
        radioSystem.PlayAmbientSound();
    }

    public void DeleteSelectedObject()
    {
        OnObjectErased?.Invoke(selectedObjectID);

        selectedObjectID = -1;

        pickedObjectLastPosition = Vector3Int.zero;
        pickedPlacementData = null;
        pickedObjectData = null;

        selectedObjectIsRotated = false;

        soundFeedback.PlaySound(SoundType.Remove);

        SaveGridData();

        if (picking)
        {
            EndPlacementState();
            StartMoving();
        }
        else if (!picking)
        {
            StopPlacement();
        }
    }

    public void RotateSelectedObject()
    {
        if (selectedObjectID > -1)
        {
            selectedObjectIsRotated = !selectedObjectIsRotated;
            preview.StopShowingPreview();

            ObjectData objectData = database.GetObjectDataWithID(selectedObjectID);

            preview.StartShowingPlacementPreview(objectData.GetRotatedSize(selectedObjectIsRotated));
            placementUI.SetObjectIcon(selectedObjectID, selectedObjectIsRotated);
            soundFeedback.PlaySound(SoundType.RotateSound);
        }
        else
            Debug.Log("No selected object to Rotate");
    }

    public void StopPlacement()
    {
        EndPlacementState();
        EndPickState();
        guiManager.SwitchGUI(GUIType.Game);
        selectedObjectIsRotated = false;
        soundFeedback.PlaySound(SoundType.StopPlacement);
        radioSystem.StopAmbientSound();

        if (picking)
        {
            picking = false;

            if (selectedObjectID > -1)
            {
                ObjectData objectData = database.GetObjectDataWithID(selectedObjectID);

                pickedObjectData.AddObjectAt(
                    pickedObjectLastPosition,
                    objectData.GetRotatedSize(selectedObjectIsRotated),
                    objectData.ID,
                    pickedPlacementData.PlacedObjectIndex,
                    selectedObjectIsRotated);

                PlacePickedPlacementData();
            }
        }
    }
    #endregion

    #region Placement Methods
    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
            return;

        Debug.Log("Placement Action");

        MapInfo mouseMapInfo = inputManager.GetSelectedMapInfo();
        Vector3 mousePosition = mouseMapInfo.mapPosition;
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectID, mouseMapInfo.layer);
        if (placementValidity == false)
        {
            Debug.Log("Wrong placement");
            soundFeedback.PlaySound(SoundType.WrongPlacement);
            return;
        }
        soundFeedback.PlaySound(SoundType.Place);

        GridData selectedData = GetPlacementGridData(selectedObjectID, mouseMapInfo.layer);
        if (selectedData == null)
        {
            Debug.Log("No data selected");
            return;
        }

        if (mouseMapInfo.layer == FurnitureLayers.rightWall)
        {
            Debug.Log("Place on RightWall");
            selectedObjectIsRotated = false;
        }
        else if (mouseMapInfo.layer == FurnitureLayers.leftWall)
        {
            Debug.Log("Place on LeftWall");
            selectedObjectIsRotated = true;
        }

        int placedIndex = -1;
        ObjectData objectData = database.GetObjectDataWithID(selectedObjectID);

        if (!picking)
        {
            placedIndex = objectPlacer.PlaceObject(objectData,
            gridPosition, selectedObjectIsRotated);

            selectedData.AddObjectAt(gridPosition,
                objectData.GetRotatedSize(selectedObjectIsRotated),
                objectData.ID,
                placedIndex,
                selectedObjectIsRotated);
        }
        else if(picking)
        {
            selectedData.AddPlacementData(
                gridPosition,
                objectData.GetRotatedSize(selectedObjectIsRotated),
                pickedPlacementData,
                selectedObjectIsRotated);

            PlacePickedPlacementData();
        }

        OnObjectPlaced?.Invoke(selectedObjectID);

        selectedObjectID = -1;
        SaveGridData();

        if (picking)
        {
            EndPlacementState();
            StartMoving();
        }
        else if(!picking) 
        {
            StopPlacement();
        }
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectID, FurnitureLayers mapLayer)
    {
        GridData selectedData = GetPlacementGridData(selectedObjectID, mapLayer);

        if (selectedData == null)
            return false;

        ObjectData objectData = database.GetObjectDataWithID(selectedObjectID);

        if (gridPosition.y > 0 && !objectData.CanBeOnTop)
            return false;
        else
            return selectedData.CanPlaceObejctAt(gridPosition, objectData.GetRotatedSize(selectedObjectIsRotated));
    }

    private GridData GetPlacementGridData(int selectedObjectID, FurnitureLayers mapLayer)
    {
        GridData returnedData = null;

        //Debug.Log("Selected Object ID: " +  selectedObjectID);
        //Debug.Log("Map Layer: " +  mapLayer);

        if (selectedObjectID > -1 && selectedObjectID < maxFloorID &&
            mapLayer == FurnitureLayers.floor)
            returnedData = floorData;
        else if (selectedObjectID >= maxFloorID && selectedObjectID < maxWallID &&
            (mapLayer == FurnitureLayers.rightWall || mapLayer == FurnitureLayers.leftWall))
            returnedData = wallData;
        else if (selectedObjectID >= maxWallID && mapLayer == FurnitureLayers.floor)
            returnedData = furnitureData;
        else
            returnedData = null;

        return returnedData;
    }

    private void EndPlacementState()
    {
        placing = false;

        gridVisualization.SetActive(false);
        selectedObjectIsRotated = false;

        if (!picking && selectedObjectID > -1)
        {
            savedObject.SaveObjectByID(selectedObjectID);
            selectedObjectID = -1;
        }

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedGridPosition = Vector3Int.zero;
        preview.StopShowingPreview();
    }
    #endregion

    #region Pick Methods
    private void PickStructure()
    {
        if (inputManager.IsPointerOverUI())
            return;

        Debug.Log("Pick Action");

        MapInfo mouseMapInfo = inputManager.GetSelectedMapInfo();
        Vector3 mousePosition = mouseMapInfo.mapPosition;
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        GridData selectedData = GetPickGridData(gridPosition, mouseMapInfo.layer);

        if (selectedData?.CanPlaceObejctAt(gridPosition, Vector3Int.one) == true)
            selectedData = null;

        if (selectedData == null)
        {
            Debug.Log("Wrong pick");
            soundFeedback.PlaySound(SoundType.WrongPlacement);
        }
        else
        {
            soundFeedback.PlaySound(SoundType.Pick);

            pickedPlacementData = selectedData.GetPlacementData(gridPosition);
            if (pickedPlacementData.PlacedObjectIndex == -1)
                return;

            Debug.Log($"Picked object with ID {pickedPlacementData.ID}");
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(pickedPlacementData.PlacedObjectIndex);

            for(int i = 0; i < pickedPlacementData.placedInObjects.Count; i++)
            {
                objectPlacer.RemoveObjectAt(pickedPlacementData.placedInObjects[i].PlacedObjectIndex);
            }

            pickedObjectLastPosition = gridPosition;
            pickedObjectData = selectedData;

            selectedObjectIsRotated = pickedPlacementData.IsRotated;

            EndPickState();
            StartPlacement(pickedPlacementData.ID);
        }
    }

    private bool CheckPickValidity(Vector3Int gridPosition, FurnitureLayers mapLayer)
    {
        GridData selectedData = GetPickGridData(gridPosition, mapLayer);

        if (selectedData == null)
            return false;

        return !(selectedData.CanPlaceObejctAt(gridPosition, Vector3Int.one));
    }

    private GridData GetPickGridData(Vector3Int gridPosition, FurnitureLayers mapLayer)
    {
        GridData returnedData = null;

        switch (mapLayer)
        {
            case FurnitureLayers.floor:
                if (furnitureData.CanPlaceObejctAt(gridPosition, Vector3Int.one) == false)
                    returnedData = furnitureData;
                else if (floorData.CanPlaceObejctAt(gridPosition, Vector3Int.one) == false)
                    returnedData = floorData;
                break;
            case FurnitureLayers.rightWall:
                returnedData = wallData;
                break;
            case FurnitureLayers.leftWall:
                returnedData = wallData;
                break;
            default:
                returnedData = null;
                break;
        }

        return returnedData;
    }

    private void EndPickState()
    {
        gridVisualization.SetActive(false);

        inputManager.OnClicked -= PickStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedGridPosition = Vector3Int.zero;
        preview.StopShowingPreview();
    }
    #endregion

    private void PlacePickedPlacementData()
    {
        objectPlacer.PlacePlacementData(pickedPlacementData);

        for (int i = 0; i < pickedPlacementData.placedInObjects.Count; i++)
            objectPlacer.PlacePlacementData(pickedPlacementData.placedInObjects[i]);
    }

    #region Save, Load and Place Saved Grid System
    public void SaveGridData()
    {
        floorData.SaveGridData(SaveDataUtility.GetMemberName(() => floorData));
        furnitureData.SaveGridData(SaveDataUtility.GetMemberName(() => furnitureData));
        wallData.SaveGridData(SaveDataUtility.GetMemberName(() => wallData));
    }

    private void LoadGridData()
    {
        floorData.LoadGridData(SaveDataUtility.GetMemberName(() => floorData));
        furnitureData.LoadGridData(SaveDataUtility.GetMemberName(() => furnitureData));
        wallData.LoadGridData(SaveDataUtility.GetMemberName(() => wallData));

        PlaceSavedGridData(floorData);
        PlaceSavedGridData(furnitureData);
        PlaceSavedGridData(wallData);
    }

    private void PlaceSavedGridData(GridData gridData)
    {
        for (int i = 0; i < gridData.placedObjects.Count; i++)
        {
            objectPlacer.PlacePlacementData(gridData.placedObjects.ElementAt(i).Value);
        }
    }
    #endregion
}
