using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics; //Debug.WriteLine
using System.Xml;
using System.Xml.Linq; //XDocument, etc.
using System.IO; //Stream
//using System.Windows.Forms;

//namespace SpreadsheetEngine
//{
//    public class Class1
//    {
//    }
//}

namespace CptS321
{
    public class ExpTree
    {
        //Expression Tree Structures -- treeRoot, var:val mapping, currentExpression
        private OpNode root;                         //tree root
        private Dictionary<string, double> variables;//variable reference dictionary
        private string currentExpression;
        //shuntingYard Structures
        private Stack<Node> operands;                //operand queue
        private Stack<OpNode> operators;             //operator stack
        //private Stack<Node> output;                  //output stack
        private Dictionary<string, int> precedence;  //precedence mapping               
        private string[] expressionTokens;
        private double evalResult; //hold result of Eval() -- save time, don't compute multiple times

        public string CurExp { get { return currentExpression; } } //allow tree to tell its current expression

        //Expression Tree Constructor
        //--builds ExpTree according to given expression, with default expression if never supplied one
        public ExpTree(Cell c, Cell[][] spc, string expression = "4+18/(9-3)")
        {
            root = new OpNode('_');                       //'_' == special marker for base tree-building case
            currentExpression = expression;               //hold the expression -- allows re-parsing -- not needed once I build the tree for real?
            variables = new Dictionary<string, double>(); //create new dictionary for variableName:value mapping
            operands = new Stack<Node>();                 //shuntingYard Operand Stack
            operators = new Stack<OpNode>();              //shuntingYard Operator Stack
            //output = new Stack<Node>();                   //shuntingYard Output Stack
            precedence = new Dictionary<string, int>()    //shuntingYard Precedence Mapping
                         {{"(", 0},{")", 3}, {"*", 2}, {"/", 2}, {"+", 1}, {"-", 1}};
            evalResult = -42.0;                             //make initial value an uncommon result
            expressionTokens = parseExp(ref c, ref spc, currentExpression); //use self-made parser, couldn't get my RegEx right..
            if (expressionTokens[0] == "" || isOperator(expressionTokens[0]))
            {
                c.Text = "#REF!";
                c.bgColor = "Red";
                c.clearRefDict(); //clear refs so it can re-evaluate
            }
            //shuntYard(expressionTokens);                      //convert to RPN & build tree with Shunting Yard Algorithm
        }

        private bool isSymbol(string s) //check if operator[+-*/] or parenthesis[()]
        {
            if (s == "(" || s == ")" || s == "+" || s == "-" || s == "*" || s == "/")
                return true;
            return false; //implicit else-case
        }
        private bool isOperator(string s) //check if operator[+-*/]
        {
            if (s == "+" || s == "-" || s == "*" || s == "/")
                return true;
            return false; //implicit else-case
        }
        //private bool isVariable(bool goodParse, string s)
        //{
        //    return (!goodParse && !isSymbol(s));
        //} //pointless?

        //parsing helper function -- find index of next symbol & return
        private int findNextSymbol(string exp, int curInd)
        {
            int i = curInd + 1;
            for (; i < exp.Length; i++)
                if (isSymbol(exp[i].ToString()))
                    return i; //return index of symbol
            return exp.Length;//failed to find symbol -- return end
        }

