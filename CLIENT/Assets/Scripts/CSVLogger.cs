using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public class CSVLogger
{
    private string[] columnNames;

    private List<string[]> entries = new List<string[]>();

    public CSVLogger( string[] columns)
    {
        this.columnNames = columns;
    }

    public void AddEntry(string[] columns)
    {
        entries.Add(columns);
    }

    public void Save(string filename)
    {
       var saveStr = String.Join(";", columnNames);
        //StreamWriter sw = new StreamWriter(filename);
        //sw.Write(String.Join(";", columnNames));
        foreach (var entry in entries)
        {
            saveStr += "\n";
            saveStr += String.Join(";", entry);
            //sw.WriteLine(String.Join(";",entry));
        }
        //sw.Close();


       File.WriteAllText(filename, saveStr);
        //File.WriteAllBytes(filename, Encoding.ASCII.GetBytes(saveStr));
       
        //writer.Close();
    }
}

