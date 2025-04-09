using ObjectPoolings;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnEntityBuffModule", menuName = "BuffSystem/SpawnEntityBuffModule")]
public class SpawnEntityBuffModule : BaseBuffModule
{
    public GameObject gameObject;
    public int count;

    public override void Apply(BuffInfo buffInfo, DamageInfo damageInfo = null)
    {
        for (int i =0; i< count; i++)
        {
            var (objectInstance, enemyPool) = ObjectPooling.GetOrCreate(gameObject, gameObject.transform.position, gameObject.transform.rotation, "Enemies");
            objectInstance.GetComponent<BaseEnemy>().InitEnemy(enemyPool);
        }
    }
}

