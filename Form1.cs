using Ownskit.Utils;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;

namespace KeyPressStat
{
    public partial class Form1 : Form
    {
        private ConcurrentDictionary<int, StatisticsEntry> _stats = new ConcurrentDictionary<int, StatisticsEntry>();
        private KeyboardListener _listener = new KeyboardListener();
        private int _lastKeyCode;

        public Form1()
        {
            InitializeComponent();

            _listener.KeyDown += OnKeyDown;

            _updateTimer.Interval = 500;
            _updateTimer.Tick += OnTimerTick;
            _updateTimer.Start();
        }

        private void OnKeyDown(object sender, RawKeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                var prevKey = Interlocked.Exchange(ref _lastKeyCode, -1);
                if (_stats.TryGetValue(prevKey, out var entry))
                    entry.Decrement();
            }
            else
            {
                Volatile.Write(ref _lastKeyCode, e.VKCode);
            }
            _stats.AddOrUpdate(e.VKCode, new StatisticsEntry(e.VKCode, e.Key, e.Character), (_,existing) => 
            {
                existing.Increment();
                return existing;
            });
        }

        private void OnTimerTick(object sender, EventArgs e)
            => UpdateView();

        private void UpdateView()
        {
            if (!Visible) return;
            _listBox.Items.Clear();
            _listBox.Items.AddRange(_stats.Select(i => $"{i.Key:X4}|{i.Value}").ToArray());
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
            foreach (var item in _stats.Values)
                file.WriteLine($"{item.VKCode:X4} | {item.Key} | {item.Character} | {item.Count}");
        }
    }
}
