﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Regnbuelinja.DAL
{
    public class Bestilling
    {
        public int BeId { get; set; }
        public virtual Kunde Kunde { get; set; }
        public double TotalPris { get; set; }
        public virtual List<Billett> Billetter { get; set; }
    } 

    public class Billett
    {
        [Key]
        public int BiId { get; set; }
        public virtual Ferd Ferd { get; set; }
        public bool Voksen { get; set; }
    }
    public class Ferd
    {
        [Key]
        public int FId { get; set; }
        public virtual Båt Båt { get; set; }
        public virtual Rute Rute { get; set; }
        public string Dato { get; set; }
    }
    public class Båt
    {
        [Key]
        public string BId { get; set; }
        public string Navn { get; set; }
    }
    public class Rute
    {
        [Key]
        public int RId { get; set; }
        public string startpunkt { get; set; }
        public string endepunkt { get; set; }
        public double pris { get; set; }
        
    }
    public class Kunde
    {
        [Key]
        public int KId { get; set; }
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public string Adresse { get; set; }
        public virtual Poststeder Poststed { get; set; }
    }

    public class Poststeder
    {
        [Key]
        public string Postnr { get; set; }
        public string Poststed { get; set; }
    }
    public class BestillingContext : DbContext
    {
        public BestillingContext(DbContextOptions<BestillingContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Poststeder> Poststeder { get; set; }
        public DbSet<Kunde> Kunder { get; set; }
        public DbSet<Rute> Ruter { get; set; }
        public DbSet<Ferd> Ferder { get; set; }
        public DbSet<Båt> Båter { get; set; }
        public DbSet<Bestilling> Bestillinger { get; set; }
        public DbSet<Billett> Billetter { get; set; }


    }
}
