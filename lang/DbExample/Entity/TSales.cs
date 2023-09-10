using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DbExample.Entity;

[PrimaryKey("RegionId", "Year", "Month")]
[Table("t_sales")]
public partial class TSales
{
    [Key]
    [Column("region_id")]
    public byte RegionId { get; set; }

    [Key]
    [Column("year")]
    public short Year { get; set; }

    [Key]
    [Column("month")]
    public byte Month { get; set; }

    [Column("revenue", TypeName = "money")]
    public decimal? Revenue { get; set; }

    [Column("expense", TypeName = "money")]
    public decimal? Expense { get; set; }

    [Column("profit", TypeName = "money")]
    public decimal? Profit { get; set; }

    [Required]
    [Column("created_by")]
    [StringLength(8)]
    [Unicode(false)]
    public string CreatedBy { get; set; }

    [Column("created_on")]
    public DateTime CreatedOn { get; set; }

    [Required]
    [Column("updated_by")]
    [StringLength(8)]
    [Unicode(false)]
    public string UpdatedBy { get; set; }

    [Column("updated_on")]
    public DateTime UpdatedOn { get; set; }

    [Required]
    [Column("version")]
    public byte[] Version { get; set; }
}
