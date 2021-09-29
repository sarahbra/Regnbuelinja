$(function () {
    const url = "Bestilling/HentBestilling?" + hentId();
    $.get(url, function (bestillingInput) {
        formaterSide(bestillingInput);
    });
})

function formaterSide(bestillingInput) {
    formaterBestilling(bestillingInput);
    if (bestillingInput.hjemreiseDato) {
        formaterReturBestilling(bestillingInput);
    } else {
        formaterKjøpsInfo(bestillingInput);
    }
}

function hentId() {
    const url = window.location.search.substring(1);
    return url;
}

// TODO - Add båtnavn
function formaterBestilling(bestillingInput) {
    $("#utreiseHeader").html("Utreise fra " + bestillingInput.startpunkt);
    let ut = "";
    ut += "<li class='list-group-item'><label for='dato' class='col-12 col-sm-3 fw-bold'>Dato</label>" + bestillingInput.avreiseDato.toString() + "</li>" +
          "<li class='list-group-item'><label for='avgang' class='col-12 col-sm-3 fw-bold'>Avgang</label>08:30</li>" +
          "<li class='list-group-item'><label for='ankomst' class='col-12 col-sm-3 fw-bold'>Ankomst</label>20:00</li>" +
        "<li class='list-group-item'><label for='skip' class='col-12 col-sm-3 fw-bold'>Skip</label>Båtten Anna</li>" +
        "<li class='list-group-item'><label for='strekning' class='col-12 col-sm-3 fw-bold'>Strekning</label>" + bestillingInput.startpunkt + " - " + bestillingInput.endepunkt + "</li>";
    $("#utreise").html(ut);
}

function hentPris() {
    let prisen = 0.0;
    const url = "Bestilling/HentPris?" + hentId();
    $.get(url, function (pris) {
        prisen = pris;
    });
    return prisen;
}


function formaterReturBestilling(bestillingInput) {
    $("#returreiseHeader").html("Hjemreise fra " + bestillingInput.endepunkt);
    let ut = "";
    ut += "<li class='list-group-item'><label for='retur_dato' class='col-12 col-sm-3 fw-bold'>Dato</label>" + bestillingInput.hjemreiseDato.toString() + "</li>" +
        "<li class='list-group-item'><label for='avgang' class='col-12 col-sm-3 fw-bold'>Avgang</label>14:00</li>" +
        "<li class='list-group-item'><label for='ankomst' class='col-12 col-sm-3 fw-bold'>Ankomst</label>23:00</li>" +
        "<li class='list-group-item'><label for='retur_skip' class='col-12 col-sm-3 fw-bold'>Skip</label>Båtten Anna</li>" +
        "<li class='list-group-item'><label for='retur_strekning' class='col-12 col-sm-3 fw-bold'>Strekning</label>" + bestillingInput.endepunkt + " - " + bestillingInput.startpunkt + "</li>";
    $("#returreise").html(ut);
    formaterKjøpsInfo(bestillingInput);
}


function formaterKjøpsInfo(bestillingInput) {
    let ut = "<table class='table'><thead><tr>";
    ut += "<th>#</th><th>Produkt</th><th>Produkt beskrivelse</th>" +
        "<th>Antall</th><th>Pris</th></tr></thead>" +
        "<tbody>" +
        "<tr><th>1</th>" +
        "<td>" + bestillingInput.startpunkt + " - " + bestillingInput.endepunkt + "</td>" +
        "<td>Økonomibillett</td> " +
        "<td>" + bestillingInput.antallVoksne + " voksne + " + bestillingInput.antallBarn + " barn. </td>" +
        "<td></td></tr>"; //TODO: reisePris ikke i kontroller

    if (bestillingInput.hjemreiseDato) {
        ut += "<tr><th>2</th>" +
            "<td>" + bestillingInput.endepunkt + " - " + bestillingInput.startpunkt + "</td>" +
            "<td>Økonomibillett</td>" +
            "<td>" + bestillingInput.antallVoksne + " voksne + " + bestillingInput.antallBarn + " barn. </td>" +
            "<td></td></tr>"; //TODO: reisePris ikke i kontroller
    }

    ut += "<tr><td></td><td></td><th></th><td>Totalpris</td><td>" + hentPris() + "</td>";
    ut += "</tbody</table>";
    $("#kjøpsInfo").html(ut);
}