using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Particle : FAnimatedSprite
{
    Vector2 vel;
    float rot;
    Vector2 accel;
    public bool isActive = false;
    public bool ignoreTransitioning = false;
    int animRandom = 180;
    int animBaseSpeed = 100;
    private Particle(string elementName)
        : base(elementName)
    {
        this.isVisible = false;
    }

    public void activate(Vector2 pos, Vector2 vel, Vector2 accel, float rot)
    {
        this.ignoreTransitioning = false;
        this.isVisible = true;
        this.isActive = true;
        this.vel = vel;
        this.SetPosition(pos);
        this.rot = rot;
        this.accel = accel;
        this.play("active", true);
        this.currentAnim.delay = animBaseSpeed + (int)(RXRandom.Float() * animRandom);
    }

    public override void Update()
    {
        if (!ignoreTransitioning && C.isTransitioning)
            return;
        this.x += vel.x * Time.deltaTime;
        this.y += vel.y * Time.deltaTime;
        vel += accel * Time.deltaTime;

        this.rotation = rot;
        //this.rotation += rot * Time.deltaTime;

        if (this.IsStopped)
        {
            this.RemoveFromContainer();
            this.isActive = false;
        }

        base.Update();
    }
    public class ParticleOne : Particle
    {

        private static ParticleOne[] particleList;
        const int MAX_PARTICLES = 100;
        public static ParticleOne getParticle()
        {

            if (particleList == null)
                particleList = new ParticleOne[MAX_PARTICLES];
            for (int x = 0; x < particleList.Length; x++)
            {
                if (particleList[x] == null)
                {
                    ParticleOne p = new ParticleOne();
                    particleList[x] = p;
                    return p;
                }
                else if (!particleList[x].isActive)
                {
                    return particleList[x];
                }
            }
            return particleList[RXRandom.Int(MAX_PARTICLES)];

        }

        private ParticleOne()
            : base("particle" + (RXRandom.Int(2) + 1))
        {

            this.animRandom = 180;
            this.animBaseSpeed = 100;
            
            this.addAnimation(new FAnimation("active", new int[] { 1, 2, 3, 4 }, animBaseSpeed + (int)(RXRandom.Float() * animRandom), false));
        }
    }



}

