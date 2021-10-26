using System.Collections.Generic;

[System.Serializable]
public class Save
{
    public List<UserData> userDatas = new List<UserData>();
}

[System.Serializable]
public struct UserData
{
    public string username;
    public int wishIndex;
    public int giftIndex;
}