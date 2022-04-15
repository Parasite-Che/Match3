using UnityEngine;
using Newtonsoft.Json;

public class JsonControl : MonoBehaviour
{
    

    
}

[JsonObject]
public class Level 
{
    [JsonProperty]
    public short _id;
    [JsonProperty]
    public byte[,] field;
    [JsonProperty]
    public int[,] goals;
    [JsonProperty]
    public int countOfMoves;
    [JsonProperty]
    public int[,] awards;
}
