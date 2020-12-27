namespace DAL.Entity
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class ApplicationContact : DbContext
    {
        public ApplicationContact()
            : base("name=ApplicationContact")
        { }
        public DbSet<Contact> Contacts { get; set; }
    }
}