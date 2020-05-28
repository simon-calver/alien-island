using System; 
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class WorldInfo
{
    public Vector2 heightOffset = new Vector2(0f, 0f);
    public Vector2 biomeOffset = new Vector2(0f, 0f);
    public Vector2 playerPos = new Vector2(0f, 0f);
    //public List<int> ID = new List<int>();
    //public List<int> Amounts = new List<int>();
    //public int life = 0;
    //public float highScore = 0;
}
