# BuildTheWord
UWAGA 
Domyślna wartość rozdzielczości dla gry BuildTheWord gry wynosi 1024x768. 

Do realizacji gry wykorzystano listę ponad 75000 anglojęzycznych słów 2of12inf opracowanej przez Alana Bealea'a, która również została przez niego udostępniona.
W grze wykorzystano skrypt Utils, udostępniony przez autora książki "Projektowanie gier przy użyciu środowiska Unity i języka C#" J. G. Bonda, jako narzędzie do wykonania projektu.

Założenia gry: Opracowany program przetwarza wszystkie słowa wspomnianej listy, wybierając te o długości od 3 do 7 znaków. Na potrzeby pojedynczej rundy wylosowane zostaje długie słowo (7 liter). Na podstawie znaków w wylosowanym słowie, program tworzy listę wszystkich słów, które można wytworzyć ze słowa wylosowanego. Zadaniem gracza w pojedynczej rundzie jest odgadnięcie jak największej liczby słów, które można złożyc ze słowa wylosowanego. Za każde odgadnięte słowo przyznawane są punkty. Gdy gracz wprowadzi słowo, które zawiera w sobie inne, pomniejsze słowo, nastąpi odkrycie obu słów (przykładowo, za wpisanie słowa PEAR odblokowane zostaną słowa PEAR oraz EAR). Punkty przyznawane są według schematu A * B, gdzie A jest
liczbą liter w odgadniętym słowie, a B jest numerem kolejnego odgadniętego słowa w pojedynczej próbie. Przykładowo, za wpisanie słowa PEAR gracz otrzyma 4 (litery) * 1 (numer słowa) = 4 pkt, oraz (za słowo EAR) 3 (litery) * 2 (numer słowa) = 6 pkt, w sumie 10 punktów. W ten sposób nagradzane jest wpisywanie dłuższych słów. Gra nie jest wyposażona w mechanikę automatycznego restartu poziomu, lub przejścia do następnego poziomu. Na ekranie widnieje przycisk, którego wciśnięcie w dowolnym momencie spowoduje wylosowanie nowego poziomu.

Sterowanie: sterowanie w grze odbywa się przy użyciu klawiatury. Należy wpisać sprawdzane słowo i zatwierdzić przyciskiem enter.

Skrypty:
- Letter - klasa przechowuje informacje o każdej literze, a także odpowiada za poruszanie się liter na ekranie
- LetterCollection - kolekcja liter (słowo); klasa odpowiada za słowa, będące zbiorem obiektów Letter,
- WordGame - główna klasa gry, odpowiada za tworzenie rozgrywki oraz sprawdzanie poprawności wpisywanych słów
- WordLevel - klasa odpowiada za generowanie poziomów w grze
- WordList - klasa przetwarzająca liste słów z zewnętrznego pliku. Przetwarzanie odbywa się z wykorzystaniem współprogramu (StartCoroutine())
- ScoreManager - klasa odpowiada za zarządzanie punktami oraz inicjację przemieszczających się po ekranie wartości punktowych
- Scoreboard - klasa odpowiedzialna za zarządzaniem wynikiem.
- FloatingScore - klasa odpowiada za przemieszczające się po ekranie wartości punktowe.
- Utils - klasa udostępniona przez autora wyżej wymienionej książki jako narzędzie do opracowania projektu.
