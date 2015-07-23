using SilverBotAndGuy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        byte[,] GetBytes (out int width, out int height)
        {
            string[] split = textBox1.Text.Replace("\r", "").Split('\n');
            width = split.OrderByDescending(o=>o.Length).ToArray()[0].Length;
            height = split.Length;
            byte[,] blocks = new byte[width, height];
            int y = (int)(0);
            int x = (int)(0);
            foreach (char c in textBox1.Text)
            {
                switch (c)
                {
                    case 'c':
                        {
                            blocks[x, y] = (byte)Block.Crate;
                            break;
                        }
                    case ' ' : case 'f':
                        {
                            blocks[x, y] = (byte)Block.Floor;
                            break;
                        }
                    case 'b':
                        {
                            blocks[x, y] = (byte)Block.Bomb;
                            break;
                        }
                    case '\n':
                        {
                            x = -1;
                            y++;
                            break;
                        }
                    case '\r':
                        break;
                    default:
                        {
                            MessageBox.Show("Invalid element: " + c);
                            return null;
                        }
                }
                x++;
            }
            return blocks;
        }

        private void saveFile_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Stream fileStream = saveFileDialog1.OpenFile();
                if (fileStream != null)
                {
                    BinaryWriter writer = new BinaryWriter(fileStream);
                    int width;
                    int height;
                    byte[,] bytes = GetBytes(out width, out height);
                    if (bytes == null)
                        return;
                    byte[] resultBytes = new byte[bytes.Length + 8];
                    Buffer.BlockCopy(bytes, 0, resultBytes, 8, bytes.Length);
                    writer.Write(width);
                    writer.Write(height);
                    writer.Write(resultBytes, 0, resultBytes.Length);
                    writer.Flush();
                    writer.Close();
                }
            }
        }
    }
}
