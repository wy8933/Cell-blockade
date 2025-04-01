using UnityEngine;

[CreateAssetMenu(fileName = "AddBuffModule", menuName = "BuffSystem/AddBuffModule")]
public class AddBuffModule : BaseBuffModule
{
    public BuffInfo Buff;

    public override void Apply(BuffInfo buffInfo, DamageInfo damageInfo = null)
    {
        Buff.creator = buffInfo.creator;
        Buff.target = buffInfo.target;

        buffInfo.target.gameObject.GetComponent<BuffHandler>().AddBuff(Buff);
    }
}
