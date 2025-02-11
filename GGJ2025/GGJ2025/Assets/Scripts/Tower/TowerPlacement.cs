using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private TowerManager towerManager;

    [Header("TileMap Variables")]
    [SerializeField] private List<GameObject> turretSelect;

    //-1 means null there's isn't a tower selected 
    [SerializeField] private int selectedTower = -1;

    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap turretTilemap;
    [SerializeField] private GameObject _selectedTowerGameObj;

    [Header("Highlighting")]

    [SerializeField] private Material highlightMaterial;

    [Header("Orginal Material")]

    [SerializeField] private Material orginalMaterial;
    [SerializeField] private Transform highlight;
    [SerializeField] private Transform selection;

    // Update is called once per frame
    void Update()
    {
        if (!EnemyWaveManager.Instance.isWaveActive)
        {
            PlaceTower();
            HighlightTile();
        }
    }


    
    private void HighlightTile()
    {
        
        Vector3Int gridPos = grid.WorldToCell(CursorControl.Instance.GetMousePosTile());

        if (highlight != null)
        {
            highlight.GetComponent<MeshRenderer>().material = orginalMaterial;
            highlight = null;
        }

        //If you dont have a tower selected it wont highlight a tower
        if (selectedTower != -1)
        {
            //Checks if the cursor is currently hoving over an object based on the event system and the current location of the cursor raycast
            if (!EventSystem.current.IsPointerOverGameObject() && CursorControl.Instance.GetMousePosTile() != Vector3.zero)
            {
                highlight.position = CursorControl.Instance.GetMousePosTile();
                //Checks if the tile is somewhere you can place a tower on
                if (highlight.CompareTag("PlaceAble") && highlight != selection)
                {
                    if (highlight.GetComponent<MeshRenderer>().material != highlightMaterial)
                    {
                        orginalMaterial = highlight.GetComponent<MeshRenderer>().material;
                        highlight.GetComponent <MeshRenderer>().material = highlightMaterial;
                    }
                }
                else
                {
                    highlight = null;
                }
            }
        }

        
        
    }

    private void PlaceTower()
    {
        Vector3Int gridPos = grid.WorldToCell(CursorControl.Instance.GetMousePosTile());

        if ()
        {

        }
    }
}
