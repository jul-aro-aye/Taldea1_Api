# Hasteko Gida

## Beharrezko gauzak

- .NET 8 SDK
- MySQL datu-basea
- `db.sql` fitxategia kargatuta izatea

## APIa martxan jarri

```bash
dotnet restore ErronkaApi/ErronkaApi.csproj
dotnet run --project ErronkaApi/ErronkaApi.csproj
```

Garapen moduan, APIa helbide hauetan dago:

- `http://localhost:5093`
- `https://localhost:7236`

Swagger ikusteko `Development` moduan egon behar da.

## Dokumentazioa DocFX-rekin egin

Hemen azaldutako dokumentazioa eta APIaren informazioa lotzen dira.

### 1. Informazioa prestatu

```bash
docfx metadata docfx.json
```

Honek kodea aztertzen du eta informazioa ateratzen du.

### 2. Webgunea eraiki

```bash
docfx build docfx.json
```

### 3. Lokalean ikusi

```bash
docfx serve _site
```

## Zer irakurri lehenago

1. `README.md`: Proiektuaren ikuspegi orokorra izateko.
2. `introduction.md`: Gauzak nola lotzen diren ikusteko.
3. `api/index.md`: Kontrolatzaileak eta DTOak ulertzeko.
4. DocFX-ek sortutako orriak: Metodoak eta datuak xehetasunez ikusteko.
