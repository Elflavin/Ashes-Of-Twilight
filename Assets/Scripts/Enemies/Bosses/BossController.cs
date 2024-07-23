using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossController : MonoBehaviour
{
    // Componentes GameObject
    private Animator animator;
    private Rigidbody2D rb;
    [SerializeField] private GameObject barrier;
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject BHObject;
    [SerializeField] private Image bossHealthBar;
    [SerializeField] private float fillSpeed;

    // Jugador
    public Transform player;

    // Objetos de ataque
    [SerializeField] private float attackRadius = 0.9f;
    [SerializeField] private Transform attackCheck;

    // Atributos del jefe
    [SerializeField] private float dmgValue = 20;
    [SerializeField] private float hp;
    private float maxHp;

    // Boleanos de ayuda
    private bool facingRight = true;
    private bool giveCoins = true;

    // Que jefe es
    [SerializeField] private string bossName;


    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        maxHp = hp;
        CancelBossFight();
    }

    private void Update()
    {
        float distanceFromPlayer = Vector2.Distance(transform.position, player.position);
        animator.SetFloat("DistanceFromPlayer", distanceFromPlayer);
    }

    public void Attack()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(attackCheck.position, attackRadius);
        foreach (var item in objects)
        {
            if (item.CompareTag("Player"))
            {
                HeroStats.Instance.ReceiveDamage(dmgValue);
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
        animator.SetTrigger("Die"); // Animacion de muerte
        ChangeStatValue(); // Marcar el jefe como muerto para que no vuelva a aparecer
        cam.GetComponent<AudioSource>().Stop(); // Parar la musica de la sala
        BHObject.SetActive(false); // Ocultar barra de vida
        barrier.GetComponent<Collider2D>().isTrigger = true; // Desbloquear puerta

        if (giveCoins)
        {
            Reward();   
        }

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }


    public void FacePlayer()
    {
        if ((player.position.x > transform.position.x && !facingRight) || (player.position.x < transform.position.x && facingRight))
        {
            facingRight = !facingRight;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
        }
    }

    // Recompensa del jefe
    private void Reward()
    {
        if (giveCoins)
        {
            int coinsToGive = 0;
            switch (bossName)
            {
                case "mini":
                    coinsToGive = 200; // cantidad para "mini"
                    break;
                case "key":
                    coinsToGive = 500; // cantidad para "key"
                    break;
                case "final":
                    coinsToGive = 10000; // cantidad para "final"
                    break;
                default:
                    coinsToGive = 0; // Cantidad predeterminada si no coincide con ningun caso
                    break;
            }
            HeroStats.Instance.AddCoins(coinsToGive);
        }
    }

    // Indicar la muerte del jefe sin importar las salas
    private void ChangeStatValue()
    {
        switch (bossName)
        {
            case "mini":
                HeroStats.Instance.miniBoss = true; break;
            case "key":
                HeroStats.Instance.keyBoss = true; break;
            case "final":
                HeroStats.Instance.finalBoss = true; break;
            default:
                Debug.Log("No le has puesto nombre al jefe");
                break;
        }
    }

    // Cancelar la batalla si ya has derrotado al jefe
    private void CancelBossFight()
    {
        if (bossName == "mini" && HeroStats.Instance.miniBoss)
        {
            giveCoins = false;
            StartCoroutine(Die());
        } 
        else if (bossName == "key" && HeroStats.Instance.keyBoss)
        {
            giveCoins = false;
            StartCoroutine(Die());
        }
        else if (bossName == "final" && HeroStats.Instance.finalBoss)
        {
            giveCoins = false;
            StartCoroutine(Die());
        }
    }

    private void OnDrawGizmos()
    {
        if (attackCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackCheck.position, attackRadius);
        }
    }
}
