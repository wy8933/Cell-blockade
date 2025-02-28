using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;
    public PlayerController playerController;
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

    /*
    public List<GameObject> Towers
    {
        get { return towers; }
        set { towers = value; }
    }
    */
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void ExitBuildMode()
    {
        TowerPlacement.Instance.StopPlacement();
        playerController.isBuildingMode = false;
    }

    public void EnterBuildingMode() 
    {

        TowerPlacement.Instance.StartPlacement();
        TowerPlacement.Instance.HighlightTile();
        playerController.isBuildingMode = true;
    }
}
