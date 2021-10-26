using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void RockEvent(Rock newRock);

public class RockManager : MonoSingleton<RockManager>
{
    public Mesh[] meshPresets;
    public Texture[] texturePresets;
    public Texture[] emojiTextures;
    public static event RockEvent AddRockAlert;
    [SerializeField] private Rock rockPrefab;
    [SerializeField] private Transform rockParentTransform;
    public static List<Rock> Rocks;
    public static List<LinkedList<Rock>> Chains;
    public static Dictionary<LinkedList<Rock>, Vector3> ChainToWanderForce;
    [SerializeField] private float minWanderForce = 0.05f, maxWanderForce = 0.1f;
    [SerializeField] private Camera closeCam, farCam;
    [SerializeField] private Toggle seeWishesToggle;

    private IEnumerator setRandomWanderData()
    {
        foreach (LinkedList<Rock> chain in Chains)
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            Vector3 direction = new Vector3(x, y, 0f).normalized;
            float magnitude = Random.Range(minWanderForce, maxWanderForce);
            ChainToWanderForce[chain] = direction * magnitude;
        }
        yield return new WaitForSeconds(2f);
        StopAllCoroutines();
        StartCoroutine(setRandomWanderData());
    }
    private int tempCurrentUserIndex = 0;

    private void Awake()
    {
        Rocks = new List<Rock>();
        Chains = new List<LinkedList<Rock>>();
        ChainToWanderForce = new Dictionary<LinkedList<Rock>, Vector3>();
        StartCoroutine(setRandomWanderData());
    }

    private Camera getCamera(bool getCurrentCamTrue_getRestingCamFalse)
    {
        Camera currentCam = DualCameraManager.currentCamera_closeCamTrue_farCamFalse ? closeCam : farCam;
        Camera restingCam = DualCameraManager.currentCamera_closeCamTrue_farCamFalse ? farCam : closeCam;
        return getCurrentCamTrue_getRestingCamFalse ? currentCam : restingCam;
    }

    public Rock AddRock(string username, int wishIndex, int giftIndex)
    {
        Camera currentCam = DualCameraManager.currentCamera_closeCamTrue_farCamFalse ? closeCam : farCam;
        Camera restingCam = DualCameraManager.currentCamera_closeCamTrue_farCamFalse ? farCam : closeCam;

        getCamera(true).GetComponent<DualCameraManager>().ClampPosition(true, true);
        getCamera(false).GetComponent<DualCameraManager>().ClampPosition(true, false);

        float spawnPosX = Mathf.Clamp(currentCam.transform.position.x, EnvironmentSpecs.boundXLeft + 2f, EnvironmentSpecs.boundXRight - 2f);
        float spawnPosY = Mathf.Clamp(currentCam.transform.position.y, EnvironmentSpecs.boundYBottomSinked + 2f, EnvironmentSpecs.boundYTop - 2f);

        Rock newRock = Instantiate(rockPrefab, new Vector3(spawnPosX, spawnPosY, 0f), Quaternion.identity); //new Vector3(Random.Range(EnvironmentSpecs.boundXLeft, EnvironmentSpecs.boundXRight), Random.Range(EnvironmentSpecs.boundYBottom, EnvironmentSpecs.boundYTop), 0f), Quaternion.identity);

        newRock.SetUsername(username);
        newRock.SetMeshAndTexture(Random.Range(0, meshPresets.Length), Random.Range(0, texturePresets.Length));
        newRock.SetWishAndGift(wishIndex, giftIndex);
        newRock.transform.SetParent(rockParentTransform);
        newRock.SetInfoDisplay(seeWishesToggle.isOn);

        Rocks.Add(newRock);

        AddRockAlert(newRock);

        tempCurrentUserIndex++;

        return newRock;
    }

    private void explode()
    {
        Debug.Log("explode");
        Vector3 camWorldZ0 = getCamera(true).transform.position;
        camWorldZ0.z = 0f;

        foreach (Rock rock in Rocks)
        {
            rock.wishRb.AddExplosionForce(4f, camWorldZ0, 10000f, 10f, ForceMode.Acceleration);
        }
    }

    public void SetAllRocksInfoDisplay(bool on)
    {
        foreach (Rock rock in Rocks)
        {
            rock.SetInfoDisplay(on);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            AddRock("USER" + tempCurrentUserIndex, Random.Range(0, emojiTextures.Length), Random.Range(0, emojiTextures.Length));

            Debug.Log("Chains : " + Chains.Count);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            explode();
        }
    }
}