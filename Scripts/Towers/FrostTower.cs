using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostTower : Tower {

    [SerializeField]
    private float slowingFactor;

    public float SlowingFactor
    {
        get
        {
            return slowingFactor;
        }
    }

    private void Start()
    {
        ElementType = Element.FROST;

        Upgrades = new TowerUpgrade[]
            {
                new TowerUpgrade(2, 1, 1,2,10), //aqui eu mexo nos upgrades
                new TowerUpgrade(2,1,1,2,20),
            };
    }
    public override Debuff GetDebuff()
    {
        return new FrostDebuff(SlowingFactor,DebuffDuration, Target);
    }

    public override string GetStatus()
    {
        if (NextUpgrade != null)  //If the next is avaliable
        {
            return String.Format("<color=#00ffffff>{0}</color>{1} \nSlowing factor: {2}% <color=#00ff00ff>+{3}</color>", "<size=20><b>Frost</b></size>", base.GetStatus(), SlowingFactor, NextUpgrade.SlowingFactor);
        }

        //Returns the current upgrade
        return String.Format("<color=#00ffffff>{0}</color>{1} \nSlowing factor: {2}%", "<size=20><b>Frost</b></size>", base.GetStatus(), SlowingFactor);
    }

    public override void Upgrade()
    {
        this.slowingFactor += NextUpgrade.SlowingFactor;
        base.Upgrade();
    }
}
