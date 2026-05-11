# ApiErronka2

`ApiErronka2` jatetxe bat kudeatzeko API bat da. .NET 8 eta NHibernate erabiltzen ditu gauza hauek egiteko: erabiltzaileak, kategoriak, produktuak, mahaiak, erreserbak, eskariak, fakturak eta Odoo-rekin lotura.

## Proiektuaren helburua

API honek sukaldea eta aretoa kudeatzen ditu:

- Mahaiak eta libre dauden ikusi.
- Erreserbak egin eta mahai bati lotu.
- Eskariak ireki mahai batean (erreserba batekin edo gabe).
- Produktuak eta stock-a kontrolatu.
- Eskariak ordaindu eta fakturak egin.
- Datuak prestatu Odoo-ra bidaltzeko.

## Nola pentsatu dugu diseinua

Dokumentazio honetan arau hauek jarraitu ditugu:

- Kontrolatzaileek `DTO`ak erabiltzen dituzte, ez datu-baseko modeloak.
- `DTO` bakoitzak bere lana du (datuak sartu edo atera).
- Azalpenetan jartzen dugu zergatik erabiltzen den `DTO` bakoitza.

Adibideak:

- `EskaeraSortuDTO`: Eskari bat sortzeko (mahaia, ordua, jende kopurua...).
- `EskaeraDTO`: Eskariak zerrendan ikusteko.
- `EskaeraLortuDTO`: Eskari baten produktuak ikusteko.
- `ErantzunaDTO<T>`: Erantzun guztiak formatu berean bidaltzeko.

## Gauzen arteko lotura

Hau da garrantzitsuena:

1. `Erreserba` batek `Mahaia` bat gordetzen du egun eta txanda batean.
2. `Eskaera` bat mahai batean irekitzen da (erreserba bati lotuta egon daiteke).
3. Eskariak produktuak ditu eta stock-a aldatzen du.
4. Eskaria ordaintzean, `Faktura` eguneratzen da.
5. Faktura PDF-a egitean, eskaria ixten da eta mahaia libre geratzen da.

## Proiektuaren egitura

- `ErronkaApi/Kontrollerrak`: APIaren helbideak (endpoints).
- `ErronkaApi/DTOak`: Datuak sartzeko eta ateratzeko fitxategiak.
- `ErronkaApi/Repositorioak`: Datu-basearekin hitz egiten duen zatia.
- `ErronkaApi/Modeloak`: Datu-baseko taulak.
- `ErronkaApi/Mapeoak`: NHibernate loturak.
- `ErronkaApi/Middlewareak`: Log-ak eta saioak kudeatzeko.

## Helbide nagusiak

- `api/login`: Sartzeko eta baimenak ikusteko.
- `api/erabiltzaileak`: Erabiltzaileak kudeatzeko.
- `api/kategoriak`: Kategoriak kudeatzeko.
- `api/produktuak`: Produktuak kudeatzeko.
- `api/mahaiak`: Mahaiak ikusteko.
- `api/Erreserbak`: Erreserbak kudeatzeko.
- `api/ErreserbaMahaiak`: Erreserba eta mahaia lotzeko.
- `api/eskaerak`: Eskariak kudeatzeko.
- `api/Fakturak`: Fakturak ikusteko.
- `api/odoo`: Odoo-rako datuak.

## Nola erabili

### Beharrezkoa

- .NET 8 SDK
- MySQL datu-basea
- Liburutegiak kargatuta (NuGet)

### Martxan jarri

```bash
dotnet restore ErronkaApi/ErronkaApi.csproj
dotnet run --project ErronkaApi/ErronkaApi.csproj
```

Garapenean hemen dago:

- `http://localhost:5093`
- `https://localhost:7236`

Swagger ikusteko `Development` moduan egon behar duzu.

## DocFX dokumentazioa

Dokumentazioa egiteko:

```bash
docfx metadata docfx.json
docfx build docfx.json
docfx serve _site
```
