$(function () {
    const id = window.location.search.substring(1);
    const url = "Bestilling/HentBestilling?" + id;
    $.get(url, function (bestillingInput) {
        formaterBestilling(bestillingInput);
    });
})

function formaterBestilling(bestillingInput) {
    $("#utreiseHeader").val("Utreise fra " + bestillingInput.startpunkt);
    $("#dato").val(bestillingInput.avreiseDato);
    $("#skip").val("Båtten Anna");  //TODO - Add båtnavn
    $("#strekning").val(bestillingInput.startpunkt + " - " + bestillingInput.endepunkt);
}