using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

	public class Switch : FutilePlatformerBaseObject
	{
        private FSprite sprite;
        private string doorName;
        private string name;
        bool pressed = false;
        public Switch(string doorName, string name, World world):base(new RXRect(0,0,7,7), world)
        {
            this.blocksOtherObjects = false;
            this.doorName = doorName;
            sprite = new FSprite("Button/button_01");
            this.AddChild(sprite);
            this.name = name;
            if (C.Save.switchesActivated.Contains(this.name))
            {
                pressed = true;
                sprite.SetElementByName("Button/button_02");
            }
        }

        
        public void HandlePlayerCollision(Player p)
        {
            if (pressed)
                return;

            if (p.isColliding(this))
            {
                C.Save.switchesActivated.Add(this.name);
                FSoundManager.PlaySound("switch");
                C.getCameraInstance().shake(.7f, .3f);
                pressed = true;
                sprite.SetElementByName("Button/button_02");
                world.OpenDoor(doorName);
            }

        }
	}

