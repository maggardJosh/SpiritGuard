using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SoulPickup : FutilePlatformerBaseObject
{
    private FSprite sprite;
    public enum SoulType
    {
        JUMP,
        SWORD,
        BOW
    }
    SoulType type;
    public SoulType Type { get { return type; } }
    public SoulPickup(SoulType type, World world, float x, float y)
        : base(new RXRect(0, -7, 7, 12), world)
    {
        this.SetPosition(x, y - 3);
        Go.to(this, 2.0f, new TweenConfig().floatProp("y", 6, true).setEaseType(EaseType.QuadInOut).setIterations(-1, LoopType.PingPong));
        this.type = type;
        sprite = new FSprite(type.ToString().ToLower() + "Soul");
        this.AddChild(sprite);
    }
    public override void OnFixedUpdate()
    {
        if (RXRandom.Float() < .15f)
            SpawnParticles();
        base.OnFixedUpdate();
    }
    public void HandlePlayerCollision(Player p)
    {
        if (p.isColliding(this))
        {
            FSoundManager.TweenVolume(.3f);
            p.isVisible = false;
            p.xVel = 0;
            p.yVel = 0;
            Go.killAllTweensWithTarget(this);
            FSprite playerPickup = new FSprite("player_13");
            C.getCameraInstance().AddChild(playerPickup);
            Vector2 playerRelativePosition = p.GetPosition() + Futile.stage.GetPosition() - Vector2.up * 16f;
            playerPickup.SetPosition(playerRelativePosition);
            this.SetPosition(this.GetPosition() + Futile.stage.GetPosition() - Vector2.up * 16f);
            world.removeObject(this);
            C.getCameraInstance().AddChild(this);
            world.forceWaitLoad = true;
            RXDebug.Log(this.GetPosition(), playerPickup.GetPosition());
            this.MoveToFront();
            FLabel label = new FLabel(C.largeFontName, type.ToString().ToUpper() + " SPIRIT");
            C.getCameraInstance().AddChild(label);
            label.y = Futile.screen.halfHeight - label.textRect.height / 2f - 10;
            label.x = Futile.screen.halfWidth + label.textRect.width / 2f + 10;
            world.ShowLoading(() =>
            {
                Go.to(playerPickup, 2.0f, new TweenConfig().floatProp("x", 0).floatProp("y", -15).setEaseType(EaseType.QuadOut));
                Go.to(this, 2.0f, new TweenConfig().floatProp("x", 0).floatProp("y", 15).setEaseType(EaseType.QuadOut));
                Go.to(label, 1.5f, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BackOut).setDelay(1.5f).onComplete(() =>
                {
                    Go.to(this, .01f, new TweenConfig().floatProp("x", 1, true).onComplete(() =>
                    {
                        Go.to(this, .01f, new TweenConfig().floatProp("x", -2, true).setIterations(100, LoopType.PingPong).onComplete(() =>
                        {
                            SpawnParticles(30);
                            this.x -= 1;
                            sprite.SetElementByName(type.ToString().ToLower() + "_soul");
                            Go.to(label, 1.5f, new TweenConfig().floatProp("x", -Futile.screen.halfWidth - label.textRect.width / 2f - 10).setEaseType(EaseType.BackIn).setDelay(2.0f).onComplete(() =>
                            {
                                label.RemoveFromContainer();
                                Go.to(playerPickup, 1.0f, new TweenConfig().floatProp("x", playerRelativePosition.x).floatProp("y", playerRelativePosition.y).setEaseType(EaseType.QuadInOut).onComplete(() =>
                                {
                                    FSoundManager.TweenVolume(1.0f); 
                                    p.PickupSoul(this);
                                    world.HideLoading(() => { p.isVisible = true; playerPickup.RemoveFromContainer(); this.RemoveFromContainer(); });
                                }));
                                Vector2 powerupPos;
                                switch(type)
                                {
                                    case SoulType.JUMP:
                                        powerupPos = world.ui.slotAPos;
                                        break;
                                    default:
                                        powerupPos = world.ui.slotBPos;
                                        break;
                                }
                                Go.to(this, 1.0f, new TweenConfig().floatProp("x", powerupPos.x).floatProp("y", powerupPos.y).setEaseType(EaseType.QuadInOut));
                            }));
                        }));
                    }));
                }));
            });
        }
    }

    private void SpawnParticles(int numParticles = 1)
    {
        for (int i = 0; i < numParticles; i++)
        {
            Particle.ParticleOne p = Particle.ParticleOne.getParticle();
            Vector2 vel = new Vector2(RXRandom.Float() * 30 - 15f, RXRandom.Float() * 20);
            Vector2 acc = new Vector2(RXRandom.Float() * 5, RXRandom.Float() * 10);

            p.activate(this.GetPosition() + new Vector2(RXRandom.Float() * 16 - 8f, RXRandom.Float() * 13 - 10), vel, acc, RXRandom.Bool() ? 180.0f : 0);
            this.container.AddChild(p);
        }
    }
}

