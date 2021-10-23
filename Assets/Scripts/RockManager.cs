using System.Collections.Generic;
using UnityEngine;

public delegate void RockEvent(Rock newRock);

public class RockManager : MonoSingleton<RockManager>
{
    public Mesh[] meshPresets;
    public Texture[] texturePresets;
    public Texture[] emojiTextures;
    public static event RockEvent AddRockAlert;
    public static List<Rock> Rocks;
    [SerializeField] private Rock rockPrefab;
    public float gravitationStrength = 2;
    private int tempCurrentUserIndex = 0;

    private void Awake()
    {
        Rocks = new List<Rock>();
    }

    public void AddRock(string username, int wishIndex, int giftIndex)
    {
        Rock newRock = Instantiate(rockPrefab, new Vector3(Random.Range(EnvironmentSpecs.boundXLeft, EnvironmentSpecs.boundXRight), Random.Range(EnvironmentSpecs.boundYBottom, EnvironmentSpecs.boundYTop), 0f), Quaternion.identity);

        newRock.SetUsername(username);

        newRock.SetMeshAndTexture(Random.Range(0, meshPresets.Length), Random.Range(0, texturePresets.Length));
        newRock.SetWishAndGift(wishIndex, giftIndex);

        Rocks.Add(newRock);

        AddRockAlert(newRock);

        tempCurrentUserIndex++;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            AddRock("USER" + tempCurrentUserIndex, Random.Range(0, emojiTextures.Length), Random.Range(0, emojiTextures.Length));
        }
    }
}