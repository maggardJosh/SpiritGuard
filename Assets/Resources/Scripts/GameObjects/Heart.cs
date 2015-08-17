using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

	public class Heart : FutilePlatformerBaseObject
	{
        FSprite sprite;
        public Heart(World world, Vector2 pos): base(new RXRect(0,0,10,10), world)
        {
            this.SetPosition(pos);
            this.y -= 3;
            Go.to(this, 2.0f, new TweenConfig().floatProp("y", 6, true).setEaseType(EaseType.QuadInOut).setIterations(-1, LoopType.PingPong));
            sprite = new FSprite("heart");
            this.AddChild(sprite);
        }

        public void HandlePlayerCollision(Player p)
        {
            if (p.isColliding(this))
            {
                FSoundManager.PlaySound("heart");
                p.Health++;
                world.removeObject(this);
            }
        }
	}

