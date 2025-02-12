using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private TowerManager towerManager;

    [Header("TileMap Variables")]
    //[SerializeField] private List<GameObject> turretSelect;

    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap turretTilemap;
    //[SerializeField] private GameObject _selectedTowerGameObj;

    [Header("Highlighting")]
    [SerializeField] private GameObject cellIndicator;

    [Header("TowerPlacement")]
    [SerializeField] private TowerData towerInfo;
    //-1 means null there's isn't a tower selected 
    [SerializeField] private int selectedTower = -1;

    // Update is called once per frame
    void Update()
    {
        /*
        if (!EnemyWaveManager.Instance.isWaveActive)
        {
            PlaceTower();
            HighlightTile();
        }
        */
        PlaceTower();
        HighlightTile();
    }


    
    private void HighlightTile()
    {

        Vector3Int gridPos = grid.WorldToCell(CursorControl.Instance.GetMousePosTile());

        cellIndicator.transform.position = grid.CellToWorld(gridPos);


    }

    private void PlaceTower()
    {
        Vector3Int gridPos = grid.WorldToCell(CursorControl.Instance.GetMousePosTile());
        /*
        if ()
        {

        }
        */
    }
}
