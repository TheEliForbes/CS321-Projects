using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Math; //allows direct calling of Log function
using static System.Console; //allows direct calling of Write, WriteLine, etc.

public abstract class BinTree<T> where T : IComparable
{
    public abstract void Insert(T val); // Insert new item of type T
    public abstract bool Contains(T val); // Returns true if item is in tree
    public abstract void InOrder(); // Print elements in tree inorder traversal
    public abstract void PreOrder();
    public abstract void PostOrder();
}
class Node<T> where T : IComparable {
                //make T comparable
    private T data;   //node value
    public Node<T> left;   //left node
    public Node<T> right;  //right node

    //Node constructor
    public Node(T val) {
        this.data = val;
        left = null;
        right = null;
    }
    //data getter
    public T Data {
        get { return data; }     
    }

    public static int Comparison(Node<T> n1, Node<T> n2)
    {
        if ((object)n1 == null || (object)n2 == null)
            return 0;
        if (n1.Data.CompareTo(n2.Data) < 0)
            return -1;
        else if (n1.Data.CompareTo(n2.Data) == 0)
            return 0;
        else if (n1.Data.CompareTo(n2.Data) > 0)
            return 1;
        return 0;
    }
    public static bool operator <(Node<T> n1, Node<T> n2)
    {
        return Comparison(n1, n2) < 0;
    }
    public static bool operator <=(Node<T> n1, Node<T> n2)
    {
        int comp = Comparison(n1, n2);
        return (comp < 0 || comp == 0);
    }
    public static bool operator >=(Node<T> n1, Node<T> n2)
    {
        int comp = Comparison(n1, n2);
        return (comp > 0 || comp == 0);
    }
    public static bool operator >(Node<T> n1, Node<T> n2)
    {
        return Comparison(n1, n2) > 0;
    }
    public static bool operator ==(Node<T> n1, Node<T> n2)
    {
        if (Object.ReferenceEquals(n1, null) && Object.ReferenceEquals(n2, null))
            return true;
        if (Object.ReferenceEquals(n1, null) || Object.ReferenceEquals(n2, null))
            return false;
        if (Object.ReferenceEquals(n1, n2))
            return true;
        if (n1.GetType() != n2.GetType())
            return false;
        if (Comparison(n1, n2) == 0)
            return true;
        else return false;
    }
    public static bool operator !=(Node<T> n1, Node<T> n2)
    {
        return !(n1 == n2);
    }
    public override bool Equals(object obj)
    {   //use overridden operators
        return (this == (Node<T>)obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }//it complained when I didn't 'override' this
}
//======================================================================
//======================================================================
class BinaryTree<T> : BinTree<T> where T : IComparable {
    private Node<T> root; //tree root
    private int count; //used in count function

    //constructor
    public BinaryTree() {
        root = null;
        count = 0;
    }
        
    //fancy C# getter for empty check
    public bool isEmpty {
        get { return (root == null);  }
    }

    public int counter() {
        count = 0; //clear, in case previously set
        countHelper(root);
        return count;
    }     
    private void countHelper(Node<T> n) {
        if(n != null)
        {
            count++;
            if(n.left != null)
                countHelper(n.left);
            if(n.right != null)
                countHelper(n.right);
        }
    }

    //insertion interface
    public override void Insert(T data) {
        Node<T> temp = new Node<T>(data);
        if (isEmpty) this.root = temp;
        else
            insertHelp(root, temp);
    }
    //insertion, using overriden operators
    private bool insertHelp(Node<T> rt, Node<T> data) {
        if (data == rt)
            return false;
        else if (data < rt) {
            if (rt.left == null) {
                rt.left = data;
                return true;
            }
            else
                insertHelp(rt.left, data);
        }
        else {// if (data > rt)        
            if (rt.right == null) {
                rt.right = data;
                return true;
            }
            else
                insertHelp(rt.right, data);
        }
        return false;//error if here
    }

