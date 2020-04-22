using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MemoryWpf
{
    class MemoryCard : Button
    {
        int bildID;
        Image bildVorne, bildHinten;

        //bild position
        int bildPos;

        bool umgedreht;
        bool nochImSpiel;

        MemoryFeld spiel;

        //der Konstruktor
        public MemoryCard(string vorne, int bildID, MemoryFeld spiel)
        {
            //vorderseite
            bildVorne = new Image();
            bildVorne.Source = new BitmapImage(new Uri(vorne, UriKind.Relative));

            //rueckseite
            bildHinten = new Image();
            bildHinten.Source = new BitmapImage(new Uri("Image/hinten.bmp", UriKind.Relative));

            //die Eigenschaften zuweisen
            Content = bildHinten;
            this.bildID = bildID;

            //die Karte ist nicht umgedreht und noch im Spiel
            umgedreht = false;
            nochImSpiel = true;

            this.spiel = spiel;

            Click += new RoutedEventHandler(ButtonClick);
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            //wenn Karte noch im Spiel?
            if ((nochImSpiel == false) || (spiel.ZugErlaubt() == false))
            {
                return;
            }
            if (umgedreht == false)
            {
                VorderseiteZeigen();
                spiel.KarteOeffnen(this);
            }
        }

        public void RueckseiteZeigen(bool rausnehmen)
        {
            if (rausnehmen == true)
            {
                //das Bild aufgedeckt zeigen und die Karte wird vom Spiel rausgenommen
                Image bildRausgenommen = new Image();
                bildRausgenommen.Source = new BitmapImage(new Uri("Image/aufgedeckt.bmp", UriKind.Relative));
                Content = bildRausgenommen;
                nochImSpiel = false;
            }
            else
            {
                Content = bildHinten;
                umgedreht = false;
            }
        }

        public void VorderseiteZeigen()
        {
            Content = bildVorne;
            umgedreht = true;
        }
        //die Methode liefert die Bild-ID einer Karte
        public int GetBildID()
        {
            return bildID;
        }

        //die Methode liefert die Position einer Karte
        public int GetBildPos()
        {
            return bildPos;
        }

        //die Methode setzt die Position einer Karte
        public void SetBildPos(int bildPos)
        {
            this.bildPos = bildPos;
        }

        //die Methode liefert den Wert des Felds umgedreht
        public bool IsUmgedreht()
        {
            return umgedreht;
        }

        //die Methode liefert den Wert des Feld nochImSpiel
        public bool IsNochImSpiel()
        {
            return nochImSpiel;
        }

    }
}
