using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics; //stopWatch!!
/*Eli Forbes - 011468780 - CS321*/
namespace WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<Int32> randList = new List<Int32>(); //new list(vector), empty so we can add, then access by index
            Random rand = new Random();               //new Randomization object
            int i, nDistinct, listSize = 10000;                         //index, #Distinct elements
            String uMem = " unique members\r\n"; //this was typed a lot ~~> variable!

            //===================================================================================
            //create list - this method actually allowed indexing
            //===================================================================================
            for (i = 0; i < listSize; i++)
                randList.Add(rand.Next(1, 20001));  //bounds are [x,y) , thus +1

            //===================================================================================
            //#1. Hash Table
            //===================================================================================
            Dictionary<Int32, Int32> hash = new Dictionary<Int32, Int32>(10007); //(capacity = 10,007) == (prime number > 10,000)
            foreach(Int32 x in randList)                 //O(n)
                if(!hash.ContainsKey(x.GetHashCode()))   //O(1) -- check if key is alread hashed
                    hash.Add(x.GetHashCode(), x);       //O(1)
            nDistinct = hash.Count; //O(1) -- simple getter of property, pre-counted on inserts
                                    //no brackets needed because single-line scopes
            //print results
            StringBuilder res = new StringBuilder("1. HashSet method: ");
            res.Append(nDistinct.ToString());
            res.Append(uMem);
            textBox1.AppendText(res.ToString());
            //complexity analysis
            textBox1.AppendText(" The worst-case time complexity of #1. is O(n).");
            textBox1.AppendText("\r\n This is because one must loop through the list (of size n),");
            textBox1.AppendText("\r\n   and add the key:vaule pair in O(1) time,");
            textBox1.AppendText("\r\n   after checking that the value is not already in the table");
            textBox1.AppendText("\r\n    which includes a hash key calculation \r\n    and a containsKey method, both in O(1) time.");
            textBox1.AppendText("\r\n Summary: One loop and 3 constant operations => O(n)\r\n");

            //===================================================================================
            //#2. "Magic Count" ~~ Determine through List methods, using nothing extra ==========
            //===================================================================================
            nDistinct = randList.Distinct().Count();
            //space complexity here is always O(1) - uses #distinct items, 
            // no additional variables created, no containers allocated

            //print results
            StringBuilder res2 = new StringBuilder("2. O(1) storage method: ");
            res2.Append(nDistinct.ToString());
            res2.Append(uMem);
            textBox1.AppendText(res2.ToString()); //returns count of distinct items in randList
            //worst-case time complexity of Distinct() would be O(n)
            //   this is assuming it uses hashing, otherwise it might be O(N^2), O("n-squared")

            //===================================================================================
            //#3. Sorted List, Count Distinct => increment on new item, toss out like ones ======
            //===================================================================================
            randList.Sort(); //use internal sorting method

            int cur = randList[0];
            nDistinct = 1; //reset ; currently have 1st item
            
            for (i=1; i<listSize; i++)
            { //loop through sorted list - we know like items are adjacent
                if (cur != randList[i]) {   //if different item                                       
                    nDistinct++;           //increment
                    cur = randList[i];    //set to new item
                }
            }

            //print results
            StringBuilder res3 = new StringBuilder("3. Sorted method: ");
            res3.Append(nDistinct.ToString());
            res3.Append(uMem);
            textBox1.AppendText(res3.ToString()); //returns count of distinct items in randList

            textBox1.AppendText("\r\n\r\nDone!");

            Console.WriteLine("Loaded and Done!");
        }
    }
}
