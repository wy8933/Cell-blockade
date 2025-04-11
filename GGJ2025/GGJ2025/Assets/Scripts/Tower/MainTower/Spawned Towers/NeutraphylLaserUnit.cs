using UnityEngine;
using UnityEngine.VFX;

public class NeutraphylLaserUnit : MonoBehaviour
{

    private Collider _collider;

    [SerializeField] private float movementSpeed = 2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private GameObject LaserHolder;

    [SerializeField] protected GameObject targetedEnemy;

    [SerializeField] private VisualEffect laserEffect;

    [Header("Random Walk")]
    [SerializeField] private bool walking;

    [SerializeField] private Vector3 spawnTowerLocation;
    [SerializeField] private float radius;

    void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public Vector3 SpawnTowerLocation
    {
        get { return spawnTowerLocation; }
        set { spawnTowerLocation = value; } 
    }

    private void Update()
    {
        if (walking)
        {
            RoamingWalk();
        }
    }

    private void RoamingWalk()
    {

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
                laserEffect.gameObject.SetActive(true);
                DamageManager.Instance.ManageDamage(new DamageInfo(gameObject, collision.gameObject, 1, DamageType.None));
                walking = false;
            }

            //Debug.Log("ITs in the area");
        }

        if (targetedEnemy != null && !targetedEnemy.activeSelf)
        {
            laserEffect.gameObject.SetActive(false);
            targetedEnemy = null;
            walking = true;
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
