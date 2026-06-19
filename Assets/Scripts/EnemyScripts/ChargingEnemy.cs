using UnityEngine;

public class ChargingEnemy : Enemy_Ai
{
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private float chargeSpeedMultiplier;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask whatIsGround;

    float timer;
    
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.ChargingEnemy_Idle);
        rb.gravityScale = 12f;
    }

        protected override void Update()
    {
        base.Update();
        if(!PlayerControl.Instance.pState.alive)
        {
            ChangeState(EnemyStates.ChargingEnemy_Idle);
        }
    }

        private void OnCollisionEnter2D(Collision2D _collision)
    {
        if(_collision.gameObject.CompareTag("Enemy"))
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
    }
    protected override void UpdateEnemyStates()
    {
        if(health <= 0)
        {
            Death(0.05f);
        }
        Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.ChargingEnemy_Idle:
                

                if(!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround) || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }

                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStart, _wallCheckDir, ledgeCheckX * 10);
                if(_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                    ChangeState(EnemyStates.ChargingEnemy_Surprised);
                }

                if(transform.localScale.x > 0)
                {
                    rb.linearVelocity = new Vector2(speed, rb.linearVelocityY);
                }
                else
                {
                    rb.linearVelocity = new Vector2(-speed, rb.linearVelocityY);
                }
                break;

            case EnemyStates.ChargingEnemy_Surprised:
                rb.linearVelocity = new Vector2(0, jumpForce);

                ChangeState(EnemyStates.ChargingEnemy_Charge);
                break;

            case EnemyStates.ChargingEnemy_Charge:
                timer += Time.deltaTime;

                if(timer < chargeDuration)
                {
                    if(Physics2D.Raycast(transform.position, Vector2.down, ledgeCheckY, whatIsGround))
                    {
                        if(transform.localScale.x > 0)
                        {
                            rb.linearVelocity = new Vector2(speed * chargeSpeedMultiplier, rb.linearVelocityY);
                        }
                        else
                        {
                        rb.linearVelocity = new Vector2(-speed * chargeSpeedMultiplier, rb.linearVelocityY);
                        }
                    }
                    else
                    {
                        rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
                    }
                }
                else
                {
                    timer = 0;
                    ChangeState(EnemyStates.ChargingEnemy_Idle);
                }
                break;
        }
    }

    protected override void ChangeCurrentAnimation()
    {
        if(GetCurrentEnemyState == EnemyStates.ChargingEnemy_Idle)
        {
            anim.speed = 1;
        }

        if(GetCurrentEnemyState == EnemyStates.ChargingEnemy_Charge)
        {
            anim.speed = chargeSpeedMultiplier;
        }
    }
}