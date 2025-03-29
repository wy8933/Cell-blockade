using UnityEngine;

public class NeutraphylSpawnedUnit : MonoBehaviour
{
    private Collider _collider;

    [SerializeField] private Vector3 _targetTransform = Vector3.zero;

    [SerializeField] private float movementSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Vector3 TargetTransform
    {
        get { return _targetTransform; }
        set { _targetTransform = value; }
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
            DamageManager.Instance.ManageDamage(new DamageInfo(gameObject, other.gameObject, 1, DamageType.None));
            Destroy(this);
        }
        
    }

    private void MoveTowardsTarget()
    {
        //This should be when the character starts roaming around
        if (_targetTransform == Vector3.zero)
        {
            //not implomented 
            RoamingWalk();
        }
        //THe character starts walking towards the enemy that its been assigned 
        else if(_targetTransform != Vector3.zero)
        {
            transform.position += _targetTransform * movementSpeed * Time.deltaTime;
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
