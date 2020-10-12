using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading; //Thread
using System.Diagnostics; //Stopwatch
using static System.Console; //Write, WriteLine
using static System.DateTimeOffset; //Timer (unsuccessful)
namespace MergeSort
{
    class Program
    {
        static void printList(List<int> l)
        {            
            foreach (var i in l)
                WriteLine(i.ToString());
        }
        static void Merge(int[] arr, int low, int mid, int high)
        {
            int i = 0,                  //Index
                left = low,             //L-Bound
                right = mid + 1,        //R-Bound
                size = high - low + 1;  //Size of current slice
            int[] temp = new int[size]; //Temporary Array

            while(left <= mid && right <= high) //if two halves
                if (arr[left] < arr[right])     //insert smallest pieces first
                    temp[i++] = arr[left++];
                else
                    temp[i++] = arr[right++];

            while (left <= mid)                 //if one half
                temp[i++] = arr[left++];        //insert into array
            while(right <= high)                //if one half
                temp[i++] = arr[right++];       //insert into array

            for (i = 0; i < temp.Length; i++)   //transfer data over
                arr[low + i] = temp[i];
        }
        private static void MergeSort(int[] arr, int low, int high)
        {
            if(low < high)
            {
                int mid = (low / 2) + (high / 2);
                MergeSort(arr, low, mid);
                MergeSort(arr, mid+1, high);
                Merge(arr, low, mid, high);
            }
        }
        static void ThreadSort(int[] arr, int low, int high)
        {
            if(low < high)
            {
                int mid = (low / 2) + (high / 2);                
                Thread tLow = new Thread(MaryHad => ThreadSort(arr, low, mid)); //lambda expressions are useful
                Thread tHigh = new Thread(aLittle => ThreadSort(arr, mid+1, high));
                Thread tMerge = new Thread(Lambda => Merge(arr, low, mid, high));
                tLow.Start();
                tHigh.Start();
                tMerge.Start(); //start all threads
                tLow.Join();
                tHigh.Join();
                tMerge.Join();
            }
        }
        static void FixArray(List<int> la, int len, int[] arr = null)
        {
            Random rand = new Random();
            int k = 0;

            la.Clear();            
            for (; k < len; k++)
                la.Add(rand.Next(0, Int32.MaxValue));
            if(arr != null)
                arr = la.ToArray();
        }
        static void Main(string[] args)
        {
            int j = 0;
            int[] lengths = { 8, 64, 256, 1024 };
            TimeSpan dur1;
            Random rand = new Random();
            List<int>[] listArr = new List<int>[lengths.Length];

            WriteLine("Starting tests of MergeSort vs. ThreadedMergeSort");            
            for(j = 0; j < 4; j++)
            {
                listArr[j] = new List<int>();
                FixArray(listArr[j], lengths[j]);
                //printList(listArr[j]);

                Write(" Starting test for size " + lengths[j].ToString());
                int[] arr1 = listArr[j].ToArray();
                
                Stopwatch time = Stopwatch.StartNew();
                MergeSort(arr1, 0, arr1.Length - 1);
                time.Stop();
                dur1 = time.Elapsed;              
                WriteLine(" - Test completed:");
                WriteLine("   Normal Sort Time (ms):\t{0}", dur1.TotalMilliseconds);

                FixArray(listArr[j], arr1.Length, arr1); //re-randomize
                //arr1 = listArr[j].ToArray(); //when you could just re-assign the previous array (for consistency)

                time = Stopwatch.StartNew();                
                Thread tMergeSort = new Thread(magic => ThreadSort(arr1, 0, arr1.Length-1));//MergeSort(arr1, 0, arr1.Length - 1);
                time.Stop();
                dur1 = time.Elapsed;                
                WriteLine("   Threaded Sort Time (ms):\t{0}", dur1.TotalMilliseconds);
                
                //printList(arr1.ToList());
                //ReadLine();
            }           
        }
    }
}
