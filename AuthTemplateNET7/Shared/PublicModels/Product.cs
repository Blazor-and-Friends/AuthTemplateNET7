using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthTemplateNET7.Shared.PublicModels;

//added

public class Product
{
    public int Id { get; set; }

    [MaxLength(2048)]
    public string Description { get; set; }

    [Required, MaxLength(256)]
    public string Name { get; set; }

    [MaxLength(128)]
    public string ImgUrl { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal Price { get; set; }
}
