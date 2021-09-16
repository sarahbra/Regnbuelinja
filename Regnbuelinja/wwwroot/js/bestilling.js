$(function () {
    const id = window.location.search.substring(1);
    const url = "Bestilling/HentBestilling?" + id;
    $.get(url, function (bestilling) {
        formaterBestilling(bestilling);
    });
})

function formaterBestilling(bestilling) {
    ut = "Hei, vindu, se jeg finnes!";

    $("#bestillingen").html(ut)
}