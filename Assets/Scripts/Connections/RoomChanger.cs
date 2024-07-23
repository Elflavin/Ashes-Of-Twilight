using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomChanger : MonoBehaviour
{
    [SerializeField] private RoomConnection connection;
    [SerializeField] private string targetSceneName;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private bool isDoor;
    [SerializeField] private GameObject image = null;
    private bool playerInDoor = false;

    private void Start()
    {
        if (connection == RoomConnection.ActiveConnection)
        {
            player.transform.position = spawnPoint.position;
        }

        if (image != null)
        {
            image.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInDoor && Input.GetKeyDown(KeyCode.E))
        {
            if (targetSceneName == "NPC Area" && SceneManager.GetActiveScene().name == "Fast Travel Room")
            {
                HeroStats.Instance.shortcutFounded = true;
            }
            RoomConnection.ActiveConnection = connection;
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            RoomConnection.ActiveConnection = connection;
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDoor && collision.gameObject.CompareTag("Player"))
        {
            image.gameObject.SetActive(true);
            playerInDoor = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            image.gameObject.SetActive(false);
            playerInDoor = false;
        }
    }
}
