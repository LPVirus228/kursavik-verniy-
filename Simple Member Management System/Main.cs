﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Simple_Member_Management_System
{
    public partial class Main : Form
    {
        SQLiteConnection conn;
        SQLiteCommand cmd;
        SQLiteDataAdapter adapter;
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        int id;
        bool isDoubleClick = false;
        String connectString;


        public Main()
        {
            InitializeComponent();
            connectString = @"Data Source=" + Application.StartupPath + @"\Database\db_mem.db;version=3";
            GenerateDatabase();
        }

        private void Add(object sender, EventArgs e) {
           
            if (txt_firstname.Text == "" || txt_lastname.Text == "" || txt_address.Text == "" || txt_age.Text == "" || cbox_gender.SelectedIndex == -1)    
            {
                MessageBox.Show("НЕ ВСЕ ПОЛЯ ЗАПОЛНЕНЫ!");
            }
            else {
                try
                {
                    conn = new SQLiteConnection(connectString);
                    cmd = new SQLiteCommand();
                    cmd.CommandText = @"INSERT INTO member (firstname, lastname, address, age, gender) VALUES(@firstname, @lastname, @address, @age, @gender)";
                    cmd.Connection = conn;
                    cmd.Parameters.Add(new SQLiteParameter("@firstname", txt_firstname.Text));
                    cmd.Parameters.Add(new SQLiteParameter("@lastname", txt_lastname.Text));
                    cmd.Parameters.Add(new SQLiteParameter("@address", txt_address.Text));
                    cmd.Parameters.Add(new SQLiteParameter("@age", txt_age.Text));
                    cmd.Parameters.Add(new SQLiteParameter("@gender", cbox_gender.SelectedItem.ToString()));
                    conn.Open();

                    int i = cmd.ExecuteNonQuery();

                    if (i == 1)
                    {
                        MessageBox.Show("ЗАПИСЬ СОЗДАНА!");
                        txt_firstname.Text = "";
                        txt_lastname.Text = "";
                        txt_address.Text = "";
                        txt_age.Text = "";
                        cbox_gender.SelectedItem = null;
                        ReadData();
                        dataGridView1.ClearSelection();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void GenerateDatabase() {
            String path = Application.StartupPath + @"\Database\db_mem.db";
            if (!File.Exists(path))
            {
                conn = new SQLiteConnection(connectString);
                conn.Open();
                string sql = "CREATE TABLE member (ID INTEGER PRIMARY KEY AUTOINCREMENT, firstname TEXT, lastname TEXT, address TEXT, age TEXT, gender TEXT)";
                cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private void ReadData() {
            try
            {
                conn = new SQLiteConnection(connectString);
                conn.Open();
                cmd = new SQLiteCommand();
                String sql = "SELECT * FROM member";
                adapter = new SQLiteDataAdapter(sql, conn);
                ds.Reset();
                adapter.Fill(ds);
                dt = ds.Tables[0];
                dataGridView1.DataSource = dt;
                conn.Close();
                dataGridView1.Columns[1].HeaderText = "НАЗВАНИЕ";
                dataGridView1.Columns[2].HeaderText = "МОДЕЛЬ";
                dataGridView1.Columns[3].HeaderText = "ПРЕДНАЗНАЧЕНИЕ";
                dataGridView1.Columns[4].HeaderText = "КОЛИЧЕСТВО";
                dataGridView1.Columns[5].HeaderText = "БРАК/ГОТОВОЕ";
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ReadData();
        }

        private void Edit(object sender, DataGridViewCellEventArgs e) {
            id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
            txt_firstname.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            txt_lastname.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            txt_address.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            txt_age.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
            cbox_gender.SelectedIndex = cbox_gender.FindStringExact(dataGridView1.SelectedRows[0].Cells[5].Value.ToString());
            isDoubleClick = true;
        }

        private void GetIdToDelete(object sender, DataGridViewCellEventArgs e) {
            id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
            isDoubleClick = false;
            txt_firstname.Text = "";
            txt_lastname.Text = "";
            txt_address.Text = "";
            txt_age.Text = "";
            cbox_gender.SelectedItem = null;
        }


        private void Update(object sender, EventArgs e) {
            if(isDoubleClick) { 
                try {
                    conn.Open();
                    cmd = new SQLiteCommand();
                    cmd.CommandText = @"UPDATE member set firstname=@firstname, lastname=@lastname, address=@address, age=@age, gender=@gender WHERE ID='"+ id +"'";
                    cmd.Connection = conn;
                    cmd.Parameters.AddWithValue("@firstname", txt_firstname.Text);
                    cmd.Parameters.AddWithValue("@lastname", txt_lastname.Text);
                    cmd.Parameters.AddWithValue("@address", txt_address.Text);
                    cmd.Parameters.AddWithValue("@age", txt_age.Text);
                    cmd.Parameters.AddWithValue("@gender", cbox_gender.SelectedItem.ToString());
                    
                    int i = cmd.ExecuteNonQuery();

                    if (i == 1)
                    {
                        MessageBox.Show("УСПЕШНО ИЗМЕНЕНО!");
                        txt_firstname.Text = "";
                        txt_lastname.Text = "";
                        txt_address.Text = "";
                        txt_age.Text = "";
                        cbox_gender.SelectedItem = null;
                        ReadData();
                        id = 0;
                        dataGridView1.ClearSelection();
                        dataGridView1.CurrentCell = null;
                        isDoubleClick = false;
                    }

                    conn.Close();
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Delete(object sender, EventArgs e) {
            DialogResult dialogResult = MessageBox.Show("ВЫ ТОЧНО ХОТИТЕ УДАЛИТЬ ЭТУ ЗАПИСЬ?", "ВНИМАНИЕ!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    conn = new SQLiteConnection(connectString);
                    conn.Open();
                    cmd = new SQLiteCommand();
                    cmd.CommandText = @"DELETE FROM member WHERE ID='" + id + "'";
                    cmd.Connection = conn;
                    int i = cmd.ExecuteNonQuery();
                    if (i == 1)
                    {
                        MessageBox.Show("УСПЕШНО УДАЛЕНО!");
                        id = 0;
                        dataGridView1.ClearSelection();
                        dataGridView1.CurrentCell = null;
                        ReadData();
                        dataGridView1.ClearSelection();
                        dataGridView1.CurrentCell = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                
            }

        }

        private void Clear(object sender, EventArgs e)
        {
            id = 0;
            txt_firstname.Text = "";
            txt_lastname.Text = "";
            txt_address.Text = "";
            txt_age.Text = "";
            cbox_gender.SelectedItem = null;
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = null;
            isDoubleClick = false;
        }

        private void cbox_gender_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
