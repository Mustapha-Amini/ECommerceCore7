using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceCore7.Models.Entities
{
    public class InventoryActionType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InventoryActionID { get; set; }

        [Required]
        [Display(Name = "نوع عملیات انبار")]
        public string InventoryActionTypeTitle { get; set; }

        // Navigation Properties
        public virtual List<InventoryAction> InventoryActions { get; set; }
    }
}
