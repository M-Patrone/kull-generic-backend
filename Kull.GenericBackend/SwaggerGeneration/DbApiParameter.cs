﻿using Kull.Data;
using Kull.DatabaseMetadata;
using Kull.GenericBackend.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Kull.GenericBackend.SwaggerGeneration
{
    public class DbApiParameter : WebApiParameter
    {
        private TableValuedParameter TableParameter;

        public SqlType DbType { get; }
        public bool IsNullable { get; }

        public DBObjectName UserDefinedType { get; }

        public DbApiParameter(string sqlName, string webApiName,
                SqlType sqlType, bool isNullable,
                DBObjectName userDefinedType,
                SqlHelper sqlHelper,
                NamingMappingHandler namingMappingHandler) : base(sqlName, webApiName)
        {
            this.DbType = sqlType;
            this.IsNullable = isNullable;
            UserDefinedType = userDefinedType;
            if (userDefinedType != null)
            {
                this.TableParameter = new TableValuedParameter(
                    GetSqlTypeWebApiName(this.UserDefinedType),
                    this.UserDefinedType,
                    sqlHelper, namingMappingHandler);
            }
        }



        private static string GetSqlTypeWebApiName(DBObjectName userDefinedType)
        {
            return (userDefinedType.Schema == "dbo" ? "" :
                                            userDefinedType.Schema + ".") + userDefinedType.Name;
        }


        public override IEnumerable<WebApiParameter> GetRequiredTypes()
        {
            if (TableParameter == null)
            {
                return Array.Empty<WebApiParameter>();
            }
            else
            {
                return new WebApiParameter[] { TableParameter };
            }
        }

        public override OpenApiSchema GetSchema()
        {
            OpenApiSchema property = new OpenApiSchema();
            property.Type = this.DbType.JsType;
            if (this.DbType.JsFormat != null)
            {
                property.Format = this.DbType.JsFormat;
            }
            property.Nullable = this.IsNullable;

            if (this.UserDefinedType != null)
            {
                property.UniqueItems = false;
                property.Items = new OpenApiSchema()
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.Schema,
                        Id = TableParameter.WebApiName
                    }
                };
            }
            return property;
        }


        private XElement ToXml(IDictionary<string, object> input)
        {
            return new XElement("el",
                input.Keys.Select(k => new XAttribute(k, input[k])));
        }


        private XElement ToXml(IEnumerable<IDictionary<string, object>> input)
        {
            return new XElement("root",
                input.Select(s => ToXml(s)));
        }

        public override object GetValue(HttpContext http, object valueProvided)
        {
            if (valueProvided is IDictionary<string, object> obj)
            {
                if (this.SqlName.EndsWith("Xml") || this.DbType.DbType == "xml")
                {
                    return ToXml(obj)?.ToString();
                }
                else if (this.UserDefinedType != null)
                {
                    return this.TableParameter.GetValue(http, new IDictionary<string, object>[] { obj });
                }
                else
                {
                    return JsonConvert.SerializeObject(obj);
                }
            }
            else if (valueProvided is IEnumerable<Dictionary<string, object>> objAr)
            {
                if (this.SqlName.EndsWith("Xml") || this.DbType.DbType == "xml")
                {
                    return ToXml(objAr)?.ToString();
                }
                else if (this.UserDefinedType != null)
                {
                    return TableParameter.GetValue(http, objAr);
                }
                else
                {
                    return JsonConvert.SerializeObject(objAr);
                }
            }
            else if (valueProvided is Newtonsoft.Json.Linq.JArray ar)
            {
                if (this.UserDefinedType != null)
                {
                    var jobjAr = ar.Cast<Newtonsoft.Json.Linq.JObject>()
                        .Select(oo => oo.Properties()
                            .ToDictionary(p => p.Name, p => p.Value.ToObject<object>())
                            ).ToArray();
                    return TableParameter.GetValue(http, jobjAr);
                }
                else
                {
                    return JsonConvert.SerializeObject(valueProvided);
                }
            }
            else if (valueProvided is Newtonsoft.Json.Linq.JObject obj2)
            {
                if (this.UserDefinedType != null)
                {
                    var jar_ob = new IDictionary<string, object>[]
                    {
                            obj2.Properties()
                                .ToDictionary(p => p.Name, p => p.Value.ToObject<object>())

                    };
                    return TableParameter.GetValue(http, jar_ob);
                }
                else
                {
                    return JsonConvert.SerializeObject(valueProvided);
                }
            }
            else
            {
                return valueProvided;
            }
        }
    }

}
