//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlacementState : IBuildingState
//{
//    private int selectedObjectIndex = -1;
//    int ID;
//    Grid grid;
//    PreviewSystem previewSystem;
//    ObjectsDatabaseSO database;
//    ObjectPlacer objectPlacer;
//    SoundFeedback soundFeedback;
//    PlacementSystem placementSystem;
//    SavedObject savedObject;

//    public PlacementState(int iD,
//                          Grid grid,
//                          PreviewSystem previewSystem,
//                          ObjectsDatabaseSO database,
//                          ObjectPlacer objectPlacer,
//                          SoundFeedback soundFeedback,
//                          PlacementSystem placementSystem,
//                          SavedObject savedObject)
//    {
//        ID = iD;
//        this.grid = grid;
//        this.previewSystem = previewSystem;
//        this.database = database;
//        this.objectPlacer = objectPlacer;
//        this.soundFeedback = soundFeedback;
//        this.placementSystem = placementSystem;
//        this.savedObject = savedObject;

//        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
//        if (selectedObjectIndex > -1)
//        {
//            previewSystem.StartShowingPlacementPreview(
//                database.objectsData[selectedObjectIndex].Prefab,
//                database.objectsData[selectedObjectIndex].Size);
//        }
//        else
//        {
//            placementSystem.StopPlacement();
//            throw new System.Exception($"No object with ID {iD}");
//        }
//    }

//    public void EndState()
//    {
//        if (selectedObjectIndex > -1)
//            savedObject.SaveObjectByID(database.objectsData[selectedObjectIndex].ID);

//        previewSystem.StopShowingPreview();
//    }

//    public void OnAction(Vector3Int gridPosition, GridData selectedData)
//    {
//        Debug.Log("Placement Action");

//        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
//        if (placementValidity == false)
//        {
//            Debug.Log("Wrong placement");
//            soundFeedback.PlaySound(SoundType.wrongPlacement);
//            return;
//        }
//        soundFeedback.PlaySound(SoundType.Place);
//        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab,
//            grid.CellToWorld(gridPosition));

//        GridData selectedData;
//        if (database.objectsData[selectedObjectIndex].ID < maxFloorID)
//            selectedData = floorData;
//        else if (database.objectsData[selectedObjectIndex].ID >= maxFloorID &&
//            database.objectsData[selectedObjectIndex].ID < maxWallID)
//            selectedData = wallData;
//        else 
//            selectedData = furnitureData;

//        selectedData.AddObjectAt(gridPosition,
//            database.objectsData[selectedObjectIndex].Size,
//            database.objectsData[selectedObjectIndex].ID,
//            index);

//        selectedObjectIndex = -1;

//        previewSystem.StopShowingPreview();

//        placementSystem.SaveGridData();
//        placementSystem.StopPlacement();
//    }

//    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
//    {
//        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
//            floorData :
//            furnitureData;

//        return selectedData.CanPlaceObejctAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
//    }

//    public void UpdateState(Vector3Int gridPosition, GridData selectedData)
//    {
//        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

//        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
//    }
//}
