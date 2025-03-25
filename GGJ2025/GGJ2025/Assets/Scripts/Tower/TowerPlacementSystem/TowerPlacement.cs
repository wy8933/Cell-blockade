using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    [Header("External Scripts")]
    [SerializeField] private ObjectPlacer objectPlacer;

    [SerializeField] private PlacementState placementState;

    IBuildingState buildingState;

    public static TowerPlacement Instance;

    //[SerializeField] private PlayerInputManager _playerInputManager;

    [Header("TileMap Variables")]
    //[SerializeField] private List<GameObject> turretSelect;

    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap turretTilemap;
    //[SerializeField] private GameObject _selectedTowerIndexGameObj;

    [Header("Highlighting")]
    [SerializeField] private GameObject gridVisualiztion;
    [SerializeField] private PreviewSystem preview;
    
    //[SerializeField] private GameObject cellIndicator;
    //[SerializeField] private Renderer previewRenderer;

    [Header("TowerPlacement")]

    [SerializeField] private TowerInfo towerDataBase;

    [SerializeField] private GridData towerData;

    private Vector3Int lastDetectedPos = Vector3Int.zero;

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

        towerData = new GridData();

    }

    /// <summary>
    /// 
    /// </summary>
    public void StartPlacement(int ID)
    {
        gridVisualiztion.SetActive(true);
        //ID can be changed if needed remove the ID input and replace ID with selectedTowerIndex
        buildingState = new PlacementState(objectPlacer, preview, ID, grid, towerDataBase, towerData);
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualiztion.SetActive(true);
        buildingState = new RemovingState(objectPlacer, preview, grid, towerData);
    }

    public void StopPlacement()
    {
        if (buildingState == null)
        {
            return;
        }
        //selectedTowerIndex = -1;
        gridVisualiztion.SetActive(false);
        buildingState.EndState();
        lastDetectedPos = Vector3Int.zero;

        NavMeshManager.Instance.BakeNavMesh();
        buildingState = null;
    }

    public void PlaceTower()
    {
        Vector3Int gridPos = grid.WorldToCell(CursorControl.Instance.GetMousePos());
        if (IsPointerOverUI())
        {
            return;
        }

        buildingState.OnAction(gridPos);
    }

    /*
    private bool CheckPlaceValidity(Vector3Int gridPos, int selectedTowerIndex)
    {
        GridData selectedData = towerDataBase.TowerList[selectedTowerIndex].ID == 0 ? towerData : towerData;

        return selectedData.CanPlaceObjectAt(gridPos, towerDataBase.TowerList[selectedTowerIndex].Size);
    }
    */

    //This will be removed when determined to be unneeded
    /*
    public void GetTowerPrefab(int ID)
    {
        int temp;

        //Debug.Log(ID);

        temp = towerDataBase.TowerList.FindIndex(data => data.ID == ID);

        //Debug.Log(towerInfo.TowerList.FindIndex(data => data.ID == ID));

        if (temp >= 0)
        {
            selectedTowerIndex = temp;
        }

        TowerManager.Instance.EnterBuildingMode();
    }
    */

    public void HighlightTile()
    {
    
        Vector3Int gridPos = grid.WorldToCell(CursorControl.Instance.GetMousePos());

        if (lastDetectedPos != gridPos)
        {
            buildingState.UpdateState(gridPos);
            lastDetectedPos = gridPos;
        }
        
    }

    private void Update()
    {
        if (buildingState == null)
        {
            return;
        }
        HighlightTile();
    }
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

}
