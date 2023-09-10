using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DbExample.Entity;

[Table("m_order")]
public partial class MOrder
{
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Required]
    [Column("order_no")]
    [StringLength(16)]
    [Unicode(false)]
    public string OrderNo { get; set; }

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

    [InverseProperty("Order")]
    public virtual ICollection<MOrderDetail> MOrderDetail { get; set; } = new List<MOrderDetail>();
}
