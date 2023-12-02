using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D m_body;
    float m_horizontal;
    float m_vertical;
    Vector3 m_zero = Vector3.zero;

    [Header("Basic Movement")]
    //How strong the movement smoothing is
    public float moveSmoothing = .05f;

    //How much the player's diagonal movement is limited
    public float moveLimiter = 0.7f;

    //How fast the player moves
    public float runSpeed = 20.0f;

    //iFrames Duration
    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;

    //Flashes that occur before the player gets out of iFrames
    [SerializeField] private int numberOfFlashes;

    [SerializeField] private TrailRenderer trail;

    private SpriteRenderer spriteOpacity;

    private bool canDash = true;
    private bool isDashing;
    // private float dashingPower = 30f;
    [Header("Dashing")]
    [SerializeField] private float dashingPower = 60f;
    [SerializeField] private float dashingTime = 0.01f;
    [SerializeField] private float dashingCooldown = 1f;

    public void Awake()
    {
        m_body = GetComponent<Rigidbody2D>();
        spriteOpacity = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //In order to not have the player do any other movements if it is currently dashing we need to add this code block
        if (isDashing)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
            StartCoroutine(Dash());
    }

    void FixedUpdate()
    {
        m_horizontal = Input.GetAxisRaw("Horizontal");
        m_vertical = Input.GetAxisRaw("Vertical");

        //Limit movement if moving diagonal
        if (m_horizontal != 0 && m_vertical != 0)
        {
            m_horizontal *= moveLimiter;
            m_vertical *= moveLimiter;
        }

        // This finds the target velocity of the player
        Vector3 targetVelocity = new Vector2(m_horizontal * runSpeed, m_vertical * runSpeed);
        // Creates fluid movement by smoothing out the difference in target and current velocity
        m_body.velocity = Vector3.SmoothDamp(m_body.velocity, targetVelocity, ref m_zero, moveSmoothing);
    }

    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);
        //invulnerability duration
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteOpacity.color = new Color(255, 255, 255, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteOpacity.color = new Color(255, 255, 255, 255);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(7, 8, false);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        StartCoroutine(Invulnerability());
        float originalGravity = m_body.gravityScale;
        m_body.gravityScale = 0f;
        m_horizontal = Input.GetAxisRaw("Horizontal");
        m_vertical = Input.GetAxisRaw("Vertical");
        if (m_horizontal != 0 && m_vertical != 0)
        {
            m_horizontal *= moveLimiter;
            m_vertical *= moveLimiter;
        }
        m_body.velocity = new Vector2(m_horizontal * dashingPower, m_vertical * dashingPower);
        trail.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        trail.emitting = false;
        m_body.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
