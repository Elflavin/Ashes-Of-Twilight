using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBSpellController : MonoBehaviour
{
    [SerializeField] private float dmgValue;
    [SerializeField] private Vector2 boxDimensions;
    [SerializeField] private Transform boxPosition;
    [SerializeField] private float ttl;

    private void Start()
    {
        Destroy(gameObject, ttl);
    }

    public void Hit()
    {
        Collider2D[] objects = Physics2D.OverlapBoxAll(boxPosition.position, boxDimensions, 0f);
        foreach (Collider2D obj in objects)
        {
            if (obj.CompareTag("Player"))
            {
                HeroStats.Instance.ReceiveDamage(dmgValue);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxPosition.position, boxDimensions);
    }
}
