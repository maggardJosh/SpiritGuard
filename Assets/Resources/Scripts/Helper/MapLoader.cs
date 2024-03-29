﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class MapLoader
{

    public static void loadObjects(World world, List<XMLNode> objects)
    {
        foreach (XMLNode node in objects)
        {
            if (!node.attributes.ContainsKey("type"))
                continue;
            switch (node.attributes["type"].ToLower())
            {
                case "spawnpoint":
                    world.addSpawn(parseSpawnPoint(node));
                    break;
                case "sign":
                    world.addObject(parseSign(node, world));
                    break;
                case "enemy":
                    Knight k = parseEnemy(node, world);
                    if (k != null)
                        world.addObject(k);
                    break;
                case "villager":
                    world.addObject(parseVillager(node, world));
                    break;
                case "pushblock":
                    world.addObject(parsePushBlock(node, world));
                    break;
                case "arrowturret":
                    world.addObject(parseArrowTurret(node, world));
                    break;
                case "magicturret":
                    world.addObject(parseMagicTurret(node, world));
                    break;
                case "ghost":

                    Ghost g = parseGhost(node, world);
                    if (g != null)
                        world.addObject(g);
                    break;
                case "soul":
                    SoulPickup s = parseSoul(node, world);
                    if (s != null)
                        world.addObject(s);
                    break;
                case "door":
                    world.addObject(parseDoor(node, world));
                    break;
                case "switch":
                    world.addObject(parseSwitch(node, world));
                    break;
                case "hitswitch":
                    world.addObject(parseHitSwitch(node, world));
                    break;
            }
        }
    }


    private static SpawnPoint parseSpawnPoint(XMLNode node)
    {
        SpawnPoint result;
        string name = "";
        string targetMap = "";
        string targetSpawn = "";
        bool showMapName = false;
        FutileFourDirectionBaseObject.Direction exitDirection = FutileFourDirectionBaseObject.Direction.DOWN;
        foreach (XMLNode property in ((XMLNode)node.children[0]).children)
        {
            switch (property.attributes["name"].ToLower())
            {
                case "name":
                    name = property.attributes["value"].ToLower();
                    break;
                case "targetmap":
                    targetMap = property.attributes["value"].ToLower();
                    break;
                case "targetspawn":
                    targetSpawn = property.attributes["value"].ToLower();
                    break;
                case "exitdirection":
                    switch (property.attributes["value"].ToLower())
                    {
                        case "up":
                            exitDirection = FutileFourDirectionBaseObject.Direction.UP;
                            break;
                        case "right":
                            exitDirection = FutileFourDirectionBaseObject.Direction.RIGHT;
                            break;
                        case "down":
                            exitDirection = FutileFourDirectionBaseObject.Direction.DOWN;
                            break;
                        case "left":
                            exitDirection = FutileFourDirectionBaseObject.Direction.LEFT;
                            break;
                    }
                    break;
                case "showmapname":
                    showMapName = true;
                    break;
            }
        }
        
        result = new SpawnPoint(showMapName, (float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"]) - 8f), name, targetMap, targetSpawn, exitDirection);
        return result;
    }

    private static Sign parseSign(XMLNode node, World world)
    {
        List<string> dialogue = new List<string>();
        if (node.children[0] != null)
        {

            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                switch (property.attributes["name"].ToLower())
                {
                    case "dialogue":
                        dialogue = property.attributes["value"].Split('|').ToList();
                        break;
                }
            }
        }
        Sign result = new Sign(world, dialogue);
        result.SetPosition((float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"]) - 8f));

        return result;
    }

    private static Knight parseEnemy(XMLNode node, World world)
    {
        string name = "";
        if (node.attributes.ContainsKey("name"))
            name = node.attributes["name"];
        if (C.Save.requiredEnemyKills.Contains(name))
            return null;
        Knight result = new Knight(world, name);
        result.SetPosition((float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"]) - 8f));

        return result;
    }

    private static Ghost parseGhost(XMLNode node, World world)
    {
        string name = "";
        if (node.attributes.ContainsKey("name"))
            name = node.attributes["name"];
        if (C.Save.requiredEnemyKills.Contains(name))
            return null;
        Ghost result = new Ghost(world, name);
        result.SetPosition((float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"]) - 8f));

        return result;
    }

    private static Villager parseVillager(XMLNode node, World world)
    {
        List<string> dialogue = new List<string>();
        string villagerType = "A";
        if (node.children.Count > 0)
        {

            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                switch (property.attributes["name"].ToLower())
                {
                    case "dialogue":
                        dialogue = property.attributes["value"].Split('|').ToList();
                        break;
                    case "type":
                        villagerType = property.attributes["value"];
                        break;
                }
            }
        }

        Villager result = new Villager(dialogue, world, villagerType);
        result.SetPosition((float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"]) - 8f));

        return result;
    }
    private static PushBlock parsePushBlock(XMLNode node, World world)
    {

        PushBlock result = new PushBlock(world);
        result.SetPosition((float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"]) - 8f));

        return result;
    }

    private static ArrowTurret parseArrowTurret(XMLNode node, World world)
    {
        float interval = 1;
        float initdelay = 0;
        FutileFourDirectionBaseObject.Direction turretDirection = FutileFourDirectionBaseObject.Direction.DOWN;
        if (node.children[0] != null)
        {

            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                switch (property.attributes["name"].ToLower())
                {
                    case "interval":
                        float.TryParse(property.attributes["value"], out interval);
                        break;
                    case "direction":
                        switch (property.attributes["value"].ToUpper())
                        {
                            case "UP": turretDirection = FutileFourDirectionBaseObject.Direction.UP; break;
                            case "RIGHT": turretDirection = FutileFourDirectionBaseObject.Direction.RIGHT; break;
                            case "DOWN": turretDirection = FutileFourDirectionBaseObject.Direction.DOWN; break;
                            case "LEFT": turretDirection = FutileFourDirectionBaseObject.Direction.LEFT; break;
                        }
                        break;
                    case "initdelay":
                        float.TryParse(property.attributes["value"], out initdelay);
                        break;
                }
            }
        }
        ArrowTurret result = new ArrowTurret(interval, initdelay, world);
        result.SetDirection(turretDirection);
        result.PlayAnim();
        result.SetPosition((float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"]) - 8f));

        return result;
    }

    private static MagicTurret parseMagicTurret(XMLNode node, World world)
    {
        float interval = 2.5f;
        float initdelay = 0;
        int distance = 4;

        if (node.children.Count > 0)
        {

            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                switch (property.attributes["name"].ToLower())
                {
                    case "interval":
                        float.TryParse(property.attributes["value"], out interval);
                        break;

                    case "initdelay":
                        float.TryParse(property.attributes["value"], out initdelay);
                        break;
                    case "distance":
                        int.TryParse(property.attributes["value"], out distance);
                        break;
                }
            }
        }
        MagicTurret result = new MagicTurret(interval, initdelay, distance, world);
        result.SetPosition((float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"]) - 8f));

        return result;
    }

    private static SoulPickup parseSoul(XMLNode node, World world)
    {
        SoulPickup.SoulType type = SoulPickup.SoulType.JUMP;
        if (node.children[0] != null)
        {

            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                switch (property.attributes["name"].ToLower())
                {
                    case "type":
                        switch (property.attributes["value"].ToLower())
                        {
                            case "jump": if (C.Save.canJump) return null; type = SoulPickup.SoulType.JUMP; break;
                            case "sword": if (C.Save.canSword) return null; type = SoulPickup.SoulType.SWORD; break;
                            case "bow": if (C.Save.canBow) return null; type = SoulPickup.SoulType.BOW; break;
                        }
                        break;
                }
            }
        }
        return new SoulPickup(type, world, (float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"]) - 8f));
    }

    private static Door parseDoor(XMLNode node, World world)
    {

        string doorName = "";
        if (node.attributes.ContainsKey("name"))
            doorName = node.attributes["name"];
        return new Door(doorName, world, (float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"]) - 8f));
    }

    private static Switch parseSwitch(XMLNode node, World world)
    {
        string doorName = "";
        if (node.children.Count > 0)
        {
            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                switch (property.attributes["name"].ToLower())
                {
                    case "door":
                        doorName = property.attributes["value"];
                        break;
                }
            }
        }
        string switchName = "";
        if (node.attributes.ContainsKey("name"))
            switchName = node.attributes["name"];
        Switch result = new Switch(doorName, switchName, world);
        result.SetPosition((float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"]) - 8f));
        return result;
    }

    private static HitSwitch parseHitSwitch(XMLNode node, World world)
    {
        string doorName = "";
        if (node.children.Count > 0)
        {
            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                switch (property.attributes["name"].ToLower())
                {
                    case "door":
                        doorName = property.attributes["value"];
                        break;
                }
            }
        }
        string switchName = "";
        if (node.attributes.ContainsKey("name"))
            switchName = node.attributes["name"];
        HitSwitch result = new HitSwitch(doorName, switchName, world);
        result.SetPosition((float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"]) - 8f));
        return result;
    }
}
