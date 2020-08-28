using Estrol.KREmu.Servers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Estrol.KREmu {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public SessionHandler session;

        public MainWindow() {
            InitializeComponent();
            Console.SetOut(new MultiTextWriter(new ControlWriter(ConsoleWindow), Console.Out));
            session = new SessionHandler(this);

            session.Start();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Environment.Exit(0);
        }
    }

    public class MultiTextWriter : TextWriter {
        private IEnumerable<TextWriter> writers;
        public MultiTextWriter(IEnumerable<TextWriter> writers) {
            this.writers = writers.ToList();
        }
        public MultiTextWriter(params TextWriter[] writers) {
            this.writers = writers;
        }

        public override void Write(char value) {
            foreach (var writer in writers)
                writer.Write(value);
        }

        public override void Write(string value) {
            foreach (var writer in writers)
                writer.Write(value);
        }

        public override void Flush() {
            foreach (var writer in writers)
                writer.Flush();
        }

        public override void Close() {
            foreach (var writer in writers)
                writer.Close();
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }

    public class ControlWriter : TextWriter {
        private TextBox textbox;
        public ControlWriter(TextBox textbox) {
            this.textbox = textbox;
        }

        public override void Write(char value) {
            textbox.Dispatcher.Invoke(new Action(() => {
                textbox.AppendText(value.ToString());
            }));
        }

        public override void Write(string value) {
            textbox.Dispatcher.Invoke(new Action(() => {
                textbox.AppendText(value.ToString());
            }));
        }

        public override Encoding Encoding {
            get { return Encoding.ASCII; }
        }
    }
}
