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
    $.post("Bestilling/LagreBestilling", $(this).serialize(), function (data) {
        //console.log($(this))
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
    for (let havn of havner) {
        selectBox.append('<option value="' + havn + '">' + havn + "</option>");
    }
}

$.get("Bestilling/HentAvgangshavner", function (havner) {
    visHavner($("#Startpunkt"), havner);
});

function visFerdKalenderAvreise(datoer) {
    $("#AvreiseDato").datepicker({
        format: "dd/mm/yyyy",
        container: "body",
        todayHighlight: true,
        autoclose: true,
        startDate: datoer[0],
        endDate: datoer[datoer.length - 1],
        beforeShowDay: function (date) {
            return date;
        },
    });
}

function visFerdKalenderHjemreise(datoer) {
    $("#HjemreiseDato").datepicker({
        format: "dd/mm/yyyy",
        container: "body",
        todayHighlight: true,
        autoclose: true,
        startDate: datoer[0],
        endDate: datoer[datoer.length - 1],
        beforeShowDay: function (date) {
            return date;
        },
    });
}

function hentTilgjengeligeFerdDatoerAvreise() {
    const startPunkt = $("#Startpunkt").val();
    const endePunkt = $("#Endepunkt").val();

    let params = {
        Startpunkt: startPunkt,
        Endepunkt: endePunkt,
        AvreiseDato: null,
    };

    $.get("Bestilling/HentDatoer", params, function (datoer) {
        let formaterteDatoer = [];

        datoer.forEach(function (dato) {
            formaterteDatoer.push(formaterDato(dato));
        });
        visFerdKalenderAvreise(formaterteDatoer);
    });
}

//Hvis avreisedato er valgt OG tur/retur er valgt
$("#AvreiseDato").change(function () {
    $("#HjemreiseDato").val("");
    if ($("#TurReturTrue").is(":checked")) {
        alert("it's checked");
        //Denne funker første gangen, men hvis man endrer avreisedato så blir den ikke trigget igjen 
        hentTilgjengeligeFerdDatoerHjemreise();
    }
});

//Hent tilgjengelige hjemreisedatoer basert på avreisedato

function hentTilgjengeligeFerdDatoerHjemreise() {
    const startPunkt = $("#Endepunkt").val();
    const endePunkt = $("#Startpunkt").val();
    const avreiseDato = $("#AvreiseDato").data().datepicker.viewDate;
    console.log(avreiseDato);

    const dato = new Date(avreiseDato);
    const avreiseDatoISOStr = avreiseDato.toISOString();

    console.log(avreiseDatoISOStr);

    let params = {
        Startpunkt: startPunkt,
        Endepunkt: endePunkt,
        AvreiseDato: avreiseDatoISOStr,
    };

    $.get("Bestilling/HentDatoer", params, function (datoer) {
        let formaterteDatoer = [];
        datoer.forEach(function (dato) {
            formaterteDatoer.push(formaterDato(dato));
        });
        console.log(formaterteDatoer);
        visFerdKalenderHjemreise(formaterteDatoer);
    });
}
