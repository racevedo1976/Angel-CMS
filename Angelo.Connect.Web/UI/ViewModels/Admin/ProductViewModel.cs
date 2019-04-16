using Angelo.Common.Extensions;
using Angelo.Common.Models;
using Angelo.Connect.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class ProductViewModel
    {
        [Display(Name = "Id", ShortName = "Id")]
        public string Id { get; set; }

        [Display(Name = "Name", ShortName = "Name")]
        [StringLength(maximumLength: 80, ErrorMessage = "Name.Error.MaxLength")]
        [Required(ErrorMessage = "dName.Error.Required")]
        public string Name { get; set; }

        [Display(Name = "Description", ShortName = "Description")]
        public string Description { get; set; }

        [Display(Name = "Schema File", ShortName = "Schema")]
        public string SchemaFile { get; set; }

        [Display(Name = "Category Id", ShortName = "Category Id")]
        public string CategoryId { get; set; }

        [Display(Name = "Category Name", ShortName = "Category")]
        public string CategoryName { get; set; }
    }



}
