using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions; //Match, Regex
using CptS321;

namespace ExpTreeApp
{
    class Program
    {        
        static void Main(string[] args)
        {
            //string expression = "1+2/(4-3)", optionString = "5", varName = "A1", varValueStr = "0.0";
            //int option = 5;
            //double varValue = 0.0;
            //bool res = false, running = true, valid = false;
            ////ExpTree eTree = new ExpTree(expression); //

            // while (running)
            // {
            //     Console.WriteLine("Menu (current expression=\"{0}\")", eTree.CurExp);
            //     Console.WriteLine("  1 = Enter a new expression");
            //     Console.WriteLine("  2 = Set a variable value");
            //     Console.WriteLine("  3 = Evaluate tree");
            //     Console.WriteLine("  4 = Quit");
            //     Console.Write(">");
            //     optionString = Console.ReadLine();
            //     res = int.TryParse(optionString, out option);
            //     if (!res) //parse unsuccessful &| bad input
            //         continue;//go back to read input again

            //     switch (option)
            //     {
            //         case 1:do {
            //                Console.Write("Enter a new expression\n : ");
            //                expression = Console.ReadLine();
            //             }while (expression == "");
                                                 
            //             eTree = new ExpTree(expression);
            //             break;
            //         case 2: valid = false; //setup the loop (ensure false)
            //             while (!valid) {
            //                 Console.Write("Enter the variable name: ");
            //                 varName = Console.ReadLine();
            //                 valid = eTree.isValidEntry(varName);
            //             }
            //             res = false; //setup the loop (ensure false)
            //             while (!res) {
            //                 Console.Write("Please enter a floating-point value: ");
            //                 varValueStr = Console.ReadLine();
            //                 res = double.TryParse(varValueStr, out varValue);
            //                if (res)
            //                    eTree.SetVar(varName, varValue);    
            //                //Console.WriteLine("vars[{0}]={1}", varName, eTree.variables[varName]);   
            //             }
            //             break;
            //         case 3:
            //             eTree.Eval();
            //             break;
            //         case 4:
            //             running = false;
            //             break;
            //         default:
            //             Console.WriteLine("Input ignored -- Refer to menu options. . .");
            //             break;
            //     }                
             //}

            //String[] expressions = { "(16 + 21)", "31 * 3", "28 / 3",
            //                   "(42-18)", "(12)*(7)",
            //                   "((12-2)*(2/3))", "asdf - b + c / 24" };
            ////String pattern = @"(\d+)\s([-+*/])\s(\d+)";
            //Console.ReadLine();
            //foreach (var expres in expressions)
            //{
            //    Console.WriteLine("eq:  {0}",expres);
            //    string[] eq = ParseExp(expres);
            //    foreach (string s in eq)
            //        Console.Write("{0} ", s);
            //    Console.WriteLine();
            //   // foreach (string s in eq)
            //   // { Console.WriteLine(s); }                
            //}


        }
    }
}
