using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Services
{
    public class EnumLocalizer
    {
        public EnumLocalizer()
        {
        }

        public string GetLocalName<EnumType>(EnumType enumValue) where EnumType : struct, IConvertible
        {
            string enumString = enumValue.ToString();
            // To Do: search the locatiation strings for the "enum.{EnumType}.{enum entry name}" entry.
            return enumString;
        }

        public SelectListItem GetSelectListItem<EnumType>(EnumType enumValue) where EnumType : struct, IConvertible
        {
            string enumString = enumValue.ToString();
            string localString = enumString; // To Do: get from locatiation strings.
            return new SelectListItem()
            {
                Text = localString,
                Value = enumString
            };
        }

        public List<SelectListItem> GetSelectList<EnumType>() where EnumType : struct, IConvertible
        {
            var list = new List<SelectListItem>();
            foreach (EnumType enumValue in Enum.GetValues(typeof(EnumType)))
            {
                list.Add(GetSelectListItem(enumValue));
            }
            return list;
        }

        public List<SelectListItem> GetSelectList<EnumType>(EnumType[] selectedValues) where EnumType : struct, IConvertible
        {
            var list = new List<SelectListItem>();
            foreach (EnumType enumValue in Enum.GetValues(typeof(EnumType)))
            {
                var item = GetSelectListItem(enumValue);
                item.Selected = (selectedValues.Contains(enumValue));
                list.Add(item);
            }
            return list;
        }

        public List<SelectListItem> GetSelectList<EnumType>(EnumType selectedValue) where EnumType : struct, IConvertible
        {
            return GetSelectList(new EnumType[] { selectedValue });
        }

    }
}
