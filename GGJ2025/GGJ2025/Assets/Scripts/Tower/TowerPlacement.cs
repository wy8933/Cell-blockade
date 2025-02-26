<<<<<<< Updated upstream
<<<<<<< Updated upstream
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
=======
using System.Collections.Generic;
using UnityEngine;
>>>>>>> Stashed changes
=======
using System.Collections.Generic;
using UnityEngine;
>>>>>>> Stashed changes
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
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

    [SerializeField] private TowerInfo towerDataBase;
    //-1 means null there's isn't a tower selected 
    [SerializeField] private int selectedTowerIndex = -1;

    [SerializeField] private GridData towerData;

    [SerializeField] private List<GameObject> placedGameObjects = new List<GameObject>();

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

        GameObject newObject = Instantiate(towerDataBase.TowerList[selectedTowerIndex].TowerPrefab);
        newObject.transform.position = grid.CellToWorld(gridPos);

        placedGameObjects.Add(newObject);

        GridData selectedData = towerDataBase.TowerList[selectedTowerIndex].ID == 0 ? towerData : towerData;
        selectedData.AddObjectAt(gridPos, towerDataBase.TowerList[selectedTowerIndex].Size, towerDataBase.TowerList[selectedTowerIndex].ID,placedGameObjects.Count - 1);
    }

    private bool CheckPlaceValidity(Vector3Int gridPos, int selectedTowerIndex)
    {
        GridData selectedData = towerDataBase.TowerList[selectedTowerIndex].ID == 0 ? towerData: towerData;

        return selectedData.CanPlaceObjectAt(gridPos, towerDataBase.TowerList[selectedTowerIndex].Size);
    } 

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
    }
=======
=======
>>>>>>> Stashed changes
    [SerializeField] private TowerManager towerManager;

    [Header("Camera Variables")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;

    [Header("TileMap Variables")]
    [SerializeField] private List<GameObject> turretSelect;

    [SerializeField] private Tilemap turretTilemap;
    [SerializeField] private GameObject selectedTower;
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        
        

        if (selectedTowerIndex != -1)
        {
            StartPlacement();
            HighlightTile();
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
=======
=======
>>>>>>> Stashed changes
        PlaceTower();
    }

    private Vector3 GetMousePosTile()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            //sets mouse pos
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }

        
    }

    private void PlaceTower()
    {

    }
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
}
