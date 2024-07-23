using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Canvas GUI;

    // Movimiento
    private float inputX;
    private float horizontalMovement = 0f;
    [SerializeField] private float movementSpeed;
    [Range(0, 0.3f)][SerializeField] private float movementSmooth;
    private Vector3 speed = Vector3.zero;
    private bool facingRight = true;

    // Salto
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask whatIsFloor;
    [SerializeField] private Transform floorController;
    [SerializeField] private Vector3 boxDimensions;
    [SerializeField] private bool isInFloor;
    private bool jumping = false;

    // Doble Salto
    private bool canDoubleJump = true;

    // Dash
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    private bool isDashing = false;

    // Animacion y sonido
    private Animator animator;
    private HeroSounds sounds;
    [SerializeField] private float stepInterval = 0.5f; // Intervalo de tiempo entre pasos
    private float stepTimer = 0f; // Temporizador para los pasos

    // Sprint
    [SerializeField] private float baseMovementSpeed;
    [SerializeField] private float extraSpeed;
    [SerializeField] private float timeToSprint;
    private float actualSprintTime;
    private float nextSprintTime;
    [SerializeField] private float timeBetweenSprints;
    private bool canRun = true;
    private bool isRunning = false;

    // Wallrun
    [SerializeField] private Transform wallController;
    [SerializeField] private Vector3 wallBoxDimensions;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float wallJumpForceX;
    [SerializeField] private float wallJumpForceY;
    [SerializeField] private float timeWallJump;
    private bool jumpingInWall;
    private bool inWall;
    private bool slidingInWall;

    private void Start()
    {
        HeroStats.Instance.hero = gameObject;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sounds = GetComponent<HeroSounds>();
        actualSprintTime = nextSprintTime;
    }

    private void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        horizontalMovement = inputX * movementSpeed;

        animator.SetFloat("SpeedX", Mathf.Abs(horizontalMovement));
        animator.SetFloat("SpeedY", rb.velocity.y);
        animator.SetBool("WallSliding", slidingInWall);
        animator.SetBool("IsDashing", isDashing);

        // Reproducir sonido de pasos
        if (Mathf.Abs(horizontalMovement) > 0 && isInFloor && !isDashing)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                sounds.Steps();
                stepTimer = stepInterval; // Reiniciar el temporizador
            }
        }
        else
        {
            stepTimer = 0f; // Reiniciar el temporizador si no se está moviendo
        }

        // Si pulsas el boton de saltar
        if (Input.GetButtonDown("Jump"))
        {
            jumping = true;
        }

        // Si pulsas el boton de dash
        if (Input.GetKeyDown(KeyCode.C) && isInFloor && !isDashing)
        {
            StartCoroutine(Dash());
        }

        // Si pulsas o sueltas el boton de correr
        if (Input.GetKeyDown(KeyCode.LeftShift) && canRun)
        {
            movementSpeed = extraSpeed;
            isRunning = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            movementSpeed = baseMovementSpeed;
            isRunning = false;
        }

        if (Math.Abs(rb.velocity.x) >= 0.1f && isRunning)
        {
            if (actualSprintTime > 0)
            {
                actualSprintTime -= Time.deltaTime;
            }
            else
            {
                movementSpeed = baseMovementSpeed;
                isRunning = false;
                canRun = false;
                nextSprintTime = Time.time + timeBetweenSprints;
            }
        }

        if (!isRunning && actualSprintTime <= timeToSprint && Time.time >= nextSprintTime)
        {
            actualSprintTime += Time.deltaTime;
            if (actualSprintTime >= timeToSprint)
            {
                canRun = true;
            }
        }

        // Si estas deslizandote por la pared
        if (!isInFloor && inWall && inputX != 0)
        {
            slidingInWall = true;
        }
        else
        {
            slidingInWall = false;
        }
    }

    private void FixedUpdate()
    {
        isInFloor = Physics2D.OverlapBox(floorController.position, boxDimensions, 0f, whatIsFloor);
        animator.SetBool("InFloor", isInFloor);

        inWall = Physics2D.OverlapBox(wallController.position, wallBoxDimensions, 0f, whatIsFloor);

        MoveHero((horizontalMovement * Time.fixedDeltaTime), jumping);

        jumping = false;

        if (slidingInWall)
        {
            WallSlide();
        }
    }

    private void MoveHero(float move, bool jump)
    {
        if (isDashing)
            return; // Evita que el movimiento normal interfiera con el dash

        Vector3 objectiveSpeed = new Vector2(move, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, objectiveSpeed, ref speed, movementSmooth);

        if (move > 0 && !facingRight)
        {
            TurnHero();
        }
        else if (move < 0 && facingRight)
        {
            TurnHero();
        }

        if (jump && isInFloor)
        {
            MakeHeroJump();
            canDoubleJump = true; // Restablece el doble salto cuando salta desde el suelo
        }
        else if (jump && canDoubleJump && !isInFloor && !slidingInWall)
        {
            MakeHeroDoubleJump();
        }

        if (jump && inWall && slidingInWall)
        {
            MakeHeroWallJump();
        }
    }

    private void TurnHero()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void MakeHeroJump()
    {
        sounds.Jump();
        isInFloor = false;
        rb.AddForce(new Vector2(0f, jumpForce));
    }

    private void MakeHeroDoubleJump()
    {
        sounds.Jump();
        canDoubleJump = false;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0f, jumpForce * 0.8f));
    }

    private void MakeHeroWallJump()
    {
        sounds.Jump();
        inWall = false;
        rb.velocity = new Vector2(wallJumpForceX * -inputX, wallJumpForceY);
        StartCoroutine(ChangeWallJump());
    }

    private IEnumerator ChangeWallJump()
    {
        jumpingInWall = true;
        yield return new WaitForSeconds(timeWallJump);
        jumpingInWall = false;
    }

    private void WallSlide()
    {
        if (rb.velocity.y < -wallSlideSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
    }

    public IEnumerator Die()
    {
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(1.33f);
        GUI.GetComponent<MakeHeroRest>().Rest();
        SceneManager.LoadSceneAsync(HeroStats.Instance.spawnRoom);
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0; // Desactiva la gravedad temporalmente para un mejor control durante el dash
        Vector2 dashVelocity = new Vector2(facingRight ? dashSpeed : -dashSpeed, 0);
        rb.velocity = dashVelocity;

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(floorController.position, boxDimensions);
        Gizmos.DrawWireCube(wallController.position, wallBoxDimensions);
    }
}
