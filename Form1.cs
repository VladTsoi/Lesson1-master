using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Zadanie
{
    public partial class Form1 : Form
    {
        string connString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        
        public Form1()
        {
            InitializeComponent();
            CountryDataBase();
        }

        private void CountryDataBase()
        {
            SqlConnection conn = new SqlConnection(connString);
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT c.Id, c.Name AS 'Название', c.Code AS 'Код страны', " +
                "ct.Name AS 'Столица', r.Name AS 'Регион', c.Area AS 'Площадь(кв.км)', c.Population AS 'Население' FROM Country c" +
                " JOIN Region r ON r.Id = c.RegionId JOIN City ct ON ct.Id = c.CityId", conn))
            {
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.ReadOnly = true;
            }
        }

        private void search_Click(object sender, EventArgs e)
        {
            if (textBoxSearch.Text.Length <= 0)
            {
                MessageBox.Show("Пожалуйста введите страну.");
            }
            else
            {
                try
                {
                    PrintCountryInfo();
                }
                catch
                {
                    MessageBox.Show("Страна " + textBoxSearch.Text + " не найдена");
                }
            }
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            CountryDataBase();
        }

        private void PrintCountryInfo()
        {
            SearchCountryInfo search = new SearchCountryInfo { textSearch = textBoxSearch.Text };
            Country country = search.GetSearch();
            label_area.Text = country.area.ToString() + " кв.км";
            label_cap.Text = country.capital;
            label_code.Text = country.numericCode.ToString();
            label_country.Text = country.name;
            label_pop.Text = country.population.ToString() + " чел.";
            label_reg.Text = country.region;
            DialogResult vibor = MessageBox.Show("Хотите сохранить информацию в базу данных?", "Да или нет", MessageBoxButtons.YesNo);

            if (vibor == DialogResult.Yes)
            {
                try
                {
                    SaveCountryInfo saveCountryInfo = new SaveCountryInfo { _country = search.GetSearch() };
                    string SaveCity = saveCountryInfo.GetSaveCity();
                    string SaveRegion = saveCountryInfo.GetSaveRegion();
                    saveCountryInfo.GetSaveCountry(SaveCity, SaveRegion);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }
    }
}
