using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MakeHeroRest : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private bool isChair = false;
    [SerializeField] private GameObject restImage = null;
    private bool playerInChair = false;

    private void Update()
    {
        if (playerInChair && Input.GetKeyDown(KeyCode.E))
        {
            Rest();
        }
    }

    public void Rest()
    {
        HeroStats.Instance.spawnRoom = SceneManager.GetActiveScene().name;
        HeroStats.Instance.hp = HeroStats.Instance.maxHp;
        HeroStats.Instance.mp = HeroStats.Instance.maxMp;
        Time.timeScale = 1f;
        StartCoroutine(FadeImage());
    }

    private IEnumerator FadeImage()
    {
        // Aparecer
        float duration = 1f; // El alfa tarda un segundo en ponerse a 100
        float elapsedTime = 0f;
        Color color = image.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        // Esperar un poco para desaparecer
        yield return new WaitForSeconds(1f);

        // Desaparecer
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (elapsedTime / duration));
            image.color = color;
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isChair && collision.gameObject.CompareTag("Player"))
        {
            restImage.SetActive(true);
            playerInChair = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isChair && collision.gameObject.CompareTag("Player"))
        {
            restImage.SetActive(false);
            playerInChair = false;
        }
    }
}
