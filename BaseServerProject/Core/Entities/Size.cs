using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseServerProject.Core.Entities;

[Table("Sizes")]
public class Size
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SizeID { get; set; }

    [Required]
    [MaxLength(20)]
    public string SizeName { get; set; } = string.Empty;

    public int SortOrder { get; set; } = 0;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}