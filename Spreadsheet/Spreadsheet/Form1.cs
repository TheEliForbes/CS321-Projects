using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; //open/save File Dialogs
using System.Xml; //Xml Saving/parsing
using System.Xml.Linq; //trying out LINQ
using System.IO; //Stream

using CptS321; //include my library
using static CptS321.SpreadsheetClass;
using static CptS321.ExpTree;

namespace Spreadsheet
{
    public partial class Form1 : Form
    {
        SpreadsheetClass spc;// = new SpreadsheetClass(50, 26);
        Cell editCell;
        double versionNumber = 5.0; //current Version #
        public Form1()
        {
            spc = new SpreadsheetClass(50, 26);
            spc.CellPropertyChanged += OnCellPropertyChanged; //subscribe to propertyChanged event
            InitializeComponent();
        }

        private void FormLoad(object sender, EventArgs e)
        {
            char alphabet = 'A';                  //column variable
            int rNum = 1;                         //row variable

            dataGridView1.Columns.Clear();        //clear columns from DGV
            
            while (alphabet <= 'Z')               //add columns A-Z
            {
                dataGridView1.Columns.Add(alphabet.ToString(), alphabet.ToString());
                alphabet++;
            }//dataGridView1.AutoResizeColumns(); //thins out column width

            while (rNum <= 50)                    //add 50 rows
            {
                dataGridView1.Rows.Add();
                rNum++;
            }

            rNum = 1;                             //rows are 1-indexed
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.HeaderCell.Value = rNum.ToString(); //number rows 1-50 
                rNum++;                                
            }
            //referenced the page below for the correct way to use this function
            // https://msdn.microsoft.com/en-us/library/ms158604(v=vs.110).aspx
            //dataGridView1.RowHeadersWidth = 50;
            dataGridView1.AutoResizeRowHeadersWidth(0, DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }

        private void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SpreadsheetCell cell = sender as SpreadsheetCell;
            if (e.PropertyName == "Value" || e.PropertyName == "Text")
                dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = cell.Value;                
            if (e.PropertyName == "bgColor")
                dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Style.BackColor = ColorTranslator.FromHtml(cell.bgColor);
        }        

        // referred to the page below for linking changes in the DataGridView back to the SpreadsheetEngine
        // https://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.cellendedit(v=vs.110).aspx
        private void dgv1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int rowInd = e.RowIndex;
            int colInd = e.ColumnIndex;
            SpreadsheetCell cell = spc.GetCell(rowInd, colInd) as SpreadsheetCell; // gets cell from SpreadSheetClass instance
            DataGridView dgv = sender as DataGridView;

            if (cell.Value == dgv[colInd, rowInd].ToString()) //forget identical values
                return;

