using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float jumpForce = 400f;                             // Fuerza que se anyade al salto del personaje
    [Range(0, .3f)][SerializeField] private float movementSmoothing = .05f;      // Suavizar el movimiento
    [SerializeField] private bool airControl = false;                            // Regular control en el aire del jugador
    [SerializeField] private LayerMask whatIsGround;                             // Mascara para confirmar que es el suelo
    [SerializeField] private Transform groundCheck;                              // Posicion que controla si el personaje esta tocando el suelo
    [SerializeField] private Transform wallCheck;                                // Posicion que controla si el personaje toca una pared

    const float GROUNDED_RADIUS = .2f; // Constante que define el radio del circulo de solapamiento para determinar si el jugador esta en el suelo
    private bool grounded;            // Esta o no en el suelo
    private Rigidbody2D rigidbody2D;
    private bool facingRight = true;  // Para determinar la direccion en la que esta mirando el jugador
    private Vector3 velocity = Vector3.zero;
    private float limitFallSpeed = 25f; // Limitar la velocidad de caida

    public bool canDoubleJump = true; // Si se puede hacer doble salto
    [SerializeField] private float dashForce = 5f;
    private bool canDash = true;
    private bool isDashing = false; // El jugador esta dasheando
    private bool isWall = false; // Hay una pared frente al jugador
    private bool isWallSliding = false; // El jugador se esta deslizando por la pared
    private bool oldWallSlidding = false; // El jugador se estaba deslizando por la pared
    private float prevVelocityX = 0f;
    private bool canCheck = false; // Puede comprobar si se esta deslizando

    public float life = 10f; // Vida del jugador
    public bool invincible = false; // El jugador puede morir
    private bool canMove = true; // El jugador se puede mover

    private Animator animator;

    private float jumpWallStartX = 0;
    private float jumpWallDistX = 0; // Distancia entre jugador y pared
    private bool limitVelOnWallJump = false; // Limitar la distancia del walljump

    [Header("Events")]
    [Space]

    public UnityEvent OnFallEvent;
    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (OnFallEvent == null)
            OnFallEvent = new UnityEvent();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }


    private void FixedUpdate()
    {
        bool wasGrounded = grounded;
        grounded = false;
        animator.SetBool("IsInFloor", false);

        // El circulo de solapamiento detecta si el jugador esta tocando el suelo usando capas
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, GROUNDED_RADIUS, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                grounded = true;
                animator.SetBool("IsInFloor", true);
            if (!wasGrounded)
            {
                OnLandEvent.Invoke();
                if (!isWall && !isDashing)
                canDoubleJump = true;
                if (rigidbody2D.velocity.y < 0f)
                    limitVelOnWallJump = false;
            }
        }

        isWall = false;

        if (!grounded)
        {
            OnFallEvent.Invoke();
            Collider2D[] collidersWall = Physics2D.OverlapCircleAll(wallCheck.position, GROUNDED_RADIUS, whatIsGround);
            for (int i = 0; i < collidersWall.Length; i++)
            {
                if (collidersWall[i].gameObject != null)
                {
                    isDashing = false;
                    isWall = true;
                }
            }
            prevVelocityX = rigidbody2D.velocity.x;
        }

        if (limitVelOnWallJump)
        {
            if (rigidbody2D.velocity.y < -0.5f)
                limitVelOnWallJump = false;
            jumpWallDistX = (jumpWallStartX - transform.position.x) * transform.localScale.x;
            if (jumpWallDistX < -0.5f && jumpWallDistX > -1f)
            {
                canMove = true;
            }
            else if (jumpWallDistX < -1f && jumpWallDistX >= -2f)
            {
                canMove = true;
                rigidbody2D.velocity = new Vector2(10f * transform.localScale.x, rigidbody2D.velocity.y);
            }
            else if (jumpWallDistX < -2f)
            {
                limitVelOnWallJump = false;
                rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
            }
            else if (jumpWallDistX > 0)
            {
                limitVelOnWallJump = false;
                rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
            }
        }
    }


    public void Move(float move, bool jump, bool dash)
    {
        if (canMove)
        {
            // Hacer el dash
            if (dash && canDash && !isWallSliding)
            {
                rigidbody2D.AddForce(new Vector2(transform.localScale.x * dashForce, 0f));
                StartCoroutine(DashCooldown());
            }
            // Si el personaje esta agachado comprobar si se puede levantar
            if (isDashing)
            {
                rigidbody2D.velocity = new Vector2(transform.localScale.x * dashForce, 0);
            }
            // Controlar el personaje solo si esta en el suelo o se puede mover en el aire
            else if (grounded || airControl)
            {
                if (rigidbody2D.velocity.y < -limitFallSpeed)
                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -limitFallSpeed);
                // Mover al personaje aplicando la fuerza que toque
                Vector3 targetVelocity = new Vector2(move * 10f, rigidbody2D.velocity.y);
                // Suavizar el movimiento
                rigidbody2D.velocity = Vector3.SmoothDamp(rigidbody2D.velocity, targetVelocity, ref velocity, movementSmoothing);

                // Si te mueves a la derecha pero miras a la izquierda...
                if (move > 0 && !facingRight && !isWallSliding)
                {
                    // ... voltear jugador.
                    Flip();
                }
                // Si te mueves a la izquierda pero miras a la derecha...
                else if (move < 0 && facingRight && !isWallSliding)
                {
                    // ... voltear jugador.
                    Flip();
                }
            }
            // Si el jugador puede saltar
            if (grounded && jump)
            {
                // Anyadir una fuerza vertical al jugador y cambiar las animaciones
                animator.SetBool("JumpUp", true);
                grounded = false;
                rigidbody2D.AddForce(new Vector2(0f, jumpForce));
                canDoubleJump = true;
            }
            else if (!grounded && jump && canDoubleJump && !isWallSliding)
            {
                canDoubleJump = false;
                animator.SetBool("JumpUp", true);
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
                rigidbody2D.AddForce(new Vector2(0f, jumpForce / 1.2f));
            }

            else if (isWall && !grounded)
            {
                if (!oldWallSlidding && rigidbody2D.velocity.y < 0 || isDashing)
                {
                    isWallSliding = true;
                    wallCheck.localPosition = new Vector3(-wallCheck.localPosition.x, wallCheck.localPosition.y, 0);
                    Flip();
                    StartCoroutine(WaitToCheck(0.1f));
                    canDoubleJump = true;
                    animator.SetBool("IsWallSliding", true);
                }
                isDashing = false;

                if (isWallSliding)
                {
                    if (move * transform.localScale.x > 0.1f)
                    {
                        StartCoroutine(WaitToEndSliding());
                    }
                    else
                    {
                        oldWallSlidding = true;
                        rigidbody2D.velocity = new Vector2(-transform.localScale.x * 2, -5);
                    }
                }

                if (jump && isWallSliding)
                {
                    animator.SetBool("JumpUp", true);
                    Debug.Log("JumpUp es true");
                    rigidbody2D.velocity = new Vector2(0f, 0f);
                    rigidbody2D.AddForce(new Vector2(transform.localScale.x * jumpForce * 1.2f, jumpForce));
                    jumpWallStartX = transform.position.x;
                    limitVelOnWallJump = true;
                    canDoubleJump = true;
                    isWallSliding = false;
                    animator.SetBool("IsWallSliding", false);
                    oldWallSlidding = false;
                    wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);
                    canMove = false;
                }
                else if (dash && canDash)
                {
                    isWallSliding = false;
                    animator.SetBool("IsWallSliding", false);
                    oldWallSlidding = false;
                    wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);
                    canDoubleJump = true;
                    StartCoroutine(DashCooldown());
                }
            }
            else if (isWallSliding && !isWall && canCheck)
            {
                isWallSliding = false;
                animator.SetBool("IsWallSliding", false);
                oldWallSlidding = false;
                wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);
                canDoubleJump = true;
            }
        }
    }


    private void Flip()
    {
        // Cambia la direccion a la que mira el personaje
        facingRight = !facingRight;

        // Multiplica la x por -1 y asi le da la vuelta al sprite
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void ApplyDamage(float damage, Vector3 position)
    {
        if (!invincible)
        {
            animator.SetBool("Hit", true);
            life -= damage;
            Vector2 damageDir = Vector3.Normalize(transform.position - position) * 40f;
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.AddForce(damageDir * 10);
            if (life <= 0)
            {
                StartCoroutine(WaitToDead());
            }
            else
            {
                StartCoroutine(Stun(0.25f));
                StartCoroutine(MakeInvincible(1f));
            }
        }
    }

    IEnumerator DashCooldown()
    {
        animator.SetBool("IsDashing", true);
        isDashing = true;
        canDash = false;
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("IsDashing", false);
        isDashing = false;
        yield return new WaitForSeconds(0.5f);
        canDash = true;
    }

    IEnumerator Stun(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
    IEnumerator MakeInvincible(float time)
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
    }
    IEnumerator WaitToMove(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    IEnumerator WaitToCheck(float time)
    {
        canCheck = false;
        yield return new WaitForSeconds(time);
        canCheck = true;
    }

    IEnumerator WaitToEndSliding()
    {
        yield return new WaitForSeconds(0.1f);
        canDoubleJump = true;
        isWallSliding = false;
        animator.SetBool("IsWallSliding", false);
        oldWallSlidding = false;
        wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x), wallCheck.localPosition.y, 0);
    }

    IEnumerator WaitToDead()
    {
        animator.SetBool("IsDead", true);
        canMove = false;
        invincible = true;
        GetComponent<Attack>().enabled = false;
        yield return new WaitForSeconds(0.4f);
        rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}