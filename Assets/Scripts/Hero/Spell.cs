using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    // Direccion en la que se movera el hechizo.
    public Vector2 direction;

    // Booleano que indica si el hechizo ha golpeado algo.
    public bool hasHit = false;

    // Velocidad a la que se movera el hechizo.
    public float speed = 10f;

    private Animator animator;
    private Rigidbody2D rb;

    // Metodo llamado en cada frame fijo para mover el hechizo.
    void FixedUpdate()
    {
        //se asigna una velocidad en la direccion especificada.
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    // Metodo llamado cuando el arma colisiona con otro objeto.
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el objeto con el que colisiona es un enemigo aplica el daño
        if (collision.gameObject.tag == "Enemy")
        {
            // collision.gameObject.SendMessage("ReceiveDamage", Mathf.Sign(direction.x) * 2f);
            collision.gameObject.SendMessage("ReceiveDamage", 25f);
            StartCoroutine(HitAnim());
        }
        // Si el objeto con el que colisiona no es el jugador se destruye el hechizo
        else if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "PlayerSpell")
        {
            StartCoroutine(HitAnim());
        }
    }

    private IEnumerator HitAnim()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        animator = gameObject.GetComponent<Animator>();
        animator.SetTrigger("Hit");
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
