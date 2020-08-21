using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using OpenMacroBoard.SDK;
using StreamDeckSharp;
using WpfMvvmTest.Annotations;

namespace WpfMvvmTest {
    public static class StreamDeck {
        private static IStreamDeckBoard _deck;

        public static IStreamDeckBoard Deck {
            get {
                if (_deck != null) return _deck;

                _deck = StreamDeckSharp.StreamDeck.OpenDevice();
                AppDomain.CurrentDomain.ProcessExit += (s, e) => { Deck_Clear(); };
                _deck.ConnectionStateChanged += (s, e) => { ConnectionStateChanged(e); };
                return _deck;
            }
        }

        private static ObservableCollection<KeyBitmap> _bitmaps;

        public static ObservableCollection<KeyBitmap> Bitmaps {
            get {
                if (_bitmaps != null)
                    return _bitmaps;

                _bitmaps = new ObservableCollection<KeyBitmap>();
                for (var i = 0; i < _deck.Keys.Count; i++) {
                    _bitmaps.Add(KeyBitmap.Black);
                }

                _bitmaps.CollectionChanged += (obj, e) => { NotifyCollectionChangedEventHandler(e); };
                return _bitmaps;
            }
        }

        private static void ConnectionStateChanged(ConnectionEventArgs eventArgs) {
            if (eventArgs.NewConnectionState) {
                NotifyCollectionChangedEventHandler(null);
            }
        }

        private static void NotifyCollectionChangedEventHandler([CanBeNull] NotifyCollectionChangedEventArgs e) {
            if (e == null) {
                for (var i = 0; i < _deck.Keys.Count; i++) {
                    _deck.SetKeyBitmap(i, _bitmaps[i % _bitmaps.Count]);
                }
            }
            else {
                _deck.SetKeyBitmap(e.NewStartingIndex, _bitmaps[e.NewStartingIndex % _bitmaps.Count]);
            }
        }

        private static void Deck_Clear() {
            _deck.ShowLogo();
        }

        public static KeyBitmap ConvertTextToImage(string txt, string fontName, int fontsize, Color bgColor,
            Color fColor) {
            var bmp = new Bitmap(72, 72);
            using (var graphics = Graphics.FromImage(bmp)) {
                var fColorBrush = new SolidBrush(fColor);

                graphics.FillRectangle(new SolidBrush(bgColor), 0, 0, bmp.Width, bmp.Height);

                var font = new Font(fontName, fontsize);
                var background = new Rectangle(0, 0, bmp.Width, bmp.Height);
                var stringFormat = new StringFormat {
                    Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center
                };
                graphics.DrawString(txt, font, fColorBrush, background, stringFormat);
                graphics.DrawRectangle(new Pen(fColorBrush), background);

                graphics.Flush();
                font.Dispose();
                graphics.Dispose();
            }

            return KeyBitmap.Create.FromBitmap(bmp);
        }
    }
}
