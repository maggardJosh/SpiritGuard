using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class InteractInd : FAnimatedSprite
{
    bool isShowing = false;
    bool isHiding = false;
    float startX, startY;
    
    public InteractInd(float x, float y) : base("Interact/pressL")
    {
        this.startX = x;
        this.startY = y;
        SetPosition(x, y);
        this.addAnimation(new FAnimation("idle", new int[] { 1, 1,1,1, 2 }, 200, true));
        this.play("idle");
        this.isVisible = false;
    }

    public void Show()
    {
        if (!this.isHiding && ( this.isVisible || this.isShowing))
            return;
        Go.killAllTweensWithTarget(this);
        isHiding = false;
        isShowing = true;
        isVisible = true;
        Go.to(this, .3f, new TweenConfig().floatProp("y", startY + 5).setEaseType(EaseType.QuadOut).onComplete(() => { isShowing = false; }));
    }

    public void Hide()
    {
        if (!this.isShowing && ( !this.isVisible || this.isHiding))
            return;
        Go.killAllTweensWithTarget(this);
        isHiding = true;
        isShowing = false;
        isVisible = true;
        Go.to(this, .1f, new TweenConfig().floatProp("y", startY).setEaseType(EaseType.QuadIn).onComplete(() => { isHiding = false; isVisible = false; }));
    }
}

