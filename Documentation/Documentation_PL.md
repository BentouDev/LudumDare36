# Tank in dungeon #
| *Autorzy* 	| Jakub Bentkowski 	| Łukasz Pyrzyk 	| Michał Wiśniowski 	| Krzysztof Smosna 	|
|---------	|------------------	|---------------	|-------------------	|-----------------	|

# 1. Opis
Tank in dungeon jest grą akcji czasu rzeczywistego z trójwymiarową grafiką. Pomimo
trójwymiarowej grafiki rozgrywka toczy się w dwóch wymiarach przestrzennych.
Gra przeznaczona jest dla pojedynczego gracza, który rywalizować będzie ze sztuczną inteligencją.
Podczas gry gracz pokonuje proceduralnie generowane światy, pokonuje znajdujących się w nim
przeciwników, za co otrzymuje punkty oraz zdobywa bonusy ułatwiające mu rozgrywkę. Zadaniem gracza
jest pokonanie określonej ilości światów.

Gra posiada dwa tryby rozgrywki:
- Time attack - liczy się jak najlepszy czas
- Score attack - liczy się jak najwyższa ilość punktów

W grze zaimplementowane jest wysyłanie wyników na serwer i globalny ranking najlepszych graczy.

Gra stworzona została przy użyciu silnika Unity3D, kod gry napisany został w języku C#, 
kod serwera zaś w języku Go. Do wykonania modeli 3D użyty został program Blender, 
do tekstur zaś Gimp.

# 2. Przestrzeń i czas gry

Rozgrywka odbywa się w dwóch wymiarach przestrzennych oraz czasie.

## Czas
Czas gry jest ciągły. Gra nie posiada limitu czasowego.

## Przestrzeń
Przestrzeń gry składa się z dwóch warstw : 

### 2.1. Świat
Warstwą nadrzędną jest ``świat``, składający się ze skończonej ilości pokoi,
umieszczonych na dwuwymiarowej, ciągłej planszy, o minimalnym rozmiarze 5 na 5 komórek.
Dana komórka może być pusta, bądź zawierać instancję pokoju.

- Liczebność - w danej chwili ``1``, w ciągu gry ilość światów będzie zależała od trybu gry. 
    - W trybie ``Time attack`` gracz będzie pokonywał od ``1`` do ``5`` światów.
    - W trybie ``Score`` gracz będzie pokonywał od ``1`` do ``N`` światów.
- Atrybuty
    - rozmiar planszy w wymiarze ``x``, większe bądź równe ``5``
    - rozmiar planszy w wymiarze ``y``, większe bądź równe ``5``
    - pokój początkowy
    - pokój końcowy
- Stany
    - aktywny
    - zaliczony
- Kompozycja
    - pokoje - dokładna ilość wyznaczana jest za pomocą algorytmu generującego świat,
     opisanego w sekcji ``8.``
    
### 2.2 Pokój
Warstwą podrzędną jest ``pokój``. To w nim toczy się faktyczna rozgrywka. Pokój reprezentowany jest przez
model 3D, składający się ze czterech ścian oraz podłogi. Aktorzy występują jedynie w tej warstwie przestrzeni.
Dodatkowo, w niektórych pokojach znajdują się elementy dekoracyjne, takie jak kolumny, czy zagłębienia.
Ściany pokoju wyznaczają jednocześnie granicę rozgrywki. Żaden z aktorów nie może znajdować się poza granicami pokoju.

- Liczebność - w danej chwili ``1``, ilość istniejących na raz w świecie pokoi 
    różni się zależnie od postępów w grze
- Atrybuty
    - pozycja ``x`` i ``y`` w świecie (ciągła)
    - rozmiar w wymiarze ``x`` oraz ``y``, wyznaczający granice pokoju
- Stany
    - nieodwiedzony
    - aktywny
    - odwiedzony
- Asocjacje / Kompozycja (?)
    - postać gracza - od ``0`` do ``1``
    - przeciwnicy - od ``0`` do ``N``
    - drzwi - od ``1`` do ``4``
    - przedmiot bonusowy - od ``0`` do ``1``

