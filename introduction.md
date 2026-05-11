# Sarrera

`ApiErronka2` jatetxea kudeatzeko proiektuaren barneko aldea da. API honek webgunea, TPV-a eta Odoo sistema lotzen ditu.

## Zer egiten duen

API honek lau gauza nagusi egiten ditu:

1. Produktuak: Zer produktu eta kategoria dauden kudeatzen du.
2. Aretoa: Erabiltzaileak, mahaiak eta erreserbak kudeatzen ditu.
3. Zerbitzua: Eskariak, produktuak eta sukaldea kudeatzen ditu.
4. Administrazioa: Ordainketak eta fakturak kudeatzen ditu.

## Datuak nola bidali

Dokumentazio hau dena ondo ulertzeko egin dugu:

- Kontrolatzaileek `DTO`ak erabiltzen dituzte datuak bidaltzeko eta jasotzeko.
- `DTO` bakoitzak lan bakarra du: sortu, aldatu, ikusi edo sinkronizatu.
- Erantzun guztiak `ErantzunaDTO<T>` barruan doaz, beti berdina izateko.

## Gauzen arteko lotura

Hona hemen garrantzitsuena:

- `Mahaia`: Jatetxeko mahai bat da.
- `Erreserba`: Mahai bat gordetzeko egun eta ordu batean.
- `Eskaera`: Mahai batean eskatzen dena da. Erreserba bati lotu ahal zaio.
- `EskaeraProduktuak`: Eskari batean dauden produktuak dira.
- `Faktura`: Azken ordainketa da eta PDF bat izan dezake.

## Zer dokumentatu dugu

- Kontrolatzaileetan azalpenak jarri ditugu XML bidez.
- DTOetan azalpenak jarri ditugu.
- Markdown orriak egin ditugu dena hobeto azaltzeko.
- `README` fitxategia instalaziorako eta erabilerarako.
