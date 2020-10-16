using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//klasa letter przechowuje informacje o każdej literze, a także odpowiada za poruszanie się liter
public class Letter : MonoBehaviour
{
    [Header("definiowanie reczne w panelu inspector")] public float timeDuration = 0.5f; // czas trwania animacji
    public string easingCurve = Easing.InOut; //funkcja wygładzająca (klasa Easing jest zdefiniowana w klasie Utils)
    [Header("definiowanie dynamiczne")]
    public TextMesh tMesh; //do prezentowania znaku
    public Renderer tRend;
    public MeshRenderer mRend;

    public bool big = false;//czy litera jest duża

    //pola do interpolacji ruchu litery
    public List<Vector3> pts = null;
    public float timeStart = -1;

    private char _c;
    private Renderer rend;

    private void Awake()
    {
        tMesh = GetComponentInChildren<TextMesh>();
        tRend = tMesh.GetComponent<MeshRenderer>();
        rend = GetComponent<Renderer>();
        visible = false;
        mRend = GetComponent<MeshRenderer>();
    }
    //właściwość do odczytania lub zapisania pola _c 
    public char c
    {
        get { return (_c); }
        set { _c = value;
            tMesh.text = _c.ToString();
        }
    }
    //odczytanie pola _c jako łańcucha
    public string str
    {
        get { return (_c.ToString());}
        set { _c = value[0]; }
    }
    //ustawia lub pobiera widzialność litery
    public bool visible
    {
        get { return (tRend.enabled); }
        set { tRend.enabled = value; }
    }
    //pobiera lub ustawia kolor prostokąta
    public Color color
    {
        get { return (mRend.materials[0].color); }
        set { mRend.materials[0].color = value; }
    }
    //krzywa bezpiera do przemieszcenia się do nowego połączenia
    public Vector3 pos
    {//tylko do zapisu
        set
        {
            //transform.position = value;
            //nowy punkt pośredni który znajuje się w losowej odległości od punktu pośredniego
            Vector3 mid = (transform.position + value);

            float mag = (transform.position - value).magnitude;
            mid += Random.insideUnitSphere * mag * 0.25f;

            pts = new List<Vector3>() { transform.position, mid, value };

            if (timeStart == -1) timeStart = Time.time;
        }
    }
    //przemieszczenie nattychmiastowe
    public Vector3 posImmediate
    {
        set
        {
            transform.position = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeStart == -1) return;
        //przemieszczanie litery na podstawie krzywej Beziera
        float u = (Time.time - timeStart) / timeDuration;
        u = Mathf.Clamp01(u);
        float u1 = Easing.Ease(u, easingCurve); //wygładzenie 
        Vector3 v = Utils.Bezier(u1, pts);
        transform.position = v;

        //jeśli ruch został zakończony, ustaw wartość -1 aby zatrzymać działanie funkcji update
        if (u == 1) timeStart = -1;
    }
}
