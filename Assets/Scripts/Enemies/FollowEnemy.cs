using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FollowEnemy : MonoBehaviour
{
    public Transform hero;
    public float speed = 3f;
    public float followDistance = 5f;
    public float attackDistance = 1f; // Distancia para iniciar el ataque
    private Rigidbody2D rb;
    private Animator animator; // Referencia al componente Animator
    private bool lastAttackWasA = false; // Variable para alternar entre ataques
    public float attackCooldown = 2f; // Tiempo de espera (en segundos) entre ataques
    private float attackTimer = 0f; // Temporizador para controlar el enfriamiento del ataque
    private bool isDead = false; // Indica si el enemigo está muerto

    public int health = 2; // Número de golpes necesarios para matar al enemigo

    // Variables para la patrulla
    public float patrolSpeed = 2f;
    public float patrolDistance = 3f;
    private bool movingRight = true;
    private float startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Inicializa la referencia al Animator
        startPosition = transform.position.x; // Guardar la posición inicial para la patrulla
    }

    void Update()
    {
        if (isDead) return; // Si el enemigo está muerto, no hacer nada

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (hero != null)
        {
            float currentDistance = Vector2.Distance(transform.position, hero.position);

            if (currentDistance < attackDistance)
            {
                Attack();
            }
            else if (currentDistance < followDistance)
            {
                FollowHero();
            }
            else
            {
                Patrol();
            }
        }
    }

    void Attack()
    {
        if (attackTimer <= 0)
        {
            if (lastAttackWasA)
            {
                animator.Play("E_atackB");
                lastAttackWasA = false;
            }
            else
            {
                animator.Play("E_atack");
                lastAttackWasA = true;
            }

            attackTimer = attackCooldown;
            rb.velocity = Vector2.zero;
        }
    }

    void FollowHero()
    {
        Vector2 direction = (hero.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

        if (hero.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (hero.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        animator.Play("E_walk");
    }

    void Patrol()
    {
        if (movingRight)
        {
            rb.velocity = new Vector2(patrolSpeed, rb.velocity.y);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            if (transform.position.x >= startPosition + patrolDistance)
            {
                movingRight = false;
            }
        }
        else
        {
            rb.velocity = new Vector2(-patrolSpeed, rb.velocity.y);
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            if (transform.position.x <= startPosition - patrolDistance)
            {
                movingRight = true;
            }
        }

        animator.Play("E_walk");
    }

    // Método para recibir daño y morir
    public void TakeDamage()
    {
        if (!isDead) // Verificar si ya está muerto para no ejecutar la muerte múltiples veces
        {
            health--; // Reducir la salud en 1
            Debug.Log("Recibió daño. Salud restante: " + health);
            if (health <= 0) // Asegurarse de que health <= 0
            {
                Debug.Log("muertoTake");
                Die();
            }
        }
    }

    void Die()
    {
        if (!isDead) // Asegurarse de que el enemigo no esté ya muerto
        {
            isDead = true; // Marcar como muerto
            rb.velocity = Vector2.zero; // Detener cualquier movimiento
            animator.Play("die"); // Reproducir la animación de muerte
            Debug.Log("Enemigo muerto");

            // Opcional: Desactivar el objeto después de un tiempo
            Destroy(gameObject, 2f); // Destruir el objeto después de 2 segundos (ajusta según necesites)
        }
    }
}
