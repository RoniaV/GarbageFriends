using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3Int gridPosition, GridData selectedData);
    void UpdateState(Vector3Int gridPosition, GridData selectedData);
}