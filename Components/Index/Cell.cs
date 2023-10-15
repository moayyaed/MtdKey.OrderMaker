/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.AspNetCore.Mvc;
using MtdKey.OrderMaker.Core;
using MtdKey.OrderMaker.Entity;
using MtdKey.OrderMaker.Models.Index;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MtdKey.OrderMaker.Components.Index
{
    [ViewComponent(Name = "IndexCell")]
    public class Cell : ViewComponent
    {

        public Task<IViewComponentResult> InvokeAsync(DocFieldModel field)
        {
            string viewName = GetViewName(field.Type);
            return Task.FromResult((IViewComponentResult) View(viewName, field));
        }

        private static string GetViewName(int idType) {

            string viewName;

            switch (idType)
            {
                case 2: { viewName = "Integer"; break; }
                case 3: { viewName = "Decimal"; break; }
                case 4: { viewName = "Memo"; break; }
                case 5: { viewName = "Date"; break; }
                case 6: { viewName = "DateTime"; break; }
                case 7: { viewName = "File"; break; }
                case 8: { viewName = "Picture"; break; }
                case 10: { viewName = "Time"; break; }
                case 11: { viewName = "List"; break; }
                case 12: { viewName = "CheckBox"; break; }
                case 13: { viewName = "Link"; break; }

                default:
                    {
                        viewName = "Text";
                        break;
                    }
            }

            return viewName;
        }
    }
}
