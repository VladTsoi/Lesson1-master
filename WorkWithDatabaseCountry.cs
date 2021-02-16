using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Zadanie
{
    class WorkWithDatabaseCountry : Form1
    {
        public void PrintCountryInfo(Country country)
        {
            DialogResult vibor = MessageBox.Show("Хотите сохранить информацию в базу данных?", "Да или нет", MessageBoxButtons.YesNo);

            if (vibor == DialogResult.Yes)
            {
                try
                {
                    string _SaveCity = SaveCity(country);
                    string _SaveRegion = SaveRegion(country);
                    SaveCountry(_SaveCity, _SaveRegion, country);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        public string SaveSqlQuery(string tableName, string save_value)
        { 
            string Id = "";

            conn = new SqlConnection(connString);
            conn.Open();
            using (SqlDataAdapter adapt = new SqlDataAdapter("SELECT Id FROM " + tableName + " WHERE Name = '" + save_value + "'", conn))
            {
                DataTable dt = new DataTable();
                adapt.Fill(dt);
                if (dt.Rows.Count <= 0)
                {
                    using (SqlCommand comm = new SqlCommand("INSERT INTO " + tableName + " (Name) VALUES(@Name) SELECT CAST(SCOPE_IDENTITY() AS varchar(max))", conn))
                    {
                        comm.Parameters.AddWithValue("Name", save_value);
                        try
                        {
                            Id = comm.ExecuteScalar().ToString();
                            return Id;
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }  
                }
            }
            return Id;
        }

        public string SaveCity(Country _country)
        {
            return SaveSqlQuery("City", _country.capital);
        }
        public string SaveRegion(Country _country)
        {
            return SaveSqlQuery("Region", _country.region);
        }

        public void SaveCountry(string cityId, string regId, Country _country)
        {
            conn = new SqlConnection(connString);
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("IF EXISTS(SELECT * FROM Country WHERE Code = '" + _country.numericCode + "')" +
                "UPDATE c SET c.Name = '" + _country.name + "', c.Area = '"
                        + _country.area.ToString() + "', c.Population = '" + _country.population.ToString() + "', c.CityId = ct.Id, c.RegionId = r.Id" +
                        " FROM Country c, Region r, City ct WHERE c.Code = '" + _country.numericCode + "' " +
                        "ELSE INSERT INTO Country (Name, Code, Area, Population, CityId, RegionId) " +
                        "VALUES(@Name, @Code, @Area, @Population, '" + cityId + "', '" + regId + "')", conn))
            {
                cmd.Parameters.AddWithValue("Name", _country.name);
                cmd.Parameters.AddWithValue("Code", _country.numericCode);
                cmd.Parameters.AddWithValue("Area", _country.area.ToString());
                cmd.Parameters.AddWithValue("Population", _country.population.ToString());
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Данные сохранены!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }    
        }
    }
}
