using Angelo.Common.Models;
using Angelo.Connect.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SettingViewModel : IMappableViewModel
    {
        [Display(Name = "Setting Id", ShortName = "Id")]
        public int Id { get; set; }

        [Display(Name = "Setting Field Name", ShortName = "Field Name")]
        public string FieldName { get; set; }

        [Display(Name = "Setting Value", ShortName = "Value")]
        public string Value { get; set; }

        // Mapping method
        public void CopyFrom(object data)
        {
            //if (data is ClientProductSetting)
            //{
            //    var setting = data as ClientProductSetting;
            //    Id = setting.Id;
            //    FieldName = setting.FieldName;
            //    Value = setting.Value;
            //}
            //else if (data is ClientModuleSetting)
            //{
            //    var setting = data as ClientModuleSetting;
            //    Id = setting.Id;
            //    FieldName = setting.FieldName;
            //    Value = setting.Value;
            //}
            //else
                throw new NotImplementedException();
        }

        // Mapping method
        public void CopyTo(object data)
        {
            //if (data is ClientProductSetting)
            //{
            //    var setting = data as ClientProductSetting;
            //    setting.Id = Id;
            //    setting.FieldName = FieldName;
            //    setting.Value = Value;
            //}
            //else if (data is ClientModuleSetting)
            //{
            //    var setting = data as ClientModuleSetting;
            //    setting.Id = Id;
            //    setting.FieldName = FieldName;
            //    setting.Value = Value;
            //}
            //else
                throw new NotImplementedException();
        }
    }
}
