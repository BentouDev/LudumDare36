# Tank in dungeon #
| *Autorzy* 	| Jakub Bentkowski 	| Łukasz Pyrzyk 	| Michał Wiśniowski 	| Krzysztof Smosna 	|
|---------	|------------------	|---------------	|-------------------	|-----------------	|

# 1. Opis
Tank in dungeon jest grą akcji czasu rzeczywistego z trójwymiarową grafiką. 
Podczas gry gracz pokonuje proceduralnie generowane światy, pokonuje znajdujących się w nim
przeciwników, za co otrzymuje punkty oraz zdobywa bonusy ułatwiające mu rozgrywkę.

Gra posiada dwa tryby rozgrywki:
- Time attack - liczy się jak najlepszy czas
- Score attack - liczy się jak najwyższa ilość punktów

W grze zaimplementowane jest wysyłanie wyników na serwer i globalny ranking najlepszych graczy.

# 2. Przestrzeń gry

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
    - W trybie ``Time attack`` gracz będzie pokonywał od ``1`` do ``10`` światów.
    - W trybie ``Score`` gracz będzie pokonywał od ``1`` do ``N`` światów.
- Atrybuty
    - rozmiar planszy w wymiarze ``x``, większe bądź równe ``5``
    - rozmiar planszy w wymiarze ``y``, większe bądź równe ``5``
    - pokój początkowy
    - pokój końcowy
- Stany
    - aktywny
    - zaliczony
- Asocjacje / Kompozycja (?)
    - pokoje - dokładna ilość wyznaczana jest za pomocą algorytmu generującego świat,
     opisanego w sekcji ``8.``
    
### 2.2 Pokój
Warstwą podrzędną jest ``pokój``. Pokój posiada dwa wymiary, x oraz y określające jego rozmiar
i  wyznaczające granicę rozgrywki. Żaden z aktorów nie może znajdować się poza granicami pokoju.

Aktorzy występują jedynie w tej warstwie przestrzeni.

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
umieszczane wogóle.
- Gdy postać gracza spełnia określone zasadami wymagania oraz zbliży się dostatecznie do ``drzwi``
zostanie przeniesiona do sąsiadującego z danej strony pokoju.

### Śmierć gracza
W przypadku otrzymania ilości obrażeń przekraczającej obecną wartość punktów życia postać gracza 
jest usuwana z obecnego pokoju co kończy rozgrywkę ze skutkiem zalieżnym od danego trybu gry.

### Koniec rozgrywki
Zasady dotyczące zwycięstwa bądź przegranej w grze zależą od danego trybu gry: 

## Tryby gry
### Time attack
- Celem trybu jest pokonanie ``10`` światów w jak najkrótszym czasie.

- Gra kończy się na jeden z dwóch sposobów :
  - ``przegraną``, w przypadku śmierci postaci gracza
  - ``zwycięstwem``, w przypadku dotarcia przez postać gracza do portalu
  znajdującego się w ``10`` świecie

- W przypadku zwycięstwa podliczany jest czas jaki uzyskał gracz.
- Za lepszy wynik uważa się jak najniższy czas.

### Score attack
- Gra kończy się jedynie w przypadku śmierci gracza.
- Zapisane zostają zdobyte przez gracza punkty. 
- Za lepszy wynik uważa się jak najwyższą ilość punktów.

# 3. Aktorzy

Aktorzy są to obiekty będące w stanie podejmować decyzje 
oraz poprzez swoje akcje wpływać na świat gry.
Aktorzy istnieją jedynie w podrzędnej warstwie przestrzeni gry, ``Pokoju``.

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
    - pocisk może zostać wystrzelony przez przeciwnika jak i postać gracza
    - w momencie wystrzału dany aktor jest przypisywany jako właściciel pocisku
    - w przypadku kolizji pocisk ulega unicestwieniu
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

# 5. Warstwa składu danych

> Dodać diagramy klas i tabelek w bazie

# 6. Interfejs użytkownika

> Dodać opis interfejsu

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

> Dodać opis algorytmu