using UnityEngine;

public class NeutraphylSpawnedUnit : MonoBehaviour
{
    private Collider _collider;

    private Transform _targetTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform TargetTransform
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
        
    }

    private void DamageAndDestroy()
    {
        
    }

    private void MoveTowardsTarget()
    {
        //This should be when the character starts roaming around
        if (_targetTransform == null)
        {

        }
        //THe character starts walking towards the enemy that its been assigned 
        else{

        }
    }

    private void RoamingWalk()
    {

    }

}
