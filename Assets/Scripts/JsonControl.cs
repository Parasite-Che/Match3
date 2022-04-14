using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonControl : MonoBehaviour
{
    


}

[System.Serializable]
public class Level
{
    public short _id;
    public byte[,] field;
    public int[,] goals;
    public int countOfMoves;
    public int[,] awards;
}
