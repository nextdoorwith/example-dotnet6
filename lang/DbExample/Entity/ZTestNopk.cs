using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DbExample.Entity;

[Keyless]
[Table("z_test_nopk")]
public partial class ZTestNopk
{
    [Required]
    [Column("desc1")]
    [StringLength(256)]
    public string Desc1 { get; set; }

    [Required]
    [Column("desc2")]
    [StringLength(256)]
    public string Desc2 { get; set; }
}
