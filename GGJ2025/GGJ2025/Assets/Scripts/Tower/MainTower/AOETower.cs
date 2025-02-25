using UnityEngine;

public class AOETower : BaseTower
{
    [SerializeField] protected GameObject LaserHolder;

    [SerializeField] protected GameObject targetedEnemy;

    protected override void Attack(Collider collision)
    {
        
    }

    protected override void ShowAttack(GameObject source, GameObject target)
    {

    }
}
