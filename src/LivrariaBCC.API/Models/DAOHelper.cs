﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace LivrariaBCC.API.Models
{
    static class DAOHelper
    {
        private static MySqlConnection OpenConnection()
        {
            MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString);
            connection.Open();

            return connection;
        }

        public static MySqlCommand CreateCommand(string command)
        {
            MySqlConnection connection = OpenConnection();
            MySqlCommand mySqlCommand = new MySqlCommand(command, connection);
            mySqlCommand.CommandType = CommandType.StoredProcedure;
            
            mySqlCommand.Disposed += (sender, e) => { connection.Dispose(); };

            return mySqlCommand;
        }

    }
}