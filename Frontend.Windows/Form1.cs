using Backend;
using System.Globalization;

namespace Frontend.Windows
{
    public partial class Form1 : Form
    {
        private readonly Label _display = new();
        private string _expression = string.Empty; // expression that the user is typing.
        private bool _showingResult;               // true is the screen displays "result-expression"

        public Form1()
        {
            InitializeComponent();
            BuildCalculator();
        }

        // build the entire interface by code: screen + buttons on a grid.
        private void BuildCalculator()
        {
            Text = "Functions Evaluator";
           ClientSize = new Size(600, 420);
            MinimumSize = new Size(500, 400);
            BackColor = Color.Black;
            StartPosition = FormStartPosition.CenterScreen;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 5,
                Padding = new Padding(8),
            };

                      
            for (var i = 0; i < 5; i++)
            {
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14));
            }

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            for (var i = 0; i < 5; i++)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            }

            // green screen (row o, occupies all six columns)
            _display.Dock = DockStyle.Fill;
            _display.BackColor = Color.Green;
            _display.ForeColor = Color.White;
            _display.Font = new Font("Segoe UI", 16);
            _display.TextAlign = ContentAlignment.MiddleLeft;
            _display.Margin = new Padding(4);
            layout.Controls.Add(_display, 0, 0);
            layout.SetColumnSpan(_display, 6);

            // buttons: text, columns, row, how many columns it occupies, and whether it is a number (white) o operator (orange)
            AddButton(layout, "7", 0, 1, 1, true);
            AddButton(layout, "8", 1, 1, 1, true);
            AddButton(layout, "9", 2, 1, 1, true);
            AddButton(layout, "(", 3, 1, 1, false);
            AddButton(layout, ")", 4, 1, 1, false);
            AddButton(layout, "Delete", 5, 1, 1, false);

            AddButton(layout, "4", 0, 2, 1, true);
            AddButton(layout, "5", 1, 2, 1, true);
            AddButton(layout, "6", 2, 2, 1, true);
            AddButton(layout, "*", 3, 2, 1, false);
            AddButton(layout, "/", 4, 2, 1, false);
            AddButton(layout, "Clear", 5, 2, 1, false);

            AddButton(layout, "1", 0, 3, 1, true);
            AddButton(layout, "2", 1, 3, 1, true);
            AddButton(layout, "3", 2, 3, 1, true);
            AddButton(layout, "+", 3, 3, 1, false);
            AddButton(layout, "-", 4, 3, 1, false);
            AddButton(layout, "^", 5, 3, 1, false);

            AddButton(layout, "0", 0, 4, 2, true);
            AddButton(layout, ".", 2, 4, 1, true);
            AddButton(layout, "=", 3, 4, 3, false);

            Controls.Add(layout);
        }

        private void AddButton(TableLayoutPanel layout, string text, int column, int row, int columnSpan, bool isNumber)
        {
            var button = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                Font = new Font("Segoe UI", 16),
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                BackColor = isNumber ? Color.White : Color.FromArgb(255, 128, 0),
                ForeColor = Color.Black,
            };

            button.Click += (sender, e) => OnButtonClick(text);

            layout.Controls.Add(button, column, row);
            layout.SetColumnSpan(button, columnSpan);
        }

        // a single handle for all buttons, acording to the buttons text.
        private void OnButtonClick(string text)
        {
            switch (text)
            {
                case "Clear":
                    ClearAll();
                    break;

                case "Delete":
                    if (_showingResult)
                    {
                        ClearAll();
                    }
                    else if (_expression.Length > 0)
                    {
                        _expression = _expression[..^1]; // removes the last character
                        _display.Text = _expression;
                    }
                    break;

                case "=":
                    Calculate();
                    break;

                default: // digits, '.', operatores and parentheses
                    if (_showingResult)
                    {
                        ClearAll(); // after a result, a new expression begins.
                    }

                    _expression += text;
                    _display.Text = _expression;
                    break;
            }
        }

        private void Calculate()
        {
            if (_expression.Length == 0 || _showingResult)
            {
                return;
            }

            try
            {
                var result = ExpressionEvaluator.Evalute(_expression);
                if (double.IsNaN(result) || double.IsInfinity(result))
                {
                    throw new Exception("Math error."); // for example division by zero
                }

                _display.Text = $"{_expression}={result.ToString(CultureInfo.InvariantCulture)}";
            }
            catch (Exception)
            {
                _display.Text = "Error";
            }

            _showingResult = true;
        }

        private void ClearAll()
        {
            _expression = string.Empty;
            _showingResult = false;
            _display.Text = string.Empty;
        }
    }
}