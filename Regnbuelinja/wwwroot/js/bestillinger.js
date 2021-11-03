$(function () {
    const id = window.location.search.substring(1);
    const url = "Bestilling/HentAlleBestillingerForKunde?"+ id;
    $.get(url, function (bestillinger) {
        formaterSide(bestillinger);
    }).fail(function (request) {
        $("#feil").html(request.responseText);
    });
});

function formaterSide(bestillinger) {
    let ut = "<table table-responsive><thead><tr><th>Bestilling</th><th>Kunde</th><th>Totalpris</th><th>Betalt</th></tr></thead>";
    for (let bestilling of bestillinger) {
        ut += "<tr><td><a href='/bestilling.html?Id=" + bestilling.id + "'>" + bestilling.Id + "</a></td>" +
            "<td>" + parseFloat(bestilling.totalpris).toFixed(2) + "</td>";
        ut += "<td>" + bestilling.kid + "</td>";
        if (bestilling.betalt) {
            ut += "<td>Ja</td>";
        } else {
            ut += "<td>Nei</td>";
        }
    }
    $("#bestillinger").html(ut);
}


