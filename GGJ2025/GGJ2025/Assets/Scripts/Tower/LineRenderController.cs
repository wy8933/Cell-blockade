using UnityEngine;

public class LineRenderController : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    //[SerializeField] private MeshCollider meshCollider;

    [Header("Texturing and Animation Change")]
    [SerializeField] private Texture[] textures;
    [SerializeField] private int animationStep;
    [SerializeField] private float fps = 60f;
    [SerializeField] private float fpsCounter = 0;

    [Header("Mouse Click Value")]
    [SerializeField] private Vector2 mouseClickVal;

    [Header("Point Vales")]
    //[SerializeField] private Transform anchorLoc;
    //[SerializeField] private Transform headLoc;
    [SerializeField] private Transform[] points;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public Vector2 MouseClickVal
    {
        set
        {
            mouseClickVal = value;
        }
    }

    private void Update()
    {
        for (int x = 0; x < points.Length; x++)
        {
            lineRenderer.SetPosition(x, points[x].position);
        }
    }

    /*
    private void AssignTarget(Transform startPos, Transform newTarget)
    {
        lineRenderer.positionCount = 0;
        lineRenderer.SetPosition(0, startPos.position);

        headLoc = newTarget;
    }
    */

    public void SetUpLine(Transform[] points)
    {
        lineRenderer.positionCount = points.Length;
        this.points = points;
    }

    public void UpdateLineTexture()
    {
        fpsCounter += Time.deltaTime;
        if (fpsCounter >= 1f / fps)
        {
            animationStep++;

            if (animationStep == textures.Length)
            {
                animationStep = 0;
            }

            lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);

            fpsCounter = 0;
        }
    }
}
