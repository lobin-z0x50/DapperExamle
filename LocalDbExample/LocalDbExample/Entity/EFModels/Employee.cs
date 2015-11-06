namespace LocalDbExample.Entity.EFModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Employee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [StringLength(32)]
        public string name { get; set; }

        public int? department_id { get; set; }

        public DateTime? updated_at { get; set; }
    }
}
