using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "GUI/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Header("Dialogue Lines")]
    public List<string> Lines = new();
   
    [Space(20)]
    
    [Header("Dialogue Options\n----------------------------------\nWhere not needed leave blanc space ")]
   
    [Space(10)]

    public List<string> FirstOption = new();
    public List<string> SecondOption = new();
    public List<string> ThirdOption = new();

    [Space(10)]

    public List<string> FirstResult = new();
    public List<string> SecondResult = new();
    public List<string> ThirdResult = new();

    [Space(10)]

    public bool OneTime = false;

    public void OnValidate()
    {
        while (FirstOption.Count < Lines.Count)
            FirstOption.Add("");
        while (SecondOption.Count < Lines.Count)
            SecondOption.Add("");
        while (ThirdOption.Count < Lines.Count)
            ThirdOption.Add("");

        while (FirstResult.Count < Lines.Count)
            FirstResult.Add("");
        while (SecondResult.Count < Lines.Count)
            SecondResult.Add("");
        while (ThirdResult.Count < Lines.Count)
            ThirdResult.Add("");
    }
}
