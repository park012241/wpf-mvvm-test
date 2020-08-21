using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using OpenMacroBoard.SDK;
using StreamDeckSharp.Exceptions;
using WpfMvvmTest.Annotations;
using WpfMvvmTest.ViewModel;

namespace WpfMvvmTest {
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private readonly MainViewModel _mainViewModel;

        private ObservableCollection<bool> _status;

        private ObservableCollection<bool> Status {
            get {
                if (_status != null) return _status;

                _status = new ObservableCollection<bool>();
                for (var i = 0; i < StreamDeck.Deck.Keys.Count; i++) {
                    _status.Add(false);
                }

                _status.CollectionChanged += (s, e) => StatusChangedHandler(e);

                return _status;
            }
        }


        public MainWindow() {
            InitializeComponent();

            try {
                var deck = StreamDeck.Deck;
                deck.SetBrightness(100);

                for (var i = 0; i < deck.Keys.Count; i++)
                    switch (i) {
                        default:
                            StreamDeck.Bitmaps[i] = StreamDeck.ConvertTextToImage((i + 1).ToString(),
                                "JetBrains Mono", 28, Color.Black, Color.White);
                            break;
                    }


                deck.KeyStateChanged += StreamDeckEventHandler;
            }
            catch (StreamDeckNotFoundException) { }


            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;
        }

        private void StreamDeckEventHandler(object sender, KeyEventArgs e) {
            if (!(sender is IMacroBoard)) return;

            if (!e.IsDown) return;
            _mainViewModel.Number = e.Key + 1;
            Status[e.Key] = !Status[e.Key];
        }

        private void StatusChangedHandler([CanBeNull] NotifyCollectionChangedEventArgs eventArgs) {
            if (eventArgs != null)
                StreamDeck.Bitmaps[eventArgs.NewStartingIndex] = StreamDeck.ConvertTextToImage(
                    (eventArgs.NewStartingIndex + 1).ToString(),
                    "JetBrains Mono", 28,
                    Status[eventArgs.NewStartingIndex] ? Color.White : Color.Black,
                    Status[eventArgs.NewStartingIndex] ? Color.Black : Color.White);
            else
                for (var i = 0; i < StreamDeck.Deck.Keys.Count; i++) {
                    StreamDeck.Bitmaps[i] = StreamDeck.ConvertTextToImage((i + 1).ToString(),
                        "JetBrains Mono", 28, Status[i] ? Color.White : Color.Black,
                        Status[i] ? Color.Black : Color.White);
                }
        }
    }
}
