using UnityEngine;

public class CursorControl : MonoBehaviour
{
    public static CursorControl Instance;

    [Header("Camera Variables")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Vector3 raycastHittingLoc;
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

    public Vector3 GetMousePos()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            //sets mouse pos
            raycastHittingLoc = raycastHit.point;
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }


    }

}
