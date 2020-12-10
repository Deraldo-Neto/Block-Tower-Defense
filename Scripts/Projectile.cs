using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private Monster target;

    private Tower parent;

    private Animator myAnimator;

    private Element elementType;

	void Start () {
        myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        MoveToTarget();
	}

    public void Initialized(Tower parent) //função inicializadora
    {
        this.target = parent.Target;
        this.parent = parent;
        this.elementType = parent.ElementType;
    }

    private void MoveToTarget()
    {
        if (target != null && target.IsActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * parent.ProjectileSpeed); //ir em direção do inimigo

            Vector2 dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else if (!target.IsActive)
        {
            GameManager.Instance.Pool.ReleaseObject(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            if (target.gameObject == other.gameObject)
            {
                target.TakeDamage(parent.Damage, elementType);
                myAnimator.SetTrigger("Impact");

                ApplyDebuff();

            }

        }
    }

    private void ApplyDebuff()
    {
        if (target.ElementType != elementType)
        {
            float roll = Random.Range(0, 100);

            if (roll <= parent.Proc)
            {
                target.AddDebuff(parent.GetDebuff());
            } //cria change para efetuar o debuff
        }
    }
}
