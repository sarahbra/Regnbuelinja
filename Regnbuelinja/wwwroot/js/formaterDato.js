function formaterDato(dato) {
    const nyDato = new Date(dato);

    //Kan lage en egen funksjon med tider også hvis det trengs til bestilling.html
    const dateString = ('0' + nyDato.getDate()).slice(-2) + '/'
        + ('0' + (nyDato.getMonth() + 1)).slice(-2) + '/'
        + nyDato.getFullYear();
  
    return dateString;
      
}