## Zasady Globalne

- aktor który utraci wszystkie punkty życia usuwany jest z planszy
- początkowo wszystkie drzwi w pokoju są zamknięte
- żaden z aktorów nie może istnieć poza granicami pokoju
- drzwi mogą istnieć tylko na z góry wyznaczonych pozycjach w pokoju
- wszystkie drzwi w danym pokoju pozostają zamknięte dopóki żyje 
przynajmniej ``1`` przeciwnik
- wszystkie drzwi nie posiadające klucza otwierają się w momencie śmierci
    ostatniego przeciwnika bądź w przypadku ich braku

### Stan początkowy gry
Gra rozpoczyna się w tzw. pokoju startowym, w którym początkowo znajduje się postać gracza.
Gracz na starcie otrzymuje ``6`` punktów życia i nie posiada żadnych bonusów.

### Rozgrywka
- Przemieszczanie się pomiędzy pokojami odbywa się za pomocą ``drzwi``
- Każdy pokój ma z góry wyznaczone ``4`` pozycje w których potencjale drzwi mogą się znajdować.
Są one odpowiednio w północnej, wchodniej, zachodniej oraz południowej częsci pokoju.
- Drzwi  prowadzą do pokoju znajdującego się w sąsiadującej z odpowiedniej strony komórce. 
W przypadku, gdy komórka jest pusta, bądź pokój znajduje się na krawędzi świata, drzwi nie są
umieszczane.
- Gdy postać gracza spełnia określone zasadami wymagania oraz zbliży się dostatecznie do ``drzwi``
zostanie przeniesiona do sąsiadującego z danej strony pokoju.

### Śmierć gracza
W przypadku otrzymania ilości obrażeń przekraczającej obecną wartość punktów życia postać gracza 
jest usuwana z obecnego pokoju co kończy rozgrywkę ze skutkiem zalieżnym od danego trybu gry.

### Koniec rozgrywki
Zasady dotyczące zwycięstwa bądź przegranej w grze zależą od danego trybu gry: 

## Tryby gry
### Time attack
- Celem trybu jest pokonanie ``5`` światów w jak najkrótszym czasie.

- Gra kończy się na jeden z dwóch sposobów :
  - ``przegraną``, w przypadku śmierci postaci gracza
  - ``zwycięstwem``, w przypadku dotarcia przez postać gracza do portalu
  znajdującego się w ``5`` świecie

- W przypadku zwycięstwa podliczany jest czas jaki uzyskał gracz.
- Za lepszy wynik uważa się jak najniższy czas przejścia gry.

### Score attack
- Gra kończy się jedynie w przypadku śmierci gracza.
- Zapisane zostają zdobyte przez gracza punkty. 
- Za lepszy wynik uważa się jak najwyższą zdobytą ilość punktów.

# 3. Aktorzy

Aktorzy są to obiekty będące w stanie podejmować decyzje 
oraz poprzez swoje akcje wpływać na świat gry.
Aktorzy istnieją jedynie w podrzędnej warstwie przestrzeni gry, ``Pokoju``.
Każdy aktor reprezentowany jest przez model 3D, oraz niewidzialny kształt, 
z którym liczone są kolizje.

W grze wyróżniamy dwa rodzaje aktorów :

### 3.1. Postać gracza
- Liczebność - od ``0`` do ``1`` na pokój
- Atrybuty
    - pozycja ``x`` i ``y`` w pokoju (dyskretna)
    - punkty życia
    - rodzaj pocisku
    - ilość wystrzeliwanych na raz pocisków
    - częstotliwość strzału
    - prędkość poruszania się
- Aktywności
    - ruch - postać gracza może poruszać się w dowolnym kierunku
    - strzał - postać gracza może strzelać pociskami z daną częstotliwością w dowolnym kierunku
    - kolizja z pociskiem wystrzelonym przez przeciwnika odbiera daną ilość punktów życia
    - gdy ilość punktów życia jest mniejsza bądź równa ``0``, następuje śmierć postaci gracza 
    oraz rozpatrywany jest zależny od obecnego trybu wynik gry
