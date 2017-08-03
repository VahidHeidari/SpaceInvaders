// please don't delete my name
//+++++++++++++++++++++++++++++++++++++++
//+                                     +
//+                                     +
//+     S P A C E    I N V A D E R S    +
//+                                     +
//+          by vahid heydari           +
//+                                     +
//+           september 2011            +
//+     http://tjs87.wordpress.com      +
//+                                     +
//+                                     +
//+++++++++++++++++++++++++++++++++++++++
using System;
using System.Drawing;
using System.Windows.Forms;

// includeing Game namespace
using GamesObjects;

namespace SpaceInvaders_1
{
    public partial class Form1 : Form
    {
        SpaceInvaders_1.SpaceInvaders game = new SpaceInvaders();
        public Form1()
        {
            // Initialize Form Components !!! Important !!!
            InitializeComponent();
            Cursor = new System.Windows.Forms.Cursor(Application.StartupPath + @"\resources\Cursor1.cur");
            // Initialize Game Components and Objects !!! Important !!!
            game.init_game(this);
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            // Do Nothing ! ! !
            // paint client game form manually in DrawScene()
            //base.OnPaint(e);
        }
        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
        {
            // Do Nothing ! ! !
            // paint game form manually in DrawScene()
            //base.OnPaintBackground(e);
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
        }
    }
}