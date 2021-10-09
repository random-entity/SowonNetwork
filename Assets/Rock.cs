using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private string username;
    private List<string> wishlist;
    private List<string> mercylist;

    private void Awake()
    {
        wishlist = new List<string>();
        mercylist = new List<string>();
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

    void Update()
    {

    }
}
