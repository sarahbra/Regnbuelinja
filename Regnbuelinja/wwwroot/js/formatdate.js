function formaterDato(dato) {
    const nyDato = new Date(dato);
    const dag = nyDato.getDate();       
    const maaned = nyDato.getMonth() + 1;
    const aar = nyDato.getFullYear();  
   

  

    const time = dag + "/" + maaned + "/" + aar;
    return time;
      
}
