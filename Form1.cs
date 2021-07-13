using Ownskit.Utils;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace KeyPressStat
{
    public partial class Form1 : Form
    {
        private ConcurrentDictionary<string, int> _stat = new ConcurrentDictionary<string, int>();
        private KeyboardListener _listener = new KeyboardListener();

        public Form1()
        {
            InitializeComponent();

            _listener.KeyDown += (s,e) =>
            {
                var key = e.Character;
                if (String.IsNullOrEmpty(key))
                    key = e.Key.ToString();
                _stat.AddOrUpdate(key, 1, (k,l) => l+1);
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
            listBox1.Items.AddRange(_stat.Select(i => $"{i.Key}: {i.Value}").ToArray());
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
            using var file = new StreamWriter(fileName);
            foreach (var item in _stat)
            {
                file.WriteLine($"{item.Key} | {item.Value}");
            }
        }
    }
}
