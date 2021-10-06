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
        if (bestillingInput.hjemreiseTid) {
            url = "Bestilling/HentAnkomstTid?" + hentId() + "&startpunkt=" + bestillingInput.endepunkt;
            $.get(url, function (ankomstTidReturSerialized) {
                formaterBestilling(bestillingInput, ankomstTidReturSerialized, true);
            });   
        }
        formaterBestilling(bestillingInput, ankomstTidSerialized, false);
    });
    formaterKjøpsInfo(bestillingInput);
}

function hentId() {
    const url = window.location.search.substring(1);
    return url;
}


function formaterBestilling(bestillingInput, ankomstTidSerialized, retur) {

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
    const ankomstDate = new Date(ankomstTidSerialized);

    const url = "Bestilling/HentBaat?" + hentId() + "&startpunkt=" + startpunkt;
    $.get(url, function (baatnavn) {
        let ut = "";
        ut += "<li class='list-group-item'><label for='avreise' class='col-12 col-sm-3 fw-bold'>Avreise</label>" + avreiseDate.toDateString() + " - " + avreiseDate.toLocaleTimeString() + "</li>" +
            "<li class='list-group-item'><label for='ankomst' class='col-12 col-sm-3 fw-bold'>Ankomst</label>" + ankomstDate.toDateString() + " - " + ankomstDate.toLocaleTimeString() + "</li>" +
            "<li class='list-group-item'><label for='skip' class='col-12 col-sm-3 fw-bold'>Skip</label>" + baatnavn + "</li>" +
            "<li class='list-group-item'><label for='strekning' class='col-12 col-sm-3 fw-bold'>Strekning</label>" + startpunkt + " - " + endepunkt + "</li>";
        container.html(ut);
    })
}


function formaterKjøpsInfo(bestillingInput) {
    const url = "Bestilling/HentPris?" + hentId();
    $.get(url, function (totalpris) {
        let ut = "<table class='table'><thead><tr>";
        ut += "<th>#</th><th>Strekning</th><th>Billettype</th>" +
            "<th>Antall</th></tr></thead>" +
            "<tbody>" +
            "<tr><th>1</th>" +
            "<td>" + bestillingInput.startpunkt + " - " + bestillingInput.endepunkt + "</td>" +
            "<td>Økonomibillett</td> " +
            "<td>" + bestillingInput.antallVoksne + " voksne + " + bestillingInput.antallBarn + " barn</td>" +
            "</tr>"; //TODO: reisePris ikke i kontroller

        if (bestillingInput.hjemreiseTid) {
            ut += "<tr><th>2</th>" +
                "<td>" + bestillingInput.endepunkt + " - " + bestillingInput.startpunkt + "</td>" +
                "<td>Økonomibillett</td>" +
                "<td>" + bestillingInput.antallVoksne + " voksne + " + bestillingInput.antallBarn + " barn</td>" +
                "</tr>"; //TODO: reisePris ikke i kontroller
        }
      
        ut += "<tr><td></td><td></td><th>Totalpris</th><td>" + totalpris + " NOK </td>";
        ut += "</tbody</table>";
        $("#kjøpsInfo").html(ut);
    });

 
}