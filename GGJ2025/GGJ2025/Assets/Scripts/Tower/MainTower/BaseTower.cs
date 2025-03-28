using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseTower : BasicTowerInfo
{
    /// <summary>
    /// An abstract method that is used by the children of this class to in someway 
    /// do damage to enemies directly
    /// </summary>
    /// <param name="other"></param>
    protected abstract void Attack(Collider other);

    /// <summary>
    /// An abstract method that is used by children classes, as a method that is mainly
    /// used for running visual representation of the attack
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    protected abstract void ShowAttack(GameObject source, GameObject target);

    /// <summary>
    /// Both abstract methods Attack and ShowAttack
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (isTowerActive)
        {
            Attack(other);

            ShowAttack(gameObject, other.gameObject);
        }
        

        //Debug.Log(other.gameObject.transform.position);
    }

}
