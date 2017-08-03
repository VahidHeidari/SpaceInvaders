// please don't delete my name
//+++++++++++++++++++++++++++++++++++++++
//+                                     +
//+                                     +
//+    G A M E    N A M E S P A C E     +
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

namespace GamesObjects
{
    public abstract class GameObject
    {
        protected int width;
        protected int height;
        protected int x;
        protected int y;
        protected int xPosition;
        protected int yPosition;
        protected Bitmap image;

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }
        public int X
        {
            set { xPosition = value; }
            get { return xPosition; }
        }
        public int Y
        {
            set { yPosition = value; }
            get { return yPosition; }
        }

        /// <summary>
        /// Create an Game Object
        /// </summary>
        /// <param name="Image">Source image</param>
        /// <param name="srcX">X posision in source image</param>
        /// <param name="srcY">Y position in source image</param>
        /// <param name="Width">Width of Object Model</param>
        /// <param name="Height">Height of Object Model</param>
        protected GameObject(Image Image, int srcX, int srcY, int Width, int Height)
        {
            x = srcX;
            y = srcY;
            width = Width;
            height = Height;
            image = Image as Bitmap;
        }

        /// <summary>
        /// Creates an Game Object
        /// </summary>
        /// <param name="ImagePath">Path of source image</param>
        /// <param name="srcX">X location of first sprite in source image in pixel</param>
        /// <param name="srcY">Y position in source image</param>
        /// <param name="Width">Width of Object Model</param>
        /// <param name="Height">Height of Object Model</param>
        protected GameObject(string ImagePath, int srcX, int srcY, int Width, int Height)
        {
            x = srcX;
            y = srcY;
            width = Width;
            height = Height;
            image = new Bitmap(ImagePath);
        }

