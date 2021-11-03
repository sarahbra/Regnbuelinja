function validerFornavn(fornavn) {
    const regexp = /^[a-zA-ZæøåÆØÅ\. \=]{2,20}$/;
    const ok = regexp.test(fornavn);
    if (!ok) {
        $("#feilFornavn").html("Fornavnet må bestå av 2-20 bokstaver.");
        return false;
    } else {
        $("#feilFornavn").html("");
        return true;
    }
}

function validerEtternavn(etternavn) {
    const regexp = /^[a-zA-ZæøåÆØÅ\. \=]{2,20}$/;
    const ok = regexp.test(etternavn);
    if (!ok) {
        $("#feilEtternavn").html("Etternavnet må bestå av 2-20 bokstaver.");
        return false;
    } else {
        $("#feilEtternavn").html("");
        return true;
    }
}

function validerEpost(epost) {
    const regexp = /^[0-9a-zA-ZæøåÆØÅ\. \=]{2,50}$/;
    const ok = regexp.test(epost);
    if (!ok) {
        $("#feilEpost").html("Adressen må bestå av 2-50 bokstaver og tall.");
        return false;
    } else {
        $("#feilEpost").html("");
        return true;
    }
}

function validerTelefonnr(Telefonnr) {
    const regexp = /^[0-9]{4}$/;
    const ok = regexp.test(postNr);
    if (!ok) {
        $("#feilTelefonnr").html("Telefonnummeret må bestå av 8 tall.");
        return false;
    } else {
        $("#feilTelefonnr").html("");
        return true;
    }
}

function validerKunde(kunde) {
    return (validerFornavn(kunde.fornavn) && validerEtternavn(kunde.etternavn) && validerEpost(kunde.epost)
        && validerTelefonnr(kunde.telefonnr));
}

function validerBrukernavn(brukernavn) {
    const regexp = /^[a-zA-ZæøåÆØÅ\. \=]{2,20}$/;
    if (!regexp.test(brukernavn)) {
        $("#feilBrukernavn").html("Brukernavnet må bestå av 2 til 20 bokstaver.");
        return false;
    } else {
        $("#feilBrukernavn").html("");
        return true;
    }
}

function validerPassord(passord) {
    const regexp = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$/;
    if (!regexp.test(passord)) {
        $("#feilPassord").html("Passordet må være minst 6 tegn langt med minst en bokstav og ett tall");
        return false;
    } else {
        $("#feilPassord").html("");
        return true;
    }
}

function validerBruker(bruker) {
    return (validerBrukernavn(bruker.brukernavn) && validerPassord(bruker.passord));
}