
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;
using Tao.DevIl;    

namespace kg_4
{
    public partial class Form1 : Form
    {
        // ряд вспомогательных переменных 
        private bool textureIsLoad = false;// флаг - загружена ли текстура      
        // имя текстуры
        public string texture_name = "";
        public int imageId = 0;// идентификатор текстуры        
        public uint mGlTextureObject = 0;// текстурный объект

        float McoordX = 0, McoordY = 0;
        float MlastX = 0, MlastY = 0;
        bool mouseButton = false;
        float rotateX;
        float rotateY;

        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            const float ZNEAR = 0.5f;
            const float ZFAR = 10;
            const float FIELD_OF_VIEW = 45;
            float aspect = (float)AnT.Width / AnT.Height;

            //инициализация библиотеки glut
            Glut.glutInit();
            // инициализация режима экрана
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE);
            //инициализация библиотеки openIL
            Il.ilInit();
            Il.ilEnable(Il.IL_ORIGIN_SET);
            Gl.glClearColor(255, 255, 255, 1);// установка цвета очистки экрана             
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);//очистка буфера цвета и буфера глубины

            Gl.glMatrixMode(Gl.GL_PROJECTION);// активация проекционной матрицы            
            Gl.glLoadIdentity();//очистка матрицы
            Glu.gluPerspective(FIELD_OF_VIEW, aspect, ZNEAR, ZFAR);//настройка области отображения
            //настройка модели
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(
                0, 0, 3, //положение глаз наблюдателя
                0, 0, 0, //точка, в которую направлена камера
                0, 1, 0); //вектор, служащий для определения вектора "вверх"

            //toolStripComboBox1.SelectedIndex = 1;
            DrawFigure();
            RenderTimer.Start();
        }

        private void DrawFigure()
        {
            int faceCount1 = 6; // количество граней
            int faceCount2 = 8;  //количество граней      
            float ScaleKof = 0.4f; //коэффициент масшатбирования


            //массив координат вершин
            float[][] vertices;
            vertices = new float[24][]
            {
                new float [] {-1.5f, -0.5f, 0}, //0
                new float [] {-1.5f, 0.5f, 0}, //1
                new float []  {-1, -1, -0.707107f}, //2
                new float [] {-1, -1, 0.707107f}, //3
                new float [] {-1, 1, -0.707107f}, //4
                new float [] {-1, 1, 0.707107f}, //5
                new float [] {-0.5f, -1.5f, 0}, //6
                new float []   {-0.5f, -0.5f, -1.41421f}, //7
                new float [] {-0.5f, -0.5f, 1.41421f}, //8
                new float []  {-0.5f, 0.5f, -1.41421f}, //9
                new float [] {-0.5f, 0.5f, 1.41421f}, //10
                new float [] {-0.5f, 1.5f, 0}, //11
                new float []  {0.5f, -1.5f, 0},//12
                new float [] {0.5f, -0.5f, -1.41421f},//13
                new float [] {0.5f, -0.5f, 1.41421f},//14
                new float [] {0.5f, 0.5f, -1.41421f},//15
                new float [] {0.5f, 0.5f, 1.41421f},//16
                new float [] {0.5f, 1.5f, 0},//17
                new float [] {1, -1, -0.707107f},//18
                new float [] {1, -1, 0.707107f},//19
                new float []  {1, 1, -0.707107f},//20
                new float []{1, 1, 0.707107f}, //21
                new float [] {1.5f, -0.5f, 0},//22
                new float [] {1.5f, 0.5f, 0}//23
            };

            //массив координат граней
            int[,] faces1;
            faces1 = new int[,] // геометрия фигуры
            {
                {16, 10, 8, 14}, //0
                {13, 7, 9, 15}, //1
                {21, 23, 20, 17}, //2
                {11, 4, 1, 5}, //3
                {12, 18, 22, 19}, //4
                {3, 0, 2, 6}, //5             
            };
            int[,] faces2;
            faces2 = new int[,]
            {
                {18, 12, 6, 2, 7, 13}, //6
                {14, 8, 3, 6, 12, 19}, //7
                {15, 9, 4, 11, 17, 20}, //8
                {21, 17, 11, 5, 10, 16}, //9
                {19, 22, 23, 21, 16, 14}, //10
                {13, 15, 20, 23, 22, 18}, //11
                {8, 10, 5, 1, 0, 3},//12
                {2, 0, 1, 4, 9, 7},//13          
            };

            byte[][] faceColors;
            faceColors = new byte[14][]
            {
                new byte [] {128,128,128},//0
                new byte [] {127,127,70},//1
                new byte [] {0,50,127},//2
                new byte [] {50,0,127},//3
                new byte [] {50,50,200},//4
                new byte [] {75,75,75},//5
                new byte [] {150,50,150},//6
                new byte [] {50,150,50},//7
                new byte [] {127,0,0},//8
                new byte [] {0,127,0},//9
                new byte [] {0,0,127},//10
                new byte [] {127,127,0},//11
                new byte [] {0,127,127},//12
                new byte [] {127,0,127},//13
            };

            Gl.glClearColor(255, 255, 255, 1);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            if (textureIsLoad)
            {
                //включаем режим текстурирования 
                Gl.glEnable(Gl.GL_TEXTURE_2D);
                //включаем режим текстурирования, указывая индификатор mGlTextureObject 
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject);


                //сохраняем состояние матрицы 
                Gl.glPushMatrix();

                // масштабирование 
                Gl.glScalef(ScaleKof, ScaleKof, ScaleKof);
                // поворот объекта 
                Gl.glRotatef(rotateX, 1, 0, 0);
                Gl.glRotatef(rotateY, 0, 1, 0);

                //выполняем перемещение для более наглядного представления сцены 
                //Gl.glTranslated(0, 0, -6); 

                Gl.glBegin(Gl.GL_QUADS); // обход вершин 

                Gl.glTexCoord2f(0.0f, 0.0f);
                Gl.glVertex3d(0.5f, 0.5f, 1.41421f);//16 

                Gl.glTexCoord2f(1.0f, 0.0f);
                Gl.glVertex3d(-0.5f, 0.5f, 1.41421f);//10

                Gl.glTexCoord2f(1.0f, 1.0f);
                Gl.glVertex3d(-0.5f, -0.5f, 1.41421f);//8

                Gl.glTexCoord2f(0.0f, 1.0f);
                Gl.glVertex3d(0.5f, -0.5f, 1.41421f);//14

                Gl.glEnd();
                Gl.glPopMatrix();
                Gl.glDisable(Gl.GL_TEXTURE_2D);
            }
            // включение режима отбраковки невидимых граней 
            Gl.glEnable(Gl.GL_CULL_FACE);
            Gl.glCullFace(Gl.GL_BACK);
            Gl.glFrontFace(Gl.GL_CCW);
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            Gl.glPushMatrix();

            // поворот объекта 
            Gl.glRotatef(rotateX, 1, 0, 0);
            Gl.glRotatef(rotateY, 0, 1, 0);

            // масштабирование 
            Gl.glScalef(ScaleKof, ScaleKof, ScaleKof);

            // цвет очиcтки окна 
            Gl.glClearColor(255, 255, 255, 1);

            for (int face = 0; face < faceCount1; face++)
            {
                
                {
                    Gl.glColor4ubv(faceColors[face]);//Устанавливает текущий цвет из уже существующего массива цветовых значений.
                    Gl.glBegin(Gl.GL_QUADS);// обход вершин четырехугольниками
                    for (int i = 0; i < 4; i++)
                    {
                        int vertexIndex = faces1[face, i];
                        Gl.glVertex3fv(vertices[vertexIndex]);
                    }
                    Gl.glEnd();
                }
            }
            for (int face = 0; face < faceCount2; face++)
            {
                
                {
                    Gl.glColor4ubv(faceColors[face + 6]);//Устанавливает текущий цвет из уже существующего массива цветовых значений.
                    Gl.glBegin(Gl.GL_POLYGON);// обход вершин полигонами
                    for (int i = 0; i < 6; i++)
                    {
                        int vertexIndex = faces2[face, i];
                        Gl.glVertex3fv(vertices[vertexIndex]);
                    }
                    Gl.glEnd();
                }
            }

            Gl.glEnd();
            Gl.glPopMatrix();
            // завершаем рисование 
            Gl.glFlush();
            AnT.Invalidate();
        }
    

        private static uint MakeGlTexture(int Format, IntPtr pixels, int w, int h)//создание текстуры в памяти OpenGL
        {
            // идентификатор текстурного объекта 
            uint texObject;
            // генерируем текстурный объект 
            Gl.glGenTextures(1, out texObject);

            // устанавливаем режим упаковки пикселей 
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
            //выравнивание в 1 байт(1 байт на один пиксель
            // создаем привязку к только что созданной текстуре 
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texObject);

            // устанавливаем режим фильтрации и повторения текстуры 
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);
            // создаем RGB или RGBA текстуру 
            switch (Format)
            {
                case Gl.GL_RGB:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, w, h, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;
                case Gl.GL_RGBA:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;
            }
            // возвращаем идентификатор текстурного объекта 
            return texObject;
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            DrawFigure();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void AnT_MouseUp_1(object sender, MouseEventArgs e)
        {
            mouseButton = false;
            MlastX = rotateX;
            MlastY = rotateY;
        }

        private void AnT_MouseMove_1(object sender, MouseEventArgs e)
        {
            // сохраняем координаты мыши
            float dx = e.X - McoordX;
            float dy = e.Y - McoordY;
            if (mouseButton)
            {
                rotateX = (dy * 180) / AnT.Width + MlastX;
                rotateY = (dx * 180) / AnT.Height + MlastY;
                DrawFigure();
            }
        }

        private void AnT_MouseDown_1(object sender, MouseEventArgs e)
        {
            mouseButton = true;
            McoordX = e.X;
            McoordY = e.Y;
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // MessageBox будет создержать вопрос, а так же кнопки Yes No и иконку Question (Вопрос)
            DialogResult rsl = MessageBox.Show("Вы действительно хотите выйти из приложения?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            // если пользователь нажал кнопку да  
            if (rsl == DialogResult.Yes)
            {
                Application.Exit();// выходим из приложения  
            }
        }

        private void AnT_Load(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // открываем окно выбора файла 
            DialogResult res = openFileDialog1.ShowDialog();
            // если файл выбран - и возвращен результат OK 
            if (res == DialogResult.OK)
            {
                Il.ilGenImages(1, out imageId);//создаем изображение с индефикатором imageId
                Il.ilBindImage(imageId);//делаем изображение текущим
                string url = openFileDialog1.FileName;//адрес изображения
                // пробуем загрузить изображение 
                if (Il.ilLoadImage(url))
                {
                    //если загрузка произошла успешно сохраняем размеры изображения
                    int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                    int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                    int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);//определяем число бит на пиксель
                    switch (bitspp)// в зависимости от полученного результата 
                    {
                        //создаем текстуру используя режим GL_RGB или GL_RGBA
                        case 24:
                            mGlTextureObject = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                            break;
                        case 32:
                            mGlTextureObject = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                            break;
                    }
                    textureIsLoad = true;// активируем флаг, сигнализирующий загрузку текстуры 
                    Il.ilDeleteImages(1, ref imageId);// очищаем память 
                }
            }
        }
        private void fileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }
    }
}
