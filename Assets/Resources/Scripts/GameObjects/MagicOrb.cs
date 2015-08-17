using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MagicOrb : FutilePlatformerBaseObject
{
    public FNode owner;
    FAnimatedSprite sprite;
    FutilePlatformerBaseObject target;
    public MagicOrb(FNode owner, World world)
        : base(new RXRect(0, 0, 5, 5), world)
    {
        this.useActualMaxVel = true;
        maxVel = 2;
        this.bounceiness = 1f;
        this.owner = owner;
        handleStateCount = true;
        collidesWithWater = false;
        sprite = new FAnimatedSprite("Magic Orb/magic_orb");
        sprite.addAnimation(new FAnimation("idle", new int[] { 1, 2 }, 150, true));
        sprite.play("idle");
        this.AddChild(sprite);
        this.xVel = RXRandom.Float() * .5f - .25f;
        this.yVel = RXRandom.Float() * .5f - .25f;
        FSoundManager.PlaySound("orbShoot");
    }

    public void SetTarget(FutilePlatformerBaseObject node)
    {
        target = node;
    }

    public override void OnFixedUpdate()
    {
        if (RXRandom.Float() < .3f)
            SpawnParticles(2);
        if (target != null)
        {
            Vector2 diff = (this.GetPosition() - target.GetPosition() - new Vector2(target.hitBox.x, target.hitBox.y)).normalized;
            xAcc = -diff.x * .1f;
            yAcc = -diff.y * .1f;
        }
        else
        {
            xAcc = 0;
            yAcc = 0;
        }
        if (stateCount > 3.0f)
        {
            FSoundManager.PlaySound("orbExplosion");
            SpawnParticles(30, true);
            world.removeObject(this);
        }
        base.OnFixedUpdate();
    }
    public void HandlePlayerCollision(Player p)
    {
        if (p.invulnCount > 0)
            return;
        if (p.isColliding(this))
        {
            FSoundManager.PlaySound("orbExplosion");
            p.TakeDamage(p.GetPosition() - new Vector2(xVel, yVel));
            SpawnParticles(30, true);
            world.removeObject(this);
        }
    }
    private void SpawnParticles(int numParticles = 10, bool explosion = false)
    {
        for (int i = 0; i < numParticles; i++)
        {
            Particle.ParticleOne p = Particle.ParticleOne.getParticle();
            Vector2 vel = new Vector2(-(xVel + RXRandom.Float() * xVel) * 20 + (explosion ? 60 * RXRandom.Float() -30 : 0), -(yVel + RXRandom.Float() * yVel) * 20 + (explosion ? 60 * RXRandom.Float() - 30: 0));
            Vector2 acc = new Vector2(xAcc, yAcc);
            Vector2 pos = new Vector2(0,0);

            if (explosion)
                vel *= -1;
            p.activate(this.GetPosition() + pos, vel, acc, RXRandom.Bool() ? 180.0f : 0);
            this.container.AddChild(p);
        }
    }
}
