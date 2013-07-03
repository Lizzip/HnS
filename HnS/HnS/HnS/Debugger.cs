using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HnS
{
    public class Debugger
    {
        int maxLines, currentLine, yIncrement;
        string[] outputLines;
        SpriteFont smallFont;
        Vector2 basePosition, stringSize;

        public Debugger(ContentManager contentManager, int lines = 8)
        {
            maxLines = lines;
            currentLine = 0;
            outputLines = new string[maxLines];
            basePosition = new Vector2(790,50);
            yIncrement = 20;
            smallFont = contentManager.Load<SpriteFont>("smallFont");
        }

        void swapItems(int pos1, int pos2)
        {
            string temp1, temp2;

            temp1 = outputLines[pos1];
            temp2 = outputLines[pos2];

            outputLines[pos1] = temp2;
            outputLines[pos2] = temp1;
        }

        void shiftLines()
        {
            for (int i = 1; i < maxLines; i++)
            {
                swapItems(i-1, i);
            }
        }

        public void Out(object info)
        {
            if (currentLine < maxLines)
            {
                outputLines[currentLine] = info.ToString();
                currentLine++;
            }
            else
            {
                shiftLines();
                outputLines[maxLines-1] = info.ToString();
            }
        }

        public void Out(object info, object info2)
        {
            if (currentLine < maxLines)
            {
                outputLines[currentLine] = info.ToString() + " " + info2.ToString();
                currentLine++;
            }
            else
            {
                shiftLines();
                outputLines[maxLines - 1] = info.ToString() + " " + info2.ToString();
            }
        }

        public void Output(SpriteBatch theSpriteBatch)
        {
            for (int i = 0; i < currentLine; i++)
            {
                stringSize = smallFont.MeasureString(outputLines[i]);
                theSpriteBatch.DrawString(smallFont, outputLines[i], new Vector2(basePosition.X - stringSize.X + 1, basePosition.Y + (i * yIncrement) + 1), Color.Black);
                theSpriteBatch.DrawString(smallFont, outputLines[i], new Vector2(basePosition.X - stringSize.X, basePosition.Y + (i* yIncrement)), Color.White);
            }
        }

    }
}
