using UnityEngine;

[CreateAssetMenu(fileName = "WeaponBuffModule", menuName = "BuffSystem/WeaponBuffModule")]
public class WeaponBuffModule : BaseBuffModule
{

    public override void Apply(BuffInfo buffInfo, DamageInfo damageInfo = null)
    {
        GameManager.Instance.Player.weaponType = WeaponType.Sniper;
    }
}
