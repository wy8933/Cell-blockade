using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3Int gridPos);
    void OnAction(Vector3Int gridPos, Quaternion objectRotation);
    void UpdateState(Vector3Int gridPos);
}
