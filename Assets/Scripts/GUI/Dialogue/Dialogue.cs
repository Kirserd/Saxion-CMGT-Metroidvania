using System.Collections.Generic;
using UnityEngine;

public enum DialogueResult
{
    None,
    Demotivation,
    Motivation
}

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

    public List<Dialogue> FirstResult = new();
    public List<Dialogue> SecondResult = new();
    public List<Dialogue> ThirdResult = new();

    [Space(10)]

    public bool OneTime = false;

    public DialogueResult result;
    public bool important;

    public void OnValidate()
    {
        while (FirstOption.Count < Lines.Count)
            FirstOption.Add("");
        while (SecondOption.Count < Lines.Count)
            SecondOption.Add("");
        while (ThirdOption.Count < Lines.Count)
            ThirdOption.Add("");

        while (FirstResult.Count < Lines.Count)
            FirstResult.Add(null);
        while (SecondResult.Count < Lines.Count)
            SecondResult.Add(null);
        while (ThirdResult.Count < Lines.Count)
            ThirdResult.Add(null);
    }
}
