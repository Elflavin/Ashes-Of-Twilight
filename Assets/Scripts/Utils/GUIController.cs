using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GUIController : MonoBehaviour
{
    // GUI Heroe
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private float fillSpeed;
    [SerializeField] private TMP_Text coinText;
    
    // Sonido
    private AudioSource source;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip buySound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip confirmSound;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        UpdateHealthBar();
        UpdateManaBar();
        UpdateCoins();
    }

    private void UpdateHealthBar()
    {
        float targetFillAmount = HeroStats.Instance.hp / HeroStats.Instance.maxHp;
        healthBar.DOFillAmount(targetFillAmount, fillSpeed);
    }

    private void UpdateManaBar()
    {
        float targetFillAmount = HeroStats.Instance.mp / HeroStats.Instance.maxMp;
        manaBar.DOFillAmount(targetFillAmount, fillSpeed);
    }

    private void UpdateCoins()
    {
        coinText.text = HeroStats.Instance.coins.ToString();
    }

    public void HoverSound()
    {
        source.PlayOneShot(hoverSound);
    }

    public void BuySound()
    {
        source.PlayOneShot(buySound);
    }

    public void ExitShopSound()
    {
        source.PlayOneShot(closeSound);
    }

    public void ConfirmSound()
    {
        source.PlayOneShot(confirmSound);
    }
}