- Ograniczenia
    - postać gracza nie może opuścić granic pokoju

### 3.2. Przeciwnik
- Liczebność - od ``0`` do ``N`` na pokój
- Atrybuty
    - pozycja ``x`` i ``y`` w pokoju (dyskretna)
    - punkty życia
    - rodzaj pocisku
    - ilość wystrzeliwanych na raz pocisków
    - częstotliwość strzału
    - prędkość poruszania się
- Aktywności
    - ruch - przeciwnik porusza się w stronę postaci gracza
    - strzał - przeciwnik z daną częstotliwością wystrzeliwuje w stronę postaci gracza pociski
    - kolizja z pociskiem wystrzelonym przez postać gracza odbiera daną ilość punktów życia
    - gdy ilość punktów życia jest mniejsza bądź równa ``0``, przeciwnik jest usuwany z pokoju

# 4. Obiekty

Obiekty są podmiotami akcji aktorów w świecie gry. Same z siebie nie wykonują żadnych działań,
lecz mogą odpowiadać na akcje podjęte przez aktorów.
Obiekty istnieją jedynie w podrzędnej warstwie przestrzeni gry, ``Pokoju``.

Każdy obiekt reprezentowany jest przez model 3D, oraz niewidzialny kształt, 
z którym liczone są kolizje.

W grze wyróżniamy następujące rodzaje obiektów :

### 4.1. Pocisk
- Liczebność - od ``0`` do ``N`` na pokój
- Atrybuty
    - pozycja ``x`` i ``y`` w pokoju (dyskretna)
    - zadawane obrażenia
    - kierunek poruszania się
    - prędkość poruszania się
    - właściciel pocisku
- Aktywności 
    - pocisk może zostać wystrzelony zarówno przez przeciwnika jak i postać gracza
    - w momencie wystrzału dany aktor jest przypisywany jako właściciel pocisku
    - w przypadku kolizji pocisk ulega usunięciu ze świata gry
- Ograniczenia
    - wystrzelony pocisk porusza się w tylko nadanym kierunku i z nadaną prędkością
    - pocisk ignoruje kolizję z aktorami tego samego typu jak jego właściciel

### 4.2. Drzwi
- Liczebność - od ``1`` do ``4`` na pokój
- Atrybuty
    - pozycja w pokoju (ciągła)
    - klucz, którym są zamknięte
- Stany
    - otwarte
    - zamknięte
    - zamknięte na klucz
- Aktywności
    - zamknięte drzwi mogą zostać otwarte poprzez kolizję postaci gracza z drzwiami
    - postać gracza może przejść przez otwarte drzwi poprzez kolizję z nimi
- Ograniczenia
    - tylko postać gracza może przejść przez drzwi
    - postać gracza nie może przejść przez zakmnięte drzwi
    - jeżeli drzwi są zamknięte przez jakiś klucz, gracz musi go posiadać by mogły się otworzyć

### 4.3. Portal
- Liczebność - od ``0`` do ``1`` na pokój
- Atrybuty
    - pozycja ``x`` i ``y`` w pokoju (ciągła)
- Aktywności
    - przejście przez portal odbywa się poprzez kolizję postaci gracza z portalem
    oraz wyzwala akcję zależną od danego trybu gry i postępów gracza
- Ograniczenia
    - tylko postać gracza może przejść przez portal

### 4.4. Przedmiot bonusowy

Wszystkie przedmioty bonusowe posiadają kilka cech wspólnych : 

- Liczebność - od ``0`` do ``1`` na pokój
- Atrybuty
    - pozycja ``x`` i ``y`` w pokoju (ciągła)
- Aktywności
    - podniesienie odbywa się poprzez kolizję postaci gracza z przedmiotem
- Ograniczenia
    - mogą być podniesione tylko przez postać gracza

W grze wyróżniamy kilka rodzai przedmiotów bonusowych.
Każdy z tych rodzai posiada kilka unikalnych cech :

#### 4.4.1. serca
- atrybuty:
    - przywracana ilość punktów życia
- aktywności
    - podniesione dodają postaci gracza daną ilość punktów życia
    
