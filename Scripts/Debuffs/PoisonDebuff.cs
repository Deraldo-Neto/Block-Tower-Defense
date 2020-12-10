using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDebuff : Debuff {

    private float timeSinceTick;
    private float tickTime;

    private PoisonSplash splashPrefab;

    private int splashDamage;

    public PoisonDebuff(int splashDamage, float tickTime, PoisonSplash splashPrefab,float duration,  Monster target) : base(target,duration)
    {
        this.splashDamage = splashDamage;
        this.tickTime = tickTime;
        this.splashPrefab = splashPrefab;

    }

    public override void Update()
    {
        if (target != null)
        {
            timeSinceTick += Time.deltaTime;
            if (timeSinceTick >= tickTime)
            {
                timeSinceTick = 0;
                splash();
            }
        }
        base.Update();
    }

    private void splash()
    {
        PoisonSplash tmp = GameObject.Instantiate(splashPrefab, target.transform.position, Quaternion.identity);

        tmp.Damage = splashDamage;

        Physics2D.IgnoreCollision(target.GetComponent<Collider2D>(), tmp.GetComponent<Collider2D>());
    }
}
