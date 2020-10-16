using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameMode
{
    preGame, //przed rozpoczęciem gry
    loading, //przetwarzanie listy słów
    makeLevel, //tworzenie poziomu
    levelPrep, //tworzenie elementów graficznych
    inLevel, //trwa gra
}

//główna klasa gry odpowiada za tworzenie rozgrywki oraz sprawdzanie poprawności wpisywanych słów
public class WordGame : MonoBehaviour
{
    static public WordGame S;

    [Header("definiowanie ręczne w panelu inspector")]
    public GameObject prefabLetter;
    public Rect wordArea = new Rect(-24, 19, 38, 28);
    public float letterSize = 1.5f;
    //words odnosi się do obiektów lettercollection
    public bool showAllWords = true;
    public float bigLetterSize = 4f;
    public Color bigColorDim = new Color(0.8f, 0.8f, 0.8f);
    public Color bigColorSelected = new Color(1f, 0.9f, 0.7f);
    public Vector3 bigLetterCenter = new Vector3(0, -16, 0);
    public Color[] letColPalette; //różne kolory dla słów o różnej długości
  


    [Header("Definiowanie dynamiczne")]
    public GameMode mode = GameMode.preGame;
    public WordLevel currLevel;
    public List<LetterCollection> letCol; //lista wszystkich słów 
    public List<Letter> bigLetters;
    public List<Letter> bigLettersActive;
    public string testWord;
    private string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; //zbiór liter, na których wprowadzenie reaguje gra

    private Transform letterAnchor, bigLetterAnchor;

    public void Restart()
    {
        SceneManager.LoadScene("__WordGame_Scene_0");
    }
    void Awake()
    {
        S = this;
        letterAnchor = new GameObject("LetterAnchor").transform;
        bigLetterAnchor = new GameObject("BigLetterAnchor").transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        mode = GameMode.loading;
        WordList.INIT();
    }
    // funkcja wywoływana gdy nastąpi zakończenie przetwarzania listy słów
    public void WordListParseComplete()
    {
        mode = GameMode.makeLevel;
        currLevel = MakeWordLevel();
    }
    //stworzenie poziomu gry
    public WordLevel MakeWordLevel(int levelNum = -1)
    {
        WordLevel level = new WordLevel();
        //wygeneruj llosowy poziom, jeśli zadano domyślną wartość parametru
        if (levelNum == -1)
        {
            //losowe długie słowo
            level.longWordIndex = Random.Range(0, WordList.LONG_WORD_COUNT);
        }
        else
        {

        }
        level.levelNum = levelNum;
        level.word = WordList.GET_LONG_WORD(level.longWordIndex);
        // sprawdź jakie litery występują w słowie
        level.charDict = WordLevel.MakeCharDict(level.word);
        // poszukaj mniejszych słów, które można zbudować z danego słowa używając współprogramu
        StartCoroutine(FindSubWordsCoroutine(level));
        return (level);
    }

