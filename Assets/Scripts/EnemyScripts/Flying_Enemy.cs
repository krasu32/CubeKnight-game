using Unity.VisualScripting;
using UnityEngine;

public class Flying_Enemy : Enemy_Ai
{
    [SerializeField] private float chaseDistance;

    [SerializeField] float stunDuration;
    float timer;
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.FlyingEnemy_Idle);
    }

        protected override void Update()
    {
        base.Update();
        if(!PlayerControl.Instance.pState.alive)
        {
            ChangeState(EnemyStates.FlyingEnemy_Idle);
        }
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerControl.Instance.transform.position);

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.FlyingEnemy_Idle:
            rb.linearVelocity = new Vector2(0, 0);
                if(_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.FlyingEnemy_Chase);
                }
                break;
            case EnemyStates.FlyingEnemy_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerControl.Instance.transform.position, Time.deltaTime * speed));

                FlipFlyingEnemy();

                if (_dist >chaseDistance)
                {
                    ChangeState(EnemyStates.FlyingEnemy_Idle);
                }
                break;
            case EnemyStates.FlyingEnemy_Stunned:
                timer += Time.deltaTime;

                if(timer > stunDuration)
                {
                    ChangeState(EnemyStates.FlyingEnemy_Idle);
                    timer = 0;
                }
                break;
            case EnemyStates.FlyingEnemy_Death:
                Death(Random.Range(5, 10));
                break;
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);

        if (health > 0)
        {
            ChangeState(EnemyStates.FlyingEnemy_Stunned);
        }
        else
        {
            ChangeState(EnemyStates.FlyingEnemy_Death);
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.FlyingEnemy_Idle);

        anim.SetBool("Chase", GetCurrentEnemyState == EnemyStates.FlyingEnemy_Chase);

        anim.SetBool("Stunned", GetCurrentEnemyState == EnemyStates.FlyingEnemy_Stunned);

        if(GetCurrentEnemyState == EnemyStates.FlyingEnemy_Death)
        {
            anim.SetTrigger("Death");
            int LayerIgnorePlayer = LayerMask.NameToLayer("Ignore Player");
            gameObject.layer = LayerIgnorePlayer;
        }
    }
    void FlipFlyingEnemy()
    {
        sr.flipX = PlayerControl.Instance.transform.position.x < transform.position.x;
    }
}
