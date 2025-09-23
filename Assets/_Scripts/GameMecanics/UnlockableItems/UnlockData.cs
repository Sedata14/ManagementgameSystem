using UnityEngine;
using System.Collections.Generic;

public class UnlockData : ScriptableObject
{
    public bool isUnlocked;
    public List<UnlockData> DependenciesRequirement = new List<UnlockData>();
    

}
