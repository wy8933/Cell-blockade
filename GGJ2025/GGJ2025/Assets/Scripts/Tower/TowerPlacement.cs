using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    public static TowerPlacement Instance;

    //[SerializeField] private PlayerInputManager _playerInputManager;

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


    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        StopPlacement();

    }

    /// <summary>
    /// 
    /// </summary>
    public void StartPlacement()
    {
        StopPlacement();
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
        Vector3Int gridPos = grid.WorldToCell(CursorControl.Instance.GetMousePos());
        if (IsPointerOverUI())
        {
            return;
        }
        GameObject newObject = Instantiate(towerInfo.TowerList[selectedTower].towerPrefab);
        newObject.transform.position = grid.CellToWorld(gridPos);
    }

    public void GetTowerPrefab(int ID)
    {
        int temp;
        if ((temp = towerInfo.TowerList.FindIndex(data => data.towerId == ID)) > 0)
        {
            selectedTower = temp;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
        HighlightTile();

        if (selectedTower != -1)
        {
            StartPlacement();
        }
    }


    
    private void HighlightTile()
    {
        Vector3Int gridPos = grid.WorldToCell(CursorControl.Instance.GetMousePos());

        cellIndicator.transform.position = grid.CellToWorld(gridPos);

    }

    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
}
