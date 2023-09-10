using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbExample.Entity.Common
{
    public interface IEntity
    {
        string CreatedBy { get; set; }

        DateTime CreatedOn { get; set; }

        string UpdatedBy { get; set; }

        DateTime UpdatedOn { get; set; }
    }
}