    //wyszukiwanie słów do ułożenia w danym poziomie
    public IEnumerator FindSubWordsCoroutine(WordLevel level)
    {
        level.subWords = new List<string>();
        string str;

        List<string> words = WordList.GET_WORDS();

        //przetwarzanie słów z klasy WORDLIST
        for (int i =0; i<WordList.WORD_COUNT; i++)
        {
            str = words[i];
            //sprawdzenie czy słowo może zostac wygenerowane w tym poziomie
            if(WordLevel.CheckWordInLevel(str, level))
            {
                level.subWords.Add(str);
            }
            if(i%WordList.NUM_TO_PARSE_BEFORE_YIELD == 0)
            {
                yield return null;
            }
        }
        //posortowanie alfabetyczne
        level.subWords.Sort();
        //posortuj słowa według długości
        level.subWords = SortWordsByLength(level.subWords).ToList();
        SubWordSearchComplete();
    }
    //użycie Linq w celu posortowania otrzymanej tablicy według długości
    public static IEnumerable<string> SortWordsByLength(IEnumerable<string> ws)
    {
        ws = ws.OrderBy(s => s.Length);
        return ws;
    }
    //metoda wywoływana gdy nastąpi zakończenie wyszukiwania mniejszych słów
    public void SubWordSearchComplete()
    {
        mode = GameMode.levelPrep;
        Layout();
    }
    //stworzenie rozgrywki
    void Layout()
    {
        //nowe słowa
        letCol = new List<LetterCollection>();

        //zmienne lokalne pomocnicze
        GameObject go;
        Letter lett;
        string word;
        Vector3 pos;
        float left = 0;
        float columnWidth = 3;
        char c;
        Color col;
        LetterCollection letC;

        //ile wierszy z literami zmieści się na ekranie
        int numRows = Mathf.RoundToInt(wordArea.height / letterSize);

        for(int i =0;i<currLevel.subWords.Count; i++)
        {
            letC = new LetterCollection();
            word = currLevel.subWords[i];

            //poszerz kolumne, jeśli słowo jest za długie
            columnWidth = Mathf.Max(columnWidth, word.Length);
            //stwórz obiekt Letter dla każdej litery słowa
            for(int j = 0; j<word.Length; j++)
            {
                c = word[j];
                go = Instantiate<GameObject>(prefabLetter);
                go.transform.SetParent(letterAnchor);
                lett = go.GetComponent<Letter>();
                lett.c = c;

                //położenie litery
                pos = new Vector3(wordArea.x + left + j * letterSize, wordArea.y, 0);
                pos.y -= (i % numRows) * letterSize;

                //początkowe położenie litery
                lett.posImmediate = pos + Vector3.up * (20 + i % numRows); 

                lett.pos = pos;

                lett.timeStart = Time.time + i * 0.05f;
                go.transform.localScale = Vector3.one * letterSize;

                letC.Add(lett);
            }
            if (showAllWords) letC.visible = true;

            //kolor zależny od długości słowa 
            letC.color = letColPalette[word.Length - WordList.WORD_LENGTH_MIN];
            letCol.Add(letC);

            //zacznij nową kolumnę gdy trzeba
            if(i%numRows == numRows - 1)
            {
                left += (columnWidth + 0.5f) * letterSize;
            }
        }
        //utworzenie dużego słowa
        bigLetters = new List<Letter>();
        bigLettersActive = new List<Letter>();

        //utwórz odpowiedni obiekt Letter dla każdej litery słowa
        for(int i = 0; i<currLevel.word.Length; i++)
        {
            c = currLevel.word[i];
            go = Instantiate<GameObject>(prefabLetter);
            go.transform.SetParent(bigLetterAnchor);
            lett = go.GetComponent<Letter>();
            lett.c = c;
            go.transform.localScale = Vector3.one * bigLetterSize;

            //położenie poczatkowe, ponizej dolnej krawedzi ekranu
            pos = new Vector3(0, -100, 0);

            lett.posImmediate = pos;
            lett.pos = pos;
            //większy time Start, aby wielkie litery przemiescily się poźniej
            lett.timeStart = Time.time + currLevel.subWords.Count * 0.05f;
            lett.easingCurve = Easing.Sin + "-0.18"; //wygładzanie typu Bounce

            col = bigColorDim;
            lett.color = col;
            //duże litery są zawsze widoczne
            lett.visible = true;
            lett.big = true;
            bigLetters.Add(lett);

        }

        //przetasuj duże litery
        bigLetters = ShuffleLetters(bigLetters);

        ArrangeBigLetters();

        mode = GameMode.inLevel;
    }

