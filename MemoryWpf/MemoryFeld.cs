using System;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace MemoryWpf
{
    class MemoryFeld
    {
        MemoryCard[] karten;

        //das Array fuer die Namen der Grafiken
        string[] bilder =
        {
            "Image/apricot.bmp", "Image/aquamarine.bmp",
            "Image/Beige.bmp", "Image/black.bmp",
            "Image/blue.bmp", "Image/brown.bmp",
            "Image/darkblue.bmp", "Image/darkgreen.bmp",
            "Image/darkred.bmp", "Image/green.bmp",
            "Image/grey.bmp", "Image/indigo.bmp",
            "Image/lilac.bmp", "Image/Magenta.bmp",
            "Image/orange.bmp", "Image/red.bmp",
            "Image/skyblue.bmp", "Image/violet.bmp",
            "Image/white.bmp", "Image/yellogreen.bmp",
            "Image/yellow.bmp" };

        //score
        int userScore, computerScore;
        Label userScoreLabel, computerScoreLabel;

        //Anzahl umgedrehte Karten
        int umgedrehteKarten;

        //fuer aktuelle paar
        MemoryCard[] paar;

        int spieler;

        //fuer Computer
        int[,] gemerkteKarte;

        //spielfeld
        UniformGrid feld;

        //timer
        DispatcherTimer timer;

        Button Hint;
        DispatcherTimer hintTimer;

        int spielstaerke;

        //der konstuktor
        public MemoryFeld(UniformGrid feld)
        {
            int count = 0;

            karten = new MemoryCard[42];
            paar = new MemoryCard[2];

            gemerkteKarte = new int[2, 21];

            userScore = 0;
            computerScore = 0;

            //fuer karten mischen
            Random zufall = new Random();

            //noch kein Karte umgedreht
            umgedrehteKarten = 0;

            spieler = 0;

            spielstaerke = 10;

            this.feld = feld;

            //es gibt keine gemerkten Karten
            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < 21; j++)
                {
                    gemerkteKarte[i, j] = -1;
                }
            }

            //spielfeld erstellen
            for(int i = 0; i < 42; i++)
            {
                //neue Karte erzeugen
                karten[i] = new MemoryCard(bilder[count], count, this);
                //bei jeder zweiten Karte kommt neues Bild
                if ((i + 1) % 2 == 0)
                {
                    count++;
                }
            }

            //die karte mischen
            for(int i = 0; i < 42; i++)
            {
                int temp1;
                MemoryCard temp2;
                temp1 = zufall.Next(42);
                temp2 = karten[temp1];
                karten[temp1] = karten[i];
                karten[i] = temp2;
            }

            //karten auf Spielfeld positinieren
            for(int i = 0; i < 42; i++)
            {
                karten[i].SetBildPos(i);
                //die Karte hinzufeugen.
                feld.Children.Add(karten[i]);
            }

            //die labels fuer die Punkte
            Label user = new Label();
            user.Content = "User";
            feld.Children.Add(user);
            userScoreLabel = new Label();
            userScoreLabel.Content = 0;
            feld.Children.Add(userScoreLabel);

            Label computer = new Label();
            computer.Content = "Computer";
            feld.Children.Add(computer);
            computerScoreLabel = new Label();
            computerScoreLabel.Content = 0;
            feld.Children.Add(computerScoreLabel);

            //Timer erzeugen
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(2000);
            timer.Tick += new EventHandler(Timer_Tick);

            //Hintbutton erzeugen
            Hint = new Button();
            Hint.Height = 50;
            Hint.Width = 50;
            Hint.VerticalAlignment = VerticalAlignment.Top;
            Hint.Click += new RoutedEventHandler(HintClick);
            Hint.Content = "Hint";
            feld.Children.Add(Hint);

            //Hint timer erzeugen
            hintTimer = new DispatcherTimer();
            hintTimer.Interval = TimeSpan.FromMilliseconds(3000);
            hintTimer.Tick += new EventHandler(hintTimer_Tick);
        }
        //die Methode liefert, ob Züge des Menschen erlaubt sind
        //die Rückgabe ist false, wenn gerade der Computer zieht 
        //oder wenn schon zwei Karten umgedreht sind
        //sonst ist die Rückgabe true
        public bool ZugErlaubt()
        {
            bool erlaubt = true;
            //zieht der Computer?
            if (spieler == 1)
                erlaubt = false;
            //sind schon zwei Karten umdreht?
            if (umgedrehteKarten == 2)
                erlaubt = false;
            return erlaubt;
        }

        //die Methode ueberprueft,ob Spieler dran ist und noch nicht Hint benutzt hat
        public void GetHintStatus()
        {
            if ((ZugErlaubt() == true) && (umgedrehteKarten == 0))
            {
                Hint.IsEnabled = true;
                Hint.Opacity = 1;
            }
            else
            {
                Hint.IsEnabled = false;
                Hint.Opacity = 0.5;
            }
        }

        private void HintClick(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < 42; i++)
            {
                //die aufgedeckte karten wird unveraendert. nur restliche karten werden gedreht.
                if (karten[i].IsNochImSpiel() == true)
                {
                    karten[i].VorderseiteZeigen();
                }
            }
            hintTimer.Start();
        }

        private void HintEnd()
        {
            for(int i = 0; i < 42; i++)
            {
                if (karten[i].IsNochImSpiel() == true)
                {
                    karten[i].RueckseiteZeigen(false);
                }
            }
        }

        private void hintTimer_Tick(object sender, EventArgs e)
        {
            hintTimer.Stop();
            HintEnd();
        }

        //die Methode für den Timer
        private void Timer_Tick(object sender, EventArgs e)
        {
            //den Timer anhalten 
            timer.Stop();
            //die Karten zurückdrehen
            KartenSchliessen();
        }

        //die Methode wird jedesmal beim Anklicken ausgefuert.
        public void KarteOeffnen(MemoryCard karte)
        {
            //zwischenspeichern
            int kartenID, kartenPos;
            paar[umgedrehteKarten] = karte;
            //ID und Pos beschaffen
            kartenID = karte.GetBildID();
            kartenPos = karte.GetBildPos();

            //die karte in das Gedaechtnis des Computers eintragen
            //erste karte gefunden
            if (gemerkteKarte[0, kartenID] == -1)
            {
                gemerkteKarte[0, kartenID] = kartenPos;
            }
            else
            {
                //wenn es schon einmal kartenid gespeichert aber nicht mit dem position stimmt, dann ist zweite karte gefunden
                if (gemerkteKarte[0, kartenID] != kartenPos)
                {
                    gemerkteKarte[1, kartenID] = kartenPos;
                }
            }

            umgedrehteKarten++;

            //sind zwei Karten umgedreht?
            if (umgedrehteKarten == 2)
            {
                //paar wird gepreuft
                paarPruefen(kartenID);
                //timer zum umdrehen der karten starten
                timer.Start();
            }

            //sind alle paare gefunden?
            if (computerScore + userScore == 21)
            {
                string ergebnisText;
                string gewinner;
                int gewinnScore;
                if (computerScore > userScore)
                {
                    gewinner = "Computer";
                    gewinnScore = computerScore;
                }
                else
                {
                    gewinner = "User";
                    gewinnScore = userScore;
                }

                timer.Stop();
                ergebnisText = "User score: " + userScore.ToString() + "\nComputer score: " + computerScore.ToString() + "\n" + gewinner + " won with " + gewinnScore + " Points";
                MessageBox.Show(ergebnisText, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
            }
            //jedesmal nach Anklicken wurd geprueft, ob hind aktive ist
            GetHintStatus();

        }

        private void paarPruefen(int kartenID)
        {
            if (paar[0].GetBildID() == paar[1].GetBildID())
            {
                paarGefunden();
                //karte vom Gedaechtnis loeschen
                gemerkteKarte[0, kartenID] = -2;
                gemerkteKarte[1, kartenID] = -2;
            }
        }

        private void paarGefunden()
        {
            if (spieler == 0)
            {
                userScore++;
                userScoreLabel.Content = userScore.ToString();
            }
            else
            {
                computerScore++;
                computerScoreLabel.Content = computerScore.ToString();
            }
        }

        private void KartenSchliessen()
        {
            bool raus = false;
            //wenn es ein paar ist, werden die karte rausgenommen
            if (paar[0].GetBildID() == paar[1].GetBildID())
                raus = true;
            //wenn es ein Paar war, nehmen wir die Karten aus 
            //dem Spiel, sonst drehen wir sie nur wieder um
            paar[0].RueckseiteZeigen(raus);
            paar[1].RueckseiteZeigen(raus);
            //es ist keine Karte mehr geöffnet
            umgedrehteKarten = 0;
            //hat der Spieler kein Paar gefunden?
            if (raus == false)
                //dann wird der Spieler gewechselt
                SpielerWechseln();
            else
                //hat der Computer ein Paar gefunden?
                //dann ist er noch einmal an der Reihe
                if (spieler == 1) 
                ComputerZug();
        }

        private void SpielerWechseln()
        {
            if (spieler == 0)
            {
                spieler = 1;
                ComputerZug();
            }
            else
                spieler = 0;
            GetHintStatus();
        }

        private void ComputerZug()
        {

        }
        
    }
}
