using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;
    public PlayerController playerController;

    private int selectedTowerIndex;
    //[SerializeField] private List<GameObject> towers;

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
    }

    public void ExitBuildMode()
    {
        TowerPlacement.Instance.StopPlacement();
        playerController.isBuildingMode = false;
    }

    public void EnterBuildingMode() 
    {
        TowerPlacement.Instance.StartPlacement(selectedTowerIndex);
        //TowerPlacement.Instance.HighlightTile();
        playerController.isBuildingMode = true;
    }

    private void OnButtonClick(int towerID)
    {
        selectedTowerIndex = towerID;
    }
}
