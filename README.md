# Regnbuelinja

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
