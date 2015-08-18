using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Sign : FutilePlatformerBaseObject
{
    FSprite sign;
    InteractInd interactIndicator;
    private float _mScale = 1;
    List<string> dialogue;

    public Sign(World world, List<string> dialogue)
        : base(new RXRect(0, -6, 8, 6), world)
    {
        this.dialogue = dialogue;
        this.shouldSortByZ = false;
        sign = new FSprite("object_sign_01");
        this.AddChild(sign);
        interactIndicator = new InteractInd(0, 7);
        this.AddChild(interactIndicator);
        
    }
    bool inFrontOf = false;
    public void CheckCollision(Player p)
    {
        if (p.swordCollision.isColliding(this) && p.CurrentDirection == FutileFourDirectionBaseObject.Direction.UP)
        {

            if (!inFrontOf)
            {
                interactIndicator.Show();
                inFrontOf = true;
            }
            else
            {
                if (C.getKeyDown(C.JUMP_KEY))
                    world.ui.dialogue.ShowMessage(dialogue);
            }
        }
        else
        {
            if (inFrontOf)
            {
                interactIndicator.Hide();
                inFrontOf = false;
            }
        }
    }
}

