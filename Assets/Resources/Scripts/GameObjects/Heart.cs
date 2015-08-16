using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

	public class Heart : FutilePlatformerBaseObject
	{
        FSprite sprite;
        public Heart(World world, Vector2 pos): base(new RXRect(0,0,4,4), world)
        {
            this.SetPosition(pos);
            sprite = new FSprite("heart");
            this.AddChild(sprite);
        }

        public void HandlePlayerCollision(Player p)
        {
            if (p.isColliding(this))
            {
                p.Health++;
                world.removeObject(this);
            }
        }
	}

