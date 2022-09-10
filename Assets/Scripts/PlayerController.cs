using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidbody;

    private Vector2 direction;

    private Vector2 velocity;
    [SerializeField]
    private int moveSpeed;
    [SerializeField]
    private float jumpForce;

    private BoxCollider2D coll;
    [SerializeField] private LayerMask ground;
    

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        direction = ctx.ReadValue<Vector2>();
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

    private bool walled = false;
    public void WallHang()
    {
        if((WalledR() || WalledL()) && !walled)
        {
            walled = true;
            StartCoroutine(DisableGravity(rigidbody));
           
        }
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

    private bool wallJumpAvailable = false;
    public void WallJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started && wallJumpAvailable)
        {
            Vector2 jumpvel = new Vector2(-10f, 10f);
            rigidbody.velocity = jumpvel;
        }
    }

    private void FixedUpdate()
    {
        if (Grounded() || walled) { 
            Vector2 right = transform.right;

            right.y = 0f;

            right.Normalize();

            velocity = right * direction.x;
            velocity *= moveSpeed;

            velocity.y = rigidbody.velocity.y;

            rigidbody.velocity = velocity;
        }

        WallHang();

        if (Grounded())
        {
            walled = false;
        }

        if(!WalledR() && !WalledL())
        {
            rigidbody.gravityScale = 5f;
            wallJumpAvailable = false;
        }
    }

    private IEnumerator DisableGravity(Rigidbody2D rb)
    {
        float oldGravity = rb.gravityScale;
        Debug.Log(rb.gravityScale);
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        wallJumpAvailable = true;

        yield return new WaitForSeconds(1f);

        //Debug.Log("waited");

        rb.gravityScale = 5f;
    }
}
