# Regnbuelinja Gruppeoppgave 2

## Studenter:

- s346419
- s356546
- s317473
- s311596

Hele applikasjonen (inkludert oppgave1) kjører på http://localhost:5000/. Det er en knapp på første siden (index.html) til admin-applikasjonen. Admin-applikasjonen kjører på http://localhost:5000/admin

## Logginn for adminbruker:

- Brukernavn: admin
- Passord: Test1234

### INFO

Vi misforstod oppgave 1 og manglet noe funksjonalitet (kunde og betaling). For å reflektere det vi manglet i oppgave 2 har vi lagt til en registreringsside for kunde i kundeapplikasjonen og vedlikehold av denne i adminapplikasjonen. Vi har lagt til funksjonalitet fra oppgave 1 i oppgave 2 fordi vi har valgt å levere prosjektet som ett, og det vil da se rart ut om admin kan vedlikeholde kunde, men i kundeapplikasjonen oppgir man ingen kundeinformasjon.

Vi hadde ingen form for betaling i første oppgave. Derfor har vi lagt til en ‘boolean betaling’ på bestilling. Vi ser for oss at man kan bestille reise uten å ha betalt ved bestilling. I vår løsning er det altså mulig å betale senere.

Vi har en relativt kompleks databasestruktur med flere avhengigheter. Vi har satt opp en del ‘regler’ for disse avhengighetene som beskriver når det går an å endre, slette og legge til entiteter. Det er mulig at noen av disse reglene kan virke noe ulogiske i forhold til en realistisk versjon men vi har tatt beslutningene for å være tro mot modellene i databasen fra første oppgave samtidig som at det skal være så realistisk som mulig.

# Regler:

## Rute:

Slett:

- Hvis rute er med i en bestillt ferd så kan ikke ruten slettes (bestilling råder)

Endre:

- Hvis rute med i eksisterende bestilling(er)/ med i en bestilt ferd så kan ikke ruten endres (bestiling råder)

## Båt:

Slett:

- Hvis båt med i en bestillt fer/ med i en bestilling, kan ikke båten slettes (bestilling råder)

Endre:

- Båt har kun båtnavn og id så en båt kan endre navn selv om den er med i en bestilt ferd. Båt kan alltid endres.

## Ferd:

Slett:

- Hvis ferd er med i bestillinger kan ikke ferden slettes (bestilling råder)

Endre:

- Ferd kan ikke endres hvis den er med i en bestilling (bestilling råder)

- En feil vi ikke har fått tid til å rette opp:
  Når man endrer en ferd til en ankomstdato som er før avreisedato får bruker feil tilbakemelding. Logikken er riktig på server men tilbakemeldingen om at det finnes avhengigheter stemmer ikke her.

## Kunde:

Slett:

- Hvis kunden har bestillinger kan ikke kunden slettes. Bestilling må slettes først (bestilling råder)

Endre:

- En kunde kan alltid endres (utenom id selvfølgelig)

## Billetter:

Slett:

- Hvis bestillingen ikke er betalt så kan ikke billetter på bestillingen slettes (Bestilling råder).

Endre:

- Billett kan ikke endres hvis den allerede er brukt
- Billett kan ikke endres dersom bestillingen er betalt (grunnet totalprisen på bestillingen kan bli endret og da må kunden enten få tilbake penger eller betale mer. For enkelthetens skyld setter vi denne regelen)
- Det eneste som kan endres er voksen/barn. Hvis andre ting skal endres må man slette og opprette ny billett

Legg til:

- En bestilling kan ikke bestå av mer enn tur/retur billetter. Man kan legge til billetter på en bestilling, men da må det være samme ferd som er bestilt fra før, hvis ikke blir det en ny bestilling.

## Bestillinger:

Slett:

- Man kan slette en bestilling som er ubetalt hvis den er fremover i tid (kunde avbestiller reise).
- Man kan slette en bestilling som er betalt og tilbake i tid (gamle bestillinger for gamle reiser)

