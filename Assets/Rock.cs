using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Rock : MonoBehaviour
{
    #region fields
    private string username;
    private List<string> wishlist, mercylist;
    private Text wishesText, merciesText, usernameText;
    private List<Rock> notYetChecked;
    private List<Rock> matches;
    public Rigidbody rb;
    #endregion

    #region getters and setters
    public void setUsername(string username)
    {
        this.username = username;
        usernameText.text = this.username;
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

        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        wishesText.text = getWishesString();
        merciesText.text = getMerciesString();
    }

    private void searchMatch()
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

    private void addForce() {
        foreach(Rock other in matches) {
            Vector3 dir = other.transform.position - transform.position;
            dir = Vector3.Normalize(dir);

            rb.AddForce(dir, ForceMode.Force);
            other.rb.AddForce(-dir, ForceMode.Force);
        }
    }
    // private static Vector3 getForce(Vector3 other) {

    // }


    private void OnDrawGizmos() {
        foreach(Rock other in matches) {
            Gizmos.DrawLine(transform.position, other.transform.position);
        }
    }
}