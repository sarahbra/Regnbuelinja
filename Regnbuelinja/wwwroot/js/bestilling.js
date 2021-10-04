$(function () {
    const url = "Bestilling/HentBestilling?" + hentId();
    $.get(url, function (bestillingInput) {
        console.log(bestillingInput)
        formaterSide(bestillingInput);
    });
})

function formaterSide(bestillingInput) {
    let avreiseTid = new Date(bestillingInput.avreiseTid);
    console.log(avreiseTid);
    const avreiseTidTicks = avreiseTid.getTime();
    console.log(avreiseTidTicks);
    $.get("Bestilling/HentAnkomstTid?avreiseTicks=" + avreiseTidTicks, function (ankomstTid) {
        url = "Bestilling/HentPris?" + hentId();
        $.get(url, function (totalPris) {
            if (bestillingInput.hjemreiseTid) {
                const hjemreiseTid = new Date(bestillingInput.hjemreiseTid).toISOString();
                $.get("Bestilling/HentAnkomstTid", hjemreiseTid, function (ankomstTid_retur) {
                    formaterBestilling(bestillingInput, ankomstTid_retur, "Båtten Anna", true);
                });
            }
            formaterKjøpsInfo(bestillingInput, totalPris);
        });
        formaterBestilling(bestillingInput, ankomstTid, "Kjærleiksbåten", false);
    });
}

function hentId() {
    const url = window.location.search.substring(1);
    return url;
}

// TODO - Add båtnavn
function formaterBestilling(bestillingInput, ankomstTid, skip, retur) {

    let startpunkt = bestillingInput.startpunkt;
    let endepunkt = bestillingInput.endepunkt;
    var container = $("#utreise");
    if (retur) {
        startpunkt = bestillingInput.endepunkt;
        endepunkt = bestillingInput.startpunkt;
        container = $("#returreise");
    }

    let ut = "";
    ut += "<li class='list-group-item'></li>" +
        "<li class='list-group-item'><label for='avgang' class='col-12 col-sm-3 fw-bold'>Avgang</label>" + bestillingInput.avreiseTid.toString() + "</li>" +
        "<li class='list-group-item'><label for='ankomst' class='col-12 col-sm-3 fw-bold'>Ankomst</label>" + ankomstTid.toString() + "</li>" +
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