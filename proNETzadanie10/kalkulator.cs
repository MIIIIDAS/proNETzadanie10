using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace proNETzadanie10
{
    public enum Operation
    {
        None,
        Addition,
        Substraction,
        Division,
        Multiplication
    }

    public partial class kalkulator : Form
    {
        private string _firstValue;
        private string _secondValue;
        private Operation _currentOperation = Operation.None;
        private bool _isTheResultOnTheScreen;
        private Stopwatch initializationTimer;

        public kalkulator()
        {
            InitializeComponent();
            initializationTimer = Stopwatch.StartNew();
            textResult.Text = "0";
        }

        private void kalkulator_Load(object sender, EventArgs e)
        {
            initializationTimer.Stop();
            var initializationTime = initializationTimer.ElapsedMilliseconds;

            const int thresholdMilliseconds = 1000;

            if (initializationTime > thresholdMilliseconds)
            {
                LogEvent($"Inicjalizacja komponentów trwała zbyt długo: {initializationTime} ms");
            }
        }

        private void OnBtnNumberClick(object sender, EventArgs e)
        {
            var clickedValue = (sender as Button).Text;

            if (textResult.Text == "0" && clickedValue != ",")
                textResult.Text = string.Empty;
            if (_isTheResultOnTheScreen)
            {
                _isTheResultOnTheScreen = false;
                textResult.Text = string.Empty;
                if (clickedValue == ",")
                {
                    clickedValue = "0";
                }
            }

            textResult.Text += clickedValue;
            SetResultBtnState(true);

            if (_currentOperation != Operation.None)
                _secondValue += clickedValue;
            else
                SetOperationBtnState(true);
        }

        private void OnBtnOperationClick(object sender, EventArgs e)
        {
            _firstValue = textResult.Text;

            var operation = (sender as Button).Text;

            _currentOperation = operation switch
            {
                "+" => Operation.Addition,
                "-" => Operation.Substraction,
                "*" => Operation.Multiplication,
                "/" => Operation.Division,
                _ => Operation.None,
            };

            textResult.Text += $" {operation} ";

            if (_isTheResultOnTheScreen)
                _isTheResultOnTheScreen = false;

            SetOperationBtnState(false);
            SetResultBtnState(false);
        }

        private void OnBtnResultClick(object sender, EventArgs e)
        {
            if (_currentOperation == Operation.None)
            {
                return;
            }

            var firstNumber = double.Parse(_firstValue);
            var secondNumber = double.Parse(_secondValue);

            var result = Calculate(firstNumber, secondNumber);
            textResult.Text = result.ToString();
            _secondValue = string.Empty;
            _currentOperation = Operation.None;
            _isTheResultOnTheScreen = true;
            SetOperationBtnState(true);
            SetResultBtnState(true);
        }

        private double Calculate(double firstNumber, double secondNumber)
        {
            switch (_currentOperation)
            {
                case Operation.None:
                    return firstNumber;
                case Operation.Addition:
                    return firstNumber + secondNumber;
                case Operation.Substraction:
                    return firstNumber - secondNumber;
                case Operation.Division:
                    if (secondNumber == 0)
                    {
                        MessageBox.Show("Nie można dzielić przez 0!");
                        return 0;
                    }
                    return firstNumber / secondNumber;
                case Operation.Multiplication:
                    return firstNumber * secondNumber;
            }
            return 0;
        }

        private void OnBtnClearClick(object sender, EventArgs e)
        {
            textResult.Text = "0";
            _firstValue = string.Empty;
            _secondValue = string.Empty;
            _currentOperation = Operation.None;
        }

        private void SetOperationBtnState(bool value)
        {
            btnAddition.Enabled = value;
            btnSubtraction.Enabled = value;
            btnMultiplication.Enabled = value;
            btnDivision.Enabled = value;
        }

        private void SetResultBtnState(bool value)
        {
            btnResult.Enabled = value;
        }

        private void LogEvent(string message)
        {
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    EventLogEntryType eventType = EventLogEntryType.Information;

                    if (message.Contains("zbyt długo"))
                    {
                        eventType = EventLogEntryType.Error;
                    }

                    eventLog.Source = "proNETzadanie10";
                    eventLog.WriteEntry(message, eventType, 101, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas zapisu do rejestru zdarzeń: {ex.Message}");
            }
        }
    }
}
