using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private static List<Osoba>[] platformy = new List<Osoba>[5];
        private static Komora_windy winda;
        private static int obecne_pietro; //pietro na ktore moze zmierzyc winda
        private System.Windows.Threading.DispatcherTimer zegar_1_start, zegar_2, zegar_3, zegar_4, zegar_5, zegar_6, zegar_7; //1 Timer
        private int winda_pozycja, animacja_drzwi = 0, stoper_powrotu_windy = 0;
        private static List<int> predkosc = new List<int>();
        private List<List<int>> Przejscie_do_windy;
        private double wyjscie_z_windy = 0;

        public void InicjalizujZegary()
        {
            zegar_1_start = new System.Windows.Threading.DispatcherTimer();
            zegar_2 = new System.Windows.Threading.DispatcherTimer();
            zegar_3 = new System.Windows.Threading.DispatcherTimer();
            zegar_4 = new System.Windows.Threading.DispatcherTimer();
            zegar_5 = new System.Windows.Threading.DispatcherTimer();
            zegar_6 = new System.Windows.Threading.DispatcherTimer();
            zegar_7 = new System.Windows.Threading.DispatcherTimer();
            zegar_1_start.Tick += Tyk_zegar_1_start;
            zegar_2.Tick += Tyk_zegar_2;
            zegar_3.Tick += Tyk_zegar_3;
            zegar_4.Tick += Tyk_zegar_4;
            zegar_5.Tick += Tyk_zegar_5;
            zegar_6.Tick += Tyk_zegar_6;
            zegar_7.Tick += Tyk_zegar_7;
            zegar_1_start.Interval = new TimeSpan(0, 0, 0, 0, 100);
            zegar_2.Interval = new TimeSpan(0, 0, 0, 0, 10);
            zegar_3.Interval = new TimeSpan(0, 0, 0, 0, 1);
            zegar_4.Interval = new TimeSpan(0, 0, 0, 0, 10);
            zegar_5.Interval = new TimeSpan(0, 0, 0, 0, 10);
            zegar_6.Interval = new TimeSpan(0, 0, 0, 0, 1);
            zegar_7.Interval = new TimeSpan(0, 0, 0, 0, 10);
        }
        public void InicjalizujSymulacje()
        {
            Random rn = new Random();
            for (int i = 0; 8 > i; i++)
            {
                predkosc.Add(rn.Next(9, 13));
            }
            for (int i = 0; 5 > i; i++)
            {
                platformy[i] = new List<Osoba>();
            }
            winda = new Komora_windy(Okno);
            obecne_pietro = 0;
            winda_pozycja = 660;

            InicjalizujZegary();

            zegar_1_start.Start();
        }

        public void Update_platforma(int x)
        {
            for (int i = 0; platformy[x].Count > i; i++)
            {
                platformy[x][i].Update_Osoba(i, x);
            }
        }

        private static bool Sprawdz_oplacalnosc_postoju() //sprawdza czy w miedzyczasie oplacalne jest odwiedzenie innego pietra mimo ze winda w tym momencie nie zawiera pasaezerow o numerze pietra
        {
            if (winda.miejsce != 0) //sprawdza czy miesjce jest jeszcze w windzie
            {
                for (int i = 0; platformy[obecne_pietro].Count > i; i++)
                {
                    if ((winda.kierunek && platformy[obecne_pietro][i].wartosc >= obecne_pietro) || (winda.kierunek == false && platformy[obecne_pietro][i].wartosc <= obecne_pietro && platformy[obecne_pietro][i].wartosc != -1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool Sprawdz_postoj() //sprawdza czy pietro jest oplacalne odwiedzenia
        {
            for (int i = 0; 8 > i; i++)
            {
                if (winda.pasazerowie[i].wartosc == obecne_pietro || Sprawdz_oplacalnosc_postoju())
                {
                    return true;
                }
            }
            return false;
        }

        private static bool Koniec_programu() //sprawdza czy kazdy doszedl na swoje miejsce
        {
            for (int i = 0; 5 > i; i++)
            {
                if (platformy[i].Count != 0)
                {
                    return true;
                }
            }
            for (int i = 0; 8 > i; i++)
            {
                if (winda.pasazerowie[i].wartosc != -1)
                {
                    return true;
                }
            }
            return false;
        }

        public MainWindow()
        {
            InitializeComponent();
            InicjalizujSymulacje();
        }

        private void Tyk_zegar_1_start(object sender, EventArgs e) //Zegar początku dzialania windy
        {
            if (Koniec_programu())
            {
                zegar_5.Stop();
                stoper_powrotu_windy = 0;
                zegar_1_start.Stop();
                if (platformy[obecne_pietro].Count != 0)
                {
                    zegar_2.Start();
                }
                else
                {
                    zegar_7.Start();
                }
            }
        }

        private void Nastepne_pietro()
        {
            if (obecne_pietro == 4)
            {
                winda.kierunek = false;
            }
            else if (obecne_pietro == 0)
            {
                winda.kierunek = true;
            }

            if (Koniec_programu())
            {
                rozmiar = platformy[obecne_pietro].Count;
                Przejscie_do_windy = winda.Wsiadz_do_windy(platformy[winda.pietro_windy]);
                odleglosc.Clear();
                for (int i = 0, j = 0; platformy[obecne_pietro].Count > i; i++)
                {
                    if (j < Przejscie_do_windy.Count && Przejscie_do_windy[j][0] == i)
                    {
                        odleglosc.Add(Canvas.GetLeft(platformy[obecne_pietro][i].obrazek) - 785 - (35 * Przejscie_do_windy[j][1]));
                        j++;
                    }
                    else
                    {
                        odleglosc.Add(obecne_pietro % 2 == 0 ? -60 * j : 60 * j);
                    }
                }
                zegar_3.Start();
            }
            else
            {
                zegar_7.Start();
            }
        }

        private void Tyk_zegar_2(object sender, EventArgs e) //Animacja otwierania drzwi
        {
            animacja_drzwi++;
            if (obecne_pietro % 2 == 0)
            {
                Drzwi_1.Height++;
                Canvas.SetTop(Drzwi_1, Canvas.GetTop(Drzwi_1) - 1);
            }
            else
            {
                Drzwi_2.Height++;
                Canvas.SetTop(Drzwi_2, Canvas.GetTop(Drzwi_2) - 1);
            }
            if (animacja_drzwi == 120)
            {
                if (winda.Oproznij_winde())
                {
                    Waga.Content = "Ciężar windy: " + Convert.ToString((8 - winda.miejsce) * 70);
                    zegar_2.Stop();
                    zegar_6.Start();
                }
                else
                {
                    Nastepne_pietro();
                    zegar_2.Stop();
                }
            }
        }

        private void Tyk_zegar_6(object sender, EventArgs e) //Animacja wysiadania z windy
        {
            wyjscie_z_windy++;
            for (int i = 0; 8 > i; i++)
            {
                if (winda.pasazerowie[i].wartosc == -1 && winda.pasazerowie[i].obrazek.Visibility == Visibility.Visible)
                {
                    Canvas.SetLeft(winda.pasazerowie[i].obrazek, obecne_pietro % 2 == 0 ? Canvas.GetLeft(winda.pasazerowie[i].obrazek) - predkosc[i] : Canvas.GetLeft(winda.pasazerowie[i].obrazek) + predkosc[i]);
                    Canvas.SetLeft(winda.pasazerowie[i].tekst, obecne_pietro % 2 == 0 ? Canvas.GetLeft(winda.pasazerowie[i].tekst) - predkosc[i] : Canvas.GetLeft(winda.pasazerowie[i].tekst) + predkosc[i]);
                    Canvas.SetTop(winda.pasazerowie[i].obrazek, winda_pozycja + 30 - Math.Abs(Math.Sin(wyjscie_z_windy / 10) * 20));
                    Canvas.SetTop(winda.pasazerowie[i].tekst, winda_pozycja + 50 - Math.Abs(Math.Sin(wyjscie_z_windy / 10) * 20));
                }
            }
            if (wyjscie_z_windy == 80)
            {
                wyjscie_z_windy = 0;
                winda.Update_winda();
                Nastepne_pietro();
                zegar_6.Stop();
            }
        }

        private void Znajdz_nastepne_pietro()
        {
            winda.Update_winda();
            Waga.Content = "Ciężar windy: " + Convert.ToString((8 - winda.miejsce) * 70);
            Update_platforma(winda.pietro_windy);
            do
            {

                if (winda.kierunek)
                {
                    obecne_pietro++;
                }
                else
                {
                    obecne_pietro--;
                }

                if (obecne_pietro == 4)
                {
                    winda.kierunek = false;
                }
                else if (obecne_pietro == 0)
                {
                    winda.kierunek = true;
                }

            } while (Sprawdz_postoj() == false && Koniec_programu()); //sprawdza ktore pietro nestepnie jest warte odwiedzenia
            winda.pietro_windy = obecne_pietro; //przemieszcza winde na pietro warte odwiedzenia
        }

        private List<double> odleglosc = new List<double>();
        private int rozmiar = 0;
        private void Tyk_zegar_3(object sender, EventArgs e) //Animacja wsiadania do windy
        {
            wyjscie_z_windy++;
            for (int i = 0; rozmiar > i; i++)
            {
                if (odleglosc[i] != 0)
                {
                    Canvas.SetLeft(platformy[obecne_pietro][i].obrazek, Canvas.GetLeft(platformy[obecne_pietro][i].obrazek) - odleglosc[i] / 80);
                    Canvas.SetLeft(platformy[obecne_pietro][i].tekst, Canvas.GetLeft(platformy[obecne_pietro][i].tekst) - odleglosc[i] / 80);
                    Canvas.SetTop(platformy[obecne_pietro][i].obrazek, winda_pozycja + 30 - Math.Abs(Math.Sin(10 * Math.PI * wyjscie_z_windy / 400)) * 20);
                    Canvas.SetTop(platformy[obecne_pietro][i].tekst, winda_pozycja + 50 - Math.Abs(Math.Sin(10 * Math.PI * wyjscie_z_windy / 400)) * 20);
                }
            }
            if (wyjscie_z_windy == 80 || Przejscie_do_windy.Count == 0)
            {
                winda.Update_winda();
                Waga.Content = "Ciężar windy: " + Convert.ToString((8 - winda.miejsce) * 70);
                wyjscie_z_windy = 0;
                for (int i = Przejscie_do_windy.Count - 1; 0 <= i; i--)
                {
                    platformy[obecne_pietro][Przejscie_do_windy[i][0]].obrazek.Source = null;
                    platformy[obecne_pietro][Przejscie_do_windy[i][0]].tekst.Content = null;
                    platformy[obecne_pietro].RemoveAt(Przejscie_do_windy[i][0]);
                }
                zegar_3.Stop();
                zegar_7.Start();
            }
        }
        private void Tyk_zegar_7(object sender, EventArgs e) //Animacja zamykania drzwi
        {
            if (animacja_drzwi == 0)
            {
                if (Koniec_programu())
                {
                    Znajdz_nastepne_pietro();
                    zegar_4.Start();
                }
                else
                {
                    zegar_1_start.Start();
                    zegar_5.Start();
                }
                zegar_7.Stop();
            }
            else
            {
                if (obecne_pietro % 2 == 0)
                {
                    animacja_drzwi--;
                    Drzwi_1.Height--;
                    Canvas.SetTop(Drzwi_1, Canvas.GetTop(Drzwi_1) + 1);
                }
                else
                {
                    animacja_drzwi--;
                    Drzwi_2.Height--;
                    Canvas.SetTop(Drzwi_2, Canvas.GetTop(Drzwi_2) + 1);
                }
            }
        }
        private void Animacja_ruchu_windy()
        {
            if (660 - (obecne_pietro * 100) < winda_pozycja)
            {
                winda_pozycja--;
            }
            else if (660 - (obecne_pietro * 100) > winda_pozycja)
            {
                winda_pozycja++;
            }
            Canvas.SetTop(Komora, winda_pozycja);
            Canvas.SetTop(Pustka, winda_pozycja + 10);
            Canvas.SetTop(Drzwi_1, winda_pozycja + 140);
            Canvas.SetTop(Drzwi_2, winda_pozycja + 140);
            for (int i = 0; 8 > i; i++)
            {
                Canvas.SetTop(winda.pasazerowie[i].obrazek, winda_pozycja + 30);
                Canvas.SetTop(winda.pasazerowie[i].tekst, winda_pozycja + 50);
            }
        }
        private void Tyk_zegar_4(object sender, EventArgs e) //Animacja na nastepne pietro
        {
            Animacja_ruchu_windy();
            if (winda_pozycja == 660 - (obecne_pietro * 100))
            {
                zegar_4.Stop();
                zegar_2.Start();
            }
        }
        private void Tyk_zegar_5(object sender, EventArgs e) //Animacja powrotu na parter po 5 sekundach
        {
            stoper_powrotu_windy++;
            if (stoper_powrotu_windy >= 80)
            {
                zegar_1_start.Stop();
                obecne_pietro = 0;
                winda.pietro_windy = 0;
                winda.kierunek = true;
                Animacja_ruchu_windy();
                if (winda_pozycja == 660 - (obecne_pietro * 100))
                {
                    stoper_powrotu_windy = 0;
                    zegar_1_start.Start();
                    zegar_5.Stop();
                }
            }
        }

        private void Button_5_1_Click(object sender, RoutedEventArgs e)
        {
            platformy[4].Add(new Osoba(0, platformy[4].Count, 4, Okno));
        }

        private void Button_5_2_Click(object sender, RoutedEventArgs e)
        {
            platformy[4].Add(new Osoba(1, platformy[4].Count, 4, Okno));
        }

        private void Button_5_3_Click(object sender, RoutedEventArgs e)
        {
            platformy[4].Add(new Osoba(2, platformy[4].Count, 4, Okno));
        }

        private void Button_5_4_Click(object sender, RoutedEventArgs e)
        {
            platformy[4].Add(new Osoba(3, platformy[4].Count, 4, Okno));
        }

        private void Button_4_1_Click(object sender, RoutedEventArgs e)
        {
            platformy[3].Add(new Osoba(0, platformy[3].Count, 3, Okno));
        }

        private void Button_4_2_Click(object sender, RoutedEventArgs e)
        {
            platformy[3].Add(new Osoba(1, platformy[3].Count, 3, Okno));
        }

        private void Button_4_3_Click(object sender, RoutedEventArgs e)
        {
            platformy[3].Add(new Osoba(2, platformy[3].Count, 3, Okno));
        }

        private void Button_4_5_Click(object sender, RoutedEventArgs e)
        {
            platformy[3].Add(new Osoba(4, platformy[3].Count, 3, Okno));
        }

        private void Button_3_1_Click(object sender, RoutedEventArgs e)
        {
            platformy[2].Add(new Osoba(0, platformy[2].Count, 2, Okno));
        }

        private void Button_3_2_Click(object sender, RoutedEventArgs e)
        {
            platformy[2].Add(new Osoba(1, platformy[2].Count, 2, Okno));
        }

        private void Button_3_4_Click(object sender, RoutedEventArgs e)
        {
            platformy[2].Add(new Osoba(3, platformy[2].Count, 2, Okno));
        }

        private void Button_3_5_Click(object sender, RoutedEventArgs e)
        {
            platformy[2].Add(new Osoba(4, platformy[2].Count, 2, Okno));
        }

        private void Button_2_1_Click(object sender, RoutedEventArgs e)
        {
            platformy[1].Add(new Osoba(0, platformy[1].Count, 1, Okno));
        }

        private void Button_2_3_Click(object sender, RoutedEventArgs e)
        {
            platformy[1].Add(new Osoba(2, platformy[1].Count, 1, Okno));
        }

        private void Button_2_4_Click(object sender, RoutedEventArgs e)
        {
            platformy[1].Add(new Osoba(3, platformy[1].Count, 1, Okno));
        }

        private void Button_2_5_Click(object sender, RoutedEventArgs e)
        {
            platformy[1].Add(new Osoba(4, platformy[1].Count, 1, Okno));
        }

        private void Button_1_2_Click(object sender, RoutedEventArgs e)
        {
            platformy[0].Add(new Osoba(1, platformy[0].Count, 0, Okno));
        }

        private void Button_1_3_Click(object sender, RoutedEventArgs e)
        {
            platformy[0].Add(new Osoba(2, platformy[0].Count, 0, Okno));
        }

        private void Button_1_4_Click(object sender, RoutedEventArgs e)
        {
            platformy[0].Add(new Osoba(3, platformy[0].Count, 0, Okno));
        }

        private void Button_1_5_Click(object sender, RoutedEventArgs e)
        {
            platformy[0].Add(new Osoba(4, platformy[0].Count, 0, Okno));
        }

    }

    public class Komora_windy
    {
        public Osoba[] pasazerowie = new Osoba[8]; //ilosc miejsca w windzie
        public bool kierunek; //true kierunek w gore, false kierunek w dol
        public int pietro_windy; //pietro na ktorej znajduje sie teraz winda
        public int miejsce; //ilosc miejsca pozostala w windzie
        public int pietro_docelowe; //pietro docelowe dla najdalszego pasazera
        public Komora_windy(Canvas Okno)
        {
            for (int i = 0; 8 > i; i++)
            {
                pasazerowie[i] = new Osoba(-1, i, 10 + pietro_windy, Okno); // -1 = miejsce w windzie puste
            }
            kierunek = true;
            pietro_windy = 0;
            miejsce = 8;
            pietro_docelowe = 0;
        }
        public bool Oproznij_winde() // oproznia pasazerow na ich pietro
        {
            int miejsce_pierwotne = miejsce;
            for (int i = 0; 8 > i; i++)
            {
                if (pasazerowie[i].wartosc == pietro_windy)
                {
                    pasazerowie[i].wartosc = -1;
                    miejsce++;
                }
            }
            return miejsce_pierwotne != miejsce;
        }
        public List<List<int>> Wsiadz_do_windy(List<Osoba> pietro)
        {
            int j = 0;
            int miejsce_poczatkowe = miejsce;
            List<List<int>> Przejscie_do_windy = new List<List<int>>();
            for (int i = 0; pietro.Count > i && miejsce != 0; i++)
            {
                if ((kierunek && pietro[i].wartosc <= pietro_windy) || (kierunek == false && pietro[i].wartosc >= pietro_windy)) { } //sprawdza czy osobie z pietra oplaca sie wchodzic do windy
                else if (pasazerowie[j].wartosc == -1)
                {
                    if (kierunek && pietro[i].wartosc > pietro_docelowe)
                    {
                        pietro_docelowe = pietro[i].wartosc;  //sprawdza pasazera co chce na najwyzszes pietro jak winda
                    }
                    else if (kierunek == false && pietro[i].wartosc < pietro_docelowe)
                    {
                        pietro_docelowe = pietro[i].wartosc; //sprawdza pasazera co chce na najnizsze pietro jak winda jedzie do dolu
                    }

                    Przejscie_do_windy.Add(new List<int>());
                    Przejscie_do_windy[^1].Add(i);
                    Przejscie_do_windy[^1].Add(j);
                    pasazerowie[j].wartosc = pietro[i].wartosc;
                    miejsce--;
                }
                else
                {
                    j++;
                    i--;
                }
            }
            return Przejscie_do_windy;
        }
        public void Update_winda()
        {
            for (int i = 0; 8 > i; i++)
            {
                Canvas.SetTop(pasazerowie[i].obrazek, 690 - (100 * pietro_windy));
                Canvas.SetTop(pasazerowie[i].tekst, 710 - (100 * pietro_windy));
                pasazerowie[i].Update_Osoba(i, 10);
            }
        }
    }
    public class Osoba
    {
        public Image obrazek;
        public Label tekst;
        public int wartosc;
        public Osoba(int numer, int x, int y, Canvas Okno)
        {
            wartosc = numer;

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("osoba.png", UriKind.Relative);
            bi.EndInit();

            obrazek = new Image
            {
                Source = bi,
                Name = "Osoba_" + x.ToString() + "_" + y.ToString(),
                Width = 100,
                Height = 120,
                Visibility = wartosc != -1 ? Visibility.Visible : Visibility.Hidden,
                Stretch = Stretch.Fill
            };
            Panel.SetZIndex(obrazek, 1);

            tekst = new Label
            {
                Name = "Tekst_" + x.ToString() + "_" + y.ToString(),
                Content = wartosc != -1 ? (wartosc + 1).ToString() : " ",
                Width = 50,
                Height = 50,
                FontSize = 35,
                Foreground = new SolidColorBrush(Colors.Red)
            };
            Panel.SetZIndex(tekst, 1);

            if (y == 10)
            {
                y -= 10;
                Canvas.SetLeft(obrazek, 785 + (35 * x));
                Canvas.SetLeft(tekst, 820 + (35 * x));
            }
            else if (y % 2 == 0)
            {
                Canvas.SetLeft(obrazek, 700 + (-60 * x));
                Canvas.SetLeft(tekst, 735 + (-60 * x));
            }
            else
            {
                Canvas.SetLeft(obrazek, 1100 + (60 * x));
                Canvas.SetLeft(tekst, 1135 + (60 * x));
            }

            Canvas.SetTop(obrazek, 690 - (100 * y));
            Canvas.SetTop(tekst, 710 - (100 * y));

            Okno.Children.Add(obrazek);
            Okno.Children.Add(tekst);

        }
        public void Update_Osoba(int x, int y)
        {
            if (y >= 10)
            {
                obrazek.Visibility = wartosc != -1 ? Visibility.Visible : Visibility.Hidden;
                tekst.Content = wartosc != -1 ? (wartosc + 1).ToString() : " ";
                Canvas.SetLeft(obrazek, 785 + (35 * x));
                Canvas.SetLeft(tekst, 820 + (35 * x));
            }
            else if (y % 2 == 0)
            {
                Canvas.SetLeft(obrazek, 700 + (-60 * x));
                Canvas.SetLeft(tekst, 735 + (-60 * x));
            }
            else
            {
                Canvas.SetLeft(obrazek, 1100 + (60 * x));
                Canvas.SetLeft(tekst, 1135 + (60 * x));
            }
        }
    }
}

