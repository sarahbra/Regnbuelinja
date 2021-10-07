$(function () {
    const url = "Bestilling/HentBestilling?" + hentId();
    $.get(url, function (bestilling) {
        formaterSide(bestilling);
    }).fail(function (request) {
        $("#feil").html(request.responseText);
    });
});


function formaterSide(bestilling) {
    let url = "Bestilling/HentAnkomstTid?" + hentId() + "&startpunkt=" + bestilling.startpunkt;
    $.get(url, function (ankomstTidSerialized) {
        if (bestilling.hjemreiseTid) {
            url = "Bestilling/HentAnkomstTid?" + hentId() + "&startpunkt=" + bestilling.endepunkt;
            $.get(url, function (ankomstTidReturSerialized) {
                formaterBestilling(bestilling, ankomstTidReturSerialized, true);
            }).fail(function (request) {
                $("#feil").html(request.responseText);
            });
        }
        formaterBestilling(bestilling, ankomstTidSerialized, false);
    }).fail(function (request) {
        $("#feil").html(request.responseText);
    });
    formaterKjøpsInfo(bestilling);
}

function hentId() {
    const url = window.location.search.substring(1);
    return url;
}


function formaterBestilling(bestilling, ankomstTidSerialized, retur) {

    let startpunkt = bestilling.startpunkt;
    let endepunkt = bestilling.endepunkt;
    let avreiseSerialized = bestilling.avreiseTid;

    let container = $("#utreise");
    let tittel = $("#utreiseHeader");
    if (retur) {
        $("#returBilde").removeClass("hidden");
        startpunkt = bestilling.endepunkt;
        endepunkt = bestilling.startpunkt;
        avreiseSerialized = bestilling.hjemreiseTid;
        container = $("#returreise");
        tittel = $("#returreiseHeader");
       
    } else {
        $("#utreiseBilde").removeClass("hidden");
    }

    tittel.html("Avgang fra " + startpunkt);

    const avreiseDate = new Date(avreiseSerialized);
    const ankomstDate = new Date(ankomstTidSerialized);

    let ut = "";
    ut += "<li class='list-group-item'><label for='avreise' class='col-12 col-sm-3 fw-bold'>Avreise</label>" + avreiseDate.toDateString() + " - " + avreiseDate.toLocaleTimeString() + "</li>" +
        "<li class='list-group-item'><label for='ankomst' class='col-12 col-sm-3 fw-bold'>Ankomst</label>" + ankomstDate.toDateString() + " - " + ankomstDate.toLocaleTimeString() + "</li>" +
        "<li class='list-group-item'><label for='strekning' class='col-12 col-sm-3 fw-bold'>Strekning</label>" + startpunkt + " - " + endepunkt + "</li>";
    container.html(ut);

    const url = "Bestilling/HentBaat?" + hentId() + "&startpunkt=" + startpunkt;
    $.get(url, function (baatnavn) {
        container.append("<li class='list-group-item'><label for='skip' class='col-12 col-sm-3 fw-bold'>Skip</label>" + baatnavn + "</li>");
    }).fail(function (request) {
        $("#feil").html(request.responseText);
    });
}


function formaterKjøpsInfo(bestilling) {
    const url = "Bestilling/HentPris?" + hentId();
    $.get(url, function (totalpris) {
        let ut = "<table class='table'><thead><tr>";
        ut += "<th>#</th><th>Strekning</th><th>Billettype</th>" +
            "<th>Antall</th></tr></thead>" +
            "<tbody>" +
            "<tr><th>1</th>" +
            "<td>" + bestilling.startpunkt + " - " + bestilling.endepunkt + "</td>" +
            "<td>Økonomibillett</td> " +
            "<td>" + bestilling.antallVoksne + " voksne + " + bestilling.antallBarn + " barn</td>" +
            "</tr>";

        if (bestilling.hjemreiseTid) {
            ut += "<tr><th>2</th>" +
                "<td>" + bestilling.endepunkt + " - " + bestilling.startpunkt + "</td>" +
                "<td>Økonomibillett</td>" +
                "<td>" + bestilling.antallVoksne + " voksne + " + bestilling.antallBarn + " barn</td>" +
                "</tr>";
        }

        ut += "<tr><td></td><td></td><th>Totalpris</th><td>" + parseFloat(totalpris).toFixed(2) + " NOK </td>";
        ut += "</tbody</table>";
        $("#kjøpsInfo").html(ut);
    }).fail(function (request) {
        $("#feil").html(request.responseText);
    });
 
}