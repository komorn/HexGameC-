# Hex Game

## Užívateľská dokumentácia

Hra HEX je hra dvoch hráčov, ktorí sa snažia spojiť dve protiľahlé strany. Hráči sa v ťahoch striedajú a každý hráč vždy zafarbí jedno políčko. Ťah je nevratný a obsadené políčko sa už nedá prefarbiť, ale je možné odstrániť posledný ťah kliknutím na tlačítko UNDO počas hry.

Hra vždy končí výhrou jedného z hráčov - nikdy nie remízou.

Hracia plocha hry je 6x6 políčok.

V hre sa striedajú červený hráč (používateľ) a modrý hráč (počítač) - na začiatku hry je vždy možné si vybrať, kto bude hru začínať.

Používateľ (červený hráč) sa snaží spojiť ľavý okraj hracieho pola s pravým okrajom a zároveň zabrániť počítaču (modrému hráčovi) spojiť vrchnú stranu so spodnou.

Počítač sa snaží o presný opak.

Program sa celý ovláda klikaním na hexagonálne tlačítka na hracej ploche.

Po dohraní hry môžete hrať znova po kliknutí na tlačítko RESTART. Potom znovu vyberáte, kto bude začínať hru a hráte odznova.

## Programátorská dokumentácia

Celá hra je vytvorená ako Windows Form App. Hra sa môže nachádzať v nasledovných stavoch:
    1. START - štartová plocha celej hry, ktorá obsahuje TUTORIAL, možnosť výberu kto bude začínať prvý ťah - použíateľ si vždy musí pred spustením hry vybrať, kto začína a taktiež obsahuje START button pre spustenie hry. Kliknutím na tlačítko START sa dostaneme do jedného zo stavou GAMEUSER alebo GAMEPC. Tieto dva stavy sa v priebehu hry po každom ťahu striedajú.
    2.  


Veľa šťastia pri hraní! :)

-----Natália Komorníková-----
