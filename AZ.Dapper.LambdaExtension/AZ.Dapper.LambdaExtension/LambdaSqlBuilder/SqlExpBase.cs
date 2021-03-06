﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.LambdaExtension.LambdaSqlBuilder.Adapter;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;
using Dapper.LambdaExtension.LambdaSqlBuilder.Resolver;

namespace Dapper.LambdaExtension.LambdaSqlBuilder
{

    public abstract class SqlExpBase
    {
        internal Builder.Builder _builder;
        internal LambdaResolver _resolver;
        internal SqlType _type;
        internal SqlAdapterType _adapter;

        internal List<SqlExpBase> _childSqlExps = new List<SqlExpBase>();

     

        public string JoinSubAliasTableName { get; internal set; }

        internal Type _entityType;

        public Builder.Builder SqlBuilder { get { return _builder; } }

        public SqlType SqlType { get { return _type; } }

        public SqlExpBase()
        {

        }

        public SqlExpBase(SqlAdapterType adater, Type entityType)
        {
            _type = SqlType.Query;
            _adapter = adater;
            _entityType = entityType;
            _builder = new Builder.Builder(_type, entityType, AdapterFactory.GetAdapterInstance(_adapter));
            _resolver = new LambdaResolver(_builder);
        }

        public SqlExpBase(SqlAdapterType adater, SqlTableDefine tableDefine, List<SqlColumnDefine> columnDefines)
        {
            _type = SqlType.Query;
            _adapter = adater;

            _builder = new Builder.Builder(_type, tableDefine, columnDefines, AdapterFactory.GetAdapterInstance(_adapter));
            _resolver = new LambdaResolver(_builder);
        }

        public string SqlString
        {
            get
            {
                return _builder.SqlString();
            }
        }

        public string QueryPage(int pageSize, int? pageNumber = null)
        {
            return _builder.QueryPage(pageSize, pageNumber);
        }

        public string QuerySubPage(int pageSize, int? pageNumber = null)
        {
            return _builder.QuerySubPage(pageSize, JoinSubAliasTableName, pageNumber);
        }

        public IDictionary<string, object> Parameters
        {
            get
            {
                if (_childSqlExps.Any())
                {
                    var parameterList = new Dictionary<string, object>();

                    foreach (var child in _childSqlExps)
                    {
                        var parameters = child.Parameters;

                        foreach (var pp in parameters)
                        {
                            parameterList.Add(pp.Key, pp.Value);
                        }
                    }

                    foreach (var pm in _builder.Parameters)
                    {
                        parameterList.Add(pm.Key,pm.Value);
                    }
                    return parameterList;
                }
                else
                {
                    return _builder.Parameters;
                }
            }
        }

        /// <summary>
        /// 主要给Dapper用
        /// </summary>
        public string[] SplitColumns
        {
            get { return _builder.SplitColumns.ToArray(); }
        }

        #region update

        public void Clear()
        {
            _builder.Clear();
        }

        #endregion

        public void SetAdapter(SqlAdapterType adapter)
        {
            _builder.Adapter = AdapterFactory.GetAdapterInstance(adapter);
        }



        //public static SqlAdapter GetAdapterByDb(IDbConnection dbconnection)
        //{
        //    SqlAdapter adapter = SqlAdapter.SqlServer2005;


        //}


    }
}
