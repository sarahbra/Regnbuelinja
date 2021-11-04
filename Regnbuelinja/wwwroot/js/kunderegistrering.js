$("#kundeForm").submit(function (event) {
    event.preventDefault();
    const valid = form.checkValidity();
    $(form).addClass("was-validated");
    if (!valid) {
        return false;
    }

    const kunde = {
        fornavn: $("#fornavn").val(),
        etternavn: $("#etternavn").val(),
        epost: $("#epost").val(),
        telefonnr: $("#telefonnr").val()
    };

    if (validerKunde(kunde)) {
        $.post("Bestilling/LagreKunde", kunde, function (id) {
            const bid = hentId();
            const params = new URLSearchParams();
            params.set("KId", id);
            params.set("BId", bid);
            $.post("Bestilling/LeggKundeTilBestilling", params.toString(), function (ok) {
                window.location.href = "bestilling.html?Id=" + bid;
            }).fail(function (request) {
                $("#feil").html(request.responseText);
            });
        }).fail(function (request) {
            $("#feil").html(request.responseText);
        });
    }
})

function hentId() {
    const params = window.location.search.substring(1).split("=");
    return decodeURIComponent(params[1]);
}

function loggInn() {
    window.location.href = "loggInn.html?Id=" + hentId();
}