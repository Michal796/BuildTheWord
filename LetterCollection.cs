using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ta klasa odpowiada za kolekcję liter, a więc każde słowo, które należy odgadnąć 
// klasa nie dziedziczy z klasy MonoBehaviour
public class LetterCollection
{
    public string str; //słowo do zaprezentowania 
    public List<Letter> letters = new List<Letter>(); //lista liter w słowie
    public bool found = false; //true jeśli odgadnięte
    
    //właściwość umożliwia ustawienie widoczności każdej litery w słowie (kolekcji)
    public bool visible
    {
        get
        {
            if (letters.Count == 0) return (false);
            return (letters[0].visible);
        }
        set
        {
            foreach(Letter l in letters)
            {
                l.visible = value;
            }
        }
    }
    // ta właściwość umożliwia ustawienie koloru każdej litery w słowie
    public Color color
    {
        get
        {
            if (letters.Count == 0) return (Color.black);
            return (letters[0].color);
        }
        set
        {
            foreach(Letter l in letters)
            {
                l.color = value;
            }
        }
    }
    //dodaj literę do słowa 
    public void Add(Letter l)
    {
        letters.Add(l);
        str += l.c.ToString();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
