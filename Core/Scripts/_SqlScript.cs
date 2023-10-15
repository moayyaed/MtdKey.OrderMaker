using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;


namespace MtdKey.OrderMaker.Core.Scripts
{
    public enum TypeRequest
    {
        GetRowCount,
        GetIds
    }
    internal static class SqlScript
    {
        private static string GetScript(string scriptName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = typeof(SqlScript).Namespace + $".{scriptName}";
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new(stream);
            string sqlScript = reader.ReadToEnd();
            return sqlScript;
        }

        public static string GetStoreIds(FilterSQLparams filter, List<DocFieldModel> docFields, TypeRequest typeRequest)
        {
            var script = GetScript("Get_Store_Ids.sql");
           
            if(filter.FormId == string.Empty || !Guid.TryParse(filter.FormId, out Guid formId)) return script;

            script = script.Replace("[FormId]", $"{formId}");

            if(filter.StoreId != null && filter.StoreId != string.Empty)
                script = script.Replace("/*and StoreId*/", $" and StoreId = '{filter.StoreId}'");
                   
            if (filter.SortByFieldId == string.Empty || filter.SortByFieldId == "date")
                script = script.Replace("/*order by store.timecr desc*/", $"order by store.timecr {filter.SortOrder}");

            if (filter.SortByFieldId.Contains("field-"))
                script = script.Replace("[FieldId]", filter.SortByFieldId
                    .Replace("field-", "")).Replace("/*order by indexSort*/", $"order by indexSort {filter.SortOrder}");

            if (filter.SortByFieldId.Contains("number"))
                script = script.Replace("/*order by sequence desc*/", $"order by sequence {filter.SortOrder}");

            if (filter.SortByFieldId.Contains("approval"))
            {
                script = script.Replace("/*left join mtd_store_approval*/", 
                    " left join mtd_store_approval as approval on store.id = approval.id ");
                script = script.Replace("/*order by approval.md_approve_stage desc*/", 
                    $" order by approval.complete  {filter.SortOrder} ");
            }

                if (filter.DateStart != DateTime.MinValue)
            {
                var dateStart = filter.DateStart.ToString("yyyy-MM-dd");
                var dateEnd = filter.DateEnd.ToString("yyyy-MM-dd");
                script = script.Replace("/*and store.timecr between*/", 
                    $" and store.timecr between '{dateStart} 00:00:00' and '{dateEnd} 23:59:59' ");
            }
            
            if (filter.SearchText != string.Empty)
            {
                var searchText = filter.SearchText.Replace("'",string.Empty);
                if (searchText.Length > 250) searchText = searchText[..250];
                script = script.Replace("Result like '%%'", $"Result like '%{searchText}%'");
            }

            if (filter.SearchNumber != string.Empty)
            {
                var searchNumber = filter.SearchNumber.Replace("'", string.Empty);
                if (searchNumber.Length > 10) searchNumber = searchNumber[..10];
                script = script.Replace("store.sequence like '%%'", $"store.sequence like '%{searchNumber}%'");
            }

            if (docFields.Count > 0)
            {
                var fieldIds = string.Empty;
                docFields.ForEach(field =>
                {
                    if (fieldIds != string.Empty) fieldIds += ",";
                    fieldIds += $"'{field.Id}'";
                });

                script = script.Replace("/*and FieldId in*/", $" and FieldId in ({fieldIds})");
            }

            bool ownerRequest = filter.OwnerId != null && filter.OwnerId != string.Empty;
            if (ownerRequest)
            {
                script = script.Replace("/*inner join mtd_store_owner*/", 
                    $"inner join mtd_store_owner as o on f.StoreId = o.id and o.user_id='{filter.OwnerId}'");
            }

            if (!ownerRequest && filter.UserInGroupIds?.Count > 0)
            {
                var userIds = string.Empty;
                filter.UserInGroupIds.ForEach(userId =>
                {
                    if (userIds != string.Empty) userIds += ",";
                    userIds += $"'{userId}'";
                });

                script = script.Replace("/*inner join mtd_store_owner*/",
                    $"inner join mtd_store_owner as o on f.StoreId = o.id and o.user_id in ({userIds})");
            }

            filter.FilterFields.ForEach(field => {
                FilterDateHandler(ref script, field);
                FilterTextHandler(ref script, field);
                FilterMemoHandler(ref script, field);
                FilterIntHandler(ref script, field);
                FilterDecimalHandler(ref script, field);
            });


            if(typeRequest == TypeRequest.GetIds)
            {
                var limitStart = filter.Page == 1 ? 0 : filter.PageSize * (filter.Page - 1);
                var limitEnd = filter.PageSize;
                script = script.Replace("limit 0,10", $"limit {limitStart},{limitEnd}");

            } else
            {
                script = script.Replace("limit 0,10", " ");
                script = script.Replace("select StoreId from (", "select count(StoreId) from (");
            }

            return script;
        }

        private static void FilterDateHandler(ref string script, FilterFieldModel model)
        {
            bool IsDate = model.Type == FieldType.Date || model.Type == FieldType.DateTime;
            if (IsDate is not true) return;

            string[] dates = model.Value.Split("***").ToArray();
            if(!DateTime.TryParseExact(dates[0], model.ValueExt, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateStart) ||
               !DateTime.TryParseExact(dates[1], model.ValueExt, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateFinish))
                return;
            var term = script.Contains("and (Result") ? "or" : "and";
            script = script.Replace("/*and Result*/", 
                $" {term} (Result between '{dateStart:yyyy-MM-dd 00:00:00}' " +
                $"and '{dateFinish:yyyy-MM-dd 23:59:59}') /*and Result*/ ");
        }

        private static void FilterTextHandler(ref string script, FilterFieldModel model)
        {
            bool IsText = model.Type == FieldType.Text || model.Type == FieldType.Link;
            if (IsText is not true) return;
            TermHandler(ref script, model);
            
        }

        private static void FilterMemoHandler(ref string script, FilterFieldModel model)
        {
            bool IsMemo = model.Type == FieldType.Memo;
            if (IsMemo is not true) return;
            TermHandler(ref script, model);
        }


        private static void FilterIntHandler(ref string script, FilterFieldModel model)
        {
            bool IsInt = model.Type == FieldType.Int || model.Type == FieldType.Checkbox;
            if (IsInt is not true) return;
            TermHandler(ref script, model);
        }

        private static void FilterDecimalHandler(ref string script, FilterFieldModel model)
        {
            bool IsDecimal = model.Type == FieldType.Decimal;
            if (IsDecimal is not true) return;
            TermHandler(ref script, model);
        }


        private static void TermHandler(ref string script, FilterFieldModel model)
        {
            var value = model.Value;
            var term = script.Contains("and (Result") ? "or" : "and";

            string result = $" {term} (Result like '%{value}%' and FieldId = '{model.FieldId}') /*and Result*/";
            switch (model.Term)
            {
                case 1:
                    {
                        result = $" {term} (Result = '{value}' and FieldId = '{model.FieldId}') /*and Result*/";
                        break;
                    }
                case 2:
                    {
                        result = $" {term} (Result < '{value}' and FieldId = '{model.FieldId}') /*and Result*/";
                        break;
                    }
                case 3:
                    {
                        result = $" {term} (Result > '{value}' and FieldId = '{model.FieldId}') /*and Result*/";
                        break;
                    }
                case 5:
                    {
                        result = $" {term} (Result <> '{value}' and FieldId = '{model.FieldId}') /*and Result*/";
                        break;
                    }
            }

            script = script.Replace("/*and Result*/", result);
        }

    }
}
