using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifAlani.Core.Models;

namespace TeklifAlani.Core.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Logo { get; set; }

        public string CompanyName { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        [DataType(DataType.EmailAddress)]
        public string ShipmentEmail { get; set; }

        public string Address { get; set; }

        public string TaxNumber { get; set; }  // VergiNo string olarak değiştirildi

        public string TC { get; set; }  // TC de string olarak değiştirildi

        public int DistrictId { get; set; }

        public District? District { get; set; }

        public List<Supplier> Suppliers { get; set; }

    }
}
