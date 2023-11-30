using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public bool IsPiercing = false;
    bool m_alive = true;

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bullet hit " + collision.gameObject.name);

        if(collision.GetComponent<Damageable>() != null)
            collision.GetComponent<Damageable>().takeDamage(damage);
        else
            collision.GetComponentInParent<Damageable>()?.takeDamage(damage);
        
        if(!IsPiercing)
            Destroy(gameObject);
        m_alive = false;
    }

}
