﻿using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Db.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.Db.BuildTasks
{
    public abstract class AbstractBuildTask : IBuildTask
    {
        private readonly ILogger _logger;

        public AbstractBuildTask(string name, ILogger logger)
        {
            Name = name;
            _logger = logger;
        }

        public bool Initialized { get; protected set; }

        public string Name { get; protected set; }

        public abstract Task Build(BuildContext context);

        public void Initialize(IDictionary<string, String> paramters)
        {
            this.Initialized = true;
        }

        protected IEnumerable<Table> FilterTable(IEnumerable<Table> tables, string buildKey, Build build)
        {
            _logger.LogInformation($"FilterTable Build:{buildKey} Start!");
            IEnumerable<Table> buildTables = CopyTables(tables);
            if (build.IgnoreTables != null)
            {
                _logger.LogInformation($"FilterTable Build:{buildKey} IgnoreTables: [{String.Join(",", build.IgnoreTables)}]!");
                buildTables = tables.Where(m => !build.IgnoreTables.Contains(m.Name));
            }
            if (build.IncludeTables != null)
            {
                _logger.LogInformation($"FilterTable Build:{buildKey} IncludeTables: [{String.Join(",", build.IncludeTables)}]!");
                buildTables = tables.Where(m => build.IncludeTables.Contains(m.Name));
            }
            _logger.LogInformation($"FilterTable Build:{buildKey} End!");
            return buildTables;
        }

        protected IList<Table> CopyTables(IEnumerable<Table> tables)
        {
            return tables.Select(m => new Table
            {
                Id = m.Id,
                Name = m.Name,
                TypeName = m.TypeName,
                ConvertedName = m.ConvertedName,
                Description = m.Description,
                Columns = m.Columns.Select(c => new Column
                {
                    Id = c.Id,
                    Name = c.Name,
                    DbType = c.DbType,
                    Description = c.Description,
                    AutoIncrement = c.AutoIncrement,
                    ConvertedName = c.ConvertedName,
                    IsNullable = c.IsNullable,
                    IsPrimaryKey = c.IsPrimaryKey,
                    LanguageType = c.LanguageType
                }).ToList()
            }).ToList();
        }
    }
}
