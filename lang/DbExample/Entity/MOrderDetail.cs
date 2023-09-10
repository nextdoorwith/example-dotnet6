using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DbExample.Entity;

[PrimaryKey("OrderId", "ProductId")]
[Table("m_order_detail")]
public partial class MOrderDetail
{
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

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

    [ForeignKey("OrderId")]
    [InverseProperty("MOrderDetail")]
    public virtual MOrder Order { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("MOrderDetail")]
    public virtual MProduct Product { get; set; }
}
