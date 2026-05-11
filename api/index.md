# APIaren Azalpena

Hemen `ApiErronka2` proiektuaren datuak nola mugitzen diren azaltzen da: zer datu sartzen diren eta zer ateratzen den.

## DTOak nola erabili

- Sartzeko `DTO`ak: Zerbait sortu edo aldatu nahi denean erabiltzen dira.
- Irteteko `DTO`ak: Informazioa kanpora bidaltzeko erabiltzen dira, datu guztiak ez erakusteko.
- Batzuetan `DTO` bera erabiltzen da sartzeko eta irteteko, datuak berdinak badira.

## Erreserbak, mahaiak eta eskariak

Hau da prozesu garrantzitsuena:

1. Erabiltzaileak `api/mahaiak` bidez ikusten du zein mahai dauden libre.
2. Erreserba bat egon ezkero, `api/Erreserbak` bidez kudeatzen da.
3. Eskari bat irekitzeko `api/eskaerak` erabiltzen da `EskaeraSortuDTO`-rekin. Erreserba bat izan dezake lotuta.
4. Eskaria mahai bati lotzen zaio eta produktuak gastatzen ditu.
5. Zerbitzua amaitzean, ordaindu egiten da eta faktura sortzen da.

## Gauza garrantzitsuak

- `EskaeraDTO`-k ez ditu produktuak erakusten; horretarako `EskaeraLortuDTO` dago.
- `ErreserbaMahaiDTO`-k erreserba eta mahaia lotzen ditu.
- DocFX-ek sortutako orrietan kontrolatzaile eta DTO guztiak ikusiko dituzu azalpenekin.

## Nola mugitu

- **APIaren xehetasunak**: Klik egin ezkerreko menuan **Generated Reference** atalean. Hor ikusiko dituzu kontrolatzaile eta DTO guztiak banan-banan.
- Kontrolatzaileetan azalpenak jarri ditugu zer egiten duten ulertzeko.
- DTOetan propietate bakoitzaren azalpena daukazu.
