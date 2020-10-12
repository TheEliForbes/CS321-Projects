using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Math; //allows direct calling of Log function
using static System.Console; //allows direct calling of Write, WriteLine, etc.

class Node {
    private int data;   //node value
    public Node left;   //left node
    public Node right;  //right node

    //Node constructor
    public Node(int val=0) {
        this.data = val;
        left = null;
        right = null;
    }
    //data getter
    public int Data {
        get { return data; }     
    }
    public static int Comparison(Node n1, Node n2)
    {

        if (n1.data < n2.data)

            return -1;

        else if (n1.data == n2.data)

            return 0;

        else if (n1.data > n2.data)

            return 1;

        return 0;

    }
    public static bool operator <(Node n1, Node n2)
    {

        return Comparison(n1, n2) < 0;

    }

    public static bool operator >(Node n1, Node n2)
    {

        return Comparison(n1, n2) > 0;

    }
    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (Object.ReferenceEquals(this, obj))
            return true;
        if (this.GetType() != obj.GetType())
            return false;
        Node temp = (Node)obj;
        if (this.data == temp.data)
            return true;
        else return false;
    }
}
//======================================================================
//======================================================================
class BinaryTree {
    private Node root; //tree root
    private int count; //used in count function

    //constructor
    public BinaryTree() {
        root = null;
        count = 0;
    }
        
    //fancy C# getter for empty check
    public bool isEmpty {
        get { return root == null;  }
    }

    public int counter() {
        count = 0; //clear, in case previously set
        countHelper(root);
        return count;
    }     
    private void countHelper(Node n) {
        if(n != null)
        {
            countHelper(n.left);
            count++;
            countHelper(n.right);
        }
    }

    //insertion interface
    public void insert(int data) {
        Node temp = new Node(data);
        if (isEmpty) this.root = temp;
        else {
            insertHelp(root, temp);
        }
    }
    //insertion, using overriden operators
    private bool insertHelp(Node rt, Node data) {
        if (data.Equals(rt))
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
        
    //traversal interface
    public void inOrderTraversal() {
        inOrderHelper(root);
    }
    //in-order traversal
    public void inOrderHelper(Node n) {
        if(n != null) //ensure that we have data
        {
            inOrderHelper(n.left); //recurse left
            Console.Write("{0} ", n.Data);//print
            inOrderHelper(n.right);//recurse right
        }
    }

    //depth-finder interface
    public int maxDepth() {
        return maxDepthHelper(root);
    }
    //depth-finder
    private int maxDepthHelper(Node n) {
        if (n == null)//base case
            return 0;
        else {
            int ldepth = maxDepthHelper(n.left); //get max height in left tree
            int rdepth = maxDepthHelper(n.right);//get max height in right tree
            
            return ((ldepth < rdepth) ? ++rdepth : ++ldepth); //return max++, ternary operator fun!
        }
    }

    public int idealHeight() {
        return idealHeightHelper(root);
    }
    private int idealHeightHelper(Node n) {
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
    private void prettyTree(Node rt, String dent, int i, int count, bool last) {
        //credit for +-| structure goes to Joshua Stachowski
           //https://stackoverflow.com/questions/1649027/how-do-i-print-out-a-tree-structure
        //adapted to this BST implementation by me, Eli Forbes
        WriteLine(dent + "+--" + rt.Data);
        dent += last ? "   " : "   |";
        if(rt.left != null) {
            last = (i == count - 1);
            prettyTree(rt.left, dent, ++i, count, last);
        }
        if(rt.right != null) {
            last = (i == count - 1);
            prettyTree(rt.right, dent, i, count, last);
        }
    }
}
//======================================================================
//======================================================================
class BSTtree {

    static void Main(string[] args) {
        BinaryTree t = new BinaryTree();

        WriteLine("Enter a collection of numbers in the range [0, 100], separated by spaces:");

        string input = ReadLine();
        string[] sList = input.Split(' ');
        int[] nList = new int[sList.Length]; //new array of length of string list

        foreach (string s in sList) //for all strings
        {
            int num;
            bool res = Int32.TryParse(s, out num);
            if (res)
                t.insert(num); //parse & insert
            else
                WriteLine("Parsing error..");
        }

        Write("Tree contents: ");
        t.inOrderTraversal();
        WriteLine();

        WriteLine("Tree statistics:");
        int numNodes = t.counter(); //assign to variable, used 2x
        WriteLine("  Number of nodes: {0}", numNodes);
        WriteLine("  Number of levels: {0}", t.maxDepth()); //direct calculation, since used 1x
        WriteLine("  Ideal Height for Tree of {0} Nodes = {1}", numNodes, t.idealHeight());

        WriteLine("\nBonus! Print Tree!");
        t.printTree();

        WriteLine("\nDone");
    }
}
