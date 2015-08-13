using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Sign : FContainer
{
    FSprite sign;
    FLabel message;
    private float _mScale = 1;

    public Sign(string message)
    {
        sign = new FSprite("object_sign_01");
        this.AddChild(sign);
        this.message = new FLabel(C.smallFontName, message.Replace("\\n", "\n"));
        this.AddChild(this.message);
        this.message.isVisible = false;
    }
    bool inFrontOf = false;
    public void CheckCollision(Player p)
    {
        if (p.x + p.hitBox.x > this.x - this.sign.width / 2 &&
            p.x + p.hitBox.x < this.x + this.sign.width / 2 &&
            p.y + p.hitBox.y > this.y - this.sign.height && p.y + p.hitBox.y < this.y)
        {
          
            if (!inFrontOf)
            {
                message.y = 0;
                inFrontOf = true;
                message.isVisible = true;
                Go.killAllTweensWithTarget(message);
                Go.to(message, 2.0f, new TweenConfig().floatProp("y", message.textRect.height/2 + 5).setEaseType(EaseType.BackOut));
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

