using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class HitSwitch : FutilePlatformerBaseObject
{
    private FAnimatedSprite sprite;
    private string doorName;
    bool pressed = false;
    private string name;
    public HitSwitch(string doorName, string name, World world)
        : base(new RXRect(0, 0, 10,10), world)
    {
        this.handleStateCount = true;
        this.blocksOtherObjects = true;
        this.doorName = doorName;
        this.name = name;
        sprite = new FAnimatedSprite("HitSwitch/switch");
        sprite.addAnimation(new FAnimation("idle", new int[] { 1 }, 100, false));
        sprite.addAnimation(new FAnimation("activate", new int[] { 1,1,1,1,2, 3 }, 150, false));
        sprite.addAnimation(new FAnimation("activated", new int[] {3 }, 150, false));
        sprite.play("idle");
        sprite.ignoreTransitioning = true;
        this.AddChild(sprite);

        if (C.Save.switchesActivated.Contains(this.name))
        {
            pressed = true;
            sprite.play("activated");
        }
    }
    public void Activate()
    {
        if (stateCount < .5f)
            return;
        stateCount = 0;
        FSoundManager.PlaySound("hitSwitch");
        if (pressed)
            return;
        C.Save.switchesActivated.Add(this.name);
        sprite.play("activate", true);
        C.getCameraInstance().shake(.7f, .3f);
        pressed = true;

        world.OpenDoor(doorName);

    }
    public void HandlePlayerCollision(Player p)
    {
        if (stateCount < .5f)
            return;
        if (p.shouldDamage && p.swordCollision.isColliding(this))
            Activate();

    }
}

