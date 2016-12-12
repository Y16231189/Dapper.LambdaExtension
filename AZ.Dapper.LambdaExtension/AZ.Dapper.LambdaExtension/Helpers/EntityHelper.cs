﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AZ.Dapper.LambdaExtension.Attributes;
using AZ.Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace AZ.Dapper.LambdaExtension.Helpers
{
    public static class EntityHelper
    {
        public static Tuple<SqlTableDefine, List<SqlColumnDefine>> GetEntityDefine<T>()
        {
            var type = typeof(T);

            //处理表定义
            var name = type.Name;

            var tableAttr=type.GetCustomAttribute<LamTableAttribute>();

            var sqlTableDef=new SqlTableDefine(tableAttr,name);

            //处理列定义
            var colDeflist=new List<SqlColumnDefine>();

            var columns = type.GetProperties();

            foreach (var cp in columns)
            {
                var ignore = cp.GetCustomAttribute<LamIgnoreAttribute>();

                if (ignore == null)
                {
                    var keyAttr = cp.GetCustomAttribute<LamKeyAttribute>();
                    var columnAttr = cp.GetCustomAttribute<LamColumnAttribute>();
                    var dataTypeAttr = cp.GetCustomAttribute<LamCustomeDataTypeAttribute>();

                    var cname = cp.Name;

                    var nullable = cp.PropertyType.IsNullableType();

                    var cd=new SqlColumnDefine(cname,columnAttr.Name,null,cp.PropertyType, nullable, columnAttr,keyAttr,dataTypeAttr);

                    colDeflist.Add(cd);
                }
            }

            return new Tuple<SqlTableDefine, List<SqlColumnDefine>>(sqlTableDef,colDeflist);
        }

        public static bool IsNullableType(this Type theType)
        {
            return (theType.IsGenericType
                && theType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}