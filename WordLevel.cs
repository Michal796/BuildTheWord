using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// klasa wykorzystywana do utworzenia nowego poziomu w grze. Klasa nie dziedziczy po klasie MonoBehaviour, przez co nie może zostać
//przypisana do konkretnego obiektu w grze
[System.Serializable]
public class WordLevel
{
    public int levelNum;
    public int longWordIndex; //numer długiego słowa, na którym oparty zostanie konkretny poziom gry
    public string word;
    //słownik ilości danej litery w słowie
    public Dictionary<char, int> charDict;
    //słowa które mogą zostać wygenerowane na podstawie liter w słowniku charDict
    public List<string> subWords;

    //zliczenie kazdej litery w danym słowie w
    static public Dictionary <char, int> MakeCharDict (string w)
    {
        Dictionary<char, int> dict = new Dictionary<char, int>();
        char c;
        for (int i=0; i<w.Length; i++)
        {
            c = w[i];
            if (dict.ContainsKey(c))
            {
                dict[c]++;
            }
            else
            {
                dict.Add(c, 1);
            }
        }
        return (dict);
    }

    //sprawdzenie czy można wygenerować jakieś słowo w level.charDict
    static public bool CheckWordInLevel(string str, WordLevel level)
    {
        Dictionary<char, int> counts = new Dictionary<char, int>();
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            if (level.charDict.ContainsKey(c))
            {
                if (!counts.ContainsKey(c))
                {
                    counts.Add(c, 1);
                }
                else
                {
                    counts[c]++;
                }
                //jeśli w zadanym słowie jest więcej wystąpień danego znaku niż w level.CharDict, nie można stworzyć słowa
                if (counts[c] > level.charDict[c]) 
                {
                    return (false);
                }
            } 
            else
            {
                return (false);
            }
        }
        return (true);
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
