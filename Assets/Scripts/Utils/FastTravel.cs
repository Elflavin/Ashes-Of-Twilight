using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FastTravel : MonoBehaviour
{
    public string objectiveRoom;


    public void TeleportHero()
    {
        if (HeroStats.Instance.shortcutFounded)
        {
            TeleportWithMoney();
        }
        else
        {
            SceneManager.LoadSceneAsync(objectiveRoom);
        }
    }

    private void TeleportWithMoney()
    {
        if (HeroStats.Instance.coins >= 100000000 && !HeroStats.Instance.shortcutFounded)
        {
            HeroStats.Instance.coins -= 100000000;
            HeroStats.Instance.shortcutFounded = true;
            SceneManager.LoadSceneAsync(objectiveRoom);
        }
    }
}
