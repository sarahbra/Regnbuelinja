
//Vi velger å avgrense besstillingsdatoer til én måned for å matche db.

$("#AvreiseDato,#HjemreiseDato").datepicker({
    format: "dd/mm/yyyy",
    container: "body",
    todayHighlight: true,
    autoclose: true,
    startDate: "01/12/2021",
    endDate: "31/12/2021",
    defaultViewDate: "01/12/2021",
});

$("input[type=radio][name=TurRetur]").change(function () {
    var tilbakeContainer = $("#TilbakeContainer");
    if (this.value === "true") {
        tilbakeContainer.removeClass("hidden");
    } else if (this.value === "false") {
        tilbakeContainer.addClass("hidden");
    }
});

$("#orderForm"  ).submit(function (event) {
    event.preventDefault();
    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/POST
    /*
      POST /test HTTP/1.1
      Host: foo.example
      Content-Type: application/x-www-form-urlencoded
      Content-Length: 27
  
      field1=value1&field2=value2
    */

    //Serialiserer til formatet ovenfor. Slipper å hente ut data på "vanlig måte"
    $.post("/Bestilling/LagreBestilling", $(this).serialize(), function (data) {
        //Gå til neste side med kunderegigistrering
    });
});


//TODO: Hente havner fra backend!
