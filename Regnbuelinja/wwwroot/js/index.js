$("input[type=radio][name=TurRetur]").change(function () {
    var hjemreiseDato = $("#HjemreiseDato");
    var tilbakeContainer = $("#TilbakeContainer");
    if (this.value === "true") {
        tilbakeContainer.removeClass("hidden");
        hjemreiseDato.attr("required", true);
    } else if (this.value === "false") {
        tilbakeContainer.addClass("hidden");
        hjemreiseDato.attr("required", false);
    }
});

$("#orderForm").submit(function (event) {
    event.preventDefault();
    var form = event.target;
    var valid = form.checkValidity();
    $(form).addClass("was-validated");
    if (!valid) {
        return false;
    }

    const input = {
        Startpunkt: $("#Startpunkt").val(),
        Endepunkt: $("#Endepunkt").val(),
        AvreiseTid: $("#AvreiseDato").val(),
        HjemreiseTid: $("#HjemreiseDato").val(),
        AntallVoksne: $('input[type=number][name=AntallVoksne]').val(),
        AntallBarn: $('input[type=number][name=AntallBarn]').val()
    }
    $.post("Bestilling/LagreBestilling", input, function (data) {
        window.location = "https://localhost:44392/bestilling.html?id=" + data;
        //Gå til neste side med billettinfo 
    }).fail(function () {
        //Kan eventuelt hente ut feilmelding fra server her hvis vi vil etter at det er implementert
        $("#feil").html("Feil på server - prøv igjen senere");
    });
});

$("#Startpunkt").change(function () {
    hentAnkomstHavner();
});

$("#Endepunkt").change(function () {
    hentTilgjengeligeFerdDatoerAvreise();
});

function hentAnkomstHavner() {
    const avgangsHavn = $("#Startpunkt").val();
    const url = "Bestilling/HentAnkomsthavner?avgangsHavn=" + avgangsHavn;
    $.get(url, function (havner) {
        visHavner($("#Endepunkt"), havner);
        $("#EndepunktDiv").show();
    });
}

function visHavner(selectBox, havner) {
    selectBox.empty();
    selectBox.append('<option value="" disabled selected>Velg havn</option>');
    for (const havn of havner) {
        selectBox.append('<option value="' + havn + '">' + havn + "</option>");
    }
}

$.get("Bestilling/HentAvgangshavner", function (havner) {
    visHavner($("#Startpunkt"), havner);
});

function visKalender(kalender, datoer) {
    if (kalender.data().datepicker) {
        kalender.data().datepicker.destroy();
    }
    kalender.datepicker({
        format: "dd/mm/yyyy",
        container: "body",
        todayHighlight: true,
        autoclose: true,
        startDate: datoer[0],
        endDate: datoer[datoer.length - 1],
        beforeShowDay: function (date) {
            // Return true dersom date skal kunne velges
            return datoer.some(d => d.getTime() === date.getTime());
        },
    });
}

function formatterKalenderDato(str) {
    const deler = str.split("/");
    return new Date(parseInt(deler[2]), parseInt(deler[1]) - 1, parseInt(deler[0]));
}

function hentTilgjengeligeFerdDatoerAvreise() {
    const startPunkt = $("#Startpunkt").val();
    const endePunkt = $("#Endepunkt").val();

    const params = {
        Startpunkt: startPunkt,
        Endepunkt: endePunkt,
        AvreiseDato: null,
    };

    $.get("Bestilling/HentDatoer", params, function (datoer) {
        visKalender($("#AvreiseDato"), datoer.map(function (d) {
            return new Date(d);
        }));
    });
}

//Hvis avreisedato er valgt OG tur/retur er valgt så hentes tilgjengelige hjemreiseDatoer
$("#AvreiseDato").change(function () {
    $("#HjemreiseDato").val("");
    if ($("#TurReturTrue").is(":checked")) {
        hentTilgjengeligeFerdDatoerHjemreise();
    }
});

//Henter tilgjengelige hjemreisedatoer basert på avreisedato

function hentTilgjengeligeFerdDatoerHjemreise() {
    const startPunkt = $("#Endepunkt").val();
    const endePunkt = $("#Startpunkt").val();
    const avreiseDato = formatterKalenderDato($("#AvreiseDato").val());

    const dato = new Date(avreiseDato);
    const avreiseDatoISOStr = avreiseDato.toISOString();

    const params = {
        Startpunkt: startPunkt,
        Endepunkt: endePunkt,
        AvreiseDato: avreiseDatoISOStr,
    };

    $.get("Bestilling/HentDatoer", params, function (datoer) {
        visKalender($("#HjemreiseDato"), datoer.map(function (d) {
            return new Date(d);
        }));
    });
}
