using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MagicTurret : FutilePlatformerBaseObject
{
    private FSprite sprite;
    private bool loaded = true;
    private float interval;
    private float initDelay = 0;
    private int distance = 4;
    public MagicTurret(float interval, float initDelay, int distance, World world)
        : base(new RXRect(0, -4, 12, 8), world)
    {
        this.distance = distance;
        this.interval = interval;
        handleStateCount = true;
        this.initDelay = initDelay;
        sprite = new FSprite("Magic Turret/turret_magic_01");

        this.AddChild(sprite);

    }

    public override void OnFixedUpdate()
    {
        if (initDelay > 0)
        {
            if (stateCount > initDelay)
            {
                initDelay = 0;
                stateCount = 0;
            }
            return;
        }
        if (loaded)
        {
            if (!(Mathf.Abs(this.x - world.player.x) < distance*16 &&
                Mathf.Abs(this.y - world.player.y) < distance*16))
                stateCount = 0;
            if (stateCount > .5f)
            {
                MagicOrb a = new MagicOrb(this, world);
                a.SetTarget(world.player);
                a.SetPosition(this.GetPosition());
                world.addObject(a);
                a.MoveToFront();
                loaded = false;
                sprite.SetElementByName("Magic Turret/turret_magic_02");
                stateCount = 0;
            }
        }
        else
        {
            if (stateCount > interval)
            {
                sprite.SetElementByName("Magic Turret/turret_magic_01");
                loaded = true;
                stateCount = 0;
            }
        }
        base.OnFixedUpdate();
    }
}

