using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
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

namespace WPF_calculator;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            string btnStr = btn.Content.ToString()!;
            double num;
            if (BigTextBlock.Text != string.Empty && double.TryParse(BigTextBlock.Text, NumberStyles.Float, CultureInfo.DefaultThreadCurrentCulture, out num) && !"1234567890".Contains(btnStr))
            {
                if (num > 0)
                {
                    string number = num.ToString();
                    MiniTextBlock.Text = string.Empty;
                    foreach (var digit in number)
                    {
                        if (digit == ',')
                        {
                            MiniTextBlock.Text += '.';
                            continue;
                        }
                        MiniTextBlock.Text += digit;
                    }
                }
            }
            BigTextBlock.Text = string.Empty;

            
            if (MiniTextBlock.Text == string.Empty && "^x+-÷".Contains(btnStr))
            {
                return;
            }
            else if (MiniTextBlock.Text == string.Empty && btnStr == ".")
            {
                MiniTextBlock.Text += "0";
            }
            else if (MiniTextBlock.Text != string.Empty && "^x+-÷.".Contains(MiniTextBlock.Text[MiniTextBlock.Text.Length - 1]))
            {
                string txt = string.Empty;


                if ("^x+-÷.".Contains(btnStr))
                {
                    for (int i = 0; i < MiniTextBlock.Text.Length - 1; ++i)
                    {
                        txt += MiniTextBlock.Text[i];
                    }
                    txt += btnStr;
                    MiniTextBlock.Text = txt;
                }
                else
                {
                    MiniTextBlock.Text += btnStr;
                }
                return;
            }
            MiniTextBlock.Text += btn.Content.ToString();
        }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        MiniTextBlock.Text = string.Empty;
        BigTextBlock.Text = string.Empty;
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        string txt = string.Empty;
        for (int i = 0; i < MiniTextBlock.Text.Length - 1; ++i)
        {
            txt += MiniTextBlock.Text[i];
        }
        MiniTextBlock.Text = txt;
        BigTextBlock.Text = string.Empty;
    }

    private void EqualsButton_Click(object sender, RoutedEventArgs e)
    {
        Stack<double> answer = new Stack<double>();
        Queue<string> operands = new Queue<string>();
        Stack<string> operators = new Stack<string>();
        string number = string.Empty;
        int lenCount = 0;
        try
        {
            if (MiniTextBlock.Text == string.Empty)
            {
                return;
            }
            foreach (char ch in MiniTextBlock.Text)
            {
                ++lenCount;
                if ((ch <= '9' && ch >= '0') || ch == '.')
                {
                    number += ch;
                    if (lenCount == MiniTextBlock.Text.Length)
                    {
                        operands.Enqueue(number);
                    }
                }
                else
                {
                    operands.Enqueue(number);
                    number = string.Empty;
                    if (operators.Count == 0)
                    {
                        operators.Push(ch.ToString());
                        continue;
                    }
                    if ("+-".Contains(ch) && "x^÷".Contains(operators.Peek()))
                    {
                        operands.Enqueue(operators.Pop());
                    }
                    else if ("÷x".Contains(ch) && "^".Contains(operators.Peek()))
                    {
                        operands.Enqueue(operators.Pop());
                    }
                    else if ("+-".Contains(ch.ToString()) && "+-".Contains(operators.Peek()))
                    {
                        operands.Enqueue(operators.Pop());
                    }
                    else if ("x÷".Contains(ch.ToString()) && "x÷".Contains(operators.Peek()))
                    {
                        operands.Enqueue(operators.Pop());
                    }
                    else if (ch == '^' && operators.Peek() == "^")
                    {
                        operands.Enqueue(operators.Pop());
                    }
                    operators.Push(ch.ToString());
                }
            }

            int len = operators.Count;
            for (int i = 0; i < len; i++)
            {
                operands.Enqueue(operators.Pop());
            }

            double num1;
            double num2;
            foreach (string op in operands)
            {
                if (double.TryParse(op, NumberStyles.Float, CultureInfo.InvariantCulture, out num1))
                {
                    answer.Push(num1);
                }
                else if ("^+-x÷".Contains(op))
                {
                    num2 = answer.Pop();
                    num1 = answer.Pop();
                    switch (op)
                    {
                        case "÷":
                            if (num2 == 0)
                            {
                                BigTextBlock.Text = "Division by zero!";
                                return;
                            }
                            answer.Push(num1 / num2);
                            break;
                        case "^":
                            answer.Push(Math.Pow(num1, num2));
                            break;
                        case "x":
                            answer.Push(num1 * num2);
                            break;
                        case "+":
                            answer.Push(num1 + num2);
                            break;
                        case "-":
                            answer.Push(num1 - num2);
                            break;
                    }
                }
                else
                {
                    BigTextBlock.Text = "Wrong Input!";
                    return;
                }
            }
            BigTextBlock.Text = answer.Pop().ToString();
        }
        catch
        {
            BigTextBlock.Text = "Wrong Input!";
        }
    }
}
