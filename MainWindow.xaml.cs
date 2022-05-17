using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace GoWithTheFlow
{
    public partial class MainWindow : Window
    {
        readonly VM vm;
        public MainWindow()
        {
            InitializeComponent();
            vm = VM.Instance;
            DataContext = vm;
        }
        private void ImportFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenFileDialogObj = new OpenFileDialog
            {
                Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*"
            };
            if (OpenFileDialogObj.ShowDialog() == true)
            {
                vm.Input = OpenFileDialogObj.FileName;
                try
                {
                    vm.Input = File.ReadAllText(OpenFileDialogObj.FileName);
                }
                catch (IOException ex)
                {
                    MessageBox.Show("The file could not be read: " + ex);
                }
            }
            if (vm.Input != "")
            {
                vm.ReadFile();
                vm.FindRiver();
            }
        }

    }
}
