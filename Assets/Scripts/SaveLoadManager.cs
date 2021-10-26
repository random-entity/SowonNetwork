using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private static Save CreateSaveGameObject()
    {
        Save save = new Save();
        foreach (Rock rock in RockManager.Rocks)
        {
            UserData data = rock.GetUserData();
            save.userDatas.Add(data);
        }

        return save;
    }

    public static void SaveGame()
    {
        Save save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("Game Saved");
    }

    public static void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            foreach (var userdata in save.userDatas)
            {
                RockManager.instance.AddRock(userdata.username, userdata.wishIndex, userdata.giftIndex);
            }

            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.Log("Save file not found ㅠㅠ");
        }
    }
}