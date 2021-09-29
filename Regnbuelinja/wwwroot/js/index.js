
$("input[type=radio][name=TurRetur]").change(function () {
    var hjemreiseDato = $("#HjemreiseDato");
    var tilbakeContainer = $("#TilbakeContainer");
    if (this.value === "true") {
        tilbakeContainer.removeClass("hidden");
        hjemreiseDato.attr("required", true);
        //Formater hjemreiseKalender
        hentTilgjengeligeFerdDatoerHjemreise();
    } else if (this.value === "false") {
        tilbakeContainer.addClass("hidden");
        hjemreiseDato.attr("required", false);
    }
});

$("#orderForm").submit(function (event) {
    event.preventDefault();
    var form = event.target;
    var valid = form.checkValidity();
    $(form).addClass('was-validated');
    if (!valid) {
        return false
    }
    $.post("Bestilling/LagreBestilling", $(this).serialize(), function (data) {
        window.location = "https://localhost:44392/bestilling.html?id=" + data;
        //Gå til neste side med billettinfo
    })
        .fail(function () {
            //Kan eventuelt hente ut feilmelding fra server her hvis vi vil etter at det er implementert

            $("#feil").html("Feil på server - prøv igjen senere");
        });
});

<<<<<<< HEAD
<<<<<<< Updated upstream

