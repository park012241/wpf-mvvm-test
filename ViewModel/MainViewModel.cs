﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfMvvmTest.Annotations;

namespace WpfMvvmTest.ViewModel {
    public sealed class MainViewModel : INotifyPropertyChanged {
        private int _iNumber;

        private ICommand _minusCommand;

        private string _pageContents;

        private ICommand _plusCommand;

        public MainViewModel() {
            Number = 1;
        }

        public int Number {
            get => _iNumber;
            set {
                var iOldNumber = _iNumber;

                _iNumber = value;
                OnPropertyChanged(nameof(Number));

                if (_iNumber > 0 && _iNumber <= 10) {
                    OnPropertyChanged(nameof(MinusEnable));
                    OnPropertyChanged(nameof(PlusEnable));

                    PageContents = $"{_iNumber} 페이지를 보고 있어요";
                }
                else {
                    MessageBox.Show("1~10 페이지만 입략 가능합니다.");
                    _iNumber = iOldNumber;
                    OnPropertyChanged(nameof(Number));
                }
            }
        }

        public ICommand MinusCommand => _minusCommand ?? (_minusCommand = new DelegateCommand(Minus));

        public ICommand PlusCommand => _plusCommand ?? (_plusCommand = new DelegateCommand(Plus));

        public string PageContents {
            get => _pageContents;
            set {
                _pageContents = value;
                OnPropertyChanged(nameof(PageContents));
            }
        }

        public bool MinusEnable => Number > 1;
        public bool PlusEnable => Number < 10;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Minus() {
            Number--;
        }

        private void Plus() {
            Number++;
        }
    }

    #region DelegateCommand Class

    public class DelegateCommand : ICommand {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        /// <summary>
        ///     Initializes a new instance of the DelegateCommand class.
        /// </summary>
        /// <param name="execute">indicate an execute function</param>
        public DelegateCommand(Action execute) : this(execute, null) { }

        /// <summary>
        ///     Initializes a new instance of the DelegateCommand class.
        /// </summary>
        /// <param name="execute">execute function </param>
        /// <param name="canExecute">can execute function</param>
        private DelegateCommand(Action execute, Func<bool> canExecute) {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        ///     can executes event handler
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///     implement of iCommand can execute method
        /// </summary>
        /// <param name="o">parameter by default of iCommand interface</param>
        /// <returns>can execute or not</returns>
        public bool CanExecute(object o) {
            return _canExecute == null || _canExecute();
        }

        /// <summary>
        ///     implement of icommand interface execute method
        /// </summary>
        /// <param name="o">parameter by default of iCommand interface</param>
        public void Execute(object o) {
            _execute();
        }

        /// <summary>
        ///     raise ca excute changed when property changed
        /// </summary>
        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion
}
