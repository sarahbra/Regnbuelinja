function formaterDato(datoer) {
    let datoArray = [];
    for (const dato of datoer) {
        const formatYmd = dato => dato.toISOString().slice(0, 10);
       
         // adjust 0 before single digit date

        let dag = ("0" + dato.getDate()).slice(-2);

        // current month

        let maaned = ("0" + (dato.getMonth() + 1)).slice(-2);

        // current year

        let aar = dato.getFullYear();

        // current hours
        let hours = date_ob.getHours();

        // current minutes
        let minutes = date_ob.getMinutes();

        // current seconds
        let seconds = date_ob.getSeconds();

        // prints date & time in YYYY-MM-DD HH:MM:SS format

        datoArray.push(dag + "/" + maaned + "/" + aar);
        console.log(datoArray);
    }
    
}
