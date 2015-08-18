﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Sign : FutilePlatformerBaseObject
{
    FSprite sign;
    InteractInd interactIndicator;
    private float _mScale = 1;

    public Sign(World world, string message)
        : base(new RXRect(0, -6, 8, 6), world)
    {
        this.shouldSortByZ = false;
        sign = new FSprite("object_sign_01");
        this.AddChild(sign);
        interactIndicator = new InteractInd(0, 7);
        this.AddChild(interactIndicator);
        
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
                interactIndicator.Show();
                inFrontOf = true;
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
                interactIndicator.Hide();
                inFrontOf = false;
            }
        }
    }
}

