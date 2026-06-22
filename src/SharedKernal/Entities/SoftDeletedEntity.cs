using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernal.Entities
{
    public class SoftDeletedEntity : BaseEntity
    {
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedDate { get; set; }
    }
}
