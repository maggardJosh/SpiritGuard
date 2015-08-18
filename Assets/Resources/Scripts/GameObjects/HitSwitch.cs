using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class HitSwitch : FutilePlatformerBaseObject
{
    private FAnimatedSprite sprite;
    private string doorName;
    bool pressed = false;
    public HitSwitch(string doorName, World world)
        : base(new RXRect(0, 0, 10,10), world)
    {
        this.handleStateCount = true;
        this.blocksOtherObjects = true;
        this.doorName = doorName;
        sprite = new FAnimatedSprite("HitSwitch/switch");
        sprite.addAnimation(new FAnimation("idle", new int[] { 1 }, 100, false));
        sprite.addAnimation(new FAnimation("activate", new int[] { 1,1,1,1,2, 3 }, 150, false));
        sprite.play("idle");
        sprite.ignoreTransitioning = true;
        this.AddChild(sprite);
    }
    public void Activate()
    {
        if (stateCount < .5f)
            return;
        stateCount = 0;
        FSoundManager.PlaySound("hitSwitch");
        if (pressed)
            return;
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

