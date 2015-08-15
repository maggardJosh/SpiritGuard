using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UI : FContainer
{
    FSprite background;
    FSprite slotA;
    FSprite slotB;
    FSprite slotASelected;
    FSprite slotBSelected;
    FSprite hearts;

    FSprite selectedSoul;
    public UI()
    {
        background = new FSprite("bg");
        slotA = new FSprite("slot_a");
        slotB = new FSprite("slot_b");
        slotASelected = new FSprite("jump_soul");
        slotBSelected = new FSprite("sword_soul");
        hearts = new FSprite("heart_full");
        this.AddChild(background);
        this.AddChild(slotA);
        this.AddChild(slotB);
        this.AddChild(slotASelected);
        this.AddChild(slotBSelected);
        this.AddChild(hearts);
        background.y = Futile.screen.halfHeight - background.height / 2f;
        slotB.y = background.y;
        slotA.y = background.y;
        slotB.x = -Futile.screen.halfWidth + slotB.width / 2f + 20;
        slotA.x = slotB.x + slotB.width / 2f + slotA.width / 2f + 3;
        slotASelected.SetPosition(slotA.GetPosition());
        slotBSelected.SetPosition(slotB.GetPosition());
        hearts.y = background.y;
        hearts.x = Futile.screen.halfWidth - hearts.width / 2f - 20;
    }

    internal void UpdateSelectedItem(Player.SecondaryItem selectedItem)
    {
        switch (selectedItem)
        {
            case Player.SecondaryItem.SWORD: slotBSelected.SetElementByName("sword_soul"); break;
            case Player.SecondaryItem.BOW: slotBSelected.SetElementByName("bow_soul"); break;
        }
    }

    public void UpdateHealth(int health)
    {
        switch (health)
        {
            case 0: hearts.SetElementByName("heart_container"); break;
            case 1: hearts.SetElementByName("heart_container1"); break;
            case 2: hearts.SetElementByName("heart_container2"); break;
            case 3: hearts.SetElementByName("heart_full"); break;
        }
    }
}

