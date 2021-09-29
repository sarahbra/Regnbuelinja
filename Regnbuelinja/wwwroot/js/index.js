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
  hentTilgjengeligeFerdDatoer();
});

function hentAnkomstHavner() {
  const avgangsHavn = $("#Startpunkt").val();
  const url = "Bestilling/HentAnkomsthavner?avgangsHavn=" + avgangsHavn;
  $.get(url, function (havner) {
    visHavner($("#Endepunkt"), havner);
    $("#EndepunktDiv").show();
  });
}

//Trenger ikke to sånne metoder som gjør det samme
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

//Hadde ikke trengt å sortere dato-strengene hvis vi hadde brukt Date backend. Da kunne vi brukt innebygde metoder på Date

function visFerdKalender(datoer) {
  //Setter det samme på hjemreise og avreise. Sånn kan det ikke være
  $("#AvreiseDato,#HjemreiseDato").datepicker({
    format: "dd/mm/yyyy",
    container: "body",
    todayHighlight: true,
    autoclose: true,
    startDate: datoer[0],
    endDate: datoer[datoer.length - 1],
    beforeShowDay: function (date) {
      const gyldig = sorterteDatoer.some(function (d) {
        return d.getTime() === date.getTime();
      });
      return gyldig;
    },
  });
}

function hentTilgjengeligeFerdDatoer() {
    const startPunkt = $("#Startpunkt").val();
    const endePunkt = $("#Endepunkt").val();

    let params = {
        "Startpunkt": startPunkt,
        "Endepunkt": endePunkt,
        "AvreiseDato": null
    };

    console.log(params);

    $.get("Bestilling/HentDatoer", params, function (datoer) {
        let datoFormat = [];
        datoer.forEach(function(dato) {
            datoFormat.push(formaterDato(dato))
        });
        visFerdKalender(datoFormat);

    });
    console.log(datoFormat);
    //visFerdKalender(datoFormat);
  });
  //Todo: Hvis tur/retur er valgt -> Hent datoer basert på startPunkt = endepunkt og endePunkt = startPunkt?
}

// https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/POST
/*
  POST /test HTTP/1.1
  Host: foo.example
  Content-Type: application/x-www-form-urlencoded
  Content-Length: 27

  field1=value1&field2=value2
*/

//Serialiserer til formatet ovenfor. Slipper å hente ut data på "vanlig måte"

//Gammel formaterDato da vi fikk String fra server

/*
    function formaterDato(datoStreng) {
        const deler = datoStreng.split("/").map(function (s) {
            return parseInt(s)
        });
        return new Date(deler[2], deler[1] - 1, deler[0]);
    }

    const sorterteDatoer = datoer.map(formaterDato).sort(function (a, b) {
        return a - b;
    });
    */