    //contains interface
    public override bool Contains(T data)
    {
        Node<T> temp = new Node<T>(data);
        if (isEmpty) return false;
        else {
            return containsHelp(root, temp);
        }
    }
    //contains, using overriden operators
    private bool containsHelp(Node<T> rt, Node<T> data)
    {
        if (data == rt)
            return true;
        else if (data < rt) {
            if (rt.left == null)
                return false;
            else
                containsHelp(rt.left, data);
        }
        else {// if (data > rt)        
            if (rt.right == null)
                return false;
            else
                containsHelp(rt.right, data);
        }
        return false;//error if here
    }

    //in-order traversal interface
    public override void InOrder() {
        inOrderHelper(root);
    }
    //in-order traversal
    private void inOrderHelper(Node<T> n) {
        if(n != null) //ensure that we have data
        {
            if (n.left != null)
            inOrderHelper(n.left); //recurse left
            Console.Write("{0} ", n.Data.ToString());//print
            if (n.right != null)
                inOrderHelper(n.right);//recurse right
        }
    }

    //pre-order traversal interface
    public override void PreOrder()
    {
        preOrderHelper(root);
    }
    //pre-order traversal
    private void preOrderHelper(Node<T> n)
    {
        if (n != null) //ensure that we have data
        {
            Console.Write("{0} ", n.Data.ToString());//print
            if (n.left != null)
                preOrderHelper(n.left); //recurse left
            if (n.right != null)
                preOrderHelper(n.right);//recurse right
        }
    }

    //post-order traversal interface
    public override void PostOrder()
    {
        postOrderHelper(root);
    }
    //post-order traversal
    private void postOrderHelper(Node<T> n)
    {
        if (n != null) //ensure that we have data
        {
            if (n.left != null)
                postOrderHelper(n.left); //recurse left
            if (n.right != null)
                postOrderHelper(n.right);//recurse right
            Console.Write("{0} ", n.Data.ToString());//print
        }
    }

    //depth-finder interface
    public int maxDepth() {
        return maxDepthHelper(root);
    }
    //depth-finder
    private int maxDepthHelper(Node<T> n) {
        int ldepth=0, rdepth=0;
        if (n == null)//base case
            return 0;
        else {
            if(n.left != null)
                ldepth = maxDepthHelper(n.left); //get max height in left tree
            if(n.right != null)
                rdepth = maxDepthHelper(n.right);//get max height in right tree
            
            return ((ldepth < rdepth) ? ++rdepth : ++ldepth); //return max++, ternary operator fun!
        }
    }

    public int idealHeight() {
        return idealHeightHelper(root);
    }
    private int idealHeightHelper(Node<T> n) {
    //log base2 (n+1) -1 == most accurate calculation for ideal tree height
    //instead of log base2 (n) , as given in cs223
        int nodeCount = counter(); //need count for ideal height calculation
        return (int)Log((double)nodeCount + 1, 2.0); //log2(n+1)-1
    }

    //interface function for printing the tree, passes all necessary variables
    public void printTree() {
        String dent = "";
        int i = 0;
        bool last = false;
        int count = counter();
        //init variables then call function with root
        prettyTree(root, dent, i, count, last);
    }
    //function for printing out the tree structure in a visually pleasing way
    private void prettyTree(Node<T> rt, String dent, int i, int count, bool last) {
        //credit for +-| structure goes to Joshua Stachowski
           //https://stackoverflow.com/questions/1649027/how-do-i-print-out-a-tree-structure
        //adapted to this BST implementation by me, Eli Forbes
        WriteLine(dent + "+--" + rt.Data.ToString());
        dent += last ? "   " : "   |";
        if (rt.left != null) {
            last = (i == count - 1);
            prettyTree(rt.left, dent, ++i, count, last);
        }
        if (rt.right != null) {
            last = (i == count - 1);
            prettyTree(rt.right, dent, i, count, last);
        }
    }

