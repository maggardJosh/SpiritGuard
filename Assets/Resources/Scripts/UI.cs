using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UI : FContainer
{
    FSprite background;
    FSprite slotA;
    FSprite slotB;
    FSprite slotASelected;
    FSprite slotBSelected;
    FSprite hearts;

    public Vector2 slotAPos { get { return slotASelected.GetPosition(); } }
    public Vector2 slotBPos { get { return slotBSelected.GetPosition(); } }

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
        slotASelected.isVisible = false;
        slotBSelected.isVisible = false;
        background.y = Futile.screen.halfHeight - background.height / 2f;
        slotB.y = background.y;
        slotA.y = background.y;
        slotB.x = -Futile.screen.halfWidth + slotB.width / 2f + 20;
        slotA.x = slotB.x + slotB.width / 2f + slotA.width / 2f + 3;
        slotASelected.SetPosition(slotA.GetPosition());
        slotBSelected.SetPosition(slotB.GetPosition());
        slotASelected.x += 1;
        slotBSelected.x += 1;
        hearts.y = background.y;
        hearts.x = Futile.screen.halfWidth - hearts.width / 2f - 20;

    }

    public void UpdateHasJump(bool hasJump)
    {
        slotASelected.isVisible = hasJump;
    }

    internal void UpdateSelectedItem(Player.SecondaryItem selectedItem)
    {
        Go.killAllTweensWithTarget(slotBSelected);
        Go.to(slotBSelected, .3f, new TweenConfig().floatProp("y", Futile.screen.halfHeight + slotBSelected.height).setEaseType(EaseType.QuadOut).onComplete(() =>
        {
            switch (selectedItem)
            {
                case Player.SecondaryItem.NONE:
                    slotBSelected.isVisible = false;
                    break;
                case Player.SecondaryItem.SWORD:
                    slotBSelected.SetElementByName("sword_soul");
                    slotBSelected.isVisible = true;
                    break;
                case Player.SecondaryItem.BOW:
                    slotBSelected.SetElementByName("bow_soul");
                    slotBSelected.isVisible = true;
                    break;
            }
            Go.to(slotBSelected, .5f, new TweenConfig().floatProp("y", Futile.screen.halfHeight - slotBSelected.height / 2f).setEaseType(EaseType.QuadOut));
        }));
    }

    int health = 3;
    public void TakeDamage(int health)
    {
        for (int i = this.health; i > health; i--)
        {
            FSprite heart = new FSprite("heart");
            heart.SetPosition(hearts.GetPosition());
            switch (i)
            {
                case 1: heart.x -= 10; break;
                case 2: break;
                case 3: heart.x += 10; break;
            }
            this.AddChild(heart);
            Go.to(heart, .7f, new TweenConfig().floatProp("y", -15, true).setEaseType(EaseType.BackOut).onComplete(() => { heart.RemoveFromContainer(); }));
        }
        this.health = health;
        setHealthSprite(health);

    }

    public void AddHealth(int health, Vector2 pos)
    {
        for (int i = this.health; i < health; i++)
        {
            FSprite heart = new FSprite("heart");

            heart.SetPosition(pos);
            Vector2 targetPos = hearts.GetPosition();
            switch (i)
            {
                case 0: targetPos.x -= 10; break;
                case 1: break;
                case 2: targetPos.x += 10; break;
            }

            this.AddChild(heart);
            Go.to(heart, .7f, new TweenConfig().floatProp("x", targetPos.x).floatProp("y", targetPos.y).setEaseType(EaseType.QuadOut).onComplete(() => { setHealthSprite(this.health); heart.RemoveFromContainer(); }));
        }
        this.health = health;
    }

    private void setHealthSprite(int health)
    {
        switch (health)
        {
            case 0:
                hearts.SetElementByName("heart_container");
                break;
            case 1:
                hearts.SetElementByName("heart_container1");
                break;
            case 2:
                hearts.SetElementByName("heart_container2");
                break;
            case 3:
                hearts.SetElementByName("heart_full");
                break;
        }
    }
}

