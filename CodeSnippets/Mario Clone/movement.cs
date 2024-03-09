using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;

public class movement : MonoBehaviour
{

    public float maxSpeed = 1.0f;
    public float maxJump = 1.0f;
    public float smoothness = 1.0f;
    public float groundAcc = 20f;

    public float maxGravity = 4.0f;
    public float minGravity = 3.0f;
    public float maxJumpTime = 1.0f;

    public float jumpTime = 0.0f;
    public bool isJumping = false;


    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;

    public Animator anim;

    public MarioAgent ma;

    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleAnim();
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    public void SetHorzTarget(int t)
    {
        
        float horz = 0f;
        float acc = groundAcc;
        if (t == 1)
        {
            anim.SetBool("isWalking", true);
            horz = -1 * maxSpeed;
        }
        else if(t == 2)
        {
            anim.SetBool("isWalking", true);
            horz =  maxSpeed;
        }
        else
        {
            anim.SetBool("isWalking", false);
            horz = 0f;
        }

        rb.velocity = new Vector2(
            Mathf.MoveTowards(rb.velocity.x, horz, acc * Time.deltaTime * smoothness),
            rb.velocity.y
        );

    }


    public void SetVertTarget(int t, bool useAdvancedJump)
    {

        if (useAdvancedJump)
        {
            switch (t)
            {
                // Jump
                case 1:
                    anim.SetBool("isJumping", true);
                    // If on the ground, add a large jump force
                    if (IsGrounded() && !isJumping)
                    {
                        ma.numJumps += 1f;
                        isJumping = true;
                        rb.velocity = new Vector3(rb.velocity.x, maxJump, 0f);
                        jumpTime = 0f;
                    }
                    else
                    {
                        jumpTime += Time.deltaTime;
                        // If "button is held down", make jump larger by lowering gravity temporarily. This can happen for a max of 1 second
                        if (isJumping && jumpTime < maxJumpTime)
                        {
                            rb.gravityScale = minGravity;
                        }
                        else
                        {
                            rb.gravityScale = maxGravity;
                            isJumping = false;
                            jumpTime = 0f;
                        }
                    }
                    break;
                // Nothing / Stop jump
                default:
                    rb.gravityScale = maxGravity;
                    isJumping = false;
                    jumpTime = 0f;
                    break;
            }
        }
        else
        {
            switch (t)
            {
                // small jump
                case 1:
                    anim.SetBool("isJumping", true);

                    if (IsGrounded() && !isJumping)
                    {
                        ma.numJumps += 1f;
                        isJumping = true;
                        rb.velocity = new Vector3(rb.velocity.x, maxJump * 0.8f, 0f);
                    }
                    StartCoroutine(JumpTimeReset());
                    break;
                // Big jump
                case 2:
                    anim.SetBool("isJumping", true);
                    // If on the ground, add a large jump force
                    if (IsGrounded() && !isJumping)
                    {
                        ma.numJumps += 1f;
                        isJumping = true;
                        rb.velocity = new Vector3(rb.velocity.x, maxJump * 1.2f, 0f);
                    }
                    break;
                // no jump
                default:
                    isJumping = false;
                    break;
            }
        }
        
    }

    private IEnumerator JumpTimeReset()
    {
        yield return new WaitForSeconds(0.05f);
        isJumping = false;
    }

    private void HandleAnim()
    {
        // Walking animation speed modifier
        float ws = Mathf.Lerp(.8f, 1.2f, 0.5f * Mathf.Abs(rb.velocity.x) / maxSpeed);
        anim.SetFloat("walkSpeed", ws);

        // Walking direction
        if (rb.velocity.x > 0.001f && rb.velocity.x < 0.001f)
        {
            //no change
        }
        else if(rb.velocity.x > 0)
        {
            gameObject.GetComponent<Transform>().localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            gameObject.GetComponent<Transform>().localScale = new Vector3(-1f, 1f, 1f);
        }

        // If mario is on the ground then he is not jumping
        if (IsGrounded())
        {
            anim.SetBool("isJumping", false);
        }

    }
}
