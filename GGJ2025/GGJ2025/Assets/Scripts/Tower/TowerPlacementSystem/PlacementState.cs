using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementState : IBuildingState
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
    [SerializeField] private TowerInfo towerDataBase;
    [SerializeField] private GridData towerData;

    public PlacementState(ObjectPlacer objectPlacer, PreviewSystem previewSystem, int toweriD, Grid grid, TowerInfo towerDataBase, GridData towerData)
    {
        this.objectPlacer = objectPlacer;
        this.previewSystem = previewSystem;
        this.towerID = toweriD;
        this.grid = grid;
        this.towerDataBase = towerDataBase;
        this.towerData = towerData;

        selectedTowerIndex = towerDataBase.TowerList.FindIndex(data => data.ID == toweriD);
        if (selectedTowerIndex > -1)
        {
            //gridVisualiztion.SetActive(true);
            previewSystem.StartShowingPlacementPreview(towerDataBase.TowerList[selectedTowerIndex].TowerPrefab, towerDataBase.TowerList[selectedTowerIndex].Size);
        }
        else
        {
            throw new System.Exception($"No tower with ID {toweriD}");
        }

    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPos)
    {

        bool placementValidity = CheckPlaceValidity(gridPos, selectedTowerIndex);

        if (!placementValidity)
        {
            return;
        }

        int index = objectPlacer.PlaceObject(towerDataBase.TowerList[selectedTowerIndex].TowerPrefab, grid.CellToWorld(gridPos));

        GridData selectedData = towerDataBase.TowerList[selectedTowerIndex].ID == 0 ? towerData : towerData;
        selectedData.AddObjectAt(gridPos, towerDataBase.TowerList[selectedTowerIndex].Size, towerDataBase.TowerList[selectedTowerIndex].ID, index);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPos), false);
    }

    private bool CheckPlaceValidity(Vector3Int gridPos, int selectedTowerIndex)
    {
        GridData selectedData = towerDataBase.TowerList[selectedTowerIndex].ID == 0 ? towerData : towerData;

        return selectedData.CanPlaceObjectAt(gridPos, towerDataBase.TowerList[selectedTowerIndex].Size);
    }

    public void UpdateState(Vector3Int gridPos)
    {
        bool placementValidity = CheckPlaceValidity(gridPos, selectedTowerIndex);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPos), placementValidity);
    }


}
