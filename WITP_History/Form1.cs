using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace WITP_History
{
    public partial class Form1 : Form
    {

        public string archivePath;

        //History <Date, Events>
        public Dictionary<string, Tuple<List<string>, List<string>>> japan;
        public Dictionary<string, Tuple<List<string>, List<string>>> allies;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            string path = "INVALID";
            if (result == DialogResult.OK)
            {
                path = folderBrowserDialog1.SelectedPath;
                
            }
            if (result == DialogResult.Cancel)
            {
                return;

            }
            Console.WriteLine(path);

            //Save new path
            File.WriteAllText("WITP_History_Settings.txt", path);
            archivePath = path;

            DisplayPath();

            LoadHistory();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Read Archive Path
            if (File.Exists("WITP_History_Settings.txt"))
            {
                archivePath = File.ReadAllText("WITP_History_Settings.txt");
                DisplayPath();
                LoadHistory();
            }
        }

        private void DisplayPath()
        {
            textBox1.Text = archivePath;
        }

        private void LoadHistory()
        {
            string[] files = Directory.GetFiles(archivePath);

            japan = new Dictionary<string, Tuple<List<string>, List<string>>>();
            allies = new Dictionary<string, Tuple<List<string>, List<string>>>();

            for (int i = 0; i < files.Length; i++)
            {
                //Japan Operations
                if (files[i].Contains("joperationsreport_"))
                {
                    string[] lines = File.ReadAllLines(files[i]);

                    string date = lines[0];
                    date = date.Replace("OPERATIONAL REPORT FOR ", "");
                    date = date.Replace("\n", "");

                    List<string> events = lines.Skip<string>(2).ToArray<string>().ToList<string>();
                    List<string> eventsLower = events.ConvertAll(d => d.ToLower());

                    for (int j = 0; j < events.Count; j++)
                    {
                        events[j] = "\t" + events[j];
                    }

                    if (japan.ContainsKey(date))
                    {
                        japan[date].Item1.AddRange(events);
                        japan[date].Item2.AddRange(eventsLower);
                    }
                    else
                        japan.Add(date, new Tuple<List<string>, List<string>>(events, eventsLower));
                }

                //Japan SigInt
                if (files[i].Contains("jsigint_"))
                {
                    string[] lines = File.ReadAllLines(files[i]);

                    string date = lines[0];
                    date = date.Replace("SIG INT REPORT FOR ", "");
                    date = date.Replace("\n", "");

                    List<string> events = lines.Skip<string>(2).ToArray<string>().ToList<string>();
                    List<string> eventsLower = events.ConvertAll(d => d.ToLower());

                    for (int j = 0; j < events.Count; j++)
                    {
                        events[j] = "\t" + events[j];
                    }

                    if (japan.ContainsKey(date))
                    {
                        japan[date].Item1.AddRange(events);
                        japan[date].Item2.AddRange(eventsLower);
                    }
                    else
                        japan.Add(date, new Tuple<List<string>, List<string>>(events, eventsLower));
                }

                //Allies Operations
                if (files[i].Contains("aoperationsreport_"))
                {
                    string[] lines = File.ReadAllLines(files[i]);

                    string date = lines[0];
                    date = date.Replace("OPERATIONAL REPORT FOR ", "");
                    date = date.Replace("\n", "");

                    List<string> events = lines.Skip<string>(2).ToArray<string>().ToList<string>();
                    List<string> eventsLower = events.ConvertAll(d => d.ToLower());

                    for (int j = 0; j < events.Count; j++)
                    {
                        events[j] = "\t" + events[j];
                    }

                    if (allies.ContainsKey(date))
                    {
                        allies[date].Item1.AddRange(events);
                        allies[date].Item2.AddRange(eventsLower);
                    }
                    else
                        allies.Add(date, new Tuple<List<string>, List<string>>(events, eventsLower));
                }

                //Allies SigInt
                if (files[i].Contains("asigint_"))
                {
                    string[] lines = File.ReadAllLines(files[i]);

                    string date = lines[0];
                    date = date.Replace("SIG INT REPORT FOR ", "");
                    date = date.Replace("\n", "");

                    List<string> events = lines.Skip<string>(2).ToArray<string>().ToList<string>();
                    List<string> eventsLower = events.ConvertAll(d => d.ToLower());

                    for (int j = 0; j < events.Count; j++)
                    {
                        events[j] = "\t" + events[j];
                    }

                    if (allies.ContainsKey(date))
                    {
                        allies[date].Item1.AddRange(events);
                        allies[date].Item2.AddRange(eventsLower);
                    }
                    else
                        allies.Add(date, new Tuple<List<string>, List<string>>(events, eventsLower));
                }

                //Combat Reports (Shared)
                if (files[i].Contains("combatreport_"))
                {
                    string[] lines = File.ReadAllLines(files[i]);

                    string date = lines[0];
                    date = date.Replace("AFTER ACTION REPORTS FOR ", "");
                    date = date.Replace("\n", "");

                    string[] events = lines.Skip<string>(1).ToArray<string>();

                    //Combine Combat Report
                    List<string> reports = new List<string>();
                    string reportLine = "";
                    for (int j = 0; j < events.Length; j++)
                    {
                        if(events[j].Contains("---"))
                        {
                            reports.Add(reportLine);
                            reportLine = "";
                        }
                        else
                        {
                            if (events[j] == "" || events[j] == " " || events[j] == "\n")
                                continue;

                            if (reportLine == "")
                                reportLine += "\t" + events[j] + "\n";
                            else
                                reportLine += "\t\t" + events[j] + "\n";
                        }
                    }

                    //Only add for each side if contains "Japanese" or "Allied" (Fog of War)
                    for (int j = 0; j < reports.Count; j++)
                    {
                        
                            if (japan.ContainsKey(date))
                            {
                                japan[date].Item1.Add(reports[j]);
                                japan[date].Item2.Add(reports[j].ToLower());
                            }
                            else
                                japan.Add(date, new Tuple<List<string>, List<string>>(new List<string>() { reports[j] }, new List<string>() { reports[j].ToLower() }));
                        
                    }

                    for (int j = 0; j < reports.Count; j++)
                    {
                        
                            if (allies.ContainsKey(date))
                            {
                                allies[date].Item1.Add(reports[j]);
                                allies[date].Item2.Add(reports[j].ToLower());
                            }
                            else
                                allies.Add(date, new Tuple<List<string>, List<string>>(new List<string>() { reports[j] }, new List<string>() { reports[j].ToLower() }));
                        
                    }
                }
            }

            richTextBox1.Text = "";

            // BENCHMARK
            if (false)
            {
                DateTime start = DateTime.Now;
                for (int i = 0; i < 10000; i++)
                {
                    Query("Kota");
                }
                DateTime end = DateTime.Now;
                Console.WriteLine("Time: " + (end - start));
            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.Text = Query(textBox2.Text.ToLower());
        }

        private string Query(string query)
        {
            if (japan == null || allies == null)
                return "Set the path to your Archives folder in the Settings tab";

            //Dont search unless some characters
            if (query.Replace(" ", "").Length <= 2)
                return "Search query too small";

            string queryResults = "";

            //Japan or Allies
            Dictionary<string, Tuple<List<string>, List<string>>> database = null;
            if (comboBox1.SelectedIndex == 0)
                database = japan;
            else
                database = allies;

            //Match
            List<string> results = new List<string>();
            foreach (var item in database)
            {
                List<string> events = database[item.Key].Item1;
                List<string> eventsLowered = database[item.Key].Item2;

                results.Clear();

                //Get matches
                for (int i = 0; i < eventsLowered.Count; i++)
                {
                    if (eventsLowered[i].Contains(query))
                    {
                        results.Add(events[i]);
                    }
                }

                //Display matches
                if (results.Count > 0)
                {
                    queryResults += "\n" + item.Key + "\n";

                    for (int i = 0; i < results.Count; i++)
                    {
                        queryResults += results[i] + "\n";
                    }
                }
            }

            return queryResults;
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            textBox2_TextChanged(sender, e);
        }
    }
}
