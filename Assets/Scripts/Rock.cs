using System.Collections;
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
    private float distanceToNext = 0f, prevDistanceToNext = 0f;
    #endregion

    #region Linked List Tower
    public LinkedList<Rock> parentTower;
    private Rock getNext()
    {
        if (parentTower == null) return null;
        if (parentTower.Find(this) == null)
        {
            Debug.LogWarning("parentTower.Find(this) == null");
            return null;
        }
        if (parentTower.Find(this).Next == null) return null;
        return parentTower.Find(this).Next.Value;
    }
    private Rock getPrevious()
    {
        if (parentTower == null) return null;
        if (parentTower.Find(this) == null)
        {
            Debug.LogWarning("parentTower.Find(this) == null");
            return null;
        }
        if (parentTower.Find(this).Previous == null) return null;
        return parentTower.Find(this).Previous.Value;
    }
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
        if (newRock != this && getNext() == null)
        {
            notYetChecked.Add(newRock);
            searchMatch();
        }
    }
    #endregion

    private void Awake()
    {
        notYetChecked = new List<Rock>();
        parentTower = new LinkedList<Rock>();
        parentTower.AddFirst(this);
        RockManager.Chains.Add(parentTower);
        RockManager.ChainToWanderForce[parentTower] = Vector3.down;
        Text usernameText = GetComponentInChildren<Text>();
        wishRb = wishTransform.GetComponent<Rigidbody>();
        giftRb = giftTransform.GetComponent<Rigidbody>();
    }
    private void searchMatch() // 나의 소원(tail) => 남의 베풂(head)
    {
        if (getNext() == null)
        {
            foreach (Rock other in notYetChecked)
            {
                if (other == this) continue;

                if (other.getPrevious() == null && other.giftIndex == this.wishIndex)
                {
                    foreach (var othersChainElements in other.parentTower)
                    {
                        this.parentTower.AddLast(othersChainElements);
                        RockManager.Chains.Remove(othersChainElements.parentTower);
                        othersChainElements.parentTower = this.parentTower;
                        break;
                    }
                    // if (this.parentTower == null)
                    // {
                    //     if (other.parentTower == null)
                    //     {
                    //         this.parentTower = new LinkedList<Rock>();
                    //         this.parentTower.AddFirst(this);
                    //         this.parentTower.AddLast(other);
                    //         other.parentTower = this.parentTower;
                    //         RockManager.Chains.Add(this.parentTower);
                    //     }
                    //     else
                    //     {
                    //         other.parentTower.AddFirst(this);
                    //         this.parentTower = other.parentTower;
                    //     }
                    //     break;
                    // }
                    // else
                    // {
                    //     if (other.parentTower == null)
                    //     {
                    //         this.parentTower.AddLast(other);
                    //         other.parentTower = this.parentTower;
                    //     }
                    //     else
                    //     {
                    //         RockManager.Chains.Remove(other.parentTower);


                    //     }
                    //     break;
                    // }
                }
            }

            notYetChecked.Clear();
        }
    }

    void Update()
    {
        constrain();
        addForce();
        wander();
    }

    #region Physics
    private void addForce()
    {
        foreach (Rock other in RockManager.Rocks)
        {
            if (other == this) continue;

            Vector3 dir = other.giftTransform.position - this.wishTransform.position;
            float dist = dir.magnitude;
            dir.Normalize();

            float gravitationStrength = RockManager.instance.gravitationStrength;

            if (other == getNext())
            {
                prevDistanceToNext = distanceToNext;
                distanceToNext = dist;

                if (distanceToNext > 1f)
                {
                    this.wishRb.AddForce(dir * gravitationStrength, ForceMode.Force);
                    getNext().giftRb.AddForce(-dir * gravitationStrength, ForceMode.Force);
                }
                else if (prevDistanceToNext <= 1f)
                {
                    this.wishRb.velocity = Vector3.zero;
                }
            }
            else
            {
                if (dist < 5f)
                {
                    this.wishRb.AddForce(-dir * gravitationStrength / (0.1f + dist), ForceMode.Force);
                    other.giftRb.AddForce(dir * gravitationStrength / (0.1f + dist), ForceMode.Force);
                }
            }
        }
    }

    private void wander()
    {
        if (parentTower != null)
        {
            this.wishRb.AddForce(RockManager.ChainToWanderForce[parentTower], ForceMode.Force);
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
    #endregion

    private void OnDrawGizmos()
    {
        if (getNext() == null) return;

        Vector3 middle = 0.5f * (wishTransform.position + getNext().giftTransform.position);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wishTransform.position, middle);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(middle, getNext().giftTransform.position);
    }
}