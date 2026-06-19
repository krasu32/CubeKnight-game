using UnityEngine;

public class Enemy_Ai : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLenght;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected float speed;

    [SerializeField] protected float damage;

    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;


    protected enum EnemyStates
    {
        BaseEnemy_Idle,
        BaseEnemy_Flip,

        FlyingEnemy_Idle,
        FlyingEnemy_Chase,
        FlyingEnemy_Stunned,
        FlyingEnemy_Death,

        ChargingEnemy_Idle,
        ChargingEnemy_Surprised,
        ChargingEnemy_Charge,

        Boss_Stage1
    }
    protected EnemyStates currentEnemyState;

    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return currentEnemyState; }
        set
        {
            if(currentEnemyState != value)
            {
                currentEnemyState = value;

                ChangeCurrentAnimation();
            }
        }
    }
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }


    protected virtual void Update()
    {
        if(GameManager.Instance.gameIsPaused) return;

        if (isRecoiling)
        {
            if (recoilTimer < recoilLenght)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        else
        {
            UpdateEnemyStates();
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            rb.AddForce(_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !PlayerControl.Instance.pState.invincible && health > 0)
        {
            Attack();
            if(PlayerControl.Instance.pState.alive)
            {
                PlayerControl.Instance.HitStopTime(0, 5, 0.5f);                
            }
        }
    }

    protected virtual void Death(float _destroyTime)
    {
        PlayerControl.Instance.RestoreMana();
        Destroy(gameObject, _destroyTime);
    }
    protected virtual void UpdateEnemyStates()
    {
        
    }
    protected virtual void ChangeCurrentAnimation()
    {
        
    }

    protected void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyState = _newState;
    }

    protected virtual void Attack()
    {
        PlayerControl.Instance.TakeDamage(damage);
    }
}
