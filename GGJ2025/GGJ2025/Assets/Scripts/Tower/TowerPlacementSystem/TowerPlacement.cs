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

    [SerializeField] private IBuildingState buildingState;

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

    [SerializeField] public int towerRotationDegrees;

    [SerializeField] public Quaternion towerRotation;

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

        Debug.Log(buildingState);
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

        if (NavMeshManager.Instance != null)
        {
            NavMeshManager.Instance.BakeNavMesh();
        }
        buildingState = null;
    }

    public void PlaceTower()
    {
        Vector3Int gridPos = grid.WorldToCell(CursorControl.Instance.GetMousePos());
        if (IsPointerOverUI())
        {
            return;
        }

        buildingState.OnAction(gridPos, towerRotation);
    }

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
