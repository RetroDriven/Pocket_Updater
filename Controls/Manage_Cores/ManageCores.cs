﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Pannella.Services;
using Pannella.Models.OpenFPGA_Cores_Inventory;
using System.Text.Json;
using Pocket_Updater.Forms.Message_Box;
using System.Xml.Linq;
using Pannella.Helpers;

namespace Pocket_Updater.Controls.Manage_Cores
{
    public partial class ManageCores : UserControl
    {
        public List<Core> _cores;

        public SettingsService _settingsManager;
        public WebClient WebClient;
        public string Current_Dir { get; set; }
        public string updateFile { get; set; }


        CheckBox headerCheckBox = new CheckBox();
        public ManageCores()
        {
            InitializeComponent();

            Select_All();

            //dataGridView1.RefreshEdit();
            //dataGridView1.Refresh();

            //Get Cores
            _ = LoadCores();
            //_readChecklist();
            Button_Save.Enabled = true;
        }

        private void Button_Save_Click(object sender, EventArgs e)
        {
            try
            {
                Button_Save.Enabled = false;
                _readChecklist();
                _settingsManager.Save();
                ServiceHelper.ReloadSettings();

                Message_Box form = new Message_Box();
                form.label1.Text = "Core Selection Has Been Saved!";
                form.Show();

                Button_Save.Enabled = true;
            }
            catch (Exception ex)
            {
                Message_Box form = new Message_Box();
                form.label1.Text = ex.Message;
                form.Show();
                Button_Save.Enabled = true;
            }
        }
        public void _readChecklist()
        {
            for (int i = 0; i <= (dataGridView1.Rows.Count - 1); i++)
            {
                string id = (string)dataGridView1.Rows[i].Tag;
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells[0];
                if ((bool)cell.Value == false)
                {
                    _settingsManager.DisableCore(id);
                }
                else
                {
                    _settingsManager.EnableCore(id);
                }
            }
        }

        public void Download_Json()
        {
            string Json_URL = "https://raw.githubusercontent.com/mattpannella/pocket_core_autoupdate_net/main/pocket_updater_cores.json";
            WebClient = new WebClient();
            string Current_Dir = Directory.GetCurrentDirectory();
            Console.WriteLine(Current_Dir);

            try
            {
                string updateFile = Current_Dir + "\\pocket_updater_cores.json";
                WebClient.DownloadFile(Json_URL, updateFile);
                string json = File.ReadAllText(updateFile);
                _cores = JsonSerializer.Deserialize<List<Core>>(json);
                WebClient.Dispose();
            }
            catch (Exception ex)
            {
                Message_Box form = new Message_Box();
                form.label1.Text = ex.Message;
                form.Show();
            }
        }
        public void Select_All()
        {
            Point headerCellLocation = this.dataGridView1.GetCellDisplayRectangle(0, -1, true).Location;

            headerCheckBox.Location = new Point(headerCellLocation.X + 55, headerCellLocation.Y + 17);
            headerCheckBox.BackColor = Color.FromArgb(94, 148, 255);
            headerCheckBox.Size = new Size(20, 20);
            headerCheckBox.Click += new EventHandler(HeaderCheckBox_Clicked);
            dataGridView1.Controls.Add(headerCheckBox);
        }
        public void HeaderCheckBox_Clicked(object sender, EventArgs e)
        {
            //Necessary to end the edit mode of the Cell.
            dataGridView1.EndEdit();

            //Loop and check and uncheck all row CheckBoxes based on Header Cell CheckBox.
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell checkBox = (row.Cells[0] as DataGridViewCheckBoxCell);
                checkBox.Value = headerCheckBox.Checked;
            }
        }
        public async Task LoadCores()
        {
            _cores = Pannella.Helpers.ServiceHelper.CoresService.Cores;

            _settingsManager = new SettingsService(Directory.GetCurrentDirectory(), _cores);

            foreach (Core core in _cores)
            {
                if (_settingsManager.GetCoreSettings(core.identifier) != null)
                {
                    string Identifier = core.identifier.Remove(core.identifier.LastIndexOf(".") + 1);
                    string Core_Author = Identifier.Substring(0, (Identifier.Length - 1));

                    //array containing the data for the 3 columns
                    //the core platform name from the api
                    var platform = core.platform;

                    //readt he platform json file
                    var name = core.platform.name;

                    //if it finds one, use the name from there instead
                    if (platform != null)
                    {
                        name = platform.name;
                    }
                    if (core.requires_license)
                    {
                        name += " (Beta Key Required from Patreon)";
                    }
                    object[] rows = { !_settingsManager.GetCoreSettings(core.identifier).skip, name, Core_Author };
                    int index = dataGridView1.Rows.Add(rows);

                    dataGridView1.Rows[index].Tag = core.identifier;
                    dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);
                }
            }
            //Focus Grid
            dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[1];
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.EndEdit();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            bool currentValue = (bool)dataGridView1[0, e.RowIndex].Value;
            dataGridView1[0, e.RowIndex].Value = !currentValue;
        }
    }
}