        abstract public void Render(int X, int Y, Graphics buffer);
    }

    public class Tile : GameObject
    {
        public Tile(Image Image, int srcX, int srcY, int Width, int Height)
            : base(Image, srcX, srcY, Width, Height) { }

        public Tile(string ImagePath, int srcX, int srcY, int Widht, int Height)
            : base(ImagePath, srcX, srcY, Widht, Height) { }

        override public void Render(int X, int Y, Graphics buffer)
        {
            buffer.DrawImage(image, new Rectangle(X, Y, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
        }

        public void Render(int X, int Y, int numberOfTiles, Graphics buffer)
        {
            for (int i = 0; i < numberOfTiles; ++i)
                Render(X + i * width, Y, buffer);
        }

        /// <summary>
        /// Render at tile's position
        /// </summary>
        /// <param name="buffer">Which buffer you want to draw tile</param>
        public void Render(Graphics buffer)
        {
            Render(this.xPosition, this.yPosition, buffer);
        }

        /// <summary>
        /// Fill tile
        /// </summary>
        /// <param name="X">Starting x position</param>
        /// <param name="Y">Starting y position</param>
        /// <param name="Width">Number of tiles in X axis</param>
        /// <param name="Height">Number of tiles in y axis</param>
        /// <param name="buffer">Which buffer</param>
        public void Fill(int X, int Y, int Width, int Height, Graphics buffer)
        {
            for (int j = 0; j < Height; ++j)
                for (int i = 0; i < Width; ++i)
                    Render(X + i * width, Y + j * height, buffer);
        }
    }

    public class Sprite : GameObject
    {
        int frameCount;
        int frameDirection = -2;
        int currentFrame;
        int delayCounter;
        int curDelay;
        public int Frame
        {
            get { return currentFrame; }
        }
        public int Delay
        {
            set { delayCounter = curDelay = value; }
            get { return curDelay; }
        }

        /// <summary>
        /// Create Sprite
        /// transparent color is top left pixel
        /// </summary>
        /// <param name="Image">source image used for sprite</param>
        /// <param name="srcX">X location of first sprite in source image in pixel</param>
        /// <param name="srcY">Y location of firset sprite in source image in pixel</param>
        /// <param name="Width">Width of one Sprite in pixel</param>
        /// <param name="Height">Height of one sprite in pixel</param>
        /// <param name="FrameDirection">Sprite animation direction 1:up -1:down 2:right -2:left any othernumbers and defult:-2</param>
        /// <param name="FrameCount"></param>
        public Sprite(Image Image, int srcX, int srcY, int Width, int Height, int FrameDirection, int FrameCount)
            : base(Image, srcX, srcY, Width, Height)
        {
            frameCount = FrameCount;
            if (FrameDirection > -3 && FrameDirection < 3 && FrameDirection != 0)
                frameDirection = FrameDirection;
            currentFrame = 0;
            image.MakeTransparent(image.GetPixel(0, 0));
        }

        /// <summary>
        /// Create Sprite
        /// transparent color is top left pixel
        /// </summary>
        /// <param name="ImagePath">Path of source image Note:This constructor creates an instance not refrence</param>
        /// <param name="srcX">X location of first sprite in source image in pixel</param>
        /// <param name="srcY">Y location of firset sprite in source image in pixel</param>
        /// <param name="Width">Width of one Sprite in pixel</param>
        /// <param name="Height">Height of one sprite in pixel</param>
        /// <param name="FrameDirection">Sprite animation direction 1:up -1:down 2:right -2:left othernumbers and defult:-2</param>
        /// <param name="FrameCount">Number of Frames</param>
        public Sprite(string ImagePath, int srcX, int srcY, int Width, int Height, int FrameDirection, int FrameCount)
            : base(ImagePath, srcX, srcY, Width, Height)
        {
            frameCount = FrameCount;
            if (FrameDirection > -3 && FrameDirection < 3 && FrameDirection != 0)
                frameDirection = FrameDirection;
            image = new Bitmap(ImagePath);
            currentFrame = 0;
            image.MakeTransparent(image.GetPixel(x, y));
        }

        /// <summary>
        /// Next Frame of sprite's animation
        /// </summary>
        public virtual void NextFrame()
        {
            if (delayCounter <= 0)
            {
                currentFrame++;
                currentFrame %= frameCount;
                delayCounter = curDelay;
            }
            else --delayCounter;
        }
        public void SetFrame(int Frame)
        {
            currentFrame = Frame;
            currentFrame %= frameCount;
        }

        /// <summary>
        /// Previous frame of sprite's animation
        /// </summary>
        public virtual void PreviousFrame()
        {
            currentFrame--;
            currentFrame %= frameCount;
        }

        /// <summary>
        /// Render sprite at position
        /// </summary>
        /// <param name="X">X position</param>
        /// <param name="Y">Y position</param>
        /// <param name="buffer">which buffer you want to draw sprite</param>
        override public void Render(int X, int Y, Graphics buffer)
        {
            int imageX = x, imageY = y;
            switch (frameDirection)
            {
                // up
                case 1:
                    imageY -= height * currentFrame;
                    break;

                // down
                case -1:
                    imageY += height * currentFrame;
                    break;

                // right
                case 2:
                    imageX += width * currentFrame;
                    break;

                // left
                case -2:
                    imageX -= width * currentFrame;
                    break;
            }
            // drawing corect frame
            buffer.DrawImage(image, new Rectangle(X, Y, width, height), new Rectangle(imageX, imageY, width, height), GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Draw some sprites
        /// </summary>
        /// <param name="X">starting x position</param>
        /// <param name="Y">Starting y position</param>
        /// <param name="NumberOfSprite">Number of sprites</param>
        /// <param name="Offset">Offset of next sprite</param>
        /// <param name="buffer">Which Buffer</param>
        public virtual void Render(int X, int Y, int NumberOfSprite, int Offset, Graphics buffer)
        {
            for (int i = 0; i < NumberOfSprite; ++i)
                Render(X + i * (width + Offset), Y, buffer);
        }

        /// <summary>
        /// Render at sprite's position
        /// </summary>
        /// <param name="buffer">Which buffer you want to draw sprite</param>
        public void Render(Graphics buffer)
        {
            Render(this.xPosition, this.yPosition, buffer);
        }

        /// <summary>
        /// Test Collision
        /// </summary>
        /// <param name="rect">collision rectangel</param>
        /// <returns>if collide return true</returns>
        public bool Collision(Rectangle rect)
        {
            Rectangle This = new Rectangle(this.xPosition, this.yPosition, this.width, this.height);
            Rectangle That = rect;
            return This.IntersectsWith(That);
        }

        /// <summary>
        /// Test collision
        /// </summary>
        /// <param name="X">X location</param>
        /// <param name="Y">Y location</param>
        /// <param name="Width">Width</param>
        /// <param name="Height">Height</param>
        /// <returns>if collide return true</returns>
        public bool Collision(int _X, int _Y, int _Width, int _Height)
        {
            return Collision(new Rectangle(_X, _Y, _Width, _Height));
        }

        /// <summary>
        /// Test Collision between two sprites
        /// </summary>
        /// <param name="sprite">Sprite</param>
        /// <returns>if collide returns true</returns>
        public bool Collision(Sprite sprite)
        {
            return Collision(new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Height));
        }
    }
}// end of namespace