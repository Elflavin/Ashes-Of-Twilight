/*using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossController : MonoBehaviour
{
    // Componentes GameObject
    private Animator animator;
    private Rigidbody2D rb;
    [SerializeField] private GameObject barrier;
    [SerializeField] private Image bossHealthBar;
    [SerializeField] private float fillSpeed;

    // Ajustes movimiento
    [SerializeField] private Transform player;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float followSpeed = 5f;

    // Objetos de busqueda
    [SerializeField] private float attackRadius = 0.9f;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float playerCheckRadius = 6f;
    [SerializeField] private Transform playerCheck;

    // Atributos del jefe
    [SerializeField] private float dmgValue = 20;
    [SerializeField] private float hp;
    private float maxHp;

    // Boleanos de ayuda
    private bool facingLeft = true;
    private bool canAttack = true;


    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        maxHp = hp;
    }

    private void Update()
    {
        FacePlayer();

        if (LookForPlayer())
        {
            FollowPlayer();
            if (canAttack)
            {
                canAttack = false;
                StartCoroutine(WaitToAttack());
            }
        }
        else
        {
            Patrol();
        }

        animator.SetFloat("SpeedX", Mathf.Abs(rb.velocity.x));
    }

    private bool LookForPlayer()
    {
        Collider2D[] colliderEnemies = Physics2D.OverlapCircleAll(playerCheck.position, playerCheckRadius);
        foreach (var item in colliderEnemies)
        {
            if (item.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * followSpeed, rb.velocity.y);

        if (direction.x > 0 && facingLeft)
        {
            Flip();
        }
        else if (direction.x < 0 && !facingLeft)
        {
            Flip();
        }
    }

    private void Patrol()
    {
        Vector2 patrolPosition = new Vector2(transform.position.x + (facingLeft ? -1 : 1) * patrolSpeed * Time.deltaTime, transform.position.y);
        rb.MovePosition(patrolPosition);
        animator.SetFloat("SpeedX", 1f);
    }

    public void Attack()
    {
        dmgValue = Mathf.Abs(dmgValue);
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackRadius);
        for (int i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].gameObject.tag == "Player" && canAttack)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                canAttack = false;
                animator.SetTrigger("Attack");
                // collidersEnemies[i].gameObject.SendMessage("ReceiveDamage", dmgValue);
            }
        }
    }

    public void ReceiveDamage(float damage)
    {
        hp -= damage;
        UpdateBossHealthBar();

        if (hp <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            animator.SetTrigger("Hitted");
        }
    }

    private void UpdateBossHealthBar()
    {
        float targetFillAmount = hp / maxHp;
        bossHealthBar.DOFillAmount(targetFillAmount, fillSpeed);
    }

    private IEnumerator Die()
    {
        animator.SetTrigger("Die");
        UnlockDoor();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private IEnumerator WaitToAttack()
    {
        Attack();
        yield return new WaitForSeconds(4f);
        canAttack = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY; // Descongelar movimiento en X y Y, pero mantener la rotacion congelada
    }

    private void FacePlayer()
    {
        if ((player.position.x < transform.position.x && !facingLeft) || (player.position.x > transform.position.x && facingLeft))
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void UnlockDoor()
    {
        barrier.GetComponent<Collider2D>().isTrigger = true;
    }
    
    private void OnDrawGizmos()
    {
        if (attackCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackCheck.position, attackRadius);
        }

        if (playerCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerCheck.position, playerCheckRadius);
        }
    }
}
*/