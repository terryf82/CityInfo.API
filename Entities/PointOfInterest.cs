using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        // By convention this is considered a 'navigation' property because the type (city) cannot be recognised as a scalar type
        // Use by convention means the relationship will always target the primary key of the principal entity (id of city will be foreign key)
        // Explicit declaration in use
        [ForeignKey("CityId")]
        public City City { get; set; }
    }
}