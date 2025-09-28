using System;
using System.Windows.Forms;
using MemoryLedgerApp.Services;

namespace MemoryLedgerWinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var storage = new DiaryStorage(AppContext.BaseDirectory);
        Application.Run(new MainForm(storage));
    }
}
