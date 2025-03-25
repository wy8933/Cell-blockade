using System;
using UnityEngine;

public class RemovingState : IBuildingState
{
    [Header("External Scripts")]
    [SerializeField] private ObjectPlacer objectPlacer;
    [SerializeField] private PreviewSystem previewSystem;

    [Header("Tower Info")]
    //-1 means null there's isn't a tower selected

    [SerializeField] private int selectedTowerIndex = -1;
    [SerializeField] private int towerID;

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

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

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
            selectedTowerIndex = selectedData.GetRepresentationIndex(gridPos);
            if (selectedTowerIndex == -1)
            {
                return;
            }
            selectedData.RemoveObjectAt(gridPos);
            objectPlacer.RemoveObjectAt(selectedTowerIndex);
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
