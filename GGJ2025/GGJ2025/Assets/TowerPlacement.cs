using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private TowerManager towerManager;

    [Header("Camera Variables")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;

    [Header("TileMap Variables")]
    [SerializeField] private List<GameObject> turretSelect;

    [SerializeField] private Tilemap turretTilemap;
    [SerializeField] private GameObject selectedTower;

    // Update is called once per frame
    void Update()
    {
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
}
