using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace TimeTracker;

public partial class MainWindow : Window
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(
        IntPtr hWnd,
        out uint processId);

    private readonly DispatcherTimer _timer;
    private readonly Dictionary<string, TimeSpan> _usage = new();

    public MainWindow()
    {
        InitializeComponent();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _timer.Tick += Timer_Tick;
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        _timer.Start();
        StatusTextBlock.Text = "Отслеживание включено";
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        StatusTextBlock.Text = "Отслеживание выключено";
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        string? applicationName = GetActiveApplicationName();

        if (string.IsNullOrEmpty(applicationName))
            return;

        if (!_usage.ContainsKey(applicationName))
            _usage[applicationName] = TimeSpan.Zero;

        _usage[applicationName] =
            _usage[applicationName].Add(TimeSpan.FromSeconds(1));

        UpdateList();
    }

    private static string? GetActiveApplicationName()
    {
        IntPtr window = GetForegroundWindow();

        if (window == IntPtr.Zero)
            return null;

        GetWindowThreadProcessId(window, out uint processId);

        try
        {
            using Process process = Process.GetProcessById((int)processId);
            return process.ProcessName;
        }
        catch
        {
            return null;
        }
    }

    private void UpdateList()
    {
        UsageListBox.Items.Clear();

        foreach (var item in _usage.OrderByDescending(x => x.Value))
        {
            UsageListBox.Items.Add(
                $"{item.Key} — {item.Value:hh\\:mm\\:ss}");
        }
    }
}