using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    public SpriteRenderer jellySprite;

    private void Start()
    {
        jellySprite = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (hitPoints <= 0)
            Destroy(this.gameObject);
    }
    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeLighter();
    }

    private void MakeLighter()
    {
        Color color = jellySprite.color;
        float alpha = color.a * .5f;
        jellySprite.color = new Color(color.r, color.g, color.b, alpha);
    }
}
