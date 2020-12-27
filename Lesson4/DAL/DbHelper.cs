using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL
{

    public class DBHelper
    {
        private ApplicationContact Context;
        //CRUD
        public List<string> contactsName = new List<string>();

        public DBHelper()
        {
            Context = new ApplicationContact();
        }
        public void AddContact(Contact contact)
        {
            Context.Contacts.Add(contact);
            Context.SaveChanges();
        }
        public List<string> GetName()
        {
            return (from x in Context.Contacts
                    select x.Name)
                    .ToList();
        }
        public Contact GetContact(int index)
        {
            return Context.Contacts.ToArray()[index];
        }
        public void DeleteContact(Contact c)
        {
            Context.Contacts.Remove(c);
            Context.SaveChanges();
        }

    }
}
