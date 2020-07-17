using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Agenda_Virtuel.Plugin;
using System.Collections.Generic;

namespace Agenda_Virtuel
{
    internal partial class PluginInstallerWindow : Window
    {
        //Enums
        public enum Mode { Add, Edit }

        //Variables
        private OpenFileDialog ofd;
        private Mode mode;
        internal PluginLoader plugin;
        public string[] secDlls;

        //Constructors
        private void CommonConstructor()
        {
            InitializeComponent();

            ofd = new OpenFileDialog()
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "dll",
                Filter = "|*.dll"
            };
        }

        public PluginInstallerWindow()
        {
            CommonConstructor();

            mode = Mode.Add;
        }

        internal PluginInstallerWindow(PluginLoader _plugin, Mode _mode = Mode.Edit)
        {
            CommonConstructor();

            mode = _mode;
            plugin = _plugin;

            this.TB_DllPath.Text = _plugin.DllPath;
            this.TB_Name.Text = _plugin.name;
            string[] tmp = _plugin.PluginClass.Split('.');
            this.TB_Namespace.Text = tmp[0];
            this.TB_MainClass.Text = tmp[1];
           // this.CB_Activate.Checked = _plugin.Enabled;

            if (_mode == Mode.Edit)
            {
                this.BT_ChooseDll.IsEnabled = false;
               // this.CB_Activate.Visible = true;
            }
        }

        //Select the dll file
        private void BT_ChooseDll_Click(object sender, RoutedEventArgs e)
        {
            ofd.Title = "Sélectionnez le dll de votre plugin";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.TB_DllPath.Text = ofd.FileName;
                this.TB_Name.Text = new FileInfo(ofd.FileName).Name.Replace(".dll", "");

                PluginLoader p = new PluginLoader();
                PluginManager.LoaderToAppDomain(ref p);
                p.HasError(ofd.FileName);
                
                if(p != null)
                {
                    List<Type> types = new List<Type>(p.assembly.GetTypes());
                    Type plugType = types.Find((x) => x.IsSubclassOf(typeof(Plugin.Plugin)));

                    if (plugType != null)
                    {
                        this.TB_Namespace.Text = plugType.Namespace;
                        this.TB_MainClass.Text = plugType.Name;

                        this.BT_Save.IsEnabled = true;
                    }
                    else
                    {
                        this.BT_Save.IsEnabled = false;
                        System.Windows.Forms.MessageBox.Show("La class de type ''" + nameof(Plugin.Plugin) +"'' n'a pa été trouvé !", "Agenda - Virtuel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    p.Unload(false);
                }

            }
        }

        //Valiidate and add or edit the plugin
        private void BT_Save_Click(object sender, RoutedEventArgs e)
        {
            //Check if all field are completed
            if (string.IsNullOrEmpty(this.TB_DllPath.Text) || string.IsNullOrEmpty(this.TB_Namespace.Text) ||
                string.IsNullOrEmpty(this.TB_MainClass.Text) || string.IsNullOrEmpty(this.TB_Name.Text) || string.IsNullOrEmpty(this.TB_Version.Text))
            {
                System.Windows.Forms.MessageBox.Show("Entrez tous les champs.", "Agenda - Virtuel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //Get plugin's informations
            string dllPath = this.TB_DllPath.Text;
            string name = this.TB_Name.Text;
            string agPluginClass = this.TB_Namespace.Text + "." + this.TB_MainClass.Text;
            string description = this.TB_Description.Text;
            string version = this.TB_Version.Text;

            PluginLoader newPlugin;

            FileInfo dllFile = new FileInfo(dllPath);

            if (mode == Mode.Edit)
            {
                newPlugin = plugin;
                newPlugin.PluginClass = agPluginClass;
                newPlugin.name = name;
                newPlugin.description = description;
                newPlugin.version = version;
            }
            else
            {
                newPlugin = new PluginLoader(dllFile.Name, agPluginClass, name, version);
                newPlugin.description = description;
            }

            //Check if there is any error
            if (newPlugin.HasError(dllPath))
            {
                newPlugin.ShowError();
                return;
            }



            if (mode == Mode.Edit)
            {
                //Enable the plugin
                // newPlugin.Enabled = this.CB_Activate.Checked;
                PluginManager.Save();
            }

            plugin = newPlugin;

            secDlls = new string[this.LV_Dlls.Items.Count];
            for (int i = 0; i < this.LV_Dlls.Items.Count; i++)
            {
                secDlls[i] = this.LV_Dlls.Items[i] as string;
            }

            //Close this window
            this.DialogResult = true;
            this.Close();
        }

        private void BT_AddDll_Click(object sender, RoutedEventArgs e)
        {
            ofd.Title = "Sélectionnez les DLLs supplémentaires";
            ofd.Multiselect = true;

            if (this.ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string d in ofd.FileNames)
                {
                    this.LV_Dlls.Items.Add(d);
                }
            }
        }

        private void BT_RemoveDll_Click(object sender, RoutedEventArgs e)
        {
            if (this.LV_Dlls.SelectedItem != null)
            {
                this.LV_Dlls.Items.Remove(this.LV_Dlls.SelectedItem);
            }
        }
    }
}
