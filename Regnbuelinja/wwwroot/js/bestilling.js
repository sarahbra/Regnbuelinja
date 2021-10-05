$(function () {
    const url = "Bestilling/HentBestilling?" + hentId();
    $.get(url, function (bestillingInput) {
        console.log(bestillingInput)
        formaterSide(bestillingInput);
    });
})


function formaterSide(bestillingInput) {
    let url = "Bestilling/HentAnkomstTid?" + hentId() + "&startpunkt=" + bestillingInput.startpunkt;
    $.get(url, function (ankomstTidSerialized) {
        url = "Bestilling/HentPris?" + hentId();
        $.get(url, function (totalPris) {
            if (bestillingInput.hjemreiseTid) {
                url = "Bestilling/HentAnkomstTid?" + hentId() + "&startpunkt=" + bestillingInput.endepunkt;
                $.get(url, function (ankomstTidReturSerialized) {
                    formaterBestilling(bestillingInput, ankomstTidReturSerialized, "Båtten Anna", true);
                });
            }
            formaterKjøpsInfo(bestillingInput, totalPris);
        });
        formaterBestilling(bestillingInput, ankomstTidSerialized, "Kjærleiksbåten", false);
    });
}

function hentId() {
    const url = window.location.search.substring(1);
    return url;
}

// TODO - Add båtnavn
function formaterBestilling(bestillingInput, ankomstTidSerialized, skip, retur) {

    let startpunkt = bestillingInput.startpunkt;
    let endepunkt = bestillingInput.endepunkt;
    let avreiseSerialized = bestillingInput.avreiseTid;

    var container = $("#utreise");
    if (retur) {
        startpunkt = bestillingInput.endepunkt;
        endepunkt = bestillingInput.startpunkt;
        avreiseSerialized = bestillingInput.hjemreiseTid;
        container = $("#returreise");
    }

    const avreiseDate = new Date(avreiseSerialized);
    console.log(avreiseDate.toDateString())
    const ankomstDate = new Date(ankomstTidSerialized);
    console.log(ankomstDate.toDateString())

    let ut = "";
    ut += "<li class='list-group-item'><label for='dato_avreise' class='col-12 col-sm-3 fw-bold'>Avreise</label>" + avreiseDate.toDateString() + "</li>" +
        "<li class='list-group-item'><label for='avgang' class='col-12 col-sm-3 fw-bold'>Klokkeslett</label>" + avreiseDate.toLocaleTimeString() + "</li>" +
        "<li class='list-group-item'><label for='dato_ankomst' class='col-12 col-sm-3 fw-bold'>Ankomst</label>" + ankomstDate.toDateString() + "</li>" +
        "<li class='list-group-item'><label for='ankomst' class='col-12 col-sm-3 fw-bold'>Klokkeslett</label>" + ankomstDate.toLocaleTimeString() + "</li>" +
        "<li class='list-group-item'><label for='skip' class='col-12 col-sm-3 fw-bold'>Skip</label>" + skip + "</li>" +
        "<li class='list-group-item'><label for='strekning' class='col-12 col-sm-3 fw-bold'>Strekning</label>" + startpunkt + " - " + endepunkt + "</li>";
    container.html(ut);
}


function formaterKjøpsInfo(bestillingInput, pris) {
    let ut = "<table class='table'><thead><tr>";
    ut += "<th>#</th><th>Produkt</th><th>Produktbeskrivelse</th>" +
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

    ut += "<tr><td></td><td></td><th></th><th>Totalpris</th><td>" + pris + "</td>";
    ut += "</tbody</table>";
    $("#kjøpsInfo").html(ut);
}