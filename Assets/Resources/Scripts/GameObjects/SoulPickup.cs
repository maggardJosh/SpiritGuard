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
                    Go.to(label, 1.5f, new TweenConfig().floatProp("x", -Futile.screen.halfWidth - label.textRect.width / 2f - 10).setEaseType(EaseType.BackIn).setDelay(2.0f).onComplete(() =>
                    {
                        label.RemoveFromContainer();
                        Go.to(playerPickup, 1.0f, new TweenConfig().floatProp("x", playerRelativePosition.x).floatProp("y", playerRelativePosition.y).setEaseType(EaseType.QuadInOut).onComplete(() =>
                        {

                            FSoundManager.TweenVolume(1.0f);
                            world.HideLoading(() => { p.isVisible = true; playerPickup.RemoveFromContainer(); });
                        }));
                        Go.to(this, 1.0f, new TweenConfig().floatProp("y", 100, true).setEaseType(EaseType.QuadIn).onComplete(() => { this.RemoveFromContainer(); }));
                    }));
                }));
            });
        }
    }
}

