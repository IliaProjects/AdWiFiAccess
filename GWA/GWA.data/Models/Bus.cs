using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace GWA.Data.Models
{
    public class Bus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        [RegularExpression(@"^(\d{4})$", ErrorMessage = "Номер троллейбуса должен состоять из 4-х цифр")]
        public string Nr { get; set; }

        [Required]
        [RegularExpression(@"^(\d{2})$", ErrorMessage = "Маршрут троллейбуса должен состоять из 2-х цифр")]
        public string Route { get; set; }

        public string Notes { get; set; }

        public virtual ICollection<BindingRouterBus> BindingRouterBuses { get; set; }
    }
}
