using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Data;
using System.Text;
using Mono.Data.SqliteClient;
using UnityEngine.Networking;

public class DBAccess : Singleton<DBAccess>
{
    private string m_connection;
    private IDbConnection m_dbcon;
    private IDbCommand m_dbcmd;
    private IDataReader m_reader;
    private StringBuilder m_builder;

    public void OpenDB(string a_file)
    {

        Debug.Log("Call to OpenDB:" + a_file);
        // check if file exists in Application.persistentDataPath

       /* if (m_dbcon != null)
        {
            Debug.Log("Connection ever open");
            return;
        }*/

        //ANDROID 
        string filepath = Application.persistentDataPath + "/" + a_file;

        //string filepath = Application.dataPath + "/" + a_file;

        if (!File.Exists(filepath))
        {
            Debug.LogWarning("File \"" + filepath + "\" does not exist. Attempting to create from \"" +
                             Application.dataPath + "!/assets/" + a_file);
            // if it doesn't ->
            // open StreamingAssets directory and load the db -> 
            UnityWebRequest loadDB = new UnityWebRequest("jar:file://" + Application.dataPath + "!/assets/" + a_file);
            while (!loadDB.isDone) { }
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDB.downloadHandler.data);
        }

        //open db connection
        m_connection = "URI=file:" + filepath;
        Debug.Log("Stablishing connection to: " + m_connection);
        m_dbcon = new SqliteConnection(m_connection);
        m_dbcon.Open();
    }

    public void CloseDB()
    {
        if (m_reader != null)
        {
            m_reader.Close();
        }
        m_reader = null;
        if (m_dbcmd != null)
        {
            m_dbcmd.Dispose();
        }
        m_dbcmd = null;
        if (m_dbcon != null)
        {
            m_dbcon.Close();
        }
        m_dbcon = null;
    }

    public IDataReader BasicQuery(string a_query)
    { // run a basic Sqlite query
        m_dbcmd = m_dbcon.CreateCommand(); // create empty command
        m_dbcmd.CommandText = a_query; // fill the command
        m_reader = m_dbcmd.ExecuteReader(); // execute command which returns a reader
        return m_reader; // return the reader

    }


    public bool CreateTable(string a_name, string[] a_col, string[] a_colType)
    { // Create a table, name, column array, column type array
        string query;
        query = "CREATE TABLE " + a_name + "(" + a_col[0] + " " + a_colType[0];
        for (var i = 1; i < a_col.Length; i++)
        {
            query += ", " + a_col[i] + " " + a_colType[i];
        }
        query += ")";
        try
        {
            m_dbcmd = m_dbcon.CreateCommand(); // create empty command
            m_dbcmd.CommandText = query; // fill the command
            m_reader = m_dbcmd.ExecuteReader(); // execute command which returns a reader
        }
        catch (Exception e)
        {

            Debug.Log(e);
            return false;
        }
        return true;
    }

    public int InsertIntoSingleInt(string a_tableName, string a_colName, string a_value)
    { // single insert
        string query;
        query = "INSERT INTO " + a_tableName + "(" + a_colName + ") " + "VALUES (" + a_value + ")";
        try
        {
            m_dbcmd = m_dbcon.CreateCommand(); // create empty command
            m_dbcmd.CommandText = query; // fill the command
            m_reader = m_dbcmd.ExecuteReader(); // execute command which returns a reader
        }
        catch (Exception e)
        {

            Debug.Log(e);
            return 0;
        }
        return 1;
    }

    public int InsertIntoSingleString(string a_tableName, string a_colName, string a_value)
    { // single insert
        string query;
        query = "INSERT INTO " + a_tableName + "(" + a_colName + ") " + "VALUES ('" + a_value + "')";
        try
        {
            m_dbcmd = m_dbcon.CreateCommand(); // create empty command
            m_dbcmd.CommandText = query; // fill the command
            m_reader = m_dbcmd.ExecuteReader(); // execute command which returns a reader
        }
        catch (Exception e)
        {

            Debug.Log(e);
            return 0;
        }
        return 1;
    }


    public int InsertIntoSpecific(string a_tableName, string[] a_col, string[] a_values)
    { // Specific insert with col and values
        string query;
        query = "INSERT INTO " + a_tableName + "(" + a_col[0];
        for (int i = 1; i < a_col.Length; i++)
        {
            query += ", " + a_col[i];
        }
        query += ") VALUES (" + a_values[0];
        for (int i = 1; i < a_col.Length; i++)
        {
            query += ", " + a_values[i];
        }
        query += ")";
        Debug.Log(query);
        try
        {
            m_dbcmd = m_dbcon.CreateCommand();
            m_dbcmd.CommandText = query;
            m_reader = m_dbcmd.ExecuteReader();
        }
        catch (Exception e)
        {

            Debug.Log(e);
            return 0;
        }
        return 1;
    }

    public int InsertInto(string a_tableName, string[] a_values)
    { // basic Insert with just values
        string query;
        query = "INSERT INTO " + a_tableName + " VALUES (" + a_values[0];
        for (int i = 1; i < a_values.Length; i++)
        {
            query += ", " + a_values[i];
        }
        query += ")";
        try
        {
            m_dbcmd = m_dbcon.CreateCommand();
            m_dbcmd.CommandText = query;
            m_reader = m_dbcmd.ExecuteReader();
        }
        catch (Exception e)
        {

            Debug.Log(e);
            return 0;
        }
        return 1;
    }

    public ArrayList SingleSelectWhere(string a_tableName, string a_itemToSelect, string a_wCol, string a_wPar, string a_wValue)
    { // Selects a single Item
        string query;
        query = "SELECT " + a_itemToSelect + " FROM " + a_tableName + " WHERE " + a_wCol + a_wPar + a_wValue;
        m_dbcmd = m_dbcon.CreateCommand();
        m_dbcmd.CommandText = query;
        m_reader = m_dbcmd.ExecuteReader();
        //string[,] readArray = new string[reader, reader.FieldCount];
        string[] row = new string[m_reader.FieldCount];
        ArrayList readArray = new ArrayList();
        while (m_reader.Read())
        {
            int j = 0;
            while (j < m_reader.FieldCount)
            {
                row[j] = m_reader.GetString(j);
                j++;
            }
            readArray.Add(row);
        }
        return readArray; // return matches
    }
}