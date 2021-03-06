﻿using Perplex.Integration.Core.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perplex.Integration.Core.Steps
{
    public abstract class AbstractSqlStep : JobStep
    {
        [ConnectionString("db", Type = "Sql")]
        public string DbConnectionString { get; set; }
        [Property()]
        public int CommandTimeout { get; set; }
        protected SqlConnection DbConnection { get; private set; }

        [Property(Description ="If set, this statement is run once before the step is executed.")]
        public string PreExecuteSqlStatement { get; set; }

        public AbstractSqlStep()
        {
            CommandTimeout = 300;
        }

        /// <summary>
        /// Creates and opens an Sql Connection.
        /// </summary>
        public override void Initialise()
        {
            base.Initialise();
            // SQL DB init stuff
            DbConnection = new SqlConnection(DbConnectionString);
            DbConnection.Open();
            // Pre execute statement
            RunPreExecuteSql();
        }

        /// <summary>
        /// Runs the <see cref="PreExecuteSqlStatement"/>.
        /// </summary>
        protected virtual void RunPreExecuteSql()
        {
            if (!string.IsNullOrEmpty(PreExecuteSqlStatement))
            {
                using SqlCommand preCmd = DbConnection.CreateCommand();
                preCmd.CommandText = PreExecuteSqlStatement;
                Log.Verbose("Executing {PreExecuteSqlStatement}", preCmd.CommandText);
                preCmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Closes the SQL connection, if it is open.
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();
            DbConnection?.Close();
            DbConnection = null;
        }
    }
}
