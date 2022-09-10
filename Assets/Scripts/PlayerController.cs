using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region variables
    private Rigidbody2D rigidbody;

    private Vector2 direction;

    private Vector2 velocity;

    [SerializeField]
    private int moveSpeed;

    [SerializeField]
    private float jumpForce;

    private BoxCollider2D coll;

    [SerializeField] 
    private LayerMask ground;

    [SerializeField]
    private Vector2 wallJumpVelocity = new Vector2(10f, 20f);

    private bool walled = false;
    private bool wallJumpAvailable = false;
    [SerializeField]
    private bool dashAvailable = true;
    [SerializeField]
    private float dashSpeed;
    private bool dashing = false;
    [SerializeField]
    private float highJumpForce;

    [SerializeField]
    private Transform spawn;

    #endregion

    #region resources

    private bool highjump = true;
    private bool wallJumpLeft = true;
    private bool wallJumpRight = true;
    private int dash = 2;

    #endregion

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        transform.position = spawn.position;
    }


    private void FixedUpdate()
    {
        if (Grounded() && !dashing)
        {
            Vector2 right = transform.right;

            right.y = 0f;

            right.Normalize();

            velocity = right * direction.x;
            velocity *= moveSpeed;

            velocity.y = rigidbody.velocity.y;

            rigidbody.velocity = velocity;
        }

        if (Grounded())
        {
            walled = false;
        }

        if (!WalledR() && !WalledL())
        {
            rigidbody.gravityScale = 5f;
            wallJumpAvailable = false;
        }

        if(transform.position.y < -10f)
        {
            transform.position = spawn.position;
            ResetAbilities();
        }

    }

    public bool Grounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, ground);
    }

    public bool WalledR()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, ground);
    }

    public bool WalledL()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, -Vector2.right, .1f, ground);
    }

    public void ResetAbilities()
    {
        highjump = true;
        wallJumpLeft = true;
        wallJumpRight = true;
        dash = 2;
    }

    public void SetSpawn(Transform transform)
    {
        spawn = transform;
    }

    public void ResetToSpawn(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            transform.position = spawn.position;
            ResetAbilities();
        }
    }

    #region Basic Controls

    public void Move(InputAction.CallbackContext ctx)
    {
        direction = ctx.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {

            if (Grounded())
            {
                Vector2 jumpvel = new Vector2(rigidbody.velocity.x, jumpForce);
                rigidbody.velocity = jumpvel + direction;
            }
        }

    }

    public void HighJump(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
        {
            Debug.Log("highjump");
            if (Grounded() && highjump)
            {
                
                Vector2 jumpvel = new Vector2(rigidbody.velocity.x, highJumpForce);
                rigidbody.velocity = jumpvel + direction;
                highjump = false;
                
            }
        }
    }

    
    public void WallHang(InputAction.CallbackContext ctx)
    {
        if (ctx.started && !Grounded())
        {

            if ((WalledR() || WalledL()) && !walled)
            {
                walled = true;
                StartCoroutine(DisableGravity(rigidbody));
            }
        }
        if (ctx.canceled)
        {
            rigidbody.gravityScale = 5f;
            WallJump(ctx);
        }
    }

    public void WallJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started && wallJumpAvailable)
        {
            if (WalledR() && wallJumpRight)
            {
                Vector2 jumpvel = new Vector2(-wallJumpVelocity.x, wallJumpVelocity.y);
                rigidbody.velocity = jumpvel;
                wallJumpRight = false;
            }
            if (WalledL() && wallJumpLeft)
            {
                Vector2 jumpvel = wallJumpVelocity;
                rigidbody.velocity = jumpvel;
                wallJumpLeft = false;
            }
        }
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if (ctx.started && dashAvailable && dash > 0)
        {
            dashAvailable = false;
            dashing = true;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x * dashSpeed, rigidbody.velocity.y);
            StartCoroutine(DashControl());
            StartCoroutine(DashDelay());
            dash--;
        }
    }

    #endregion

    #region Coroutines
    private IEnumerator DisableGravity(Rigidbody2D rb)
    {
        float oldGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        wallJumpAvailable = true;

        yield return new WaitForSeconds(1f);

        rb.gravityScale = 5f;
    }

    private IEnumerator DashDelay()
    {
        yield return new WaitForSeconds(.3f);
        dashAvailable = true;
    }

    private IEnumerator DashControl()
    {
        yield return new WaitForSeconds(.1f);
        dashing = false;
        rigidbody.velocity = new Vector2(rigidbody.velocity.x / 3, rigidbody.velocity.y);
    }
    #endregion
}
