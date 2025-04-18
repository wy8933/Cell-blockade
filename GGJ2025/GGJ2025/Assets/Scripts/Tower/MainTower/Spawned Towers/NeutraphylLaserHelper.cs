using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class NeutraphylLaserHelper : MonoBehaviour
{
    private Collider _collider;

    [SerializeField] private GameObject _targetObject;

    [SerializeField] private float movementSpeed = 2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private GameObject LaserHolder;
    [SerializeField] protected GameObject targetedEnemy;
    [SerializeField] private VisualEffect laserEffect;

    [Header("Walk Values")]

    [SerializeField] private bool isWalking;

    [SerializeField] private bool hasPoint;
    [SerializeField] private float roamingRadius;
    [SerializeField] private Vector3 _parentTransform;
    [SerializeField] private Vector3 targetPosition;

    [SerializeField] private float wanderRadius;
    [SerializeField] private float speed;
    [SerializeField] private float arrivalThreshold;

    [SerializeField] private float waitTime;
    [SerializeField] private float waitTimer = 0f;

    public Vector3 ParentTransform
    {
        get { return _parentTransform; }
        set { _parentTransform = value; }
    }

    void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetedEnemy == null)
        {
            RoamingWalk();
        }
    }


    private void RoamingWalk()
    {
        if (!hasPoint)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                PickNewPoint();
            }
            return;
        }

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < arrivalThreshold)
        {
            hasPoint = false;
            waitTimer = waitTime;
        }
    }


    private void PickNewPoint()
    {
        Vector2 randomOffset = Random.insideUnitCircle * roamingRadius;
        targetPosition = _parentTransform + new Vector3(randomOffset.x, 0, randomOffset.y);
        hasPoint = true;
    }

    private void OnTriggerStay(Collider other)
    {
        Attack(other);

        ShowAttack(gameObject, other.gameObject);
    }

    /// <summary>
    /// An override of the Attack method that takes a collision as an input
    /// checks if a target is within the detection radius
    /// </summary>
    /// <param name="collision"></param>
    protected void Attack(Collider collision)
    {
        //Debug.Log(collision.tag);

        if (collision.tag == "Enemy")
        {
            if (targetedEnemy == null)
            {
                targetedEnemy = collision.gameObject;
            }
            else if (targetedEnemy != collision.gameObject)
            {
                targetedEnemy = collision.gameObject;
            }

            if (targetedEnemy == collision.gameObject)
            {
                DamageManager.Instance.ManageDamage(new DamageInfo(gameObject, collision.gameObject, 1, DamageType.None));
            }

            //Debug.Log("ITs in the area");
        }

        if (targetedEnemy != null && !targetedEnemy.activeSelf)
        {
            targetedEnemy = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    protected void ShowAttack(GameObject source, GameObject target)
    {
        //Debug.Log(target);
        if (target.tag == "Enemy")
        {
            if (target != null)
            {
                Debug.Log(target.transform.position);
                if (targetedEnemy == target)
                {
                    LaserHolder.SetActive(true);
                    LaserHolder.transform.LookAt(new Vector3(target.transform.position.x, (target.transform.position.y), target.transform.position.z));
                }
                //Debug.Log(target.transform.position);
            }
        }
        else if (targetedEnemy == null)
        {
            LaserHolder.SetActive(false);
        }
    }
}
