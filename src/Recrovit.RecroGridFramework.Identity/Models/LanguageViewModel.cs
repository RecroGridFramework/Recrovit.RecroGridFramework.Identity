using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recrovit.RecroGridFramework.Identity.Models
{
    public class LanguageViewModel
    {
        public LanguageViewModel()
        {
            this.Language = Recrovit.RecroGridFramework.RecroDict.DefaultLanguage;
        }
        public LanguageViewModel(HttpContext context)
        {
            this.Language = Recrovit.RecroGridFramework.RecroDict.GetSessionLanguage(context);
        }

        public string Language { get; set; }
        public string ReturnUrl { get; set; }

        public List<SelectListItem> Languages
        {
            get
            {
                var languages = new List<SelectListItem>();
                foreach (var item in RecroDict.ValidLanguages)
                {
                    var text = RecroDict.Get(item.Key, "RGF.Language", item.Key);
                    languages.Add(new SelectListItem { Value = item.Key, Text = text });
                }
                return languages.OrderBy(e => e.Text).ToList();
            }
        }
    }
}
