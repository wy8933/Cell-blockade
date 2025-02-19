using System;
using System.Collections.Generic;

[Serializable]
public class SkillTreeGraphSaveData
{
    public List<SkillNodeData> nodes;
    public List<SkillConnectionData> connections;
}

[Serializable]
public class SkillNodeData
{
    public string GUID;          
    public string title;         
    public float posX;           
    public float posY;           
    public string buffDataPath;  
    public int cost;             
    public bool isUnlocked;
    public List<string> prerequisiteIDs = new List<string>();
}

[Serializable]
public class SkillConnectionData
{
    public string outputNodeGUID; 
    public string inputNodeGUID;  
}
