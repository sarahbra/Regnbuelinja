$(function () {
    //$("#returReise").hide();       //TODO - Make this not show up on refresh
    const id = window.location.search.substring(1);
    const url = "Bestilling/HentBestilling?" + id;
    $.get(url, function (bestillingInput) {
        formaterSide(bestillingInput);
    });
})

function formaterSide(bestillingInput) {
    formaterBestilling(bestillingInput);
    formaterKjøpsInfo(bestillingInput);
    if (bestillingInput.hjemreiseDato) {
        formaterReturReise(bestillingInput);
    }
}

function formaterBestilling(bestillingInput) {
    $("#utreiseHeader").html("Utreise fra " + bestillingInput.startpunkt);
    $("#dato").html(bestillingInput.avreiseDato);
    $("#skip").html("Båtten Anna");  //TODO - Add båtnavn
    $("#strekning").html(bestillingInput.startpunkt + " - " + bestillingInput.endepunkt);
    $("#id").html(best);
}

//Funker ikke skikkelig. Teori om at hjemreiseDato = null
function formaterReturBestilling(bestillingInput) {
    $("#returReiseHeader").html("Hjemreise fra " + bestillingInput.endepunkt);
    $("#retur_dato").html(bestillingInput.hjemreiseDato);
    $("#retur_skip").html("Båtten Anna");
    $("#retur_strekning").html(bestillingInput.endepunkt + " - " + bestillingInput.startpunkt);
    $("#returReise").show();
}

//Fungerer ikke som den skal
function formaterKjøpsInfo(bestillingInput) {
    let ut = "<table class='table'><thead><tr>";
    ut += "<th>#</th><th>Produkt></th><th>Produkt beskrivelse</th>" +
        "<th>Antall</th><th>Pris</th></tr></thead>" +
        "<tbody>" +
        "<tr><th>1</th>" +
        "<td>" + bestillingInput.startpunkt + "-" + bestillingInput.endepunkt + "</td>" +
        "<td>Økonomibillett</td> " +
        "<td>" + bestillingInput.antallVoksne + "voksne + " + bestillingInput.antallBarn + " barn</td>" +
        "<td>1350</td></tr>"; //TODO: pris ikke lagt inn i bestillingInput!!!

    if (bestillingInput.hjemreiseDato) {
        ut += "<tr><th>2</th>" +
            "<td>" + bestillingInput.endepunkt + "-" + bestillingInput.startpunkt + "</td>" +
            "<td>Økonomibillett</td>" +
            "<td>" + bestillingInput.antallVoksne + "voksne + " + bestillingInput.antallBarn + " barn</td>" +
            "<td>650</td></tr>"; //TODO: pris ikke lagt inn i bestillingInput!!!
    }
    ut += "</tbody</table>";
    $("#kjøpsInfo").html(ut);
}