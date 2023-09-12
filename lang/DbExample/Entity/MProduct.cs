using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DbExample.Entity;

[PrimaryKey("Type", "Id")]
[Table("m_product")]
public partial class MProduct
{
    [Key]
    [Column("type")]
    public short Type { get; set; }

    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("product_code")]
    [StringLength(16)]
    [Unicode(false)]
    public string ProductCode { get; set; }

    [Required]
    [Column("product_name")]
    [StringLength(256)]
    public string ProductName { get; set; }

    [Column("price", TypeName = "money")]
    public decimal Price { get; set; }

    [Column("remarks")]
    [StringLength(1024)]
    public string Remarks { get; set; }

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

    [InverseProperty("Product")]
    public virtual ICollection<MOrderDetail> MOrderDetail { get; set; } = new List<MOrderDetail>();
}
