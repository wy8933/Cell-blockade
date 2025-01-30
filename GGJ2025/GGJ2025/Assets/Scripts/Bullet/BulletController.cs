using Unity.VisualScripting;
using UnityEngine;
using ObjectPoolings;
[RequireComponent(typeof(Rigidbody))]
public class BulletController : MonoBehaviour
{
    public Animator animator;
    public int minSpeed;
    public int maxSpeed; 
    public float baseDamage = 10f;
    private float _damage;
    public float lifeTime;
    public float lifeTimer;
    public DamageType damageType;
    public AudioSource audioSource;

    [Tooltip("Use to make sure the varibale doesn't get released multiple time")]
    private bool isReleased = false;
    private PrefabPool pool;

    private void Update()
    {
        lifeTimer -= Time.deltaTime;

        // Make pop sound before the bubble pop to make sure the sound will correctly finished
        if (lifeTimer < 0.3 && !isReleased) {
            PopSound();
            isReleased = true;
        }

        // Play the pop animatino and get it ready for release
        if (lifeTimer < 0)
        {
            animator.SetBool("IsExplode", true);
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero; 
            isReleased = true;
        }
        CheckBubblePop();
    }

    /// <summary>
    /// Init the bullet when it's created or got from the object pool, to make sure the bullet will function correctly
    /// </summary>
    /// <param name="pool">The object pool bullet is in, for release bullet in the future</param>
    /// <param name="damageMult">The multiply of damage value from bullet source</param>
    public void InitBullet(PrefabPool pool, float damageMult) {
        _damage = baseDamage * damageMult;
        lifeTimer = lifeTime;
        isReleased = false;
        this.pool = pool;
        GetComponent<Rigidbody>().linearVelocity = transform.forward * new System.Random().Next(minSpeed, maxSpeed);
    }

    /// <summary>
    /// Check collider's tag, and trigger different behavior 
    /// </summary>
    /// <param name="other">The object collider with bullet</param>
    private void OnTriggerEnter(Collider other)
    {
        // Damage enemy and release the bullet when the collider is enemy
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out BaseEnemy enemy))
            {
                DamageInfo damageInfo = new DamageInfo(gameObject, other.gameObject, _damage, damageType);
                DamageManager.Instance.ManageDamage(damageInfo);
            }
            if (!isReleased)
            {
                animator.SetBool("IsExplode", true);
                PopSound();
                GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            }
        } // Realse bullet when collider is wall
        else if (other.CompareTag("Wall")) 
        {
            if (!isReleased)
            {
                animator.SetBool("IsExplode", true);
                PopSound();
                GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// Check if the current animation is bubble explosion and relased the bullet when the animation is finished
    /// </summary>
    public void CheckBubblePop() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("BubbleExplosion")&&animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length) {
            pool.Release(gameObject);
            isReleased = true;
        }
    }

    // Play the bubble explode sound
    public void PopSound() 
    {
        audioSource.Play();
        audioSource.volume = SoundManager.Instance.SFXMult;
    }
}
