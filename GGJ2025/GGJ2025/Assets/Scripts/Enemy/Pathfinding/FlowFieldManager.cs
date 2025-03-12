using UnityEngine;
public enum FlowFieldTarget { Player, BlockedPlayer, Core, BlockedCore }

public class FlowFieldManager : MonoBehaviour
{
    public static FlowFieldManager Instance;

    [Header("Grid Settings")]
    public Vector3 gridOrigin = Vector3.zero;
    public int gridSizeX = 50;
    public int gridSizeY = 50;
    public float cellSize = 1f;

    [Header("Obstacle Settings")]
    public LayerMask obstacleMask;
    public int obstacleCost = 100;

    // Normal flow fields that consider obstacles
    public FlowField playerFlowField;
    public FlowField coreFlowField;

    // Blocked flow fields that ignore obstacles
    public FlowField blockedPlayerFlowField;
    public FlowField blockedCoreFlowField;

    [Header("Gizmo Settings")]
    public bool showPlayerGizmos = true;
    public bool showBlockedPlayerGizmos = true;
    public bool showCoreGizmos = true;
    public bool showBlockedCoreGizmos = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Initialize all four flow fields with the same grid settings
        playerFlowField = new FlowField(gridSizeX, gridSizeY, cellSize, gridOrigin);
        coreFlowField = new FlowField(gridSizeX, gridSizeY, cellSize, gridOrigin);
        blockedPlayerFlowField = new FlowField(gridSizeX, gridSizeY, cellSize, gridOrigin);
        blockedCoreFlowField = new FlowField(gridSizeX, gridSizeY, cellSize, gridOrigin);
    }

    private void Update()
    {
        // Mark obstacles for the normal flow fields
        MarkObstaclesForField(playerFlowField, false);
        MarkObstaclesForField(coreFlowField, false);
        // For the blocked flow fields, ignore obstacles
        MarkObstaclesForField(blockedPlayerFlowField, true);
        MarkObstaclesForField(blockedCoreFlowField, true);

        // Update each flow field with its target position
        UpdateFlowField(GameManager.Instance.Player.transform.position, playerFlowField);
        UpdateFlowField(GameManager.Instance.Player.transform.position, blockedPlayerFlowField);

        // For the core fields, we use the core target's position
        UpdateFlowField(Wound.Instance.transform.position, coreFlowField);
        UpdateFlowField(Wound.Instance.transform.position, blockedCoreFlowField);
    }

    /// <summary>
    /// Marks obstacles on a given flow field, if ignoreObstacles is true, all nodes are set as walkable
    /// </summary>
    private void MarkObstaclesForField(FlowField field, bool ignoreObstacles)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                FlowFieldNode node = field.grid[x, y];
                if (ignoreObstacles)
                {
                    node.baseCost = 1;
                }
                else
                {
                    Collider[] colliders = Physics.OverlapSphere(node.worldPosition, cellSize * 0.5f, obstacleMask);
                    node.baseCost = colliders.Length > 0 ? obstacleCost : 1;
                }
            }
        }
    }

    /// <summary>
    /// Updates a specific flow field using the provided target position
    /// </summary>
    public void UpdateFlowField(Vector3 targetPosition, FlowField field)
    {
        Vector2Int targetIndex = field.WorldPositionToIndex(targetPosition);
        targetIndex.x = Mathf.Clamp(targetIndex.x, 0, gridSizeX - 1);
        targetIndex.y = Mathf.Clamp(targetIndex.y, 0, gridSizeY - 1);

        field.ComputeIntegrationField(targetIndex);
        field.ComputeFlowField();
    }

    /// <summary>
    /// Returns the best flow direction for a given world position and target type
    /// </summary>
    public Vector3 GetFlowDirection(FlowFieldTarget targetType, Vector3 worldPosition)
    {
        switch (targetType)
        {
            case FlowFieldTarget.Player:
                return playerFlowField.GetDirection(worldPosition);
            case FlowFieldTarget.BlockedPlayer:
                return blockedPlayerFlowField.GetDirection(worldPosition);
            case FlowFieldTarget.Core:
                return coreFlowField.GetDirection(worldPosition);
            case FlowFieldTarget.BlockedCore:
                return blockedCoreFlowField.GetDirection(worldPosition);
            default:
                return Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        if (playerFlowField != null && showPlayerGizmos)
            DrawFlowFieldGizmos(playerFlowField, Color.white);
        if (blockedPlayerFlowField != null && showBlockedPlayerGizmos)
            DrawFlowFieldGizmos(blockedPlayerFlowField, Color.blue);
        if (coreFlowField != null && showCoreGizmos)
            DrawFlowFieldGizmos(coreFlowField, Color.green);
        if (blockedCoreFlowField != null && showBlockedCoreGizmos)
            DrawFlowFieldGizmos(blockedCoreFlowField, Color.cyan);
    }

    /// <summary>
    /// Draws the grid and best directions for a given flow field
    /// </summary>
    private void DrawFlowFieldGizmos(FlowField field, Color gizmoColor)
    {
        float sphereRadius = cellSize * 0.2f;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                FlowFieldNode node = field.grid[x, y];

                Gizmos.color = (node.baseCost >= obstacleCost) ? Color.red : gizmoColor;
                Gizmos.DrawWireCube(node.worldPosition, new Vector3(cellSize, 0.1f, cellSize));
                Gizmos.DrawSphere(node.worldPosition, sphereRadius);

                if (node.bestDirection != Vector3.zero)
                {
                    Gizmos.color = Color.yellow; 
                    Vector3 start = node.worldPosition;
                    Vector3 end = start + node.bestDirection * cellSize * 0.5f;
                    Gizmos.DrawLine(start, end);
                }
            }
        }
    }
}