using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RCSHepler.ViewModels;

public class TaskBarIconViewModel
{
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; } = null!;
        public Func<bool> CanExecuteFunc { get; set; } = null!;

        public void Execute(object? parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object? parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    /// <summary>
    /// 显示主窗口
    /// </summary>
    public ICommand ShowWindowCommand
    {
        get
        {
            return new DelegateCommand
            {
                CanExecuteFunc = () => Application.Current.MainWindow.Visibility == Visibility.Hidden,
                CommandAction = () =>
                {
                    Application.Current.MainWindow.Show();
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                }
            };
        }
    }

    /// <summary>
    /// 隐藏主窗口
    /// </summary>
    public ICommand HideWindowCommand
    {
        get
        {
            return new DelegateCommand
            {
                CommandAction = () => Application.Current.MainWindow.Hide(),
                CanExecuteFunc = () => Application.Current.MainWindow.Visibility == Visibility.Visible,
            };
        }
    }

    /// <summary>
    /// 退出程序
    /// </summary>
    public ICommand ExitApplicationCommand
    {
        get
        {
            return new DelegateCommand
            {
                CommandAction = () => Application.Current.Shutdown(),
                CanExecuteFunc = () => true
            };
        }
    }
}
