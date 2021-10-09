using System.Collections.Generic;
using UnityEngine;

public class RockManager : MonoBehaviour
{
    public static List<Rock> rockList;
    [SerializeField] private Rock rockPrefab;

    private void Awake()
    {
        rockList = new List<Rock>();
    }

    void addRock(string username, string wishes, string mercies)
    {
        Rock newRock = Instantiate(rockPrefab);

        newRock.setUsername(username);
        newRock.addWishData(wishes);
        newRock.addMercyData(mercies);

        rockList.Add(newRock);
    }
}