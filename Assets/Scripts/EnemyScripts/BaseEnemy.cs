using UnityEngine;

public class BaseEnemy : Enemy_Ai
{
    [SerializeField] private float flipWaitTime;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private LayerMask whatIsGround;

    float timer;
    
    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
    }

    protected override void Update()
    {
        base.Update();
        if(!PlayerControl.Instance.pState.alive)
        {
            ChangeState(EnemyStates.BaseEnemy_Idle);
        }
    }

    private void OnCollisionEnter2D(Collision2D _collision)
    {
        if(_collision.gameObject.CompareTag("Enemy"))
        {
            ChangeState(EnemyStates.BaseEnemy_Flip);
        }
    }

    protected override void UpdateEnemyStates()
    {
        if(health <= 0)
        {
            Death(0.05f);
        }

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.BaseEnemy_Idle:
                Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
                Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

                if(!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround) || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    ChangeState(EnemyStates.BaseEnemy_Flip);
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
            case EnemyStates.BaseEnemy_Flip:
                timer += Time.deltaTime;

                if(timer > flipWaitTime)
                {
                    timer = 0;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    ChangeState(EnemyStates.BaseEnemy_Idle);
                }
                break;
        }
    }
}
