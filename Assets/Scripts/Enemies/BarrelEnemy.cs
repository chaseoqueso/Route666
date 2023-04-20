using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelEnemy : MeleeEnemy
{
    public ExplodingBarrel barrelScript;

    public override void StartAttack()
    {
        barrelScript.OnShoot();
    }
}
