using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    private NewPlayerActions playerActions;
    private Rigidbody2D rigidbody;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float highJumpForce;
    [SerializeField] private Vector2 wallJumpVelocity;
    [SerializeField] private LayerMask ground;
    [SerializeField] private ParticleSystem jumpSmoke;
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private Transform Spawn;
    [SerializeField] private float dashSpeed;

    private Vector2 direction;
    private Collider2D playerCollider;
    private Vector3 velocity;
    private bool isJumping = false;
    private InputAction movement;
    private bool isWallJumping = false;
    

    #region AbilityResources

    private bool highJumpAvailable = true;
    private int dashesAvailable = 2;
    private bool wallJumpRight = true;
    private bool wallJumpLeft = true;

    #endregion

    private void Awake()
    {
        transform.position = Spawn.position;
        playerActions = new NewPlayerActions();
        rigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();    
    }

    private void OnEnable()
    {
        movement = playerActions.Player.Move;
        movement.Enable();

        playerActions.Player.Jump.performed += DoJump;
        playerActions.Player.Jump.Enable();

        playerActions.Player.WallJump.performed += WallJump;
        playerActions.Player.WallJump.Enable();

        playerActions.Player.HighJump.performed += HighJump;
        playerActions.Player.HighJump.Enable();

        playerActions.Player.ResetToSpawn.performed += ResetToSpawn;
        playerActions.Player.ResetToSpawn.Enable();

        playerActions.Player.Dash.performed += Dash;
        playerActions.Player.Dash.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
        playerActions.Player.Jump.Disable();
        playerActions.Player.WallJump.Disable();
        playerActions.Player.HighJump.Disable();
        playerActions.Player.ResetToSpawn.Disable();
        playerActions.Player.Dash.Disable();
    }

    #region Controls

    public bool Grounded()
    {
        return Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size - new Vector3(.35f, 0f), 0f, Vector2.down, .1f, ground);
    }

    public bool WalledR()
    {
        return Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size - new Vector3(.2f, .45f), 0f, Vector2.right, .2f, ground);
    }

    public bool WalledL()
    {
        return Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size - new Vector3(.2f, .45f), 0f, -Vector2.right, .2f, ground);
    }

    private void DoJump(InputAction.CallbackContext ctx)
    {
        if (Grounded())
        {
            Instantiate(jumpSmoke, new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.center.y - .5f, playerCollider.bounds.center.z), new Quaternion());
            Vector2 jumpvel = new Vector2(rigidbody.velocity.x, jumpForce);
            rigidbody.velocity = jumpvel + direction;
        }
    }

    private void WallHang()
    {
        if (WalledL() && movement.ReadValue<Vector2>().x < 0f)
        {
            rigidbody.velocity = new Vector2(0f, -3f);
        }
        if (WalledR() && movement.ReadValue<Vector2>().x > 0f)
        {
            rigidbody.velocity = new Vector2(0f, -3f);
        }
        
    }

    private void WallJump(InputAction.CallbackContext ctx)
    {
        if (WalledL() && !Grounded() && wallJumpLeft)
        {
            wallJumpLeft = false;
            isWallJumping = true;
            wallJumpDelay = true;
            Instantiate(explosion, new Vector3(playerCollider.bounds.center.x - .2f, playerCollider.bounds.center.y, playerCollider.bounds.center.z), new Quaternion());
            StartCoroutine("WallJumpTimer");
            StartCoroutine("WallJumpDelay");
        }
        if (WalledR() && !Grounded() && wallJumpRight)
        {
            wallJumpRight = false;
            isWallJumping = true;
            wallJumpDelay = true;
            Instantiate(explosion, new Vector3(playerCollider.bounds.center.x + .2f, playerCollider.bounds.center.y, playerCollider.bounds.center.z), new Quaternion());
            StartCoroutine("WallJumpTimer");
            StartCoroutine("WallJumpDelay");
        }
    }

    private void HighJump(InputAction.CallbackContext ctx)
    {
        if (Grounded() && highJumpAvailable)
        {
            Instantiate(explosion, new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.center.y - .3f, playerCollider.bounds.center.z), new Quaternion());
            highJumpAvailable = false;
            Vector2 highJumpVel = new Vector2(rigidbody.velocity.x, highJumpForce);
            rigidbody.velocity = highJumpVel;
        }
    }

    private bool isDashing = false;

    private bool canDash = true;
    private void Dash(InputAction.CallbackContext ctx)
    {
        if(!isDashing && dashesAvailable > 0 && canDash)
        {
            isDashing = true;
            canDash = false;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x * dashSpeed, rigidbody.velocity.y);
            StartCoroutine(DashControl());
            StartCoroutine(DashDelay());
            dashesAvailable--;
        }
    }

    public void ResetToSpawn(InputAction.CallbackContext ctx)
    {
        transform.position = Spawn.position;
        ResetAbilities();
    }

    public void ResetAbilities()
    {
        highJumpAvailable = true;
        dashesAvailable = 2;
        wallJumpLeft = true;
        wallJumpRight = true;
    }

    public void SetSpawn(Transform spawn)
    {
        Spawn = spawn;
    }

    #endregion

    #region UpdateMethods

    private void LateUpdate()
    {
        
    }

    private bool wallJumpDelay = false;
    private void Update()
    {
        if (!Grounded())
        {
            WallHang();
        }
        
        if(isWallJumping && !wallJumpDelay && (WalledL() || WalledR()))
        {
            StopCoroutine("WallJumpTimer");
            isWallJumping = false;
            Debug.Log("STOP");
        }

        if(Grounded() && isWallJumping)
        {
            StopCoroutine("WallJumpTimer");
            isWallJumping = false;
        }

        PlayAnimations(rigidbody.velocity.x);

        if(rigidbody.velocity.x > 2)
        {
            viewDirectionX = 1;
        }
        if(rigidbody.velocity.x < -2)
        {
            viewDirectionX = -1;
        }

        if(transform.position.y < -10)
        {
            transform.position = Spawn.position;
            ResetAbilities();
        }
    }

    private void FixedUpdate()
    {
        if(!isWallJumping && !isDashing)
        {
        
            direction = movement.ReadValue<Vector2>();

            Vector2 right = transform.right;

            right.y = 0f;

            right.Normalize();

            velocity = right * direction.x;
            velocity *= moveSpeed;

            velocity.y = rigidbody.velocity.y;

            rigidbody.velocity = velocity;

            
        }
        if (isWallJumping)
        {
            if (WalledL())
            {
                rigidbody.velocity = wallJumpVelocity;
            }
            if (WalledR())
            {
                rigidbody.velocity = new Vector2(-wallJumpVelocity.x, wallJumpVelocity.y);
            }
        }


    }

    #endregion

    #region animation |-------------------------------------------------------------------------------------------------------------------------------------------------

    [SerializeField]
    private GameObject headBodyPart;
    [SerializeField]
    private GameObject bodyBodyPart;
    [SerializeField]
    private GameObject LArmBodyPart;
    [SerializeField]
    private GameObject RArmBodyPart;
    [SerializeField]
    private GameObject LLegBodyPart;
    [SerializeField]
    private GameObject RLegBodyPart;

    private int viewDirectionX = 1;

    private void BodyAnimation(GameObject bodyPart, float running)
    {
        bodyPart.GetComponent<Animator>().SetFloat("Running", running);
        bodyPart.GetComponent<Animator>().SetBool("Grounded", Grounded());
        bodyPart.GetComponent<Animator>().SetInteger("ViewDirectionX", viewDirectionX);
    }

    private void PlayAnimations(float running)
    {
        BodyAnimation(headBodyPart, running);
        BodyAnimation(bodyBodyPart, running);
        BodyAnimation(LArmBodyPart, running);
        BodyAnimation(RArmBodyPart, running);
        BodyAnimation(LLegBodyPart, running);
        BodyAnimation(RLegBodyPart, running);
    }


    #endregion

    #region Coroutines |------------------------------------------------------------------------------------------------------------------------------------------------

    private IEnumerator WallJumpDelay()
    {
        yield return new WaitForSeconds(.1f);
        wallJumpDelay = false;
    }

    private IEnumerator WallJumpTimer()
    {
        yield return new WaitForSeconds(.5f);
        isWallJumping = false;
    }

    private IEnumerator DashControl()
    {
        yield return new WaitForSeconds(.1f);
        isDashing = false;
        rigidbody.velocity = new Vector2(rigidbody.velocity.x / 3, rigidbody.velocity.y);
    }

    private IEnumerator DashDelay()
    {
        yield return new WaitForSeconds(.3f);
        canDash = true;
    }

    #endregion
}