- Man kan ikke slette bestilling hvis den er betalt og fremover i tid (tilbakebetaling til kunde må skje først).
- Man kan ikke slette bestilling hvis den er ubetalt og reisen er gjennomført (kunden skylder penger)
- Man kan slette en bestilling som ikke er betalt hvis den er framover i tid (ikke har vært

Endre:

- En ubetalt bestilling: kan endre kunde og betaling.
- En betalt bestilling kan ikke endres i det hele tatt -> endre-knapp fjernes i dette tilfellet ved sjekk om betaling = true. Hvis man endrer en ubetalt bestilling til betalt så kan man ikke endre den igjen.
- Hvis det kun er enveisbillett på bestilling kan man legge til en returbillett. Da er det kun mulig å legge til en billett med en ferd som er etter ankomsttid(dato og klokkeslett). Dette for å forhindre at kunden kan bestille returreise mens kunden er midt på havet på en annen reise. (Det er kun gyldige returreiser som dukker opp i dropdown når man prøver å legge til en returreise på en bestilling).

Legg til:

- Når man legger til en bestilling vil den være tom -> uten billetter. For å fylle bestillingen må man gå til billetter og legge til disse på bestillingen. Totalpris vil være 0 for en bestilling uten billetter (totalpris genereres ut fra billettpriser)

# FRA FORRIGE OPPGAVE:

# Regnbuelinja Gruppeoppgave 1

# ITPE3200 Webapplikasjoner - Webapplikasjon for bestilling av båtreiser

## Studenter:

- s346419
- s356546
- s317473
- s311596

## Funksjonalitet/informasjon vi ønsker å fremheve:

### Bestillingsbekreftelse og billett i klienten: (bestilling.html):

- Det er ikke spesifisert i kravspesifikasjonen at man trenger en utskfift av billett eller bekreftelse på bestilling. Vi har valgt å implementere dette da vi mener at kunden trenger bekreftelse på bestillingen samt vite detaljer om reisen som er bestilt for at løsningen skal være brukervennlig.

### Forklaring av entiteter:

- _Bestillinger:_
  Én kunde må kunne bestille flere billetter. Én billett gjelder for én person for én strekning for å kunne skille voksen/barn og pris. Dersom kunden bestiller for flere personer eller flere strekninger (tur/retur f.eks.) vil kunden ha flere billetter. Kunde er kommentert ut fordi vi ikke har noen funksjonalitet for å registrere kunde (i henhold til kravspek).

- _Billett:_
  Knyttet til én person (barn eller voksen) og én ferd

- _Båt:_
  Siden vi har flere ruter er det praktisk med flere båter hvis vi ikke skal ha veldig få datoer per ferd. Én båt kan ikke kjøre flere ruter samtidig.

- _Ferd:_
  Én båt kan kjøre flere ruter, men båten må da være tilknyttet dato og tid for at det skal være realistisk. På denne måten er det også mulig å knytte billetter til avgangs- og anksomsttid og gjøre det umulig for kunden å velge en hjemreisedato før avreisedato. Hjemreisedato er implementert utfra ankomsttid (slik at man bare kan velge hjemreise etter man har ankommet). I grensesnittet hentes mulige avreisedatoer og dermed hjemreisedatoer basert på avreisedato slik at klienten blir mer brukervennlig (rask tilbakemelding på når man kan reise).

- _Rute:_
  Ved å skille rute fra ferd kan fastpris på en strekning enklere oppdateres. En pris trenger ikke å være tilknyttet en spesifikk båt og spesifikke datoer, men kun avreisehavn og ankomsthavn. Dermed kan man gi rabatterte priser på en ferd basert på fastpris ved behov men likvel ha en referanse til opprinnelig pris på en rute.

### DBinit:

Vi har kun lagt inn avreise- og hjemreisedatoer for desember måned 2021. Dersom man velger avreisedato på en av de siste dagene i desember er det ikke mulig å velge returdato når reisen tar 2-3 dager (avhengig av rute). I praksis ville man hatt flere tilgjengelige avreisemåneder men vi har gjort det sånn nå for enkelthetensskyld. Vi viser feilmelding til bruker i klienten om at man man må velge en tidligere avreisedato dersom hjemreisedato ikke er tilgjengelig (kun dersom tur/retur er valgt).
