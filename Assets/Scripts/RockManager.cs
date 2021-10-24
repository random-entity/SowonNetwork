using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void RockEvent(Rock newRock);

public class RockManager : MonoSingleton<RockManager>
{
    public Mesh[] meshPresets;
    public Texture[] texturePresets;
    public Texture[] emojiTextures;
    public static event RockEvent AddRockAlert;
    [SerializeField] private Rock rockPrefab;
    public static List<Rock> Rocks;
    public static List<LinkedList<Rock>> Chains;
    public static Dictionary<LinkedList<Rock>, Vector3> ChainToWanderForce;

    private IEnumerator setRandomWanderData()
    {
        foreach (LinkedList<Rock> chain in Chains)
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            Vector3 direction = new Vector3(x, y, 0f).normalized;
            float magnitude = Random.Range(4f, 8f);
            ChainToWanderForce[chain] = direction * magnitude;
        }
        yield return new WaitForSeconds(2f);
        setRandomWanderData();
    }
    public float gravitationStrength = 1.5f;
    private int tempCurrentUserIndex = 0;

    private void Awake()
    {
        Rocks = new List<Rock>();
        Chains = new List<LinkedList<Rock>>();
        ChainToWanderForce = new Dictionary<LinkedList<Rock>, Vector3>();
        StartCoroutine(setRandomWanderData());
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

            Debug.Log("Chains :" + Chains.Count);
        }
    }
}