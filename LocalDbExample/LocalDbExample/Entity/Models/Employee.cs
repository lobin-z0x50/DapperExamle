using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalDbExample.Entity.Models
{
    /// <summary>
    /// 従業員
    /// </summary>
    [Table("Employees")]
    public class Employee
    {
        [Key]
        [Column("id")]
        public virtual int Id { get; set; }

        [Column("name")]
        public virtual string Name { get; set; }

        [Column("department_id")]
        public virtual int DepartmentId { get; set; }

        [Column("updated_at")]
        public virtual DateTime UpdatedAt { get; set; }

        /// <summary>
        ///  コンストラクタ
        /// </summary>
        public Employee()
        {
            UpdatedAt = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Employee {{ Id:{Id}, Name:{Name}, DepartmentId:{DepartmentId}, UpdatedAt{UpdatedAt} }}";
        }
    }
}
