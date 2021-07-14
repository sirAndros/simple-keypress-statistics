using Ownskit.Utils;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace KeyPressStat
{
    public partial class Form1 : Form
    {
        private ConcurrentDictionary<int, StatisticsEntry> _stat = new ConcurrentDictionary<int, StatisticsEntry>();
        private KeyboardListener _listener = new KeyboardListener();

        public Form1()
        {
            InitializeComponent();

            _listener.KeyDown += (s,e) =>
            {
                _stat.AddOrUpdate(e.VKCode, new StatisticsEntry(e.VKCode, e.Key, e.Character), (_,existing) => 
                {
                    existing.Increment();
                    return existing;
                });
            };

            timer1.Interval = 500;
            timer1.Tick += Timer1_Tick;
            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            if (!Visible) return;
            listBox1.Items.Clear();
            listBox1.Items.AddRange(_stat.Select(i => $"{i.Key:X4}|{i.Value}").ToArray());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _listener.Dispose();
            SaveToFile();
        }

        private void SaveToFile()
        {
            var fileName = Guid.NewGuid().ToString() + ".txt";
            using var file = new StreamWriter(fileName, append: false, Encoding.UTF8);
            foreach (var item in _stat.Values)
                file.WriteLine($"{item.VKCode:X4} | {item.Key} | {item.Character} | {item.Count}");
        }
    }
}
