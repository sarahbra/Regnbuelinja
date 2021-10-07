

$("input[type=radio][name=TurRetur]").change(function () {
    const hjemreiseDato = $("#HjemreiseDato");
    const tilbakeContainer = $("#TilbakeContainer");
    if (this.value === "true" && $("#Startpunkt").val() && $("#Endepunkt").val()) {
        tilbakeContainer.removeClass("hidden");
        hjemreiseDato.attr("required", true);
        if ($("#AvreiseDato").val()) {
            hentTilgjengeligeFerdDatoerHjemreise();
        }
    } else if (this.value === "false") {
        tilbakeContainer.addClass("hidden");
        hjemreiseDato.attr("required", false);
        $("#returDatoFinnesIkke").html("");
    }
});

$("#orderForm").submit(function (event) {
    event.preventDefault();
    const form = event.target;
    const valid = form.checkValidity();
    $(form).addClass("was-validated");
    if (!valid) {
        return false;
    }

    const params = new URLSearchParams();
    params.set("Startpunkt", $("#Startpunkt").val());
    params.set("Endepunkt", $("#Endepunkt").val());
    params.set("TurRetur", $("#TurRetur").val());
    params.set("AvreiseTid", formaterKalenderDato($("#AvreiseDato").val()).toISOString());
    if ($("#TurReturTrue").is(":checked")) {
        params.set("HjemreiseTid", formaterKalenderDato($("#HjemreiseDato").val()).toISOString());
    }
    params.set("AntallVoksne", $("#AntallVoksne").val());
    const antallBarn = ($("#AntallBarn").val() || "").trim();
    if (antallBarn.length) {
        params.set("AntallBarn", antallBarn);
    }

    $.post("Bestilling/LagreBestilling", params.toString(), function (id) {
             //Gå til neste side med billettinfo

        window.location.assign("/bestilling.html?id=" + id);
    }).fail(function (jqXHR) {
        $("#feil").html("Feil på server - prøv igjen senere: " + jqXHR.responseText);
        return false;
    });
});


$("#Startpunkt").change(function () {
    nullstillKalender($("#AvreiseDato,#HjemreiseDato"));
    $("#TilbakeContainer").addClass("hidden");
    $("#AvreiseContainer").addClass("hidden");
    hentAnkomstHavner();
});

$("#Endepunkt").change(function () {
    nullstillKalender($("#AvreiseDato,#HjemreiseDato"));
    hentTilgjengeligeFerdDatoerAvreise();
    const avreiseContainer = $("#AvreiseContainer");
    avreiseContainer.removeClass("hidden");
});

function hentAnkomstHavner() {
    const avgangsHavn = $("#Startpunkt").val();
    const url = "Bestilling/HentAnkomsthavner?avgangsHavn=" + avgangsHavn;
    $.get(url, function (havner) {
        visHavner($("#Endepunkt"), havner);
        $("#EndepunktDiv").removeClass("hidden");
        $("#Endepunkt").attr("required", true);
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

function nullstillKalender(kalender) {
    kalender.val("");
    if (kalender.data().datepicker) {
        kalender.data().datepicker.destroy();
    }
}

function visKalender(kalender, datoer) {
    nullstillKalender(kalender);
    kalender.datepicker({
        format: "dd/mm/yyyy",
        container: "body",
        todayHighlight: true,
        autoclose: true,
        startDate: datoer[0],
        endDate: datoer[datoer.length - 1],
         // Returnerer true dersom date skal kunne velges
        beforeShowDay: function (date) {
            return datoer.some(d => d.getTime() === date.getTime());
        },
    });
}

function formaterKalenderDato(str) {
    const deler = str.split("/");
    return new Date(parseInt(deler[2]), (parseInt(deler[1])-1), parseInt(deler[0]));
}

function hentTilgjengeligeFerdDatoerAvreise() {
    
    const startPunkt = $("#Startpunkt").val();
    const endePunkt = $("#Endepunkt").val();

    const params = {
        Startpunkt: startPunkt,
        Endepunkt: endePunkt,
        AvreiseTid: null,
    };

    $.get("Bestilling/HentDatoer", params, function (datoer) {
        visKalender($("#AvreiseDato"), datoer.map(function (d) {
            return new Date(d);
        }));
    }).fail(function (jqXHR) {
        const json = $.parseJSON(jqXHR.responseText);
        $("#feil").html(json.message);
        nullstillKalender($("#AvreiseDato, #HjemreiseDato"));
        return;
    });
}


$("#AvreiseDato").change(function () {
    if ($("#TurReturTrue").is(":checked")) {
        nullstillKalender($("#HjemreiseDato"));
        $("#feil").html("");
        hentTilgjengeligeFerdDatoerHjemreise();
    }
});

//Henter tilgjengelige hjemreisedatoer basert på avreisedato

function hentTilgjengeligeFerdDatoerHjemreise() {
    //Hvis tur/retur = true så vil startpunkt og endepunkt være motsatt ved hjemreise
    $("#TilbakeContainer").removeClass("hidden");
    $("#HjemreiseDato").attr("required", true);
    $("#returDatoFinnesIkke").html("");

    const startPunkt = $("#Endepunkt").val();
    const endePunkt = $("#Startpunkt").val();
    const avreiseFormatert = $("#AvreiseDato").val();

    if (avreiseFormatert === null) {
        return;
    }

    const avreiseDato = formaterKalenderDato(avreiseFormatert);
    const avreiseDatoISOStr = avreiseDato.toISOString();

    const params = {
        Startpunkt: startPunkt,
        Endepunkt: endePunkt,
        AvreiseTid: avreiseDatoISOStr,
    };

    $.get("Bestilling/HentDatoer", params, function (datoer) {
        visKalender($("#HjemreiseDato"), datoer.map(function (d) {
            return new Date(d);
        }));
    }).fail(function (request) {
        $("#returDatoFinnesIkke").html(request.responseText);
        nullstillKalender($("#AvreiseDato, #HjemreiseDato"));
        $("#TilbakeContainer").addClass("hidden");
        $("#hjemreiseDato").attr("required", false);
        hentTilgjengeligeFerdDatoerAvreise();
        return;
    });
}