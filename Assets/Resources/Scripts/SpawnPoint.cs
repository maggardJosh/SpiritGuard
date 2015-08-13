using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

	public class SpawnPoint
	{
        public Vector2 pos;
        FutileFourDirectionBaseObject.Direction exitDirection;
        public string name;
        public string targetMap;
        public string targetSpawn;

        public SpawnPoint(Vector2 pos, string name, string targetMap, string targetSpawn, FutileFourDirectionBaseObject.Direction exitDirection)
        {
            Init(pos, name, targetMap, targetSpawn, exitDirection);
        }

        public SpawnPoint(float x, float y, string name, string targetMap, string targetSpawn, FutileFourDirectionBaseObject.Direction exitDirection)
        {
            Init(new Vector2(x, y), name, targetMap, targetSpawn, exitDirection);
        }

        private void Init(Vector2 pos, string name, string targetMap, string targetSpawn, FutileFourDirectionBaseObject.Direction exitDirection)
        {
            this.pos = pos;
            this.name = name;
            this.targetMap = targetMap;
            this.targetSpawn = targetSpawn;
            this.exitDirection = exitDirection;
        }

        public const float SPAWN_COLLISION_DIST = 6;
        public bool CheckCollision(Player p)
        {
            if (p.State == Player.PlayerState.JUMP)
                return false;
            return Mathf.Abs(p.x + p.hitBox.x - pos.x) < SPAWN_COLLISION_DIST &&
                Mathf.Abs(p.y + p.hitBox.y - pos.y) < SPAWN_COLLISION_DIST;
                
        }

        public void SpawnPlayer(Player p)
        {
            p.SetPosition(pos);
            p.SetDirection(exitDirection);
            p.PlayAnim(true);
            p.xVel = 0;
            p.yVel = 0;
            Vector2 newPos = pos;
            
            switch(exitDirection)
            {
                case FutileFourDirectionBaseObject.Direction.UP: newPos.y += 16; break;
                case FutileFourDirectionBaseObject.Direction.RIGHT: newPos.x += 16; break;
                case FutileFourDirectionBaseObject.Direction.DOWN: newPos.y -= 16; break;
                case FutileFourDirectionBaseObject.Direction.LEFT: newPos.x -= 16; break;
            }
           Go.to(p, 1.0f, new TweenConfig().floatProp("x", newPos.x).floatProp("y", newPos.y));
        }
	}

