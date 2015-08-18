using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Sign : FutilePlatformerBaseObject
{
    FSprite sign;
    FLabel message;
    private float _mScale = 1;

    public Sign(World world, string message)
        : base(new RXRect(0, -6, 8, 6), world)
    {
        this.shouldSortByZ = false;
        sign = new FSprite("object_sign_01");
        this.AddChild(sign);
        this.message = new FLabel(C.smallFontName, "Watch Out!");// new FLabel(C.smallFontName, message.Replace("\\n", "\n"));
        this.AddChild(this.message);
        this.message.isVisible = false;
    }
    bool inFrontOf = false;
    public void CheckCollision(Player p)
    {
        if (p.x + p.hitBox.x > this.x - this.sign.width / 2 &&
            p.x + p.hitBox.x < this.x + this.sign.width / 2 &&
            p.y + p.hitBox.y > this.y - 16 && p.y + p.hitBox.y < this.y && p.CurrentDirection == FutileFourDirectionBaseObject.Direction.UP)
        {

            if (!inFrontOf)
            {

                message.y = 0;
                inFrontOf = true;
                message.isVisible = true;
                Go.killAllTweensWithTarget(message);
                Go.to(message, 2.0f, new TweenConfig().floatProp("y", message.textRect.height / 2 + 5).setEaseType(EaseType.BackOut));

            }
            else
            {
                p.hasInteractObject = true;
                if (C.getKeyDown(C.JUMP_KEY))
                    world.ui.dialogue.ShowMessage(new List<string>() { "Hello!", "Welcome to Spirit Guard!", "WASD - Moves" });
            }
        }
        else
        {
            if (inFrontOf)
            {
                inFrontOf = false;
                Go.killAllTweensWithTarget(message);
                Go.to(message, .3f, new TweenConfig().floatProp("y", 0).setEaseType(EaseType.QuadIn).onComplete(() => { message.isVisible = false; }));
            }
        }
    }
}

