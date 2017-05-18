using System;
using OpenTK;
using System.Windows.Forms;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Generating.Shaders;
using Generating.GUI;
using Generating.Textures;
using Generating.SceneObjects;


/* TODO:
 * Redbook 8th edition 
 *- 1) Упростить структуру программы;
 *- 2) Загрузка существующей heightmap;
 *- 4) Разбить terrain на чанки;
 *- 10) UI для задания параметров карты;
 *- 9) Доработать алгоритм генерации;
 *- 6) Shadow map;
 *- 8) terrain patterns;
 *- 7) correct blending, texture splatting;
 * */
namespace Generating
{
    class Program
    {
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}