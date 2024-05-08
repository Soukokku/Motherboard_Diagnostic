﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Motherboard_Diagnostic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartDiagnosic();
        }
        private void StartDiagnosic()
        {
            Diagnostic.Init();
            InitializeInstrumentPanel();
            Motherboard.Init();
        }
        private void RestartDiagnostic(object sender, RoutedEventArgs e)
        {
            Window repairWindow = ObjectsManager.FindChild<Window>(this, "RepairWindow");
            if (repairWindow != null)
            {
                repairWindow.Hide();
            }
            EventPanel.RemoveAllEvents();
            Button bt = ObjectsManager.FindChild<Button>(this, "StartPCButton");
            bt.Background = Brushes.LightGreen;
            bt.Content = "Запустить ПК";
            StartDiagnosic();
        }
        private Instruments GetSelectedInstrument()
        {
            StackPanel instruments = InstrumentsPanel;
            foreach (var instr in instruments.Children.OfType<RadioButton>())
            {
                if (instr.IsChecked ?? false)
                {
                    return DiagnosticHandbook.InstrumentsDictionary.FirstOrDefault(x => x.Value == instr.Name).Key;
                }
            }
            return Instruments.Ohmmeter;
        }

        private void LaunchPCButton(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)e.Source;
            switch (bt.Content)
            {
                case "Запустить ПК":
                    if (Diagnostic.Faults.Count != 0)
                    {
                        EventPanel.AddEvent("ПК не запускается, устраните неисправности", EventType.Warning);
                    }
                    else
                    {
                        EventPanel.AddEvent("ПК запущен", EventType.Victory);
                    }
                    bt.Background = Brushes.IndianRed;
                    bt.Content = "Выключить";
                    Diagnostic.IsRunning = false;
                    Diagnostic.PCIsLaunch = true;
                    break;

                

                case "Выключить":
                    bt.Background = Brushes.LightGreen;
                    bt.Content = "Запустить ПК";
                    Diagnostic.IsRunning = true;
                    Diagnostic.PCIsLaunch = false;
                    EventPanel.AddEvent("ПК выключен");
                    break;
            }
        }
        private void DiagnosticPower(object sender, RoutedEventArgs e)
        {
            Motherboard.Power.MakeDiagnostic(GetSelectedInstrument());
        }

        private void DiagnosticSouthBridge(object sender, RoutedEventArgs e)
        {
            Motherboard.SouthBridge.MakeDiagnostic(GetSelectedInstrument());
        }

        private void DiagnosticBIOS(object sender, RoutedEventArgs e)
        {
            Motherboard.BIOS.MakeDiagnostic(GetSelectedInstrument());
        }
        private void DiagnosticBiosbattery(object sender, RoutedEventArgs e)
        {
            Motherboard.Biosbattery.MakeDiagnostic(GetSelectedInstrument());
        }

        private void DiagnosticPCIEInterface(object sender, RoutedEventArgs e)
        {
            Motherboard.PCInterface.MakeDiagnostic(GetSelectedInstrument());
        }

        private void RepairButton(object sender, RoutedEventArgs e)
        {
            if (Diagnostic.IsRunning)
            {
                new RepairWindow().Show();
            }
            else if (Diagnostic.PCIsLaunch)
            {
                EventPanel.AddEvent("Ремонт включенного компьютера невозможен", EventType.Warning);
            }
            else{
                EventPanel.AddEvent("Продиагностируйте неисправность");
            }
        }
        private void InitializeInstrumentPanel()
        {
            Thickness margin = new(50, 0, 0, 0);
            InstrumentsPanel.Children.Clear();
            foreach (var instr in DiagnosticHandbook.InstrumentsDictionary.Values)
            {
                RadioButton btn = new()
                {
                    Name = instr,
                    Content = DiagnosticHandbook.RusInstrumentsNames[instr],
                    Margin = margin,
                    VerticalAlignment = VerticalAlignment.Center,
                    GroupName = "Instruments"
                };
                InstrumentsPanel.Children.Add(btn);
            }
            ((RadioButton)InstrumentsPanel.Children[0]).IsChecked = true;
        }
    }
}
