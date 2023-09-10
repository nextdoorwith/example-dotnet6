using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DbExample.Entity;

[Table("m_employee")]
public partial class MEmployee
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [Column("employee_no")]
    [StringLength(10)]
    [Unicode(false)]
    public string EmployeeNo { get; set; }

    [Required]
    [Column("name")]
    [StringLength(256)]
    public string Name { get; set; }

    [Column("address")]
    [StringLength(2048)]
    public string Address { get; set; }

    [Column("gender")]
    public byte? Gender { get; set; }

    [Column("retired")]
    public bool Retired { get; set; }

    [Column("birthday")]
    public DateTime Birthday { get; set; }

    [Column("internal_id")]
    public Guid? InternalId { get; set; }

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
