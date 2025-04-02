using System;
using UnityEngine;

public class RemovingState : IBuildingState
{
    [Header("External Scripts")]
    [SerializeField] private ObjectPlacer objectPlacer;
    [SerializeField] private PreviewSystem previewSystem;

    [Header("Tower Info")]
    //-1 means null there's isn't a tower selected

    [SerializeField] private int gameObjectIndex = -1;

    [Header("Tower Grid Info")]
    [SerializeField] private Grid grid;
    [SerializeField] private GridData towerData;

    public RemovingState(ObjectPlacer objectPlacer, PreviewSystem previewSystem, Grid grid, GridData towerData)
    {
        this.objectPlacer = objectPlacer;
        this.previewSystem = previewSystem;
        this.grid = grid;
        this.towerData = towerData;

        previewSystem.StartShowingrRemovePreview();
    }

    /// <summary>
    /// 
    /// </summary>
    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridPos"></param>
    public void OnAction(Vector3Int gridPos)
    {
        GridData selectedData = null;
        if (towerData.CanPlaceObjectAt(gridPos,Vector2Int.one) == false)
        {
            selectedData = towerData;
        }

        if (selectedData == null)
        {
            //sounds
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPos);
            if (gameObjectIndex == -1)
            {
                return;
            }
            selectedData.RemoveObjectAt(gridPos);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
        Vector3 cellPos = grid.CellToWorld(gridPos);
        previewSystem.UpdatePosition(cellPos, CheckIfSelectionIsValid(gridPos));
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPos)
    {
        return !(towerData.CanPlaceObjectAt(gridPos, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPos)
    {
        bool validity = CheckIfSelectionIsValid(gridPos);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPos), validity);
    }
}