            if (dgv[colInd, rowInd].Value != null) //if text in DataGridView cell, update text to trigger events
            {
                cell.Text = dgv[colInd, rowInd].Value.ToString();
                textBox1.Text = cell.Text;
            }
                //dgv[colInd, rowInd].Value = cell.Value;         //would defeat the purpose of events/handlers
        }

        private void doDemo1(object sender, EventArgs e)
        {
            int i = 0, 
                j = 1, //column 'B'
                k = 0, //randomization loop variable
                s = 0, //chatter index
                rowCount = spc.RowCount, 
                colCount = spc.ColumnCount;
            string c = "", r = "";
            string[] hellos = { "Hello World!", "Howdy World!", "Bonjour a la monde!", "Shalom Olam!", "Hey pal.." };

            Random rand = new Random(); //make randomization object
            for(; k < 50; k++)          //print out 50 random strings
            {
                do
                {
                    i = rand.Next(1, 50);   //random row #, [1,50)
                    j = rand.Next(2, 26);   //random column #, [2,26), [C,Z]
                    s = rand.Next(0, 5);    //random string #, [0,5)
                } while (spc.GetCell(i, j).Text != ""); //disallow assignment of multiple strings to one cell
                spc.GetCell(i, j).Text = hellos[s]; //set cell text to a random hello
            }

            j = 1; //column 'B'
            c = ((char)(j + 'A')).ToString(); //c == "B"
            for (i = 0; i < rowCount; i++)
            {
                r = (i + 1).ToString();
                spc.GetCell(i, j).Text = "This is cell " + c + r; //Text = "This is cell B#"
                spc.GetCell(i, j-1).Text = "=" + c + r;
            }

            dataGridView1.AutoResizeColumns(); //fit columns to data
        }

        private void doClearTable(object sender, EventArgs e)
        { //had to use nested foreach-loops, 
          //nested for-loops only modified first row (oddly..)
            foreach (Cell[] cellArr in spc.Sheet)
                foreach (Cell c in cellArr)
                {
                    c.Text = "";
                    c.clearAllDicts();
                }
            dataGridView1.AutoResizeColumns(); //fix column widths
        }

        private void OnCellClick(object sender, DataGridViewCellEventArgs e)
        {            
            int rowInd = e.RowIndex;
            int colInd = e.ColumnIndex;
            SpreadsheetCell cell = spc.GetCell(rowInd, colInd) as SpreadsheetCell; // gets cell from SpreadSheetClass instance
            editCell = cell;
            // ensure text is not null / empty
            if (cell.Text != null)//|| cell.Text != ""
                textBox1.Text = cell.Text; // getting the value at the specified cell and setting it to the appropriate text        
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                editCell.Text = textBox1.Text;
        }
        private void CellKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (editCell.RowIndex < 49) //< (50-1)
                    editCell = spc.GetCell(editCell.RowIndex + 1, editCell.ColumnIndex);
                if(editCell != null && editCell.Text != null)
                    textBox1.Text = editCell.Text;
            }
            if (e.KeyCode == Keys.Up)
            {
                if(editCell.RowIndex > 0)
                    editCell = spc.GetCell(editCell.RowIndex - 1, editCell.ColumnIndex);
                if (editCell != null && editCell.Text != null)
                    textBox1.Text = editCell.Text;
            }
            if (e.KeyCode == Keys.Down)
            {
                if (editCell.RowIndex < 49)
                    editCell = spc.GetCell(editCell.RowIndex + 1, editCell.ColumnIndex);
                if (editCell != null && editCell.Text != null)
                    textBox1.Text = editCell.Text;
            }
            if (e.KeyCode == Keys.Right)
            {
                if (editCell.ColumnIndex < 25) // < (26-1)
                    editCell = spc.GetCell(editCell.RowIndex, editCell.ColumnIndex + 1);
                if (editCell != null && editCell.Text != null)
                    textBox1.Text = editCell.Text;
            }
            if (e.KeyCode == Keys.Left)
            {
                if (editCell.ColumnIndex > 0)
                    editCell = spc.GetCell(editCell.RowIndex, editCell.ColumnIndex - 1);
                if (editCell != null && editCell.Text != null)
                    textBox1.Text = editCell.Text;
            }
        }

        private void doCascadingDemo(object sender, EventArgs e)
        {
            spc.GetCell(0, 1).Text = "1"; spc.GetCell(0, 2).Text = "2"; 
            spc.GetCell(1, 1).Text = "3"; spc.GetCell(1, 2).Text = "4";  
            spc.GetCell(0, 0).Text = "=B1+C1";
            spc.GetCell(1, 0).Text = "=B2+C2";
            spc.GetCell(2, 0).Text = "=A1+A2";
            //spc.GetCell(2, 0).Text = "=A1+";
            //spc.GetCell(3, 0).Text = "Wait on it..";
            //System.Threading.Thread.Sleep(2000);
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            
            //t.Interval = 2000; // specify interval time as you want         
            //t.Start();
            //t.Stop();
            spc.GetCell(3, 1).Text = "=A1+B2/(C2-C1)";
            spc.GetCell(4, 1).Text = "You can fill in the rest..";
        }

        private void LoadXMLSheet(object sender, EventArgs e)
        {
            doClearTable(sender, e); //clear table before loading?

            OpenFileDialog ofd = new OpenFileDialog();                 //instantiate new openDialog
            ofd.Filter = "XML Files(.xml)|*.xml|all Files(*.*)|*.*";//filter for .txt or .* files
            ofd.FilterIndex = 1;
            ofd.Multiselect = true; //allows selection of multiple files

            if(ofd.ShowDialog() == DialogResult.OK) //if OK clicked, file is selected from dialog
            {
                Stream fStream = ofd.OpenFile(); //implicit open for read
                using (StreamReader sr = new StreamReader(fStream)) //open stream for duration of 'using' (?)
                {
                    spc.LoadSheet(sr); //given a StreamReader, we can use loadText to read everything!
                }
                fStream.Close(); //close stream, done with it
            }
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoResizeRows(); //resize rows & columns
        }

        private void SaveXMLSheet(object sender, EventArgs e)
        {
            Stream myStream;                             //declare stream
            SaveFileDialog sfd = new SaveFileDialog();   //instantiate saveFileDialog

            sfd.Filter = "XML Files(.xml)|*.xml";// .xml and .* files
            sfd.FilterIndex = 2;
            sfd.RestoreDirectory = true;                                //should keep directory & files in place

            if (sfd.ShowDialog() == DialogResult.OK) //user 'saved' file (chose name, clicked OK)
            {                                       //turn dialog into stream, open file  
                if ((myStream = sfd.OpenFile()) != null)
                {
                    spc.SaveSheet(myStream);   //write to stream
                    myStream.Close();                   //close stream, done with it
                }
            }
        }

        private void AboutPopup(object sender, EventArgs e)
        {
            string versionString = string.Format("{0:0.00}",versionNumber);
            DialogResult result = MessageBox.Show(
                "!XCell (\"NotXCell\") -- v" + versionString + 
             "\n\nAn application for spreadsheeting"+
               "\nWhatever that means.."+
             "\n\nCreated by Eli Forbes - eli.forbes@wsu.edu"+              
               "\n\t   The One and Only.."+
               "\n\nCopyright (2018) -- (CC BY-NC-SA 4.0)\n\n"+
              "Warranty: If you break it, it's broken. Schucks buck.",
              
                "About !XCell",
                MessageBoxButtons.AbortRetryIgnore);

            //var form = new Form();
            //form.Text = "\tAn application for spreadsheeting\n\tWhatever that means..";
            //form.Name = "!XCell -- v5.02";
            //form.Show();
                
        }
        
    }
}

//this function is pointless because the DataGridView is subscribed to changes in the Spreadsheet Cells
// https://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.cellbeginedit(v=vs.110).aspx
//private void dgv1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
//{
//    int rowInd = e.RowIndex;
//    int colInd = e.ColumnIndex;
//    SpreadsheetCell cell = spc.GetCell(rowInd, colInd) as SpreadsheetCell; // gets cell from SpreadSheetClass instance

//    // ensure text is not null / empty
//    if (cell.Text != null)//|| cell.Text != ""
//        textBox1.Text = cell.Text; // getting the value at the specified cell and setting it to the appropriate text            
//}

