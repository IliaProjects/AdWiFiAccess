using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GWA.Data.Models
{
    public class MainMenu
    {
        public MainMenu()
        {
            Location = "";
            JsFunction = "";
            OrderId = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ParentId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [StringLength(150)]
        public string Text { get; set; }       // название пункта меню на Русском языке (хотя, когда будем возвращать меню, то Text подставим из ресурсов \Resources\MenuItems.resx)

        [Required]
        [StringLength(150)]
        public string TextRo { get; set; }

        [Required]
        [StringLength(150)]
        public string TextEn { get; set; }

        public string Icon { get; set; }

        [DefaultValue("true")]
        public bool IsActive { get; set; }

        public string PermissionEnumText { get; set; }

        public string JsFunction { get; set; } // имя js функции на стороне клиента которая выполнится, нажав на этот пункт меню
        public string Location { get; set; }   // url на который произойдет переход, когда пользователь кликнет на данном пункте меню
    }
}
