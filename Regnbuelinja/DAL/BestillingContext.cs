using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Regnbuelinja.DAL
{
    public class Bestillinger
    {
        [Key]
        public int Id { get; set; }
        public virtual List<Billett> Billetter { get; set; } = new List<Billett>();
        public bool Betalt { get; set; } = false;
        public virtual Person Kunde {get; set;}
    } 

    public class Person
    {
        public int Id { get; set; }
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public string Telefonnr { get; set; }
        public string Epost { get; set; }
        public virtual List<Bestillinger> Bestillinger { get; set; } = new List<Bestillinger>();
        // Settes kun i dbInit av sikkerhetsgrunner
        public bool Admin { get; set; } = false;
    }

    public class Billett
    {
        [Key]
        public int Id { get; set; }
        public virtual Ferd Ferd { get; set; }
        public bool Voksen { get; set; }
        public virtual Bestillinger Bestilling { get; set; }
    }

    public class Ferd
    {
        [Key]
        public int Id { get; set; }
        public virtual Baat Baat { get; set; }
        public virtual Rute Rute { get; set; }
        public DateTime AvreiseTid{ get; set; }
        public DateTime AnkomstTid { get; set; }
    }

    public class Baat
    {
        [Key]
        public int Id { get; set; }
        public string Navn { get; set; }
    }

    public class Brukere
    {
        public int Id { get; set; }
        public string Brukernavn { get; set; }
        public byte[] Passord { get; set; }
        public byte[] Salt { get; set; }
        public virtual Person Person { get; set; }
    }

    public class Rute
    {
        [Key]
        public int Id { get; set; }
        public string Startpunkt { get; set; }
        public string Endepunkt { get; set; }
        public double Pris { get; set; }
        
    }

    //Trenger ikke kundeklasse i denne innleveringen
    //public class Kunde
    //{
    //    [Key]
    //    public int KId { get; set; }
    //    public string Fornavn { get; set; }
    //    public string Etternavn { get; set; }
    //    public string Adresse { get; set; }
    //    public virtual Poststeder poststed { get; set; }
    //}

  
    //public class Poststeder
    //{
    //    [Key]
    //    public string Postnr { get; set; }
    //    public string Poststed { get; set; }
    //}


    public class BestillingContext : DbContext
    {
        public BestillingContext(DbContextOptions<BestillingContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Brukere> Brukere { get; set; }
        public DbSet<Rute> Ruter { get; set; }
        public DbSet<Ferd> Ferder { get; set; }
        public DbSet<Baat> Baater { get; set; }
        public DbSet<Bestillinger> Bestillinger { get; set; }
        public DbSet<Billett> Billetter { get; set; }
        public DbSet<Person> KunderOgAnsatte { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

    }
}
