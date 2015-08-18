using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

	public class Switch : FutilePlatformerBaseObject
	{
        private FSprite sprite;
        private string doorName;
        bool pressed = false;
        public Switch(string doorName, World world):base(new RXRect(0,0,7,7), world)
        {
            this.blocksOtherObjects = false;
            this.doorName = doorName;
            sprite = new FSprite("Button/button_01");
            this.AddChild(sprite);
        }

        
        public void HandlePlayerCollision(Player p)
        {
            if (pressed)
                return;

            if (p.isColliding(this))
            {
                C.getCameraInstance().shake(.4f, .3f);
                pressed = true;
                sprite.SetElementByName("Button/button_02");
                world.OpenDoor(doorName);
            }

        }
	}