#### 4.4.2. klucze
- atrybuty:
    - klucz
- aktywności
    - podniesiony dodaje postaci gracza swój klucz
- ograniczenia
    - klucz traci się po przejściu przez portal do innego świata

#### 4.4.3. mapa
- aktywności
    - podniesiona odkrywa graczowi mapę podziemi, oznaczając nieodwiedzone pokoje
- ograniczenia
    - efekt traci się po przejściu przez portal do innego świata

#### 4.4.3. kompas
- aktywności
    - podniesiona pokazuje na mapie gdzie znajdują się klucze
- ograniczenia
    - efekt traci się po przejściu przez portal do innego świata

### 4.5 Element dekoracyjny

W niektórych pokojach znajdują się także elementy dekoracyjne. Reprezentowane są one przez
jakiś model 3D, np. kolumnę czy zagłębienie. Z każdym takim elementem możliwa jest kolizja,
powoduje ona jedynie niemożność ruchu danego aktora, czy też obiektu.

# 5. Warstwa składu danych

Diagram klas

![Diagram Klas](./TankInDungeonClassDiagram.png)

# 6. Interfejs i Interakcje użytkownika
## Interfejs
Po uruchomieniu gry przenosimy się od menu głownego. Sterowanie po menu odbywa się za pomocą strzałek, potwierdzenie zaznaczonej opcji znajduje się pod klawiszem enter.
Poniżej znajduje się schemat menu :

![Schemat Menu](./TankInDungeonMenuScheme.png)

Podczas rozgrywki możemy wydzielić cztery glówne elementy interface'u :
- Pasek punktów życia - pojedynczy punk reprezentowany jest przez połówkę serca, każda para punktów oznacza całe serce
- Mini mapę - na niej gracz ma możliwość zapoznania się z układem odwiedzonych pokoi w świecie. Mapa przedstawia również ikony znajdujących się w niej przedmiotów bonusowych. Czerwonym kafelkiem oznaczony jest pokój końcowy.
- Aktualną ilość punktów (prawy górny róg)
- Aktualny czas gry (prawy dolny róg)

## Interakcje
Poruszanie się postacią odbywa się za pomocą dwóch grup przycisków. Klawisze WSAD odpowiadają za poruszanie się postaci gracza w świecie, natomiast strzał w danym kierunku odbywa się poprzez wciśnięcie odpowienich klawiszy strzałek.
Przytrzymanie danego klawisza powoduje ciągły ruch postaci w danym kierunku, podobnie jak przytrzymanie klawisza strzałek powoduje ciągły ostrzał z odpowiednią częstotliwością.

Gra pozwala również na sterowanie za pomocą pada do gier. W tym przypadku ruch odbywa się poprzez wychylenie lewej gałki analogowej w danym kierunku, strzał zaś poprzez wychylenie prawej.

# 7. Globalny ranking
## 7.1. Baza danych
Gra przechowuje globalny ranking najlepszych wyników w nierelacyjnej (NoSQL) bazie danych ``MongoDB``, która uruchomiona jest na systemie Linux w chmurze ``Microsoft Azure``.
Baza przetrzymuje dwie kolekcje danych - wyniki dla trybu ``Time attack`` i ``Score attack``.

## 7.2. API
Ze względów bezpieczeństwa instancja gry nie komunikuje się bezpośrednio z bazą danych. 
Elementem zapewniającym komunikację jest API opartę o styl ``REST``. System został napisany w języku ``Go``, potocznie zwalynm ``Golang``. Jest to nowoczesny język programowania, którego celem jest dostarczenie wysokiej wydajności oraz natywnej kompilacji.
Dostępne są dwa rodzaje endpointów - ``POST`` i ``GET`` dla każdej z kolekcji wyników.

```ini
http://domain.com/timeresults/N # GET, gdzie N to liczba najlepszych wyników
http://domain.com/timeresults # POST, gdzie ciało to struktura danych w formacie JSON 
```

```ini
http://domain.com/scoreresults/N # GET, gdzie N to liczba najlepszych wyników
http://domain.com/scoreresults # POST, gdzie ciało to struktura danych w formacie JSON 
```

