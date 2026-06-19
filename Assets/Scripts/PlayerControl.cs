using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed = 1;

    [Header("Vertical Movement Settings:")]
    [SerializeField] private float jumpForce = 45f;
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;

    private float coyoteTimeCounter = 0;

    [SerializeField] private float coyoteTime;

    private int airJumpCounter = 0;
    [SerializeField] public int maxAirJumps;

    [Header("Ground Check Settings:")]
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float groundCheckY = 0.2f;

    [SerializeField] private float groundCheckX = 0.5f;

    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    [Header("Attacking")]
    bool attack = false;
    
    [SerializeField] Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
    [SerializeField] Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] private float timeBetweenAttack;
    private float timeSinceAttack;
    [SerializeField] float damage;
    [SerializeField] GameObject slashEffect;

    [Header("Recoil")]
    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepsXRecoiled, stepsYRecoiled;

    [Header("Health Settings:")]
    public int health;
    public int maxHealth;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;
    
    float healTimer;
    [SerializeField] float timeToHeal;
    [Space(5)]

    [Header("Mana Settings")]
    [SerializeField] UnityEngine.UI.Image manaStorage;
    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    public bool halfMana;
    [Space(5)]

    [Header("Camera Stuff")]
    [SerializeField] private float playerFallSpeedThreshold = -10;

    [HideInInspector] public PlayerState pState;
    public Rigidbody2D rb;
    private float xAxis, yAxis;
    private float gravity;
    private Animator anim;
    private bool canDash = true;
    private bool dashed;
    private SpriteRenderer sr;

    bool restoreTime;
    float restoreTimeSpeed;

    

    public static PlayerControl Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

    }


    void Start()
    {
        pState = GetComponent<PlayerState>();
    
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        anim = GetComponent<Animator>();

        SaveData.Instance.LoadPlayerData();
        if (halfMana)
        {
            UiManager.Instance.SwitchMana(UiManager.ManaState.HalfMana);
        }

        gravity = rb.gravityScale;

        Mana = mana;
        manaStorage.fillAmount = Mana;
        Health = maxHealth;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    void Update()
    {
        if(GameManager.Instance.gameIsPaused) return;

        if (pState.cutscene) return;
        if(pState.alive)
        {
            GetInputs();   
        }

        UpdateJumpVariables();
        UpdateCameraYDampForPlayerFall();
        RestoreTimeScale();

        if (pState.dashing) return;
        if(pState.alive)
        {
            Flip();
            Move();
            Jump();
            StartDash();
            Attack();
            Heal();
        }
        
        Recoil();
        FlashWhileInvincible();
    }

    private void FixedUpdate()
    {
        if(pState.cutscene) return;

        if(pState.dashing) return;
        Recoil();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        if(pState.recoilingX) return;
        rb.linearVelocity = new Vector2(walkSpeed * xAxis, rb.linearVelocityY);
        anim.SetBool("Walking", rb.linearVelocityX != 0 && Grounded());
    }

    void UpdateCameraYDampForPlayerFall()
    {
        if (rb.linearVelocityY < playerFallSpeedThreshold && !CameraManager.Instance.isLerpingYDamp && !CameraManager.Instance.hasLerpingYDamp)
        {
            StartCoroutine(CameraManager.Instance.LerpYDamping(true));
        }

        if (rb.linearVelocityY >= 0 && !CameraManager.Instance.isLerpingYDamp && CameraManager.Instance.hasLerpingYDamp)
        {
            CameraManager.Instance.hasLerpingYDamp = false;
            StartCoroutine(CameraManager.Instance.LerpYDamping(false));
        }
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        int _dir = pState.lookingRight ? 1 : -1;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(_dir * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        if(_exitDir.y > 0)
        {
            rb.linearVelocity = jumpForce * _exitDir;
        }

        if(_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;

            Move();
        }
        Flip();
        yield return new WaitForSeconds(_delay);
        pState.cutscene = false;
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;

                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilXSpeed);
                Instantiate(slashEffect, SideAttackTransform);
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 90, UpAttackTransform);
            }
            else if (yAxis < 0 && !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        List<Enemy_Ai> hitEnemies = new List<Enemy_Ai>();

        if (objectToHit.Length > 0)
        {
            _recoilBool = true;
        }
        for (int i = 0; i < objectToHit.Length; i++)
        {
            Enemy_Ai e = objectToHit[i].GetComponent<Enemy_Ai>();
            if (e && !hitEnemies.Contains(e))
            {
                e.EnemyHit(damage, _recoilDir, _recoilStrength);
                hitEnemies.Add(e);

                if (objectToHit[i].CompareTag("Enemy"))
                {
                    Mana += manaGain;
                }
            }
        }
    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.linearVelocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.linearVelocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, recoilYSpeed);
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }

        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }

    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    public void TakeDamage(float _damage)
    {
        if(pState.alive)
        {
            Health -= Mathf.RoundToInt(_damage);
            if(Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopTakingDamage());
            }
        }
    }

    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        anim.SetTrigger("TakeDamage");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }
    void FlashWhileInvincible()
    {
        sr.material.color = pState.invincible ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
    }
    void RestoreTimeScale()
    {
        if(restoreTime)
        {
            if(Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;

        if(_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
    }
    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }

    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(0.9f);
        StartCoroutine(UiManager.Instance.ActivateDeathScreen());
    }

    public void Respawned()
    {
        if(!pState.alive)
        {
            pState.alive = true;
            halfMana = true;
            UiManager.Instance.SwitchMana(UiManager.ManaState.HalfMana);
            Mana = 0;
            Health = maxHealth;
            anim.Play("Player_Idle");
        }
    }

    public void RestoreMana()
    {
        halfMana = false;
        UiManager.Instance.SwitchMana(UiManager.ManaState.FullMana);
    }

    public int Health
    {
        get { return health; }
        set
        {
            if(health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if(onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }

    void Heal()
    {
        if(Input.GetButton("Healing") && Health < maxHealth && Mana > 0 && !pState.jumping && !pState.dashing)
        {
            pState.healing = true;
            anim.SetBool("Healing", true);

            healTimer += Time.deltaTime;
            if(healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }

            Mana -= Time.deltaTime * manaDrainSpeed;
        }
        else
        {
            pState.healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;
        }
    }

    public float Mana
    {
        get { return mana; }
        set
        {
            if(mana != value)
            {
                if(!halfMana)
                {
                    mana = Mathf.Clamp(value, 0, 1);
                }
                else
                {
                    mana = Mathf.Clamp(value, 0, 0.5f);
                }
                manaStorage.fillAmount = Mana;
            }
        }
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheck.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheck.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocityX, jumpForce);

                pState.jumping = true;
            }

            if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                pState.jumping = true;

                airJumpCounter++;

                rb.linearVelocity = new Vector3(rb.linearVelocityX, jumpForce);
            }

        if (Input.GetButtonUp("Jump") && rb.linearVelocityY > 3)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);

            pState.jumping = false;
        }

        anim.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }
}
