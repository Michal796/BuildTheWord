using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// klasa odpowiadająca za przetworzenie listy słów z zewnętrznego słownika. Przetwarzanie słów odbywa się przy wykorzystaniu 
// współprogramu (StartCoroutine())
public class WordList : MonoBehaviour
{
    private static WordList S;
    [Header("definiowanie ręczne w panelu inspecjtor")]
    public TextAsset wordListText;
    public int numToParseBeforeYield = 10000;//po przetworzeniu 10000 słów, współprogram zostanie wstrzymany
    public int wordLengthMin = 3;
    public int wordLengthMax = 7;

    [Header("definiowanie dynamiczne")]
    public int currLine = 0;
    public int totalLines;
    public int longWordCount; // długie słowa zostaną wykorzystane do zbudowania z nich mniejszych słów
    public int wordCount;

    private string[] lines;
    private List<string> longWords;
    private List<string> words;

    private void Awake()
    {
        S = this;
    }
    // Start is called before the first frame update
    void Init()
    {
        lines = wordListText.text.Split('\n'); // oddziela słowa na podstawie znacznika \n
        totalLines = lines.Length;
        StartCoroutine(ParseLines());
    }
    static public void INIT()
    {
        S.Init();
    }
    public IEnumerator ParseLines()
    {
        string word;
        //list długich słów oraz słów poprawnych
        longWords = new List<string>();
        words = new List<string>();

        for(currLine = 0; currLine<totalLines; currLine++)
        {
            word = lines[currLine];

            if (word.Length == wordLengthMax)
            {
                longWords.Add(word);
            }
            if(word.Length >=wordLengthMin && word.Length <= wordLengthMax)
            {
                words.Add(word);
            }
            //wstrzymanie działania współprogramu
            if(currLine % numToParseBeforeYield == 0)
            {
                longWordCount = longWords.Count;
                wordCount = words.Count;
                yield return null; //zwieszenie działania kodu do następnej klatki
                //pozwala na wykonanie pozostałych kodów
            }
        }
        longWordCount = longWords.Count;
        wordCount = words.Count;

        gameObject.SendMessage("WordListParseComplete");
    }
    //statyczne właściwości umożliwiające dostęp do danych
    static public List<string> GET_WORDS()
    {
        return (S.words);
    }
    static public string GET_WORD(int ndx)
    {
        return(S.words[ndx]);
    }
    static public List<string> GET_LONG_WORDS()
    {
        return (S.longWords);
    }
    static public string GET_LONG_WORD(int ndx)
    {
        return (S.longWords[ndx]);
    }
    static public int WORD_COUNT
    {
        get { return S.wordCount; }
    }
    static public int LONG_WORD_COUNT
    {
        get { return S.longWordCount; }
    }
    static public int NUM_TO_PARSE_BEFORE_YIELD
    {
        get { return S.numToParseBeforeYield; }
    }
    static public int WORD_LENGTH_MIN
    {
        get { return S.wordLengthMin; }
    }
    static public int WORD_LENGTH_MAX
    {
        get { return S.wordLengthMax; }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
