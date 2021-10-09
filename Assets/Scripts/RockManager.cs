using System.Collections.Generic;
using UnityEngine;

public delegate void RockEvent(Rock newRock);

public class RockManager : MonoBehaviour
{
    // private static string emojiPresetsString = "😃 🤢 🤮 🤕 👿 👹 🤡 💩 👻 💀 👽 👾 🤖 🎃 😺 🍗 🍤 🍺 🍿 🧘 🎸 ✈️ 🏝";
    // private static string[] emojiPresets;
    private static string[] wishPresets = { "강아지", "고양이", "맛있는거", "친구", "춤", "취직", "운동", "성적", "아이폰", "애인", "건강", "가족", "천재", "돈", "시력" };

    private static int temp;

    public static RockEvent AddRockAlert;
    public static List<Rock> rockList;
    [SerializeField] private Rock rockPrefab;

    private void Awake()
    {
        rockList = new List<Rock>();
    }

    public void addRock(string username, string wishes, string mercies)
    {
        Rock newRock = Instantiate(rockPrefab, new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f), Quaternion.identity);

        newRock.setUsername(username);
        newRock.addWishData(wishes);
        newRock.addMercyData(mercies);

        rockList.Add(newRock);

        AddRockAlert(newRock);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            temp++;
            addRock("USER" + temp, wishPresets[Random.Range(0, wishPresets.Length)], wishPresets[Random.Range(0, wishPresets.Length)]);
        }
    }
}