using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormTower : Tower {
    private void Start()
    {
        ElementType = Element.STORM;

        Upgrades = new TowerUpgrade[]
           {
                new TowerUpgrade(2,2,1,2), //aqui eu mexo nos upgrades
                new TowerUpgrade(5,4,1,2),
           };
    }
    public override Debuff GetDebuff()
    {
        return new StormDebuff(Target, DebuffDuration);
    }

    public override string GetStatus()
    {
        return string.Format("<color=#add8e6ff>{0}</color>{1}", "<size=20><b>Storm</b></size>", base.GetStatus());
    }
}