    //this got repeated too much, so make it callable!
    public void showStats()
    {
        Write("Tree contents: ");
        InOrder();
        WriteLine();

        WriteLine("Tree statistics:");
        int numNodes = counter(); //assign to variable, used 2x
        WriteLine("  Number of nodes: {0}", numNodes);
        WriteLine("  Number of levels: {0}", maxDepth()); //direct calculation, since used 1x
        WriteLine("  Ideal Height for Tree of {0} Nodes = {1}", numNodes, idealHeight());

        WriteLine("\nBonus! Print Tree!");
        printTree();
    }

    public void makeTree(string[] sList, Object t)
    {
        int num = 0;
        double d = 0.0;
        char c = '\0';
        bool res = false;

        if (t.GetType() == typeof(Int32))
        {
            foreach (string s in sList) //for all strings
            {
                res = Int32.TryParse(s, out num);
                if (res)
                    Insert((T)(object)num); //parse & insert
                else
                    WriteLine("Parsing error..");
            }
        }
        else if (t.GetType() == typeof(Double))
        {
            foreach (string s in sList) //for all strings
            {
                res = Double.TryParse(s, out d);
                if (res)
                    Insert((T)(object)d); //parse & insert
                else
                    WriteLine("Parsing error..");
            }
        }
        else if (t.GetType() == typeof(Char))
        {
            foreach (string s in sList) //for all strings
            {
                res = Char.TryParse(s, out c);
                if (res)
                    Insert((T)(object)c); //parse & insert
                else
                    WriteLine("Parsing error..");
            }
        }
        else //(t.GetType() == typeof(string))
        {
            foreach (string s in sList) //for all strings
                Insert((T)(object)s); //parse & insert
        }
        showStats();   //show tree stats
    }
}
//======================================================================
//======================================================================
class BSTtree {
    public static char menu()
    {
        Write("Decide: [ int | double | char | string | quit ]\n~[i|d|c|s|q]: ");
        char decision = (char)Read();
        ReadLine();//clear buffer

        switch (decision)
        {
            case 'i':
                WriteLine("Enter a collection of integers, separated by spaces:");
                break;
            case 'd':
                WriteLine("Enter a collection of decimal values, separated by spaces:");
                break;
            case 'c':
                WriteLine("Enter a collection of characters, separated by spaces:");
                break;
            case 's':
                WriteLine("Enter a collection of gibberish, separated by spaces:");
                break;
            case 'q':
                WriteLine("Press any key to continue . . .");
                break;
            default:
                WriteLine("Enter a collection of gibberish, separated by spaces:");
                break;
        }
        return decision;
    }

    static void Main(string[] args) {
        //tree declarations, for all directly supported types
        BinaryTree<int> iTree = new BinaryTree<int>();
        BinaryTree<double> dTree = new BinaryTree<double>();
        BinaryTree<char> cTree = new BinaryTree<char>();
        BinaryTree<string> sTree = new BinaryTree<string>();
        
        int i = 0;       //variables for all directly supported types
        double d = 0.0;
        char c = '\0';
        string str = "";
        bool running = true;
        
        while (running == true)
        {
            char typeDecision = menu();
                        
            string input = ReadLine(); //get values
            string[] sList = input.Split(' '); //split by spaces, into a list                   

            switch (typeDecision) {
                case 'i': iTree.makeTree(sList, i); //int case, use iTree
                    break;
                case 'd': dTree.makeTree(sList, d); //double case, use dTree
                    break;
                case 'c': cTree.makeTree(sList, c); //char case, use cTree
                    break;
                case 's': sTree.makeTree(sList, str); //string case, use sTree
                    break;
                case 'q': running = false; //stop program
                    break;
                default: sTree.makeTree(sList, str); //default case, take any key(s) with strings
                    break;
            }
            WriteLine("\n");//separate with newlines
        }
        WriteLine("\nDone");
    }
}

