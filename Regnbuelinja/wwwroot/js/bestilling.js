$(function () {
    const url = "Bestilling/HentBestilling?id=" + hentId();
    $.get(url, function (bestillingInput) {
        formaterSide(bestillingInput);
    });
})

function formaterSide(bestillingInput) {
    const url = "Bestilling/HentAnkomstTid?avreiseTid=" + bestillingInput.avreiseTid;
    $.get(url, function (ankomstTid) {
        url = "Bestilling/HentPris?id=" + hentId();
        $.get(url, function (totalPris) {
            if (bestillingInput.hjemreiseTid) {
                url = "Bestilling/HentAnkomstTid?avreiseTid=" + bestillingInput.avreiseTid;
                $.get(url, function (ankomstTid_retur) {
                    formaterReturBestilling(bestillingInput, ankomstTid_retur);
                });
            }
            formaterKjøpsInfo(bestillingInput, totalPris);
        });
        formaterBestilling(bestillingInput, ankomstTid);
    });
}

function hentId() {
    const url = window.location.search.substring(1);
    return url;
}

// TODO - Add båtnavn
function formaterBestilling(bestillingInput, ankomstTid) {
    $("#utreiseHeader").html("Utreise fra " + bestillingInput.startpunkt);
    let ut = "";
    ut += "<li class='list-group-item'><label for='dato' class='col-12 col-sm-3 fw-bold'>Dato</label></li>" +
        "<li class='list-group-item'><label for='avgang' class='col-12 col-sm-3 fw-bold'>Avgang</label>" + bestillingInput.avreiseTid.toString() + "</li>" +
        "<li class='list-group-item'><label for='ankomst' class='col-12 col-sm-3 fw-bold'>Ankomst</label>" + ankomstTid + "</li>" +
        "<li class='list-group-item'><label for='skip' class='col-12 col-sm-3 fw-bold'>Skip</label>Båtten Anna</li>" +
        "<li class='list-group-item'><label for='strekning' class='col-12 col-sm-3 fw-bold'>Strekning</label>" + bestillingInput.startpunkt + " - " + bestillingInput.endepunkt + "</li>";
    $("#utreise").html(ut);
}


function formaterReturBestilling(bestillingInput, ankomstTid) {
    $("#returreiseHeader").html("Hjemreise fra " + bestillingInput.endepunkt);
    let ut = "";
    ut += "<li class='list-group-item'><label for='retur_dato' class='col-12 col-sm-3 fw-bold'>Dato</label></li>" +
        "<li class='list-group-item'><label for='avgang' class='col-12 col-sm-3 fw-bold'>Avgang</label>" + bestillingInput.hjemreiseTid.toString() + "</li>" +
        "<li class='list-group-item'><label for='ankomst' class='col-12 col-sm-3 fw-bold'></label>" + ankomstTid + "</li>" +
        "<li class='list-group-item'><label for='retur_skip' class='col-12 col-sm-3 fw-bold'>Skip</label>Båtten Anna</li>" +
        "<li class='list-group-item'><label for='retur_strekning' class='col-12 col-sm-3 fw-bold'>Strekning</label>" + bestillingInput.endepunkt + " - " + bestillingInput.startpunkt + "</li>";
    $("#returreise").html(ut);
    formaterKjøpsInfo(bestillingInput);
}


function formaterKjøpsInfo(bestillingInput, pris) {
    let ut = "<table class='table'><thead><tr>";
    ut += "<th>#</th><th>Produkt</th><th>Produkt beskrivelse</th>" +
        "<th>Antall</th><th>Pris</th></tr></thead>" +
        "<tbody>" +
        "<tr><th>1</th>" +
        "<td>" + bestillingInput.startpunkt + " - " + bestillingInput.endepunkt + "</td>" +
        "<td>Økonomibillett</td> " +
        "<td>" + bestillingInput.antallVoksne + " voksne + " + bestillingInput.antallBarn + " barn. </td>" +
        "<td></td></tr>"; //TODO: reisePris ikke i kontroller

    if (bestillingInput.hjemreiseTid) {
        ut += "<tr><th>2</th>" +
            "<td>" + bestillingInput.endepunkt + " - " + bestillingInput.startpunkt + "</td>" +
            "<td>Økonomibillett</td>" +
            "<td>" + bestillingInput.antallVoksne + " voksne + " + bestillingInput.antallBarn + " barn. </td>" +
            "<td></td></tr>"; //TODO: reisePris ikke i kontroller
    }

    ut += "<tr><td></td><td></td><th></th><td>Totalpris</td><td>" + pris + "</td>";
    ut += "</tbody</table>";
    $("#kjøpsInfo").html(ut);
}