# Hex Game

## Užívateľská dokumentácia

Hra HEX je hra dvoch hráčov, ktorí sa snažia spojiť dve protiľahlé strany. Hráči sa v ťahoch striedajú a každý hráč vždy zafarbí jedno políčko. Ťah je nevratný a obsadené políčko sa už nedá prefarbiť. Je však možné odstrániť posledný ťah a to kliknutím na tlačítko UNDO počas hry.

Hra vždy končí výhrou jedného z hráčov - nikdy nie remízou.

Hracia plocha hry je 6x6 políčok.

V hre sa striedajú červený hráč (používateľ) a modrý hráč (počítač) - na začiatku hry je vždy možné si vybrať, kto bude hru začínať.

Používateľ (červený hráč) sa snaží spojiť ľavý okraj hracieho pola s pravým okrajom a zároveň zabrániť počítaču (modrému hráčovi) spojiť vrchnú stranu so spodnou.

Počítač sa snaží o presný opak.

Program sa celý ovláda klikaním na hexagonálne tlačítka na hracej ploche. Pred spustením hry si pozorne prečítajte TUTORIAL a nezabudnite si vybrať, kto bude začínať.

Po dohraní hry Vám program zvýrazní výhernú cestu a následne je potrebné kliknúť na tlačítko MENU dostanete sa do okna kde je napísané kto vyhral a máte možnosť hrať znova - po kliknutí na tlačítko RESTART potom znovu vyberáte, kto bude začínať hru a hráte odznova, alebo hru ukončíte kliknutím na tlačítko END GAME.

## Programátorská dokumentácia

Celá hra je vytvorená ako Windows Form App. Hra sa môže nachádzať v nasledovných stavoch:
1. START - štartová plocha celej hry, ktorá obsahuje TUTORIAL, možnosť výberu kto bude začínať prvý ťah - použíateľ si vždy musí pred        spustením hry vybrať, kto začína a taktiež obsahuje START button pre spustenie hry. Kliknutím na tlačítko START sa dostaneme do jedného zo stavou GAMEUSER alebo GAMEPC. Tieto dva stavy sa v priebehu hry po každom ťahu striedajú.
2. GAMEPC - stav kedy je na ťahu počítač. Odtiaľto volám funkcie, ktoré vyberajú ťah počítača a nastavujem, ktoré komponenty vo Form sú viditeľné
3. GAMEUSER – stav kedy je na ťahu hráč. Hráč, ale vyberá ťah sám a ostatné sa deje z funkcie event nastavenej pre každé hexagonálne tlačítko. Odtiaľto nevolám žiadne funkcie.
4. WIN -  výhra používateľa, možnosť prepnúť sa do MENU a vyznačenie výhernej cesty
5. WINPC - výhra počítača, možnosť prepnúť sa do MENU a vyznačenie výhernej cesty
6. END - konečná obrazovka - možnosť spustenia hry odznova, alebo celkové ukončenie aplikácie


Prvotne som mala vymyslených menej stavov - START, GAMEPC, GAMEUSER, WIN, no postupne som pridala aj WINPC a END nakoľko mi hra takto prišla prehľadnejšia. Medzi stavmi sa prepínam pomocou funkcie SetState.

Logiku hry počítača som vymyslela tak, aby bolo jeho cieľom prvotne blokovať používateľa a druhotne aby si vyberal políčko, kde má najvyššiu šancu spojenia vrchnej strany zo spodnou. Všetky tieto kroky robí pomocou viacerých funkcií a to SetValue, SetValueForPC, NumOfRoadsLeft, NumOfRoadsRight, NumOfRoadsUp, NumOfRoadsDown a konečne vyberie bestMove vo funkcií gamePC. Všetky funkcie sú stručným komentárom popísané v kóde.

Hexagonálne tlačítka som vytvorila ako samostatnú class (HexagonalButton.cs), ktorá je potomkom triedy Button. Pracuje sa s nimi potom tak ako s normálnymi tlačítkami. Dlhšie mi trvalo vymyslieť ako sa dostanem k ich pozícii, tak aby som si ich interne vedela prefarbovať a nastavovať im hodnotu pre hru počítača - k tomu som si vytvorila ku každému tlačítku AccessibleName vo funkcií CreateAccessibleNameOfButtons. Následne som už všetko spravila pomocou 
-  pola colors kde mením hodnoty na "r"(červené políčko), "b"(modré políčko) alebo je tam " " ak políčko nie je zafarbené 
-  pola valuesUSER, kde nastavujem hodnoty políčka z pohľadu používateľa 
-  pola valuesPC, kde nastavujem hodnoty z pohľadu PC.


Ku koncu vytvárania hry som si pomyslela, že by bolo užitočné dať používateľovi možnosť zvrátiť posledný ťah. Preto som vytvorila button UNDO a k nemu event funkciu, kde pri kliknutí na toto tlačítko odstránim posledný ťah používateľa a tým zároveň aj posledný ťah počítača, ktorý už medzi tým stihol ťahať tiež. Ďalej som pridala aj možnosť vyznačenia políčka, ktoré si PC vybral ako svoj posledný ťah - tlačítko SHOW LAST PC MOVE.

V každom ťahu program prehľadáva, či náhodou už jeden z hráčov nevyhral a to pomocou BFS - funkcie BfsHexUser a BfsHexPC. Ak došlo k výhre volám funkciu HexWin a hra sa prepína buď do stavu WIN alebo do stavu WINPC podľa toho kto vyhral.

Veľa šťastia pri hraní! :)

-----Natália Komorníková-----
