using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float dmgValue = 4; // Cantidad de daño
    public GameObject spellObject; // Hechizos
    public Transform attackCheck; // Posicion para comprobar hit
    public Animator animator;
    public GameObject cam;
    public bool canAttack = true;
    public float attackRadius = 0.9f; // Radio del area de ataque

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && canAttack)
        {
            canAttack = false;
            StartCoroutine(AttackCooldown());
        }

        if (Input.GetKeyDown(KeyCode.V) && HeroStats.Instance.mp > 0)
        {
            GameObject spell = Instantiate(spellObject, transform.position + new Vector3(transform.localScale.x * 0.5f, -0.2f), Quaternion.identity);
            TurnSpell(spell);
            Vector2 direction = new Vector2(transform.localScale.x, 0);
            spell.GetComponent<Spell>().direction = direction;
            spell.name = "Spell";
            HeroStats.Instance.mp -= 10;
        }
    }

    private void TurnSpell(GameObject spell)
    {
        Vector3 scale = transform.localScale;
        spell.transform.localScale = scale;
    }

    IEnumerator AttackCooldown()
    {
        animator.SetTrigger("Attack");
        DoDamage();
        yield return new WaitForSeconds(0.5f);
        canAttack = true;
    }

    public void DoDamage()
    {
        dmgValue = Mathf.Abs(dmgValue);
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackRadius);
        for (int i = 0; i < collidersEnemies.Length; i++)
        {
            if (collidersEnemies[i].gameObject.tag == "Enemy")
            {
                /* Esto vale para empujar a los enemigos normales *
                if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
                {
                    dmgValue = -dmgValue;
                }*/
                collidersEnemies[i].gameObject.SendMessage("ReceiveDamage", dmgValue);
                RecoverMpAndHp();
                cam.GetComponent<CameraFollow>().ShakeCamera();
            }
        }
    }

    // Para recuperar vida y mana en medio del combate
    private void RecoverMpAndHp()
    {
        if (HeroStats.Instance.mp < HeroStats.Instance.maxMp)
        {
            HeroStats.Instance.mp = Mathf.Clamp(HeroStats.Instance.mp + 5, 0, HeroStats.Instance.maxMp);
        }

        if (HeroStats.Instance.hp < HeroStats.Instance.maxHp)
        {
            HeroStats.Instance.hp = Mathf.Clamp(HeroStats.Instance.hp + 5, 0, HeroStats.Instance.maxHp);
        }
    }

    // Metodo OnDrawGizmos para visualizar el rango de ataque en el editor
    private void OnDrawGizmos()
    {
        if (attackCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackCheck.position, attackRadius);
        }
    }
}
