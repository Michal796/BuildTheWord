using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//klasa odpowiedzialna za zarządzanie punktami, oraz inicjowanie przemieszczających się wartości punktowych
public class ScoreManager : MonoBehaviour
{
    static private ScoreManager S;

    [Header("definiowanie ręczne w panelu inspector")]
    //rozmiar czcionki ulega zmianie podczas przemieszczania się
    public List<float> scoreFontSizes = new List<float> { 36, 64, 64, 1 };
    public Vector3 scoreMidPoint = new Vector3(1, 1, 0);
    public float scoreTravelTime = 3f;
    public float scoreComboDelay = 0.5f;

    private RectTransform rectTrans;

    private void Awake()
    {
        S = this;
        rectTrans = GetComponent<RectTransform>();

    }
    //statyczna metoda pozwalająca na dodanie punktów z dowolnego miejsca
    static public void SCORE(LetterCollection letCol, int combo)
    {
        S.Score(letCol, combo);
    }
    void Score(LetterCollection letCol, int combo) //combo jest numerem słowa w danej sekwencji
    {
        List<Vector2> pts = new List<Vector2>(); //lista punktów do wyznaczenia krzywej beziera obiektu FloatingScore

        //położenie pierwszej litery słowa
        Vector3 pt = letCol.letters[0].transform.position;
        pt = Camera.main.WorldToViewportPoint(pt);//stworzenie punktu w przestrzeni viewport
        //pierwszy punkt Beziera
        pts.Add(pt);
        //drugi punkt
        pts.Add(scoreMidPoint);
        //ostatni punkt (docelowy)
        pts.Add(rectTrans.anchorMax);
        //wartość punktów obiektu FloatingScore
        int value = letCol.letters.Count * combo;
        FloatingScore fs = Scoreboard.S.CreateFloatingScore(value, pts);

        fs.timeDuration = scoreTravelTime;
        //inicjacja ruchu każdej przemieszczającej się wartości punktowej za każde kolejne odkryte podczas pojedynczej próby słowo jest opóźniana, aby wszystkie wartości nie ruszyły w tej samej chwili
        fs.timeStart = Time.time + combo * scoreComboDelay;
        fs.fontSizes = scoreFontSizes;

        fs.easingCurve = Easing.InOut + Easing.InOut; //podwójny efekt wygładzenia
        //tekst przemieszczającej się wartości
        string txt = letCol.letters.Count.ToString();
        if(combo > 1)
        {
            txt += " x " + combo;
        }
        fs.GetComponent<Text>().text = txt;
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
