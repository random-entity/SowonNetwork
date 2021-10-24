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
    private Material material;
    private List<Rock> notYetChecked;
    private float DistanceToNext = 0f, prevDistanceToNext = 0f;
    #endregion

    #region Linked List Chain
    public LinkedList<Rock> parentChain;
    private bool sinked = false;
    private Rock getNext()
    {
        if (parentChain == null) return null;
        if (parentChain.Find(this) == null)
        {
            Debug.LogWarning("parentTower.Find(this) == null");
            return null;
        }
        if (parentChain.Find(this).Next == null) return null;
        return parentChain.Find(this).Next.Value;
    }
    private Rock getPrevious()
    {
        if (parentChain == null) return null;
        if (parentChain.Find(this) == null)
        {
            Debug.LogWarning("parentTower.Find(this) == null");
            return null;
        }
        if (parentChain.Find(this).Previous == null) return null;
        return parentChain.Find(this).Previous.Value;
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
        parentChain = new LinkedList<Rock>();
        parentChain.AddFirst(this);
        RockManager.Chains.Add(parentChain);
        RockManager.ChainToWanderForce[parentChain] = Vector3.down;
        Text usernameText = GetComponentInChildren<Text>();
        wishRb = wishTransform.GetComponent<Rigidbody>();
        giftRb = giftTransform.GetComponent<Rigidbody>();
        material = GetComponentInChildren<MeshRenderer>().material;
    }
    private void searchMatch() // 나의 소원(tail) => 남의 베풂(head)
    {
        if (sinked) return;

        if (getNext() == null)
        {
            foreach (Rock other in notYetChecked)
            {
                if (other == this) continue;

                if (other.getPrevious() == null && other.giftIndex == this.wishIndex)
                {
                    foreach (var currentChainElements in this.parentChain)
                    {
                        currentChainElements.transform.localScale *= 1.1f;
                    }
                    foreach (var othersChainElements in other.parentChain)
                    {
                        this.parentChain.AddLast(othersChainElements);
                        RockManager.Chains.Remove(othersChainElements.parentChain);
                        othersChainElements.parentChain = this.parentChain;
                    }

                    if (parentChain.Count >= 3)
                    {
                        if (other.wishIndex == this.parentChain.First.Value.giftIndex)
                        {
                            foreach (var chainElements in this.parentChain)
                            {
                                chainElements.sinked = true;
                            }
                        }
                    }
                }
            }

            notYetChecked.Clear();
        }
    }

    void Update()
    {
        constrain();
        addForce();

        if (sinked)
        {
            material.color = Color.yellow;
            if (parentChain.First.Value == this)
            {
                compulseBetweenSinkedRoots();

                material.color = Color.blue;
            }
        }
        else
        {
            wander();

            if (parentChain.First.Value == this)
            {
                material.color = Color.green;
            }
            else if (parentChain.Last.Value == this)
            {
                material.color = Color.red;
            }
            else
            {
                material.color = Color.white;
            }
        }
    }

    #region Physics
    private void attractNext(float strength)
    {
        if (this.getNext() == null) return;
        Rock next = this.getNext();

        Vector3 dir = next.giftTransform.position - this.wishTransform.position;
        float dist = dir.magnitude;
        dir.Normalize();

        strength *= dist; // dist > 1f ? dist : -dist;
        next.wishRb.AddForce(-dir * strength, ForceMode.Force);

        prevDistanceToNext = DistanceToNext;
        DistanceToNext = dist;

        if (DistanceToNext > 0.5f && prevDistanceToNext <= 0.5f)
        {
            this.wishRb.velocity = Vector3.zero;
        }
    }

    private void compulse(Rock other, float strength)
    {
        if (other == this) return;

        Vector3 dir = other.giftTransform.position - this.wishTransform.position;
        float sqrDist = dir.sqrMagnitude;
        dir.Normalize();

        float compulseThreshold = parentChain.Contains(other) ? 1.5f : 8f;

        if (sqrDist < compulseThreshold)
        {
            this.wishRb.AddForce(-dir * strength / (0.1f + sqrDist), ForceMode.Force);
            other.giftRb.AddForce(dir * strength / (0.1f + sqrDist), ForceMode.Force);
        }
    }

    private void compulseBetweenSinkedRoots()
    {
        foreach (Rock other in RockManager.Rocks)
        {
            if (other == this) continue;

            if (other.sinked && other.parentChain.First.Value)
            {
                compulse(other, 5f);
            }
        }
    }

    private void addForce()
    {
        attractNext(3f);

        foreach (Rock other in RockManager.Rocks)
        {
            compulse(other, 1f);
        }

        if (sinked)
        {
            if (parentChain.First.Value == this)
            {
                this.wishRb.AddForce(Vector3.down * 8f, ForceMode.Force);
            }
            else
            {
                this.wishRb.AddForce(Vector3.up * 7f, ForceMode.Force);
            }
        }
    }

    private void wander()
    {
        this.giftRb.AddForce(RockManager.ChainToWanderForce[parentChain], ForceMode.Force);
    }

    private void constrain()
    {
        Vector3 pos = giftTransform.position;
        if (pos.x < EnvironmentSpecs.boundXLeft) wishRb.AddForce(Vector3.right, ForceMode.Impulse);
        if (pos.x > EnvironmentSpecs.boundXRight) wishRb.AddForce(Vector3.left, ForceMode.Impulse);
        if (sinked)
        {
            if (pos.y < EnvironmentSpecs.boundYBottomSinked) giftTransform.position = new Vector3(pos.x, EnvironmentSpecs.boundYBottomSinked, pos.z); // wishRb.AddForce(Vector3.up, ForceMode.Impulse);
        }
        else
        {
            if (pos.y < EnvironmentSpecs.boundYBottom) wishRb.AddForce(Vector3.up, ForceMode.Impulse);
        }
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