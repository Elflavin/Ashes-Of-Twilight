using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroStats : MonoBehaviour
{
    public static HeroStats Instance { get; private set; }

    [HideInInspector] public GameObject hero;
    public float hp = 100;
    public float maxHp = 100;
    public float mp = 100;
    public float maxMp = 100;
    public float attack = 5f;
    public float maxAt;
    public int coins = 0;
    [HideInInspector] public string spawnRoom = "GameSpawn";
    [HideInInspector] public string terrain = "rock";
    [HideInInspector] public bool shortcutFounded = false;

    // Jefes muertos
    [HideInInspector] public bool miniBoss = false;
    [HideInInspector] public bool keyBoss = false;
    [HideInInspector] public bool finalBoss = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "NPC Area")
        {
            terrain = "grass";
        } 
        else
        {
            terrain = "rock";
        }
    }

    public void ReceiveDamage(float damage)
    {
        hp -= damage;
        hero.GetComponent<Animator>().SetTrigger("Hit");
        if (hp <= 0)
        {
            hero.GetComponent<Animator>().SetTrigger("Death");
            StartCoroutine(Die());
        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadSceneAsync(spawnRoom);
        yield return new WaitForSeconds(0.1f); // Esperar a que la escena cargue
        hp = maxHp; // Restablecer vida
        mp = maxMp; // Restablecer mana
    }
}
