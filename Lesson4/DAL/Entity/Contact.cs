using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entity
{
  public  class Contact
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public  string  Name  { get; set; }
        [Required]
        [MaxLength(10)]
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
