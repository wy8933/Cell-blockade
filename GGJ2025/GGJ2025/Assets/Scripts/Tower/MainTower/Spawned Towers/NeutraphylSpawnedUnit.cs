using UnityEngine;

public class NeutraphylSpawnedUnit : MonoBehaviour
{
    private Collider _collider;

    [SerializeField] private GameObject _targetObject;

    [SerializeField] private float movementSpeed = 2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject TargetObject
    {
        get { return _targetObject; }
        set { _targetObject = value; }
    }

    void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsTarget();
    }

    private void DamageAndDestroy(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Destroy(gameObject);
            DamageManager.Instance.ManageDamage(new DamageInfo(gameObject, other.gameObject, 1, DamageType.None));
        }
        
    }

    private void MoveTowardsTarget()
    {
        //This should be when the character starts roaming around
        if (_targetObject.gameObject == null)
        {
            //not implomented 
            RoamingWalk();
        }
        else if (_targetObject != null && !_targetObject.activeSelf)
        {
            _targetObject = null;
        }
        //THe character starts walking towards the enemy that its been assigned 
        else if (_targetObject != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetObject.transform.position, movementSpeed * Time.deltaTime);
        }
    }

    private void RoamingWalk()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            DamageAndDestroy(other);
        }
        
    }

}