        //manually parse the expression string into value/variable/symbol tokens
        //-this implements equation validation too
        //--tried using RegEx, see the hidden/commented code at the bottom of the function
        private string[] parseExp(ref Cell c, ref Cell[][] spc, string expression)
        {
            List<string> result = new List<string>(); //List allows dynamic sizing            
            int i = 0, bound = 0, parens = 0, curRow=0; //index, bound, and parenthesis counter(ensure balance)
            double leafVal = 0.0, junk = 0.0;
            string t = "";
            string[] badRes = { "" };
            bool res = false, cellRes = false, goodOperators = true;

            expression = expression.Trim(); //so we don't worry about spaces
                        
            while (i < expression.Length)
            {
                if (isSymbol(expression[i].ToString())) {    //+,-,*,/,(,)
                    result.Add(expression[i].ToString());   //add symbol to list
                    if (expression[i] == '(') parens++;     //increment #parentheses to close
                    else if (expression[i] == ')') parens--;//closing parenthesis, decrement #
                    if (isOperator(expression[i].ToString())) //check for consecutive operators
                        if (i + 1 < expression.Length)        //ensure next item is within bound
                            if (isOperator(expression[i + 1].ToString())) //doesn't account for attempted 'negative' values
                                goodOperators = false;
                }
                else                                       //else, we have a value|variable
                {
                    bound = findNextSymbol(expression, i); //find its end in the expression
                    if (bound != 0)                         //help to ensure valid substring(?)
                    {
                        t = expression.Substring(i, (bound - i)).Trim(); //get value|variable -- trim whitespace
                        if (!isOperator(t))
                            result.Add(t);
                        else
                            return badRes;                                  
                                                              
                        res = Double.TryParse(t, out leafVal);                       //check if variable
                        if (!res) {                                                  //if it IS a variable                        
                            int.TryParse(t.Substring(1), out curRow);  curRow--;       //get row && swap indexing mode
                            int col = (int)(t[0]-'A');                                 //get column

                            if (0 <= curRow && curRow < 50 && 0 <= col && col < 26 && c.ToVarName != t)   //if location within bounds & not self-ref.
                            {                                
                                double.TryParse(spc[curRow][col].Value, out leafVal);  //find value mapped to given cell
                                if (!c.references.TryGetValue(t, out junk))
                                    if(spc[curRow][col].Value != "")
                                        c.references.Add(t, leafVal);                  //add dictionary reference to cell
                                else
                                    c.references.Add(t, 0);
                                
                                if (!spc[curRow][col].referencedBy.TryGetValue(c.ToVarName, out leafVal)) //don't re-ref.
                                    spc[curRow][col].referencedBy.Add(c.ToVarName, 0); //make cell know it's referenced
                            }
                            else
                                return badRes;
                        }

                        i = bound; //go to end of substring
                        continue;  //forget the increment, already updated i with bound
                    }
                }
                i++; //move forward in expression
            }
            if (parens == 0 && goodOperators) //if balanced parentheses, and no consecutive operators[+-*/], return expression tokens
                return result.ToArray(); //return the array of strings once size is concrete&final
            else
                return badRes;
            /*****************************************************************************************************\
            \/-- The 2 patterns below were (seemingly) my closest attempts 
            \/-- However, they did not include parentheses in the token list, so I do it manually (above)
            \/-- I could've done it a lot more easily with Recursive Regular Expressions, but this isn't Perl...
            \*****************************************************************************************************/
            //String pattern = @"([-+*/])*\(*(\d+)\)*\s*([-+*/])\s*\(*(\d+)\)*";
            //String pattern3 = @"\(*(\d+(\(*\s*([-+*/])\s*(\d+)\)*)*?)\)*\s*([-+*/])\s*\(*(\(*\d+(\s*([-+*/])\s*(\d+)\)*)*?)\)*";
            //\-- I feel like the *? was getting me somewhere..
            //String originalPattern = @"(\d+)\s*([-+*/])\s*(\d+)"; //ex. "12 + 34", "4/2", "8    /  4",...
            //String badpattern2 = @"\(?\(*(\(*\(*(\d+)\)*\s*([-+*/])\s*\(*(\d+)\)*\)*)\)*\s*([-+*/])\s*\(*(\(*\(*(\d+)\)*\s*([-+*/])\s*\(*(\d+)\)*\)*)\)*\)?"; //include parentheses
            //the above kinda accepts "((12-2)*(2/3))"
            /* foreach (Match m in Regex.Matches(expression, pattern))
            {
                Console.WriteLine("m={0}",m.ToString());
                int k = 1;
                string tok = "";
                while (k < m.Groups.Count)
                {
                    if (m.Groups[k].Value != " " && m.Groups[k].Value != "")
                    {
                        tok = m.Groups[k++].Value;
                        Console.WriteLine("{0}", tok);
                        result.Add(tok);
                    }
                    else k++;                   
                }
            }*/
        }

