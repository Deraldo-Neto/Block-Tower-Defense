using UnityEngine;
using System.Collections;

public class PoisonSplash : MonoBehaviour
{
    public int Damage { get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<Monster>().TakeDamage(Damage, Element.POISON);
            Destroy(gameObject);
        }
    }
}
