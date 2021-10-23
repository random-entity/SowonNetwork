using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rock : MonoBehaviour
{
    #region fields
    private string username;
    [SerializeField] private Text usernameText;
    private int wishIndex, giftIndex;
    [SerializeField] private Transform wishTransform, giftTransform, meshTransform; // head = gift, tail = wish
    [SerializeField] private Transform wishEmoji, giftEmoji;
    private Rigidbody wishRb, giftRb;
    private List<Rock> notYetChecked;
    private Rock previous, next;
    private float distanceToNext = 0f, prevDistanceToNext = 0f;
    #endregion

    #region getters and setters
    public void SetUsername(string username)
    {
        this.username = username;
        usernameText.text = this.username;
        gameObject.name = "ROCK_" + this.username;
    }
    #endregion

    #region methods for initializing data
    public void SetWishAndGift(int wishIndex, int giftIndex)
    {
        this.wishIndex = wishIndex;
        this.giftIndex = giftIndex;
        wishEmoji.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", RockManager.instance.emojiTextures[wishIndex]);
        giftEmoji.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", RockManager.instance.emojiTextures[giftIndex]);
    }
    public void SetMeshAndTexture(int meshIndex, int textureIndex)
    {
        meshTransform.GetComponent<MeshFilter>().mesh = RockManager.instance.meshPresets[meshIndex];
        meshTransform.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", RockManager.instance.texturePresets[textureIndex]);
    }
    #endregion

    #region AddRock event handling
    private void OnEnable()
    {
        RockManager.AddRockAlert += OnAddRock;
    }
    private void OnDisable()
    {
        RockManager.AddRockAlert -= OnAddRock;
    }
    private void OnAddRock(Rock newRock)
    {
        if (newRock != this && next == null)
        {
            notYetChecked.Add(newRock);
            searchMatch();
        }
    }
    #endregion

    private void Awake()
    {
        notYetChecked = new List<Rock>();
        previous = null;
        next = null;
        Text usernameText = GetComponentInChildren<Text>();
        wishRb = wishTransform.GetComponent<Rigidbody>();
        giftRb = giftTransform.GetComponent<Rigidbody>();
    }
    private void searchMatch() // 나의 소원(tail) => 남의 베풂(head)
    {
        if (next == null)
        {
            foreach (Rock other in notYetChecked)
            {
                if (other == this) continue;

                if (other.previous == null && other.giftIndex == this.wishIndex)
                {
                    next = other;
                    other.previous = this;
                }
            }
            notYetChecked.Clear();
        }
    }

    void Update()
    {
        constrain();
        addForce();
    }

    private void addForce()
    {
        if (next == null) return;

        Vector3 dir = next.giftTransform.position - this.wishTransform.position;
        prevDistanceToNext = distanceToNext;
        distanceToNext = dir.magnitude;
        dir.Normalize();

        float gravitationStrength = RockManager.instance.gravitationStrength;

        if (distanceToNext > 1f)
        {
            this.wishRb.AddForce(dir * gravitationStrength, ForceMode.Force);
            next.giftRb.AddForce(-dir * gravitationStrength, ForceMode.Force);
        }
        else if (prevDistanceToNext <= 1f)
        {
            this.wishRb.velocity = Vector3.zero;
        }
    }

    private void constrain()
    {
        Vector3 pos = wishTransform.position;
        if (pos.x < EnvironmentSpecs.boundXLeft) wishRb.AddForce(Vector3.right, ForceMode.Impulse);
        if (pos.x > EnvironmentSpecs.boundXRight) wishRb.AddForce(Vector3.left, ForceMode.Impulse);
        if (pos.y < EnvironmentSpecs.boundYBottom) wishRb.AddForce(Vector3.up, ForceMode.Impulse);
        if (pos.y > EnvironmentSpecs.boundYTop) wishRb.AddForce(Vector3.down, ForceMode.Impulse);
        if (pos.z < EnvironmentSpecs.boundZFront) wishRb.AddForce(Vector3.forward, ForceMode.Impulse);
        if (pos.z > EnvironmentSpecs.boundZBack) wishRb.AddForce(Vector3.back, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        if (next == null) return;

        Vector3 middle = 0.5f * (wishTransform.position + next.giftTransform.position);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wishTransform.position, middle);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(middle, next.giftTransform.position);
    }
}