        //perform the shuntingYard algorithm to convert expression token list into RPN
        // builds tree during conversion -- preserves order of operations
        private bool shuntYard(Cell c, string[] tokens)
        {
            double leafVal = 0.0;
            bool res = false;
            char op = '\0';
            if (currentExpression[0] != '#')//tokens[0] != "" //if not badRef
            {
                foreach (string tok in tokens)
                {
                    res = Double.TryParse(tok, out leafVal); //parse -- determines if variable|value
                    if (res)
                    {
                        operands.Push(new ValNode(leafVal));         //if value, add to queue 
                    }
                    else if (!res && !isSymbol(tok))       //if it's a Variable                    
                    {
                        leafVal = c.references[tok];                           //get variable value from dictionary
                        operands.Push(new VarNode(tok, leafVal));             //add variable to queue, assume value already given                     
                    }

                    else if (isOperator(tok))                   //if operator [+-*/]
                    {
                        while (operators.Count != 0 && precedence[operators.Peek().operationType.ToString()] > precedence[tok])  //while operator on stack-top has greater precedence
                            buildTree();  //build
                        char.TryParse(tok, out op);          //don't need to check result, guaranteed to work
                        operators.Push(new OpNode(op));      //push current operator onto stack
                    }

                    else if (tok == "(")
                    {                        //if opening parenthesis                
                        char.TryParse(tok, out op);          //don't need to check result, guaranteed to work
                        operators.Push(new OpNode(op));    //add left parenthetical to stack 
                    }
                    else if (tok == ")")         //if closing parentesis
                    {                            //while the parenthetical is not matched
                        while (operators.Count != 0 && operators.Peek().operationType != '(')
                            buildTree();  //build
                        operators.Pop(); //pop "(" and discard it
                    }
                }
                //While there are operators on the stack, build & evaluate
                while (operators.Count != 0)
                    buildTree();  //build    
                return true;   
            }                        
            return false;
        }

        //build the tree (bottom up) using the shuntingYard stacks
        private bool buildTree()
        {
            //pop operators from stack -> build tree
            //OpNode temp;
            //   if (root.operationType == '_')
            //   {
            
            root = operators.Pop();
            root.right = operands.Pop();
            root.left = operands.Pop();        //after building tree,            
            operands.Push(new ValNode(root.Eval()));  //evaluate, and push value to output
            return true;
                //return true;
         //   }
            //else
            //{
            //    temp = root;                //hold sub-tree
            //    //operands.Pop();             //pop the result -- right sub-tree == result
            //    root = operators.Pop();     //grab an operator
            //    root.right = temp;          //restore sub-tree, making right-heavy tree
            //    root.left = operands.Pop(); //if we don't push the result (leave it in tree) this works

            //    return true;
            //}
            //int valCount = eTokens.Length;  //keep #items handy
            //int leafVal = 0;  //out variable -- required for parsing

            
        }

        //removes variable value from reference Dictionary -- used during tree re-creation
        public void RemoveVar(Cell c, string varName)
        {
            c.references.Remove(varName);
        }
        //sets variable value in reference Dictionary
        public void SetVar(Cell c, string varName, double varValue)
        {                       
           c.references[varName] = varValue;        
        }    

        //evaluates ExpTree according to root operation type
        //-- by recursively evaluating sub-tree expressions & leaf Variables/Values  
        //evaluate tree if not previously evaluated
        //otherwise return the previous result -- (don't re-calculate)
        public string Eval(Cell c)
        {
            double rootResult = 0.0;            
          //  if(evalResult == -42.0) //check the arbitrary starting value
          //  {                
            bool res = shuntYard(c, expressionTokens);                             //convert to RPN & build tree with Shunting Yard Algorithm
            Node lastNode;
            if (currentExpression[0] != '#')
            {//rootResult = root.Eval();
                lastNode = operands.Pop();
                rootResult = lastNode.Eval();
            }
            else
            {
                Console.Write("Can't evaluate! Default ");
                //SpreadsheetCell ssc = c as SpreadsheetCell;
                //ssc.Value =
                return "#REF!";
            }
            evalResult = rootResult;                
          //  }
            Console.WriteLine("= {0}\n", evalResult);
            return evalResult.ToString(); //return previously made result
        }

        

        //returns true if varName in dictionary, false if not
        //uses TryGetValue so we don't have to use try/catch blocks
        public bool isValidEntry(Cell c, string varName)
        {
            double junk = 0.0; //must have out variable
            return c.references.TryGetValue(varName, out junk); 
        }

        //base Node class
        private abstract class Node
        {
            public abstract double Eval(); //node Evaluation mFunction
        }

        //Variable Node class
        private class VarNode : Node
        {
            public string Name { get; set; } //variable name
            public double Value { get; set; }//variable value -- initially 0.0 until set

            public VarNode(string name = "", double val=0.0) { Name = name; Value = val; }

