using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject shopImage;
    [SerializeField] private GameObject hero;
    [SerializeField] private TMP_Text totalHp;
    [SerializeField] private TMP_Text totalMp;
    [SerializeField] private TMP_Text totalAt;
    [SerializeField] private TMP_Text priceHp;
    [SerializeField] private TMP_Text priceMp;
    [SerializeField] private TMP_Text priceAt;
    [SerializeField] private TMP_Text totalText;

    private bool isPlayerInRange;
    private bool didShopOpen;

    // Contador para el limite de compras de la partida
    private static int hpCount = 0;
    private static int mpCount = 0;
    private static int atCount = 0;

    // Cantidad a comprar en este momento
    private int buyHp = 0;
    private int buyMp = 0;
    private int buyAt = 0;

    // Dinero
    private int playerCoins;
    private int total = 0;
    private int itemPrice;

    private const int MaxItemCount = 10; // Maximo numero de items que se pueden comprar

    private void Start()
    {
        itemPrice = 100;
        playerCoins = HeroStats.Instance.coins;
        isPlayerInRange = false;
        didShopOpen = false;
        UpdateUI();
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!didShopOpen)
            {
                OpenShop();
            }
            else
            {
                CloseShop(false);
            }
        }
    }

    private void OpenShop()
    {
        didShopOpen = true;
        shopPanel.SetActive(true);
        hero.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void CloseShop(bool emptyStock)
    {
        // Si no he comprado nada, restablezco el limite de compras
        if (!emptyStock)
        {
            hpCount -= buyHp;
            mpCount -= buyMp;
            atCount -= buyAt;
        }

        // Restablecer variables de compra a 0
        buyHp = 0;
        buyMp = 0;
        buyAt = 0;

        // Actualizar monedas del jugador
        playerCoins = HeroStats.Instance.coins;

        // Actualizar UI
        UpdateUI();

        didShopOpen = false;
        shopPanel.SetActive(false);
        hero.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        hero.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void BuyHp()
    {
        if (hpCount < MaxItemCount && playerCoins >= itemPrice)
        {
            hpCount++;
            buyHp++;
            playerCoins -= itemPrice;
            UpdateUI();
        }
    }

    public void BuyMp()
    {
        if (mpCount < MaxItemCount && playerCoins >= itemPrice)
        {
            mpCount++;
            buyMp++;
            playerCoins -= itemPrice;
            UpdateUI();
        }
    }

    public void BuyAt()
    {
        if (atCount < MaxItemCount && playerCoins >= itemPrice)
        {
            atCount++;
            buyAt++;
            playerCoins -= itemPrice;
            UpdateUI();
        }
    }

    public void RemoveHp()
    {
        if (buyHp > 0)
        {
            hpCount--;
            buyHp--;
            playerCoins += itemPrice;
            UpdateUI();
        }
    }

    public void RemoveMp()
    {
        if (buyMp > 0)
        {
            mpCount--;
            buyMp--;
            playerCoins += itemPrice;
            UpdateUI();
        }
    }

    public void RemoveAt()
    {
        if (buyAt > 0)
        {
            atCount--;
            buyAt--;
            playerCoins += itemPrice;
            UpdateUI();
        }
    }

    public void BuyStock()
    {
        HeroStats.Instance.coins -= total;
        HeroStats.Instance.maxHp += buyHp * 10;
        HeroStats.Instance.maxMp += buyMp * 10;
        HeroStats.Instance.maxAt += buyAt * 2;

        // Restablecer variables de compra a 0 después de comprar
        buyHp = 0;
        buyMp = 0;
        buyAt = 0;

        // Actualizar UI después de comprar
        UpdateUI();

        CloseShop(true);
    }

    private void UpdateUI()
    {
        totalHp.text = hpCount >= MaxItemCount ? "MAX" : $"x{buyHp} vida";
        priceHp.text = $"| {buyHp * itemPrice}";
        totalMp.text = mpCount >= MaxItemCount ? "MAX" : $"x{buyMp} mana";
        priceMp.text = $"| {buyMp * itemPrice}";
        totalAt.text = atCount >= MaxItemCount ? "MAX" : $"x{buyAt} ataque";
        priceAt.text = $"| {buyAt * itemPrice}";
        total = (buyHp * itemPrice) + (buyMp * itemPrice) + (buyAt * itemPrice);
        totalText.text = $"Total: {total}";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            shopImage.SetActive(true);
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            shopImage.SetActive(false);
            isPlayerInRange = false;
        }
    }
}