function formaterAvgangsHavner(havner) {
    let ut = "";
    for (let havn of havner) {
        ut += "<option value='" + havn + "'>" + havn + "</option>";
    }
=======

$("#Startpunkt").change(function () {
    hentAnkomstHavner();
});

$("#Endepunkt").change(function () {
    hentTilgjengeligeFerdDatoerAvreise();
});
>>>>>>> Stashed changes
=======
$("#Startpunkt").change(function () {
    hentAnkomstHavner();
});

$("#Endepunkt").change(function () {
    hentTilgjengeligeFerdDatoer();
});
>>>>>>> master


function hentAnkomstHavner() {
    const avgangsHavn = $("#Startpunkt").val();
    const url = "Bestilling/HentAnkomsthavner?avgangsHavn=" + avgangsHavn;
    $.get(url, function (havner) {
        visHavner($("#Endepunkt"), havner);
        $("#EndepunktDiv").show();
    });
}

<<<<<<< HEAD
<<<<<<< Updated upstream
function formaterAnkomstHavner(havner) {
    let ut = "<label for='Endepunkt'>Hvor vil du reise til?</label>";
    ut += "<select name='Endepunkt' class='form-control' id='Endepunkt'>";
   
=======
//Trenger ikke to sånne metoder som gjør det samme
function visHavner(selectBox, havner) {
    selectBox.empty();
    selectBox.append('<option value="" disabled selected>Velg havn</option>');
>>>>>>> master
    for (let havn of havner) {
        selectBox.append('<option value="' + havn + '">' + havn + '</option>');
    }
}

$.get("Bestilling/HentAvgangshavner", function (havner) {
    visHavner($("#Startpunkt"), havner);
});


//Hadde ikke trengt å sortere dato-strengene hvis vi hadde brukt Date backend. Da kunne vi brukt innebygde metoder på Date

function visFerdKalender(datoer) {
    function formaterDato(datoStreng) {
        const deler = datoStreng.split("/").map(function (s) {
            return parseInt(s)
        });
        return new Date(deler[2], deler[1] - 1, deler[0]);
    }

<<<<<<< HEAD
    ut += "</select>";
    $("#EndepunktDiv").html(ut);
=======
//Trenger ikke to sånne metoder som gjør det samme har derfor endret til én metode som tar inn selectBox
function visHavner(selectBox, havner) {
    selectBox.empty();
    selectBox.append('<option value="" disabled selected>Velg havn</option>');
    for (let havn of havner) {
        selectBox.append('<option value="' + havn + '">' + havn + '</option>');
    }
=======
    const sorterteDatoer = datoer.map(formaterDato).sort(function (a, b) {
        return a - b;
    });

    //Setter det samme på hjemreise og avreise. Sånn kan det ikke være

    $("#AvreiseDato,#HjemreiseDato").datepicker({
        format: "dd/mm/yyyy",
        container: "body",
        todayHighlight: true,
        autoclose: true,
        startDate: sorterteDatoer.slice(0).shift(),
        endDate: sorterteDatoer.slice(0).pop(),
        beforeShowDay: function (date) {
            const gyldig = sorterteDatoer.some(function (d) {
                return d.getTime() === date.getTime();
            })
            return gyldig;
        }
    });
}



function hentTilgjengeligeFerdDatoer() {
    const startPunkt = $("#Startpunkt").val();
    const endePunkt = $("#Endepunkt").val();

    $.get("Bestilling/HentRuter?nyttStartPunkt=" + startPunkt, function (startRuter) {
        const ruter = startRuter.filter(function (startRute) {
            return startRute.endepunkt === endePunkt;
        });
        Promise.all(ruter.map(function (rute) {
            return $.get("Bestilling/HentFerder?ruteId=" + rute.rId);
        })).then(function (results) {
            const datoer = [];
            for (const result of results) {
                for (const ferd of result) {
                    datoer.push(ferd.dato);
                }
            }
            visFerdKalender(datoer);
        });
    });

    //Todo: Hvis tur/retur er valgt -> Hent datoer basert på startPunkt = endepunkt og endePunkt = startPunkt?
>>>>>>> master
}

$.get("Bestilling/HentAvgangshavner", function (havner) {
    visHavner($("#Startpunkt"), havner);
});


//Hadde ikke trengt å sortere dato-strengene hvis vi hadde brukt Date backend. Da kunne vi brukt innebygde metoder på Date
function visFerdKalenderAvreise(datoer) {
    function formaterDato(datoStreng) {
        const deler = datoStreng.split("/").map(function (s) {
            return parseInt(s)
        });
        return new Date(deler[2], deler[1] - 1, deler[0]);
    }

    const sorterteDatoer = datoer.map(formaterDato).sort(function (a, b) {
        return a - b;
    });

    //Lager avreise kalender
    $("#AvreiseDato").datepicker({
        format: "dd/mm/yyyy",
        container: "body",
        todayHighlight: true,
        autoclose: true,
        startDate: sorterteDatoer.slice(0).shift(),
        endDate: sorterteDatoer.slice(0).pop(),
        beforeShowDay: function (date) {
            const gyldig = sorterteDatoer.some(function (d) {
                return d.getTime() === date.getTime();
            })
            return gyldig;
        }
    });
}

function visFerdKalenderHjemreise(datoer) {
    function formaterDato(datoStreng) {
        const deler = datoStreng.split("/").map(function (s) {
            return parseInt(s)
        });
        return new Date(deler[2], deler[1] - 1, deler[0]);
    }

    const sorterteDatoer = datoer.map(formaterDato).sort(function (a, b) {
        return a - b;
    });

    console.log(sorterteDatoer);

    $("#HjemreiseDato").datepicker({
        format: "dd/mm/yyyy",
        container: "body",
        todayHighlight: true,
        autoclose: true,
        startDate: sorterteDatoer.slice(0).shift(),
        endDate: sorterteDatoer.slice(0).pop(),
        beforeShowDay: function (date) {
            const gyldig = sorterteDatoer.some(function (d) {
                return d.getTime() === date.getTime();
            })
            return gyldig;
        }
    });
}



function hentTilgjengeligeFerdDatoerAvreise() {
    const startPunkt = $("#Startpunkt").val();
    const endePunkt = $("#Endepunkt").val();

    $.get("Bestilling/HentRuter?nyttStartPunkt=" + startPunkt, function (startRuter) {
        const ruter = startRuter.filter(function (startRute) {
            return startRute.endepunkt === endePunkt;
        });
        Promise.all(ruter.map(function (rute) {
            return $.get("Bestilling/HentFerder?ruteId=" + rute.rId);
        })).then(function (results) {
            const datoer = [];
            for (const result of results) {
                for (const ferd of result) {
                    datoer.push(ferd.dato);
                }
            }
            visFerdKalenderAvreise(datoer);
        });
    });
>>>>>>> Stashed changes
}

    function hentTilgjengeligeFerdDatoerHjemreise() {
        //I vår datamodell har vi ingen egen tabell for returreise, kun boolean tur/retur på en Rute.
        //Derfor blir startPunkt = endepunkt og endePunkt = startPunkt

        const startPunkt = $("#Endepunkt").val();
        const endePunkt = $("#Startpunkt").val();

        $.get("Bestilling/HentRuter?nyttStartPunkt=" + startPunkt, function (startRuter) {
            const ruter = startRuter.filter(function (startRute) {
                return startRute.endepunkt === endePunkt;
            });
            Promise.all(ruter.map(function (rute) {
                return $.get("Bestilling/HentFerder?ruteId=" + rute.rId);
            })).then(function (results) {
                const datoer = [];
                for (const result of results) {
                    for (const ferd of result) {
                        datoer.push(ferd.dato);
                    }
                }
                visFerdKalenderHjemreise(datoer);
            });
        });
    }

//Opprette en funksjon som henter datoer basert på startPunkt og endePunkt og gjøre om alle metoder henter dette.








// https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/POST
/*
  POST /test HTTP/1.1
  Host: foo.example
  Content-Type: application/x-www-form-urlencoded
  Content-Length: 27

  field1=value1&field2=value2
*/

//Serialiserer til formatet ovenfor. Slipper å hente ut data på "vanlig måte"