            public override double Eval(){
                return Value; //return referenced double
            }
        }
        private class OpNode : Node
        {
            public char operationType { get; set; } // +, -, *, /
            public Node left { get; set; }  //left sub-tree
            public Node right { get; set; } //right sub-tree
            
            //constructor -- initialize operation type and sub-trees
            public OpNode(char op = '+') { operationType = op; left = null; right = null; }

            //evaluates OpNode according to OpNode operation type
            //-- by recursively evaluating sub-tree expressions & leaf Variables/Values
            public override double Eval()
            {
                switch (operationType)
                {
                    case '+': return this.left.Eval() + this.right.Eval();
                    case '-': return this.left.Eval() - this.right.Eval();
                    case '*': return this.left.Eval() * this.right.Eval();
                    case '/': if(this.right.Eval() != 0) return this.left.Eval() / this.right.Eval();
                              else return 0;
                    default: return 0.0;
                }              
            }
        }
        private class ValNode : Node
        {
            public double Value { get; set; } //value property

            //constructor -- sets only property in ValNode, value
            public ValNode (double val) { Value = val; }

            //return value of ValNode directly
            public override double Eval() {
                return Value;
            }
        }        
    }
    
    public abstract class Cell : INotifyPropertyChanged
    {
        protected string text, _value, _bgColor;                            //string fields
        protected int rowIndex, columnIndex;                      //location fields
        public event PropertyChangedEventHandler PropertyChanged; //event field
        public Dictionary<string, double> references;
        public Dictionary<string, double> referencedBy;

        public ExpTree eq;

        //constructor -- initialize all fields to given value/null
        public Cell(int mRow = 0, int mCol = 0)
        {
            references = new Dictionary<string, double>();
            referencedBy = new Dictionary<string, double>();

            text = string.Empty;
            _value = string.Empty;
            _bgColor = "#FFFFFF";
            rowIndex = mRow;
            columnIndex = mCol;
        }
        
        //cell factory function
        public Cell CreateCell() { return this; }
        
        //location field getters
        public int RowIndex { get { return rowIndex; } }
        public int ColumnIndex { get { return columnIndex; } }    

