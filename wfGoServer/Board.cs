using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Media;

namespace wfGoServer
{
    abstract class Board
    {
        protected int linenum;
        protected int size;
        protected List<Matrix> matrixlist;
        protected int number;//目前是第几手（第0手表示双方均未落子）
        protected int cellSize;

        protected Bitmap bmp;
        private SoundPlayer soundplayer;
        //###############

        private void DrawBoard(Graphics gr)
        {
            //背景
            Image img = Image.FromFile("Image/背景.jpg");
            gr.DrawImage(img, 0, 0);

            //横线
            for (int i = 1; i <= linenum; i++)
            {
                Point start = new Point(cellSize, i * cellSize);
                Point end = new Point(linenum * cellSize, i * cellSize);
                gr.DrawLine(Pens.Black, start, end);
            }
            //竖线
            for (int i = 1; i <= linenum; i++)
            {
                Point start = new Point(i * cellSize, cellSize);
                Point end = new Point(i * cellSize, linenum * cellSize);
                gr.DrawLine(Pens.Black, start, end);
            }
            //横坐标
            for (int i = 1; i <= linenum; i++)
            {
                string drawstr = i.ToString();
                Font drawfont = new Font("宋体", 12);
                SolidBrush drawbrush = new SolidBrush(Color.Black);
                PointF drawpointf = new PointF((float)(i * cellSize) - 10, 0);
                gr.DrawString(drawstr, drawfont, drawbrush, drawpointf);
            }
            //纵坐标
            for (int i = 1; i <= linenum; i++)
            {
                string drawstr = i.ToString();
                Font drawfont = new Font("宋体", 12);
                SolidBrush drawbrush = new SolidBrush(Color.Black);
                PointF drawpointf = new PointF(0, (float)(i * cellSize) - 10);
                gr.DrawString(drawstr, drawfont, drawbrush, drawpointf);
            }
            //9个小圆点
            for (int i = 4; i <= 16; i += 6)
            {
                for (int j = 4; j <= 16; j += 6)
                {
                    int X = i * cellSize;
                    int Y = j * cellSize;

                    int width = cellSize / 4;
                    int height = width;

                    int circleX = X - width / 2;
                    int circleY = Y - height / 2;

                    gr.FillEllipse(Brushes.Black, circleX, circleY, width, height);


                }
            }
        }

        private void DrawPieces(Graphics gr)
        {
            int n = linenum + 1;
            for(int i=1;i<n;i++)
            {
                for(int j=1;j<n;j++)
                {
                    if(matrixlist[number][i,j]!=0)
                    {
                        bool isblack = true;
                        if(matrixlist[number][i,j]<0)
                        {
                            isblack = false;
                        }
                        DrawPiece(gr, isblack, i, j);
                    }
                }
            }
        }

        private void DrawPiece(Graphics gr,bool isblack,int x,int y)
        {
            int r = ConstNumber.r;
            Color color = isblack == true ? Color.Black : Color.White;
            Brush mybrush = new SolidBrush(color);

            int rectX = x * 2 * r - r;
            int rectY = y * 2 * r - r;

            gr.FillEllipse(mybrush, rectX, rectY, 2 * r, 2 * r);
            gr.DrawEllipse(Pens.Black, rectX, rectY, 2 * r, 2 * r);
        }

        private bool SetPiece(bool isblack, int x, int y)
        {
            //合法判断
            if (matrixlist[number][x, y] != 0)
            {
                return false;
            }

            //
            Matrix m = matrixlist[number].Copy();
            number = number + 1;
            int flag = 1;
            if (isblack == false)
            {
                flag = -1;
            }
            m[x, y] = flag * number;
            matrixlist.Add(m);

            soundplayer.SoundLocation = "Sound/落子.wav";
            soundplayer.Play();

            return true;

        }

        //################

        public Board()
        {
            size = ConstNumber.BoardSize;
            linenum = ConstNumber.linenum;
            cellSize = 2 * ConstNumber.r;

            number = 0;
            matrixlist = new List<Matrix>();


            Matrix m = new Matrix();
            matrixlist.Add(m);

            bmp = new Bitmap(size, size);
            soundplayer = new SoundPlayer();
        }

        public virtual void Draw(Graphics gr)
        {
            gr.DrawImage(bmp, 0, 0);
        }

        public void DrawToBmp()
        {
            Graphics grbmp = Graphics.FromImage(bmp);
            DrawBoard(grbmp);
            DrawPieces(grbmp);
        }

        public bool SetBlackPiece(int x, int y)
        {
            return SetPiece(true, x, y);
        }

        public bool SetWhitePiece(int x, int y)
        {
            return SetPiece(false, x, y);
        }

        public bool RemovePiece(int x, int y)
        {
            //该位置没有棋子
            if (matrixlist[number][x, y] == 0)
            {
                return false;
            }
            //有棋子，移除它
            int num = matrixlist[number][x, y];
            matrixlist[number][x, y] = 0;

            return false;

        }

        public virtual void Regret()
        {

        }

    }

}
