//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MovingState : IBuildingState
//{
//    private int gameObjectIndex = -1;
//    Grid grid;
//    ObjectsDatabaseSO database;
//    PreviewSystem previewSystem;
//    ObjectPlacer objectPlacer;
//    SoundFeedback soundFeedback;
//    PlacementSystem placementSystem;

//    private bool picking = true;
//    private int pickedObjectID = -1;

//    public MovingState(Grid grid,
//                     ObjectsDatabaseSO database,
//                     PreviewSystem previewSystem,
//                     ObjectPlacer objectPlacer,
//                     SoundFeedback soundFeedback,
//                     PlacementSystem placementSystem)
//    {
//        this.grid = grid;
//        this.database = database;
//        this.previewSystem = previewSystem;
//        this.objectPlacer = objectPlacer;
//        this.soundFeedback = soundFeedback;
//        this.placementSystem = placementSystem;

//        previewSystem.StartShowingRemovePreview();
//    }

//    public void EndState()
//    {
//        previewSystem.StopShowingPreview();
//    }

//    public void OnAction(Vector3Int gridPosition, GridData selectedData)
//    {
//        if (picking)
//            PickObject(gridPosition, selectedData);
//        else
//            DropObject(gridPosition, pickedObjectID, selectedData);
//    }

//    public void UpdateState(Vector3Int gridPosition, GridData selectedData)
//    {
//        if (picking)
//        {
//            bool validity = CheckIfPickSelectionIsValid(gridPosition, selectedData);
//            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
//        }
//        else
//        {
//            bool placementValidity = CheckPlacementValidity(gridPosition, pickedObjectID, selectedData);
//            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
//        }
//    }

//    private void PickObject(Vector3Int gridPosition, GridData selectedData)
//    {
//        if (selectedData.CanPlaceObejctAt(gridPosition, Vector3Int.one) == true)
//            selectedData = null;

//        if (selectedData == null)
//        {
//            //sound
//            soundFeedback.PlaySound(SoundType.WrongPlacement);
//        }
//        else
//        {
//            soundFeedback.PlaySound(SoundType.Pick);
//            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
//            pickedObjectID = selectedData.GetObjectID(gridPosition);
//            if (gameObjectIndex == -1)
//                return;
//            selectedData.RemoveObjectAt(gridPosition);
//            objectPlacer.RemoveObjectAt(gameObjectIndex);

//            Debug.Log($"Picked object with ID {pickedObjectID}");
//            picking = false;

//            int selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == pickedObjectID);
//            if (selectedObjectIndex > -1)
//            {
//                previewSystem.StopShowingPreview();
//                previewSystem.StartShowingPlacementPreview(
//                    database.objectsData[selectedObjectIndex].Prefab,
//                    database.objectsData[selectedObjectIndex].Size);

//                Vector3 cellPosition = grid.CellToWorld(gridPosition);
//                previewSystem.UpdatePosition(cellPosition, CheckPlacementValidity(gridPosition, pickedObjectID, selectedData));
//            }
//            else
//                throw new System.Exception($"Picked object with ID  {pickedObjectID}");
//        }
//    }

//    private bool CheckIfPickSelectionIsValid(Vector3Int gridPosition, GridData selectedData)
//    {
//        return !(selectedData.CanPlaceObejctAt(gridPosition, Vector3Int.one));
//    }

//    private void DropObject(Vector3Int gridPosition, int objectIndex, GridData selectedData) 
//    {
//        bool placementValidity = CheckPlacementValidity(gridPosition, objectIndex, selectedData);
//        if (placementValidity == false)
//        {
//            soundFeedback.PlaySound(SoundType.WrongPlacement);
//            return;
//        }
//        soundFeedback.PlaySound(SoundType.Place);
//        objectPlacer.PlaceObjectAtIndex(database.objectsData[objectIndex].Prefab,
//            grid.CellToWorld(gridPosition), gameObjectIndex);
                
//        selectedData.AddObjectAt(gridPosition,
//            database.objectsData[objectIndex].Size,
//            database.objectsData[objectIndex].ID,
//            gameObjectIndex,
//            false);

//        picking = true;
//        pickedObjectID = -1;
//        gameObjectIndex = -1;

//        previewSystem.StopShowingPreview();
//        previewSystem.StartShowingRemovePreview();
//        placementSystem.SaveGridData();

//        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), CheckIfPickSelectionIsValid(gridPosition, selectedData));
//    }

//    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex, GridData selectedData)
//    {
//        return selectedData.CanPlaceObejctAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
//    }
//}
