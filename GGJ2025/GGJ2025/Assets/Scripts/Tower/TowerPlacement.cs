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
    //[SerializeField] private GameObject _selectedTowerIndexGameObj;

    [Header("Highlighting")]
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private GameObject gridVisualiztion;

    [SerializeField] private Renderer previewRenderer;

    [Header("TowerPlacement")]

    [SerializeField] private TowerInfo towerInfo;
    //-1 means null there's isn't a tower selected 
    [SerializeField] private int selectedTowerIndex = -1;

    [SerializeField] private GridData towerData;

    private List<GameObject> placedGameObjects = new();

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
        //selectedTowerIndex = -1;
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

        bool placementValidity = CheckPlaceValidity(gridPos, selectedTowerIndex);

        if (!placementValidity)
        {
            return;
        }

        GameObject newObject = Instantiate(towerInfo.TowerList[selectedTowerIndex].towerPrefab);
        newObject.transform.position = grid.CellToWorld(gridPos);

        placedGameObjects.Add( newObject );

        GridData selectedData = towerInfo.TowerList[selectedTowerIndex].ID == 0 ? towerData : towerData;
        selectedData.AddObjectAt(gridPos, towerInfo.TowerList[selectedTowerIndex].size, towerInfo.TowerList[selectedTowerIndex].ID,placedGameObjects.Count - 1);
    }

    private bool CheckPlaceValidity(Vector3Int gridPos, int selectedTowerIndex)
    {
        GridData selectedData = towerInfo.TowerList[selectedTowerIndex].ID == 0 ? towerData: towerData;

        return selectedData.CanPlaceObjectAt(gridPos, towerInfo.TowerList[selectedTowerIndex].size);
    } 

    public void GetTowerPrefab(int ID)
    {
        int temp;

        //Debug.Log(ID);

        temp = towerInfo.TowerList.FindIndex(data => data.ID == ID);

        //Debug.Log(towerInfo.TowerList.FindIndex(data => data.ID == ID));

        if (temp >= 0)
        {
            selectedTowerIndex = temp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        HighlightTile();

        if (selectedTowerIndex != -1)
        {
            StartPlacement();
        }
    }


    
    private void HighlightTile()
    {
        Vector3Int gridPos = grid.WorldToCell(CursorControl.Instance.GetMousePos());

        bool placementValidity = CheckPlaceValidity(gridPos, selectedTowerIndex);
        previewRenderer.material.color = placementValidity ? Color.white : Color.red;

        cellIndicator.transform.position = grid.CellToWorld(gridPos);

    }

    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
}
