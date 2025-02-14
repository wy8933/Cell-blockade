using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private TowerManager _towerManager;

    [SerializeField] private PlayerInputManager _playerInputManager;

[Header("TileMap Variables")]
    //[SerializeField] private List<GameObject> turretSelect;

    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap turretTilemap;
    //[SerializeField] private GameObject _selectedTowerGameObj;

    [Header("Highlighting")]
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private GameObject gridVisualiztion;

    [Header("TowerPlacement")]

    [SerializeField] private TowerInfo towerInfo;
    //-1 means null there's isn't a tower selected 
    [SerializeField] private int selectedTower = -1;


    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        Vector3Int gridPos = grid.WorldToCell(CursorControl.Instance.GetMousePos());
        selectedTower = towerInfo.TowerList.FindIndex(data => data.towerId == ID);
        if (selectedTower < 0)
        {
            return;
        }
        gridVisualiztion.SetActive(true);
        cellIndicator.SetActive(true);
    }

    public void StopPlacement()
    {
        selectedTower = -1;
        gridVisualiztion.SetActive(false);
        cellIndicator.SetActive(false);
    }

    public void PlaceTower()
    {
        if (_playerInputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3Int gridPos = grid.WorldToCell(CursorControl.Instance.GetMousePos());
        GameObject newObject = Instantiate(towerInfo.TowerList[selectedTower].towerPrefab);
        newObject.transform.position = grid.CellToWorld(gridPos);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (!EnemyWaveManager.Instance.isWaveActive)
        {
            
        }
        */
        HighlightTile();
    }


    
    private void HighlightTile()
    {
        if (selectedTower < 0 || !EnemyWaveManager.Instance.isWaveActive)
        {

        }
        Vector3Int gridPos = grid.WorldToCell(CursorControl.Instance.GetMousePos());

        cellIndicator.transform.position = grid.CellToWorld(gridPos);

    }

    
}
