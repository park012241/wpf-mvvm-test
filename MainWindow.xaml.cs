using System;
using System.Drawing;
using OpenMacroBoard.SDK;
using StreamDeckSharp.Exceptions;
using WpfMvvmTest.ViewModel;

namespace WpfMvvmTest {
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private readonly MainViewModel _mainViewModel;

        public MainWindow() {
            InitializeComponent();

            try {
                var deck = StreamDeck.Deck;
                deck.SetBrightness(100);

                for (var i = 0; i < deck.Keys.Count; i++)
                    switch (i) {
                        default:
                            StreamDeck.Bitmaps[i] = StreamDeck.ConvertTextToImage((i + 1).ToString(),
                                "JetBrains Mono", 32, Color.Black, Color.White, 72, 72);
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
            StreamDeck.Bitmaps[e.Key] = KeyBitmap.Black;
        }
    }
}
