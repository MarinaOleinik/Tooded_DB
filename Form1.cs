using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tooded_DB
{
    public partial class Form1 : Form
    {
        SqlConnection connect = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\AppData\Tooded.mdf; Integrated Security = True");
        //
        SqlCommand command;
        SqlDataAdapter adapter, adapter2;
        int Id = 0;
        public Form1()
        {
            InitializeComponent();
            DisplayData();
        }
        //List<string[]> kat_list = new List<string[]>();
        private BindingSource bindingSource1 = new BindingSource();
        private void DisplayData()
        {
            connect.Open();
            DataTable tabel = new DataTable();
            adapter = new SqlDataAdapter("SELECT * FROM Toodetable", connect);//, Kategooria WHERE Toodetable.Kategooria_Id=Kategooria.Id
            adapter.Fill(tabel);
            dataGridView1.DataSource = tabel;
            pictureBox1.Image = Image.FromFile("../../Images/list.png");
           
            adapter2 = new SqlDataAdapter("SELECT Kategooria_nimetus FROM Kategooria", connect);//, Kategooria WHERE Toodetable.Kategooria_Id=Kategooria.Id
            DataTable kat_tabel = new DataTable();
            adapter2.Fill(kat_tabel);
            foreach (DataRow row in kat_tabel.Rows)
            {
                comboBox1.Items.Add(row["Kategooria_nimetus"]);
            }           
            connect.Close();
        }
        private void ClearData()
        {
            //Id = 0;
            Toodetxt.Text = "";
            Kogustxt.Text = "";
            Hindtxt.Text = "";
            //save.FileName = "";
            pictureBox1.Image = Image.FromFile("../../Images/list.png");

        }

        
        private void btn_Insert_Click(object sender, EventArgs e)
        {
            if (Toodetxt.Text != "" && Kogustxt.Text != "" && Hindtxt.Text != "" && comboBox1.SelectedItem != null)
            {
                try
                {
                    command = new SqlCommand("INSERT INTO Toodetable(Toodenimetus,Kogus,Hind,Pilt,Kategooria_Id) VALUES(@toode,@kogus,@hind,@pilt,@kat)", connect);
                    connect.Open();
                    command.Parameters.AddWithValue("@toode", Toodetxt.Text);
                    command.Parameters.AddWithValue("@kogus", Kogustxt.Text);
                    command.Parameters.AddWithValue("@hind", Hindtxt.Text);
                    string file_pilt = Toodetxt.Text+".jpg";
                    command.Parameters.AddWithValue("@pilt", file_pilt);
                    command.Parameters.AddWithValue("@kat", (comboBox1.SelectedIndex+1));
                    command.ExecuteNonQuery();
                    connect.Close();
                    DisplayData();
                    ClearData();
                    MessageBox.Show("Andmed on lisatud");
                }
                catch (Exception)
                {

                    MessageBox.Show("Viga lisamisega");
                }
            }
            else
            {
                MessageBox.Show("Viga else");
            }
        }
        SaveFileDialog save;
        private void btn_Update_Click(object sender, EventArgs e)
        {
            
            if (Toodetxt.Text != "" && Kogustxt.Text != "" && Hindtxt.Text != "" && pictureBox1.Image!=null)
            {
                command = new SqlCommand("UPDATE Toodetable  SET Toodenimetus=@toode,Kogus=@kogus,Hind=@hind, Pilt=@pilt WHERE Id=@id", connect);
                connect.Open();
                command.Parameters.AddWithValue("@id", Id);
                command.Parameters.AddWithValue("@toode", Toodetxt.Text);
                command.Parameters.AddWithValue("@kogus", Kogustxt.Text);
                command.Parameters.AddWithValue("@hind", Hindtxt.Text.Replace(",","."));
                string file_pilt = Toodetxt.Text + ".jpg";
                command.Parameters.AddWithValue("@pilt", file_pilt);
                command.ExecuteNonQuery();
                connect.Close();
                DisplayData();
                ClearData();
                MessageBox.Show("Andmed uuendatud");
            }
            else
            {
                MessageBox.Show("Viga");
            }
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
            Toodetxt.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            Kogustxt.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            Hindtxt.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            pictureBox1.Image=Image.FromFile(@"C:\Users\marina.oleinik\source\repos\Tooded_DB\Images\"+dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString());
            string v = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
            comboBox1.SelectedIndex = Int32.Parse(v)-1;
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (Id!=0)
            {
                command = new SqlCommand("DELETE Toodetable WHERE Id=@id", connect);
                connect.Open();
                command.Parameters.AddWithValue("@id", Id);
                command.ExecuteNonQuery();
                connect.Close();
                DisplayData();
                ClearData();
                MessageBox.Show("Andmed on kustutatud");
            }
            else
            {
                MessageBox.Show("Viga kustutamisega");
            }
        }
        
        private void btn_LisaPilt_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpeg;*.bmp;*.png;*.jpg)|*.jpeg;*.bmp;*.png;*.jpg";
            open.InitialDirectory = Path.GetFullPath(@"C:\Users\marina.oleinik\Pictures");
            if (open.ShowDialog()==DialogResult.OK)
            {
                save = new SaveFileDialog();
                save.FileName = Toodetxt.Text+".jpg";
                save.Filter = "Image Files(*.jpeg;*.bmp;*.png;*.jpg)|*.jpeg;*.bmp;*.png;*.jpg";
                save.InitialDirectory = Path.GetFullPath(@"C:\Users\marina.oleinik\source\repos\Tooded_DB\Images");
              
                if (save.ShowDialog()==DialogResult.OK)
                {
                    File.Copy(open.FileName, save.FileName); 
                    save.RestoreDirectory = true;
                    pictureBox1.Image = Image.FromFile(save.FileName); 
                } 

            }
        }
        string Strquery;
        string text="";

     

        private void button1_Click(object sender, EventArgs e)
        {
            connect.Open();
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count-1; j++)
                {
                    text+=(dataGridView1.Rows[i].Cells[j].Value).ToString();
                }
                Strquery = @"INSERT INTO Toodetable(Id,Toodenimetus,Kogus,Hind,Pilt) " +
                    "VALUES (" + text + ")";
                command.CommandText = Strquery;
                command.ExecuteNonQuery();
            }
            connect.Close();
        }
    }
}