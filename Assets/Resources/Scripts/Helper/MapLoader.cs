using System;
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
                    world.addSign(parseSign(node));
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
            }
        }

        result = new SpawnPoint((float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"])-  8f), name, targetMap, targetSpawn, exitDirection);
        return result;
    }

     private static Sign parseSign(XMLNode node)
    {
        string message = "Default";
        if (node.children[0] != null)
        {

            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                switch (property.attributes["name"].ToLower())
                {
                    case "message":
                        message = property.attributes["value"];
                        break;
                }
            }
        }
        Sign result= new Sign(message);
        result.SetPosition((float.Parse(node.attributes["x"]) + 8f), -(float.Parse(node.attributes["y"])-  8f));

        return result;
    } 

}
