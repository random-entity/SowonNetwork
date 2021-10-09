using System.Collections.Generic;
using UnityEngine;

public class RockManager : MonoBehaviour
{
    public static List<Rock> rockList;
    [SerializeField] private Rock rockPrefab;

    private void Awake()
    {
        rockList = new List<Rock>();

        addRock("chu", "dog cat cola", "friend dance job");
        addRock("ku", "cat soccer dance", "cola dog boyfriend");
        addRock("mi", "grade health muscle iphone", "friend girlfriend house");
    }

    public void addRock(string username, string wishes, string mercies)
    {
        Rock newRock = Instantiate(rockPrefab);

        newRock.setUsername(username);
        newRock.addWishData(wishes);
        newRock.addMercyData(mercies);

        rockList.Add(newRock);
    }
}