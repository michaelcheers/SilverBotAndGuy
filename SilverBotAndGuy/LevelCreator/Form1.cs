using SilverBotAndGuy;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LevelCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        byte[,] GetBytes (out uint width, out uint height, out uint startPosX, out uint startPosY)
        {
            startPosX = 1;
            startPosY = 1;
            string[] split = textBox1.Text.Replace("\r", "").Split('\n');
            width = (uint)(split.OrderByDescending(o=>o.Length).ToArray()[0].Length);
            height = (uint)(split.Length);
            byte[,] blocks = new byte[width, height];
            int y = (int)(0);
            int x = (int)(0);
            foreach (char c in textBox1.Text)
            {
                switch (c)
                {
                    case 's':
                        {
                            startPosX = (uint)x;
                            startPosY = (uint)y;
                            blocks[x, y] = (byte)Block.Floor;
                            break;
                        }
                    case 'r':
                        {
                            blocks[x, y] = (byte)Block.LaserGunRight;
                            break;
                        }
                    case 'l':
                        {
                            blocks[x, y] = (byte)Block.LaserGunLeft;
                            break;
                        }
                    case 'u':
                        {
                            blocks[x, y] = (byte)Block.LaserGunUp;
                            break;
                        }
                    case 'd':
                        {
                            blocks[x, y] = (byte)Block.LaserGunDown;
                            break;
                        }
                    case 'w':
                        {
                            blocks[x, y] = (byte)Block.Wall;
                            break;
                        }
                    case '#':
                        {
                            blocks[x, y] = (byte)Block.LaserProofWall;
                            break;
                        }
                    case 'p':
                        {
                            blocks[x, y] = (byte)Block.Panel;
                            break;
                        }
                    case 'c':
                        {
                            blocks[x, y] = (byte)Block.Crate;
                            break;
                        }
                    case ' ' : case 'f': case '/': case '\\':
                        {
                            blocks[x, y] = (byte)Block.Floor;
                            break;
                        }
                    case 'x':
                        {
                            blocks[x, y] = (byte)Block.Exit;
                            break;
                        }
                    case 'b':
                        {
                            blocks[x, y] = (byte)Block.Bomb;
                            break;
                        }
                    case 'i':
                        {
                            blocks[x, y] = (byte)Block.Ice;
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
        
        System.Version version = SilverBotAndGuy.Version.Current;

        void saveFileFunc (BinaryWriter writer)
        {

            uint width;
            uint height;
            uint startDozerBotX;
            uint startDozerBotY;
            byte[,] bytes = GetBytes(out width, out height, out startDozerBotX, out startDozerBotY);
            if (bytes == null)
                return;
            byte[] resultBytes = new byte[bytes.Length];
            Buffer.BlockCopy(bytes, 0, resultBytes, 0, resultBytes.Length);
            bytes = null;
            writer.Write(SilverBotAndGuy.Version.Current);
            writer.Write(width);
            writer.Write(height);
            bool silverBot = textBox1.Text.Contains('/');
            writer.Write(silverBot);
            if (silverBot)
            {
                string[] split = textBox1.Text.Replace("\r", "").Split('\n');
                int posOfSilverBotX = 0;
                int posOfSilverBotY = 0;
                while (posOfSilverBotY < height)
                {
                    int indexOf = split[posOfSilverBotY].IndexOf('/');
                    if (indexOf != -1)
                    {
                        posOfSilverBotX = indexOf;
                        break;
                    }
                    posOfSilverBotY++;
                }
                writer.Write(posOfSilverBotX);
                writer.Write(posOfSilverBotY);
            }
            writer.Write(startDozerBotX);
            writer.Write(startDozerBotY);
            writer.Write(resultBytes, 0, resultBytes.Length);
            writer.Flush();
            writer.Close();
        }

        private void saveFile_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream fileStream = saveFileDialog1.OpenFile();
                if (fileStream != null)
                {
                    BinaryWriter writer = new BinaryWriter(fileStream);
                    saveFileFunc(writer);
                }
            }
        }

        char ParseBlock (Block block)
        {
            char currentChar;
            switch (block)
            {
                case (Block)0xFF:
                    {
                        currentChar = 's';
                        break;
                    }
                case (Block)0xFE:
                    {
                        currentChar = '/';
                        break;
                    }
                case (Block)0xFD:
                    {
                        currentChar = '\\';
                        break;
                    }
                case Block.Ice:
                    {
                        currentChar = 'i';
                        break;
                    }
                case Block.Exit:
                    {
                        currentChar = 'x';
                        break;
                    }
                case Block.Floor:
                    {
                        currentChar = ' ';
                        break;
                    }
                case Block.Wall:
                    {
                        currentChar = 'w';
                        break;
                    }
                case Block.LaserProofWall:
                    {
                        currentChar = '#';
                        break;
                    }
                case Block.LaserGunRight:
                    {
                        currentChar = 'r';
                        break;
                    }
                case Block.LaserGunDown:
                    {
                        currentChar = 'd';
                        break;
                    }
                case Block.LaserGunLeft:
                    {
                        currentChar = 'l';
                        break;
                    }
                case Block.LaserGunUp:
                    {
                        currentChar = 'u';
                        break;
                    }
                case Block.Bomb:
                    {
                        currentChar = 'b';
                        break;
                    }
                case Block.Panel:
                    {
                        currentChar = 'p';
                        break;
                    }
                case Block.Crate:
                    {
                        currentChar = 'c';
                        break;
                    }
                default:
                    throw new ArgumentException("No possible Block");
            }
            return currentChar;
        }

        void LoadBlocks (Block[,] blocks, uint startX, uint startY, bool silverBot, uint startSilverBotX, uint startSilverBotY)
        {
            if (silverBot)
            {
                blocks[startSilverBotX, startSilverBotY] = (Block)0xFE;
                blocks[startSilverBotX + 1, startSilverBotY] = (Block)0xFD;
                blocks[startSilverBotX, startSilverBotY + 1] = (Block)0xFD;
                blocks[startSilverBotX + 1, startSilverBotY + 1] = (Block)0xFE;
            }
            blocks[startX, startY] = (Block)0xFF;
            textBox1.Text = "";
            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                for (int x = 0; x < blocks.GetLength(0); x++)
                {
                    Block current = blocks[x, y];
                    char currentChar = ParseBlock(current);
                    textBox1.Text += currentChar;
                }
                textBox1.Text += "\r\n";
            }
        }

        private void Load_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream fileStream = openFileDialog1.OpenFile();
                if (fileStream != null)
                {
                    uint StartX;
                    uint StartY;
                    bool isSilverBot;
                    uint StartSilverBotX;
                    uint StartSilverBotY;
                    LoadBlocks(FileLoader.ReadFile(fileStream, out version, out StartX, out StartY, out isSilverBot, out StartSilverBotX, out StartSilverBotY), StartX, StartY, isSilverBot, StartSilverBotX, StartSilverBotY);
                }
            }
        }

        private void tryButton_Click(object sender, EventArgs e)
        {
            string tmpPath = Path.GetTempFileName();
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(tmpPath));
            saveFileFunc(writer);
            ProcessStartInfo info = new ProcessStartInfo(Path.GetFullPath("../../../SilverBotAndGuy/bin/Debug/SilverBotAndGuy.exe"), tmpPath);
            info.WorkingDirectory = Path.GetFullPath("../../../SilverBotAndGuy/bin/Debug/");
            Process.Start(info);
        }
    }
}