Dane przesyłane są w formacie JSON
```json
{
    "PlayerName": "Adam",
    "Score": 100,
    "Time": 71
}
```

## 7.3. Deployment
W celu zapewnienia ``Continuous Integration`` (CI), API jak i baza danych jest uruchomiona na Linuxie poprzez system konteneryzacji ``Docker``. Pozwala on na odizolowanie od siebie usług, swobodne wdrażanie nowej wersji oraz monitorowanie zużycia zasobów.

Obraz MongoDB jest dostarczony przez twórców bazy danych. Aby uruchomić kontener z MongoDB, należy wykonać polecenie
```bash
docker run -d mongo
```

API dla ``Tank in dungeon`` jest również dostępne dla Dockera,
```bash
docker run -d lukaszpyrzyk/tankindungeonapi
``` 

# 8. Generowanie proceduralne plansz

W grze zaimplementowany jest algorytm generujący proceduralne planszę.

Przyjmuje on kilka parametrów wejściowych : 
- Seed - liczba ustawiana jako seed w generatorze liczb pseudolosowych
- CorridorLength - maksymalna długość wygenerowanego korytarza w pokojach
- AllowLoops - czy algorytm ma pozwolić na powstawanie pętli korytarzy (kilka alternatywnych ścieżek do celu)
- MinWidth - minimalna szerokość świata w pokojach
- MaxWidth - maksymalna szerokość świata w pokojach
- MinHeight - minimalna wysokość świata w pokojach
- MaxHeight - maksymalna wysokość świata w pokojach
- MinMapFullfill - minimalny procent wypełnienia świata pokojami
- MaxMapFullfill - maksymalny procent wypełnienia świata pokojami

Algorytm jest deterministyczny, więc dla takich samych parametrów wejściowych wygeneruje identyczny świat.

## Sposób działania
Na samym początku ustawiany jest seed generatora liczb pseudolosowych. Następnie losowane są następujące wartości:
- Szerokość świata w pokojach
- Wysokość świata w pokojach
- Procentowe wypełnienie świata pokojami
- Koordynaty pierwszego pokoju

```
WorldHeight = Mathf.CeilToInt(Random.Range(MinHeight, MaxHeight));
WorldWidth = Mathf.CeilToInt(Random.Range(MinWidth, MaxWidth));
WorldFill = Random.Range(MinMapFullfill, MaxMapFullfill);

var FirstRoomPosX = Random.Range(0, WorldWidth - 1);
var FirstRoomPosY = Random.Range(0, WorldHeight - 1);
```

Kolejnym krokiem jest ustalenie finalnej ilości pokojów w świecie
```
RoomCount = Mathf.CeilToInt((WorldHeight * WorldWidth) * WorldFill);
```

Następnie powstaje dwuwymiarowa tablica reprezentująca świat, 
domyślnie wypełniona pustymi komórkami. Świat budowany jest w sposób rekurencyjny,
zaczynając od wylosowanych koordynatów pierwszego pokoju.
Pojedynczy krok rekurencji wygląda następująco :

- sprawdź czy dany koordynat odpowiada pustej komórce, jeżeli tak, to stwórz w tym miejscu pokój i dodaj go do listy
- zdekrementuj obecną długość korytarza
- sprawdź ilość wolnych komórek wookół pokoju (będzie w przedziale od 0 do 4)
- dopóki ilość pokoi w liście jest mniejsza od docelowej, ilość wolnych pokoi większa od zera oraz obecna długość korytarza jest dłuższa od 0 :
    - wylosuj liczbę w przedziale od 0 do ilości wolnych komórek
    - wylicz koordynat dla wylosowanej komórki
    - wykonaj kolejny krok rekurencji dla koordynatów wylosowanej komórki
    - jeśli pozwalamy na tworzenie pętli
        - przelicz okoliczne wolne pokoje ponownie - nasz pokój-dziecko mógł stworzyć swoje własne dzieci, które zajęły już wolne wcześniej pola
- zwróć stworzony pokój
