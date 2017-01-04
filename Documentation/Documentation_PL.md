# Tank in dungeon #
| Autorzy 	| Jakub Bentkowski 	| Łukasz Pyrzyk 	| Michał Wiśniowski 	| Krzysztof Smosna 	|
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
Przestrzeń gry składa się z dwóch warstw.

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
    - pokoje - ilość określona wzorem [*jakiśtam wzór z opisem*]
    
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
- Zasady
    - początkowo wszystkie drzwi w pokoju są zamknięte
    - żaden z aktorów nie może istnieć poza granicami pokoju
    - drzwi mogą istnieć tylko na z góry wyznaczonych pozycjach w pokoju
    - wszystkie drzwi pozostają zamknięte dopóki żyje przynajmniej ``1`` przeciwnik
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
Celem trybu jest pokonanie ``10`` światów w jak najkrótszym czasie.

Gra kończy się na jeden z dwóch sposobów :
  - ``przegraną``, w przypadku śmierci postaci gracza
  - ``zwycięstwem``, w przypadku dotarcia przez postać gracza do portalu
  znajdującego się w ``10`` świecie

W przypadku zwycięstwa podliczany jest czas jaki uzyskał gracz.
Za lepszy wynik uważa się jak najniższy czas.

### Score attack
Gra kończy się jedynie w przypadku śmierci gracza. Zapisane zostają zdobyte przez gracza punkty. 
Za lepszy wynik uważa się jak najwyższą ilość punktów.

# 3. Aktorzy

Aktorzy istnieją jedynie w podrzędnej warstwie przestrzeni gry, ``Pokoju``.

### 3.1. Postać gracza
- Liczebność - od ``0`` do ``1`` na pokój
- Atrybuty
    - pozycja ``x`` i ``y`` w pokoju (dyskretna)
    - punkty życia
    - rodzaj pocisku
    - ilość wystrzeliwanych na raz pocisków
    - częstotliwość strzału
    - prędkość poruszania się

### 3.2. Przeciwnik
- Liczebność - od ``0`` do ``N`` na pokój
- Atrybuty
    - pozycja ``x`` i ``y`` w pokoju (dyskretna)
    - punkty życia
    - rodzaj pocisku
    - ilość wystrzeliwanych na raz pocisków
    - częstotliwość strzału
    - prędkość poruszania się
- Zasady 
    - przeciwnik porusza się w stronę postaci gracza
    - przeciwnik z daną częstotliwością wystrzeliwuje w stronę postaci gracza pociski
    - kolizja z pociskiem wystrzelonym przez postać gracza odbiera daną ilość punktów życia
    - gdy ilość punktów życia jest mniejsza bądź równa ``0``, przeciwnik jest usuwany z pokoju

### 3.3. Pocisk
- Liczebność - od ``0`` do ``N`` na pokój
- Atrybuty
    - pozycja ``x`` i ``y`` w pokoju (dyskretna)
    - zadawane obrażenia
    - kierunek poruszania się
    - prędkość poruszania się
    - właściciel pocisku
- Zasady
    - wystrzelony pocisk porusza się w nadanym kierunku z nadaną prędkością
    - w przypadku kolizji pocisk ulega unicestwieniu
    - pocisk ignoruje kolizję z aktorami tego samego typu jak jego właściciel

### 3.4. Drzwi
- Liczebność - od ``1`` do ``4`` na pokój
- Atrybuty
    - pozycja w pokoju (ciągła)
    - klucz, którym są zamknięte
- Stany
    - otwarte
    - zamknięte
    - zamknięte na klucz
- Zasady
    - tylko postać gracza może przejść przez drzwi
    - postać gracza nie może przejść przez zakmnięte drzwi
    - jeżeli drzwi są zamknięte przez jakiś klucz, gracz musi go posiadać by mogły się otworzyć

### 3.5. Portal
- Liczebność - od ``0`` do ``1`` na pokój
- Atrybuty
    - pozycja ``x`` i ``y`` w pokoju (ciągła)
- Zasady
    - tylko postać gracza może przejść przez portal
    - przejście przez portal wyzwala akcję zależną od danego trybu gry i postępów gracza

### 3.6. Przedmiot bonusowy
- Liczebność - od ``0`` do ``1`` na pokój
- Atrybuty
    - pozycja ``x`` i ``y`` w pokoju (ciągła)
- Zasady
    - mogą być podniesione tylko przez postać gracza
    - podniesienie odbywa się poprzez kolizję postaci gracza z przedmiotem

W grze wyróżniamy kilka rodzai przedmiotów bonusowych. 

#### 3.6.1. serca
- atrybuty:
    - przywracana ilość punktów życia
- zasady
    - podniesione dodają postaci gracza daną ilość punktów życia
    
#### 3.6.2. klucze
- atrybuty:
    - klucz
- zasady
    - dodaje postaci gracza swój klucz
    - klucz traci się po przejściu przez portal do innego świata

#### 3.6.3. mapa
- zasady
    - podniesiona odkrywa graczowi mapę podziemi, oznaczając nieodwiedzone pokoje
    - efekt traci się po przejściu przez portal do innego świata

#### 3.6.3. kompas
- zasady
    - podniesiona pokazuje na mapie gdzie znajdują się klucze
    - efekt traci się po przejściu przez portal do innego świata

# 4. Globalny ranking
## 4.1. Baza danych
Gra przechowuje globalny ranking najlepszych wyników z nierelacyjną (NoSQL) bazą danych ``MongoDB``, która uruchomiona jest na systemie Linux w chmurze ``Microsoft Azure``.
Baza przetrzymuje dwie kolekcje danych - wyniki dla trybu ``Time attack`` i ``Score attack``.

## 4.2. API
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

## 4.3. Deployment
W celu zapewnienia ``Continuous Integration`` (CI), API jak i baza danych jest uruchomiona na Linuxie poprzez system konteneryzacji ``Docker``. Pozwala on na odizolowanie od siebie usług, swobodne wdrażanie nowej wersji oraz monitorowanie zużycia zasobów.

Obraz MongoDB jest dostarczony przez twórców bazy danych. Aby uruchomić kontener z MongoDB, należy wykonać polecenie
```bash
docker run -d mongo
```

API dla ``Tank in dungeon`` jest również dostępne dla Dockera,
```bash
docker run -d lukaszpyrzyk/tankindungeonapi
``` 
