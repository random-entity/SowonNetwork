using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rock : MonoBehaviour
{
    private string username;
    private List<string> wishlist;
    private List<string> mercylist;
    private Text wishesText, merciesText, usernameText;

    public void setUsername(string username) {
        this.username = username;
        usernameText.text = this.username;
    }

    private void Awake()
    {
        wishlist = new List<string>();
        mercylist = new List<string>();

        Text[] texts = GetComponentsInChildren<Text>();
        wishesText = texts[0];
        merciesText = texts[1];
        usernameText = texts[2];
    }

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

    private void searchMatch() {
        foreach(string wish in wishlist ) {
            
        }
    }

    private string getWishesString() {
        string wishes = "";
        foreach(string wish in wishlist) {
            wishes += wish + " ";
        }
        return wishes;
    }
    private string getMerciesString() {
        string mercies = "";
        foreach(string mercy in mercylist) {
            mercies += mercy + " ";
        }
        return mercies;
    }

    void Update()
    {
        wishesText.text = getWishesString();
        merciesText.text = getMerciesString();
    }
}