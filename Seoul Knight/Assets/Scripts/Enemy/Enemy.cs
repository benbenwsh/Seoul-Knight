﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy: MonoBehaviour
{
    public SpriteRenderer sprite;
    public Rigidbody2D rb;
    public Material material;
    public Animator animator;
    public EnemyAI enemyAI;

    private int hp = 2;
    private bool invincible = false;

    public bool isDead = false;
    public string enemyDirection = "Right";

    private Color materialTintColour;
    
    

    private void Start()
    {
        this.sprite.material.mainTexture = material.mainTexture;
        this.materialTintColour = new Color(255, 255, 255, 0);
        this.sprite.material.SetColor("_Tint", materialTintColour);
    }



    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (!invincible)
        {
            hp -= damage;

            if (hp > 0)
            {
                StartCoroutine(TakeDamageAnimation());
                knockback *= 5;
                rb.AddForce(knockback, ForceMode2D.Impulse);
            }
            else
            {
                //  Die
                Die(knockback);
            }
        }
    }



    private void Die(Vector2 knockback)
    {
        isDead = true;
        enemyAI.CancelInvoke();
        animator.enabled = false;

        knockback *= 20;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        if (enemyDirection == "Right")
        {
            transform.rotation = Quaternion.Euler(Vector3.forward * 90);
        }
        else
        {
            transform.rotation = Quaternion.Euler(Vector3.back * 90);
        }

        sprite.color = new Color(0.25f, 0.25f, 0.25f, 1);
        gameObject.layer = LayerMask.NameToLayer("Dead Enemy");
    }



    IEnumerator TakeDamageAnimation()
    {
        invincible = true;
        materialTintColour = new Color(255, 255, 255, 255);
        this.sprite.material.SetColor("_Tint", materialTintColour);

        
        yield return new WaitForSeconds(0.05f);

        invincible = false;
        materialTintColour = new Color(255, 255, 255, 0);
        this.sprite.material.SetColor("_Tint", materialTintColour);
    }



}