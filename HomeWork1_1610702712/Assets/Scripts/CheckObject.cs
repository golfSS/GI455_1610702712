using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckObject : MonoBehaviour
{
    public Text EnterText;
    public Text ShowText;

    private string Pig,Dog,Crow,Chicken,Cat;
    


    // Start is called before the first frame update
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FindBotton()
    {
        if ((EnterText.text == "Pig") || (EnterText.text == "Dog") || (EnterText.text == "Crow") || (EnterText.text == "Chicken") || (EnterText.text == "Cat"))
        {
           
            ShowText.text = " [ <color=green>"+EnterText.text+"</color> ] is found ";
            
        }
        else
        {

            ShowText.text = " [ <color=red>" + EnterText.text + "</color> ] is found ";
            
        }
    }
}
