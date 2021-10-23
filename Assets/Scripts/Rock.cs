using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rock : MonoBehaviour
{
    #region fields
    private string username;
    private List<string> wishlist, mercylist;
    private Text wishesText, merciesText, usernameText;
    private List<Rock> notYetChecked;
    private List<Rock> matches;
    public Transform head, tail; // head = 베풂, tail = 소원
    [HideInInspector] public Rigidbody headRb, tailRb;
    #endregion

    #region getters and setters
    public void setUsername(string username)
    {
        this.username = username;
        usernameText.text = this.username;
        gameObject.name = "ROCK_" + this.username;
    }
    public List<string> getMercylist()
    {
        return mercylist;
    }
    private string getWishesString()
    {
        string wishes = "";
        foreach (string wish in wishlist)
        {
            wishes += wish + " ";
        }
        return wishes;
    }
    private string getMerciesString()
    {
        string mercies = "";
        foreach (string mercy in mercylist)
        {
            mercies += mercy + " ";
        }
        return mercies;
    }
    #endregion

    #region methods for initializing data
    private void addData(string input, List<string> list)
    {
        string[] splitInput = input.Split(' ');
        foreach (string s in splitInput)
        {
            list.Add(s);
        }
    }
    public void addWishData(string input)
    {
        addData(input, wishlist);
    }
    public void addMercyData(string input)
    {
        addData(input, mercylist);
    }
    public void setMesh(int index)
    {
        head.GetComponent<MeshFilter>().mesh = MeshSelector.instance.meshesHead[index];
        tail.GetComponent<MeshFilter>().mesh = MeshSelector.instance.meshesTail[index];
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
        if (newRock != this)
        {
            notYetChecked.Add(newRock);
            searchMatch();
        }
    }
    #endregion

    private void Awake()
    {
        wishlist = new List<string>();
        mercylist = new List<string>();
        notYetChecked = new List<Rock>();
        matches = new List<Rock>();

        Text[] texts = GetComponentsInChildren<Text>();
        wishesText = texts[0];
        merciesText = texts[1];
        usernameText = texts[2];

        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        headRb = head.GetComponent<Rigidbody>();
        tailRb = tail.GetComponent<Rigidbody>();
    }
    private void Start()
    {
        wishesText.text = getWishesString();
        merciesText.text = getMerciesString();
    }

    private void searchMatch() // 나의 소원(tail) => 남의 베풂(head)
    {
        foreach (string myWish in wishlist)
        {
            foreach (Rock other in notYetChecked)
            {
                foreach (string othersmercy in other.getMercylist())
                {
                    if (othersmercy == myWish)
                    {
                        matches.Add(other);
                    }
                }
            }
        }
        notYetChecked.Clear();
    }

    void Update()
    {
        addForce();
    }

    private void addForce()
    {
        foreach (Rock other in RockManager.rockList)
        {
            if (other == this) continue;

            Vector3 dir = other.head.position - tail.position; // head = 베풂, tail = 소원
            float sqrDist = dir.sqrMagnitude;
            dir = Vector3.Normalize(dir);

            Rigidbody otherHeadRb = other.headRb;

            if (matches.Contains(other))
            {
                tailRb.AddForce(dir, ForceMode.Force);
                otherHeadRb.AddForce(-dir, ForceMode.Force);
            }
            else
            {
                if (sqrDist < 0.25f)
                {
                    tailRb.AddForce(-4f * dir / sqrDist, ForceMode.Force);
                    otherHeadRb.AddForce(4f * dir / sqrDist, ForceMode.Force);
                }
            }
        }
    }

    private void constrain()
    {
        // if(transform.)
    }

    private void OnDrawGizmos()
    {
        foreach (Rock other in matches)
        {
            Vector3 middle = 0.5f * (tail.position + other.head.position);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(tail.position, middle);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(middle, other.head.position);
        }
    }
}