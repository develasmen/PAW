using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace PAW.Models;

public partial class Task
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(255)]
    public string? Name { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastModified { get; set; }

    [StringLength(255)]
    public string? ModifiedBy { get; set; }
}