        //event method -- invokes event
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)            
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));            
        }        
        
        //text property (setter/getter)
        //fires propertyChanged event on assignment(setter)
        public string Text {
            get { return text; }
            set {
                if (text != value)
                {
                    text = value;
                    if (text == "#REF!")
                        bgColor = "Red";
                    OnPropertyChanged("Text");//propertyChanged event                   
                }
            }
        }

        //value property (getter)
        //returns the cell's value
        public string Value {
            get { return _value; }
        }
        public string bgColor
        { get { return _bgColor; }
        set { if(_bgColor != value)
                {
                    _bgColor = value;
                    OnPropertyChanged("bgColor");
                }
            }
        }
        //public string LastExp { set; get; }
        public string ToVarName
        {
            get
            {
                string row = (this.RowIndex + 1).ToString();
                string col = ((char)(this.ColumnIndex + 'A')).ToString();
                return col + row;
            }
        }
        public double ToNumber
        {
            get
            {
                double res = 0.0;
                Double.TryParse(Value, out res);
                return res;
            }
        }
        //will be used for checking if cells need to be saved
        public bool IsModified
        {
            get
            {
                if (Value != "" || Text != "")
                    return true;
                else
                    return false;
            }
        }

        //convert the current cell into an XElement for export to .xml files using LINQ
        public XElement ToXElement {
            get {
                return new XElement("cell", new XAttribute("name", ToVarName),
                                          new XElement("text", Text),
                                          new XElement("bgcolor", bgColor.Substring(1))); //leave out #
            }
        }
        //clear all dictionary entries (discard previous variables)
        //used when we must make a new expression tree
        public void clearRefDict()
        {
            references.Clear(); 

        }
        public void clearRByDict()
        {
            referencedBy.Clear(); 
        }
        public void clearAllDicts()
        {
            clearRByDict();
            clearRefDict();
        }
    }
    public class SpreadsheetCell : Cell
    {
        public SpreadsheetCell(int mRow, int mCol) : base(mRow, mCol) { }
        public static SpreadsheetCell CreateCell(int mRow, int mCol)
        {
            return new SpreadsheetCell(mRow, mCol);
        }
        public string Value
        {//strongly hinted that we should make specialized class that allows Value setting
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    if (ToNumber >= 25)
                        bgColor = "#00FF00";
                    if (ToNumber < 0)
                        bgColor = "#FF00FF";
                    if (0 <= ToNumber && ToNumber < 25)
                        bgColor = "#FFFFFF";
                    OnPropertyChanged("Value");//propertyChanged event on Value update
                }//no need to update subcriptions -- it's the same event!
            }
        }        
    }
    public class SpreadsheetClass// : INotifyPropertyChanged
    {
        private Cell[][] sheet;                                        //'jagged' 2D array (table)
        private int columnCount, rowCount;                            //table bounds
        public event PropertyChangedEventHandler CellPropertyChanged; //event field

        //Constructor -- initializes sheet and bound variables, creates cells, and subscribes to events
        public SpreadsheetClass(int nRows, int nCols)
        {
            sheet = new SpreadsheetCell[nRows][];//init rows array (1st dimension)
            for (int i = 0; i < nRows; i++)
                sheet[i] = new SpreadsheetCell[nCols];//init cols array (2nd dimension)

            for (int i = 0; i < nRows; i++)
                for (int j = 0; j < nCols; j++)//initialize cells & locations
                {
                    SpreadsheetCell nCell = SpreadsheetCell.CreateCell(i, j); //make each cell
                    sheet[i][j] = nCell;                                      //assign to correct place
                    nCell.PropertyChanged += OnCellPropertyChanged;           //subscribe to events
                }

            columnCount = nCols;
            rowCount = nRows;//set both bounds
        }        
        //Getters for Sheet Bounds (#rows & #columns)
        public int ColumnCount { get { return columnCount; } }
        public int RowCount { get { return rowCount; } }
        //Cell getter, interface to sheet
        public Cell GetCell(int row, int col) {
            if (0 <= row && row < 50 && 0 <= col && col < 26)
            {
                if (sheet[row][col] != null)//if cell exists
                    return sheet[row][col]; //return cell
                else                        //if no cell
                    return null;            //return null
            }
            return new SpreadsheetCell(0, 0); //default garbage cell?
        }
        public Cell GetCell(string t)
        {
            int row = 0;            
            int.TryParse(t.Substring(1), out row); //get row
            row--;
            int col = (int)(t[0] - 'A');                  //get column
            if (0 <= row && row < 50 && 0 <= col && col < 26)
                return sheet[row][col];//return found cell
            else
                return null;
        }
        public double GetVal(string t)
        {
            int row = 0;
            double leafVal = 0.0;
            int.TryParse(t.Substring(1), out row); //get row
            row--;
            int col = (int)(t[0] - 'A');                  //get column
            double.TryParse(sheet[row][col].Value, out leafVal);  //find value mapped to given cell

            return leafVal;
        }
        //Sheet getter, need access outside class ; 
        // must use foreach loops for iteration through whole sheet (oddly)
        public Cell[][] Sheet { get { return sheet; } }
        private void parseSingleVariable(SpreadsheetCell c)
        {
            string txt = c.Text;//use text field to set value
            int row=0, col=0, singularValue=0;   //=ColRow (ex. =B2)
            double cellVal = 0.0, junk = 0.0;
            bool res = false;
            res = int.TryParse(txt.Substring(1), out singularValue);
            if (txt.Substring(1) == c.ToVarName)
            {
                c.Value = "#REF!";
                c.bgColor = "#FF0000";
                return;
            }
            if (!res)
            {
                int.TryParse(txt.Substring(2), out row); //parse row#  //substring to grab all numbers for row
                row--;                                   //change indexing mode
                if ('a' <= txt[1] && txt[1] <= 'z')      //columns are letters => ascii math
                    col = (int)(txt[1] - 'a');           //support lower-case letters (extra)
                else
                    col = (int)(txt[1] - 'A');           //support upper-case (default)
                                                         //Debug.WriteLine("row={0}, col={1}", row, col);
                Double.TryParse(this.sheet[row][col].Value, out cellVal);
                string temp = this.sheet[row][col].Value;     //set value (not text, else infinite loop)
                c.Value = temp;// singularValue;
                               //int.TryParse(t.Substring(1), out curRow); //get row
                               //int col = (int)(t[0] - 'A');                  //get column
                               //double.TryParse(sheet[row - 1][col].Value, out leafVal);  //find value mapped to given cell
                if(!c.references.TryGetValue(txt.Substring(1), out junk))
                    if(this.sheet[row][col].Value != "") //this bypasses short-circuit eval.
                        c.references.Add(txt.Substring(1), cellVal); //add dictionary reference to cell
                else
                    c.references.Add(txt.Substring(1), 0); //add null dictionary reference to cell

                if (!sheet[row][col].referencedBy.TryGetValue(c.ToVarName, out junk))
                    sheet[row][col].referencedBy.Add(c.ToVarName, cellVal); //make cell know it's referenced
            }
            else
                c.Value = singularValue.ToString();
        }
        private bool containsOp(string s)
        {
            if (s.Contains("+") || s.Contains("-") || s.Contains("*") || s.Contains("/"))
                return true;
            return false;
        }
        protected void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)  // Create method, to raise its event
        {
            SpreadsheetCell c = sender as SpreadsheetCell;
            //int row, col; //=ColRow (ex. =B2)
            if (e.PropertyName == "Text")
            {
                string txt = c.Text;
                if (txt == null || txt == "" || txt[0] != '=') //if null, empty, or string without equation
                    c.Value = txt;                             //then set text directly
                else { //we have an '=' -- Evaluate 'equation'
                    if (containsOp(txt))
                    {
                       // c.LastExp = c.Text;
                        c.eq = new ExpTree(c, sheet, txt.Substring(1));
                        if (c.Value != "#REF!")
                            c.Value = c.eq.Eval(c);
                    }
                    else {
                        parseSingleVariable(c);          //parse variable text in cell
                        while (c.Value != "" && c.Value[0] == '=') //handle reference-chains (Ex. A1=B1=C1=42 )
                            parseSingleVariable(c);//c.Value != "" && 
                    }
                }
                              
            }
            if (c.referencedBy.Count > 0 && c.Value != "" && c.Value[0] != '#' && c.Text != "")// update all cells that c is referenced by if !badRef
                foreach (var r in c.referencedBy)
                {
                    SpreadsheetCell cur = GetCell(r.Key) as SpreadsheetCell;
                    cur.references.Clear(); //clear references -- must re-add them since changed(inefficient?)

                    //cur.eq = new ExpTree(cur, sheet, cur.Text.Substring(1)); 
                    //cur.Value = cur.eq.Eval(cur).ToString();
                    if (containsOp(cur.Text))
                    {
                       // c.LastExp = c.Text;
                        cur.eq = new ExpTree(cur, sheet, cur.Text.Substring(1));
                        if (cur.Value != "#REF!")
                            cur.Value = cur.eq.Eval(cur);
                    }
                    else if(cur.Text != "" && cur.Text[0] == '='){
                        parseSingleVariable(cur);          //parse variable text in cell
                        while (cur.Value[0] == '=') //handle reference-chains (Ex. A1=B1=C1=42 )
                            parseSingleVariable(cur);
                    }
                }
            if (CellPropertyChanged != null)    //if we have an event
                CellPropertyChanged(sender, e); //fire the event!
        }      

        public void SaveSheet(Stream myStream)
        {
            new XDocument( new XElement("spreadsheet",
                         ( from cellArr in sheet
                   from cell in cellArr
                  where cell.IsModified
                 select cell.ToXElement))).Save(myStream);
            //How simple it is, 
                //to summon a LINQ Query out of thin air, 
                    //save our file,
                        //and drop it into a black hole..
        }
        
        public void LoadSheet(StreamReader sr)
        {
            XDocument doc = XDocument.Load(sr); //load a new XDocument from the given StreamReader
                                                //fortunately it's overloaded already..
            foreach (XElement xe in (from cell in doc.Root.Descendants()
                                     where cell.Attribute("name") != null
                                     select cell))
            {
                if(xe.Attribute("name") != null && xe.Element("text") != null)
                    GetCell(xe.Attribute("name").Value).Text = xe.Element("text").Value;
                if(xe.Element("bgcolor") != null)
                    GetCell(xe.Attribute("name").Value).bgColor = "#" + xe.Element("bgcolor").Value;
            }
            //This is bafflingly short..
        }
        /*
        <spreadsheet>
          <cell name="B1">
            <bgcolor>FF8000</bgcolor>
            <text>=A1+6</text>
          </cell>
        </spreadsheet>
        
        <spreadsheet>
            <cell unusedattr=“abc” name=“B1”> <text>=A1+6</text>
            <some_tag_you_didnt_write>blah</some_tag_you_didnt_write>
            <bgcolor>FF8000</bgcolor> <another_unused_tag>data</another_unused_tag>
            </cell>
        </spreadsheet>  
        */
    }
}
