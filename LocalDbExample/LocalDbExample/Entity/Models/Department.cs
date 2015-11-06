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
    /// 部署
    /// @auther W.Maeda w.maeda@neogenia.co.jp
    /// </summary>
    [Table("Departments")]
    public class Department
    {
        [Key]
        public int ID { get; set; }

        public string NAME { get; set; }

        public DateTime UPDATED_AT { get; set; }

        /// <summary>
        ///  コンストラクタ
        /// </summary>
        public Department()
        {
            UPDATED_AT = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Department {{ Id:{ID}, Name:{NAME}, UpdatedAt:{UPDATED_AT} }}";
        }
    }
}
