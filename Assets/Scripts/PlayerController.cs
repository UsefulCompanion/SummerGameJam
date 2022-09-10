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

    #endregion

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
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
            if (WalledR())
            {
                Vector2 jumpvel = new Vector2(-wallJumpVelocity.x, wallJumpVelocity.y);
                rigidbody.velocity = jumpvel;
            }
            if (WalledL())
            {
                Vector2 jumpvel = wallJumpVelocity;
                rigidbody.velocity = jumpvel;
            }
        }
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if (ctx.started && dashAvailable)
        {
            dashAvailable = false;
            dashing = true;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x * 3f, rigidbody.velocity.y);
            StartCoroutine(DashControl());
            StartCoroutine(DashDelay());
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
