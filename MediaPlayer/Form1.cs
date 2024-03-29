﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using System.IO;

namespace MediaPlayer
{
    public partial class Form1 : Form
    {
        public WMPLib.WindowsMediaPlayer Player;
        public ListViewItem currentlyPlaying;
        public Form1()
        {
            InitializeComponent();
            Player = new WMPLib.WindowsMediaPlayer();
            reloadList();
        }
        private void btnPrev_Click(object sender, EventArgs e)
        {
            int i = songLibrary.Items.IndexOf(currentlyPlaying);

            if (i == 0)
            {
                Player.URL = songLibrary.Items[songLibrary.Items.Count-1].Text;
                currentlyPlaying = songLibrary.Items[songLibrary.Items.Count - 1];
            }
            else
            {
                i--;
                Player.URL = songLibrary.Items[i].Text;
                Player.controls.play();
                currentlyPlaying = songLibrary.Items[i];
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (Player.playState == WMPPlayState.wmppsPaused)
            {
                Player.controls.play();
                return;
            }
            
            if (songLibrary.SelectedItems.Count > 0)
            {
                Player.URL = songLibrary.SelectedItems[0].Text;
                currentlyPlaying = songLibrary.SelectedItems[0];
            }
            else
            {
                Player.URL = songLibrary.Items[0].Text;
                currentlyPlaying = songLibrary.Items[0];
            }

            Player.controls.play();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (Player.playState == WMPPlayState.wmppsPaused)
            {
                Player.controls.play();
            }
            else
            {
                Player.controls.pause();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Player.controls.stop();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int i = songLibrary.Items.IndexOf(currentlyPlaying);

            if (i == songLibrary.Items.Count - 1)
            {
                Player.URL = songLibrary.Items[0].Text;
                currentlyPlaying = songLibrary.Items[0];
            }
            else
            {
                i++;
                Player.URL = songLibrary.Items[i].Text;
                Player.controls.play();
                currentlyPlaying = songLibrary.Items[i];
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
                foreach (string file in files)
                {
                    if (File.Exists(file) && file.EndsWith(".mp3"))
                        SQLManager.getInstance().Insert(file);
                }

                reloadList();
            }

        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SQLManager.getInstance().CloseDB();
        }

        public void reloadList()
        {
            songLibrary.Items.Clear();
            List<Song> SongList = SQLManager.getInstance().getSongs();

            for (int i = 0; i < SongList.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = SongList[i].File;
                item.SubItems.Add(SongList[i].Title);
                item.SubItems.Add(SongList[i].Artist);
                item.SubItems.Add(SongList[i].Album);
                item.SubItems.Add(SongList[i].Year.ToString());
                item.SubItems.Add(SongList[i].Comment);
                item.SubItems.Add(SongList[i].Genre);

                songLibrary.Items.Add(item);
            }
        }

        private void deleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < songLibrary.SelectedItems.Count; i++)
            {
                SQLManager.getInstance().deleteFile(songLibrary.SelectedItems[i].Text);
            }

            reloadList();
        }
        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void playASongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openSongs = new OpenFileDialog();
            openSongs.Multiselect = false;
            openSongs.Filter = "*.mp3|*.mp3";

            if (openSongs.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Player.URL = openSongs.FileNames[0].ToString();
                Player.controls.play();
            }
        }

        private void deleteSelectedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in songLibrary.SelectedItems)
            {
                songLibrary.Items.Remove(item);
                SQLManager.getInstance().deleteFile(item.Text);
            }
        }

        private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openSongs = new OpenFileDialog();
            openSongs.Multiselect = true;
            openSongs.Filter = "*.mp3|*.mp3";

            if (openSongs.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string file in openSongs.FileNames)
                {
                    SQLManager.getInstance().Insert(file);
                }

                reloadList();
            }
        }

        
    }
}
