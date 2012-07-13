using System;
using System.Management.Automation.Host;

namespace NuDeploy.Core.Services.Installation.PowerShell
{
    public class NuDeployRawPowerShellUserInterface : PSHostRawUserInterface
    {
        public override ConsoleColor BackgroundColor
        {
            get
            {
                return System.Console.BackgroundColor;
            }

            set
            {
                System.Console.BackgroundColor = value;
            }
        }

        public override Size BufferSize
        {
            get
            {
                return new Size(100, 100);
            }

            set
            {
                System.Console.SetBufferSize(value.Width, value.Height);
            }
        }

        public override Coordinates CursorPosition
        {
            get
            {
                return new Coordinates(System.Console.CursorLeft, System.Console.CursorTop);
            }

            set
            {
                System.Console.CursorTop = value.Y;
                System.Console.CursorLeft = value.X;
            }
        }

        public override int CursorSize
        {
            get
            {
                return System.Console.CursorSize;
            }

            set
            {
                System.Console.CursorSize = value;
            }
        }

        public override ConsoleColor ForegroundColor
        {
            get
            {
                return System.Console.ForegroundColor;
            }

            set
            {
                System.Console.ForegroundColor = value;
            }
        }

        public override bool KeyAvailable
        {
            get
            {
                return System.Console.KeyAvailable;
            }
        }

        public override Size MaxPhysicalWindowSize
        {
            get
            {
                return new Size(System.Console.LargestWindowWidth, System.Console.LargestWindowHeight);
            }
        }

        public override Size MaxWindowSize
        {
            get
            {
                return new Size(System.Console.LargestWindowWidth, System.Console.LargestWindowHeight);
            }
        }

        public override Coordinates WindowPosition
        {
            get
            {
                return new Coordinates(System.Console.WindowLeft, System.Console.WindowTop);
            }

            set
            {
                System.Console.SetWindowPosition(value.X, value.Y);
            }
        }

        public override Size WindowSize
        {
            get
            {
                return new Size(System.Console.WindowWidth, System.Console.WindowHeight);
            }

            set
            {
                System.Console.SetWindowSize(value.Width, value.Height);
            }
        }

        public override string WindowTitle
        {
            get
            {
                return System.Console.Title;
            }

            set
            {
                System.Console.Title = value;
            }
        }

        public override void FlushInputBuffer()
        {
        }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }
}
