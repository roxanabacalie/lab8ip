/**************************************************************************
 *                                                                        *
 *  File:        MainForm.cs                                              *
 *  Copyright:   (c) 2008-2013, Florin Leon                               *
 *  E-mail:      florin.leon@tuiasi.ro                                    *
 *  Website:     http://florinleon.byethost24.com/lab_ip.htm              *
 *  Description: Monster game to optimize using the Prototype             *
 *               Design Pattern (Software Engineering lab 8)              *
 *                                                                        *
 *                                                                        *
 *  This code and information is provided "as is" without warranty of     *
 *  any kind, either expressed or implied, including but not limited      *
 *  to the implied warranties of merchantability or fitness for a         *
 *  particular purpose. You are free to use this source code in your      *
 *  applications as long as the original copyright notice is included.    *
 *                                                                        *
 **************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Monster
{
    public partial class MainForm : Form
    {
        private List<Monster> _listMonsters;
        private MonsterSprite _mainMonster;
        private Random _rand = new Random();
        private int _monsterType = 0;
        private int _x, _y;
        private long _timeStart, _timeFinish;
        private const int MonsterSize = 200;
        private const int MaxLevels = 4;
        private bool _gameOver = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // incarca
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AllMonsters));
                FileStream fs = new FileStream("settings.xml", FileMode.Open);
                XmlReader reader = new XmlTextReader(fs);
                AllMonsters ab = (AllMonsters)serializer.Deserialize(reader);
                reader.Close(); fs.Close();
                serializer = null;

                _listMonsters = new List<Monster>();

                for (int i = 0; i < ab.Monsters.Length; i++)
                    _listMonsters.Add(ab.Monsters[i]);
            }
            catch
            {
                MessageBox.Show("Nu s-a putut incarca settings.xml");
                return;
            }

            if (_listMonsters == null || _listMonsters.Count == 0)
            {
                MessageBox.Show("Fisier de configurare invalid: settings.xml");
                _listMonsters = null;
                return;
            }
        }

        private void startNewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // start
            if (_listMonsters == null || _listMonsters.Count == 0)
                loadSettingsToolStripMenuItem.PerformClick();
            if (_listMonsters == null)
                return;

            _monsterType = 0;
            _gameOver = false;

            try
            {
                _mainMonster = new MonsterSprite(_listMonsters[_monsterType]);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                _mainMonster = null;
                return;
            }

            _timeStart = DateTime.Now.Ticks;
            timer.Start();

            Redraw();
        }

        private void Redraw()
        {
            try
            {
                _x = _rand.Next(pictureBox.Width - MonsterSize + 20);
                _y = _rand.Next(pictureBox.Height - MonsterSize - 10);
                pictureBox.Refresh();
            }
            catch
            {
                MessageBox.Show("Fereastra este prea mica");
                _mainMonster = null;
                return;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // about

            const string copyright =
               "Sablonul de proiectare Prototip\r\n" +
               "Ingineria programarii, Laboratorul 8\r\n" +
               "(c)2008 Florin Leon\r\n" +
               "http://florinleon.byethost24.com/lab_ip.htm";

            MessageBox.Show(copyright, "Despre Monstri");
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (_mainMonster == null)
            {
                timer.Stop();
                e.Graphics.Clear(Color.White);
                if (_gameOver)
                {
                    e.Graphics.DrawString("Jocul s-a terminat!", new Font("Arial", 48), Brushes.Red, 10, 10);
                    long dt = _timeFinish - _timeStart;
                    double ms = dt / 10000000.0;
                    e.Graphics.DrawString(ms.ToString("F3") + " s", new Font("Arial", 48), Brushes.Red, 10, 80);
                }
                return;
            }

            _mainMonster.Draw(e.Graphics, _x, _y);
        }

        private void ShootMonster()
        {
            if (_mainMonster.Shoot())
            {
                _monsterType++;
                if (_monsterType < MaxLevels)
                {
                    try
                    {
                        _mainMonster = new MonsterSprite(_listMonsters[_monsterType]);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                        _mainMonster = null;
                        return;
                    }

                    Redraw();
                }
                else
                {
                    _mainMonster = null;
                    _gameOver = true;
                    _timeFinish = DateTime.Now.Ticks;
                    pictureBox.Refresh();
                }
            }
            else
                Redraw();
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (_mainMonster != null)
            {
                if (e.X > _x && e.X < _x + MonsterSize && e.Y > _y && e.Y < _y + MonsterSize)
                    ShootMonster();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Graphics g = pictureBox.CreateGraphics();
            long dt = DateTime.Now.Ticks - _timeStart;
            double ms = dt / 10000000.0;
            g.FillRectangle(Brushes.White, 1, 1, 100, 20);
            g.DrawString(ms.ToString("F3") + " s", new Font("Arial", 10), Brushes.Black, 1, 1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
    }
}