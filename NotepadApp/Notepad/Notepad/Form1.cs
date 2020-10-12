/*
- Eli Forbes - 011468780 - eecsGithub:eforbes1
- WinForms Notepad Application
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace Notepad
{
    
    public partial class Form1 : Form
    {
        public class FibonacciTextReader : System.IO.TextReader
        {              //fibTextReader inherits from TextReader
            private int numLines;       //furthest readable line
            private int curLine;        //current line being read
            private BigInteger f1;      //fib. intermediate result
            private BigInteger f2;      //fib. intermediate result
            private BigInteger result;  //fib. result - Displayed
            public FibonacciTextReader(int lines = 1)
            {
                if (lines < int.MaxValue)   //ensure #lines is reasonable
                    numLines = lines;       //set accordingly
                curLine = 0;                //start at 0, F(0)=0
                f1 = 0; f2 = 1; result = 0; //calculation variables, used when line>1
            }

            public override string ReadLine()
            {
                if (curLine <= numLines) //while 'readable' (still have fib lines)
                    return curLine.ToString() + ": " + doFib(curLine++).ToString() + " \r\n";
                    //return formatted string
                else 
                    return ""; //return 'null' string
            }
            public override string ReadToEnd()
            {
                StringBuilder res = new StringBuilder(""); //Use to create whole 'list'

                while (curLine <= numLines)
                    res.Append(ReadLine()); //read formatted line, formatting in ReadLine()

                return res.ToString();
            }

            public BigInteger doFib(BigInteger n)
            {
                if (n <= 1)//base cases: F(0)=0, F(1)=1
                    return n;
                else {
                    result = f1 + f2; //res = F(n-2) + F(n-1)
                    f1 = f2; //F(n-2) = F(n-1) //advance
                    f2 = result; //F(n-1) = F  //advance
                    return result; //return F(n)
                }
                /*
                if (n <= 1)     //recursion takes too long...
                    return n;           
                return (BigInteger)doFib(n - 1) + (BigInteger)doFib(n - 2);
                */
            }
        }
        //==================================================================
        public Form1()
        {   //starts everything!
            InitializeComponent();
        }

        private void loadText(System.IO.TextReader sr)
        {      
            textBox1.Text = sr.ReadToEnd(); //replace all contents of textBox
            //ReadToEnd() forms a string with a StringBuilder, thus equality
        }
    
        private void loadFile(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();                 //instantiate new openDialog
            ofd.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";//filter for .txt or .* files
            ofd.FilterIndex = 1;
            ofd.Multiselect = true; //allows selection of multiple files

            if(ofd.ShowDialog() == DialogResult.OK) //if OK clicked, file is selected from dialog
            {
                System.IO.Stream fStream = ofd.OpenFile(); //implicit open for read
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fStream)) //open stream for duration of 'using' (?)
                {
                    loadText(sr); //given a StreamReader, we can use loadText to read everything!
                }
                fStream.Close(); //close stream, done with it
            }
        }

        private void addText(System.IO.Stream fs, string s)
        {
            UTF8Encoding utf8 = new UTF8Encoding(); //new encoder for UTF8. Needed for conversion
            Byte[] info = utf8.GetBytes(s);         //use encoder to convert string to bytes
            fs.Write(info, 0, info.Length);         //write byte info to Stream
        }
        private void saveFile(object sender, EventArgs e)
        {
            System.IO.Stream myStream;                      //declare stream
            SaveFileDialog sfd = new SaveFileDialog();      //instantiate saveDialog

            sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"; //filter for .txt and .* files
            sfd.FilterIndex = 2;
            sfd.RestoreDirectory = true;                                //should keep directory & files in place

            if(sfd.ShowDialog() == DialogResult.OK) //user 'saved' file (chose name, clicked OK)
            {                                       //turn dialog into stream, open file  
                if((myStream = sfd.OpenFile()) != null)  {
                    addText(myStream, textBox1.Text);   //write to stream
                    myStream.Close();                   //close stream, done with it
                }
            }
        }

        private void displayFibFifty(object sender, EventArgs e)
        {   //new FibTextReader, max line count == 50, done by constructor
            FibonacciTextReader fib50 = new FibonacciTextReader(50);
            loadText(fib50);    //reads FibTextReader to end
        }
        private void displayFibHundred(object sender, EventArgs e)
        {   //new FibTextReader, max line count == 100, done by constructor
            FibonacciTextReader fib100 = new FibonacciTextReader(100);
            loadText(fib100);    //reads FibTextReader to end
        }
    }
}
