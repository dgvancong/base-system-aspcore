using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseServerProject.Core.Entities;

[Table("Colors")]
public class Color
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ColorID { get; set; }

    [Required]
    [MaxLength(50)]
    public string ColorName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? ColorCode { get; set; }

    public int SortOrder { get; set; } = 0;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}