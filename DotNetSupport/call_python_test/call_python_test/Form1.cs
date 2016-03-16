using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace call_python_test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string  a ="2";
            string b = "b";

            Process p = new Process(); // create process (i.e., the python program
            p.StartInfo.FileName = @"C:\Python27\ArcGISx6410.2\python.exe";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false; // make sure we can read the output from stdout
            p.StartInfo.Arguments = @"D:\Python\testprojectofPython\test3.py " + a + " " + b; // start the python program with two parameters
            p.Start(); // start the process (the python program)
           
            StreamReader s = p.StandardOutput;
            String output = s.ReadToEnd();
            string[] r = output.Split(new char[] { ' ' }); // get the parameter
            Console.WriteLine(r[0]);
            p.WaitForExit();


        }
        private void run_cmd(string cmd, string args)
        {

            Process p = new Process(); // create process (i.e., the python program
            p.StartInfo.FileName = cmd;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false; // make sure we can read the output from stdout
            p.StartInfo.Arguments = @"D:\Python\testprojectofPython\test.py " + args; // start the python program with two parameters
            p.Start(); // start the process (the python program)
            StreamReader s = p.StandardOutput;
            String output = s.ReadToEnd();
            string[] r = output.Split(new char[] { ' ' }); // get the parameter
            Console.WriteLine(r[0]);
            p.WaitForExit();

            Console.ReadLine(); // wait for a key press
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String prg = @"import sys
x = int(sys.argv[1])
y = int(sys.argv[2])
print x+y";
            StreamWriter sw = new StreamWriter(@"D:\Python\testprojectofPython\test2.py");
            sw.Write(prg); // write this program to a file
            sw.Close();

            int a = 2;
            int b = 2;

            Process p = new Process(); // create process (i.e., the python program
            p.StartInfo.FileName = @"C:\Python27\ArcGISx6410.2\python.exe";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false; // make sure we can read the output from stdout
            p.StartInfo.Arguments = @"D:\Python\testprojectofPython\test2.py " + a + " " + b; // start the python program with two parameters
            p.Start(); // start the process (the python program)
            StreamReader s = p.StandardOutput;
            String output = s.ReadToEnd();
            string[] r = output.Split(new char[] { ' ' }); // get the parameter
            Console.WriteLine(r[0]);
            p.WaitForExit();

            Console.ReadLine(); // wait for a key press
        }
    }


}