    List<Letter> ShuffleLetters(List<Letter> letts)
    {
        List<Letter> newL = new List<Letter>();
        int ndx;
        while(letts.Count > 0)
        {
            ndx = Random.Range(0, letts.Count);
            newL.Add(letts[ndx]);
            letts.RemoveAt(ndx);
        }
        return (newL);
    }
    //wyświetlenie dużego słowa na ekranie
    void ArrangeBigLetters()
    {
        //zmienne halfWidth umożliwiają rozmieszczenie dużych słów na środku ekranu
        float halfWidth = (bigLetters.Count) / 2f - 0.5f;
        Vector3 pos;
        for(int i=0; i<bigLetters.Count; i++)
        {
            pos = bigLetterCenter;
            pos.x += (i - halfWidth) * bigLetterSize;
            bigLetters[i].pos = pos;
        }

        halfWidth = (bigLettersActive.Count) / 2f - 0.5f;
        for (int i = 0; i < bigLettersActive.Count; i++)
        {
            pos = bigLetterCenter;
            pos.x += (i - halfWidth) * bigLetterSize;
            pos.y += bigLetterSize * 1.25f;
            bigLettersActive[i].pos = pos;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Letter ltr;
        char c;
        switch (mode)
        {
            case GameMode.inLevel:
                //każdy znak wprowadzony przez gracza
                foreach (char cIt in Input.inputString)
                {
                    c = System.Char.ToUpperInvariant(cIt); // zmiana na wielką literę
                    if (upperCase.Contains(c))//jeśli zawiera tę literę
                    {
                        //wyszukaj literę dla tego znaku na liście liter dużego słowa
                        ltr = FindNextLetterByChar(c);
                        if (ltr != null)
                        {
                            //dodanie litery do słowa, które próbuje wpisać gracz
                            testWord += c.ToString();

                            bigLettersActive.Add(ltr);
                            bigLetters.Remove(ltr);
                            ltr.color = bigColorSelected;
                            ArrangeBigLetters(); //zaktualizuj położenie liter
                        }
                    }
                    if (c == '\b')//klawisz backspace 
                    {
                        //usun ostatni znak z biggLettersActive
                        if (bigLettersActive.Count == 0) return;
                        if(testWord.Length > 1)
                        {
                            testWord = testWord.Substring(0, testWord.Length - 1);//usun ostatni znak
                        }
                        else
                        {
                            testWord = "";
                        }
                        ltr = bigLettersActive[bigLettersActive.Count - 1];
                        bigLettersActive.Remove(ltr);
                        bigLetters.Add(ltr);
                        ltr.color = bigColorDim;

                        ArrangeBigLetters();
                    }
                    if(c == '\n' || c == '\r')//enter
                    {
                        CheckWord(); //sprawdzenie czy słowo jest poprawne
                    }
                    if(c == ' ')//spacja
                    {
                        bigLetters = ShuffleLetters(bigLetters);
                        ArrangeBigLetters();
                    }
                }
                break;
        }
    }
    //sprawdza, czy zadana litera zawiera się w dużym słowie
    Letter FindNextLetterByChar(char c)
    {
        foreach (Letter ltr in bigLetters)
        {
            if (ltr.c == c)
            {
                return (ltr);
            }
        }
        return (null);
    }
    //sprawdzenie, czy wpisane słowo jest poprawne
    public void CheckWord()
    {
        string subWord;
        bool foundTestWord = false;

        //lista indeksów słów, które zostały już znalezione
        List<int> containedWords = new List<int>();

        for(int i=0; i < currLevel.subWords.Count; i++)
        {
            //pomiń, jeśli słowo zostało już odgadnięte
            if (letCol[i].found)
            {
                continue;
            }

            subWord = currLevel.subWords[i];
            //porównanie słów
            if (string.Equals(testWord, subWord))
            {
                HighlightWord(i);
                ScoreManager.SCORE(letCol[i], 1);
                foundTestWord = true;
            }
            //znalezienie mniejszych słów
            else if (testWord.Contains(subWord))
            {
                containedWords.Add(i);
            }
        }
        // jeśli znaleziono słowo, podświetl również pomniejsze słowa zawarte w słowie wprowadzonym przez gracza
        if (foundTestWord)
        {
            int numContained = containedWords.Count;
            int ndx;

            for (int i=0; i<containedWords.Count; i++)
            {
                ndx = numContained - i - 1;
                HighlightWord(containedWords[ndx]);
                ScoreManager.SCORE(letCol[containedWords[ndx]], i + 2);//dodanie punktów za ewentualnie odkryte mniejsze słowa
            }
        }
        //wyczyść słowo testowe 
        ClearBigLettersActive();
    }
    void HighlightWord(int ndx)
    {
        //wyświetl zadane słowo
        letCol[ndx].found = true;
        letCol[ndx].color = (letCol[ndx].color + Color.white) / 2f;
        letCol[ndx].visible = true;
    }
    //wyczyść wpisywane słowo
    void ClearBigLettersActive()
    {
        testWord = "";
        foreach(Letter ltr in bigLettersActive)
        {
            bigLetters.Add(ltr);
            ltr.color = bigColorDim;
        }
        bigLettersActive.Clear();
        ArrangeBigLetters();
    }
}
